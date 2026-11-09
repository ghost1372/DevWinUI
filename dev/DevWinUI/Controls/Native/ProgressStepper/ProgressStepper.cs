using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Shapes;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

[TemplatePart(Name = HostPartName, Type = typeof(Border))]
public sealed partial class ProgressStepper : Control
{
    private const string HostPartName = "PART_Host";
    private const double MarkerSize = 24;
    private const double ChildMarkerSize = 16;
    private const double RailGap = 4;

    public IList<ProgressStep>? Steps
    {
        get { return (IList<ProgressStep>?)GetValue(StepsProperty); }
        set { SetValue(StepsProperty, value); }
    }

    public static readonly DependencyProperty StepsProperty =
        DependencyProperty.Register(nameof(Steps), typeof(IList<ProgressStep>), typeof(ProgressStepper), new PropertyMetadata(null, StepsChanged));

    public int Current
    {
        get { return (int)GetValue(CurrentProperty); }
        set { SetValue(CurrentProperty, value); }
    }

    public static readonly DependencyProperty CurrentProperty =
        DependencyProperty.Register(nameof(Current), typeof(int), typeof(ProgressStepper), new PropertyMetadata(0, ProgressChanged));

    public int CurrentChild
    {
        get { return (int)GetValue(CurrentChildProperty); }
        set { SetValue(CurrentChildProperty, value); }
    }

    public static readonly DependencyProperty CurrentChildProperty =
        DependencyProperty.Register(nameof(CurrentChild), typeof(int), typeof(ProgressStepper), new PropertyMetadata(0, ProgressChanged));

    public ProgressStepperStepperAlign Align
    {
        get { return (ProgressStepperStepperAlign)GetValue(AlignProperty); }
        set { SetValue(AlignProperty, value); }
    }

    public static readonly DependencyProperty AlignProperty =
        DependencyProperty.Register(nameof(Align), typeof(ProgressStepperStepperAlign), typeof(ProgressStepper), new PropertyMetadata(ProgressStepperStepperAlign.Right, LayoutChanged));

    public bool ShowStepLabel
    {
        get { return (bool)GetValue(ShowStepLabelProperty); }
        set { SetValue(ShowStepLabelProperty, value); }
    }

    public static readonly DependencyProperty ShowStepLabelProperty =
        DependencyProperty.Register(nameof(ShowStepLabel), typeof(bool), typeof(ProgressStepper), new PropertyMetadata(true, LayoutChanged));

    public string StepLabelFormat
    {
        get { return (string)GetValue(StepLabelFormatProperty); }
        set { SetValue(StepLabelFormatProperty, value); }
    }

    public static readonly DependencyProperty StepLabelFormatProperty =
        DependencyProperty.Register(nameof(StepLabelFormat), typeof(string), typeof(ProgressStepper), new PropertyMetadata("Step {0}", LayoutChanged));

    public DataTemplate? TitleTemplate
    {
        get { return (DataTemplate?)GetValue(TitleTemplateProperty); }
        set { SetValue(TitleTemplateProperty, value); }
    }

    public static readonly DependencyProperty TitleTemplateProperty =
        DependencyProperty.Register(nameof(TitleTemplate), typeof(DataTemplate), typeof(ProgressStepper), new PropertyMetadata(null, LayoutChanged));

    public DataTemplate? DescriptionTemplate
    {
        get { return (DataTemplate?)GetValue(DescriptionTemplateProperty); }
        set { SetValue(DescriptionTemplateProperty, value); }
    }

    public static readonly DependencyProperty DescriptionTemplateProperty =
        DependencyProperty.Register(nameof(DescriptionTemplate), typeof(DataTemplate), typeof(ProgressStepper), new PropertyMetadata(null, LayoutChanged));

