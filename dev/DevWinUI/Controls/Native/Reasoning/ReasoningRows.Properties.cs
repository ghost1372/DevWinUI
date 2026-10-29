namespace DevWinUI;

public sealed partial class ReasoningRows
{
    public IReadOnlyList<ReasoningRow> Rows
    {
        get { return (IReadOnlyList<ReasoningRow>)GetValue(RowsProperty); }
        set { SetValue(RowsProperty, value); }
    }

    public static readonly DependencyProperty RowsProperty =
        DependencyProperty.Register(nameof(Rows), typeof(IReadOnlyList<ReasoningRow>), typeof(ReasoningRows), new PropertyMetadata(Array.Empty<ReasoningRow>(), (d, _) => ((ReasoningRows)d).Rebuild()));

    public RowsKind Kind
    {
        get { return (RowsKind)GetValue(KindProperty); }
        set { SetValue(KindProperty, value); }
    }

    public static readonly DependencyProperty KindProperty =
        DependencyProperty.Register(nameof(Kind), typeof(RowsKind), typeof(ReasoningRows), new PropertyMetadata(RowsKind.Steps, (d, _) => ((ReasoningRows)d).Rebuild()));

    public string? Query
    {
        get { return (string?)GetValue(QueryProperty); }
        set { SetValue(QueryProperty, value); }
    }

    public static readonly DependencyProperty QueryProperty =
        DependencyProperty.Register(nameof(Query), typeof(string), typeof(ReasoningRows), new PropertyMetadata(null, (d, _) => ((ReasoningRows)d).Rebuild()));

    public int SelectedIndex
    {
        get { return (int)GetValue(SelectedIndexProperty); }
        set { SetValue(SelectedIndexProperty, value); }
    }

    public static readonly DependencyProperty SelectedIndexProperty =
        DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(ReasoningRows), new PropertyMetadata(-1, (d, _) => ((ReasoningRows)d).OnSelectedChanged()));
}
