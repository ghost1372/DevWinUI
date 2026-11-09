using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Shapes;
using System.Reflection.Emit;

namespace DevWinUI;

/// <summary>
/// Clamps long content to a number of lines and offers an expander only when the content overflows.
/// The last visible line fades into the surface while more content is available.
/// </summary>
public sealed partial class ShowMore : ContentControl
{
    private const double AnimationDuration = 250d;
    private const double VeilLines = 1.5d;

    private FrameworkElement? _region;
    private ScrollViewer? _scroller;
    private ContentPresenter? _content;
    private Rectangle? _veil;
    private ToggleButton? _toggle;
    private TextBlock? _toggleLabel;
    private RotateTransform? _chevronRotate;
    private TextBlock? _probe;

    private double _height = double.NaN;
    private double _opacity;
    private double _angle;
    private double _fromHeight;
    private double _fromOpacity;
    private double _fromAngle;
    private double _toHeight = double.NaN;
    private double _toOpacity;
    private double _toAngle;
    private long _startTick;
    private bool _animating;
    private bool _scrollable;

    public ShowMore()
    {
        DefaultStyleKey = typeof(ShowMore);
        Unloaded += OnUnloaded;
    }

    public event EventHandler<bool>? ExpandedChanged;

    protected override void OnApplyTemplate()
    {
        Detach();
        base.OnApplyTemplate();

        _region = GetTemplateChild("PART_Region") as FrameworkElement;
        _scroller = GetTemplateChild("PART_Scroller") as ScrollViewer;
        _content = GetTemplateChild("PART_Content") as ContentPresenter;
        _veil = GetTemplateChild("PART_Veil") as Rectangle;
        _toggle = GetTemplateChild("PART_Toggle") as ToggleButton;
        _toggleLabel = GetTemplateChild("PART_ToggleLabel") as TextBlock;
        _chevronRotate = GetTemplateChild("PART_ChevronRotate") as RotateTransform;

        if (_region is null || _scroller is null || _content is null || _veil is null || _toggle is null)
        {
            return;
        }

        _content.SizeChanged += OnContentSizeChanged;
        _scroller.ViewChanged += OnScrollerViewChanged;
        AutomationProperties.GetControlledPeers(_toggle)?.Add(_region);

        UpdateToggleLabel();
        Update(animate: false);
    }

