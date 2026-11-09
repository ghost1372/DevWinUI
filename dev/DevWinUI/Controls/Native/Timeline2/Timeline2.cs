using Microsoft.UI.Dispatching;

namespace DevWinUI;

public sealed partial class Timeline2 : ItemsControl
{
    internal static readonly double[] IndicatorSizes = { 16, 20, 24, 28, 32, 36, 40, 44, 48 };
    internal const double DotSize = 16;

    private readonly DispatcherQueueHandler _refreshHandler;
    private bool _syncing;
    private bool _refreshQueued;

    public event EventHandler<Timeline2ItemClickEventArgs>? ItemClick;

    public Timeline2()
    {
        DefaultStyleKey = typeof(Timeline2);
        _refreshHandler = OnQueuedRefresh;
    }

    internal bool HasOpposite
    {
        get { return OppositeTemplate is not null || DatePlacement == Timeline2DatePlacement.Opposite; }
    }

    #region Active item

    private object? ValueAt(int index)
    {
        if (index < 0 || index >= Items.Count)
        {
            return null;
        }

        return Items[index] is Timeline2Entry { Value: { } value } ? value : index;
    }

    private int IndexOfValue(object? value)
    {
        if (value is null)
        {
            return -1;
        }

        int count = Items.Count;
        for (int i = 0; i < count; i++)
        {
            if (Items[i] is Timeline2Entry { Value: { } key })
            {
                if (Equals(key, value))
                {
                    return i;
                }
            }
            else if (value is int number && number == i)
            {
                return i;
            }
        }

        return -1;
    }

    private void OnActiveIndexChanged()
    {
        if (!_syncing)
        {
            _syncing = true;
            try
            {
                ActiveValue = ValueAt(ActiveIndex);
            }
            finally
            {
                _syncing = false;
            }
        }

        Refresh();
    }

    private void OnActiveValueChanged()
    {
        if (_syncing)
        {
            return;
        }

        _syncing = true;
        try
        {
            ActiveIndex = IndexOfValue(ActiveValue);
        }
        finally
        {
            _syncing = false;
        }
    }

    private void SyncActiveToItems()
    {
        if (_syncing)
        {
            return;
        }

        _syncing = true;
        try
        {
            if (ActiveValue is not null)
            {
                ActiveIndex = IndexOfValue(ActiveValue);
            }
            else if (ActiveIndex >= 0)
            {
                ActiveValue = ValueAt(ActiveIndex);
            }
        }
        finally
        {
            _syncing = false;
        }
    }

    internal Timeline2ItemState GetState(int index)
    {
        int active = ActiveIndex;
        if (active < 0)
        {
            return Timeline2ItemState.Default;
        }

        if (index == active)
        {
            return Timeline2ItemState.Active;
        }

        bool completed = Reverse ? index > active : index < active;
        return completed ? Timeline2ItemState.Completed : Timeline2ItemState.Pending;
    }

    #endregion

    #region Containers

    protected override bool IsItemItsOwnContainerOverride(object item) => item is Timeline2Item;

    protected override DependencyObject GetContainerForItemOverride() => new Timeline2Item();

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);
        SyncPanel();

        if (element is Timeline2Item timelineItem)
        {
            timelineItem.IsOwnItem = ReferenceEquals(element, item);
            Configure(timelineItem, IndexFromContainer(element), Items.Count);
        }
    }

    protected override void OnItemsChanged(object e)
    {
        base.OnItemsChanged(e);
        SyncActiveToItems();
        QueueRefresh();
    }

    private void Configure(Timeline2Item item, int index, int count)
    {
        if (index < 0)
        {
            return;
        }

        item.Owner = this;
        item.Index = index;
        item.IsLast = index == count - 1;
        item.Update();
    }

    private void Refresh()
    {
        SyncPanel();

        int count = Items.Count;
        for (int i = 0; i < count; i++)
        {
            if (ContainerFromIndex(i) is Timeline2Item item)
            {
                Configure(item, i, count);
            }
        }
    }

    private void SyncPanel()
    {
        if (ItemsPanelRoot is Timeline2Panel panel)
        {
            panel.Orientation = Orientation;
        }
    }

    private void QueueRefresh()
    {
        if (_refreshQueued)
        {
            return;
        }

        _refreshQueued = true;
        if (!DispatcherQueue.TryEnqueue(_refreshHandler))
        {
            _refreshQueued = false;
        }
    }

    private void OnQueuedRefresh()
    {
        _refreshQueued = false;
        Refresh();
    }

    #endregion

    #region Brushes and interaction

    internal Brush? GetColorBrush(Timeline2Color color) => color switch
    {
        Timeline2Color.Primary => PrimaryColorBrush,
        Timeline2Color.Secondary => SecondaryColorBrush,
        Timeline2Color.Success => SuccessColorBrush,
        Timeline2Color.Info => InfoColorBrush,
        Timeline2Color.Warning => WarningColorBrush,
        Timeline2Color.Error => ErrorColorBrush,
        _ => NeutralColorBrush,
    };

    internal Brush? GetOnColorBrush(Timeline2Color color) =>
        color == Timeline2Color.Neutral ? OnNeutralForeground : OnColorForeground;

    internal void NotifyItemClick(Timeline2Item item)
    {
        if (!IsItemClickEnabled)
        {
            return;
        }

        if (ActivateOnClick)
        {
            ActiveIndex = item.Index;
        }

        ItemClick?.Invoke(this, new Timeline2ItemClickEventArgs(item.Content, item.Index));
    }

    #endregion
}
