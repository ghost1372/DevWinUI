using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[TemplatePart(Name = BeforeImageElement, Type = typeof(Image))]
[TemplatePart(Name = RectangleElement, Type = typeof(Rectangle))]
[TemplatePart(Name = PART_Thumb, Type = typeof(Thumb))]
[TemplatePart(Name = PART_Line, Type = typeof(Line))]
public partial class CompareSlider : Control
{
    public bool ShowLineAndThumb
    {
        get { return (bool)GetValue(ShowLineAndThumbProperty); }
        set { SetValue(ShowLineAndThumbProperty, value); }
    }

    public static readonly DependencyProperty ShowLineAndThumbProperty =
        DependencyProperty.Register(nameof(ShowLineAndThumb), typeof(bool), typeof(CompareSlider), new PropertyMetadata(true));

    public Style LineStyle
    {
        get { return (Style)GetValue(LineStyleProperty); }
        set { SetValue(LineStyleProperty, value); }
    }

    public static readonly DependencyProperty LineStyleProperty =
        DependencyProperty.Register(nameof(LineStyle), typeof(Style), typeof(CompareSlider), new PropertyMetadata(default(Style)));

    public Style ThumbStyle
    {
        get { return (Style)GetValue(ThumbStyleProperty); }
        set { SetValue(ThumbStyleProperty, value); }
    }

    public static readonly DependencyProperty ThumbStyleProperty =
        DependencyProperty.Register(nameof(ThumbStyle), typeof(Style), typeof(CompareSlider), new PropertyMetadata(default(Style)));

