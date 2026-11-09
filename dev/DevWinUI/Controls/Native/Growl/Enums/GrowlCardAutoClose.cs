namespace DevWinUI;

[Flags]
public enum GrowlCardAutoClose
{
    None = 0,
    Attention = 1,
    Success = 2,
    Warning = 4,
    Error = 8,
    Info = 16,
    Default = Attention | Success | Info,
    All = Attention | Success | Warning | Error | Info
}
