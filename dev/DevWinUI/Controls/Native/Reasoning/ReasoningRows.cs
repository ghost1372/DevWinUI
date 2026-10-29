namespace DevWinUI;

public sealed partial class ReasoningRows : Control, IReasoningReplay
{
    public event TypedEventHandler<ReasoningRows, int>? SelectionChanged;

    readonly List<ReasoningRowItem> _items = new();
    FrameworkElement _query = null!;
    TextBlock _queryText = null!;
    StackPanel _host = null!;
    bool _templated;

    public ReasoningRows()
    {
        DefaultStyleKey = typeof(ReasoningRows);
        Loaded += (_, _) => Replay();
    }

    public void Replay()
    {
        for (int i = 0; i < _items.Count; i++) ReasoningFx.FadeUp(_items[i], i * 80);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _query = (FrameworkElement)GetTemplateChild("PART_Query");
        _queryText = (TextBlock)GetTemplateChild("PART_QueryText");
        _host = (StackPanel)GetTemplateChild("PART_Host");
        _templated = true;
        Rebuild();
    }

    void Rebuild()
    {
        if (!_templated) return;

        bool showQuery = Kind == RowsKind.Search && !string.IsNullOrEmpty(Query);
        _query.Visibility = showQuery ? Visibility.Visible : Visibility.Collapsed;
        _queryText.Text = Query ?? string.Empty;

        _host.Children.Clear();
        _items.Clear();

        var rows = Rows ?? Array.Empty<ReasoningRow>();
        for (int i = 0; i < rows.Count; i++)
        {
            int index = i;
            var item = new ReasoningRowItem { Row = rows[i], Kind = Kind, IsSelected = i == SelectedIndex };
            item.Clicked += _ => SelectedIndex = index;
            _items.Add(item);
            _host.Children.Add(item);
        }
    }

    void OnSelectedChanged()
    {
        for (int i = 0; i < _items.Count; i++) _items[i].IsSelected = i == SelectedIndex;
        SelectionChanged?.Invoke(this, SelectedIndex);
    }
}
