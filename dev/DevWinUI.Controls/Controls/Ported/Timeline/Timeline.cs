// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System.Globalization;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_AnnotationCanvas), Type = typeof(Canvas))]
[TemplatePart(Name = nameof(PART_TimelineCanvas), Type = typeof(Canvas))]
[TemplatePart(Name = nameof(PART_HeaderCanvas), Type = typeof(Canvas))]
public partial class Timeline : Control
{
    private const string PART_HeaderCanvas = "PART_HeaderCanvas";
    private const string PART_TimelineCanvas = "PART_TimelineCanvas";
    private const string PART_AnnotationCanvas = "PART_AnnotationCanvas";

    private Canvas headerCanvas;
    private Canvas timelineCanvas;
    private Canvas annotationCanvas;

    private readonly List<int> _tickHours = new();

    // Locale 24h/12h
    private readonly bool _is24h = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.Contains('H');

    // Visuals
    private readonly List<Line> _ticks = new();
    private readonly List<TextBlock> _majorTickBottomLabels = new(); // 00,06,12,18,24 (below)

    private readonly List<Border> _darkRects = new(); // up to 2 (wrap)
    private readonly List<Border> _lightRects = new(); // up to 2 (complement)

    private TextBlock _startEdgeLabel; // top-of-chart
    private TextBlock _endEdgeLabel;

    private Line _sunriseTick;
    private Line _sunsetTick;

    // Add/replace these constants (top of your class)
    private const int TickHourStep = 2; // <-- every 2 hours

    private StackPanel _sunrisePanel;  // icon + time (below chart)
    private StackPanel _sunsetPanel;

    public TimeSpan StartTime
    {
        get => (TimeSpan)GetValue(StartTimeProperty);
        set => SetValue(StartTimeProperty, value);
    }

    public static readonly DependencyProperty StartTimeProperty =
        DependencyProperty.Register(nameof(StartTime), typeof(TimeSpan), typeof(Timeline), new PropertyMetadata(defaultValue: new TimeSpan(22, 0, 0), OnTimeChanged));

    public TimeSpan EndTime
    {
        get => (TimeSpan)GetValue(EndTimeProperty);
        set => SetValue(EndTimeProperty, value);
    }

    public static readonly DependencyProperty EndTimeProperty =
        DependencyProperty.Register(nameof(EndTime), typeof(TimeSpan), typeof(Timeline), new PropertyMetadata(defaultValue: new TimeSpan(7, 0, 0), OnTimeChanged));

    public TimeSpan? Sunrise
    {
        get => (TimeSpan?)GetValue(SunriseProperty);
        set => SetValue(SunriseProperty, value);
    }

    public static readonly DependencyProperty SunriseProperty =
        DependencyProperty.Register(nameof(Sunrise), typeof(TimeSpan), typeof(Timeline), new PropertyMetadata(defaultValue: null, OnTimeChanged));

    public TimeSpan? Sunset
    {
        get => (TimeSpan?)GetValue(SunsetProperty);
        set => SetValue(SunsetProperty, value);
    }

