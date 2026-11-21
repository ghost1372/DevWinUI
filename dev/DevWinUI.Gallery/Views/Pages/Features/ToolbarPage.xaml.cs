namespace DevWinUIGallery.Views;

public sealed partial class ToolbarPage : Page
{
    public ObservableCollection<ToolbarItem> Items { get; set; } = new ObservableCollection<ToolbarItem>()
    {
        new ToolbarItem()
        {
            Label = "Cut",
            ItemType = ToolbarItemTypes.Button,
            ThemedIcon = (Style)Application.Current.Resources["App.ThemedIcons.Cut"]
        },
        new ToolbarItem()
        {
            Label = "Copy",
            ItemType = ToolbarItemTypes.Button,
            ThemedIcon = (Style)Application.Current.Resources["App.ThemedIcons.Copy"]
        },
        new ToolbarItem()
        {
            Label = "Paste",
            ItemType = ToolbarItemTypes.Button,
            ThemedIcon = (Style)Application.Current.Resources["App.ThemedIcons.Paste"]
        },
         new ToolbarItem()
        {
            ItemType = ToolbarItemTypes.Separator,
        },
         new ToolbarItem()
        {
            Label = "Rename",
            ItemType = ToolbarItemTypes.Button,
            ThemedIcon = (Style)Application.Current.Resources["App.ThemedIcons.Rename"]
        },
         new ToolbarItem()
        {
            Label = "Refresh",
            ItemType = ToolbarItemTypes.Button,
            ThemedIcon = (Style)Application.Current.Resources["App.ThemedIcons.Refresh"]
        },
         new ToolbarItem()
        {
            ItemType = ToolbarItemTypes.Separator,
        },
        new ToolbarItem()
        {
            Label = "Cut",
            ItemType = ToolbarItemTypes.ToggleButton,
            ThemedIcon = (Style)Application.Current.Resources["App.ThemedIcons.Cut"]
        },
        new ToolbarItem()
        {
            Label = "Copy",
            ItemType = ToolbarItemTypes.ToggleButton,
            ThemedIcon = (Style)Application.Current.Resources["App.ThemedIcons.Copy"]
        },
        new ToolbarItem()
        {
            Label = "Paste",
            ItemType = ToolbarItemTypes.ToggleButton,
            ThemedIcon = (Style)Application.Current.Resources["App.ThemedIcons.Paste"]
        },
    };
    public ToolbarPage()
    {
        InitializeComponent();
    }
}
