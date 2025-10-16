using Microsoft.UI.Xaml.Controls.Primitives;

namespace DevWinUI;

public class TrayIconEventArgs : EventArgs
{
    internal TrayIconEventArgs()
    {
    }

    public FlyoutBase? Flyout { get; set; }

    public bool Handled { get; set; }
}
