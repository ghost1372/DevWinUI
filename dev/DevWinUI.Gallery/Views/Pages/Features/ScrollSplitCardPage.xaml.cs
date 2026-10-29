namespace DevWinUIGallery.Views;

public sealed partial class ScrollSplitCardPage : Page
{
    private static readonly ScrollSplitCardItem[] AllCards =
    {
        new("Find what moves you", "Browse curated collections tailored to your taste.", "Discover"),
        new("Build without limits", "Compose layouts from flexible, reusable pieces.", "Create"),
        new("Work better together", "Share, comment and iterate in real time.", "Collaborate"),
        new("Ship with confidence", "Automated checks keep every release solid.", "Deliver"),
        new("Learn from the data", "Understand usage and refine every detail.", "Measure"),
    };

    public ObservableCollection<ScrollSplitCardSize> SizeItems { get; set; } = new ObservableCollection<ScrollSplitCardSize>(Enum.GetValues<ScrollSplitCardSize>());
    public ObservableCollection<ScrollSplitCardTone> ToneItems { get; set; } = new ObservableCollection<ScrollSplitCardTone>(Enum.GetValues<ScrollSplitCardTone>());

    public IReadOnlyList<ScrollSplitCardItem> Cards { get; } = AllCards[..3];

    public ScrollSplitCardPage()
    {
        InitializeComponent();
    }

    private static IReadOnlyList<ScrollSplitCardItem> GetCards(double count) => AllCards[..(int)count];

    private static Uri? GetUri(string value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri) ? uri : null;
}
