using System.Diagnostics;

namespace DevWinUI;
public static partial class ProcessInfoHelper
{
    private static readonly FileVersionInfo _fileVersionInfo;
    private static readonly Process _process;
    static ProcessInfoHelper()
    {
        using var process = Process.GetCurrentProcess();
        _process = process;
        _fileVersionInfo = process.MainModule.FileVersionInfo;
    }
    public static string Version => GetVersion()?.ToString();
    public static string VersionWithPrefix => $"v{Version}";
    public static string ProductName => _fileVersionInfo?.ProductName ?? "Unknown Product";
    public static string ProductNameAndVersion => $"{ProductName} {VersionWithPrefix}";
    public static string Publisher => _fileVersionInfo?.CompanyName ?? "Unknown Publisher";
    public static Version GetVersion()
    {
        return new Version(_fileVersionInfo.FileMajorPart, _fileVersionInfo.FileMinorPart, _fileVersionInfo.FileBuildPart, _fileVersionInfo.FilePrivatePart);
    }
    public static FileVersionInfo GetFileVersionInfo()
    {
        return _fileVersionInfo;
    }
    public static Process GetProcess()
    {
        return _process;
    }
}
