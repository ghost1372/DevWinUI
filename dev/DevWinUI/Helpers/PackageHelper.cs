namespace DevWinUI;
public partial class PackageHelper
{
    public static bool IsPackaged { get; } = GetCurrentPackageName() != null;

    public static string GetCurrentPackageName()
    {
        unsafe
        {
            uint packageFullNameLength = 0;

            var result = PInvoke.GetCurrentPackageFullName(&packageFullNameLength, null);

            if (result == WIN32_ERROR.ERROR_INSUFFICIENT_BUFFER)
            {
                char* packageFullName = stackalloc char[(int)packageFullNameLength];

                result = PInvoke.GetCurrentPackageFullName(&packageFullNameLength, packageFullName);

                if (result == 0) // S_OK or ERROR_SUCCESS
                {
                    return new string(packageFullName, 0, (int)packageFullNameLength);
                }
            }
        }

        return null;
    }

    public static Windows.ApplicationModel.Package GetPackageDetails()
    {
        return Windows.ApplicationModel.Package.Current;
    }
    public static Windows.ApplicationModel.PackageVersion GetPackageVersion()
    {
        return GetPackageDetails().Id.Version;
    }
}
