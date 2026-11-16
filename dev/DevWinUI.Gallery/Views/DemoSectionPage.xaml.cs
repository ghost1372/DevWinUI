using DevWinUI;
using Microsoft.UI.Xaml.Media.Imaging;

namespace DevWinUIGallery.Views;

public sealed partial class DemoSectionPage : Page
{
    public string WASDKVersion { get; } = "Windows App SDK 2.2+";

    private string _uniqueId;
    private string _sectionId;

    public DemoSectionPage()
    {
        this.InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        ActualThemeChanged += (_, _) => DispatcherQueue.TryEnqueue(UpdateHeroImage);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        var item = AppHelper.GetUniqueIdAndSectionId(e.Parameter);
        _uniqueId = item.UniqueId;
        _sectionId = item.SectionId;

        Refresh();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateHeroImage();

        DataSource.Instance.ParsingCompleted -= OnParsingCompleted;
        DataSource.Instance.ParsingCompleted += OnParsingCompleted;

        Refresh();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        DataSource.Instance.ParsingCompleted -= OnParsingCompleted;
    }

    private void OnParsingCompleted(object sender, EventArgs e)
    {
        if (DispatcherQueue.HasThreadAccess)
        {
            Refresh();
        }
        else
        {
            DispatcherQueue.TryEnqueue(Refresh);
        }
    }

    private void UpdateHeroImage()
    {
        if (HeroRoot.Tag is string path && !string.IsNullOrEmpty(path))
        {
            HeroBrush.ImageSource = new BitmapImage(new Uri(path));
        }
    }

    private void Refresh()
    {
        if (string.IsNullOrEmpty(_uniqueId) || string.IsNullOrEmpty(_sectionId))
        {
            return;
        }

        var group = FindUniqueGroup(_uniqueId, _sectionId);

        var items = group?.Items?
            .Where(item => !item.HideItem)
            .OrderBy(item => item.Title)
            .ToList() ?? new List<DataItem>();

        TitleText.Text = group?.Title ?? string.Empty;
        TotalText.Text = items.Count.ToString();
        UpdateChip(items, "New", NewChip, NewText);
        UpdateChip(items, "Updated", UpdatedChip, UpdatedText);

        LoadingPanel.Visibility = HomeLandingPage.ToVisibility(group == null);
        ItemsRepeater.ItemsSource = items;
    }

    private static void UpdateChip(List<DataItem> items, string key, FrameworkElement chip, TextBlock chipText)
    {
        var count = items.Count(item => string.Equals(item.BadgeString, key, StringComparison.Ordinal));
        chip.Visibility = HomeLandingPage.ToVisibility(count > 0);
        chipText.Text = count.ToString();
    }

    // Same matching rules as SectionPage: the group must be unique.
    private static DataGroup FindUniqueGroup(string uniqueId, string sectionId)
    {
        DataGroup match = null;
        foreach (var group in DataSource.Instance.Groups)
        {
            if (string.IsNullOrEmpty(group.UniqueId) || string.IsNullOrEmpty(group.SectionId) ||
                !string.Equals(group.UniqueId, uniqueId, StringComparison.Ordinal) ||
                !string.Equals(group.SectionId, sectionId, StringComparison.Ordinal))
            {
                continue;
            }

            if (match != null)
            {
                return null;
            }

            match = group;
        }

        return match;
    }

    private void OnCardClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: DataItem item })
        {
            App.Current.NavService.NavigateTo(item.UniqueId);
        }
    }
}
