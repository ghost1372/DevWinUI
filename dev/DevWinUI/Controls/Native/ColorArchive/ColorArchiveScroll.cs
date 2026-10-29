using System.Collections.Specialized;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using Windows.System;

namespace DevWinUI;

/// <summary>
/// Endless project list that repaints the whole stage in the active row's colour,
/// with a draggable preview card, a ruler, tick sound, wheel / touch / keyboard stepping and infinite wrap.
/// </summary>
[TemplatePart(Name = PartRoot, Type = typeof(Grid))]
[TemplatePart(Name = PartRows, Type = typeof(Canvas))]
[TemplatePart(Name = PartRulerHit, Type = typeof(Rectangle))]
[TemplatePart(Name = PartCard, Type = typeof(Border))]
[TemplatePart(Name = PartImageFront, Type = typeof(Image))]
[TemplatePart(Name = PartImageBack, Type = typeof(Image))]
public sealed partial class ColorArchiveScroll : Control
{
    const string PartRoot = "PART_Root";
    const string PartRows = "PART_Rows";
    const string PartRulerHit = "PART_RulerHit";
    const string PartCard = "PART_Card";
    const string PartImageFront = "PART_ImageFront";
    const string PartImageBack = "PART_ImageBack";

    private const double RulerGutter = 84;
    private const double CompactGutter = 28;
    private const double MdBreakpoint = 768;

    private Grid _root = new();
    private Canvas _rowsLayer = new();
    private Rectangle _rulerHit = new();
    private Border _card = new();
    private Image _imgFront = new();
    private Image _imgBack = new();

    private readonly CompositeTransform _cardT = new();
    private readonly SolidColorBrush _bgBrush = new(Microsoft.UI.Colors.White);
    private readonly SolidColorBrush _inkBrush = new(Microsoft.UI.Colors.Black);

    private readonly List<ColorArchiveRow> _rows = new();
    private readonly List<Rectangle> _dashes = new();
    private double[] _dashW = Array.Empty<double>();
    private double[] _dashY = Array.Empty<double>();

    private List<ColorArchiveProject> _items = new();
    private double _pos;
    private int _targetIdx;
    private int _active;
    private bool _built, _loaded, _rulerOn;

    private double[] _bg = { 255, 255, 255 }, _bgT = { 255, 255, 255 };
    private double[] _ink = { 17, 17, 17 }, _inkT = { 17, 17, 17 };

    private double _rulerY = double.NaN;
    private double _wheelAcc, _touchAcc;
    private double _xfade = 1;
    private double _dragX, _dragY;
    private bool _dragging;
    private Point _dragStart, _dragOrigin;
    private double _cardSize = 320;

    private double _last = double.NaN;
    private bool _loopOn;

    private MediaPlayer? _player;
    private bool _soundInit;

    public event EventHandler<ColorArchiveActiveChangedEventArgs>? ActiveChanged;

    public ColorArchiveScroll()
    {
        DefaultStyleKey = typeof(ColorArchiveScroll);
        AutomationProperties.SetName(this, Label);

        AddHandler(PointerPressedEvent, new PointerEventHandler((_, _) => Focus(FocusState.Pointer)), true);

        RegisterPropertyChangedCallback(FontSizeProperty, (_, _) => Refresh(reset: false));
        SizeChanged += (_, _) => DoLayout();
        Loaded += (_, _) =>
        {
            _loaded = true;
            AttachProjects();
            Refresh(reset: !_built);
        };
        Unloaded += (_, _) =>
        {
            _loaded = false;
            DetachProjects();
            StopLoop();
            ResetSound();
        };
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _root.PointerWheelChanged -= OnWheel;
        _root.ManipulationDelta -= OnManipulation;
        _rulerHit.PointerMoved -= OnRulerMoved;
        _rulerHit.PointerExited -= OnRulerExited;
        _rulerHit.Tapped -= OnRulerTapped;
        _card.PointerPressed -= OnCardPressed;
        _card.PointerMoved -= OnCardMoved;
        _card.PointerReleased -= OnCardReleased;
        _card.PointerCanceled -= OnCardReleased;
        _card.PointerCaptureLost -= OnCardReleased;

        _root = GetTemplateChild(PartRoot) as Grid ?? new Grid();
        _rowsLayer = GetTemplateChild(PartRows) as Canvas ?? new Canvas();
        _rulerHit = GetTemplateChild(PartRulerHit) as Rectangle ?? new Rectangle();
        _card = GetTemplateChild(PartCard) as Border ?? new Border();
        _imgFront = GetTemplateChild(PartImageFront) as Image ?? new Image();
        _imgBack = GetTemplateChild(PartImageBack) as Image ?? new Image();

        _root.Background = _bgBrush;
        _root.ManipulationMode = ManipulationModes.TranslateY;
        _card.RenderTransform = _cardT;

        _root.PointerWheelChanged += OnWheel;
        _root.ManipulationDelta += OnManipulation;
        _rulerHit.PointerMoved += OnRulerMoved;
        _rulerHit.PointerExited += OnRulerExited;
        _rulerHit.Tapped += OnRulerTapped;
        _card.PointerPressed += OnCardPressed;
        _card.PointerMoved += OnCardMoved;
        _card.PointerReleased += OnCardReleased;
        _card.PointerCanceled += OnCardReleased;
        _card.PointerCaptureLost += OnCardReleased;

        if (_loaded)
        {
            Refresh(reset: !_built);
        }
    }

