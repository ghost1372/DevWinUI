using Microsoft.Win32;
using Windows.Win32.System.SystemInformation;

namespace DevWinUI;
public static partial class OSVersionHelper
{
    public static readonly Version OSVersion = GetOSVersion();

    /// <summary>
    /// Windows NT
    /// </summary>
    public static bool IsWindowsNT { get; } = Environment.OSVersion.Platform == PlatformID.Win32NT;

    /// <summary>
    /// Windows 10 Redstone5 Version 1809 Build 17763 (October 2018 Update)
    /// </summary>
    public static bool IsWindows10_1809 { get; } = IsWindowsNT && OSVersion == new Version(10, 0, 17763, OSVersion.Revision);

    /// <summary>
    /// Windows 10 Redstone5 Version 1809 Build 17763 Or Greater (October 2018 Update)
    /// </summary>
    public static bool IsWindows10_1809_OrGreater { get; } = IsWindowsNT && OSVersion >= new Version(10, 0, 17763, OSVersion.Revision);

    /// <summary>
    ///     Windows 11 Build 22000
    /// </summary>
    public static bool IsWindows11_22000 { get; } = IsWindowsNT && OSVersion == new Version(10, 0, 22000, OSVersion.Revision);

    /// <summary>
    ///     Windows 11 Build 22000 Or Greater
    /// </summary>
    public static bool IsWindows11_22000_OrGreater { get; } = IsWindowsNT && OSVersion >= new Version(10, 0, 22000, OSVersion.Revision);

    /// <summary>
    ///     Windows 11 Build 22621
    /// </summary>
    public static bool IsWindows11_22621 { get; } = IsWindowsNT && OSVersion == new Version(10, 0, 22621, OSVersion.Revision);

    /// <summary>
    ///     Windows 11 Build 22621 Or Greater
    /// </summary>
    public static bool IsWindows11_22621_OrGreater { get; } = IsWindowsNT && OSVersion >= new Version(10, 0, 22621, OSVersion.Revision);

    /// <summary>
    ///     Windows 11 Build 22631
    /// </summary>
    public static bool IsWindows11_22631 { get; } = IsWindowsNT && OSVersion == new Version(10, 0, 22631, OSVersion.Revision);

    /// <summary>
    ///     Windows 11 Build 22631 Or Greater
    /// </summary>
    public static bool IsWindows11_22631_OrGreater { get; } = IsWindowsNT && OSVersion >= new Version(10, 0, 22631, OSVersion.Revision);

    /// <summary>
    ///     Windows 11 Build 26100
    /// </summary>
    public static bool IsWindows11_26100 { get; } = IsWindowsNT && OSVersion == new Version(10, 0, 26100, OSVersion.Revision);

    /// <summary>
    ///     Windows 11 Build 26100 Or Greater
    /// </summary>
    public static bool IsWindows11_26100_OrGreater { get; } = IsWindowsNT && OSVersion >= new Version(10, 0, 26100, OSVersion.Revision);

    /// <summary>
    /// Retrieves the operating system version as a Version object, optionally using the registry for the revision
    /// number.
    /// </summary>
    /// <param name="useRegistryForRevision">Determines whether to fetch the revision number from the system registry.</param>
    /// <returns>Returns a Version object representing the OS version, including major, minor, build, and revision numbers.</returns>
    /// <exception cref="Win32Exception">Thrown when the call to retrieve the OS version fails.</exception>
    public static Version GetOSVersion(bool useRegistryForRevision = true)
    {
        var osv = new OSVERSIONINFOW
        {
            dwOSVersionInfoSize = (uint)Marshal.SizeOf<OSVERSIONINFOW>()
        };

        int result = Windows.Wdk.PInvoke.RtlGetVersion(ref osv);

        if (result < 0)
        {
            throw new Win32Exception(result);
        }

        // Get Revision Number

        int revisionNumber = 0;
        if (useRegistryForRevision)
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            if (registryKey != null)
            {
                var ubr = registryKey.GetValue("UBR");
                if (ubr != null)
                {
                    revisionNumber = Convert.ToInt32(ubr);
                }
            }
        }

        return new Version((int)osv.dwMajorVersion, (int)osv.dwMinorVersion, (int)osv.dwBuildNumber, revisionNumber);
    }

    public static bool IsEqualOrGreater(Version version)
    {
        return IsWindowsNT && OSVersion >= new Version(version.Major, version.Minor, version.Build, version.Revision);
    }
}
