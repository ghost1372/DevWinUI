using System.Globalization;
using System.Web;
using Microsoft.UI.Input;
using Microsoft.Win32;
using Windows.Globalization;

namespace DevWinUI;
public partial class GeneralHelper
{
    private const string firstRunKey = "IsFirstRun";

    /// <summary>
    /// Determines whether the application is running for the first time.
    /// </summary>
    /// <returns>
    /// Returns <c>true</c> if this is the first run; otherwise, returns <c>false</c>.
    /// </returns>
    /// <remarks>
    /// - If the application is running as a packaged app, it checks a local setting in `ApplicationData.LocalSettings`.
    /// - If the application is running as an unpackaged app, it checks a registry key under `HKEY_CURRENT_USER\Software\{Publisher}\{ProductNameAndVersion}`.
    /// - On the first run, the method updates the respective setting or registry entry to prevent future first-run detections.
    /// </remarks>
    public static bool IsFirstRun()
    {
        if (PackageHelper.IsPackaged)
        {
            var settings = Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalSettings;
            if (settings.Values.TryGetValue(firstRunKey, out object keyExist) &&
                keyExist is bool isFirstRun && isFirstRun)
            {
                return false;
            }

            settings.Values[firstRunKey] = true;
            return true;
        }
        else
        {
            //Todo: Replace Registry with Microsoft.Windows.Storage.ApplicationData.GetForUnPackaged()
            return IsFirstRunForUnPackaged();
        }
    }

    private static bool IsFirstRunForUnPackaged()
    {
        string registryPath = $@"Software\{ProcessInfoHelper.Publisher}\{ProcessInfoHelper.ProductNameAndVersion}";

        using var key = Registry.CurrentUser.OpenSubKey(registryPath, writable: true) ??
                        Registry.CurrentUser.CreateSubKey(registryPath);

        if (key == null) return false;

        var value = key.GetValue(firstRunKey);
        if (value == null || value.ToString().Equals("True", StringComparison.OrdinalIgnoreCase))
        {
            key.SetValue(firstRunKey, "False", RegistryValueKind.String);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Retrieves the application data for a packaged app. If the app is not packaged, an exception is thrown.
    /// </summary>
    /// <returns>Returns the default application data for the current app.</returns>
    /// <exception cref="NotImplementedException">Thrown when attempting to access application data for an unpackaged app, which is not yet implemented.</exception>
    public static Microsoft.Windows.Storage.ApplicationData GetApplicationData()
    {
        if (PackageHelper.IsPackaged)
        {
            return Microsoft.Windows.Storage.ApplicationData.GetDefault();
        }
        else
        {
            throw new NotImplementedException("Microsoft has not implemented GetForUnpackaged in Microsoft.Windows.Storage.ApplicationData yet.");
            //return Microsoft.Windows.Storage.ApplicationData.GetForUnpackaged(ProcessInfoHelper.Publisher, ProcessInfoHelper.ProductName);
        }
    }

    /// <summary>
    /// Changes the cursor appearance for a specified UI element.
    /// </summary>
    /// <param name="uiElement">The visual component whose cursor appearance will be modified.</param>
    /// <param name="cursor">The new cursor style to be applied to the specified UI element.</param>
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

    /// <summary>
    /// Enables sound for the elements
    /// mode.
    /// </summary>
    /// <param name="elementSoundPlayerState">Specifies the audio state to be set for the element sound player.</param>
    /// <param name="withSpatial">Determines whether spatial audio is enabled or disabled.</param>
    public static void EnableSound(ElementSoundPlayerState elementSoundPlayerState = ElementSoundPlayerState.On, bool withSpatial = false)
    {
        ElementSoundPlayer.State = elementSoundPlayerState;

        ElementSoundPlayer.SpatialAudioMode = !withSpatial ? ElementSpatialAudioMode.Off : ElementSpatialAudioMode.On;
    }

    /// <summary>
    /// Retrieves an enumeration value from a string representation. It requires the generic type to be an enum.
    /// </summary>
    /// <typeparam name="TEnum">The generic type must be an enumeration type to convert the string into its corresponding enum value.</typeparam>
    /// <param name="text">The string representation of the enumeration value to be converted.</param>
    /// <returns>The corresponding enumeration value of the specified type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the generic type parameter is not an enumeration.</exception>
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

    /// <summary>
    /// Retrieves the corresponding element theme based on the provided index.
    /// </summary>
    /// <param name="themeIndex">The index used to determine which theme to return.</param>
    /// <returns>Returns the element theme associated with the specified index.</returns>
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

    /// <summary>
    /// Sets the application layout to right-to-left for a specified window.
    /// </summary>
    /// <param name="hwnd"></param>
    public static void SetApplicationLayoutRTL(IntPtr hwnd)
    {
        int exstyle = PInvoke.GetWindowLong(new HWND(hwnd), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        PInvoke.SetWindowLong(new HWND(hwnd), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, exstyle | (int)NativeValues.WindowStyle.WS_EX_LAYOUTRTL);
    }

    /// <summary>
    /// Sets the application layout to right-to-left for a specified window.
    /// </summary>
    /// <param name="window"></param>
    public static void SetApplicationLayoutRTL(Microsoft.UI.Xaml.Window window)
    {
        IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        SetApplicationLayoutRTL(hWnd);
    }

    /// <summary>
    /// Sets the application layout to left-to-right for a specified window.
    /// </summary>
    /// <param name="hwnd"></param>
    public static void SetApplicationLayoutLTR(IntPtr hwnd)
    {
        int exstyle = PInvoke.GetWindowLong(new HWND(hwnd), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        PInvoke.SetWindowLong(new HWND(hwnd), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, exstyle | (int)NativeValues.WindowStyle.WS_EX_LAYOUTLTR);
    }

    /// <summary>
    /// Sets the application layout to left-to-right for the specified window.
    /// </summary>
    /// <param name="window"></param>
    public static void SetApplicationLayoutLTR(Microsoft.UI.Xaml.Window window)
    {
        IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        SetApplicationLayoutLTR(hWnd);
    }

    /// <summary>
    /// Decodes HTML-encoded strings into their plain text representation.
    /// </summary>
    /// <param name="text">The input string that may contain HTML-encoded characters.</param>
    /// <returns>Returns the decoded string if changes were made; otherwise, returns the original string.</returns>
    public static string GetDecodedStringFromHtml(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var decoded = HttpUtility.HtmlDecode(text);
        var result = decoded != text;
        return result ? decoded : text;
    }

    /// <summary>
    /// Determines whether the current system's geographic region matches any of the specified privacy-sensitive region codes.
    /// </summary>
    /// <param name="privacySensitiveRegions">An array of three-letter ISO 3166 country codes (e.g., "USA", "IRN", "CHN") that are considered privacy-sensitive.</param>
    /// <returns></returns>
    public static bool IsPrivacySensitiveRegion(string[] privacySensitiveRegions)
    {
        var geographicRegion = new GeographicRegion();
        return privacySensitiveRegions.Contains(geographicRegion.CodeThreeLetter, StringComparer.OrdinalIgnoreCase);
    }
}
