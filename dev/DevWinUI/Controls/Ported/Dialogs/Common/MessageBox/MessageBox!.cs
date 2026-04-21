namespace DevWinUI;

public partial class MessageBox
{
    public static FlowDirection FlowDirection { get; set; }
    public static WindowStartupLocation StartupLocation { get; set; } = WindowStartupLocation.CenterOwner;
    public static bool CanResize { get; set; } = false;
    public static bool HasTitleBar { get; set; } = true;
    public static bool CanDragMoveWindow { get; set; } = false;

}
