namespace DevWinUI;
public static class MessageBox
{
    private static MessageBoxResult ShowBase(IntPtr hwnd, string message, string title, MessageBoxStyle messageBoxStyle)
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

    /// <summary>
    /// Displays a message box with specified options and returns the user's response.
    /// </summary>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <param name="title">Sets the title of the message box window.</param>
    /// <param name="messageBoxStyle">Determines the style and buttons that will be available in the message box.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    private static MessageBoxResult Show(string message, string title, MessageBoxStyle messageBoxStyle)
    {
        return Show(IntPtr.Zero, message, title, messageBoxStyle);
    }

    /// <summary>
    /// Displays a message box with specified options and returns the user's response.
    /// </summary>
    /// <param name="hwnd">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <param name="title">Sets the title of the message box window.</param>
    /// <param name="messageBoxStyle">Determines the style and buttons that will be available in the message box.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    private static MessageBoxResult Show(IntPtr hwnd, string message, string title, MessageBoxStyle messageBoxStyle)
    {
        return ShowBase(hwnd, message, title, messageBoxStyle);
    }

    /// <summary>
    /// Displays a message box with an OK button to the user.
    /// </summary>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <param name="title">Sets the title of the message box window.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult Show(string message, string title)
    {
        return Show(IntPtr.Zero, message, title);
    }

    /// <summary>
    /// Displays a message box with an OK button to the user.
    /// </summary>
    /// <param name="hwnd">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <param name="title">Sets the title of the message box window.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult Show(IntPtr hwnd, string message, string title)
    {
        return ShowBase(hwnd, message, title, MessageBoxStyle.Ok);
    }

    /// <summary>
    /// Displays a message box with specified text and style. It also includes the product name in the message box.
    /// </summary>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <param name="messageBoxStyle">Determines the style and buttons that will be available in the message box.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult Show(string message, MessageBoxStyle messageBoxStyle)
    {
        return Show(IntPtr.Zero, message, messageBoxStyle);
    }

    /// <summary>
    /// Displays a message box with specified text and style. It also includes the product name in the message box.
    /// </summary>
    /// <param name="hwnd">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <param name="messageBoxStyle">Determines the style and buttons that will be available in the message box.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult Show(IntPtr hwnd, string message, MessageBoxStyle messageBoxStyle)
    {
        return ShowBase(hwnd, message, ProcessInfoHelper.ProductName, messageBoxStyle);
    }

    /// <summary>
    /// Displays a message box with a specified message and an OK button.
    /// </summary>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <returns>Returns the result of the message box interaction.</returns>
    public static MessageBoxResult Show(string message)
    {
        return Show(IntPtr.Zero, message);
    }

    /// <summary>
    /// Displays a message box with a specified message and an OK button.
    /// </summary>
    /// <param name="hwnd">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <returns>Returns the result of the message box interaction.</returns>
    public static MessageBoxResult Show(IntPtr hwnd, string message)
    {
        return ShowBase(hwnd, message, ProcessInfoHelper.ProductName, MessageBoxStyle.Ok);
    }

    /// <summary>
    /// Displays an information message box with an OK button and an information icon.
    /// </summary>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <param name="title">Sets the title of the message box window.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult ShowInformation(string message, string title)
    {
        return ShowInformation(IntPtr.Zero, message, title);
    }

    /// <summary>
    /// Displays an information message box with an OK button and an information icon.
    /// </summary>
    /// <param name="hwnd">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <param name="title">Sets the title of the message box window.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult ShowInformation(IntPtr hwnd, string message, string title)
    {
        return ShowBase(hwnd, message, title, MessageBoxStyle.Ok | MessageBoxStyle.IconInformation);
    }

    /// <summary>
    /// Displays an information message box to the user with an OK button and an information icon.
    /// </summary>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult ShowInformation(string message)
    {
        return ShowInformation(IntPtr.Zero, message);
    }

    /// <summary>
    /// Displays an information message box to the user with an OK button and an information icon.
    /// </summary>
    /// <param name="hwnd">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult ShowInformation(IntPtr hwnd, string message)
    {
        return ShowBase(hwnd, message, ProcessInfoHelper.ProductName, MessageBoxStyle.Ok | MessageBoxStyle.IconInformation);
    }

    /// <summary>
    /// Displays an error message box with a specified message and title.
    /// </summary>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <param name="title">Sets the title of the message box window.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult ShowError(string message, string title)
    {
        return ShowError(IntPtr.Zero, message, title);
    }

    /// <summary>
    /// Displays an error message box with a specified message and title.
    /// </summary>
    /// <param name="hwnd">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <param name="title">Sets the title of the message box window.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult ShowError(IntPtr hwnd, string message, string title)
    {
        return ShowBase(hwnd, message, title, MessageBoxStyle.Ok | MessageBoxStyle.IconError);
    }

    /// <summary>
    /// Displays an error message box to the user with an OK button and an error icon.
    /// </summary>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult ShowError(string message)
    {
        return ShowError(IntPtr.Zero, message);
    }

