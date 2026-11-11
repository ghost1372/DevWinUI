using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.Win32;
using DispatcherQueuePriority = Microsoft.UI.Dispatching.DispatcherQueuePriority;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

public sealed record MagicKeyEventArgs(string Code, string Label, bool IsDown, MagicKeyboardKeyEventSource Source, MagicKeyboardKeyModifiers Modifiers = MagicKeyboardKeyModifiers.None);

internal sealed record KeyDef(
    string Code, string Top, string Bottom, MagicKeyboardKeyStyle Style, MagicKeyboardKeyAlign Align,
    double X, double Y, double W, double H, (double X, double Y)[]? Poly = null)
{
    public string Display => Style is MagicKeyboardKeyStyle.Modifier or MagicKeyboardKeyStyle.Function ? $"{Top} {Bottom}".Trim() : Bottom;
}

[TemplatePart(Name = PartBoard, Type = typeof(Grid))]
[TemplatePart(Name = PartChassis, Type = typeof(Border))]
[TemplatePart(Name = PartCanvas, Type = typeof(Canvas))]
[TemplatePart(Name = PartPreview, Type = typeof(Border))]
[TemplatePart(Name = PartPreviewText, Type = typeof(TextBlock))]
public sealed partial class MagicKeyboard : Control
{
    private const string PartBoard = "PART_Board";
    private const string PartChassis = "PART_Chassis";
    private const string PartCanvas = "PART_Canvas";
    private const string PartPreview = "PART_Preview";
    private const string PartPreviewText = "PART_PreviewText";

    private MagicKeyboardSkin EffectiveSkin()
    {
        if (Skin != MagicKeyboardSkin.System) return Skin;
        if (IsHighContrast) return MagicKeyboardSkin.Custom;
        return ActualTheme == ElementTheme.Dark ? DarkSkin : LightSkin;
    }

    private void QueueRebuild()
    {
        if (_rebuildQueued) return;

        _rebuildQueued = true;
        if (!DispatcherQueue.TryEnqueue(() => { _rebuildQueued = false; Rebuild(); }))
        {
            _rebuildQueued = false;
            Rebuild();
        }
    }

    private static LinearGradientBrush VGrad(Color a, Color b) => new()
    {
        StartPoint = new Point(0, 0),
        EndPoint = new Point(0, 1),
        GradientStops =
        {
            new GradientStop { Color = a, Offset = 0 },
            new GradientStop { Color = b, Offset = 1 }
        }
    };

    private readonly Dictionary<string, MagicKeyboardKeyVisual> _keys = new();
    private readonly Dictionary<uint, string> _ptr = new();
    private readonly HashSet<string> _physical = new();
    private readonly DispatcherTimer _previewTimer = new() { Interval = TimeSpan.FromMilliseconds(700) };
    private readonly DispatcherTimer _pollTimer = new() { Interval = TimeSpan.FromMilliseconds(400) };
    private readonly KeyEventHandler _onDown, _onUp;
    private readonly TypedEventHandler<XamlRoot, XamlRootChangedEventArgs> _onRootChanged;
    private readonly Dictionary<string, int> _physicalVk = new();
    private XamlRoot? _xamlRoot;

    private Grid? _board;
    private Border? _chassis;
    private Canvas? _canvas;
    private Border? _preview;
    private TextBlock? _previewText;

    private AccessibilitySettings? _access;

    private bool IsHighContrast
    {
        get
        {
            try { return (_access ??= new AccessibilitySettings()).HighContrast; }
            catch { return false; }
        }
    }
    private MagicKeyboardColors _t = MagicKeyboardColors.Fallback;
    private Brush _glowFallback = new SolidColorBrush(MagicKeyboardColors.Fallback.LedActive);
    private bool _rebuildQueued;
    private UIElement? _hooked;
    private bool _caps, _num, _inView = true;
    private readonly HashSet<string> _latched = new();

    private IntPtr _hkl;
    private Dictionary<string, (string Base, string Shift)>? _labels;

    private MediaPlayer[] _downPool = Array.Empty<MediaPlayer>(), _upPool = Array.Empty<MediaPlayer>();
    private int _downIdx, _upIdx;

