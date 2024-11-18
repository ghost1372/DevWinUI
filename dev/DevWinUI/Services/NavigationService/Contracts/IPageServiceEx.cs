namespace DevWinUI;

public interface IPageServiceEx
{
    Type GetPageType(string key);

    string DefaultPageKey { get; set; }
    string SettingsPageKey { get; set; }

    void SetDefaultPage(Type pageType);

    void SetSettingsPage(Type pageType);
}