    private static void StepsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is ProgressStepper control)
        {
            control.Build();
        }
    }

    private static void ProgressChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is ProgressStepper control)
        {
            control.OnProgressChanged();
        }
    }

    private static void LayoutChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is ProgressStepper control)
        {
            control.QueueBuild();
        }
    }

    private readonly List<ProgressStepperStepView> _views = new();
    private readonly List<ProgressStep> _tracked = new();
    private readonly List<INotifyCollectionChanged> _trackedCollections = new();
    private Border? _host;
    private bool _built;
    private bool _isLoaded;
    private bool _batching;

    public ProgressStepper()
    {
        DefaultStyleKey = typeof(ProgressStepper);
        Loaded += (_, _) =>
        {
            _isLoaded = true;
            Build();
        };
        Unloaded += (_, _) =>
        {
            _isLoaded = false;
            StopAll();
            Untrack();
        };
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _host = GetTemplateChild(HostPartName) as Border;
        Build();
    }

    public bool Next()
    {
        var steps = Steps;
        if (steps is null || Current >= steps.Count) return false;

        var active = steps[Current];
        if (CurrentChild < active.Children.Count - 1)
        {
            MoveTo(Current, CurrentChild + 1);
        }
        else
        {
            MoveTo(Current + 1, 0);
        }
        return true;
    }

    public bool Previous()
    {
        if (CurrentChild > 0)
        {
            MoveTo(Current, CurrentChild - 1);
            return true;
        }

        var steps = Steps;
        if (steps is null) return false;

        int prev = Math.Min(Current, steps.Count) - 1;
        if (prev < 0) return false;

        MoveTo(prev, Math.Max(steps[prev].Children.Count - 1, 0));
        return true;
    }

    public void Reset() => MoveTo(0, 0);

    private void MoveTo(int current, int child)
    {
        _batching = true;
        try
        {
            Current = current;
            CurrentChild = child;
        }
        finally
        {
            _batching = false;
        }

        if (_built) Update(animate: true);
    }

    private void OnProgressChanged()
    {
        if (_built && !_batching) Update(animate: true);
    }

    private void OnCollectionChanged(object? s, NotifyCollectionChangedEventArgs e) => QueueBuild();

    private void OnStepPropertyChanged(object? s, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ProgressStep.Id)) return;

        if (e.PropertyName == nameof(ProgressStep.Status) && _built)
        {
            Update(animate: true);
        }
        else
        {
            QueueBuild();
        }
    }

    private void Track()
    {
        Untrack();
        if (!_isLoaded) return;

        var steps = Steps;
        if (steps is null) return;

        if (steps is INotifyCollectionChanged outer)
        {
            outer.CollectionChanged += OnCollectionChanged;
            _trackedCollections.Add(outer);
        }

        foreach (var step in steps)
        {
            TrackStep(step);

            if (step.Children is INotifyCollectionChanged inner)
            {
                inner.CollectionChanged += OnCollectionChanged;
                _trackedCollections.Add(inner);
            }

            foreach (var child in step.Children)
            {
                TrackStep(child);
            }
        }
    }

    private void TrackStep(ProgressStep step)
    {
        step.PropertyChanged += OnStepPropertyChanged;
        _tracked.Add(step);
    }

    private void Untrack()
    {
        foreach (var step in _tracked) step.PropertyChanged -= OnStepPropertyChanged;
        foreach (var c in _trackedCollections) c.CollectionChanged -= OnCollectionChanged;
        _tracked.Clear();
        _trackedCollections.Clear();
    }

    private void StopAll()
    {
        foreach (var v in _views)
        {
            v.Marker.Spin?.Stop();
            v.HeightSb?.Stop();
            foreach (var c in v.Children) c.Marker.Spin?.Stop();
        }
    }

    private void Build()
    {
        StopAll();
        _views.Clear();
        _built = false;
        Track();

        if (_host is null) return;

        var root = new StackPanel();
        var steps = Steps;
        if (steps != null)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                var v = CreateStepView(steps[i], i, isLast: i == steps.Count - 1);
                _views.Add(v);
                root.Children.Add(v.Row);
            }
        }

        _host.Child = root;
        _built = true;
        Update(animate: false);
    }

    private string? FormatStepLabel(int index)
    {
        var format = StepLabelFormat;
        if (string.IsNullOrEmpty(format)) return null;

        try
        {
            return string.Format(CultureInfo.CurrentCulture, format, index + 1);
        }
        catch (FormatException)
        {
            return format;
        }
    }

    private ProgressStepperStepView CreateStepView(ProgressStep step, int index, bool isLast)
    {
        bool right = Align == ProgressStepperStepperAlign.Right;
        var v = new ProgressStepperStepView { Step = step, Index = index };

        var row = new Grid();
        row.ColumnDefinitions.Add(new ColumnDefinition
        {
            Width = right ? new GridLength(1, GridUnitType.Star) : GridLength.Auto
        });
        row.ColumnDefinitions.Add(new ColumnDefinition
        {
            Width = right ? GridLength.Auto : new GridLength(1, GridUnitType.Star)
        });
        int labelCol = right ? 0 : 1, railCol = right ? 1 : 0;

        var railCell = new Grid
        {
            Width = MarkerSize,
            Margin = right ? new Thickness(12, 0, 0, 0) : new Thickness(0, 0, 12, 0)
        };
        v.Marker.Host = new Grid
        {
            Width = MarkerSize,
            Height = MarkerSize,
            VerticalAlignment = VerticalAlignment.Top
        };
        railCell.Children.Add(v.Marker.Host);
        if (!isLast)
        {
            v.Rail = new Rectangle
            {
                Width = 2,
                RadiusX = 1,
                RadiusY = 1,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, MarkerSize + RailGap, 0, RailGap)
            };
            railCell.Children.Add(v.Rail);
        }
        Grid.SetColumn(railCell, railCol);
        row.Children.Add(railCell);

        var align = right ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        var textAlign = right ? TextAlignment.Right : TextAlignment.Left;
        var label = new StackPanel
        {
            HorizontalAlignment = align,
            Padding = new Thickness(0, 2, 0, isLast ? 0 : 24)
        };

        if (TitleTemplate != null)
        {
            v.Title = new ContentControl
            {
                Content = step,
                ContentTemplate = TitleTemplate,
                HorizontalAlignment = align
            };
        }
        else
        {
            v.Title = new TextBlock
            {
                Text = step.Title,
                FontSize = 14,
                TextAlignment = textAlign,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = align
            };
        }
        label.Children.Add(v.Title);

        if (DescriptionTemplate != null)
        {
            v.Description = new ContentControl
            {
                Content = step,
                ContentTemplate = DescriptionTemplate,
                HorizontalAlignment = align
            };
        }
        else
        {
            string? desc = step.Description ?? (ShowStepLabel ? FormatStepLabel(index) : null);
            if (desc != null)
            {
                v.Description = new TextBlock
                {
                    Text = desc,
                    FontSize = 12,
                    TextAlignment = textAlign,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = align,
                    Foreground = TertiaryForeground
                };
            }
        }
        if (v.Description != null) label.Children.Add(v.Description);

        v.ChildrenPanel = new StackPanel { Spacing = 8, Margin = new Thickness(0, 8, 0, 0), HorizontalAlignment = align };
        for (int c = 0; c < step.Children.Count; c++)
        {
            var cv = new ProgressStepperChildView { Step = step.Children[c], Index = c };
            cv.Marker.Host = new Grid { Width = ChildMarkerSize, Height = ChildMarkerSize, VerticalAlignment = VerticalAlignment.Center };
            cv.Text = new TextBlock
            {
                Text = cv.Step.Title,
                FontSize = 13,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };
            var childRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, HorizontalAlignment = align };
            if (right) { childRow.Children.Add(cv.Text); childRow.Children.Add(cv.Marker.Host); }
            else { childRow.Children.Add(cv.Marker.Host); childRow.Children.Add(cv.Text); }
            v.ChildrenPanel.Children.Add(childRow);
            v.Children.Add(cv);
        }

        v.ChildrenWrap = new Grid { Visibility = Visibility.Collapsed, Children = { v.ChildrenPanel } };
        v.ChildrenWrap.SizeChanged += (s, e) =>
            ((Grid)s).Clip = new RectangleGeometry { Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height) };
        label.Children.Add(v.ChildrenWrap);

        Grid.SetColumn(label, labelCol);
        row.Children.Add(label);

        v.Row = row;
        return v;
    }

    private ProgressStepperStepStatus Resolve(ProgressStepperStepView v)
    {
        if (v.Step.Status is { } s) return s;
        int cur = Current;
        return v.Index < cur ? ProgressStepperStepStatus.Completed
             : v.Index == cur ? ProgressStepperStepStatus.InProgress
             : ProgressStepperStepStatus.NotStarted;
    }

    private ProgressStepperStepStatus ResolveChild(ProgressStepperChildView c, ProgressStepperStepStatus parent)
    {
        if (c.Step.Status is { } s) return s;
        if (parent == ProgressStepperStepStatus.Completed) return ProgressStepperStepStatus.Completed;
        if (parent != ProgressStepperStepStatus.InProgress) return ProgressStepperStepStatus.NotStarted;
        int cc = CurrentChild;
        return c.Index < cc ? ProgressStepperStepStatus.Completed
             : c.Index == cc ? ProgressStepperStepStatus.InProgress
             : ProgressStepperStepStatus.NotStarted;
    }

    private static string StateText(ProgressStepperStepStatus s) => s switch
    {
        ProgressStepperStepStatus.Completed => "completed",
        ProgressStepperStepStatus.InProgress => "in progress",
        ProgressStepperStepStatus.Skipped => "skipped",
        _ => "not started"
    };

    private void Update(bool animate)
    {
        foreach (var v in _views)
        {
            var st = Resolve(v);

            if (v.Marker.Last != st)
                ApplyMarker(v.Marker, st, MarkerSize, animate && v.Marker.Last != null);

            if (v.Title is TextBlock tb)
            {
                tb.Foreground = st == ProgressStepperStepStatus.NotStarted || st == ProgressStepperStepStatus.Skipped ? SecondaryForeground : PrimaryForeground;
                tb.FontWeight = st == ProgressStepperStepStatus.InProgress
                    ? Microsoft.UI.Text.FontWeights.SemiBold
                    : Microsoft.UI.Text.FontWeights.Normal;
            }

            if (v.Rail != null)
                v.Rail.Fill = st == ProgressStepperStepStatus.Completed ? SuccessBrush : RailBrush;

            foreach (var c in v.Children)
            {
                var cst = ResolveChild(c, st);
                if (c.Marker.Last != cst)
                    ApplyMarker(c.Marker, cst, ChildMarkerSize, animate && c.Marker.Last != null);
                c.Text.Foreground = cst == ProgressStepperStepStatus.InProgress ? PrimaryForeground : SecondaryForeground;
            }

            bool expanded = st == ProgressStepperStepStatus.InProgress && v.Children.Count > 0;
            if (v.Expanded != expanded)
            {
                bool first = v.Expanded == null;
                v.Expanded = expanded;
                SetExpanded(v, expanded, animate && !first);
            }

            string title = v.Title is TextBlock t ? t.Text : v.Step.Title;
            AutomationProperties.SetName(v.Row, $"{title}, {StateText(st)}");
            AutomationProperties.SetItemStatus(v.Row, st == ProgressStepperStepStatus.InProgress ? "current" : string.Empty);
        }
    }

    private void ApplyMarker(ProgressStepperMarkerView m, ProgressStepperStepStatus st, double size, bool animate)
    {
        m.Spin?.Stop();
        m.Spin = null;
        m.Host.Children.Clear();
        m.Last = st;

        var g = new Grid { Width = 24, Height = 24 };

        switch (st)
        {
            case ProgressStepperStepStatus.Completed:
                {
                    g.Children.Add(new Ellipse
                    {
                        Stroke = SuccessBrush,
                        StrokeThickness = 2,
                        Fill = SuccessFillBrush
                    });

                    var fig = new PathFigure { StartPoint = new Point(7, 12.5) };
                    fig.Segments.Add(new LineSegment { Point = new Point(10.5, 16) });
                    fig.Segments.Add(new LineSegment { Point = new Point(17, 8.5) });
                    var geo = new PathGeometry();
                    geo.Figures.Add(fig);

                    var check = new Path
                    {
                        Data = geo,
                        Stroke = SuccessBrush,
                        StrokeThickness = 2,
                        StrokeLineJoin = PenLineJoin.Round,
                        StrokeStartLineCap = PenLineCap.Round,
                        StrokeEndLineCap = PenLineCap.Round,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top
                    };
                    g.Children.Add(check);

                    if (animate)
                    {
                        check.StrokeDashArray = new DoubleCollection { 8, 8 };
                        check.StrokeDashOffset = 8;
                        var a = new DoubleAnimation
                        {
                            From = 8,
                            To = 0,
                            Duration = TimeSpan.FromMilliseconds(350),
                            EnableDependentAnimation = true,
                            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(a, check);
                        Storyboard.SetTargetProperty(a, "StrokeDashOffset");
                        var sb = new Storyboard();
                        sb.Children.Add(a);
                        sb.Begin();
                    }
                    break;
                }

            case ProgressStepperStepStatus.InProgress:
                {
                    g.Children.Add(new Ellipse
                    {
                        Stroke = AccentTrackBrush,
                        StrokeThickness = 2
                    });

                    var fig = new PathFigure { StartPoint = new Point(12, 1) };
                    fig.Segments.Add(new ArcSegment
                    {
                        Point = new Point(23, 12),
                        Size = new Size(11, 11),
                        SweepDirection = SweepDirection.Clockwise
                    });
                    var geo = new PathGeometry();
                    geo.Figures.Add(fig);

                    var arc = new Path
                    {
                        Data = geo,
                        Stroke = AccentBrush,
                        StrokeThickness = 2,
                        StrokeStartLineCap = PenLineCap.Round,
                        StrokeEndLineCap = PenLineCap.Round,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top
                    };
                    var spinHost = new Grid
                    {
                        Width = 24,
                        Height = 24,
                        RenderTransformOrigin = new Point(0.5, 0.5),
                        Children = { arc }
                    };
                    var rot = new RotateTransform();
                    spinHost.RenderTransform = rot;
                    g.Children.Add(spinHost);

                    var a = new DoubleAnimation
                    {
                        From = 0,
                        To = 360,
                        Duration = TimeSpan.FromSeconds(0.9),
                        RepeatBehavior = RepeatBehavior.Forever
                    };
                    Storyboard.SetTarget(a, rot);
                    Storyboard.SetTargetProperty(a, "Angle");
                    var sb = new Storyboard();
                    sb.Children.Add(a);
                    m.Spin = sb;
                    sb.Begin();
                    break;
                }

            case ProgressStepperStepStatus.Skipped:
                {
                    g.Children.Add(new Ellipse
                    {
                        Stroke = TertiaryForeground,
                        StrokeThickness = 2
                    });
                    g.Children.Add(new Line
                    {
                        X1 = 8, Y1 = 12, X2 = 16, Y2 = 12,
                        Stroke = TertiaryForeground,
                        StrokeThickness = 2,
                        StrokeStartLineCap = PenLineCap.Round,
                        StrokeEndLineCap = PenLineCap.Round
                    });
                    break;
                }

            default:
                g.Children.Add(new Ellipse
                {
                    Stroke = TertiaryForeground,
                    StrokeThickness = 2,
                    StrokeDashArray = new DoubleCollection { 2, 2 }
                });
                break;
        }

        m.Host.Children.Add(new Viewbox { Width = size, Height = size, Child = g });
    }

    private void SetExpanded(ProgressStepperStepView v, bool expanded, bool animate)
    {
        var w = v.ChildrenWrap;
        v.HeightSb?.Stop();
        v.HeightSb = null;

        if (!animate)
        {
            w.Height = double.NaN;
            w.Opacity = 1;
            w.Visibility = expanded ? Visibility.Visible : Visibility.Collapsed;
            return;
        }

        v.ChildrenPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        double target = v.ChildrenPanel.DesiredSize.Height;
        double from = expanded ? 0 : w.ActualHeight;
        double to = expanded ? target : 0;

        w.Visibility = Visibility.Visible;
        w.Height = from;

        var ease = new CubicEase { EasingMode = EasingMode.EaseOut };
        var h = new DoubleAnimation
        {
            From = from,
            To = to,
            Duration = TimeSpan.FromMilliseconds(250),
            EnableDependentAnimation = true,
            EasingFunction = ease
        };
        Storyboard.SetTarget(h, w);
        Storyboard.SetTargetProperty(h, "Height");

        var o = new DoubleAnimation
        {
            From = expanded ? 0 : 1,
            To = expanded ? 1 : 0,
            Duration = TimeSpan.FromMilliseconds(250),
            EasingFunction = ease
        };
        Storyboard.SetTarget(o, w);
        Storyboard.SetTargetProperty(o, "Opacity");

        var sb = new Storyboard();
        sb.Children.Add(h);
        sb.Children.Add(o);
        sb.Completed += (_, _) =>
        {
            sb.Stop();
            w.Height = double.NaN;
            w.Opacity = 1;
            w.Visibility = expanded ? Visibility.Visible : Visibility.Collapsed;
        };
        v.HeightSb = sb;
        sb.Begin();
    }
}