    public static readonly DependencyProperty SunsetProperty =
        DependencyProperty.Register(nameof(Sunset), typeof(TimeSpan), typeof(Timeline), new PropertyMetadata(defaultValue: null, OnTimeChanged));
    private static void OnTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Timeline)d).Setup();
    }
    public Timeline()
    {
        DefaultStyleKey = typeof(Timeline);
        this.IsEnabledChanged += TimeLine_IsEnabledChanged;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        timelineCanvas = GetTemplateChild(PART_TimelineCanvas) as Canvas;
        headerCanvas = GetTemplateChild(PART_HeaderCanvas) as Canvas;
        annotationCanvas = GetTemplateChild(PART_AnnotationCanvas) as Canvas;

        // SizeChanged wiring here (as requested)
        headerCanvas.SizeChanged -= (_, __) => UpdateAll();
        headerCanvas.SizeChanged += (_, __) => UpdateAll();
        timelineCanvas.SizeChanged -= (_, __) => UpdateAll();
        timelineCanvas.SizeChanged += (_, __) => UpdateAll();
        annotationCanvas.SizeChanged -= (_, __) => UpdateAll();
        annotationCanvas.SizeChanged += (_, __) => UpdateAll();
        Setup();
        CheckEnabledState();
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new TimelineAutomationPeer(this);
    }
    private void TimeLine_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        CheckEnabledState();
    }

    private void CheckEnabledState()
    {
        if (IsEnabled)
        {
            this.Opacity = 1.0;
        }
        else
        {
            this.Opacity = 0.4;
        }
    }

    private void Setup()
    {
        if (timelineCanvas == null || headerCanvas == null || annotationCanvas == null)
            return;

        EnsureBands();
        EnsureTicks();
        EnsureStartEndEdgeLabels();
        EnsureSunriseSunsetTicks();
        EnsureSunPanels();
        EnsureMajorTickLabels();
        UpdateAll();
    }

    private void UpdateAll()
    {
        UpdateBandsLayout();
        UpdateTicksLayout();
        UpdateStartEndEdgeLabelsLayout();
        UpdateSunriseSunsetTicksLayout();
        UpdateSunPanelsLayout();
        UpdateMajorTickLabelsLayout();
        AutomationProperties.SetHelpText(this, $"Start={StartTime};End={EndTime};Sunrise={Sunrise};Sunset={Sunset}");
    }

    // ===== Ticks =====
    private void EnsureTicks()
    {
        if (_ticks.Count > 0)
        {
            return;
        }

        _tickHours.Clear();

        // Build ticks at 0,2,4,...,24 but skip the first/last MAJOR ticks (0 and 24)
        for (int hour = 0; hour <= 24; hour += TickHourStep)
        {
            bool isMajor = hour % 6 == 0;
            if (isMajor && (hour == 0 || hour == 24))
            {
                continue; // skip first/last major ticks
            }

            var line = new Line
            {
                Style = (Style)Application.Current.Resources[isMajor ? "MajorHourTickStyle" : "HourTickStyle"],
            };

            Canvas.SetZIndex(line, 0); // above bands (adjust if needed)

            _ticks.Add(line);
            _tickHours.Add(hour);

            // If you actually want these IN the chart, use TimeLineCanvas instead:
            annotationCanvas.Children.Add(line); // or TimeLineCanvas.Children.Add(line);
        }
    }

    private void UpdateTicksLayout()
    {
        double w = timelineCanvas.ActualWidth;
        double h = timelineCanvas.ActualHeight; // keeping your offset
        if (w <= 0 || h <= 0)
        {
            return;
        }

        double minorLen = h * 0.1;
        double majorLen = h * 0.2;

        for (int i = 0; i < _ticks.Count; i++)
        {
            int hour = _tickHours[i];
            double x = Math.Round((hour / 24.0) * w);

            var line = _ticks[i];
            double len = (hour % 6 == 0) ? majorLen : minorLen;

            line.X1 = x;
            line.Y1 = 0;
            line.X2 = x;
            line.Y2 = len;
        }
    }

    // ===== Bands (Dark + Light) =====
    private void EnsureBands()
    {
        if (_darkRects.Count == 0)
        {
            _darkRects.Add(MakeBandRect(isDark: false));
            _darkRects.Add(MakeBandRect(isDark: false));
        }

        if (_lightRects.Count == 0)
        {
            _lightRects.Add(MakeBandRect(isDark: true));
            _lightRects.Add(MakeBandRect(isDark: true));
        }
    }

    private Border MakeBandRect(bool isDark)
    {
        var r = new Border();
        if (isDark)
        {
            r.Style = (Style)Application.Current.Resources["DarkBandStyle"];
            FontIcon icon = new FontIcon();
            icon.Style = (Style)Application.Current.Resources["DarkBandIconStyle"];
            r.Child = icon;
        }
        else
        {
            r.Style = (Style)Application.Current.Resources["LightBandStyle"];
        }

        Canvas.SetZIndex(r, 5); // below ticks/labels
        timelineCanvas.Children.Add(r);
        return r;
    }

    private void UpdateBandsLayout()
    {
        double w = timelineCanvas.ActualWidth;
        double h = timelineCanvas.ActualHeight;
        if (w <= 0 || h <= 0)
        {
            return;
        }

        foreach (var r in _darkRects)
        {
            r.Height = h;
            Canvas.SetTop(r, 0);
        }

        foreach (var r in _lightRects)
        {
            r.Height = h;
            Canvas.SetTop(r, 0);
        }

        var darkRanges = ToRanges(StartTime, EndTime);       // 1 or 2 segments
        var lightRanges = ComplementRanges(darkRanges);       // 0..2

        LayoutRangeRects(_darkRects, darkRanges, w);
        LayoutRangeRects(_lightRects, lightRanges, w);
    }

    private static void LayoutRangeRects(List<Border> rects, List<(TimeSpan Start, TimeSpan End)> ranges, double width)
    {
        for (int i = 0; i < rects.Count; i++)
        {
            if (i < ranges.Count)
            {
                var (start, end) = ranges[i];
                double x = Math.Round((start.TotalHours / 24.0) * width);
                double x2 = Math.Round((end.TotalHours / 24.0) * width);

                var r = rects[i];
                Canvas.SetLeft(r, x);
                r.Width = Math.Max(0, x2 - x);
                r.Visibility = Visibility.Visible;
            }
            else
            {
                rects[i].Visibility = Visibility.Collapsed;
            }
        }
    }

    private static List<(TimeSpan Start, TimeSpan End)> ToRanges(TimeSpan start, TimeSpan end)
    {
        // Full day
        if (start == end)
        {
            return new() { (TimeSpan.Zero, TimeSpan.FromHours(24)) };
        }

        if (start < end)
        {
            return new() { (start, end) };
        }

        // Wraps midnight
        return new()
        {
            (start, TimeSpan.FromHours(24)),
            (TimeSpan.Zero, end),
        };
    }

    private static List<(TimeSpan Start, TimeSpan End)> ComplementRanges(List<(TimeSpan Start, TimeSpan End)> dark)
    {
        var res = new List<(TimeSpan, TimeSpan)>();

        // If dark covers the full day, there is no light
        if (dark.Count == 1 && dark[0].Start == TimeSpan.Zero && dark[0].End == TimeSpan.FromHours(24))
        {
            return res;
        }

        if (dark.Count == 1)
        {
            var (ds, de) = dark[0];
            if (ds > TimeSpan.Zero)
            {
                res.Add((TimeSpan.Zero, ds));
            }

            if (de < TimeSpan.FromHours(24))
            {
                res.Add((de, TimeSpan.FromHours(24)));
            }
        }
        else
        {
            // dark[0] = [a,24), dark[1] = [0,b) => single light [b,a)
            var a = dark[0].Start;
            var b = dark[1].End;
            res.Add((b, a));
        }

        return res;
    }

    // ===== Start & End labels (TOP of chart, ABOVE rectangles) =====
    private void EnsureStartEndEdgeLabels()
    {
        if (_startEdgeLabel == null)
        {
            _startEdgeLabel = new TextBlock { Style = (Style)Application.Current.Resources["EdgeLabelStyle"] };
            headerCanvas.Children.Add(_startEdgeLabel);
            Canvas.SetZIndex(_startEdgeLabel, 25);
        }

        if (_endEdgeLabel == null)
        {
            _endEdgeLabel = new TextBlock { Style = (Style)Application.Current.Resources["EdgeLabelStyle"] };
            headerCanvas.Children.Add(_endEdgeLabel);
            Canvas.SetZIndex(_endEdgeLabel, 25);
        }
    }

    private void UpdateStartEndEdgeLabelsLayout()
    {
        double w = timelineCanvas.ActualWidth;
        if (w <= 0)
        {
            return;
        }

        _startEdgeLabel.Text = TimeSpanHelper.Convert(StartTime);
        _endEdgeLabel.Text = TimeSpanHelper.Convert(EndTime);

        PlaceTopLabelAtTime(_startEdgeLabel, StartTime, w);
        PlaceTopLabelAtTime(_endEdgeLabel, EndTime, w);
    }

    private void PlaceTopLabelAtTime(TextBlock tb, TimeSpan t, double TimeLineWidth)
    {
        double x = Math.Round((t.TotalHours / 24.0) * TimeLineWidth);
        double textW = MeasureTextWidth(tb);
        double desiredLeft = x - (textW / 2.0);

        Canvas.SetLeft(tb, Clamp(desiredLeft, 0, TimeLineWidth - textW));
        Canvas.SetTop(tb, 0);
        tb.Visibility = Visibility.Visible;
    }

    // ===== Sunrise/Sunset ticks on chart =====
    private void EnsureSunriseSunsetTicks()
    {
        if (_sunriseTick == null)
        {
            _sunriseTick = new Line { Style = (Style)Application.Current.Resources["SunRiseMarkerTickStyle"] };
            timelineCanvas.Children.Add(_sunriseTick);
            Canvas.SetZIndex(_sunriseTick, 12);
        }

        if (_sunsetTick == null)
        {
            _sunsetTick = new Line { Style = (Style)Application.Current.Resources["SunSetMarkerTickStyle"] };
            timelineCanvas.Children.Add(_sunsetTick);
            Canvas.SetZIndex(_sunsetTick, 12);
        }
    }

    private void UpdateSunriseSunsetTicksLayout()
    {
        double w = timelineCanvas.ActualWidth;
        double h = timelineCanvas.ActualHeight + 24;
        if (w <= 0 || h <= 0)
        {
            return;
        }

        void Place(Line tick, TimeSpan t)
        {
            double x = Math.Round((t.TotalHours / 24.0) * w);
            tick.X1 = x;
            tick.X2 = x;
            tick.Y1 = 0;
            tick.Y2 = h;
        }

        if (_sunriseTick != null)
        {
            if (Sunrise.HasValue)
            {
                Place(_sunriseTick, Sunrise.Value);
                _sunriseTick.Visibility = Visibility.Visible;
            }
            else
            {
                _sunriseTick.Visibility = Visibility.Collapsed;
            }
        }

        if (_sunsetTick != null)
        {
            if (Sunset.HasValue)
            {
                Place(_sunsetTick, Sunset.Value);
                _sunsetTick.Visibility = Visibility.Visible;
            }
            else
            {
                _sunsetTick.Visibility = Visibility.Collapsed;
            }
        }
    }

    // ===== Sunrise/Sunset panels (below chart) =====
    private void EnsureSunPanels()
    {
        if (_sunrisePanel == null)
        {
            _sunrisePanel = MakeSunPanel("\uEC8A");
            annotationCanvas.Children.Add(_sunrisePanel);
        }

        if (_sunsetPanel == null)
        {
            _sunsetPanel = MakeSunPanel("\uED3A");
            annotationCanvas.Children.Add(_sunsetPanel);
        }
    }

    private StackPanel MakeSunPanel(string iconEmoji)
    {
        var icon = new FontIcon { Glyph = iconEmoji, Style = (Style)Application.Current.Resources["SunIconStyle"] };
        var sp = new StackPanel { Orientation = Orientation.Vertical, Spacing = 2 };
        sp.Children.Add(icon);
        return sp;
    }

    private void UpdateSunPanelsLayout()
    {
        double TimeLineW = timelineCanvas.ActualWidth;
        double annotationW = annotationCanvas.ActualWidth;
        if (annotationW <= 0)
        {
            annotationW = TimeLineW;
        }

        if (TimeLineW <= 0 || annotationW <= 0)
        {
            return;
        }

        void Place(StackPanel sp, TimeSpan t)
        {
            double panelW = MeasureElementWidth(sp);
            double xTimeLine = Math.Round((t.TotalHours / 24.0) * TimeLineW);
            double left = Clamp(xTimeLine - (panelW / 2.0), 0, annotationW - panelW);
            Canvas.SetLeft(sp, left);
            Canvas.SetTop(sp, 8);
        }

        if (_sunrisePanel != null)
        {
            if (Sunrise.HasValue)
            {
                ToolTipService.SetToolTip(_sunrisePanel, $"Sunrise: {TimeSpanHelper.Convert(Sunrise.Value)}");
                _sunrisePanel.Visibility = Visibility.Visible;
                Place(_sunrisePanel, Sunrise.Value);
            }
            else
            {
                ToolTipService.SetToolTip(_sunrisePanel, null);
                _sunrisePanel.Visibility = Visibility.Collapsed;
            }
        }

        if (_sunsetPanel != null)
        {
            if (Sunset.HasValue)
            {
                ToolTipService.SetToolTip(_sunsetPanel, $"Sunset: {TimeSpanHelper.Convert(Sunset.Value)}");
                _sunsetPanel.Visibility = Visibility.Visible;
                Place(_sunsetPanel, Sunset.Value);
            }
            else
            {
                ToolTipService.SetToolTip(_sunsetPanel, null);
                _sunsetPanel.Visibility = Visibility.Collapsed;
            }
        }
    }

    // ===== Major labels BELOW chart (00,06,12,18,24) =====
    private void EnsureMajorTickLabels()
    {
        if (_majorTickBottomLabels.Count > 0)
        {
            return;
        }

        // Includes 24:00 at end
        for (int i = 0; i < 5; i++)
        {
            var tb = new TextBlock { Style = (Style)Application.Current.Resources["MajorTickLabelStyle"] };
            Canvas.SetZIndex(tb, 5); // on annotation canvas
            _majorTickBottomLabels.Add(tb);
            annotationCanvas.Children.Add(tb);
        }
    }

    private void UpdateMajorTickLabelsLayout()
    {
        double TimeLineW = timelineCanvas.ActualWidth;
        double annotationW = annotationCanvas.ActualWidth;
        if (annotationW <= 0)
        {
            annotationW = TimeLineW;
        }

        if (TimeLineW <= 0 || annotationW <= 0)
        {
            return;
        }

        int[] hours = { 0, 6, 12, 18, 24 };

        // 1) Place labels first
        for (int i = 0; i < hours.Length; i++)
        {
            var tb = _majorTickBottomLabels[i];
            var t = TimeSpan.FromHours(hours[i]);
            tb.Text = TimeSpanHelper.Convert(t);

            double xTimeLine = Math.Round((t.TotalHours / 24.0) * TimeLineW);
            double textW = MeasureTextWidth(tb);
            double left = xTimeLine - (textW / 2.0);

            // Middle ones (06, 12) exact center; edges clamp inside canvas
            if (i == 1 || i == 2)
            {
                Canvas.SetLeft(tb, left);
            }
            else
            {
                Canvas.SetLeft(tb, Clamp(left, 0, annotationW - textW));
            }

            Canvas.SetTop(tb, 8); // your existing baseline below chart
            tb.Visibility = Visibility.Visible;
        }

        // 2) Compute sunrise/sunset occupied horizontal ranges (if present)
        (double Left, double Right)? sunriseBounds = null;
        (double Left, double Right)? sunsetBounds = null;

        if (Sunrise.HasValue && _sunrisePanel != null)
        {
            sunriseBounds = GetAnnotationBoundsForTime(Sunrise.Value, TimeLineW, annotationW, _sunrisePanel);
        }

        if (Sunset.HasValue && _sunsetPanel != null)
        {
            sunsetBounds = GetAnnotationBoundsForTime(Sunset.Value, TimeLineW, annotationW, _sunsetPanel);
        }

        // 3) Hide any label that intersects the sunrise/sunset panel bounds
        for (int i = 0; i < hours.Length; i++)
        {
            var tb = _majorTickBottomLabels[i];
            if (tb.Visibility != Visibility.Visible)
            {
                continue;
            }

            var lbl = GetLabelBounds(tb);

            bool hide =
                (sunriseBounds.HasValue && Intersects(lbl, sunriseBounds.Value)) ||
                (sunsetBounds.HasValue && Intersects(lbl, sunsetBounds.Value)); // include sunset too; remove if you only want sunrise

            tb.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    // ===== Utilities =====
    private static double Clamp(double v, double min, double max) => Math.Max(min, Math.Min(max, v));

    private static double MeasureElementWidth(FrameworkElement el)
    {
        el.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        return el.DesiredSize.Width;
    }

    private static double MeasureTextWidth(TextBlock tb)
    {
        tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        return tb.DesiredSize.Width;
    }

    private static bool Intersects((double Left, double Right) a, (double Left, double Right) b, double pad = 4)
    {
        // Horizontal overlap with padding
        return !(a.Right + pad <= b.Left || b.Right + pad <= a.Left);
    }

    private (double Left, double Right) GetAnnotationBoundsForTime(TimeSpan t, double TimeLineW, double annotationW, FrameworkElement element)
    {
        // Compute the *actual* left/right the panel will occupy in AnnotationCanvas
        double panelW = MeasureElementWidth(element);
        double xTimeLine = Math.Round((t.TotalHours / 24.0) * TimeLineW);
        double left = Clamp(xTimeLine - (panelW / 2.0), 0, annotationW - panelW);
        return (left, left + panelW);
    }

    private (double Left, double Right) GetLabelBounds(TextBlock tb)
    {
        double w = MeasureTextWidth(tb);
        double left = Canvas.GetLeft(tb);
        return (left, left + w);
    }
}