    private void Detach()
    {
        EndAnimation();

        if (_content is not null)
        {
            _content.SizeChanged -= OnContentSizeChanged;
        }

        if (_scroller is not null)
        {
            _scroller.ViewChanged -= OnScrollerViewChanged;
        }

        _height = double.NaN;
        _toHeight = double.NaN;
        _scrollable = false;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_animating)
        {
            EndAnimation();
            Apply(_toHeight, _toOpacity, _toAngle);
        }
    }

    private void OnContentSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Update(animate: false);
    }

    private void OnScrollerViewChanged(object? sender, ScrollViewerViewChangedEventArgs e)
    {
        if (!_scrollable && _scroller is not null && _scroller.VerticalOffset > 0)
        {
            _scroller.ChangeView(null, 0, null, disableAnimation: true);
        }
    }

    private void OnIsExpandedChanged(bool expanded)
    {
        UpdateToggleLabel();
        Update(animate: true);

        if (!expanded)
        {
            _scroller?.ChangeView(null, 0, null, disableAnimation: true);
        }

        ExpandedChanged?.Invoke(this, expanded);
    }

    private void Update(bool animate)
    {
        if (_region is null || _scroller is null || _content is null || _veil is null || _toggle is null)
        {
            return;
        }

        var lineHeight = MeasureLineHeight(_content);
        var full = _content.ActualHeight;

        if (full <= 0)
        {
            EndAnimation();
            _height = double.NaN;
            _toHeight = double.NaN;
            _region.Height = double.NaN;
            _region.MaxHeight = lineHeight * Math.Max(1, Lines);
            _toggle.Visibility = Visibility.Collapsed;
            return;
        }

        var collapsed = Math.Min(lineHeight * Math.Max(1, Lines), full);
        var expandable = full - collapsed > 1;
        var expanded = expandable && IsExpanded;
        var expandedHeight = Math.Min(full, Math.Max(MaxExpandedHeight, collapsed));
        var scrollable = expanded && full > expandedHeight;

        _region.MaxHeight = double.PositiveInfinity;
        _veil.Height = Math.Min(lineHeight * VeilLines, collapsed);
        _toggle.Visibility = expandable ? Visibility.Visible : Visibility.Collapsed;
        ApplyScrollable(scrollable);

        var height = expanded ? expandedHeight : collapsed;
        var opacity = expandable && (!expanded || scrollable) ? 1d : 0d;
        var angle = IsExpanded ? 180d : 0d;
        Transition(height, opacity, angle, animate);
    }

    private void ApplyScrollable(bool scrollable)
    {
        if (_scroller is null || _scrollable == scrollable)
        {
            return;
        }

        _scrollable = scrollable;
        _scroller.VerticalScrollMode = scrollable ? ScrollMode.Enabled : ScrollMode.Disabled;
        _scroller.VerticalScrollBarVisibility = scrollable ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden;
        _scroller.IsTabStop = scrollable;

        if (scrollable)
        {
            AutomationProperties.SetLandmarkType(_scroller, AutomationLandmarkType.Custom);
            AutomationProperties.SetLocalizedLandmarkType(_scroller, "region");
            UpdateScrollerName();
        }
        else
        {
            _scroller.ClearValue(AutomationProperties.NameProperty);
            _scroller.ClearValue(AutomationProperties.LandmarkTypeProperty);
            _scroller.ClearValue(AutomationProperties.LocalizedLandmarkTypeProperty);
        }
    }

    private void UpdateScrollerName()
    {
        if (_scrollable && _scroller is not null)
        {
            AutomationProperties.SetName(_scroller, Label);
        }
    }

    private void UpdateToggleLabel()
    {
        if (_toggleLabel is not null)
        {
            _toggleLabel.Text = IsExpanded ? LessLabel : MoreLabel;
        }
    }

    private double MeasureLineHeight(DependencyObject content)
    {
        var source = FindFirst<TextBlock>(content);
        var probe = _probe ??= new TextBlock { Text = "Ag", TextWrapping = TextWrapping.NoWrap };

        if (source is not null)
        {
            probe.FontFamily = source.FontFamily;
            probe.FontSize = source.FontSize;
            probe.FontWeight = source.FontWeight;
            probe.FontStyle = source.FontStyle;
            probe.LineHeight = source.LineHeight;
            probe.LineStackingStrategy = source.LineStackingStrategy;
        }
        else
        {
            probe.FontFamily = FontFamily;
            probe.FontSize = FontSize;
            probe.FontWeight = FontWeight;
            probe.FontStyle = FontStyle;
            probe.LineHeight = 0;
            probe.LineStackingStrategy = LineStackingStrategy.MaxHeight;
        }

        probe.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        var height = probe.DesiredSize.Height;
        return height > 0 ? height : FontSize * 1.5;
    }

    private static T? FindFirst<T>(DependencyObject root) where T : DependencyObject
    {
        var count = VisualTreeHelper.GetChildrenCount(root);
        for (var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            if (child is T match)
            {
                return match;
            }

            var nested = FindFirst<T>(child);
            if (nested is not null)
            {
                return nested;
            }
        }

        return null;
    }

    private void Transition(double height, double opacity, double angle, bool animate)
    {
        var unchanged = Math.Abs(height - (_animating ? _toHeight : _height)) < 0.5
            && opacity == (_animating ? _toOpacity : _opacity)
            && angle == (_animating ? _toAngle : _angle);

        if (unchanged)
        {
            return;
        }

        _toHeight = height;
        _toOpacity = opacity;
        _toAngle = angle;

        if (!animate || double.IsNaN(_height))
        {
            EndAnimation();
            Apply(height, opacity, angle);
            return;
        }

        _fromHeight = _height;
        _fromOpacity = _opacity;
        _fromAngle = _angle;
        _startTick = Environment.TickCount64;

        if (!_animating)
        {
            _animating = true;
            CompositionTarget.Rendering += OnRendering;
        }
    }

    private void OnRendering(object? sender, object e)
    {
        var progress = Math.Min(1d, (Environment.TickCount64 - _startTick) / AnimationDuration);
        var eased = 1d - Math.Pow(1d - progress, 4d);

        Apply(
            _fromHeight + (_toHeight - _fromHeight) * eased,
            _fromOpacity + (_toOpacity - _fromOpacity) * eased,
            _fromAngle + (_toAngle - _fromAngle) * eased);

        if (progress >= 1d)
        {
            EndAnimation();
        }
    }

    private void EndAnimation()
    {
        if (_animating)
        {
            _animating = false;
            CompositionTarget.Rendering -= OnRendering;
        }
    }

    private void Apply(double height, double opacity, double angle)
    {
        if (_region is null || _veil is null)
        {
            return;
        }

        _height = height;
        _opacity = opacity;
        _angle = angle;

        _region.Height = height;
        _veil.Opacity = opacity;

        if (_chevronRotate is not null)
        {
            _chevronRotate.Angle = angle;
        }
    }
}
