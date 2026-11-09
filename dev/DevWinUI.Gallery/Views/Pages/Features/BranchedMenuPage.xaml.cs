namespace DevWinUIGallery.Views;

public sealed partial class BranchedMenuPage : Page
{
    public ObservableCollection<BranchedMenuItem> MenuItems { get; } = new()
    {
        new BranchedMenuItem
        {
            Label = "Getting started",
            Children =
            {
                new BranchedMenuChild { Value = "install", Label = "Installation", Glyph = "\uE896" },
                new BranchedMenuChild { Value = "quickstart", Label = "Quick start", Glyph = "\uE768" },
            },
        },
        new BranchedMenuItem
        {
            Label = "Controls",
            Children =
            {
                new BranchedMenuChild { Value = "arc", Label = "ArcProgress", Glyph = "\uE8A5" },
                new BranchedMenuChild { Value = "donut", Label = "DonutChart", Glyph = "\uE8F1" },
                new BranchedMenuChild { Value = "beam", Label = "BorderBeam", Glyph = "\uE7C3" },
            },
        },
        new BranchedMenuItem
        {
            Label = "Settings",
            Value = "settings",
        },
    };

    public BranchedMenuPage()
    {
        InitializeComponent();

        MyBranchedMenu.ItemSelected += OnItemSelected;
        MyBranchedMenu.SectionToggled += OnSectionToggled;
    }

    public int ToInt(double value) => (int)value;

    private void OnItemSelected(object? sender, BranchedMenuSelectedEventArgs e)
    {
        TxtSelected.Text = $"Selected: {e.Value}";
    }

    private void OnSectionToggled(object? sender, BranchedMenuToggledEventArgs e)
    {
        TxtToggled.Text = $"Toggled: {MenuItems[e.Index].Label} ({(e.IsOpen ? "opened" : "closed")})";
    }

    private void ExpandAll_Click(object sender, RoutedEventArgs e) => SetAllOpen(true);

    private void CollapseAll_Click(object sender, RoutedEventArgs e) => SetAllOpen(false);

    private void SelectSettings_Click(object sender, RoutedEventArgs e)
    {
        MyBranchedMenu.Select("settings", raiseEvent: true);
    }

    private void SetAllOpen(bool open)
    {
        for (int i = 0; i < MenuItems.Count; i++)
        {
            MyBranchedMenu.SetOpen(i, open);
        }
    }
}
