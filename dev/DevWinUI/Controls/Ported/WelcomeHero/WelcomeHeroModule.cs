namespace DevWinUI;

/// <summary>
/// A module shown in the Welcome hero.
/// </summary>
public sealed partial class WelcomeHeroModule
{
    public WelcomeHeroModule(string asset, string navigationTag, string title)
    {
        Asset = asset;
        NavigationTag = navigationTag;
        Title = title;
    }

    /// <summary>
    /// Gets or sets the full address (e.g. a <c>ms-appx:///...</c> URI) of the icon image.
    /// </summary>
    public string Asset { get; set; }

    /// <summary>
    /// Gets or sets the tag of the matching OOBE navigation item (a <c>PowerToysModules</c> name).
    /// </summary>
    public string NavigationTag { get; set; }

    /// <summary>
    /// Gets or sets the localized module name shown in the hover label.
    /// </summary>
    public string Title { get; set; }
}
