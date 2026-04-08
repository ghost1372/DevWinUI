namespace DevWinUI;

public class ShortcutValidationEventArgs : EventArgs
{
    public IReadOnlyList<KeyVisualInfo> KeysInfo { get; set; }
    public ContentDialog ContentDialog { get; set; }
}
