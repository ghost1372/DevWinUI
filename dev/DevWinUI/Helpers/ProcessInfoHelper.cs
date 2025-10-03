using System.Diagnostics;

namespace DevWinUI;
public static partial class ProcessInfoHelper
{
    private static readonly FileVersionInfo _fileVersionInfo;
    private static readonly Process _process;
    static ProcessInfoHelper()
    {
        _process = Process.GetCurrentProcess();
        _fileVersionInfo = _process.MainModule.FileVersionInfo;
    }

    /// <summary>
    /// Returns the version string.
    /// </summary>
    public static string Version => GetVersion()?.ToString();

    /// <summary>
    /// Returns "Dev" if compiled in Debug mode; otherwise, returns the version string.
    /// </summary>
    public static string VersionDev
    {
        get
        {
#if DEBUG
            return "Dev";
#else
            return GetVersion()?.ToString();
#endif
        }
    }

    /// <summary>
    /// Returns the version string prefixed with 'v'.
    /// </summary>
    public static string VersionWithPrefix => $"v{Version}";

    /// <summary>
    /// Returns "Dev" if compiled in Debug mode; otherwise, returns the version string  prefixed with 'v'.
    /// </summary>
    public static string VersionWithPrefixDev
    {
        get
        {
#if DEBUG
            return "Dev";
#else
            return $"v{Version}";
#endif
        }
    }


    /// <summary>
    /// Retrieves the product name. If not available, it returns 'Unknown Product'.
    /// </summary>
    public static string ProductName => _fileVersionInfo?.ProductName ?? "Unknown Product";

    /// <summary>
    /// Retrieves the product name with dots (.) replaced by spaces. 
    /// If the product name is not available, returns "Unknown Product".
    /// </summary>
    public static string ProductNameDisplay => _fileVersionInfo?.ProductName?.Replace(".", " ") ?? "Unknown Product";

    /// <summary>
    /// Retrieves the product name. 
    /// If compiled in Debug mode, appends "Dev" to indicate a development build. 
    /// If the product name is not available, returns "Unknown Product".
    /// </summary>
    public static string ProductNameDev
    {
        get
        {
#if DEBUG
            return ProductName + " Dev";
#else
            return ProductName;
#endif
        }
    }

    /// <summary>
    /// Retrieves the product name with dots (.) replaced by spaces. 
    /// If compiled in Debug mode, appends "Dev" to indicate a development build. 
    /// If the product name is not available, returns "Unknown Product".
    /// </summary>
    public static string ProductNameDisplayDev
    {
        get
        {
#if DEBUG
            return ProductNameDisplay + " Dev";
#else
            return ProductNameDisplay;
#endif
        }
    }


    /// <summary>
    /// Combines the product name and version into a single string. The version includes a prefix.
    /// </summary>
    public static string ProductNameAndVersion => $"{ProductName} {VersionWithPrefix}";

    /// <summary>
    /// Combines the product name (with dots replaced by spaces) and version into a single string. 
    /// The version includes a prefix. If the product name is not available, "Unknown Product" is used.
    /// </summary>
    public static string ProductNameAndVersionDisplay => $"{ProductNameDisplay} {VersionWithPrefix}";

    /// <summary>
    /// Combines the product name and version into a single string. 
    /// If compiled in Debug mode, appends "Dev" to the product name to indicate a development build. 
    /// The version includes a prefix. If the product name is not available, "Unknown Product" is used.
    /// </summary>
    public static string ProductNameAndVersionDev => $"{ProductNameDev} {VersionWithPrefix}";

    /// <summary>
    /// Combines the product name (with dots replaced by spaces) and version into a single string. 
    /// If compiled in Debug mode, appends "Dev" to the product name to indicate a development build. 
    /// The version includes a prefix. If the product name is not available, "Unknown Product" is used.
    /// </summary>
    public static string ProductNameAndVersionDisplayDev => $"{ProductNameDisplayDev} {VersionWithPrefix}";

    /// <summary>
    /// Returns the company name of the publisher. If not available, it defaults to 'Unknown Publisher'.
    /// </summary>
    public static string Publisher => _fileVersionInfo?.CompanyName ?? "Unknown Publisher";

    public static Version GetVersion()
    {
        return new Version(_fileVersionInfo.FileMajorPart, _fileVersionInfo.FileMinorPart, _fileVersionInfo.FileBuildPart, _fileVersionInfo.FilePrivatePart);
    }

    /// <summary>
    /// Retrieves the file version information for the current assembly.
    /// </summary>
    /// <returns>Returns a FileVersionInfo object containing version details.</returns>
    public static FileVersionInfo GetFileVersionInfo()
    {
        return _fileVersionInfo;
    }

    /// <summary>
    /// Retrieves the current process instance.
    /// </summary>
    /// <returns>Returns the current Process object.</returns>
    public static Process GetProcess()
    {
        return _process;
    }
}
