namespace DevWinUIGallery.Views;

public sealed partial class Timeline2OrderEvent
{
    public string Status { get; set; }
    public string Date { get; set; }
    public string Icon { get; set; }
    public Timeline2OrderEvent(string status, string date, string icon)
    {
        this.Status = status;
        this.Date = date;
        this.Icon = icon;
    }
}
public sealed partial class Timeline2Page : Page
{
    public List<Timeline2Entry> Entries { get; } = new()
    {
        new() { Date = "Mar 15, 2025", Title = "Project Kickoff", Icon = "\uE7C1", Value = "kickoff", Description = "Kicked off the project with team alignment." },
        new() { Date = "Mar 22, 2025", Title = "Design Phase", Icon = "\uE790", Value = "design", Description = "User research and design workshops." },
        new() { Date = "Mar 29, 2025", Title = "Development Sprint", Icon = "\uE943", Value = "development", Description = "Frontend and backend development." },
        new() { Date = "Apr 5, 2025", Title = "Testing & Deployment", Icon = "\uE73E", Value = "deployment", Description = "QA testing and performance optimization." },
    };

    public List<Timeline2OrderEvent> Orders { get; } = new()
    {
        new("Ordered", "15/10/2020 10:30", "\uE7BF"),
        new("Processing", "15/10/2020 14:00", "\uE895"),
        new("Shipped", "15/10/2020 16:15", "\uE7C0"),
        new("Delivered", "16/10/2020 10:00", "\uE73E"),
    };

    public Timeline2Page()
    {
        InitializeComponent();
    }

    private void OnItemClick(object sender, Timeline2ItemClickEventArgs e)
    {
        if (e.Item is Timeline2Entry entry)
        {
            SelectionText.Text = $"Selected: {entry.Title}";
        }
    }
}
