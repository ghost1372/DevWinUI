using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;

namespace DevWinUIGallery.Views;

public sealed partial class SearchBoxPage : Page
{
    private static readonly FlyoutPlacementMode[] PlacementModes =
    {
        FlyoutPlacementMode.Auto,
        FlyoutPlacementMode.Bottom,
        FlyoutPlacementMode.Top,
        FlyoutPlacementMode.Left,
        FlyoutPlacementMode.Right
    };

    private List<SearchResultItem> _all;

    public SearchBoxPage()
    {
        InitializeComponent();

        _all = CreateItems();

        Search.DefaultMenuItems.Add(new SearchMenuItem { Text = "Open", IconSource = new FontIconSource { Glyph = "\uE8E5" }, Tag = "open" });
        Search.DefaultMenuItems.Add(new SearchMenuItem { Text = "Open file location", IconSource = new FontIconSource { Glyph = "\uE838" }, Tag = "location" });
        Search.DefaultMenuItems.Add(new SearchMenuItem { IsSeparator = true });
        Search.DefaultMenuItems.Add(new SearchMenuItem { Text = "Copy path", IconSource = new SymbolIconSource { Symbol = Symbol.Copy }, Tag = "copy" });
        Search.DefaultMenuItems.Add(new SearchMenuItem { Text = "Remove from list", IconSource = new SymbolIconSource { Symbol = Symbol.Delete }, Tag = "remove" });

        Refill(string.Empty);
    }

    private static List<SearchResultItem> CreateItems()
    {
        return new List<SearchResultItem>
        {
            new()
            {
                Title = "Italy 2026 Itinerary",
                Subtitle = "25 minutes ago • Word",
                IconSource = new FontIconSource { Glyph = "\uE8A5" },
                IconBrush = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 43, 87, 154)),
                Details = new List<SearchBoxDetailRow>
                {
                    new("Location", @"C:\Users\Casey\Documents"),
                    new("Author", "Casey Jensen")
                }
            },
            new()
            {
                Title = "Screenshot 2026-09-14 162854.png",
                Subtitle = "4 hours ago • PNG File",
                IconSource = new FontIconSource { Glyph = "\uEB9F" },
                IconBrush = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 16, 124, 16)),
                Details = new List<SearchBoxDetailRow>
                {
                    new("Dimension", "1920 x 1080"),
                    new("File Size", "812 KB")
                }
            },
            new()
            {
                Title = "Photos",
                Subtitle = "4 hours ago • App",
                IconSource = new SymbolIconSource { Symbol = Symbol.Pictures },
                IconBrush = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 0, 120, 212))
            },
            new()
            {
                Title = "Tuscany Vineyard Tour & Tasting Guide",
                Subtitle = "1 day ago • PowerPoint",
                IconSource = new FontIconSource { Glyph = "\uE8A5" },
                IconBrush = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 196, 62, 28)),
                Details = new List<SearchBoxDetailRow>
                {
                    new("Location", @"C:\Users\Casey\Documents"),
                    new("Author", "Casey Jensen")
                }
            },
            new()
            {
                Title = "Snipping Tool",
                Subtitle = "3 days ago • App",
                IconSource = new FontIconSource { Glyph = "\uE70F" },
                IconBrush = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 209, 52, 56))
            },
            new()
            {
                Title = "Sound settings",
                Subtitle = "5 days ago • Settings",
                IconSource = new SymbolIconSource { Symbol = Symbol.Volume },
                IconBrush = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 96, 94, 92))
            },
            new()
            {
                Title = "Turn on do not disturb",
                Subtitle = "New • Action",
                IconSource = new FontIconSource { Glyph = "\uE7ED" },
                IconBrush = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 96, 94, 92))
            }
        };
    }

    private void Refill(string query)
    {
        Search.Results.Clear();

        var matches = string.IsNullOrWhiteSpace(query)
            ? _all
            : _all.Where(i => i.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
                              || i.Subtitle.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

        foreach (var item in matches)
        {
            Search.Results.Add(item);
        }
    }

    private void Search_QueryChanged(object sender, SearchEventArgs e)
    {
        Refill(e.Query);
    }

    private void Search_ResultInvoked(object sender, SearchResultEventArgs e)
    {
        StatusText.Text = e.Item != null ? $"Opened: {e.Item.Title}" : $"Search: {e.Query}";
    }

    private void Search_MenuItemInvoked(object sender, SearchMenuItemEventArgs e)
    {
        if (e.MenuItem.Tag as string == "remove")
        {
            _all.Remove(e.Item);
            Refill(Search.QueryText);
        }

        StatusText.Text = $"{e.MenuItem.Text}: {e.Item.Title}";
    }

    private void Show_Click(object sender, RoutedEventArgs e)
    {
        Search.Show();
    }

    private void Hide_Click(object sender, RoutedEventArgs e)
    {
        Search.Hide();
    }

    private void ResetItems_Click(object sender, RoutedEventArgs e)
    {
        _all = CreateItems();
        Refill(Search.QueryText);
        StatusText.Text = "Items restored";
    }

    private void FlyoutWidth_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (Search is null || double.IsNaN(args.NewValue))
        {
            return;
        }

        Search.FlyoutWidth = args.NewValue;
    }

    private void ResultsMaxHeight_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (Search is null || double.IsNaN(args.NewValue))
        {
            return;
        }

        Search.ResultsMaxHeight = args.NewValue;
    }

    private void Placement_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Search is null)
        {
            return;
        }

        Search.FlyoutPlacement = PlacementModes[((ComboBox)sender).SelectedIndex];
    }
}
