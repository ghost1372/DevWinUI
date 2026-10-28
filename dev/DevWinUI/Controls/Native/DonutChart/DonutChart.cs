using System.Collections.Specialized;
using System.Globalization;
using Microsoft.UI.Xaml.Automation;
using Color = Windows.UI.Color;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

[TemplatePart(Name = PlotPartName, Type = typeof(FrameworkElement))]
[TemplatePart(Name = CanvasPartName, Type = typeof(Canvas))]
[TemplatePart(Name = TotalPartName, Type = typeof(TextBlock))]
[TemplatePart(Name = CaptionPartName, Type = typeof(TextBlock))]
[TemplatePart(Name = LegendPartName, Type = typeof(Panel))]
public sealed partial class DonutChart : Control
{
    private const string PlotPartName = "PART_Plot";
    private const string CanvasPartName = "PART_Canvas";
    private const string TotalPartName = "PART_Total";
    private const string CaptionPartName = "PART_Caption";
    private const string LegendPartName = "PART_Legend";

    private const double Tau = Math.PI * 2;
    private const double LiftDistance = 6;
    private const double DimmedOpacity = 0.35;
    private static readonly TimeSpan SweepDuration = TimeSpan.FromMilliseconds(1100);

    private static readonly Color[] Palette =
    {
        Microsoft.UI.ColorHelper.FromArgb(255, 0x3B, 0x82, 0xF6),
        Microsoft.UI.ColorHelper.FromArgb(255, 0x10, 0xB9, 0x81),
        Microsoft.UI.ColorHelper.FromArgb(255, 0xF5, 0x9E, 0x0B),
        Microsoft.UI.ColorHelper.FromArgb(255, 0xEF, 0x44, 0x44),
        Microsoft.UI.ColorHelper.FromArgb(255, 0x8B, 0x5C, 0xF6),
        Microsoft.UI.ColorHelper.FromArgb(255, 0x06, 0xB6, 0xD4),
        Microsoft.UI.ColorHelper.FromArgb(255, 0xEC, 0x48, 0x99),
        Microsoft.UI.ColorHelper.FromArgb(255, 0x84, 0xCC, 0x16),
    };

    private readonly List<DonutSlice> _slices = new();
    private Canvas? _canvas;
    private TextBlock? _totalText;
    private TextBlock? _captionText;
    private Panel? _legendPanel;
    private Storyboard? _sweep;
    private INotifyCollectionChanged? _observed;
    private bool _isLoaded;
    private bool _hasFraction;
    private double _total;
    private int _hoverIndex = -1;

    public DonutChart()
    {
        DefaultStyleKey = typeof(DonutChart);
        AutomationProperties.SetName(this, "Donut chart");

        Loaded += (_, _) =>
        {
            _isLoaded = true;
            HookData();
            Rebuild();
        };
        Unloaded += (_, _) =>
        {
            _isLoaded = false;
            _sweep?.Stop();
            HookData();
        };
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _sweep?.Stop();
        _canvas = GetTemplateChild(CanvasPartName) as Canvas;
        _totalText = GetTemplateChild(TotalPartName) as TextBlock;
        _captionText = GetTemplateChild(CaptionPartName) as TextBlock;
        _legendPanel = GetTemplateChild(LegendPartName) as Panel;

        if (GetTemplateChild(PlotPartName) is FrameworkElement plot)
        {
            plot.Margin = new Thickness(LiftDistance);
        }

        ApplyGeometry();
        Rebuild();
    }

