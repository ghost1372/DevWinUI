using Microsoft.Windows.ApplicationModel.Resources;

namespace DevWinUI;
internal static partial class Helper
{
    internal static string GetLocalizedText(string input, bool useXUid, ResourceManager resourceManager)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        try
        {
            if (useXUid && resourceManager != null)
            {
                var candidate = resourceManager.MainResourceMap.TryGetValue($"Resources/{input}");
                var value = candidate != null ? candidate.ValueAsString : input;
                return value;
            }
        }
        catch (Exception)
        {
            return input;
        }

        return input;
    }
}
