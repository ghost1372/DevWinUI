namespace DevWinUI;
public sealed partial class AllLandingPage : ItemsPageBase
{
    internal static AllLandingPage Instance { get; private set; }

    public AllLandingPage()
    {
        this.InitializeComponent();
        Instance = this;
        Loading -= AllLandingPage_Loading;
        Loading += AllLandingPage_Loading;
    }

    private void AllLandingPage_Loading(FrameworkElement sender, object args)
    {
        if (CanExecuteInternalCommand)
        {
            GetData();
            OrderBy(i => i.Title);
        }
    }

    public void GetData()
    {
        var allItems = DataSource.Instance.Groups
            .Where(group => !group.HideGroup && !group.IsSpecialSection)
            .SelectMany(group => group.Items)
            .Where(item => !item.HideItem)
            .ToList();

        Items = allItems;
    }

    public async Task GetDataAsync(string jsonFilePath, PathType pathType = PathType.Relative)
    {
        await DataSource.Instance.GetGroupsAsync(jsonFilePath, pathType);

        var allItems = DataSource.Instance.Groups
            .Where(group => !group.HideGroup && !group.IsSpecialSection)
            .SelectMany(group => group.Items)
            .Where(item => !item.HideItem)
            .ToList();

        Items = allItems;
    }
 
    public void OrderBy(Func<DataItem, object> orderby = null)
    {
        if (orderby != null)
        {
            Items = Items?.OrderBy(orderby)?.ToList();
        }
        else
        {
            Items = Items?.OrderBy(i => i.Title)?.ToList();
        }
    }

    public void OrderByDescending(Func<DataItem, object> orderByDescending = null)
    {
        if (orderByDescending != null)
        {
            Items = Items?.OrderByDescending(orderByDescending)?.ToList();
        }
        else
        {
            Items = Items?.OrderByDescending(i => i.Title)?.ToList();
        }
    }
}