    private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var chart = (DonutChart)d;
        chart.HookData();
        chart.Rebuild();
    }

    private static void OnGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((DonutChart)d).ApplyGeometry();

    private static void OnShowLegendChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((DonutChart)d).ApplyLegendVisibility();

    private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((DonutChart)d).Render();

    private void HookData()
    {
        if (_observed is not null)
        {
            _observed.CollectionChanged -= OnCollectionChanged;
            _observed = null;
        }

        if (_isLoaded && Data is INotifyCollectionChanged observable)
        {
            _observed = observable;
            _observed.CollectionChanged += OnCollectionChanged;
        }
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => Rebuild();

    private void ApplyLegendVisibility()
    {
        if (_legendPanel is not null)
        {
            _legendPanel.Visibility = ShowLegend ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void Rebuild()
    {
        if (!_isLoaded || _canvas is null || _legendPanel is null)
        {
            return;
        }

        _sweep?.Stop();
        _hoverIndex = -1;
        _canvas.Children.Clear();
        _legendPanel.Children.Clear();
        _slices.Clear();

        var items = (Data ?? Enumerable.Empty<DonutChartDatum>()).ToList();
        _total = items.Sum(i => Math.Max(0, i.Value));
        _hasFraction = items.Any(i => i.Value % 1 != 0);

        double running = 0;
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            double value = Math.Max(0, item.Value);
            double start = _total > 0 ? running / _total * Tau : 0;
            double end = _total > 0 ? (running + value) / _total * Tau : 0;
            running += value;

            double mid = (start + end) / 2;
            var lift = new Vector3((float)(Math.Sin(mid) * LiftDistance), (float)(-Math.Cos(mid) * LiftDistance), 0);
            var color = item.Color ?? Palette[i % Palette.Length];
            int index = i;

            var path = new Path
            {
                Fill = new SolidColorBrush(color),
                OpacityTransition = new ScalarTransition(),
                TranslationTransition = new Vector3Transition(),
            };
            path.PointerEntered += (_, _) => SetHover(index);
            path.PointerExited += (_, _) => SetHover(-1);
            _canvas.Children.Add(path);

            var legendItem = CreateLegendItem(index, item.Label, Format(item.Value), color);
            _legendPanel.Children.Add(legendItem);

            _slices.Add(new DonutSlice
            {
                Path = path,
                LegendItem = legendItem,
                Value = value,
                StartAngle = start,
                EndAngle = end,
                Lift = lift,
            });
        }

        ApplyLegendVisibility();
        StartSweep();
    }

    private Button CreateLegendItem(int index, string label, string value, Color color)
    {
        var grid = new Grid { ColumnSpacing = 10, MinWidth = 160 };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var swatch = new Microsoft.UI.Xaml.Shapes.Ellipse
        {
            Width = 10,
            Height = 10,
            Fill = new SolidColorBrush(color),
            VerticalAlignment = VerticalAlignment.Center,
        };
        var name = new TextBlock { Text = label, VerticalAlignment = VerticalAlignment.Center };
        var amount = new TextBlock
        {
            Text = value,
            Opacity = 0.7,
            VerticalAlignment = VerticalAlignment.Center,
        };
        Grid.SetColumn(name, 1);
        Grid.SetColumn(amount, 2);
        grid.Children.Add(swatch);
        grid.Children.Add(name);
        grid.Children.Add(amount);

        var button = new Button
        {
            Content = grid,
            Background = new SolidColorBrush(Colors.Transparent),
            BorderThickness = new Thickness(0),
            Padding = new Thickness(8, 4, 8, 4),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
        };
        AutomationProperties.SetName(button, $"{label}: {value}");

        button.PointerEntered += (_, _) => SetHover(index);
        button.PointerExited += (_, _) => SetHover(-1);
        button.GotFocus += (_, _) => SetHover(index);
        button.LostFocus += (_, _) =>
        {
            if (_hoverIndex == index)
            {
                SetHover(-1);
            }
        };
        return button;
    }

    private void StartSweep()
    {
        _sweep?.Stop();

        if (_slices.Count == 0)
        {
            Progress = 1;
            Render();
            return;
        }

        Progress = 0;
        Render();

        var animation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = new Duration(SweepDuration),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            EnableDependentAnimation = true,
        };
        Storyboard.SetTarget(animation, this);
        Storyboard.SetTargetProperty(animation, nameof(Progress));

        _sweep = new Storyboard();
        _sweep.Children.Add(animation);
        _sweep.Begin();
    }

    private void SetHover(int index)
    {
        if (_hoverIndex == index)
        {
            return;
        }

        _hoverIndex = index;
        for (int i = 0; i < _slices.Count; i++)
        {
            var slice = _slices[i];
            bool active = i == index;
            bool dimmed = index >= 0 && !active;
            slice.Path.Opacity = dimmed ? DimmedOpacity : 1;
            slice.Path.Translation = active ? slice.Lift : Vector3.Zero;
            slice.LegendItem.Opacity = dimmed ? 0.6 : 1;
        }
    }

    private void ApplyGeometry()
    {
        double size = Math.Max(1, Diameter);

        if (_totalText is not null)
        {
            _totalText.FontSize = Math.Max(12, size * 0.13);
        }

        if (_captionText is not null)
        {
            _captionText.FontSize = Math.Max(10, size * 0.055);
        }

        Render();
    }

    private void Render()
    {
        double progress = Math.Clamp(Progress, 0, 1);
        double sweepAngle = progress * Tau;
        double outer = Math.Max(1, Diameter) / 2;
        double inner = Math.Max(0, outer - Math.Clamp(Thickness, 0, outer));
        double gap = Math.Max(0, Gap);

        foreach (var slice in _slices)
        {
            if (slice.Value <= 0 || slice.StartAngle >= sweepAngle)
            {
                slice.Path.Data = null;
                continue;
            }

            bool clipped = sweepAngle < slice.EndAngle;
            double a0 = slice.StartAngle + gap / 2;
            double a1 = clipped ? sweepAngle : slice.EndAngle - gap / 2;

            slice.Path.Data = a1 - a0 > 0.001
                ? BuildSliceGeometry(outer, outer, inner, outer, a0, a1, SliceCornerRadius)
                : null;
        }

        if (_totalText is not null)
        {
            _totalText.Text = Format(_total * progress);
        }
    }

    private string Format(double value)
        => value.ToString(_hasFraction ? "N2" : "N0", CultureInfo.CurrentCulture);

    private static Geometry BuildSliceGeometry(
        double cx, double cy, double rIn, double rOut, double a0, double a1, double corner)
    {
        static Point P(double cx, double cy, double r, double a)
            => new(cx + r * Math.Sin(a), cy - r * Math.Cos(a));

        static ArcSegment Arc(Point to, double radius, SweepDirection dir, bool large)
            => new()
            {
                Point = to,
                Size = new Size(radius, radius),
                SweepDirection = dir,
                IsLargeArc = large,
            };

        double span = a1 - a0;
        double thickness = rOut - rIn;

        double c = Math.Max(0, Math.Min(corner, thickness / 2 - 0.01));
        if (c > 0)
        {
            double s = Math.Sin(Math.Min(span / 2, Math.PI / 2));
            double byOuter = rOut * s / (1 + s);
            double byInner = s < 1 ? rIn * s / (1 - s) : double.MaxValue;
            c = Math.Min(c, Math.Min(byOuter, byInner));
        }

        var figure = new PathFigure { IsClosed = true, IsFilled = true };

        if (c < 0.5)
        {
            bool large = span > Math.PI;
            figure.StartPoint = P(cx, cy, rOut, a0);
            figure.Segments.Add(Arc(P(cx, cy, rOut, a1), rOut, SweepDirection.Clockwise, large));
            figure.Segments.Add(new LineSegment { Point = P(cx, cy, rIn, a1) });
            if (rIn > 0)
            {
                figure.Segments.Add(Arc(P(cx, cy, rIn, a0), rIn, SweepDirection.Counterclockwise, large));
            }
        }
        else
        {
            double dOut = Math.Asin(Math.Min(1, c / (rOut - c)));
            double dIn = Math.Asin(Math.Min(1, c / (rIn + c)));
            double eOut = Math.Sqrt((rOut - c) * (rOut - c) - c * c);
            double eIn = Math.Sqrt((rIn + c) * (rIn + c) - c * c);
            bool largeOut = span - 2 * dOut > Math.PI;
            bool largeIn = span - 2 * dIn > Math.PI;

            figure.StartPoint = P(cx, cy, rOut, a0 + dOut);
            figure.Segments.Add(Arc(P(cx, cy, rOut, a1 - dOut), rOut, SweepDirection.Clockwise, largeOut));
            figure.Segments.Add(Arc(P(cx, cy, eOut, a1), c, SweepDirection.Clockwise, false));
            figure.Segments.Add(new LineSegment { Point = P(cx, cy, eIn, a1) });
            figure.Segments.Add(Arc(P(cx, cy, rIn, a1 - dIn), c, SweepDirection.Clockwise, false));
            figure.Segments.Add(Arc(P(cx, cy, rIn, a0 + dIn), rIn, SweepDirection.Counterclockwise, largeIn));
            figure.Segments.Add(Arc(P(cx, cy, eIn, a0), c, SweepDirection.Clockwise, false));
            figure.Segments.Add(new LineSegment { Point = P(cx, cy, eOut, a0) });
            figure.Segments.Add(Arc(P(cx, cy, rOut, a0 + dOut), c, SweepDirection.Clockwise, false));
        }

        var geometry = new PathGeometry();
        geometry.Figures.Add(figure);
        return geometry;
    }
}
