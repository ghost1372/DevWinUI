namespace DevWinUI;
public partial class GrowlInfo
{
    public string Title { get; set; }
    public string Message { get; set; }
    public string DateTime { get; set; }
    public string ConfirmButtonText { get; set; } = "Confirm";
    public string CloseButtonText { get; set; } = "Close";
    public string Token { get; set; }
    public object Content { get; set; }
    public InfoBarSeverity Severity { get; set; }
    public IconSource IconSource { get; set; }
    public bool IsClosable { get; set; } = true;
    public bool ShowCloseButton { get; set; }
    public bool ShowConfirmButton { get; set; }
    public bool ShowDateTime { get; set; } = true;
    public bool StaysOpen { get; set; }
    public bool IsIconVisible { get; set; } = true;
    public bool UseBlueColorForInfo { get; set; }
    public TimeSpan WaitTime { get; set; } = TimeSpan.FromSeconds(6);
    public Func<object, RoutedEventArgs, bool> ConfirmButtonClicked { get; set; }
    public Func<object, RoutedEventArgs, bool> CloseButtonClicked { get; set; }
}
