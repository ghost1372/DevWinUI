namespace DevWinUI;
public partial class MessageBoxBaseOptions
{
    /// <summary>
    /// ElementTheme.Default is treated as following owner window
    /// </summary>
    public ElementTheme RequestedTheme { get; set; }

    public UnderlayMode Underlay { get; set; }
    public UnderlaySystemBackdropOptions UnderlaySystemBackdrop { get; set; }

    public FlowDirection FlowDirection { get; set; }
}
