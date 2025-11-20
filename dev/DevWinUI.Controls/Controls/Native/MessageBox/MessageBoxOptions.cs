namespace DevWinUI;

public partial class MessageBoxOptions : MessageBoxBaseOptions
{
    public SystemBackdrop? SystemBackdrop { get; set; }

    public bool HasTitleBar { get; set; } = true;
    public bool IsResizable { get; set; } = false;

    public bool CenterInParent { get; set; } = true;

    public static MessageBoxOptions Default => new() { SystemBackdrop = new MicaBackdrop() };
}