    /// <summary>
    /// Displays an error message box to the user with an OK button and an error icon.
    /// </summary>
    /// <param name="hwnd">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult ShowError(IntPtr hwnd, string message)
    {
        return ShowBase(hwnd, message, ProcessInfoHelper.ProductName, MessageBoxStyle.Ok | MessageBoxStyle.IconError);
    }

    /// <summary>
    /// Displays a warning message box with an OK button and a warning icon.
    /// </summary>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <param name="title">Sets the title of the message box window.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult ShowWarning(string message, string title)
    {
        return ShowWarning(IntPtr.Zero, message, title);
    }

    /// <summary>
    /// Displays a warning message box with an OK button and a warning icon.
    /// </summary>
    /// <param name="hwnd">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <param name="title">Sets the title of the message box window.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult ShowWarning(IntPtr hwnd, string message, string title)
    {
        return ShowBase(hwnd, message, title, MessageBoxStyle.Ok | MessageBoxStyle.IconWarning);
    }

    /// <summary>
    /// Displays a warning message box to the user with an OK button and a warning icon.
    /// </summary>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult ShowWarning(string message)
    {
        return ShowWarning(IntPtr.Zero, message);
    }

    /// <summary>
    /// Displays a warning message box to the user with an OK button and a warning icon.
    /// </summary>
    /// <param name="hwnd">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult ShowWarning(IntPtr hwnd, string message)
    {
        return ShowBase(hwnd, message, ProcessInfoHelper.ProductName, MessageBoxStyle.Ok | MessageBoxStyle.IconWarning);
    }

    /// <summary>
    /// Displays a message box with specified options and returns the user's response.
    /// </summary>
    /// <param name="window">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <param name="title">Sets the title of the message box window.</param>
    /// <param name="messageBoxStyle">Determines the style and buttons that will be available in the message box.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult Show(Microsoft.UI.Xaml.Window window, string message, string title, MessageBoxStyle messageBoxStyle)
    {
        return ShowBase(WindowNative.GetWindowHandle(window), message, title, messageBoxStyle);
    }

    /// <summary>
    /// Displays a message box with a specified message and title in the context of a given window.
    /// </summary>
    /// <param name="window">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <param name="title">Sets the title of the message box window.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult Show(Microsoft.UI.Xaml.Window window, string message, string title)
    {
        return Show(WindowNative.GetWindowHandle(window), message, title);
    }

    /// <summary>
    /// Displays a message box in the specified window with a given message and style.
    /// </summary>
    /// <param name="window">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <param name="messageBoxStyle">Determines the style and buttons that will be available in the message box.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult Show(Microsoft.UI.Xaml.Window window, string message, MessageBoxStyle messageBoxStyle)
    {
        return Show(WindowNative.GetWindowHandle(window), message, messageBoxStyle);
    }

    /// <summary>
    /// Displays a message box in the specified window with a given message.
    /// </summary>
    /// <param name="window">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult Show(Microsoft.UI.Xaml.Window window, string message)
    {
        return Show(WindowNative.GetWindowHandle(window), message);
    }

    /// <summary>
    /// Displays an information message box in the specified window.
    /// </summary>
    /// <param name="window">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <param name="title">Sets the title of the message box window.</param>
    /// <returns>Returns the result of the message box interaction.</returns>
    public static MessageBoxResult ShowInformation(Microsoft.UI.Xaml.Window window, string message, string title)
    {
        return ShowInformation(WindowNative.GetWindowHandle(window), message, title);
    }

    /// <summary>
    /// Displays an information message box in the specified window.
    /// </summary>
    /// <param name="window">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult ShowInformation(Microsoft.UI.Xaml.Window window, string message)
    {
        return ShowInformation(WindowNative.GetWindowHandle(window), message);
    }

    /// <summary>
    /// Displays an error message in a message box associated with a specified window.
    /// </summary>
    /// <param name="window">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <param name="title">Sets the title of the message box window.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult ShowError(Microsoft.UI.Xaml.Window window, string message, string title)
    {
        return ShowError(WindowNative.GetWindowHandle(window), message, title);
    }

    /// <summary>
    /// Displays an error message in a message box associated with a specified window.
    /// </summary>
    /// <param name="window">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <returns>Returns the result of the message box interaction.</returns>
    public static MessageBoxResult ShowError(Microsoft.UI.Xaml.Window window, string message)
    {
        return ShowError(WindowNative.GetWindowHandle(window), message);
    }

    /// <summary>
    /// Displays a warning message box in the specified window. It provides a way to show a message with a title to the
    /// user.
    /// </summary>
    /// <param name="window">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <param name="title">Sets the title of the message box window.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult ShowWarning(Microsoft.UI.Xaml.Window window, string message, string title)
    {
        return ShowWarning(WindowNative.GetWindowHandle(window), message, title);
    }

    /// <summary>
    /// Displays a warning message box in the specified window. It utilizes the window handle to present the message.
    /// </summary>
    /// <param name="window">Specifies the owner window of the message box.</param>
    /// <param name="message">Text that will be shown in the message box.</param>
    /// <returns>Returns the result of the user's interaction with the message box.</returns>
    public static MessageBoxResult ShowWarning(Microsoft.UI.Xaml.Window window, string message)
    {
        return ShowWarning(WindowNative.GetWindowHandle(window), message);
    }
}
