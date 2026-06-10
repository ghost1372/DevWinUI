namespace DevWinUIGallery.Views;

public sealed partial class MaterialAttachPage : Page
{
    public MaterialAttachPage()
    {
        this.InitializeComponent();

        var items = new List<string>();
        for (int i = 0; i < 6; i++)
        {
            items.Add($"Item {i}");
        }

        ComboBox1.ItemsSource = items;
        ComboBox2.ItemsSource = items;
        AutoSuggestBox1.ItemsSource = items;
        AutoSuggestBox2.ItemsSource = items;
    }
}
