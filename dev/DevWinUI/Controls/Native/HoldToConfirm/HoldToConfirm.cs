using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

[TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
[TemplatePart(Name = "PART_BaseFace", Type = typeof(Border))]
[TemplatePart(Name = "PART_FillFace", Type = typeof(Border))]
public sealed partial class HoldToConfirm : Control
{
    private const double OmegaSpring = 22.0;
    private const double MaxFrameTime = 0.05;
    private const double PointerSlack = 24;

    private enum Source { None, Pointer, Key }

    private Grid? _root;
    private HoldToConfirmFace? _base;
    private HoldToConfirmFace? _fill;
    private InsetClip? _clip;

    private double _p;
    private double _v;
    private double _w;
    private bool _holding;
    private bool _doneVisual;
    private bool _loop;
    private bool _completing;
    private bool _hasLastFrame;
    private Source _source;
    private Windows.System.VirtualKey _key;
    private TimeSpan _lastFrame;

    public HoldToConfirm()
    {
        DefaultStyleKey = typeof(HoldToConfirm);

        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerReleased += OnPointerReleased;
        PointerCanceled += OnPointerEnded;
        PointerCaptureLost += OnPointerEnded;
        LostFocus += OnLostFocus;
        IsEnabledChanged += OnIsEnabledChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    internal bool IsDone => _doneVisual;

    private bool HasGlyph => !string.IsNullOrEmpty(IconGlyph);

    protected override void OnApplyTemplate()
    {
        if (_root is not null) _root.SizeChanged -= OnRootSizeChanged;

        base.OnApplyTemplate();

        _root = GetTemplateChild("PART_Root") as Grid;
        _base = HoldToConfirmFace.Create(GetTemplateChild, "PART_Base");
        _fill = HoldToConfirmFace.Create(GetTemplateChild, "PART_Fill");
        _clip = null;

        if (_root is null || _base is null || _fill is null)
        {
            _base = null;
            _fill = null;
            return;
        }

        _root.SizeChanged += OnRootSizeChanged;
        _w = _root.ActualWidth;

        var fillVisual = ElementCompositionPreview.GetElementVisual(_fill.Root);
        _clip = fillVisual.Compositor.CreateInsetClip();
        _clip.RightInset = 10000f;
        fillVisual.Clip = _clip;

        UpdateVisualStates();
        UpdateText();
        UpdateIcon();
        ApplyFill();
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new HoldToConfirmPeer(this);

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        if (IsHoldKey(e.Key))
        {
            if (!e.KeyStatus.WasKeyDown) BeginHold(Source.Key, e.Key);
            e.Handled = true;
            return;
        }
        base.OnKeyDown(e);
    }

    protected override void OnKeyUp(KeyRoutedEventArgs e)
    {
        if (_source == Source.Key && e.Key == _key)
        {
            EndHold();
            e.Handled = true;
            return;
        }
        base.OnKeyUp(e);
    }

    public void Reset()
    {
        ApplyDone(false, true);
        if (Confirmed == true) Confirmed = false;
    }

    private static bool IsHoldKey(Windows.System.VirtualKey k) =>
        k is Windows.System.VirtualKey.Space or Windows.System.VirtualKey.Enter or Windows.System.VirtualKey.GamepadA;

    private void OnLoaded(object sender, RoutedEventArgs e) => ApplyFill();

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _holding = false;
        _completing = false;
        _source = Source.None;
        if (!_doneVisual) _p = 0;
        StopLoop();
    }

    private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        UpdateVisualStates();
        if (!IsEnabled) EndHold();
    }

    private void OnLostFocus(object sender, RoutedEventArgs e)
    {
        if (_source == Source.Key) EndHold();
    }

    private void OnRootSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _w = e.NewSize.Width;
        ApplyFill();
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var pt = e.GetCurrentPoint(this);
        if (pt.PointerDeviceType == PointerDeviceType.Mouse && !pt.Properties.IsLeftButtonPressed) return;
        if (_doneVisual || !IsEnabled) return;

        CapturePointer(e.Pointer);
        Focus(FocusState.Pointer);
        BeginHold(Source.Pointer, default);
        e.Handled = true;
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (_source != Source.Pointer) return;

        var pos = e.GetCurrentPoint(this).Position;
        double dx = Math.Max(Math.Max(-pos.X, pos.X - ActualWidth), 0);
        double dy = Math.Max(Math.Max(-pos.Y, pos.Y - ActualHeight), 0);
        if (Math.Sqrt(dx * dx + dy * dy) > PointerSlack) EndHold();
    }

    private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (_source != Source.Pointer) return;

        EndHold();
        e.Handled = true;
    }

    private void OnPointerEnded(object sender, PointerRoutedEventArgs e)
    {
        if (_source == Source.Pointer) EndHold();
    }

    private void BeginHold(Source source, Windows.System.VirtualKey key)
    {
        if (_holding || _doneVisual || !IsEnabled || _base is null) return;

        _holding = true;
        _source = source;
        _key = key;
        _v = 0;
        Press(true);
        StartLoop();
    }

    private void EndHold()
    {
        if (!_holding || _completing) return;

        _holding = false;
        _source = Source.None;
        _v = 0;
        Press(false);
        ReleasePointerCaptures();

        if (!IsAnimationEnabled)
        {
            _p = 0;
            ApplyFill();
            StopLoop();
        }
    }

    private void Complete()
    {
        _completing = false;
        if (_doneVisual || !IsLoaded) return;

        _holding = false;
        _source = Source.None;
        _p = 1;
        _v = 0;
        StopLoop();
        Press(false);
        ReleasePointerCaptures();
        ApplyFill();

        ApplyDone(true, true);
        if (Confirmed is not null) Confirmed = true;
        Confirm?.Invoke(this, null);
    }

    private void StartLoop()
    {
        if (_loop) return;

        _loop = true;
        _hasLastFrame = false;
        Microsoft.UI.Xaml.Media.CompositionTarget.Rendering += OnRendering;
    }

    private void StopLoop()
    {
        if (!_loop) return;

        _loop = false;
        Microsoft.UI.Xaml.Media.CompositionTarget.Rendering -= OnRendering;
    }

    private void OnRendering(object? sender, object e)
    {
        if (_completing) return;

        var frameTime = ((RenderingEventArgs)e).RenderingTime;
        double dt = _hasLastFrame ? Math.Min((frameTime - _lastFrame).TotalSeconds, MaxFrameTime) : 0;
        _lastFrame = frameTime;
        _hasLastFrame = true;
        if (dt <= 0) return;

        if (_holding)
        {
            _p += dt / Math.Max(0.05, Duration / 1000.0);
            if (_p >= 1)
            {
                _p = 1;
                _completing = true;
                ApplyFill();
                DispatcherQueue.TryEnqueue(Complete);
                return;
            }
        }
        else
        {
            int steps = Math.Max(1, (int)Math.Ceiling(dt / 0.008));
            double h = dt / steps;
            for (int i = 0; i < steps; i++)
            {
                double a = -OmegaSpring * OmegaSpring * _p - 2 * OmegaSpring * _v;
                _v += a * h;
                _p += _v * h;
            }

            if (_p < 0) _p = 0;
            if (_p < 0.002 && Math.Abs(_v) < 0.02)
            {
                _p = 0;
                _v = 0;
                StopLoop();
            }
        }

        ApplyFill();
    }

    private void ApplyFill()
    {
        if (_clip is null) return;

        double p = IsAnimationEnabled ? _p : (_p >= 1 ? 1 : 0);
        _clip.RightInset = (float)Math.Max(0, (1 - Math.Clamp(p, 0, 1)) * _w);
    }

    private void Press(bool down)
    {
        if (!IsAnimationEnabled || _root is null) return;

        var v = ElementCompositionPreview.GetElementVisual(_root);
        v.CenterPoint = new Vector3((float)_root.ActualWidth / 2f, (float)_root.ActualHeight / 2f, 0);
        v.StopAnimation("Scale");

        var a = v.Compositor.CreateVector3KeyFrameAnimation();
        a.InsertKeyFrame(1f, down ? new Vector3(0.97f, 0.97f, 1f) : Vector3.One,
            v.Compositor.CreateCubicBezierEasingFunction(new Vector2(0.2f, 0f), new Vector2(0f, 1f)));
        a.Duration = TimeSpan.FromMilliseconds(down ? 120 : 200);
        v.StartAnimation("Scale", a);
    }

    private void ApplyDone(bool done, bool animate)
    {
        if (_doneVisual == done) return;
        _doneVisual = done;

        if (_holding)
        {
            _holding = false;
            _source = Source.None;
            Press(false);
            ReleasePointerCaptures();
        }

        bool anim = animate && IsLoaded && IsAnimationEnabled;
        bool glyph = HasGlyph;
        if (anim)
        {
            _base?.Morph(done, glyph);
            _fill?.Morph(done, glyph);
        }
        else
        {
            _base?.Snap(done, glyph);
            _fill?.Snap(done, glyph);
        }

        if (done)
        {
            _p = 1;
            _v = 0;
            StopLoop();
            ApplyFill();
        }
        else if (_p > 0)
        {
            _v = 0;
            if (IsAnimationEnabled) StartLoop();
            else
            {
                _p = 0;
                ApplyFill();
            }
        }

        UpdateAutomation();
        if (done && animate && FrameworkElementAutomationPeer.FromElement(this) is { } peer)
        {
            peer.RaiseNotificationEvent(
                AutomationNotificationKind.ActionCompleted,
                AutomationNotificationProcessing.ImportantMostRecent,
                ConfirmedLabel,
                "HoldToConfirm.Done");
        }
    }

    private void UpdateText()
    {
        if (_base is not null && _fill is not null)
        {
            _base.LabelA.Text = _fill.LabelA.Text = Label ?? string.Empty;
            _base.LabelB.Text = _fill.LabelB.Text = ConfirmedLabel ?? string.Empty;
        }

        UpdateAutomation();
    }

    private void UpdateIcon()
    {
        if (_base is null || _fill is null) return;

        _base.Glyph.Glyph = _fill.Glyph.Glyph = IconGlyph ?? string.Empty;
        _base.Snap(_doneVisual, HasGlyph);
        _fill.Snap(_doneVisual, HasGlyph);
    }

    private void UpdateAutomation()
    {
        double s = Duration / 1000.0;
        string hint = $"Press and hold for {s:0.##} {(s == 1 ? "second" : "seconds")}";
        AutomationProperties.SetName(this, _doneVisual ? ConfirmedLabel : Label);
        AutomationProperties.SetHelpText(this, _doneVisual ? string.Empty : hint);
    }

    private void UpdateVisualStates()
    {
        VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", false);

        VisualStateManager.GoToState(this, Kind switch
        {
            HoldToConfirmKind.Destructive => "VariantDestructive",
            HoldToConfirmKind.Secondary => "VariantSecondary",
            _ => "VariantDefault",
        }, false);

        VisualStateManager.GoToState(this, Size switch
        {
            HoldToConfirmSize.Small => "SizeSm",
            HoldToConfirmSize.Large => "SizeLg",
            _ => "SizeDefault",
        }, false);
    }
}
