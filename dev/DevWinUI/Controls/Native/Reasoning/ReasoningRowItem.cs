using Microsoft.UI.Xaml.Automation;

namespace DevWinUI;

public sealed partial class ReasoningRowItem : Control
{
    public ReasoningRow? Row
    {
        get { return (ReasoningRow?)GetValue(RowProperty); }
        set { SetValue(RowProperty, value); }
    }

    public static readonly DependencyProperty RowProperty =
        DependencyProperty.Register(nameof(Row), typeof(ReasoningRow), typeof(ReasoningRowItem), new PropertyMetadata(null, (d, _) => ((ReasoningRowItem)d).Apply()));

    public RowsKind Kind
    {
        get { return (RowsKind)GetValue(KindProperty); }
        set { SetValue(KindProperty, value); }
    }

    public static readonly DependencyProperty KindProperty =
        DependencyProperty.Register(nameof(Kind), typeof(RowsKind), typeof(ReasoningRowItem), new PropertyMetadata(RowsKind.Steps, (d, _) => ((ReasoningRowItem)d).Apply()));

    public bool IsSelected
    {
        get { return (bool)GetValue(IsSelectedProperty); }
        set { SetValue(IsSelectedProperty, value); }
    }

    public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(ReasoningRowItem), new PropertyMetadata(false, (d, _) => ((ReasoningRowItem)d).Apply()));

    Button _button = null!;
    TextBlock _label = null!;
    TextBlock _host = null!;
    TextBlock _added = null!;
    TextBlock _removed = null!;
    bool _templated;

    public ReasoningRowItem()
    {
        DefaultStyleKey = typeof(ReasoningRowItem);
    }

    internal event Action<ReasoningRowItem>? Clicked;

    protected override void OnApplyTemplate()
    {
        if (_templated) _button.Click -= OnClick;

        base.OnApplyTemplate();

        _button = (Button)GetTemplateChild("PART_Button");
        _label = (TextBlock)GetTemplateChild("PART_Label");
        _host = (TextBlock)GetTemplateChild("PART_Host");
        _added = (TextBlock)GetTemplateChild("PART_Added");
        _removed = (TextBlock)GetTemplateChild("PART_Removed");

        _button.Click += OnClick;
        _templated = true;
        Apply();
    }

    void OnClick(object sender, RoutedEventArgs e)
    {
        if (Kind == RowsKind.Search && Row?.Url is { } url) _ = Windows.System.Launcher.LaunchUriAsync(url);
        else if (Kind == RowsKind.Coding) Clicked?.Invoke(this);
    }

    void Apply()
    {
        if (!_templated || Row is not { } row) return;

        VisualStateManager.GoToState(this, Kind.ToString(), false);
        VisualStateManager.GoToState(this, row.Status.ToString(), false);
        VisualStateManager.GoToState(this, IsSelected ? "Selected" : "Unselected", false);

        _label.Text = row.Label;
        _host.Text = row.Url?.Host ?? string.Empty;
        _added.Text = row.Added > 0 ? $"+{row.Added}" : string.Empty;
        _removed.Text = row.Removed > 0 ? $"−{row.Removed}" : string.Empty;

        bool interactive = Kind == RowsKind.Coding || Kind == RowsKind.Search && row.Url is not null;
        _button.IsHitTestVisible = interactive;
        _button.IsTabStop = interactive;

        AutomationProperties.SetName(_button, Kind switch
        {
            RowsKind.Search => $"{row.Label}, {row.Url?.Host}",
            RowsKind.Coding => $"{row.Label}, {row.Added} added, {row.Removed} removed",
            _ => row.Label,
        });

        AutomationProperties.SetItemStatus(this, Kind switch
        {
            RowsKind.Steps => row.Status switch
            {
                ReasoningStepStatus.Done => "done",
                ReasoningStepStatus.Active => "in progress",
                _ => "pending",
            },
            RowsKind.Coding when IsSelected => "selected",
            _ => string.Empty,
        });
    }
}
