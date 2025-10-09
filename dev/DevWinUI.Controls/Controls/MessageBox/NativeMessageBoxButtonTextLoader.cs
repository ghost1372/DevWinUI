//https://github.com/SuGar0218/WindowedContentDialog

namespace DevWinUI;

internal partial class NativeMessageBoxButtonTextLoader
{
    public static string OK => field ??= GeneralHelper.LoadNativeString(800u);
    public static string Cancel => field ??= GeneralHelper.LoadNativeString(801u);
    public static string Abort => field ??= GeneralHelper.LoadNativeString(802u);
    public static string Retry => field ??= GeneralHelper.LoadNativeString(803u);
    public static string Ignore => field ??= GeneralHelper.LoadNativeString(804u);
    public static string Yes => field ??= GeneralHelper.LoadNativeString(805u);
    public static string No => field ??= GeneralHelper.LoadNativeString(806u);
    public static string Close => field ??= GeneralHelper.LoadNativeString(807u);
    public static string Help => field ??= GeneralHelper.LoadNativeString(808u);
    public static string TryAgain => field ??= GeneralHelper.LoadNativeString(809u);
    public static string Continue => field ??= GeneralHelper.LoadNativeString(810u);
}
