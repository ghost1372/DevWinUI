using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace DevWinUI;

/// <summary>
/// A call to action whose icon tile expands into a full fill on hover or keyboard focus and swaps the label.
/// Content should be text, because it is presented twice: as the label and as its decorative copy on the fill.
/// </summary>
[TemplatePart(Name = PartRoot, Type = typeof(FrameworkElement))]
[TemplatePart(Name = PartSurface, Type = typeof(Border))]
[TemplatePart(Name = PartClip, Type = typeof(Grid))]
[TemplatePart(Name = PartLayer, Type = typeof(FrameworkElement))]
[TemplatePart(Name = PartFill, Type = typeof(FrameworkElement))]
[TemplatePart(Name = PartTrack, Type = typeof(FrameworkElement))]
[TemplatePart(Name = PartIconCell, Type = typeof(FrameworkElement))]
[TemplatePart(Name = PartDefaultIcon, Type = typeof(FrameworkElement))]
[TemplatePart(Name = PartCustomIcon, Type = typeof(FrameworkElement))]
[TemplatePart(Name = PartLabel, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PartFillLabel, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PartRing, Type = typeof(Border))]
[TemplatePart(Name = PartRingOffset, Type = typeof(Border))]
[TemplateVisualState(GroupName = ToneStates, Name = SoftState)]
[TemplateVisualState(GroupName = ToneStates, Name = OutlineState)]
public sealed partial class FillButton : Button
{
    private const string PartRoot = "PART_Root";
    private const string PartSurface = "PART_Surface";
    private const string PartClip = "PART_Clip";
    private const string PartLayer = "PART_Layer";
    private const string PartFill = "PART_Fill";
    private const string PartTrack = "PART_Track";
    private const string PartIconCell = "PART_IconCell";
    private const string PartDefaultIcon = "PART_DefaultIcon";
    private const string PartCustomIcon = "PART_CustomIcon";
    private const string PartLabel = "PART_Label";
    private const string PartFillLabel = "PART_FillLabel";
    private const string PartRing = "PART_Ring";
    private const string PartRingOffset = "PART_RingOffset";

    private const string CommonStates = "CommonStates";
    private const string ToneStates = "ToneStates";
    private const string SoftState = "Soft";
    private const string OutlineState = "Outline";
    private const string PointerOverState = "PointerOver";
    private const string PressedState = "Pressed";

    private const float PressScale = 0.97f;
    private const double LabelInset = 4;
    private const double RingOffsetGrowth = 2;
    private const double RingGrowth = 4;

    private static readonly TimeSpan FillDuration = TimeSpan.FromMilliseconds(500);
    private static readonly TimeSpan PressDuration = TimeSpan.FromMilliseconds(150);
    private static readonly TimeSpan Instant = TimeSpan.FromMilliseconds(1);
    private static readonly Vector2 EaseP1 = new(0.16f, 1f);
    private static readonly Vector2 EaseP2 = new(0.3f, 1f);

    private readonly record struct Metrics(
        double Height, double Radius, double PaddingRight, double FontSize,
        double Gap, double FillRadius, double Tile, double Icon);

    private static readonly Metrics SmallMetrics = new(36, 8, 14, 14, 10, 6, 28, 14);
    private static readonly Metrics DefaultMetrics = new(44, 12, 18, 14, 14, 8, 36, 16);
    private static readonly Metrics LargeMetrics = new(56, 16, 24, 16, 18, 12, 48, 20);

    private FrameworkElement _root = null!;
    private Border _surface = null!;
    private Grid _clipHost = null!;
    private FrameworkElement _layer = null!;
    private FrameworkElement _fill = null!;
    private FrameworkElement _track = null!;
    private ContentPresenter _label = null!;
    private ContentPresenter _fillLabel = null!;
    private FrameworkElement? _iconCell;
    private FrameworkElement? _defaultIcon;
    private FrameworkElement? _customIcon;
    private Border? _ring;
    private Border? _ringOffset;

    private Compositor _compositor = null!;
    private CubicBezierEasingFunction _ease = null!;
    private Visual _rootVisual = null!;
    private Visual _trackVisual = null!;
    private Visual _labelVisual = null!;
    private Visual _fillLabelVisual = null!;
    private CompositionRoundedRectangleGeometry _fillGeometry = null!;
    private readonly RectangleGeometry _clipGeometry = new();

    private InputSystemCursor? _handCursor;
    private Metrics _metrics = DefaultMetrics;
    private bool _templated;
    private bool _hover;
    private bool _pressed;
    private bool _keyboardFocus;
    private bool _filled;

    public FillButton()
    {
        DefaultStyleKey = typeof(FillButton);
        Click += OnClickInternal;
    }