    private double RowHeight => Math.Max(8, FontSize * Math.Max(0.5, RowLeading));
    private static int Mod(int a, int n) => ((a % n) + n) % n;

    public void Refresh() => Refresh(reset: false);

    private void OnProjectsChanged(IList<ColorArchiveProject>? oldValue, IList<ColorArchiveProject>? newValue)
    {
        if (oldValue is INotifyCollectionChanged oldNotify)
        {
            oldNotify.CollectionChanged -= OnProjectsCollectionChanged;
        }

        if (_loaded && newValue is INotifyCollectionChanged newNotify)
        {
            newNotify.CollectionChanged += OnProjectsCollectionChanged;
        }

        Refresh(reset: true);
    }

    private void OnProjectsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Refresh(reset: false);
    }

    private void AttachProjects()
    {
        if (Projects is INotifyCollectionChanged notify)
        {
            notify.CollectionChanged -= OnProjectsCollectionChanged;
            notify.CollectionChanged += OnProjectsCollectionChanged;
        }
    }

    private void DetachProjects()
    {
        if (Projects is INotifyCollectionChanged notify)
        {
            notify.CollectionChanged -= OnProjectsCollectionChanged;
        }
    }

    private void Refresh(bool reset)
    {
        if (!_loaded)
        {
            if (reset)
            {
                _built = false;
            }
            return;
        }

        _items = new List<ColorArchiveProject>(Projects ?? Array.Empty<ColorArchiveProject>());
        _rowsLayer.Children.Clear();
        _rows.Clear();
        _dashes.Clear();

        int n = _items.Count;
        if (n == 0)
        {
            StopLoop();
            return;
        }

        if (reset || !_built)
        {
            int start = Math.Clamp(ActiveIndex ?? DefaultActiveIndex, 0, n - 1);
            _targetIdx = start;
            _pos = start;
        }
        else if (!InfiniteScroll)
        {
            _targetIdx = Math.Clamp(_targetIdx, 0, n - 1);
            _pos = Math.Clamp(_pos, 0, n - 1);
        }
        _active = Mod(_targetIdx, n);
        _built = true;

        for (int i = 0; i < n; i++)
        {
            var p = _items[i];
            int idx = i;

            var line = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
            line.Children.Add(new TextBlock
            {
                Text = p.Name,
                FontSize = FontSize,
                FontFamily = FontFamily,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Foreground = _inkBrush,
                IsHitTestVisible = false
            });
            line.Children.Add(new TextBlock
            {
                Text = p.Badge,
                FontSize = FontSize * 0.26,
                FontFamily = FontFamily,
                Foreground = _inkBrush,
                Opacity = 0.7,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(16, 0, 0, FontSize * 0.22),
                IsHitTestVisible = false
            });
            line.Children.Add(new TextBlock
            {
                Text = p.Date,
                FontSize = FontSize * 0.26,
                FontFamily = FontFamily,
                Foreground = _inkBrush,
                Opacity = 0.5,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(12, 0, 0, FontSize * 0.22),
                IsHitTestVisible = false
            });

            var t = new CompositeTransform();
            var host = new Grid
            {
                Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent),
                RenderTransform = t,
                Children = { line }
            };
            host.Tapped += (_, _) => GoToIndex(idx);
            _rowsLayer.Children.Add(host);
            _rows.Add(new ColorArchiveRow { Host = host, T = t });
        }

        _dashW = new double[n];
        _dashY = new double[n];
        for (int i = 0; i < n; i++)
        {
            _dashW[i] = 10;
            var d = new Rectangle { Height = 2, Width = 10, Fill = _inkBrush, IsHitTestVisible = false };
            _rowsLayer.Children.Add(d);
            _dashes.Add(d);
        }

        SetColorTargets(snap: true);
        SetCardImage(_imgFront, _items[_active].Image);
        _imgFront.Opacity = 1;
        _imgBack.Opacity = 0;
        _xfade = 1;

        DoLayout();
    }

    private void DoLayout()
    {
        if (!_loaded || _items.Count == 0)
        {
            return;
        }

        double W = ActualWidth, H = ActualHeight;
        if (W <= 0 || H <= 0)
        {
            return;
        }

        _root.Clip = new RectangleGeometry { Rect = new Rect(0, 0, W, H) };

        _rulerOn = ShowRuler && W >= MdBreakpoint;
        double gutter = _rulerOn ? RulerGutter : CompactGutter;
        double rowH = RowHeight;
        double anchorY = H * Math.Clamp(ActiveAnchor, 0.05, 0.95);

        foreach (var r in _rows)
        {
            r.Host.Height = rowH;
            r.Host.Width = Math.Max(0, W - gutter - 24);
            Canvas.SetLeft(r.Host, gutter);
            Canvas.SetTop(r.Host, anchorY - rowH / 2);
        }

        int n = _items.Count;
        double spacing = Math.Min(16, (H * 0.7) / Math.Max(1, n));
        double y0 = H / 2 - (n - 1) * spacing / 2;
        for (int i = 0; i < n; i++)
        {
            _dashY[i] = y0 + i * spacing;
            _dashes[i].Visibility = _rulerOn ? Visibility.Visible : Visibility.Collapsed;
            Canvas.SetLeft(_dashes[i], 24);
            Canvas.SetTop(_dashes[i], _dashY[i] - 1);
        }
        _rulerHit.Visibility = _rulerOn ? Visibility.Visible : Visibility.Collapsed;
        _rulerHit.Width = RulerGutter;
        _rulerHit.Height = H;
        Canvas.SetLeft(_rulerHit, 0);
        Canvas.SetTop(_rulerHit, 0);

        double ceiling = PreviewSize switch
        {
            ColorArchivePreviewSize.Small => 320,
            ColorArchivePreviewSize.Medium => 440,
            _ => 560
        };
        _cardSize = Math.Max(120, Math.Min(ceiling, Math.Min(W * 0.36, H * 0.62)));
        _card.Visibility = ShowPreview ? Visibility.Visible : Visibility.Collapsed;
        _card.Width = _card.Height = _cardSize;
        double left = W - _cardSize - Math.Max(24, W * 0.07);
        double top = Math.Max(16, anchorY - _cardSize / 2);
        Canvas.SetLeft(_card, left);
        Canvas.SetTop(_card, top);
        ClampDrag();

        ApplyRows();
        EnsureLoop();
    }

    private void ApplyRows()
    {
        int n = _rows.Count;
        if (n == 0)
        {
            return;
        }

        bool wrap = InfiniteScroll;
        double rowH = RowHeight;

        for (int i = 0; i < n; i++)
        {
            double d = i - _pos;
            if (wrap)
            {
                d -= n * Math.Round(d / n);
            }
            double ad = Math.Abs(d);

            var r = _rows[i];
            r.T.TranslateY = d * rowH;
            r.T.TranslateX = 18 * (1 - Math.Min(1, ad));

            double op = Math.Max(0.08, 1 / (1 + 1.6 * ad));
            if (wrap && n >= 6)
            {
                op *= Math.Clamp((n / 2.0 - ad) / 1.5, 0, 1);
            }
            r.Host.Opacity = op;
        }
    }

    private void RequestTarget(int unwrapped)
    {
        int n = _items.Count;
        if (n == 0)
        {
            return;
        }

        if (!InfiniteScroll)
        {
            unwrapped = Math.Clamp(unwrapped, 0, n - 1);
        }

        int wrapped = Mod(unwrapped, n);
        if (ActiveIndex.HasValue)
        {
            if (wrapped != _active)
            {
                ActiveChanged?.Invoke(this, new ColorArchiveActiveChangedEventArgs(wrapped, _items[wrapped]));
            }
            return;
        }
        ApplyTarget(unwrapped);
    }

    private void ApplyTarget(int unwrapped)
    {
        int n = _items.Count;
        if (n == 0)
        {
            return;
        }

        _targetIdx = unwrapped;
        int w = Mod(unwrapped, n);
        if (w != _active)
        {
            _active = w;
            SetColorTargets(snap: false);
            StartCardTransition();
            PlayTick();
            ActiveChanged?.Invoke(this, new ColorArchiveActiveChangedEventArgs(w, _items[w]));
        }
        EnsureLoop();
    }

    private int ShortestTarget(int index)
    {
        int n = _items.Count;
        index = Mod(index, n);
        if (!InfiniteScroll)
        {
            return index;
        }

        int diff = Mod(index - _active + n / 2, n) - n / 2;
        return _targetIdx + diff;
    }

    private void GoToIndex(int index)
    {
        if (_items.Count == 0)
        {
            return;
        }
        RequestTarget(ShortestTarget(index));
    }

    private void Step(int rows) => RequestTarget(_targetIdx + rows);

    private void OnActiveIndexChanged()
    {
        if (!_built || _items.Count == 0 || !ActiveIndex.HasValue)
        {
            return;
        }
        ApplyTarget(ShortestTarget(ActiveIndex.Value));
    }

    private void OnInfiniteChanged()
    {
        if (!_built || _items.Count == 0)
        {
            return;
        }

        int n = _items.Count;
        _targetIdx = Mod(_targetIdx, n);
        _pos = _targetIdx;
        ApplyRows();
    }

    private void OnWheel(object sender, PointerRoutedEventArgs e)
    {
        var props = e.GetCurrentPoint(_root).Properties;
        e.Handled = true;
        if (props.IsHorizontalMouseWheel)
        {
            return;
        }

        _wheelAcc += -props.MouseWheelDelta * 0.75;
        Drain(ref _wheelAcc);
    }

    private void OnManipulation(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        _touchAcc -= e.Delta.Translation.Y;
        Drain(ref _touchAcc);
        e.Handled = true;
    }

    private void Drain(ref double acc)
    {
        double th = Math.Max(10, ScrollThreshold);
        while (Math.Abs(acc) >= th)
        {
            int dir = Math.Sign(acc);
            acc -= dir * th;
            Step(dir);
        }
    }

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        base.OnKeyDown(e);
        if (_items.Count == 0)
        {
            return;
        }

        int page = Math.Max(1, (int)(ActualHeight / RowHeight) / 2);
        switch (e.Key)
        {
            case VirtualKey.Down: Step(1); break;
            case VirtualKey.Up: Step(-1); break;
            case VirtualKey.PageDown: Step(page); break;
            case VirtualKey.PageUp: Step(-page); break;
            case VirtualKey.Home: GoToIndex(0); break;
            case VirtualKey.End: GoToIndex(_items.Count - 1); break;
            default: return;
        }
        e.Handled = true;
    }

    private void OnRulerMoved(object sender, PointerRoutedEventArgs e)
    {
        _rulerY = e.GetCurrentPoint(_root).Position.Y;
        EnsureLoop();
    }

    private void OnRulerExited(object sender, PointerRoutedEventArgs e)
    {
        _rulerY = double.NaN;
        EnsureLoop();
    }

    private void OnRulerTapped(object sender, TappedRoutedEventArgs e)
    {
        if (_items.Count == 0)
        {
            return;
        }

        double y = e.GetPosition(_root).Y;
        int best = 0;
        double bestD = double.MaxValue;
        for (int i = 0; i < _dashY.Length; i++)
        {
            double d = Math.Abs(_dashY[i] - y);
            if (d < bestD)
            {
                bestD = d;
                best = i;
            }
        }
        GoToIndex(best);
        e.Handled = true;
    }

    private void OnCardPressed(object sender, PointerRoutedEventArgs e)
    {
        _dragging = true;
        _dragStart = e.GetCurrentPoint(_root).Position;
        _dragOrigin = new Point(_dragX, _dragY);
        _card.CapturePointer(e.Pointer);
        e.Handled = true;
    }

    private void OnCardMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!_dragging)
        {
            return;
        }

        var p = e.GetCurrentPoint(_root).Position;
        _dragX = _dragOrigin.X + (p.X - _dragStart.X);
        _dragY = _dragOrigin.Y + (p.Y - _dragStart.Y);
        ClampDrag();
        e.Handled = true;
    }

    private void OnCardReleased(object sender, PointerRoutedEventArgs e)
    {
        if (!_dragging)
        {
            return;
        }

        _dragging = false;
        _card.ReleasePointerCaptures();
    }

    private void ClampDrag()
    {
        double W = ActualWidth, H = ActualHeight;
        double left = Canvas.GetLeft(_card), top = Canvas.GetTop(_card);
        _dragX = Math.Clamp(_dragX, 8 - left, Math.Max(8 - left, W - _cardSize - 8 - left));
        _dragY = Math.Clamp(_dragY, 8 - top, Math.Max(8 - top, H - _cardSize - 8 - top));
        _cardT.TranslateX = _dragX;
        _cardT.TranslateY = _dragY;
    }

    private static void SetCardImage(Image img, string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            img.Source = null;
            return;
        }

        try
        {
            img.Source = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
        }
        catch
        {
            img.Source = null;
        }
    }

    private void StartCardTransition()
    {
        SetCardImage(_imgBack, _items[_active].Image);
        _imgBack.Opacity = 0;
        _xfade = 0;
    }

    private void SetColorTargets(bool snap)
    {
        var c = _items[_active].BgColor;
        _bgT = new double[] { c.R, c.G, c.B };
        double lum = (0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B) / 255.0;
        _inkT = lum > 0.58 ? new double[] { 17, 17, 17 } : new double[] { 250, 250, 250 };

        if (snap || ReduceMotion)
        {
            Array.Copy(_bgT, _bg, 3);
            Array.Copy(_inkT, _ink, 3);
            PushColors();
        }
    }

    private void PushColors()
    {
        static byte B(double v) => (byte)Math.Clamp((int)Math.Round(v), 0, 255);
        _bgBrush.Color = Windows.UI.Color.FromArgb(255, B(_bg[0]), B(_bg[1]), B(_bg[2]));
        _inkBrush.Color = Windows.UI.Color.FromArgb(255, B(_ink[0]), B(_ink[1]), B(_ink[2]));
    }

    private void EnsureLoop()
    {
        if (_loopOn || !_loaded)
        {
            return;
        }

        _loopOn = true;
        _last = double.NaN;
        CompositionTarget.Rendering += OnRendering;
    }

    private void StopLoop()
    {
        if (!_loopOn)
        {
            return;
        }

        _loopOn = false;
        CompositionTarget.Rendering -= OnRendering;
    }

    private void OnRendering(object? sender, object e)
    {
        double now = e is RenderingEventArgs args
            ? args.RenderingTime.TotalSeconds
            : (double.IsNaN(_last) ? 0 : _last) + 1.0 / 60;
        double dt = double.IsNaN(_last) ? 1.0 / 60 : Math.Clamp(now - _last, 0.001, 0.05);
        _last = now;

        bool motion = !ReduceMotion;
        bool anim = false;

        if (_pos != _targetIdx)
        {
            if (!motion)
            {
                _pos = _targetIdx;
            }
            else
            {
                _pos += (_targetIdx - _pos) * (1 - Math.Exp(-dt * 9));
                if (Math.Abs(_targetIdx - _pos) < 0.002)
                {
                    _pos = _targetIdx;
                }
            }
            ApplyRows();
            anim |= _pos != _targetIdx;
        }

        double f = motion ? 1 - Math.Exp(-dt * 6) : 1;
        for (int c = 0; c < 3; c++)
        {
            _bg[c] += (_bgT[c] - _bg[c]) * f;
            if (Math.Abs(_bgT[c] - _bg[c]) < 0.3) _bg[c] = _bgT[c]; else anim = true;
            _ink[c] += (_inkT[c] - _ink[c]) * f;
            if (Math.Abs(_inkT[c] - _ink[c]) < 0.3) _ink[c] = _inkT[c]; else anim = true;
        }
        PushColors();

        if (_rulerOn)
        {
            anim |= UpdateRuler(motion ? 1 - Math.Exp(-dt * 14) : 1);
        }

        if (_xfade < 1)
        {
            _xfade = motion ? Math.Min(1, _xfade + dt / 0.28) : 1;
            double ease = 1 - Math.Pow(1 - _xfade, 3);
            _imgBack.Opacity = ease;
            double s = 0.95 + 0.05 * ease;
            _cardT.ScaleX = _cardT.ScaleY = s;
            if (_xfade >= 1)
            {
                _imgFront.Source = _imgBack.Source;
                _imgFront.Opacity = 1;
                _imgBack.Opacity = 0;
                _cardT.ScaleX = _cardT.ScaleY = 1;
            }
            else
            {
                anim = true;
            }
        }

        if (!anim)
        {
            StopLoop();
        }
    }

    private bool UpdateRuler(double f)
    {
        bool moving = false;
        for (int i = 0; i < _dashes.Count; i++)
        {
            double boost = double.IsNaN(_rulerY) ? 0 : Math.Exp(-Math.Pow((_dashY[i] - _rulerY) / 32, 2));
            bool act = i == _active;
            double tw = 10 + 26 * boost + (act ? 12 : 0);

            _dashW[i] += (tw - _dashW[i]) * f;
            if (Math.Abs(tw - _dashW[i]) > 0.2) moving = true; else _dashW[i] = tw;

            _dashes[i].Width = Math.Max(1, _dashW[i]);
            _dashes[i].Opacity = act ? 1 : 0.35 + 0.45 * boost;
        }
        return moving;
    }

    private void ResetSound()
    {
        _player?.Dispose();
        _player = null;
        _soundInit = false;
        if (_loaded && EnableSound)
        {
            EnsureSound();
        }
    }

    private async void EnsureSound()
    {
        if (_soundInit)
        {
            return;
        }

        _soundInit = true;
        try
        {
            MediaSource source;
            if (TickSource != null)
            {
                source = MediaSource.CreateFromUri(TickSource);
            }
            else
            {
                var stream = new InMemoryRandomAccessStream();
                await stream.WriteAsync(BuildTickWav().AsBuffer());
                stream.Seek(0);
                source = MediaSource.CreateFromStream(stream, "audio/wav");
            }
            _player = new MediaPlayer { AutoPlay = false, Volume = 0.5, Source = source };
        }
        catch
        {
            _soundInit = false;
        }
    }

    private void PlayTick()
    {
        if (!EnableSound)
        {
            return;
        }

        if (_player == null)
        {
            EnsureSound();
            return;
        }

        try
        {
            _player.PlaybackSession.Position = TimeSpan.Zero;
            _player.Play();
        }
        catch { }
    }

    private static byte[] BuildTickWav()
    {
        const int sr = 44100;
        int n = (int)(sr * 0.045);

        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write(new[] { 'R', 'I', 'F', 'F' });
        w.Write(36 + n * 2);
        w.Write(new[] { 'W', 'A', 'V', 'E' });
        w.Write(new[] { 'f', 'm', 't', ' ' });
        w.Write(16);
        w.Write((short)1);
        w.Write((short)1);
        w.Write(sr);
        w.Write(sr * 2);
        w.Write((short)2);
        w.Write((short)16);
        w.Write(new[] { 'd', 'a', 't', 'a' });
        w.Write(n * 2);
        for (int i = 0; i < n; i++)
        {
            double t = i / (double)sr;
            double v = Math.Sin(2 * Math.PI * 1700 * t) * Math.Exp(-t * 120) * 0.5;
            w.Write((short)(v * short.MaxValue));
        }
        w.Flush();
        return ms.ToArray();
    }
}
