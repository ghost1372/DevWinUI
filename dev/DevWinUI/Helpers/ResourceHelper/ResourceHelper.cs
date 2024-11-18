using Microsoft.Windows.ApplicationModel.Resources;

namespace DevWinUI;

public partial class ResourceHelper : IResourceHelper
{
    public ResourceManager ResourceManager { get; set; }
    public ResourceContext ResourceContext { get; set; }

    public ResourceHelper()
    {
        ResourceManager = new ResourceManager();
        ResourceContext = ResourceManager.CreateResourceContext();
    }

    public ResourceHelper(ResourceManager resourceManager) : this()
    {
        ResourceManager = resourceManager ?? new ResourceManager();
        ResourceContext = ResourceManager.CreateResourceContext();
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
        var oldLanguage = Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride;
        if (!string.IsNullOrEmpty(language))
        {
            Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = language;
        }

        var candidate = ResourceManager.MainResourceMap.TryGetValue($"{filename}/{key}", ResourceContext);
        var value = candidate != null ? candidate.ValueAsString : key;

        if (!string.IsNullOrEmpty(language) && !string.IsNullOrEmpty(oldLanguage))
        {
            Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = oldLanguage;
        }

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
