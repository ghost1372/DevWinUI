namespace DevWinUI;
public class BreadcrumbPageConfig
{
    public string PageTitle { get; set; }
    public bool IsHeaderVisible { get; set; }
    public bool ClearNavigation { get; set; }

    public BreadcrumbPageConfig()
    {
        PageTitle = string.Empty;
        IsHeaderVisible = true;
        ClearNavigation = true;
    }
}
