using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DevWinUI;
public abstract partial class ItemsPageBase : Page, INotifyPropertyChanged
{
    public bool CanExecuteInternalCommand { get; set; } = true;

    public event EventHandler<RoutedEventArgs> OnItemClick;

    public event PropertyChangedEventHandler PropertyChanged;

    private string _itemId;
    private IEnumerable<DataItem> _items;

    public IEnumerable<DataItem> Items
    {
        get => _items;
        set => SetProperty(ref _items, value);
    }

    protected ItemsPageBase()
    {
        Resources.MergedDictionaries.AddIfNotExists(new GridViewItemTemplate());
    }

    /// <summary>
    /// Gets a value indicating whether the application's view is currently in "narrow" mode - i.e. on a mobile-ish device.
    /// </summary>
    protected virtual bool GetIsNarrowLayoutState()
    {
        throw new NotImplementedException();
    }

    protected void OnItemGridViewContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
    {
        if (sender.ContainerFromItem(sender.Items.LastOrDefault()) is GridViewItem container)
        {
            container.XYFocusDown = container;
        }

        var item = args.Item as DataItem;
        if (item != null)
        {
            args.ItemContainer.IsEnabled = item.IncludedInBuild;
            args.ItemContainer.Visibility = item.HideItem ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    protected void OnItemGridViewItemClick(object sender, ItemClickEventArgs e)
    {
        var gridView = (GridView)sender;
        var item = (DataItem)e.ClickedItem;

        _itemId = item.UniqueId;

        if (OnItemClick == null)
        {
            if (AllLandingPage.Instance != null)
            {
                AllLandingPage.Instance.Navigate(sender, e);
            }

            if (MainLandingPage.Instance != null)
            {
                MainLandingPage.Instance.Navigate(sender, e);
            }
        }

        OnItemClick?.Invoke(gridView, e);
    }

    protected void OnItemGridViewLoaded(object sender, RoutedEventArgs e)
    {
        if (_itemId != null)
        {
            var gridView = (GridView)sender;
            var items = gridView.ItemsSource as IEnumerable<DataItem>;
            var item = items?.FirstOrDefault(s => s.UniqueId == _itemId);
            if (item != null)
            {
                gridView.ScrollIntoView(item);
                ((GridViewItem)gridView.ContainerFromItem(item))?.Focus(FocusState.Programmatic);
            }
        }
    }

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(storage, value)) return false;

        storage = value;
        NotifyPropertyChanged(propertyName);
        return true;
    }

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    internal void Navigate(object sender, RoutedEventArgs e)
    {
        if (JsonNavigationService != null)
        {
            var args = (ItemClickEventArgs)e;
            var item = (DataItem)args.ClickedItem;

            JsonNavigationService.NavigateTo(item.UniqueId, item);
        }
    }
    
    protected virtual void OnIsTileImageChanged(DependencyPropertyChangedEventArgs e)
    {
    }
}
