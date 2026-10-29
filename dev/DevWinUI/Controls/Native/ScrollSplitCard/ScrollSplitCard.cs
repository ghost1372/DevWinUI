using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;

namespace DevWinUI;

public sealed record ScrollSplitCardItem(string Title, string Description = "", string? Eyebrow = null);

public sealed partial class ScrollSplitCard : Control
{
    readonly TranslateTransform _rowLift = new();
    readonly TranslateTransform _endShift = new();
    readonly RectangleGeometry _clip = new();
    readonly List<ScrollSplitCardPanel> _slices = new();

    Grid _frame = null!;
    Grid _row = null!;
    FrameworkElement _hint = null!;
    TextBlock _hintText = null!;
    TextBlock _end = null!;
    ScrollViewer _scroll = null!;
    Border _track = null!;
    BitmapImage? _bitmap;
    double _aspect;
    double _progress;
    double _pending;
    bool _queued, _templated;

    public ScrollSplitCard()
    {
        DefaultStyleKey = typeof(ScrollSplitCard);
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    (double h, double rowH, double maxW, double radius, double title, double body, double pad) Metrics => Size switch
    {
        ScrollSplitCardSize.Small => (320.0, 200.0, 520.0, 12.0, 14.0, 11.0, 12.0),
        ScrollSplitCardSize.Large => (560.0, 380.0, 1000.0, 20.0, 20.0, 14.0, 24.0),
        _ => (440.0, 280.0, 760.0, 16.0, 16.0, 12.5, 16.0),
    };

    protected override void OnApplyTemplate()
    {
        if (_templated)
        {
            _frame.SizeChanged -= OnFrameSizeChanged;
            _row.SizeChanged -= OnRowSizeChanged;
            _scroll.ViewChanging -= OnViewChanging;
            _scroll.ViewChanged -= OnViewChanged;
        }

        base.OnApplyTemplate();

        _frame = (Grid)GetTemplateChild("PART_Frame");
        _row = (Grid)GetTemplateChild("PART_Row");
        _hint = (FrameworkElement)GetTemplateChild("PART_Hint");
        _hintText = (TextBlock)GetTemplateChild("PART_HintText");
        _end = (TextBlock)GetTemplateChild("PART_End");
        _scroll = (ScrollViewer)GetTemplateChild("PART_Scroll");
        _track = (Border)GetTemplateChild("PART_Track");

        _frame.Clip = _clip;
        _row.RenderTransform = _rowLift;
        _end.RenderTransform = _endShift;

        _frame.SizeChanged += OnFrameSizeChanged;
        _row.SizeChanged += OnRowSizeChanged;
        _scroll.ViewChanging += OnViewChanging;
        _scroll.ViewChanged += OnViewChanged;
        _templated = true;

        Rebuild();
        UpdateTexts();
    }

    static CornerRadius Corners(int index, int count, double outer, double inner)
    {
        if (count == 1) return new CornerRadius(outer);

        double left = index == 0 ? outer : inner;
        double right = index == count - 1 ? outer : inner;
        return new CornerRadius(left, right, right, left);
    }

    void OnLoaded(object sender, RoutedEventArgs e) => ApplyMotionMode();

    void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (!_queued) return;

        _queued = false;
        CompositionTarget.Rendering -= OnFrame;
    }

