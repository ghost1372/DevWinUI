namespace DevWinUIGallery.Views;

public sealed partial class GitHubActivityGridPage : Page
{
    private int _seed = 7;

    public IList<ActivityDay> Days { get; private set; }

    public GitHubActivityGridPage()
    {
        Days = CreateDays(_seed);
        InitializeComponent();
    }

    private int? GetMaxCount(double value) => double.IsNaN(value) ? null : (int)value;

    private void OnRegenerateClick(object sender, RoutedEventArgs e)
    {
        Days = CreateDays(++_seed);
        ActivityGrid.Days = Days;
        PaletteGrid.Days = Days;
    }

    private List<ActivityDay> CreateDays(int seed)
    {
        var random = new Random(seed);
        var today = DateTime.Today;
        var days = new List<ActivityDay>(365);

        for (int i = 364; i >= 0; i--)
        {
            var date = today.AddDays(-i);
            bool weekend = date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
            double idleChance = weekend ? 0.55 : 0.2;
            int count = random.NextDouble() < idleChance ? 0 : random.Next(1, weekend ? 6 : 16);
            days.Add(new ActivityDay(date, count));
        }

        return days;
    }
}