    public MagicKeyboard()
    {
        DefaultStyleKey = typeof(MagicKeyboard);

        _onDown = (_, e) => OnKey(e, true);
        _onUp = (_, e) => OnKey(e, false);
        _onRootChanged = (_, _) => UpdateListeners();

        _previewTimer.Tick += (_, _) =>
        {
            _previewTimer.Stop();
            if (_preview is not null) _preview.Opacity = 0;
        };
        _pollTimer.Tick += (_, _) =>
        {
            CheckLayout();
            ReleaseStaleKeys();
        };

        AutomationProperties.SetName(this, "Keyboard");

        Loaded += (_, _) =>
        {
            if (UsesThemeResources) Rebuild();
            _xamlRoot = XamlRoot;
            if (_xamlRoot is not null) _xamlRoot.Changed += _onRootChanged;
            EnsureSounds();
            UpdateListeners();
            CheckLayout();
            _pollTimer.Start();
        };
        Unloaded += (_, _) =>
        {
            _pollTimer.Stop();
            _previewTimer.Stop();
            if (_xamlRoot is not null) _xamlRoot.Changed -= _onRootChanged;
            _xamlRoot = null;
            ReleaseLatched();
            UpdateListeners(forceOff: true);
            DisposeSounds();
        };

        ActualThemeChanged += (_, _) =>
        {
            if (UsesThemeResources) QueueRebuild();
        };

        EffectiveViewportChanged += (_, e) =>
        {
            bool visible = e.BringIntoViewDistanceX <= 0 && e.BringIntoViewDistanceY <= 0;
            if (visible != _inView) { _inView = visible; UpdateListeners(); }
        };
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _board = GetTemplateChild(PartBoard) as Grid;
        _chassis = GetTemplateChild(PartChassis) as Border;
        _canvas = GetTemplateChild(PartCanvas) as Canvas;
        _preview = GetTemplateChild(PartPreview) as Border;
        _previewText = GetTemplateChild(PartPreviewText) as TextBlock;

        UpdatePreviewVisibility();
        ReloadLabels();
        Rebuild();
    }

    private void ReloadLabels()
    {
        _hkl = SystemKeyLabels.CurrentLayout();
        _labels = FollowSystemKeyboardLayout ? SystemKeyLabels.Read(_hkl) : null;
    }

    private void OnLanguageModeChanged()
    {
        ReloadLabels();
        Rebuild();
    }

    private void CheckLayout()
    {
        if (!FollowSystemKeyboardLayout) return;
        if (SystemKeyLabels.CurrentLayout() == _hkl) return;
        ReloadLabels();
        Rebuild();
    }

    private List<KeyDef> ApplyLabels(List<KeyDef> defs)
    {
        if (_labels is null) return defs;

        for (int i = 0; i < defs.Count; i++)
        {
            var d = defs[i];
            if (d.Style is not (MagicKeyboardKeyStyle.Letter or MagicKeyboardKeyStyle.Dual)) continue;
            if (!_labels.TryGetValue(d.Code, out var l)) continue;

            string b = l.Base, s = l.Shift;
            if (b.Length == 0 && s.Length == 0) continue;

            if (b.Length == 0)
                defs[i] = d with { Style = MagicKeyboardKeyStyle.Letter, Top = "", Bottom = s };
            else if (s.Length == 0 || s == b || s == b.ToUpperInvariant())
                defs[i] = d with { Style = MagicKeyboardKeyStyle.Letter, Top = "", Bottom = b };
            else
                defs[i] = d with { Style = MagicKeyboardKeyStyle.Dual, Top = s, Bottom = b };
        }
        return defs;
    }

