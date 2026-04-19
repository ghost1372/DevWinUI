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
    /// Retrieves a list of resource keys that start with a specified identifier or default to 'Resources'.
    /// </summary>
    /// <param name="identifier">Specifies the prefix to filter resource keys, defaulting to 'Resources' if not provided.</param>
    /// <returns>A list of resource keys formatted as strings.</returns>
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

    /// <summary>
    /// Sets the primary language override for the application.
    /// </summary>
    /// <param name="language">The language code to set as the primary language override. This should be a valid BCP-47 language tag.</param>
    /// <remarks>
    /// This method changes the primary language of the application by setting the 
    /// <see cref="Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride"/> property. 
    /// This affects the language used for resource lookups and other language-dependent operations within the application.
    /// 
    /// Example usage:
    /// <code>
    /// var resourceHelper = new ResourceHelper();
    /// resourceHelper.SetLanguage("en-US");
    /// </code>
    /// 
    /// After calling this method, the application will use the specified language for all subsequent resource lookups.
    /// </remarks>
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

    /// <summary>
    /// Retrieves a resource string by its key.
    /// </summary>
    /// <param name="key">The key of the resource string to retrieve.</param>
    /// <returns>The resource string associated with the specified key.</returns>
    public string GetString(string key)
    {
        return GetStringBase(key, null, "Resources");
    }

    /// <summary>
    /// Retrieves a resource string by its key and language.
    /// </summary>
    /// <param name="key">The key of the resource string to retrieve.</param>
    /// <param name="language">The language code to use for the resource lookup. This should be a valid BCP-47 language tag.</param>
    /// <returns>The resource string associated with the specified key and language.</returns>
    public string GetString(string key, string language)
    {
        return GetStringBase(key, language, "Resources");
    }

    /// <summary>
    /// Retrieves a resource string by its key and filename.
    /// </summary>
    /// <param name="key">The key of the resource string to retrieve.</param>
    /// <param name="filename">The filename where the resource is located.</param>
    /// <returns>The resource string associated with the specified key and filename.</returns>
    public string GetStringFromResource(string key, string filename)
    {
        return GetStringBase(key, null, filename);
    }

    /// <summary>
    /// Retrieves a resource string by its key, language, and filename.
    /// </summary>
    /// <param name="key">The key of the resource string to retrieve.</param>
    /// <param name="language">The language code to use for the resource lookup. This should be a valid BCP-47 language tag.</param>
    /// <param name="filename">The filename where the resource is located.</param>
    /// <returns>The resource string associated with the specified key, language, and filename.</returns>
    public string GetStringFromResource(string key, string language, string filename)
    {
        return GetStringBase(key, language, filename);
    }
}
