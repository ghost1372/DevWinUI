namespace DevWinUI;

public sealed partial class ShowMore
{
    public int Lines
    {
        get { return (int)GetValue(LinesProperty); }
        set { SetValue(LinesProperty, value); }
    }

    public static readonly DependencyProperty LinesProperty =
        DependencyProperty.Register(nameof(Lines), typeof(int), typeof(ShowMore), new PropertyMetadata(3, OnLayoutPropertyChanged));

    public double MaxExpandedHeight
    {
        get { return (double)GetValue(MaxExpandedHeightProperty); }
        set { SetValue(MaxExpandedHeightProperty, value); }
    }

    public static readonly DependencyProperty MaxExpandedHeightProperty =
        DependencyProperty.Register(nameof(MaxExpandedHeight), typeof(double), typeof(ShowMore), new PropertyMetadata(320d, OnLayoutPropertyChanged));

    public bool IsExpanded
    {
        get { return (bool)GetValue(IsExpandedProperty); }
        set { SetValue(IsExpandedProperty, value); }
    }

    public static readonly DependencyProperty IsExpandedProperty =
        DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(ShowMore), new PropertyMetadata(false, OnIsExpandedChanged));

    public string MoreLabel
    {
        get { return (string)GetValue(MoreLabelProperty); }
        set { SetValue(MoreLabelProperty, value); }
    }

    public static readonly DependencyProperty MoreLabelProperty =
        DependencyProperty.Register(nameof(MoreLabel), typeof(string), typeof(ShowMore), new PropertyMetadata("Show more", OnToggleLabelChanged));

    public string LessLabel
    {
        get { return (string)GetValue(LessLabelProperty); }
        set { SetValue(LessLabelProperty, value); }
    }

    public static readonly DependencyProperty LessLabelProperty =
        DependencyProperty.Register(nameof(LessLabel), typeof(string), typeof(ShowMore), new PropertyMetadata("Show less", OnToggleLabelChanged));

    public string Label
    {
        get { return (string)GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(ShowMore), new PropertyMetadata("Details", OnLabelChanged));

    public Brush? VeilBrush
    {
        get { return (Brush?)GetValue(VeilBrushProperty); }
        set { SetValue(VeilBrushProperty, value); }
    }

    public static readonly DependencyProperty VeilBrushProperty =
        DependencyProperty.Register(nameof(VeilBrush), typeof(Brush), typeof(ShowMore), new PropertyMetadata(null));

    private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ShowMore)d).Update(animate: false);
    }

    private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ShowMore)d).OnIsExpandedChanged((bool)e.NewValue);
    }

    private static void OnToggleLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ShowMore)d).UpdateToggleLabel();
    }

    private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ShowMore)d).UpdateScrollerName();
    }
}
