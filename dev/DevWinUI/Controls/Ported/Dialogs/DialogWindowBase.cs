// https://github.com/SuGar0218/SuGarToolkit.WinUI3

namespace DevWinUI;

public partial class DialogWindowBase : ContentWindow
{
    public DialogWindowBase()
    {
        DefaultStyleKey = typeof(DialogWindowBase);
        ExtendsContentIntoTitleBar = true;
        StartupLocation = WindowStartupLocation.CenterOwner;
        SizeToContent = true;
        CanMinimize = false;
        CanMaximize = false;
        ShowInTaskbar = false;
    }
}
