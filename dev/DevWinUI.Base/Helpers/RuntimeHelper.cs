using System.Security.Principal;

namespace DevWinUI;

public static partial class RuntimeHelper
{
    /// <summary>
    /// Checks if the application is packaged or UnPackaged.
    /// </summary>
    /// <returns>Returns true if the application is packaged, otherwise false.</returns>
    public static bool IsPackaged()
    {
        return PackageHelper.IsPackaged;
    }

    /// <summary>
    /// Checks if the current application is running with administrative privileges.
    /// </summary>
    /// <returns>Returns true if the application is running as an administrator, otherwise false.</returns>
    public static bool IsAppRunningAsAdmin()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    /// <summary>
    /// Checks if the current process is running with administrative privileges by examining the user's identity.
    /// </summary>
    /// <returns>Returns true if the user belongs to the Builtin Administrators group, otherwise false.</returns>
    public static bool IsCurrentProcessRunningAsAdmin()
    {
        var identity = WindowsIdentity.GetCurrent();
        return identity.Owner?.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid) ?? false;
    }

    /// <summary>
    /// Determine whether the current process is running elevated in a split token session
    /// will not return true if UAC is disabled and the user is running as administrator by default
    /// </summary>
    public static unsafe bool IsCurrentProcessRunningElevated()
    {
        HANDLE tokenHandle;
        if (!PInvoke.OpenProcessToken(PInvoke.GetCurrentProcess(), Windows.Win32.Security.TOKEN_ACCESS_MASK.TOKEN_QUERY, &tokenHandle))
        {
            throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        }

        try
        {
            Windows.Win32.Security.TOKEN_ELEVATION_TYPE elevationType;
            var elevationTypeSize = (uint)Unsafe.SizeOf<Windows.Win32.Security.TOKEN_ELEVATION_TYPE>();
            uint returnLength;

            if (!PInvoke.GetTokenInformation(tokenHandle, Windows.Win32.Security.TOKEN_INFORMATION_CLASS.TokenElevationType, &elevationType, elevationTypeSize, &returnLength))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }

            return elevationType == Windows.Win32.Security.TOKEN_ELEVATION_TYPE.TokenElevationTypeFull;
        }
        finally
        {
            PInvoke.CloseHandle(tokenHandle);
        }
    }

    /// <summary>
    /// Verifies if the current process is running with administrative privileges. Throws an exception if it is not.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException">Thrown when the current process does not have elevated privileges.</exception>
    public static void VerifyCurrentProcessRunningAsAdmin()
    {
        if (!IsCurrentProcessRunningAsAdmin())
        {
            throw new UnauthorizedAccessException("This operation requires elevated privileges.");
        }
    }
}
