namespace DevWinUI;
public partial class PackageHelper
{
    /// <summary>
    /// Indicates whether the current application is packaged. Returns true if the package name is not null.
    /// </summary>
    public static bool IsPackaged { get; } = GetCurrentPackageName() != null;

    /// <summary>
    /// Retrieves the full name of the current package.
    /// </summary>
    /// <returns>Returns the package full name as a string or null if an error occurs.</returns>
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

    /// <summary>
    /// Retrieves the current application package details.
    /// </summary>
    /// <returns>Returns the current application package as a Package object.</returns>
    public static Windows.ApplicationModel.Package GetPackageDetails()
    {
        return Windows.ApplicationModel.Package.Current;
    }

    /// <summary>
    /// Retrieves the version of the current application package.
    /// </summary>
    /// <returns>Returns a PackageVersion object representing the application's version.</returns>
    public static Windows.ApplicationModel.PackageVersion GetPackageVersion()
    {
        return GetPackageDetails().Id.Version;
    }

    /// <summary>
    /// Retrieves the package family name from the package details.
    /// </summary>
    /// <returns>Returns the family name as a string.</returns>
    public static string GetPackageFamilyName()
    {
        return GetPackageDetails().Id.FamilyName;
    }
}
