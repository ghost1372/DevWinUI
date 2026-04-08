using Microsoft.UI.Xaml.Controls.Primitives;

namespace DevWinUI;

public class SystemTrayIconEventArgs : EventArgs
{
    internal SystemTrayIconEventArgs() { }
    public FlyoutBase? Flyout { get; set; }

    public bool Handled { get; set; }
}
