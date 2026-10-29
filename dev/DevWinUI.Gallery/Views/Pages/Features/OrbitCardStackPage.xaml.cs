namespace DevWinUIGallery.Views;

public sealed partial class OrbitCardStackPage : Page
{
    private static readonly string[] Glyphs = { "\uE716", "\uE70F", "\uE774", "\uE8F1", "\uE9D9", "\uE713" };

    public ObservableCollection<OrbitCardLayout> LayoutItems { get; set; } = new ObservableCollection<OrbitCardLayout>(Enum.GetValues<OrbitCardLayout>());
    public ObservableCollection<OrbitCardSize> SizeItems { get; set; } = new ObservableCollection<OrbitCardSize>(Enum.GetValues<OrbitCardSize>());

    public IReadOnlyList<OrbitCardItem> Team { get; } = new[]
    {
        new OrbitCardItem("Ava Chen", "Design Lead", "Shapes the visual language and keeps every control consistent.", Stat: "24 projects", Href: new Uri("https://github.com/Ghost1372/DevWinUI")),
        new OrbitCardItem("Liam Novak", "Platform Engineer", "Owns performance, rendering and the build pipeline.", Stat: "18 projects"),
        new OrbitCardItem("Sofia Rossi", "Accessibility Specialist", "Makes sure every interaction works with keyboard and narrator.", Stat: "12 projects"),
        new OrbitCardItem("Noah Weber", "Developer Advocate", "Writes samples and documentation that people actually read.", Stat: "30 projects", Href: new Uri("https://github.com/Ghost1372/DevWinUI")),
        new OrbitCardItem("Mia Tanaka", "Motion Designer", "Tunes every transition until it feels effortless.", Stat: "15 projects"),
        new OrbitCardItem("Omar Haddad", "QA Engineer", "Finds the edge cases before anyone else does.", Stat: "9 projects"),
    };

    public OrbitCardStackPage()
    {
        InitializeComponent();
        IconStack.ImageFactory = (item, index) => new FontIcon { Glyph = Glyphs[index % Glyphs.Length], FontSize = 40 };
    }

    private OrbitCardLabels GetLabels(string link) => new(Link: link);

    private void OnValueChanged(OrbitCardStack sender, int value)
    {
        TxtActive.Text = $"Active: {Team[value].Name}";
    }

    private void OnItemInvoked(OrbitCardStack sender, OrbitCardInvokedEventArgs e)
    {
        TxtInvoked.Text = $"Invoked: {e.Item.Name} (index {e.Index})";
    }
}