    protected override void OnApplyTemplate()
    {
        Detach();
        base.OnApplyTemplate();

        _templated = false;

        var root = GetTemplateChild(PartRoot) as FrameworkElement;
        var surface = GetTemplateChild(PartSurface) as Border;
        var clipHost = GetTemplateChild(PartClip) as Grid;
        var layer = GetTemplateChild(PartLayer) as FrameworkElement;
        var fill = GetTemplateChild(PartFill) as FrameworkElement;
        var track = GetTemplateChild(PartTrack) as FrameworkElement;
        var label = GetTemplateChild(PartLabel) as ContentPresenter;
        var fillLabel = GetTemplateChild(PartFillLabel) as ContentPresenter;

        if (root is null || surface is null || clipHost is null || layer is null ||
            fill is null || track is null || label is null || fillLabel is null) return;

        _root = root;
        _surface = surface;
        _clipHost = clipHost;
        _layer = layer;
        _fill = fill;
        _track = track;
        _label = label;
        _fillLabel = fillLabel;
        _iconCell = GetTemplateChild(PartIconCell) as FrameworkElement;
        _defaultIcon = GetTemplateChild(PartDefaultIcon) as FrameworkElement;
        _customIcon = GetTemplateChild(PartCustomIcon) as FrameworkElement;
        _ring = GetTemplateChild(PartRing) as Border;
        _ringOffset = GetTemplateChild(PartRingOffset) as Border;

        _rootVisual = ElementCompositionPreview.GetElementVisual(_root);
        _compositor = _rootVisual.Compositor;
        _ease = _compositor.CreateCubicBezierEasingFunction(EaseP1, EaseP2);

        var fillVisual = ElementCompositionPreview.GetElementVisual(_fill);
        _trackVisual = ElementCompositionPreview.GetElementVisual(_track);
        _labelVisual = ElementCompositionPreview.GetElementVisual(_label);
        _fillLabelVisual = ElementCompositionPreview.GetElementVisual(_fillLabel);
        ElementCompositionPreview.SetIsTranslationEnabled(_track, true);
        ElementCompositionPreview.SetIsTranslationEnabled(_label, true);
        ElementCompositionPreview.SetIsTranslationEnabled(_fillLabel, true);

        _fillGeometry = _compositor.CreateRoundedRectangleGeometry();
        fillVisual.Clip = _compositor.CreateGeometricClip(_fillGeometry);

        _clipHost.Clip = _clipGeometry;

        _root.SizeChanged += OnRootSizeChanged;
        _clipHost.SizeChanged += OnClipHostSizeChanged;
        _layer.SizeChanged += OnLayoutChanged;
        _label.SizeChanged += OnLayoutChanged;
        _fillLabel.SizeChanged += OnLayoutChanged;
        SubscribeStates(attach: true);

        _templated = true;

        ApplyTone();
        ApplyIcon();
        ApplyLinkBehavior();
        ApplySize();
        UpdateRing();
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new FillButtonAutomationPeer(this);

    private void Detach()
    {
        if (!_templated) return;

        _root.SizeChanged -= OnRootSizeChanged;
        _clipHost.SizeChanged -= OnClipHostSizeChanged;
        _layer.SizeChanged -= OnLayoutChanged;
        _label.SizeChanged -= OnLayoutChanged;
        _fillLabel.SizeChanged -= OnLayoutChanged;
        SubscribeStates(attach: false);
    }

    private void SubscribeStates(bool attach)
    {
        foreach (var group in VisualStateManager.GetVisualStateGroups(_root))
        {
            if (group.Name != CommonStates) continue;

            if (attach) group.CurrentStateChanged += OnCommonStateChanged;
            else group.CurrentStateChanged -= OnCommonStateChanged;
        }
    }

    private void ApplyTone() =>
        VisualStateManager.GoToState(this, Tone == FillButtonTone.Outline ? OutlineState : SoftState, false);

    private void ApplyIcon()
    {
        if (_defaultIcon is null || _customIcon is null) return;

        bool custom = Icon is not null;
        _customIcon.Visibility = custom ? Visibility.Visible : Visibility.Collapsed;
        _defaultIcon.Visibility = custom ? Visibility.Collapsed : Visibility.Visible;
    }

    private void ApplyLinkBehavior()
    {
        if (NavigateUri is null)
        {
            ProtectedCursor = null;
            return;
        }

        _handCursor ??= InputSystemCursor.Create(InputSystemCursorShape.Hand);
        ProtectedCursor = _handCursor;
    }

    private void ApplySize()
    {
        _metrics = Size switch
        {
            FillButtonSize.Small => SmallMetrics,
            FillButtonSize.Large => LargeMetrics,
            _ => DefaultMetrics,
        };

        if (!_templated) return;

        _root.Height = _metrics.Height;
        _surface.CornerRadius = new CornerRadius(_metrics.Radius);

        if (_ringOffset is not null) _ringOffset.CornerRadius = new CornerRadius(_metrics.Radius + RingOffsetGrowth);
        if (_ring is not null) _ring.CornerRadius = new CornerRadius(_metrics.Radius + RingGrowth);

        _label.Margin = new Thickness(LabelInset, 0, _metrics.PaddingRight, 0);
        _label.Padding = new Thickness(_metrics.Tile + _metrics.Gap, 0, 0, 0);
        _label.FontSize = _metrics.FontSize;

        _fillLabel.Padding = new Thickness(_metrics.Gap, 0, 0, 0);
        _fillLabel.FontSize = _metrics.FontSize;

        if (_iconCell is not null) _iconCell.Width = _metrics.Tile;
        SizeIcon(_defaultIcon);
        SizeIcon(_customIcon);

        _fillGeometry.CornerRadius = new Vector2((float)_metrics.FillRadius);
        ApplyFillState(animate: false);
    }

    private void SizeIcon(FrameworkElement? icon)
    {
        if (icon is null) return;

        icon.Width = _metrics.Icon;
        icon.Height = _metrics.Icon;
    }

    private void UpdateRing()
    {
        var visibility = _keyboardFocus ? Visibility.Visible : Visibility.Collapsed;
        if (_ring is not null) _ring.Visibility = visibility;
        if (_ringOffset is not null) _ringOffset.Visibility = visibility;
    }

    private void OnCommonStateChanged(object sender, VisualStateChangedEventArgs e)
    {
        string? name = e.NewState?.Name;
        bool pressed = name == PressedState;
        bool pressChanged = pressed != _pressed;

        _hover = name is PointerOverState or PressedState;
        _pressed = pressed;

        UpdateFilled();
        if (pressChanged) UpdatePress();
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
        base.OnGotFocus(e);

        _keyboardFocus = FocusState == FocusState.Keyboard;
        UpdateRing();
        UpdateFilled();
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);

        _keyboardFocus = false;
        UpdateRing();
        UpdateFilled();
    }

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        if (NavigateUri is not null && e.Key == VirtualKey.Space)
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    protected override void OnKeyUp(KeyRoutedEventArgs e)
    {
        if (NavigateUri is not null && e.Key == VirtualKey.Space)
        {
            e.Handled = true;
            return;
        }

        base.OnKeyUp(e);
    }

