namespace DevWinUI;
public partial class NavigationParameterExtension : MarkupExtension
{
    public NavigationTransitionInfo NavigationTransitionInfo { get; set; } = new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromRight };
    public Type PageType { get; set; }
    public string BreadCrumbHeader { get; set; }

    public NavigationParameterExtension() { }

    public NavigationParameterExtension(Type pageType)
    {
        PageType = pageType;
    }
    public NavigationParameterExtension(Type pageType, NavigationTransitionInfo navigationTransitionInfo)
    {
        PageType = pageType;
        NavigationTransitionInfo = navigationTransitionInfo;
    }
    public NavigationParameterExtension(Type pageType, string header)
    {
        PageType = pageType;
        BreadCrumbHeader = header;
    }
    public NavigationParameterExtension(Type pageType, NavigationTransitionInfo navigationTransitionInfo, string header)
    {
        PageType = pageType;
        BreadCrumbHeader = header;
        NavigationTransitionInfo = navigationTransitionInfo;
    }

    protected override object ProvideValue()
    {
        return new NavigationParameterExtension(PageType, NavigationTransitionInfo, BreadCrumbHeader);
    }
}