    public double Value
    {
        get { return (double)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(double), typeof(CompareSlider), new PropertyMetadata(0.0, OnValueChanged));

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CompareSlider)d;
        if (ctl != null)
        {
            ctl.ValueChanged?.Invoke(ctl, (double)e.NewValue);
            ctl.UpdateValue((double)e.NewValue);
        }
    }

    public ImageSource SourceImage
    {
        get => (ImageSource)GetValue(SourceImageProperty);
        set => SetValue(SourceImageProperty, value);
    }

    public static readonly DependencyProperty SourceImageProperty =
        DependencyProperty.Register(nameof(SourceImage), typeof(ImageSource), typeof(CompareSlider), new PropertyMetadata(default(ImageSource)));

    public ImageSource TargetImage
    {
        get => (ImageSource)GetValue(TargetImageProperty);
        set => SetValue(TargetImageProperty, value);
    }
    public static readonly DependencyProperty TargetImageProperty =
        DependencyProperty.Register(nameof(TargetImage), typeof(ImageSource), typeof(CompareSlider), new PropertyMetadata(default(ImageSource)));

    public Orientation Orientation
    {
        get { return (Orientation)GetValue(OrientationProperty); }
        set { SetValue(OrientationProperty, value); }
    }
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(CompareSlider), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

    private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CompareSlider)d;
        if (ctl != null)
        {
            ctl.UpdateTemplate();
        }
    }
    private void UpdateTemplate()
    {
        Template = Orientation switch
        {
            Orientation.Horizontal => HorizontalTemplate,
            Orientation.Vertical => VerticalTemplate,
            _ => Template
        };
    }
    public event EventHandler<double> ValueChanged;
    public ControlTemplate? HorizontalTemplate { get; set; }
    public ControlTemplate? VerticalTemplate { get; set; }

    private const string BeforeImageElement = "PART_BeforeImage";
    private const string RectangleElement = "PART_Rectangle";
    private const string PART_Line = "PART_Line";
    private const string PART_Thumb = "PART_Thumb";
    private Image beforeImage;
    private Rectangle rectangle;
    private Thumb thumb;
    private Line line;
    private double _currentOffsetX;
    private double _currentOffsetY;

    private TranslateTransform translateTransform;

    public CompareSlider()
    {
        this.DefaultStyleKey = typeof(CompareSlider);

        if (Application.Current.Resources["ComapreSliderHorizontalTemplate"] is ControlTemplate horizontalTemplate)
            HorizontalTemplate = horizontalTemplate;

        if (Application.Current.Resources["CompareSliderVerticalTemplate"] is ControlTemplate verticalTemplate)
            VerticalTemplate = verticalTemplate;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        line = GetTemplateChild(PART_Line) as Line;
        thumb = GetTemplateChild(PART_Thumb) as Thumb;
        beforeImage = GetTemplateChild(BeforeImageElement) as Image;
        rectangle = GetTemplateChild(RectangleElement) as Rectangle;

        beforeImage.SizeChanged -= OnSizeChanged;
        beforeImage.SizeChanged += OnSizeChanged;

        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;

        beforeImage.ImageOpened -= OnImageOpened;
        beforeImage.ImageOpened += OnImageOpened;

        thumb.DragDelta -= OnThumbDragDelta;
        thumb.DragDelta += OnThumbDragDelta;

        _currentOffsetX = 0;
        _currentOffsetY = 0;

        translateTransform = new TranslateTransform();
        thumb.RenderTransform = translateTransform;
        line.RenderTransform = translateTransform;
    }

    private void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
    {
        if (beforeImage == null || thumb == null || line == null || rectangle == null)
        {
            return;
        }

        if (Orientation == Orientation.Horizontal)
        {
            if (_currentOffsetX == 0)
            {
                _currentOffsetX = translateTransform.X;
            }

            double beforeImageWidth = (beforeImage.ActualWidth - thumb.ActualWidth) / 2;
            double offsetAdjustment = thumb.ActualWidth / 2;
            double leftBound = -beforeImageWidth - offsetAdjustment;
            double rightBound = beforeImageWidth + offsetAdjustment;

            // Calculate the new offset
            double newOffsetX = _currentOffsetX + e.HorizontalChange;
            newOffsetX = Math.Max(leftBound, Math.Min(rightBound, newOffsetX));

            // Update the offset and translate transform
            _currentOffsetX = newOffsetX;
            translateTransform.X = _currentOffsetX;

            // Calculate the percentage value
            double totalRange = rightBound - leftBound;
            double normalizedValue = (_currentOffsetX - leftBound) / totalRange;
            double percentageValue = normalizedValue * 100;

            // Update the value based on the drag
            Value = percentageValue;
        }
        else
        {
            if (_currentOffsetY == 0)
            {
                _currentOffsetY = translateTransform.Y;
            }

            double beforeImageHeight = (beforeImage.ActualHeight - thumb.ActualHeight) / 2;
            double offsetAdjustment = thumb.ActualHeight / 2;
            double topBound = -beforeImageHeight - offsetAdjustment;
            double bottomBound = beforeImageHeight + offsetAdjustment;

            // Calculate the new offset
            double newOffsetY = _currentOffsetY + e.VerticalChange;
            newOffsetY = Math.Max(topBound, Math.Min(bottomBound, newOffsetY));

            // Update the offset and translate transform
            _currentOffsetY = newOffsetY;
            translateTransform.Y = _currentOffsetY;

            // Calculate the percentage value
            double totalRange = bottomBound - topBound;
            double normalizedValue = (_currentOffsetY - topBound) / totalRange;
            double percentageValue = normalizedValue * 100;

            // Update the value based on the drag
            Value = percentageValue;
        }
    }

    private void OnImageOpened(object sender, RoutedEventArgs e)
    {
        beforeImage.ImageOpened -= OnImageOpened;
        UpdateValue(Value);
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateValue(Value);
    }

    public void UpdateValue(double newValue)
    {
        if (rectangle == null || beforeImage == null || thumb == null || line == null)
        {
            return;
        }

        if (Orientation == Orientation.Horizontal)
        {
            newValue = Math.Max(0, Math.Min(100, newValue));

            // Recalculate the rectangle width based on the new value
            double rectangleWidth = beforeImage.ActualWidth * (newValue / 100.0);
            rectangle.Width = rectangleWidth;
            rectangle.Height = beforeImage.ActualHeight;
            line.Height = beforeImage.ActualHeight;

            // Calculate the new translateTransform.X value based on the rectangle's width
            translateTransform.X = rectangleWidth - (beforeImage.ActualWidth / 2);

            // Update _currentOffsetX to the corresponding offset for the new value
            double beforeImageWidth = (beforeImage.ActualWidth - thumb.ActualWidth) / 2;
            double offsetAdjustment = thumb.ActualWidth / 2;
            double leftBound = -beforeImageWidth - offsetAdjustment;
            double rightBound = beforeImageWidth + offsetAdjustment;

            // Map the new value (0-100) to the corresponding offset
            double totalRange = rightBound - leftBound;
            double newOffsetX = leftBound + (newValue / 100) * totalRange;

            // Set _currentOffsetX and update the thumb position
            _currentOffsetX = newOffsetX;
            translateTransform.X = _currentOffsetX;
        }
        else
        {
            newValue = Math.Max(0, Math.Min(100, newValue));

            // Recalculate the rectangle height based on the new value
            double rectangleHeight = beforeImage.ActualHeight * (newValue / 100.0);
            rectangle.Height = rectangleHeight;
            rectangle.Width = beforeImage.ActualWidth;
            line.Width = beforeImage.ActualWidth;

            // Calculate the new translateTransform.Y value based on the rectangle's height
            translateTransform.Y = rectangleHeight - (beforeImage.ActualHeight / 2);

            // Update _currentOffsetY to the corresponding offset for the new value
            double beforeImageHeight = (beforeImage.ActualHeight - thumb.ActualHeight) / 2;
            double offsetAdjustment = thumb.ActualHeight / 2;
            double topBound = -beforeImageHeight - offsetAdjustment;
            double bottomBound = beforeImageHeight + offsetAdjustment;

            // Map the new value (0-100) to the corresponding offset
            double totalRange = bottomBound - topBound;
            double newOffsetY = topBound + (newValue / 100) * totalRange;

            // Set _currentOffsetY and update the thumb position
            _currentOffsetY = newOffsetY;
            translateTransform.Y = _currentOffsetY;
        }
    }
}
