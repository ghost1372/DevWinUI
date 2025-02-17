using Microsoft.Windows.ApplicationModel.Resources;

namespace DevWinUI;
internal static class InternalLocalizationHelper
{
    internal static ResourceManager _resourceManager;
    internal static ResourceContext _resourceContext;
    internal static void InitializeInternalLocalization(ResourceManager resourceManager, ResourceContext resourceContext)
    {
        _resourceManager = resourceManager;
        _resourceContext = resourceContext;
    }
    internal static string GetLocalizedText(string resourceId, ResourceType resourceType)
    {
        if (string.IsNullOrEmpty(resourceId))
            return "{LocalizeId}";

        try
        {
            if (_resourceManager != null && _resourceContext != null)
            {
                var candidate = _resourceManager.MainResourceMap.TryGetValue($"Resources/{resourceId}/{resourceType}", _resourceContext);
                return candidate != null ? candidate.ValueAsString : resourceId;
            }
            else
            {
                throw new NullReferenceException("ResourceManager and ResourceContext is null, make sure you are using ConfigureLocalizer()");
            }
        }
        catch (Exception)
        {
            return resourceId;
        }
    }
}
