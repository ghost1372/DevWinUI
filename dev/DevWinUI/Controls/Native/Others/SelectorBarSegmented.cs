namespace DevWinUI;

[TemplatePart(Name = PART_ItemsView, Type = typeof(ItemsView))]
public partial class SelectorBarSegmented : SelectorBar
{
    private const string PART_ItemsView = "PART_ItemsView";
    private ItemsView _ItemsView { get; set; }

    private IEnumerable<object> _backupItems;

    private List<int> _selectedIndex;

    private bool _syncingIndex;

    public IReadOnlyList<object> SelectedItems { get; internal set; }

    public int SelectedIndex
    {
        get { return (int)GetValue(SelectedIndexProperty); }
        set { SetValue(SelectedIndexProperty, value); }
    }

    public static readonly DependencyProperty SelectedIndexProperty =
        DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(SelectorBarSegmented), new PropertyMetadata(-1, OnSelectedIndexChanged));

    private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SelectorBarSegmented)d;
        if (ctl != null && !ctl._syncingIndex)
        {
            ctl.UpdateSelectedIndex((int)e.NewValue);
        }
    }

    private void UpdateSelectedIndex(int value)
    {
        var items = Items;
        if (items == null)
            return;

        SelectorBarItem target = null;
        var i = 0;
        foreach (var item in items)
        {
            if (value < 0 ? item.IsSelected : i == value)
            {
                target = item;
                break;
            }

            i++;
        }

        if (target == null || ReferenceEquals(target, SelectedItem))
            return;

        SelectedItem = target;
        SyncItemsState();
    }

    private void SetIndexFromSelection(int index)
    {
        if (index == SelectedIndex)
            return;

        _syncingIndex = true;
        try
        {
            SelectedIndex = index;
        }
        finally
        {
            _syncingIndex = false;
        }
    }
    public ItemsViewSelectionMode SelectionMode
    {
        get { return (ItemsViewSelectionMode)GetValue(SelectionModeProperty); }
        set { SetValue(SelectionModeProperty, value); }
    }

    public static readonly DependencyProperty SelectionModeProperty =
        DependencyProperty.Register(nameof(SelectionMode), typeof(ItemsViewSelectionMode), typeof(SelectorBarSegmented), new PropertyMetadata(ItemsViewSelectionMode.Single));

    public Orientation Orientation
    {
        get { return (Orientation)GetValue(OrientationProperty); }
        set { SetValue(OrientationProperty, value); }
    }

    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(SelectorBarSegmented), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

    private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SelectorBarSegmented)d;
        if (ctl != null)
        {
            Style style = new Style(typeof(SelectorBarItem));

            switch ((Orientation)e.NewValue)
            {
                case Orientation.Vertical:
                    style = Application.Current.Resources["SelectorBarItemVerticalStyle"] as Style;
                    break;
                case Orientation.Horizontal:
                    style = Application.Current.Resources["SelectorBarItemHorizontalStyle"] as Style;
                    break;
            }

            ctl.Resources.Remove(typeof(SelectorBarItem));
            ctl.Resources.Add(typeof(SelectorBarItem), style);

            ctl.UpdateItemsView((Orientation)e.NewValue);
        }
    }

    public SelectorBarSegmented()
    {
        this.DefaultStyleKey = typeof(SelectorBarSegmented);

        Style style = new Style(typeof(SelectorBarItem));
        style = Application.Current.Resources["SelectorBarItemHorizontalStyle"] as Style;
        Resources.Remove(typeof(SelectorBarItem));
        Resources.Add(typeof(SelectorBarItem), style);

        this.IsEnabledChanged += SelectorBarSegmented_IsEnabledChanged;
        this.Loaded += SelectorBarSegmented_Loaded;

        this.SelectionChanged += SelectorBarSegmented_SelectionChanged;
    }

    private void SelectorBarSegmented_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        if (SelectedItem == null)
        {
            SetIndexFromSelection(-1);
            return;
        }

        var index = Items.IndexOf(SelectedItem);
        if (index >= 0)
        {
            SetIndexFromSelection(index);
        }
    }
    private void SelectorBarSegmented_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        SyncItemsState();
    }

    private void SelectorBarSegmented_Loaded(object sender, RoutedEventArgs e)
    {
        SyncItemsState();
    }

    public void SyncItemsState()
    {
        var items = Items;
        if (items == null) return;
        bool enabled = IsEnabled;
        int selectedIdx = SelectedIndex;
        if (selectedIdx < 0 && SelectedItem != null)
        {
            selectedIdx = items.IndexOf(SelectedItem);
        }

        var i = 0;
        foreach (var item in items)
        {
            item.IsSelected = (i == selectedIdx);
            item.IsEnabled = enabled;
            VisualStateManager.GoToState(item, enabled ? "Enabled" : "Disabled", false);
            i++;
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _ItemsView = GetTemplateChild(PART_ItemsView) as ItemsView;

        if (_ItemsView != null)
        {
            _ItemsView.SelectionChanged += _ItemsView_SelectionChanged;
        }

        UpdateItemsView(Orientation);
        UpdateSelectedIndex(SelectedIndex);
    }

    private void _ItemsView_SelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        SelectedItems = _ItemsView.SelectedItems;
    }

    private void UpdateItemsView(Orientation orientation)
    {
        if (_ItemsView != null)
        {
            _selectedIndex = new List<int>();
            _backupItems = _ItemsView.ItemsSource as IEnumerable<object>;

            var itemsRepeater = _ItemsView.FindChild<ItemsRepeater>();
            if (itemsRepeater != null)
            {
                foreach (SelectorBarItem item in _ItemsView.SelectedItems)
                {
                    var index = itemsRepeater.GetElementIndex(item);
                    _selectedIndex.Add(index);
                }
            }

            _ItemsView.ItemsSource = null;
            _ItemsView.Layout = new StackLayout { Orientation = orientation };

            _ItemsView.ItemsSource = _backupItems;

            foreach (var item in _selectedIndex)
            {
                _ItemsView.Select(item);
            }

            _ItemsView.UpdateLayout();
        }
    }
}
