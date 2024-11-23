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
    public IJsonNavigationService JsonNavigationService
    {
        get { return (IJsonNavigationService)GetValue(JsonNavigationServiceProperty); }
        set { SetValue(JsonNavigationServiceProperty, value); }
    }

    public ImageSource PlaceholderSource
    {
        get => (ImageSource)GetValue(PlaceholderSourceProperty);
        set => SetValue(PlaceholderSourceProperty, value);
    }
    public bool IsCacheEnabled
    {
        get => (bool)GetValue(IsCacheEnabledProperty);
        set => SetValue(IsCacheEnabledProperty, value);
    }
    public bool EnableLazyLoading
    {
        get => (bool)GetValue(EnableLazyLoadingProperty);
        set => SetValue(EnableLazyLoadingProperty, value);
    }
    public double LazyLoadingThreshold
    {
        get => (double)GetValue(LazyLoadingThresholdProperty);
        set => SetValue(LazyLoadingThresholdProperty, value);
    }

    public string HeaderImage
    {
        get => (string)GetValue(HeaderImageProperty);
        set => SetValue(HeaderImageProperty, value);
    }
    public string HeaderOverlayImage
    {
        get => (string)GetValue(HeaderOverlayImageProperty);
        set => SetValue(HeaderOverlayImageProperty, value);
    }

    public GridLength SpaceBetweenHeaderAndGridView
    {
        get => (GridLength)GetValue(SpaceBetweenHeaderAndGridViewProperty);
        set => SetValue(SpaceBetweenHeaderAndGridViewProperty, value);
    }

    public Stretch Stretch
    {
        get { return (Stretch)GetValue(StretchProperty); }
        set { SetValue(StretchProperty, value); }
    }
    public string NormalizedCenterPoint
    {
        get { return (string)GetValue(NormalizedCenterPointProperty); }
        set { SetValue(NormalizedCenterPointProperty, value); }
    }
    public double OverlayOpacity
    {
        get { return (double)GetValue(OverlayOpacityProperty); }
        set { SetValue(OverlayOpacityProperty, value); }
    }

    public VerticalAlignment HeaderVerticalAlignment
    {
        get { return (VerticalAlignment)GetValue(HeaderVerticalAlignmentProperty); }
        set { SetValue(HeaderVerticalAlignmentProperty, value); }
    }
    public CornerRadius HeaderCornerRadius
    {
        get => (CornerRadius)GetValue(HeaderCornerRadiusProperty);
        set => SetValue(HeaderCornerRadiusProperty, value);
    }
    public Thickness HeaderContentMargin
    {
        get { return (Thickness)GetValue(HeaderContentMarginProperty); }
        set { SetValue(HeaderContentMarginProperty, value); }
    }

    public double HeaderSubtitleFontSize
    {
        get => (double)GetValue(HeaderSubtitleFontSizeProperty);
        set => SetValue(HeaderSubtitleFontSizeProperty, value);
    }

    public string HeaderSubtitleText
    {
        get => (string)GetValue(HeaderSubtitleTextProperty);
        set => SetValue(HeaderSubtitleTextProperty, value);
    }

    public Thickness GridViewPadding
    {
        get { return (Thickness)GetValue(GridViewPaddingProperty); }
        set { SetValue(GridViewPaddingProperty, value); }
    }
    public VerticalAlignment GridViewVerticalAlignment
    {
        get { return (VerticalAlignment)GetValue(GridViewVerticalAlignmentProperty); }
        set { SetValue(GridViewVerticalAlignmentProperty, value); }
    }
    public double HeaderFontSize
    {
        get => (double)GetValue(HeaderFontSizeProperty);
        set => SetValue(HeaderFontSizeProperty, value);
    }

    public string HeaderText
    {
        get => (string)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    

    public bool IsTileImage
    {
        get { return (bool)GetValue(IsTileImageProperty); }
        set { SetValue(IsTileImageProperty, value); }
    }

    public static readonly DependencyProperty IsTileImageProperty = DependencyProperty.Register(nameof(IsTileImage), typeof(bool), typeof(ItemsPageBase), new PropertyMetadata(false, OnIsTileImageChanged));
    public static readonly DependencyProperty HeaderFontSizeProperty = DependencyProperty.Register(nameof(HeaderFontSize), typeof(double), typeof(ItemsPageBase), new PropertyMetadata(28.0));
    public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register(nameof(HeaderText), typeof(string), typeof(ItemsPageBase), new PropertyMetadata("All"));
    public static readonly DependencyProperty GridViewVerticalAlignmentProperty = DependencyProperty.Register(nameof(GridViewVerticalAlignment), typeof(VerticalAlignment), typeof(ItemsPageBase), new PropertyMetadata(VerticalAlignment.Bottom));
    public static readonly DependencyProperty GridViewPaddingProperty = DependencyProperty.Register(nameof(GridViewPadding), typeof(Thickness), typeof(ItemsPageBase), new PropertyMetadata(new Thickness(24, 0, 24, 72)));
    public static readonly DependencyProperty HeaderSubtitleTextProperty = DependencyProperty.Register(nameof(HeaderSubtitleText), typeof(string), typeof(ItemsPageBase), new PropertyMetadata(default(string)));
    public static readonly DependencyProperty HeaderSubtitleFontSizeProperty = DependencyProperty.Register(nameof(HeaderSubtitleFontSize), typeof(double), typeof(ItemsPageBase), new PropertyMetadata(18.0));
    public static readonly DependencyProperty HeaderContentMarginProperty = DependencyProperty.Register(nameof(HeaderContentMargin), typeof(Thickness), typeof(ItemsPageBase), new PropertyMetadata(new Thickness(36, 100, 0, 0)));
    public static readonly DependencyProperty HeaderCornerRadiusProperty = DependencyProperty.Register(nameof(HeaderCornerRadius), typeof(CornerRadius), typeof(ItemsPageBase), new PropertyMetadata(new CornerRadius(8, 0, 0, 0)));
    public static readonly DependencyProperty HeaderVerticalAlignmentProperty = DependencyProperty.Register(nameof(HeaderVerticalAlignment), typeof(VerticalAlignment), typeof(ItemsPageBase), new PropertyMetadata(VerticalAlignment.Center));
    public static readonly DependencyProperty OverlayOpacityProperty = DependencyProperty.Register(nameof(OverlayOpacity), typeof(double), typeof(ItemsPageBase), new PropertyMetadata(0.5));
    public static readonly DependencyProperty NormalizedCenterPointProperty = DependencyProperty.Register(nameof(NormalizedCenterPoint), typeof(string), typeof(ItemsPageBase), new PropertyMetadata("0.5"));
    public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(ItemsPageBase), new PropertyMetadata(Stretch.UniformToFill));
    public static readonly DependencyProperty SpaceBetweenHeaderAndGridViewProperty = DependencyProperty.Register(nameof(SpaceBetweenHeaderAndGridView), typeof(GridLength), typeof(ItemsPageBase), new PropertyMetadata(new GridLength(50.0)));
    public static readonly DependencyProperty LazyLoadingThresholdProperty = DependencyProperty.Register(nameof(LazyLoadingThreshold), typeof(double), typeof(ItemsPageBase), new PropertyMetadata(300.0));
    public static readonly DependencyProperty EnableLazyLoadingProperty = DependencyProperty.Register(nameof(EnableLazyLoading), typeof(bool), typeof(ItemsPageBase), new PropertyMetadata(true));
    public static readonly DependencyProperty PlaceholderSourceProperty = DependencyProperty.Register(nameof(PlaceholderSource), typeof(ImageSource), typeof(ItemsPageBase), new PropertyMetadata(default(ImageSource)));
    public static readonly DependencyProperty IsCacheEnabledProperty = DependencyProperty.Register(nameof(IsCacheEnabled), typeof(bool), typeof(ItemsPageBase), new PropertyMetadata(true));
    public static readonly DependencyProperty HeaderImageProperty = DependencyProperty.Register(nameof(HeaderImage), typeof(string), typeof(ItemsPageBase), new PropertyMetadata(default(string)));
    public static readonly DependencyProperty HeaderOverlayImageProperty = DependencyProperty.Register(nameof(HeaderOverlayImage), typeof(string), typeof(ItemsPageBase), new PropertyMetadata(default(string)));
    public static readonly DependencyProperty JsonNavigationServiceProperty = DependencyProperty.Register(nameof(JsonNavigationService), typeof(IJsonNavigationService), typeof(ItemsPageBase), new PropertyMetadata(null));
    
    private static void OnIsTileImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ItemsPageBase)d;
        if (ctl != null)
        {
            ctl.OnIsTileImageChanged(e);
        }
    }
    protected virtual void OnIsTileImageChanged(DependencyPropertyChangedEventArgs e)
    {
    }
}
