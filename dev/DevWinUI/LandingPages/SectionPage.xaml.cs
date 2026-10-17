namespace DevWinUI;

public sealed partial class SectionPage : ItemsPageBase
{
    public SectionPage()
    {
        this.InitializeComponent();
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

    /// <summary>
    /// Loads and orders the items in one step, so Items is only set once.
    /// </summary>
    public void GetData(string uniqueId, string sectionId, Func<DataItem, object> orderBy, bool descending = false)
    {
        var group = FindUniqueGroup(uniqueId, sectionId);
        if (group != null)
        {
            GetItems(group, orderBy, descending);
        }
    }

    public void GetData(string uniqueId, string sectionId)
    {
        var group = FindUniqueGroup(uniqueId, sectionId);
        if (group != null)
        {
            GetItems(group);
        }
    }

    public async Task GetDataAsync(string uniqueId, string sectionId, string jsonFilePath, PathType pathType = PathType.Relative)
    {
        await DataSource.Instance.GetGroupsAsync(jsonFilePath, pathType);

        var group = FindUniqueGroup(uniqueId, sectionId);
        if (group != null)
        {
            GetItems(group);
        }
    }

    private static DataGroup FindUniqueGroup(string uniqueId, string sectionId)
    {
        DataGroup match = null;
        foreach (var group in DataSource.Instance.Groups)
        {
            if (string.IsNullOrEmpty(group.UniqueId) || string.IsNullOrEmpty(group.SectionId) ||
                !string.Equals(group.UniqueId, uniqueId, StringComparison.Ordinal) ||
                !string.Equals(group.SectionId, sectionId, StringComparison.Ordinal))
            {
                continue;
            }

            if (match != null)
            {
                return null;
            }

            match = group;
        }

        return match;
    }

    private void GetItems(DataGroup group, Func<DataItem, object> orderBy = null, bool descending = false)
    {
        if (group != null)
        {
            TitleTxt.Text = group?.Title;

            var items = group.Items?.Where(i => !i.HideItem);

            if (items != null && orderBy != null)
            {
                items = descending ? items.OrderByDescending(orderBy) : items.OrderBy(orderBy);
            }

            Items = items?.ToList();
        }
    }

    protected override bool GetIsNarrowLayoutState()
    {
        return LayoutVisualStates.CurrentState == NarrowLayout;
    }
}