    private void Rebuild()
    {
        if (_canvas is null || _chassis is null) return;

        if (_board is not null)
        {
            _board.Width = MagicKeyboardLayoutHelper.Width(FormFactor);
            _board.Height = MagicKeyboardLayoutHelper.Height;
        }

        _keys.Clear();
        _canvas.Children.Clear();
        _t = LoadColors(EffectiveSkin());
        _glowFallback = new SolidColorBrush(_t.LedActive);
        _chassis.Background = VGrad(_t.ChassisTop, _t.ChassisBottom);
        _chassis.BorderBrush = new SolidColorBrush(_t.ChassisStroke);

        foreach (var d in ApplyLabels(MagicKeyboardLayoutHelper.Build(Layout, FormFactor, Platform)))
        {
            var kv = BuildKey(d);
            _keys[d.Code] = kv;
            Canvas.SetLeft(kv.Root, d.X);
            Canvas.SetTop(kv.Root, d.Y);
            _canvas.Children.Add(kv.Root);
        }
        foreach (var code in _keys.Keys.ToArray()) Refresh(code);
        UpdateLed();
    }

    private const double Gap = 3, Radius = 4;

    private static ArcSegment Arc(double x, double y, double r) => new()
    {
        Point = new Point(x, y),
        Size = new Size(r, r),
        SweepDirection = SweepDirection.Clockwise,
        IsLargeArc = false
    };

    private static PathGeometry RoundedRect(double x, double y, double w, double h, double r)
    {
        var fig = new PathFigure { StartPoint = new Point(x + r, y), IsClosed = true, IsFilled = true };
        fig.Segments.Add(new LineSegment { Point = new Point(x + w - r, y) });
        fig.Segments.Add(Arc(x + w, y + r, r));
        fig.Segments.Add(new LineSegment { Point = new Point(x + w, y + h - r) });
        fig.Segments.Add(Arc(x + w - r, y + h, r));
        fig.Segments.Add(new LineSegment { Point = new Point(x + r, y + h) });
        fig.Segments.Add(Arc(x, y + h - r, r));
        fig.Segments.Add(new LineSegment { Point = new Point(x, y + r) });
        fig.Segments.Add(Arc(x + r, y, r));
        return new PathGeometry { Figures = { fig } };
    }

    private static Path MakeShape(KeyDef d, Color top, Color bottom)
    {
        Brush fill = top == bottom ? (Brush)new SolidColorBrush(top) : VGrad(top, bottom);

        if (d.Poly is null)
        {
            double r = d.H < 40 ? 6 : 9;
            return new Path
            {
                Fill = fill,
                Data = RoundedRect(Gap, Gap, d.W - 2 * Gap, d.H - 2 * Gap, r)
            };
        }

        var fig = new PathFigure { StartPoint = new Point(d.Poly[0].X, d.Poly[0].Y), IsClosed = true, IsFilled = true };
        foreach (var p in d.Poly.Skip(1)) fig.Segments.Add(new LineSegment { Point = new Point(p.X, p.Y) });
        return new Path
        {
            Fill = fill,
            Stroke = fill,
            StrokeThickness = 2 * Radius,
            StrokeLineJoin = PenLineJoin.Round,
            Data = new PathGeometry { Figures = { fig } }
        };
    }

    private static Color Mix(Color a, Color b, double t) => Color.FromArgb(255,
        (byte)(a.R + (b.R - a.R) * t),
        (byte)(a.G + (b.G - a.G) * t),
        (byte)(a.B + (b.B - a.B) * t));

    private (Color Top, Color Bottom) FaceFor(KeyDef d) =>
        d.Style is MagicKeyboardKeyStyle.Modifier or MagicKeyboardKeyStyle.Function or MagicKeyboardKeyStyle.Arrow
            ? (Mix(_t.FaceTop, _t.Edge, 0.12), Mix(_t.FaceBottom, _t.Edge, 0.12))
            : (_t.FaceTop, _t.FaceBottom);

    private Path? MakeRim(KeyDef d)
    {
        if (d.Poly is not null) return null;
        bool darkFace = _t.FaceTop.R < 0x80;
        var top = Color.FromArgb(darkFace ? (byte)0x30 : (byte)0xE0, 255, 255, 255);
        var bottom = Color.FromArgb(0, 255, 255, 255);
        double r = d.H < 40 ? 6 : 9;
        return new Path
        {
            Data = RoundedRect(Gap + 0.5, Gap + 0.5, d.W - 2 * Gap - 1, d.H - 2 * Gap - 1, r),
            Stroke = VGrad(top, bottom),
            StrokeThickness = 1,
            IsHitTestVisible = false
        };
    }

