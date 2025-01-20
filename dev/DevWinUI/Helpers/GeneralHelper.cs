using System.Globalization;
using System.Security.Principal;
using System.Web;
using Microsoft.UI.Input;

namespace DevWinUI;
public partial class GeneralHelper
{
    public static bool IsAppRunningAsAdmin()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    public static void ChangeCursor(UIElement uiElement, InputCursor cursor)
    {
        Type type = typeof(UIElement);
        type.InvokeMember("ProtectedCursor", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, uiElement, new object[] { cursor });
    }

    /// <summary>
    /// Convert a Glyph Code like E700, into Unicode Char for using in Code-Behind. output will be \uE700
    /// </summary>
    /// <param name="glyph"></param>
    /// <returns></returns>
    public static char GetGlyphCharacter(string glyph)
    {
        var unicodeValue = Convert.ToInt32(glyph, 16);
        return Convert.ToChar(unicodeValue);
    }

    /// <summary>
    /// Sets the preferred app mode based on the specified element theme.
    /// </summary>
    /// <param name="theme">The element theme to set the preferred app mode to.</param>
    /// <remarks>
    /// This method sets the preferred app mode based on the specified element theme. If the "theme" parameter is set to "Dark", it sets the preferred app mode to "ForceDark", forcing the app to use a dark theme. If the "theme" parameter is set to "Light", it sets the preferred app mode to "ForceLight", forcing the app to use a light theme. Otherwise, it sets the preferred app mode to "Default", using the system default theme. After setting the preferred app mode, the method flushes the menu themes to ensure that any changes take effect immediately. 
    /// </remarks>
    public static void SetPreferredAppMode(ElementTheme theme)
    {
        if (theme == ElementTheme.Dark)
        {
            NativeMethods.SetPreferredAppMode(NativeValues.PreferredAppMode.ForceDark);
        }
        else if (theme == ElementTheme.Light)
        {
            NativeMethods.SetPreferredAppMode(NativeValues.PreferredAppMode.ForceLight);
        }
        else
        {
            NativeMethods.SetPreferredAppMode(NativeValues.PreferredAppMode.Default);
        }
        NativeMethods.FlushMenuThemes();
    }

    public static double GetElementRasterizationScale(UIElement element)
    {
        if (element.XamlRoot != null)
        {
            return element.XamlRoot.RasterizationScale;
        }
        return 0.0;
    }

    public static void EnableSound(ElementSoundPlayerState elementSoundPlayerState = ElementSoundPlayerState.On, bool withSpatial = false)
    {
        ElementSoundPlayer.State = elementSoundPlayerState;

        ElementSoundPlayer.SpatialAudioMode = !withSpatial ? ElementSpatialAudioMode.Off : ElementSpatialAudioMode.On;
    }

    public static TEnum GetEnum<TEnum>(string text) where TEnum : struct
    {
        return !typeof(TEnum).GetTypeInfo().IsEnum
            ? throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.")
            : (TEnum)Enum.Parse(typeof(TEnum), text);
    }

    public static int GetThemeIndex(ElementTheme elementTheme)
    {
        return elementTheme switch
        {
            ElementTheme.Default => 0,
            ElementTheme.Light => 1,
            ElementTheme.Dark => 2,
            _ => 0,
        };
    }

    public static ElementTheme GetElementThemeEnum(int themeIndex)
    {
        return themeIndex switch
        {
            0 => ElementTheme.Default,
            1 => ElementTheme.Light,
            2 => ElementTheme.Dark,
            _ => ElementTheme.Default,
        };
    }

    public static Geometry GetGeometry(string key)
    {
        return (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), (string)Application.Current.Resources[key]);
    }

    /// <summary>
    /// Get Glyph string
    /// </summary>
    /// <param name="key">Example: EA6A</param>
    /// <returns></returns>
    public static string GetGlyph(string key)
    {
        // Try parsing the key as a hexadecimal number
        if (int.TryParse(key, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var codePoint))
        {
            string glyph = char.ConvertFromUtf32(codePoint);

            // Check if the resulting glyph is the null character '\0'
            if (glyph == "\0")
            {
                // Return null or throw an exception, depending on your logic
                return null; // or throw new ArgumentException("Invalid key", nameof(key));
            }

            return glyph;
        }
        else
        {
            // Handle the case where key could not be parsed
            return null; // or throw new ArgumentException("Invalid key", nameof(key));
        }
    }

    public static void SetApplicationLayoutRTL(IntPtr windowHandle)
    {
        int exstyle = PInvoke.GetWindowLong(new HWND(windowHandle), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        PInvoke.SetWindowLong(new HWND(windowHandle), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, exstyle | (int)NativeValues.WindowStyle.WS_EX_LAYOUTRTL);
    }
    public static void SetApplicationLayoutRTL(Window window)
    {
        IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        SetApplicationLayoutRTL(hWnd);
    }

    public static void SetApplicationLayoutLTR(IntPtr windowHandle)
    {
        int exstyle = PInvoke.GetWindowLong(new HWND(windowHandle), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        PInvoke.SetWindowLong(new HWND(windowHandle), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, exstyle | (int)NativeValues.WindowStyle.WS_EX_LAYOUTLTR);
    }
    public static void SetApplicationLayoutLTR(Window window)
    {
        IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        SetApplicationLayoutLTR(hWnd);
    }

    public static string GetDecodedStringFromHtml(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var decoded = HttpUtility.HtmlDecode(text);
        var result = decoded != text;
        return result ? decoded : text;
    }
}
