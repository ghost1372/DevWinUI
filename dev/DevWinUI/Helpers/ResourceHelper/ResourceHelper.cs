using Microsoft.Windows.ApplicationModel.Resources;

namespace DevWinUI;

public partial class ResourceHelper : IResourceHelper
{
    private ResourceManager resourceManager { get; set; }

    public ResourceHelper()
    {
        resourceManager = new ResourceManager();
    }

    public ResourceHelper(ResourceManager resourceManager) : this()
    {
        this.resourceManager = resourceManager ?? new ResourceManager();
    }
    
    /// <summary>
    /// Get All Resources Keys
    /// </summary>
    /// <param name="identifier">Default value is Resources</param>
    /// <returns></returns>
    public List<string> GetAllResourcesKeys(string identifier = null)
    {
        var reslist = new List<string>();

        var rmap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap;
        foreach (var str in rmap.Keys)
        {
            if (str.StartsWith(identifier ?? "Resources"))
            {
                reslist.Add($"/{str}");
            }
        }

        return reslist;
    }

    public void SetLanguage(string language)
    {
        Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = language;
    }

    private string GetStringBase(string key, string language, string filename)
    {
        var resourceContext = resourceManager.CreateResourceContext();
        if (!string.IsNullOrEmpty(language))
        {
            resourceContext.QualifierValues["Language"] = language;
        }

        var candidate = resourceManager.MainResourceMap.TryGetValue($"{filename}/{key}", resourceContext);
        var value = candidate != null ? candidate.ValueAsString : key;
        return value;
    }

    public string GetString(string key)
    {
        return GetStringBase(key, null, "Resources");
    }
    public string GetString(string key, string language)
    {
        return GetStringBase(key, language, "Resources");
    }
    public string GetStringFromResource(string key, string filename)
    {
        return GetStringBase(key, null, filename);
    }

    public string GetStringFromResource(string key, string language, string filename)
    {
        return GetStringBase(key, language, filename);
    }
}
