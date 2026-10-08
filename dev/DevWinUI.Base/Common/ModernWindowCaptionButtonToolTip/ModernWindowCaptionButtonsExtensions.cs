// Ported from https://github.com/HO-COOH/WinUIEssentials

using Windows.Win32.UI.WindowsAndMessaging;

namespace DevWinUI;

internal static partial class ModernWindowCaptionButtonsExtensions
{
    public static ModernWindowCaptionButtons FromHitTest(nint hitTest) => hitTest switch
    {
        (nint)PInvoke.HTMINBUTTON => ModernWindowCaptionButtons.Minimize,
        (nint)PInvoke.HTMAXBUTTON => ModernWindowCaptionButtons.Maximize,
        (nint)PInvoke.HTCLOSE => ModernWindowCaptionButtons.Close,
        _ => ModernWindowCaptionButtons.None
    };

    public static bool IsEnabled(this ModernWindowCaptionButtons button, nint hwnd)
    {
        var style = (uint)NativeMethods.GetWindowLongPtr((HWND)hwnd, (int)WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        return button switch
        {
            ModernWindowCaptionButtons.Minimize => (style & (uint)WINDOW_STYLE.WS_MINIMIZEBOX) != 0,
            ModernWindowCaptionButtons.Maximize => (style & (uint)WINDOW_STYLE.WS_MAXIMIZEBOX) != 0,
            ModernWindowCaptionButtons.Close => true,
            _ => false
        };
    }
}
