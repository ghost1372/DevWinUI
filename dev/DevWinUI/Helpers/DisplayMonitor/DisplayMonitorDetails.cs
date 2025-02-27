namespace DevWinUI;
public partial class DisplayMonitorDetails
{
    public string Name { get; set; } = string.Empty;

    public Rect RectMonitor { get; set; }

    public Rect RectWork { get; set; }

    public bool IsPrimary { get; set; } = false;
}
