using DevWinUI;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;

namespace DevWinUIGallery.Views;

public sealed partial class HomeLandingPage : Page
{
    public string WASDKVersion { get; } = "Windows App SDK 2.2+";

    public string NewGroupText { get; set; } = "Recently added";
    public string UpdatedGroupText { get; set; } = "Recently updated";
    public string PreviewGroupText { get; set; } = "Preview";

    public HomeLandingPage()
    {
        this.InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        ActualThemeChanged += (_, _) => DispatcherQueue.TryEnqueue(UpdateHeroImage);
    }

    public static Visibility ToVisibility(bool value) => value ? Visibility.Visible : Visibility.Collapsed;

    public static string GetGlyph(object key) => key as string switch
    {
        "New" => "\uE734",
        "Updated" => "\uE777",
        "Preview" => "\uE7B3",
        _ => "\uE71D"
    };

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
            HeroImage.Source = new BitmapImage(new Uri(path));
        }
    }

    private void Refresh()
    {
        var all = DataSource.Instance.Groups
            .Where(group => !group.HideGroup)
            .SelectMany(group => group.Items)
            .Where(item => !item.HideItem)
            .ToList();

        var tagged = all.Where(item => item.BadgeString != null).ToList();
        var sections = new ObservableCollection<GroupInfoList>();

        AddSection(sections, tagged, "New", NewGroupText, NewChip, NewText);
        AddSection(sections, tagged, "Updated", UpdatedGroupText, UpdatedChip, UpdatedText);
        AddSection(sections, tagged, "Preview", PreviewGroupText, null, null);

        TotalText.Text = all.Count.ToString();
        LoadingPanel.Visibility = ToVisibility(all.Count == 0);
        SectionsRepeater.ItemsSource = sections;
    }

    private static void AddSection(ObservableCollection<GroupInfoList> sections, List<DataItem> source, string key, string title, FrameworkElement chip, TextBlock chipText)
    {
        var items = source
            .Where(item => string.Equals(item.BadgeString, key, StringComparison.Ordinal))
            .OrderBy(item => item.Title)
            .ToList();

        if (chip != null)
        {
            chip.Visibility = ToVisibility(items.Count > 0);
            chipText.Text = items.Count.ToString();
        }

        if (items.Count > 0)
        {
            sections.Add(new GroupInfoList(items) { Key = key, Title = title });
        }
    }

    private void OnCardClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: DataItem item })
        {
            App.Current.NavService.NavigateTo(item.UniqueId, item);
        }
    }
}
