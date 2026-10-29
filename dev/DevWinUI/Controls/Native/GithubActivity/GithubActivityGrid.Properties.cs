namespace DevWinUI;

public sealed partial class GitHubActivityGrid
{
    public IList<ActivityDay>? Days
    {
        get { return (IList<ActivityDay>?)GetValue(DaysProperty); }
        set { SetValue(DaysProperty, value); }
    }

    public static readonly DependencyProperty DaysProperty =
        DependencyProperty.Register(nameof(Days), typeof(IList<ActivityDay>), typeof(GitHubActivityGrid),
            new PropertyMetadata(null, (d, e) => ((GitHubActivityGrid)d).OnDaysChanged(
                (IList<ActivityDay>?)e.OldValue, (IList<ActivityDay>?)e.NewValue)));

    public int? MaxCount
    {
        get { return (int?)GetValue(MaxCountProperty); }
        set { SetValue(MaxCountProperty, value); }
    }

    public static readonly DependencyProperty MaxCountProperty =
        DependencyProperty.Register(nameof(MaxCount), typeof(int?), typeof(GitHubActivityGrid),
            new PropertyMetadata(null, (d, e) => ((GitHubActivityGrid)d).Rebuild()));

    public double CellSize
    {
        get { return (double)GetValue(CellSizeProperty); }
        set { SetValue(CellSizeProperty, value); }
    }

    public static readonly DependencyProperty CellSizeProperty =
        DependencyProperty.Register(nameof(CellSize), typeof(double), typeof(GitHubActivityGrid),
            new PropertyMetadata(12.0, (d, e) => ((GitHubActivityGrid)d).Rebuild()));

    public double CellGap
    {
        get { return (double)GetValue(CellGapProperty); }
        set { SetValue(CellGapProperty, value); }
    }

    public static readonly DependencyProperty CellGapProperty =
        DependencyProperty.Register(nameof(CellGap), typeof(double), typeof(GitHubActivityGrid),
            new PropertyMetadata(3.0, (d, e) => ((GitHubActivityGrid)d).Rebuild()));

    public string UnitSingular
    {
        get { return (string)GetValue(UnitSingularProperty); }
        set { SetValue(UnitSingularProperty, value); }
    }

    public static readonly DependencyProperty UnitSingularProperty =
        DependencyProperty.Register(nameof(UnitSingular), typeof(string), typeof(GitHubActivityGrid),
            new PropertyMetadata("contribution", (d, e) => ((GitHubActivityGrid)d).Rebuild()));

    public string UnitPlural
    {
        get { return (string)GetValue(UnitPluralProperty); }
        set { SetValue(UnitPluralProperty, value); }
    }

    public static readonly DependencyProperty UnitPluralProperty =
        DependencyProperty.Register(nameof(UnitPlural), typeof(string), typeof(GitHubActivityGrid),
            new PropertyMetadata("contributions", (d, e) => ((GitHubActivityGrid)d).Rebuild()));

    public string LessText
    {
        get { return (string)GetValue(LessTextProperty); }
        set { SetValue(LessTextProperty, value); }
    }

    public static readonly DependencyProperty LessTextProperty =
        DependencyProperty.Register(nameof(LessText), typeof(string), typeof(GitHubActivityGrid),
            new PropertyMetadata("Less", (d, e) => ((GitHubActivityGrid)d).Rebuild()));

    public string MoreText
    {
        get { return (string)GetValue(MoreTextProperty); }
        set { SetValue(MoreTextProperty, value); }
    }

    public static readonly DependencyProperty MoreTextProperty =
        DependencyProperty.Register(nameof(MoreText), typeof(string), typeof(GitHubActivityGrid),
            new PropertyMetadata("More", (d, e) => ((GitHubActivityGrid)d).Rebuild()));

    public Brush? Level0Brush
    {
        get { return (Brush?)GetValue(Level0BrushProperty); }
        set { SetValue(Level0BrushProperty, value); }
    }

    public static readonly DependencyProperty Level0BrushProperty =
        DependencyProperty.Register(nameof(Level0Brush), typeof(Brush), typeof(GitHubActivityGrid),
            new PropertyMetadata(null, (d, e) => ((GitHubActivityGrid)d).ApplyBrushes()));

    public Brush? Level1Brush
    {
        get { return (Brush?)GetValue(Level1BrushProperty); }
        set { SetValue(Level1BrushProperty, value); }
    }

    public static readonly DependencyProperty Level1BrushProperty =
        DependencyProperty.Register(nameof(Level1Brush), typeof(Brush), typeof(GitHubActivityGrid),
            new PropertyMetadata(null, (d, e) => ((GitHubActivityGrid)d).ApplyBrushes()));

    public Brush? Level2Brush
    {
        get { return (Brush?)GetValue(Level2BrushProperty); }
        set { SetValue(Level2BrushProperty, value); }
    }

    public static readonly DependencyProperty Level2BrushProperty =
        DependencyProperty.Register(nameof(Level2Brush), typeof(Brush), typeof(GitHubActivityGrid),
            new PropertyMetadata(null, (d, e) => ((GitHubActivityGrid)d).ApplyBrushes()));

    public Brush? Level3Brush
    {
        get { return (Brush?)GetValue(Level3BrushProperty); }
        set { SetValue(Level3BrushProperty, value); }
    }

    public static readonly DependencyProperty Level3BrushProperty =
        DependencyProperty.Register(nameof(Level3Brush), typeof(Brush), typeof(GitHubActivityGrid),
            new PropertyMetadata(null, (d, e) => ((GitHubActivityGrid)d).ApplyBrushes()));

    public Brush? Level4Brush
    {
        get { return (Brush?)GetValue(Level4BrushProperty); }
        set { SetValue(Level4BrushProperty, value); }
    }

    public static readonly DependencyProperty Level4BrushProperty =
        DependencyProperty.Register(nameof(Level4Brush), typeof(Brush), typeof(GitHubActivityGrid),
            new PropertyMetadata(null, (d, e) => ((GitHubActivityGrid)d).ApplyBrushes()));

    public Brush? TipBackground
    {
        get { return (Brush?)GetValue(TipBackgroundProperty); }
        set { SetValue(TipBackgroundProperty, value); }
    }

    public static readonly DependencyProperty TipBackgroundProperty =
        DependencyProperty.Register(nameof(TipBackground), typeof(Brush), typeof(GitHubActivityGrid), new PropertyMetadata(null));

    public Brush? TipForeground
    {
        get { return (Brush?)GetValue(TipForegroundProperty); }
        set { SetValue(TipForegroundProperty, value); }
    }

    public static readonly DependencyProperty TipForegroundProperty =
        DependencyProperty.Register(nameof(TipForeground), typeof(Brush), typeof(GitHubActivityGrid), new PropertyMetadata(null));

    public Brush? TipSecondaryForeground
    {
        get { return (Brush?)GetValue(TipSecondaryForegroundProperty); }
        set { SetValue(TipSecondaryForegroundProperty, value); }
    }

    public static readonly DependencyProperty TipSecondaryForegroundProperty =
        DependencyProperty.Register(nameof(TipSecondaryForeground), typeof(Brush), typeof(GitHubActivityGrid), new PropertyMetadata(null));

    public bool ReduceMotion
    {
        get { return (bool)GetValue(ReduceMotionProperty); }
        set { SetValue(ReduceMotionProperty, value); }
    }

    public static readonly DependencyProperty ReduceMotionProperty =
        DependencyProperty.Register(nameof(ReduceMotion), typeof(bool), typeof(GitHubActivityGrid), new PropertyMetadata(false));
}