    private static void SetFill(Path p, Color top, Color bottom)
    {
        Brush b = VGrad(top, bottom);
        p.Fill = b;
        if (p.Stroke is not null) p.Stroke = b;
    }

    private MagicKeyboardKeyVisual BuildKey(KeyDef d)
    {
        var edge = MakeShape(d, _t.Edge, _t.Edge);
        edge.RenderTransform = new TranslateTransform { Y = 2.5 };
        var (faceTop, faceBottom) = FaceFor(d);
        var face = MakeShape(d, faceTop, faceBottom);

        var dip = new TranslateTransform();
        var cap = new Grid { RenderTransform = dip };
        var halo = MakeGlow(d, 6, 0.4);
        cap.Children.Add(halo);
        cap.Children.Add(face);
        if (MakeRim(d) is { } rim) cap.Children.Add(rim);
        var ring = d.Poly is null ? MakeGlow(d, 1.5, 1) : null;
        if (ring is not null) cap.Children.Add(ring);
        var legend = MakeLegend(d, out var led, out var letterText, out var dualStack);
        legend.IsHitTestVisible = false;
        cap.Children.Add(legend);

        var root = new Grid { Width = d.W, Height = d.H };
        AutomationProperties.SetName(root, d.Display);
        root.Children.Add(edge);
        root.Children.Add(cap);

        var kv = new MagicKeyboardKeyVisual { Def = d, Root = root, Face = face, Halo = halo, Ring = ring, Dip = dip, Led = led, Letter = letterText, Dual = dualStack, FaceTop = faceTop, FaceBottom = faceBottom };
        string code = d.Code;

        root.PointerPressed += (s, e) =>
        {
            var pt = e.GetCurrentPoint(root);
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse && !pt.Properties.IsLeftButtonPressed) return;
            if (IsLatchable(code))
            {
                ToggleLatch(code);
                e.Handled = true;
                return;
            }
            root.CapturePointer(e.Pointer);
            _ptr[e.Pointer.PointerId] = code;
            if (code == "CapsLock") OnScreenCapsPressed();
            else if (code == "NumLock") SetNum(!_num);
            RefreshKey(code);
            Raise(code, true, MagicKeyboardKeyEventSource.Pointer);
            e.Handled = true;
        };
        root.PointerReleased += (s, e) => EndPointer(e.Pointer, root);
        root.PointerCanceled += (s, e) => EndPointer(e.Pointer, null);
        root.PointerCaptureLost += (s, e) => EndPointer(e.Pointer, null);
        return kv;
    }

    private void EndPointer(Pointer p, UIElement? el)
    {
        if (!_ptr.Remove(p.PointerId, out var code)) return;
        el?.ReleasePointerCapture(p);
        RefreshKey(code);
        Raise(code, false, MagicKeyboardKeyEventSource.Pointer);
        ReleaseStickyAfter(code);
    }

    private static TextBlock TB(string text, double size, Color color, Windows.UI.Text.FontWeight? weight = null) => new()
    {
        Text = text,
        FontSize = size,
        Foreground = new SolidColorBrush(color),
        FontFamily = new FontFamily("Segoe UI, Segoe UI Symbol"),
        FontWeight = weight ?? FontWeights.Normal,
        IsHitTestVisible = false
    };

    private FrameworkElement MakeLegend(KeyDef d, out Ellipse? led, out TextBlock? letter, out StackPanel? dual)
    {
        led = null;
        letter = null;
        dual = null;
        var ha = d.Align switch
        {
            MagicKeyboardKeyAlign.Left => HorizontalAlignment.Left,
            MagicKeyboardKeyAlign.Right => HorizontalAlignment.Right,
            _ => HorizontalAlignment.Center
        };

        switch (d.Style)
        {
            case MagicKeyboardKeyStyle.Letter:
                letter = TB(ApplyCase(d.Bottom), 19, _t.Legend, FontWeights.Medium);
                letter.HorizontalAlignment = HorizontalAlignment.Center;
                letter.VerticalAlignment = VerticalAlignment.Center;
                return letter;

            case MagicKeyboardKeyStyle.Dual:
                dual = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                dual.Children.Add(TB(d.Top, 11, _t.Dim));
                dual.Children.Add(TB(d.Bottom, 14, _t.Legend));
                StyleDual(dual);
                return dual;

            case MagicKeyboardKeyStyle.Function:
                if (d.Top.Length > 0)
                {
                    var fnPair = new StackPanel { HorizontalAlignment = ha, VerticalAlignment = VerticalAlignment.Center };
                    var fnTop = TB(d.Top, 9, _t.Dim);
                    var fnBottom = TB(d.Bottom, 9, _t.Dim);
                    fnTop.HorizontalAlignment = ha;
                    fnBottom.HorizontalAlignment = ha;
                    fnPair.Children.Add(fnTop);
                    fnPair.Children.Add(fnBottom);
                    return fnPair;
                }
                var fn = TB(d.Bottom, d.Bottom.Length == 1 ? 13 : 10, _t.Dim);
                fn.HorizontalAlignment = ha;
                fn.VerticalAlignment = VerticalAlignment.Center;
                fn.Margin = new Thickness(12, 0, 12, 0);
                return fn;

            case MagicKeyboardKeyStyle.Arrow:
                var arrow = TB(d.Bottom, 10, _t.Dim);
                arrow.HorizontalAlignment = HorizontalAlignment.Center;
                arrow.VerticalAlignment = VerticalAlignment.Center;
                return arrow;

            case MagicKeyboardKeyStyle.Numpad:
                var numGrid = new Grid();
                bool hasHint = d.Top.Length > 0;
                var numMain = TB(d.Bottom, 17, _t.Legend, FontWeights.Medium);
                numMain.HorizontalAlignment = HorizontalAlignment.Center;
                numMain.VerticalAlignment = hasHint ? VerticalAlignment.Bottom : VerticalAlignment.Center;
                numMain.Margin = hasHint ? new Thickness(0, 0, 0, 6) : new Thickness(0);
                numGrid.Children.Add(numMain);
                if (hasHint)
                {
                    var numHint = TB(d.Top, 9, _t.Dim);
                    numHint.HorizontalAlignment = HorizontalAlignment.Center;
                    numHint.VerticalAlignment = VerticalAlignment.Top;
                    numHint.Margin = new Thickness(0, 7, 0, 0);
                    numGrid.Children.Add(numHint);
                }
                return numGrid;

            case MagicKeyboardKeyStyle.Modifier:
                var grid = new Grid { Margin = new Thickness(9, 0, 9, 7) };
                var col = new StackPanel { HorizontalAlignment = ha, VerticalAlignment = VerticalAlignment.Bottom };
                if (d.Top.Length > 0)
                {
                    var sym = TB(d.Top, d.Top.Length > 1 ? 10 : 12, _t.Legend);
                    sym.HorizontalAlignment = ha;
                    col.Children.Add(sym);
                }
                var modLabel = TB(d.Bottom, 10, _t.Legend);
                modLabel.HorizontalAlignment = ha;
                col.Children.Add(modLabel);
                grid.Children.Add(col);
                if (d.Code == "CapsLock" || (d.Code == "NumLock" && Platform == MagicKeyboardPlatform.Windows))
                {
                    led = new Ellipse
                    {
                        Width = 6,
                        Height = 6,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(1, 3, 0, 0)
                    };
                    grid.Children.Add(led);
                }
                return grid;

            default:
                return new Grid();
        }
    }

    private void Refresh(string code)
    {
        if (!_keys.TryGetValue(code, out var kv)) return;
        bool pressed = IsPressed(code);
        kv.Dip.Y = pressed ? 1.8 : 0;
        if (pressed) SetFill(kv.Face, _t.PressTop, _t.PressBottom);
        else SetFill(kv.Face, kv.FaceTop, kv.FaceBottom);

        var glow = pressed && IsGlowEnabled ? Visibility.Visible : Visibility.Collapsed;
        kv.Halo.Visibility = glow;
        if (kv.Ring is not null) kv.Ring.Visibility = glow;
        if (glow == Visibility.Visible)
        {
            var brush = GlowBrush ?? _glowFallback;
            kv.Halo.Stroke = brush;
            if (kv.Ring is not null) kv.Ring.Stroke = brush;
        }
    }

    private void RefreshAllKeys()
    {
        foreach (var code in _keys.Keys.ToArray()) Refresh(code);
    }

    private static Path MakeGlow(KeyDef d, double thickness, double opacity)
    {
        PathGeometry geometry;
        double extra = 0;

        if (d.Poly is null)
        {
            double r = d.H < 40 ? 6 : 9;
            geometry = RoundedRect(Gap, Gap, d.W - 2 * Gap, d.H - 2 * Gap, r);
        }
        else
        {
            var fig = new PathFigure { StartPoint = new Point(d.Poly[0].X, d.Poly[0].Y), IsClosed = true, IsFilled = true };
            foreach (var p in d.Poly.Skip(1)) fig.Segments.Add(new LineSegment { Point = new Point(p.X, p.Y) });
            geometry = new PathGeometry { Figures = { fig } };
            extra = 2 * Radius;
        }

        return new Path
        {
            Data = geometry,
            StrokeThickness = thickness + extra,
            StrokeLineJoin = PenLineJoin.Round,
            Opacity = opacity,
            IsHitTestVisible = false,
            Visibility = Visibility.Collapsed
        };
    }

    private static readonly string[] ModifierCodes =
        { "ShiftLeft", "ShiftRight", "ControlLeft", "ControlRight", "AltLeft", "AltRight", "MetaLeft", "MetaRight" };

    private static bool IsModifierCode(string code) => Array.IndexOf(ModifierCodes, code) >= 0;

    private bool IsLatchable(string code) => ModifierMode != MagicKeyboardKeyModifierMode.Hold && IsModifierCode(code);

    private static string? PartnerOf(string code) => code switch
    {
        "ShiftLeft" => "ShiftRight",
        "ShiftRight" => "ShiftLeft",
        "ControlLeft" => "ControlRight",
        "ControlRight" => "ControlLeft",
        "AltLeft" => "AltRight",
        "AltRight" => "AltLeft",
        _ => null
    };

    private bool IsDown(string code) =>
        _ptr.ContainsValue(code) || _physical.Contains(code) || _latched.Contains(code);

    private bool IsPressed(string code)
    {
        if (IsDown(code)) return true;
        return LinkModifierKeys && PartnerOf(code) is { } partner && IsDown(partner);
    }

    private bool IsShiftDown => IsDown("ShiftLeft") || IsDown("ShiftRight");

    private MagicKeyboardKeyModifiers CurrentModifiers()
    {
        var m = MagicKeyboardKeyModifiers.None;
        if (IsShiftDown) m |= MagicKeyboardKeyModifiers.Shift;
        if (IsDown("ControlLeft") || IsDown("ControlRight")) m |= MagicKeyboardKeyModifiers.Control;
        if (IsDown("AltLeft") || IsDown("AltRight")) m |= MagicKeyboardKeyModifiers.Alt;
        if (IsDown("MetaLeft") || IsDown("MetaRight")) m |= MagicKeyboardKeyModifiers.Windows;
        return m;
    }

    private void RefreshKey(string code)
    {
        Refresh(code);
        if (PartnerOf(code) is { } partner) Refresh(partner);
        if (code is "ShiftLeft" or "ShiftRight") UpdateCase();
    }

    private void RefreshModifierPairs()
    {
        foreach (var code in ModifierCodes) Refresh(code);
    }

    private void ToggleLatch(string code)
    {
        if (LinkModifierKeys && PartnerOf(code) is { } partner && _latched.Contains(partner)) code = partner;

        bool down = _latched.Add(code);
        if (!down) _latched.Remove(code);
        RefreshKey(code);
        Raise(code, down, MagicKeyboardKeyEventSource.Pointer);
    }

    private void ReleaseLatched()
    {
        foreach (var code in _latched.ToArray())
        {
            _latched.Remove(code);
            RefreshKey(code);
            Raise(code, false, MagicKeyboardKeyEventSource.Pointer);
        }
    }

    private void ReleaseStickyAfter(string code)
    {
        if (ModifierMode == MagicKeyboardKeyModifierMode.Sticky && !IsModifierCode(code) && code != "Fn") ReleaseLatched();
    }

    private void StyleDual(StackPanel dual)
    {
        bool shift = IsShiftDown;
        SetActive((TextBlock)dual.Children[0], shift);
        SetActive((TextBlock)dual.Children[1], !shift);
    }

    private void SetActive(TextBlock text, bool active)
    {
        text.Foreground = new SolidColorBrush(active ? _t.Legend : _t.Dim);
        text.FontSize = active ? 14 : 11;
        text.FontWeight = active ? FontWeights.Medium : FontWeights.Normal;
    }

    private bool IsUpper => (LetterCase == MagicKeyboardKeyCase.Upper) ^ _caps ^ IsShiftDown;

    private string ApplyCase(string s) => IsUpper ? s.ToUpperInvariant() : s.ToLowerInvariant();

    private string LabelFor(KeyDef d) => d.Style switch
    {
        MagicKeyboardKeyStyle.Letter => ApplyCase(d.Bottom),
        MagicKeyboardKeyStyle.Dual when IsShiftDown && d.Top.Length > 0 => d.Top,
        _ => d.Display
    };

    private void UpdateCase()
    {
        foreach (var kv in _keys.Values)
        {
            if (kv.Letter is { } text) text.Text = ApplyCase(kv.Def.Bottom);
            if (kv.Dual is { } dual) StyleDual(dual);
        }
    }

    private void SetCaps(bool on)
    {
        if (_caps == on) return;
        _caps = on;
        UpdateLed();
        UpdateCase();
    }

    private void UpdateLed()
    {
        SetLed("CapsLock", _caps);
        SetLed("NumLock", _num);
    }

    private void SetLed(string code, bool on)
    {
        if (_keys.TryGetValue(code, out var kv) && kv.Led is { } led)
            led.Fill = new SolidColorBrush(on
                ? _t.LedActive
                : Color.FromArgb(0x55, _t.Dim.R, _t.Dim.G, _t.Dim.B));
    }

    private void SetNum(bool on)
    {
        if (_num == on) return;
        _num = on;
        UpdateLed();
    }

    private void OnScreenCapsPressed() => SetCaps(!_caps);

    private static bool IsLocked(VirtualKey key) =>
        InputKeyboardSource.GetKeyStateForCurrentThread(key).HasFlag(CoreVirtualKeyStates.Locked);

    private void SyncCapsFromSystem()
    {
        if (SyncCapsLock) SetCaps(IsLocked(VirtualKey.CapitalLock));
    }

    private void SyncNumFromSystem()
    {
        if (SyncNumLock) SetNum(IsLocked(VirtualKey.NumberKeyLock));
    }

    private void Raise(string code, bool down, MagicKeyboardKeyEventSource src)
    {
        if (!_keys.TryGetValue(code, out var kv)) return;
        KeyEvent?.Invoke(this, new MagicKeyEventArgs(code, LabelFor(kv.Def), down, src, CurrentModifiers()));
        if (IsSoundEnabled) PlaySound(down);
        if (down && IsPreviewVisible && _preview is not null && _previewText is not null)
        {
            _previewText.Text = LabelFor(kv.Def);
            _preview.Opacity = 1;
            _previewTimer.Stop();
            _previewTimer.Start();
        }
    }

    private void UpdatePreviewVisibility()
    {
        if (_preview is not null)
            _preview.Visibility = IsPreviewVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    private void OnKey(KeyRoutedEventArgs e, bool down)
    {
        var st = e.KeyStatus;
        var code = PhysicalKeyMap.Resolve(e.Key, st.ScanCode, st.IsExtendedKey);
        if (code == "IntlBackslash" && !_keys.ContainsKey(code)) code = "Backslash";
        if (code is null || !_keys.ContainsKey(code)) return;

        bool changed = down ? _physical.Add(code) : _physical.Remove(code);
        if (down) _physicalVk[code] = PollKey(code, e.Key);
        else _physicalVk.Remove(code);

        if (changed)
        {
            RefreshKey(code);
            Raise(code, down, MagicKeyboardKeyEventSource.Physical);
            if (!down) ReleaseStickyAfter(code);
        }

        if (code == "CapsLock" && down && changed)
        {
            if (SyncCapsLock)
            {
                SyncCapsFromSystem();
                DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, SyncCapsFromSystem);
            }
            else
            {
                SetCaps(!_caps);
            }
        }

        if (code == "NumLock" && down && changed)
        {
            if (SyncNumLock)
            {
                SyncNumFromSystem();
                DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, SyncNumFromSystem);
            }
            else
            {
                SetNum(!_num);
            }
        }
    }

    private static int PollKey(string code, VirtualKey key) => code switch
    {
        "ShiftLeft" => 0xA0,
        "ShiftRight" => 0xA1,
        "ControlLeft" => 0xA2,
        "ControlRight" => 0xA3,
        "AltLeft" => 0xA4,
        "AltRight" => 0xA5,
        _ => (int)key
    };

    private void ReleasePhysical(string code)
    {
        if (!_physical.Remove(code)) return;
        _physicalVk.Remove(code);
        RefreshKey(code);
        Raise(code, false, MagicKeyboardKeyEventSource.Physical);
    }

    private void ReleaseStaleKeys()
    {
        if (_physical.Count == 0) return;
        foreach (var code in _physical.ToArray())
            if (_physicalVk.TryGetValue(code, out int vk) && !IsKeyDown(vk))
                ReleasePhysical(code);
    }
    public bool IsKeyDown(int virtualKey) => PInvoke.GetAsyncKeyState(virtualKey) < 0;

    private void UpdateListeners(bool forceOff = false)
    {
        bool want = !forceOff && IsLoaded && IsPhysicalKeyboardEnabled && (IsAlwaysActive || _inView)
                    && XamlRoot is { IsHostVisible: true };

        if (want && _hooked is null && XamlRoot!.Content is UIElement root)
        {
            root.AddHandler(KeyDownEvent, _onDown, true);
            root.AddHandler(KeyUpEvent, _onUp, true);
            _hooked = root;
            SyncCapsFromSystem();
            SyncNumFromSystem();
        }
        else if (!want && _hooked is not null)
        {
            _hooked.RemoveHandler(KeyDownEvent, _onDown);
            _hooked.RemoveHandler(KeyUpEvent, _onUp);
            _hooked = null;

            foreach (var code in _physical.ToArray()) ReleasePhysical(code);
        }
    }

    private static MediaPlayer[] MakePool(string uri)
    {
        if (!Uri.TryCreate(uri, UriKind.Absolute, out var source)) return Array.Empty<MediaPlayer>();
        try
        {
            return Enumerable.Range(0, 6).Select(_ => new MediaPlayer
            {
                AudioCategory = MediaPlayerAudioCategory.SoundEffects,
                CommandManager = { IsEnabled = false },
                Source = MediaSource.CreateFromUri(source),
                AutoPlay = false,
                IsLoopingEnabled = false
            }).ToArray();
        }
        catch { return Array.Empty<MediaPlayer>(); }
    }

    private void EnsureSounds()
    {
        DisposeSounds();
        if (!IsSoundEnabled || !IsLoaded) return;
        _downPool = MakePool(KeyDownSoundUri);
        _upPool = MakePool(KeyUpSoundUri);
    }

    private void DisposeSounds()
    {
        foreach (var p in _downPool.Concat(_upPool)) p.Dispose();
        _downPool = _upPool = Array.Empty<MediaPlayer>();
    }

    private void PlaySound(bool down)
    {
        var pool = down ? _downPool : _upPool;
        if (pool.Length == 0) return;
        var p = pool[(down ? _downIdx++ : _upIdx++) % pool.Length];
        try
        {
            p.Volume = Math.Clamp(SoundVolume, 0, 1);
            p.PlaybackSession.Position = TimeSpan.Zero;
            p.Play();
        }
        catch { }
    }
}