    private async void OnClickInternal(object sender, RoutedEventArgs e)
    {
        if (NavigateUri is not { } uri) return;

        try
        {
            await Launcher.LaunchUriAsync(uri);
        }
        catch (Exception)
        {
        }
    }

    private void UpdateFilled()
    {
        bool filled = _hover || _keyboardFocus;
        if (filled == _filled) return;

        _filled = filled;
        ApplyFillState(animate: true);
    }

    private void OnRootSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _rootVisual.CenterPoint = new Vector3((float)e.NewSize.Width / 2f, (float)e.NewSize.Height / 2f, 0f);
    }

    private void OnClipHostSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _clipGeometry.Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);
    }

    private void OnLayoutChanged(object sender, SizeChangedEventArgs e) => ApplyFillState(animate: false);

    private void ApplyFillState(bool animate)
    {
        if (!_templated) return;

        float width = (float)_layer.ActualWidth;
        float height = (float)_layer.ActualHeight;
        if (width <= 0 || height <= 0) return;

        var duration = animate ? FillDuration : Instant;
        float tile = (float)_metrics.Tile;
        float labelWidth = (float)_label.ActualWidth;
        float fillLabelWidth = (float)_fillLabel.ActualWidth;

        StartVector2(_fillGeometry, "Size", _filled ? new Vector2(width, height) : new Vector2(tile, height), duration);
        StartVector3(_trackVisual, "Translation", new Vector3(_filled ? width - tile : 0f, 0f, 0f), duration);

        StartVector3(_labelVisual, "Translation", new Vector3(_filled ? labelWidth : 0f, 0f, 0f), duration);
        StartScalar(_labelVisual, "Opacity", _filled ? 0f : 1f, duration);

        StartVector3(_fillLabelVisual, "Translation", new Vector3(_filled ? 0f : -fillLabelWidth, 0f, 0f), duration);
        StartScalar(_fillLabelVisual, "Opacity", _filled ? 1f : 0f, duration);
    }

    private void UpdatePress()
    {
        if (!_templated) return;

        float scale = _pressed ? PressScale : 1f;

        var animation = _compositor.CreateVector3KeyFrameAnimation();
        animation.InsertKeyFrame(1f, new Vector3(scale, scale, 1f), _ease);
        animation.Duration = PressDuration;
        _rootVisual.StartAnimation("Scale", animation);
    }

    private void StartScalar(CompositionObject target, string property, float value, TimeSpan duration)
    {
        var animation = _compositor.CreateScalarKeyFrameAnimation();
        animation.InsertKeyFrame(1f, value, _ease);
        animation.Duration = duration;
        target.StartAnimation(property, animation);
    }

    private void StartVector2(CompositionObject target, string property, Vector2 value, TimeSpan duration)
    {
        var animation = _compositor.CreateVector2KeyFrameAnimation();
        animation.InsertKeyFrame(1f, value, _ease);
        animation.Duration = duration;
        target.StartAnimation(property, animation);
    }

    private void StartVector3(CompositionObject target, string property, Vector3 value, TimeSpan duration)
    {
        var animation = _compositor.CreateVector3KeyFrameAnimation();
        animation.InsertKeyFrame(1f, value, _ease);
        animation.Duration = duration;
        target.StartAnimation(property, animation);
    }
}