    void OnFrameSizeChanged(object sender, SizeChangedEventArgs e) =>
        _clip.Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);

    void OnRowSizeChanged(object sender, SizeChangedEventArgs e)
    {
        LayoutImage();
        Apply(_progress);
    }

    void OnViewChanging(object? sender, ScrollViewerViewChangingEventArgs e) => Queue(Progress(e.NextView.VerticalOffset));

    void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs e) => Queue(Progress(_scroll.VerticalOffset));

    void Rebuild()
    {
        if (!_templated) return;

        var m = Metrics;
        _frame.Height = m.h;
        _row.Height = m.rowH;
        _row.MaxWidth = m.maxW;

        _row.Children.Clear();
        _row.ColumnDefinitions.Clear();
        _slices.Clear();

        _bitmap = null;
        _aspect = 0;
        if (Image is { } uri)
        {
            var bitmap = new BitmapImage(uri);
            bitmap.ImageOpened += (_, _) =>
            {
                if (!ReferenceEquals(_bitmap, bitmap) || bitmap.PixelHeight <= 0) return;
                _aspect = (double)bitmap.PixelWidth / bitmap.PixelHeight;
                LayoutImage();
            };
            _bitmap = bitmap;
        }

        var cards = Cards ?? Array.Empty<ScrollSplitCardItem>();
        for (int i = 0; i < cards.Count; i++)
        {
            _row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var panel = new ScrollSplitCardPanel
            {
                Item = cards[i],
                Index = i,
                Tone = Tone,
                TitleFontSize = m.title,
                BodyFontSize = m.body,
                Padding = new Thickness(m.pad),
                CornerRadius = new CornerRadius(m.radius),
            };
            panel.SetImage(_bitmap);
            Grid.SetColumn(panel, i);
            _row.Children.Add(panel);
            _slices.Add(panel);
        }

        UpdateAutomation();
        ApplyMotionMode();
    }

    void LayoutImage()
    {
        double rw = _row.ActualWidth, rh = _row.ActualHeight;
        int n = _slices.Count;
        if (rw <= 0 || rh <= 0 || n == 0) return;

        double pw = rw / n;
        double aspect = _aspect > 0 ? _aspect : rw / rh;
        double dw, dh;
        if (rw / rh > aspect)
        {
            dw = rw;
            dh = rw / aspect;
        }
        else
        {
            dh = rh;
            dw = rh * aspect;
        }

        double dx = (rw - dw) / 2, dy = (rh - dh) / 2;

        for (int i = 0; i < n; i++)
        {
            var transform = _slices[i].ImageTransform;
            transform.ScaleX = dw / pw;
            transform.ScaleY = dh / rh;
            transform.TranslateX = (dx - i * pw) / pw;
            transform.TranslateY = dy / rh;
        }
    }

    void Retone()
    {
        foreach (var slice in _slices) slice.Tone = Tone;
    }

    void UpdateTexts()
    {
        if (!_templated) return;

        _hintText.Text = Hint ?? string.Empty;
        _hint.Visibility = string.IsNullOrEmpty(Hint) ? Visibility.Collapsed : Visibility.Visible;
        _end.Text = EndLabel ?? string.Empty;
        _end.Visibility = string.IsNullOrEmpty(EndLabel) ? Visibility.Collapsed : Visibility.Visible;
    }

    void UpdateAutomation()
    {
        if (!_templated) return;

        AutomationProperties.SetName(_scroll, Label);
        for (int i = 0; i < _slices.Count; i++)
        {
            bool described = i == 0 && !string.IsNullOrEmpty(ImageAlt);
            AutomationProperties.SetName(_slices[i], i == 0 ? ImageAlt : string.Empty);
            AutomationProperties.SetAccessibilityView(_slices[i], described ? AccessibilityView.Content : AccessibilityView.Raw);
        }
    }

    double Progress(double offset)
    {
        double range = _scroll.ExtentHeight - _scroll.ViewportHeight;
        return range > 1 ? Math.Clamp(offset / range, 0, 1) : 0;
    }

    void Queue(double progress)
    {
        _pending = progress;
        if (_queued) return;

        _queued = true;
        CompositionTarget.Rendering += OnFrame;
    }

    void OnFrame(object? sender, object e)
    {
        CompositionTarget.Rendering -= OnFrame;
        _queued = false;
        Apply(_pending);
    }

    void ApplyMotionMode()
    {
        if (!_templated) return;

        bool motion = IsAnimationEnabled;
        double h = Metrics.h;
        _track.Height = motion ? h * 3 : h;
        _scroll.VerticalScrollMode = motion ? ScrollMode.Enabled : ScrollMode.Disabled;
        _scroll.VerticalScrollBarVisibility = motion ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
        _scroll.IsTabStop = motion;
        Apply(motion ? Progress(_scroll.VerticalOffset) : 1);
    }

    void Apply(double progress)
    {
        if (!_templated) return;

        _progress = progress;
        if (!IsAnimationEnabled) progress = 1;

        double t1 = ScrollSplitCardMath.Smooth(ScrollSplitCardMath.Phase(progress, 0.0, 0.4));
        double t2 = ScrollSplitCardMath.Smooth(ScrollSplitCardMath.Phase(progress, 0.4, 0.8));
        double t3 = ScrollSplitCardMath.Smooth(ScrollSplitCardMath.Phase(progress, 0.8, 1.0));

        int n = _slices.Count;
        double center = (n - 1) / 2.0;
        double radius = Metrics.radius;
        double inner = radius * t1;
        double scale = 1 - 0.1 * t1;
        double gap = 28 * t1 - 24 * t2;
        double angle = 180 * t2;
        double tilt = 6 * Math.Sin(Math.PI * t2);

        for (int i = 0; i < n; i++)
        {
            var slice = _slices[i];
            double d = i - center;

            var pose = slice.PoseTransform;
            pose.ScaleX = scale;
            pose.ScaleY = scale;
            pose.TranslateX = d * gap;
            pose.Rotation = Math.Sign(d) * tilt;

            slice.FrontProjection.RotationY = angle;
            slice.BackProjection.RotationY = angle - 180;
            slice.FaceUp = angle >= 90;
            slice.FrontCorners = Corners(i, n, radius, inner);
        }

        _rowLift.Y = -18 * t3;
        _end.Opacity = t3;
        _endShift.Y = 10 * (1 - t3);
        _hint.Opacity = 1 - ScrollSplitCardMath.Phase(progress, 0, 0.08);
    }
}
