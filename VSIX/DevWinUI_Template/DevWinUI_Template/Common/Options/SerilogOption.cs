using System;
using System.Collections.Generic;
using System.Linq;
namespace DevWinUI_Template;

public class SerilogOption
{
    public void ConfigSerilog(Dictionary<string, string> replacementsDictionary)
    {
        var hasFileLib = WizardConfig.Current.NuGetPackages.Any(x => x.PackageName.Equals("Serilog.Sinks.File", StringComparison.OrdinalIgnoreCase));
        var hasDebugLib = WizardConfig.Current.NuGetPackages.Any(x => x.PackageName.Equals("Serilog.Sinks.Debug", StringComparison.OrdinalIgnoreCase));

        if (hasFileLib)
        {
            WizardConfig.Current.UseFileLogger = true;
            replacementsDictionary.Add("$SerilogFilePath$", Environment.NewLine + """public static readonly string LogFilePath = Path.Combine(LogDirectoryPath, "Log.txt");""");
            replacementsDictionary.Add("$SerilogFile$", Environment.NewLine + "            .WriteTo.File(Constants.LogFilePath, rollingInterval: RollingInterval.Day)");
        }
        else
        {
            replacementsDictionary.Add("$SerilogFile$", "");
            replacementsDictionary.Add("$SerilogFilePath$", "");
        }

        if (hasDebugLib)
        {
            WizardConfig.Current.UseDebugLogger = true;

            replacementsDictionary.Add("$SerilogDebug$", Environment.NewLine + "            .WriteTo.Debug()");
        }
        else
        {
            replacementsDictionary.Add("$SerilogDebug$", "");
        }

        if (hasDebugLib || hasFileLib)
        {
            replacementsDictionary.Add("$SerilogDirectoryPath$", Environment.NewLine + """public static readonly string LogDirectoryPath = Path.Combine(RootDirectoryPath, "Log");""");
            replacementsDictionary.Add("$UnhandeledException$", Environment.NewLine + Environment.NewLine + """UnhandledException += (s, e) => Logger?.Error(e.Exception, "UnhandledException");""");
            if (WizardConfig.Current.UseJsonSettings && WizardConfig.Current.UseDeveloperModeSetting)
            {
                replacementsDictionary.Add("$ConfigLogger$", Environment.NewLine + Environment.NewLine + """
                    if (Settings.UseDeveloperMode)
                    {
                        ConfigureLogger();
                    }
                    """);
            }
            else
            {
                replacementsDictionary.Add("$ConfigLogger$", Environment.NewLine + Environment.NewLine + "ConfigureLogger();");
            }
        }
        else
        {
            replacementsDictionary.Add("$SerilogDirectoryPath$", "");
            replacementsDictionary.Add("$ConfigLogger$", "");
            replacementsDictionary.Add("$UnhandeledException$", "");
        }
    }
}
