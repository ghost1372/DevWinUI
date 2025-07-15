using Serilog;

namespace DevWinUIGallery.Common;
public static class LoggerSetup
{
    public static ILogger Logger { get; private set; }

    public static void ConfigureLogger()
    {
        if (!Directory.Exists(Constants.LogDirectoryPath))
        {
            Directory.CreateDirectory(Constants.LogDirectoryPath);
        }

        Logger = new LoggerConfiguration()
            .Enrich.WithProperty("Version", ProcessInfoHelper.Version)
            .WriteTo.File(Constants.LogFilePath, rollingInterval: RollingInterval.Day)
            .WriteTo.Debug()
            .CreateLogger();
    }
}

