using Microsoft.Windows.ApplicationModel.Resources;

namespace DevWinUI;
public sealed partial class AllLandingPage : ItemsPageBase
{
    internal static AllLandingPage Instance { get; private set; }

    public static readonly DependencyProperty UseFullScreenHeaderImageProperty =
        DependencyProperty.Register(nameof(UseFullScreenHeaderImage), typeof(bool), typeof(AllLandingPage), new PropertyMetadata(false, OnFullScreenHeaderImageChanged));
    
    public bool UseFullScreenHeaderImage
    {
        get { return (bool)GetValue(UseFullScreenHeaderImageProperty); }
        set { SetValue(UseFullScreenHeaderImageProperty, value); }
    }
    private static void OnFullScreenHeaderImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AllLandingPage)d;
        if (ctl != null)
        {
            ctl.ToggleFullScreen((bool)e.NewValue);
        }
    }

    private void ToggleFullScreen(bool value)
    {
        
    }
    public AllLandingPage()
    {
        this.InitializeComponent();
        Instance = this;
        Loading -= AllLandingPage_Loading;
        Loading += AllLandingPage_Loading;
        ToggleFullScreen(UseFullScreenHeaderImage);
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
        GetData(new ResourceManager());
    }
    public void GetData(ResourceManager resourceManager)
    {
        var allItems = DataSource.Instance.Groups
            .Where(group => !group.HideGroup && !group.IsSpecialSection)
            .SelectMany(group => group.Items)
            .Where(item => !item.HideItem)
            .SelectMany(item => GetLocalizedItemsRecursively(item, resourceManager))
            .ToList();

        Items = allItems;
    }
    private IEnumerable<DataItem> GetLocalizedItemsRecursively(DataItem currentItem, ResourceManager resourceManager)
    {
        LocalizeItem(currentItem, resourceManager);
        yield return currentItem;
    }
    private void LocalizeItem(DataItem item, ResourceManager resourceManager)
    {
        item.Title = Helper.GetLocalizedText(item.Title, item.UsexUid, resourceManager);
        item.SecondaryTitle = Helper.GetLocalizedText(item.SecondaryTitle, item.UsexUid, resourceManager);
        item.Subtitle = Helper.GetLocalizedText(item.Subtitle, item.UsexUid, resourceManager);
        item.Description = Helper.GetLocalizedText(item.Description, item.UsexUid, resourceManager);
    }
    public async Task GetDataAsync(string jsonFilePath, PathType pathType = PathType.Relative)
    {
        await GetDataAsync(jsonFilePath, new ResourceManager(), pathType);
    }
    public async Task GetDataAsync(string jsonFilePath, ResourceManager resourceManager, PathType pathType = PathType.Relative)
    {
        await DataSource.Instance.GetGroupsAsync(jsonFilePath, pathType);

        var allItems = DataSource.Instance.Groups
            .Where(group => !group.HideGroup && !group.IsSpecialSection)
            .SelectMany(group => group.Items)
            .Where(item => !item.HideItem)
            .SelectMany(item => GetLocalizedItemsRecursively(item, resourceManager))
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
