namespace DevWinUIGallery.Views;

public sealed partial class ThemedIconPage : Page
{
    public List<KeyValuePair<object, Style>> ThemedIconStyles { get; set; }
    public ThemedIconPage()
    {
        InitializeComponent();

        ThemedIconStyles = Application.Current.Resources.Where(r => r.Value is Style s && s.TargetType == typeof(ThemedIcon))
            .Select(r => new KeyValuePair<object, Style>(r.Key, (Style)r.Value)).ToList();

    }
}
