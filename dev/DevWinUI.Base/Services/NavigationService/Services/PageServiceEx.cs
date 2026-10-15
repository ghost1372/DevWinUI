namespace DevWinUI;

public partial class PageServiceEx : IPageServiceEx
{
    public Dictionary<string, Type> _pageKeyToTypeMap;

    public string DefaultPageKey { get; set; } = nameof(DefaultPageKey);
    public string SettingsPageKey { get; set; } = nameof(SettingsPageKey);
    internal string SectionPageKey { get; set; } = nameof(SectionPageKey);

    public virtual Type GetPageType(string key)
    {
        var pageMap = _pageKeyToTypeMap;
        if (key == null || pageMap == null)
        {
            return null;
        }

        lock (pageMap)
        {
            return pageMap.TryGetValue(key, out var pageType) ? pageType : null;
        }
    }

    public virtual void SetDefaultPage(Type pageType)
    {
        var pageMap = _pageKeyToTypeMap;
        if (pageType != null && pageMap != null)
        {
            lock (pageMap)
            {
                pageMap.TryAdd(DefaultPageKey, pageType);
            }
        }
    }

    public virtual void SetSettingsPage(Type pageType)
    {
        var pageMap = _pageKeyToTypeMap;
        if (pageType != null && pageMap != null)
        {
            lock (pageMap)
            {
                pageMap.TryAdd(SettingsPageKey, pageType);
            }
        }
    }

    internal virtual void SetSectionPage(Type pageType)
    {
        var pageMap = _pageKeyToTypeMap;
        if (pageType != null && pageMap != null)
        {
            lock (pageMap)
            {
                pageMap.TryAdd(SectionPageKey, pageType);
            }
        }
    }
}
