namespace DevWinUI;

public sealed partial class AnimatedToc
{
    public ObservableCollection<AnimatedTocItem> Items { get; } = new();

    public int SelectedIndex
    {
        get { return (int)GetValue(SelectedIndexProperty); }
        set { SetValue(SelectedIndexProperty, value); }
    }

    public static readonly DependencyProperty SelectedIndexProperty =
        DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(AnimatedToc), new PropertyMetadata(0, OnSelectedIndexChanged));

    public double RowHeight
    {
        get { return (double)GetValue(RowHeightProperty); }
        set { SetValue(RowHeightProperty, value); }
    }

    public static readonly DependencyProperty RowHeightProperty =
        DependencyProperty.Register(nameof(RowHeight), typeof(double), typeof(AnimatedToc), new PropertyMetadata(48.0, OnLayoutChanged));

    public double Indent
    {
        get { return (double)GetValue(IndentProperty); }
        set { SetValue(IndentProperty, value); }
    }

    public static readonly DependencyProperty IndentProperty =
        DependencyProperty.Register(nameof(Indent), typeof(double), typeof(AnimatedToc), new PropertyMetadata(24.0, OnLayoutChanged));

    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(AnimatedToc), new PropertyMetadata("On this page", OnLayoutChanged));

    public Brush? RailBrush
    {
        get { return (Brush?)GetValue(RailBrushProperty); }
        set { SetValue(RailBrushProperty, value); }
    }

    public static readonly DependencyProperty RailBrushProperty =
        DependencyProperty.Register(nameof(RailBrush), typeof(Brush), typeof(AnimatedToc), new PropertyMetadata(null));

    public Brush? ProgressBrush
    {
        get { return (Brush?)GetValue(ProgressBrushProperty); }
        set { SetValue(ProgressBrushProperty, value); }
    }

    public static readonly DependencyProperty ProgressBrushProperty =
        DependencyProperty.Register(nameof(ProgressBrush), typeof(Brush), typeof(AnimatedToc), new PropertyMetadata(null));

    private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((AnimatedToc)d).ApplySelection(animate: true);

    private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((AnimatedToc)d).RequestRebuild();
}
