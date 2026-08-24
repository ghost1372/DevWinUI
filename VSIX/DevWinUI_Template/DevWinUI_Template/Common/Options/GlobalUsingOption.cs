using System;
using System.Collections.Generic;
using System.Text;

namespace DevWinUI_Template;

public class GlobalUsingOption
{
    public GlobalUsingOption(
        Dictionary<string, string> replacementsDictionary,
        string safeProjectName,
        bool? useJsonSettings = null,
        bool? useLogger = null)
    {
        var includeJsonSettings = useJsonSettings ?? WizardConfig.Current.UseJsonSettings;
        var includeLogger = useLogger ?? (WizardConfig.Current.UseFileLogger || WizardConfig.Current.UseDebugLogger);

        var outputBuilder = new StringBuilder();

        if (includeJsonSettings)
        {
            outputBuilder.AppendLine(Environment.NewLine + $"global using static {safeProjectName}.Common.AppHelper;");
        }

        if (includeLogger)
        {
            outputBuilder.AppendLine(Environment.NewLine + $"global using static {safeProjectName}.Common.LoggerSetup;");
        }

        replacementsDictionary.AddIfNotExists("$ExtraGlobalUsing$", outputBuilder.ToString().Trim());
    }
}
