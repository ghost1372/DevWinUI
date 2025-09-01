namespace DevWinUI;
public partial class MessageBoxOptions
{
    public object? Content { get; set; }
    public string? Title { get; set; }
    public Window? OwnerWindow { get; set; }
    public SystemBackdrop? SystemBackdrop { get; set; } = new MicaBackdrop();
    public FlowDirection FlowDirection { get; set; }
    public MessageBoxButtons Buttons { get; set; }
    public MessageBoxDefaultButton? DefaultButton { get; set; }
    public ElementTheme RequestedTheme { get; set; } = ElementTheme.Default;
    public MessageBoxIcon Icon { get; set; } = MessageBoxIcon.None;
    public UnderlayMode Underlay { get; set; } = UnderlayMode.SmokeLayer;
    public UnderlaySystemBackdropOptions UnderlaySystemBackdrop { get; set; } = new UnderlaySystemBackdropOptions();

    public bool IsModal { get; set; }
    public bool IsResizable { get; set; }
    public bool HasTitleBar { get; set; } = true;
    public bool CenterInParent { get; set; } = true;
}
