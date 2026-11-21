namespace DevWinUI;

public class ShortcutValidationEventArgs : EventArgs
{
    public IReadOnlyList<KeyVisualInfo> KeysInfo { get; set; } = null!;
    public ContentDialog ContentDialog { get; set; } = null!;
}
