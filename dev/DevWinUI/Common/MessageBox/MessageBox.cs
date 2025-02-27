using WinRT.Interop;

namespace DevWinUI;
public static class MessageBox
{
    public static MessageBoxResult Show(Microsoft.UI.Xaml.Window window, string message, string title, MessageBoxStyle messageBoxStyle)
    {
        return Show(WindowNative.GetWindowHandle(window), message, title, messageBoxStyle);
    }
    public static MessageBoxResult Show(IntPtr hwnd, string message, string title, MessageBoxStyle messageBoxStyle)
    {
        Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE mbStyle = 0;

        if (messageBoxStyle.HasFlag(MessageBoxStyle.AbortRetryIgnore))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_ABORTRETRYIGNORE;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.ApplicationModal))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_APPLMODAL;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.CancelTryAgainContinue))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_CANCELTRYCONTINUE;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.DefaultDesktopOnly))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_DEFAULT_DESKTOP_ONLY;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.DefualtButton1))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_DEFBUTTON1;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.DefualtButton2))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_DEFBUTTON2;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.DefualtButton3))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_DEFBUTTON3;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.DefualtButton4))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_DEFBUTTON4;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.DefualtMask))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_DEFMASK;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.Help))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_HELP;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.IconAsterisk))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_ICONASTERISK;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.IconError))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_ICONERROR;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.IconExclamation))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_ICONEXCLAMATION;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.IconHand))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_ICONHAND;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.IconInformation))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_ICONINFORMATION;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.IconMask))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_ICONMASK;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.IconQuestion))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_ICONQUESTION;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.IconStop))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_ICONSTOP;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.IconWarning))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_ICONWARNING;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.MiscMask))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_MISCMASK;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.ModeMask))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_MODEMASK;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.NoFocus))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_NOFOCUS;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.Ok))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_OK;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.OkCancel))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_OKCANCEL;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.RetryCancel))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_RETRYCANCEL;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.Right))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_RIGHT;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.RtlReading))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_RTLREADING;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.ServiceNotification))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_SERVICE_NOTIFICATION;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.ServiceNotificationNT3X))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_SERVICE_NOTIFICATION_NT3X;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.SetForeground))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_SETFOREGROUND;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.SystemModal))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_SYSTEMMODAL;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.TaskModal))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_TASKMODAL;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.Topmost))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_TOPMOST;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.TypeMask))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_TYPEMASK;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.UserIcon))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_USERICON;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.YesNo))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_YESNO;
        if (messageBoxStyle.HasFlag(MessageBoxStyle.YesNoCancel))
            mbStyle |= Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_YESNOCANCEL;

        var result = PInvoke.MessageBox(new HWND(hwnd), message, title, mbStyle);
        switch (result)
        {
            case Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_RESULT.IDOK:
                return MessageBoxResult.OK;
            case Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_RESULT.IDCANCEL:
                return MessageBoxResult.CANCEL;
            case Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_RESULT.IDABORT:
                return MessageBoxResult.ABORT;
            case Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_RESULT.IDRETRY:
                return MessageBoxResult.RETRY;
            case Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_RESULT.IDIGNORE:
                return MessageBoxResult.IGNORE;
            case Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_RESULT.IDYES:
                return MessageBoxResult.YES;
            case Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_RESULT.IDNO:
                return MessageBoxResult.NO;
            case Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_RESULT.IDCLOSE:
                return MessageBoxResult.CLOSE;
            case Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_RESULT.IDHELP:
                return MessageBoxResult.HELP;
            case Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_RESULT.IDTRYAGAIN:
                return MessageBoxResult.TRYAGAIN;
            case Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_RESULT.IDCONTINUE:
                return MessageBoxResult.CONTINUE;
            case Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_RESULT.IDASYNC:
                return MessageBoxResult.ASYNC;
            case Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_RESULT.IDTIMEOUT:
                return MessageBoxResult.TIMEOUT;
            default:
                return MessageBoxResult.OK;
        }
    }

    public static MessageBoxResult Show(Microsoft.UI.Xaml.Window window, string message, string title)
    {
        return Show(WindowNative.GetWindowHandle(window), message, title);
    }
    public static MessageBoxResult Show(IntPtr hwnd, string message, string title)
    {
        return Show(hwnd, message, title, MessageBoxStyle.Ok);
    }

    public static MessageBoxResult Show(Microsoft.UI.Xaml.Window window, string message, MessageBoxStyle messageBoxStyle)
    {
        return Show(WindowNative.GetWindowHandle(window), message, messageBoxStyle);
    }
    public static MessageBoxResult Show(IntPtr hwnd, string message, MessageBoxStyle messageBoxStyle)
    {
        return Show(hwnd, message, ProcessInfoHelper.ProductName, messageBoxStyle);
    }

    public static MessageBoxResult Show(Window window, string message)
    {
        return Show(WindowNative.GetWindowHandle(window), message);
    }
    public static MessageBoxResult Show(IntPtr hwnd, string message)
    {
        return Show(hwnd, message, ProcessInfoHelper.ProductName, MessageBoxStyle.Ok);
    }

    public static MessageBoxResult ShowInformation(Microsoft.UI.Xaml.Window window, string message, string title)
    {
        return ShowInformation(WindowNative.GetWindowHandle(window), message, title);
    }
    public static MessageBoxResult ShowInformation(IntPtr hwnd, string message, string title)
    {
        return Show(hwnd, message, title, MessageBoxStyle.Ok | MessageBoxStyle.IconInformation);
    }

    public static MessageBoxResult ShowInformation(Microsoft.UI.Xaml.Window window, string message)
    {
        return ShowInformation(WindowNative.GetWindowHandle(window), message);
    }
    public static MessageBoxResult ShowInformation(IntPtr hwnd, string message)
    {
        return Show(hwnd, message, ProcessInfoHelper.ProductName, MessageBoxStyle.Ok | MessageBoxStyle.IconInformation);
    }

    public static MessageBoxResult ShowError(Microsoft.UI.Xaml.Window window, string message, string title)
    {
        return ShowError(WindowNative.GetWindowHandle(window), message, title);
    }
    public static MessageBoxResult ShowError(IntPtr hwnd, string message, string title)
    {
        return Show(hwnd, message, title, MessageBoxStyle.Ok | MessageBoxStyle.IconError);
    }
    public static MessageBoxResult ShowError(Microsoft.UI.Xaml.Window window, string message)
    {
        return ShowError(WindowNative.GetWindowHandle(window), message);
    }
    public static MessageBoxResult ShowError(IntPtr hwnd, string message)
    {
        return Show(hwnd, message, ProcessInfoHelper.ProductName, MessageBoxStyle.Ok | MessageBoxStyle.IconError);
    }

    public static MessageBoxResult ShowWarning(Microsoft.UI.Xaml.Window window, string message, string title)
    {
        return ShowWarning(WindowNative.GetWindowHandle(window), message, title);
    }
    public static MessageBoxResult ShowWarning(IntPtr hwnd, string message, string title)
    {
        return Show(hwnd, message, title, MessageBoxStyle.Ok | MessageBoxStyle.IconWarning);
    }

    public static MessageBoxResult ShowWarning(Microsoft.UI.Xaml.Window window, string message)
    {
        return ShowWarning(WindowNative.GetWindowHandle(window), message);
    }
    public static MessageBoxResult ShowWarning(IntPtr hwnd, string message)
    {
        return Show(hwnd, message, ProcessInfoHelper.ProductName, MessageBoxStyle.Ok | MessageBoxStyle.IconWarning);
    }
}
