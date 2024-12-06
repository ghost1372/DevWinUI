using System;
using System.Collections.Generic;
using System.Text;
using DevWinUI_Template.WizardUI;

namespace DevWinUI_Template;

public class GlobalUsingOption
{
    public GlobalUsingOption(Dictionary<string, string> replacementsDictionary, string safeProjectName, bool fileLogger, bool debugLogger)
    {
        StringBuilder outputBuilder = new StringBuilder();

        if (WizardConfig.UseJsonSettings)
        {
            outputBuilder.AppendLine(Environment.NewLine + $"global using static {safeProjectName}.Common.AppHelper;");
        }

        if (!fileLogger && !debugLogger)
        {
        }
        else
        {
            outputBuilder.AppendLine(Environment.NewLine + $"global using static {safeProjectName}.Common.LoggerSetup;");
        }

        replacementsDictionary.AddIfNotExists("$ExtraGlobalUsing$", outputBuilder.ToString().Trim());
    }
}
