using System.Collections.Specialized;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using DispatcherQueuePriority = Microsoft.UI.Dispatching.DispatcherQueuePriority;

namespace DevWinUI;

[TemplatePart(Name = TriggerButtonPartName, Type = typeof(Button))]
[TemplatePart(Name = SearchTextBoxPartName, Type = typeof(Microsoft.UI.Xaml.Controls.TextBox))]
[TemplatePart(Name = ResultsListPartName, Type = typeof(ListView))]
[TemplatePart(Name = OpenButtonPartName, Type = typeof(Button))]
[TemplatePart(Name = NoResultsPresenterPartName, Type = typeof(FrameworkElement))]
[TemplatePart(Name = FlyoutRootPartName, Type = typeof(FrameworkElement))]
public sealed partial class SearchBox : Control
{
    private const string TriggerButtonPartName = "PART_TriggerButton";
    private const string SearchTextBoxPartName = "PART_SearchTextBox";
    private const string ResultsListPartName = "PART_ResultsList";
    private const string OpenButtonPartName = "PART_OpenButton";
    private const string NoResultsPresenterPartName = "PART_NoResultsPresenter";
    private const string FlyoutRootPartName = "PART_FlyoutRoot";

    private static readonly SearchBoxTemplates DefaultItemTemplateResources = new();

    private static readonly DataTemplate DefaultItemTemplate =
        (DataTemplate)DefaultItemTemplateResources["SearchResultTemplate"];

    private Button? _triggerButton;
    private FlyoutBase? _flyout;
    private FrameworkElement? _flyoutRoot;
    private Microsoft.UI.Xaml.Controls.TextBox? _searchTextBox;
    private ListView? _resultsList;
    private Button? _openButton;
    private FrameworkElement? _noResultsPresenter;
    private bool _suppressQueryChanged;
    private bool _updatingQueryText;
    private bool _layoutRefreshQueued;

    public SearchBox()
    {
        DefaultStyleKey = typeof(SearchBox);
        Results.CollectionChanged += Results_CollectionChanged;
    }

    public string PlaceholderText
    {
        get { return (string)GetValue(PlaceholderTextProperty); }
        set { SetValue(PlaceholderTextProperty, value); }
    }

    public static readonly DependencyProperty PlaceholderTextProperty =
        DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(SearchBox), new PropertyMetadata("Search", OnFlyoutContentOptionChanged));

    public string QueryText
    {
        get { return (string)GetValue(QueryTextProperty); }
        set { SetValue(QueryTextProperty, value); }
    }

    public static readonly DependencyProperty QueryTextProperty =
        DependencyProperty.Register(nameof(QueryText), typeof(string), typeof(SearchBox), new PropertyMetadata(string.Empty, OnQueryTextChanged));

    public bool ShowPreviousSearch
    {
        get { return (bool)GetValue(ShowPreviousSearchProperty); }
        set { SetValue(ShowPreviousSearchProperty, value); }
    }

    public static readonly DependencyProperty ShowPreviousSearchProperty =
        DependencyProperty.Register(nameof(ShowPreviousSearch), typeof(bool), typeof(SearchBox), new PropertyMetadata(false));

    public double FlyoutWidth
    {
        get { return (double)GetValue(FlyoutWidthProperty); }
        set { SetValue(FlyoutWidthProperty, value); }
    }

    public static readonly DependencyProperty FlyoutWidthProperty =
        DependencyProperty.Register(nameof(FlyoutWidth), typeof(double), typeof(SearchBox), new PropertyMetadata(640d, OnFlyoutContentOptionChanged));

    public double ResultsMaxHeight
    {
        get { return (double)GetValue(ResultsMaxHeightProperty); }
        set { SetValue(ResultsMaxHeightProperty, value); }
    }

    public static readonly DependencyProperty ResultsMaxHeightProperty =
        DependencyProperty.Register(nameof(ResultsMaxHeight), typeof(double), typeof(SearchBox), new PropertyMetadata(560d, OnFlyoutContentOptionChanged));

    public bool ConstrainToAppBounds
    {
        get { return (bool)GetValue(ConstrainToAppBoundsProperty); }
        set { SetValue(ConstrainToAppBoundsProperty, value); }
    }

    public static readonly DependencyProperty ConstrainToAppBoundsProperty =
        DependencyProperty.Register(nameof(ConstrainToAppBounds), typeof(bool), typeof(SearchBox), new PropertyMetadata(false, OnFlyoutOptionChanged));

    public FlyoutPlacementMode FlyoutPlacement
    {
        get { return (FlyoutPlacementMode)GetValue(FlyoutPlacementProperty); }
        set { SetValue(FlyoutPlacementProperty, value); }
    }

    public static readonly DependencyProperty FlyoutPlacementProperty =
        DependencyProperty.Register(nameof(FlyoutPlacement), typeof(FlyoutPlacementMode), typeof(SearchBox), new PropertyMetadata(FlyoutPlacementMode.Bottom, OnFlyoutOptionChanged));

    public DataTemplate ItemTemplate
    {
        get { return (DataTemplate)GetValue(ItemTemplateProperty); }
        set { SetValue(ItemTemplateProperty, value); }
    }

    public static readonly DependencyProperty ItemTemplateProperty =
        DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(SearchBox), new PropertyMetadata(DefaultItemTemplate));

    public string NoResultsText
    {
        get { return (string)GetValue(NoResultsTextProperty); }
        set { SetValue(NoResultsTextProperty, value); }
    }

    public static readonly DependencyProperty NoResultsTextProperty =
        DependencyProperty.Register(nameof(NoResultsText), typeof(string), typeof(SearchBox), new PropertyMetadata("No results found"));

    public string NoResultsDescription
    {
        get { return (string)GetValue(NoResultsDescriptionProperty); }
        set { SetValue(NoResultsDescriptionProperty, value); }
    }

    public static readonly DependencyProperty NoResultsDescriptionProperty =
        DependencyProperty.Register(nameof(NoResultsDescription), typeof(string), typeof(SearchBox), new PropertyMetadata("Try a different search term"));

    public ObservableCollection<SearchResultItem> Results { get; } = new();

    public ObservableCollection<SearchMenuItem> DefaultMenuItems { get; } = new();

    public event EventHandler<SearchEventArgs>? QueryChanged;
    public event EventHandler<SearchResultEventArgs>? ResultInvoked;
    public event EventHandler<SearchMenuItemEventArgs>? MenuItemInvoked;

    public static Visibility ToVisibility(bool value) => value ? Visibility.Visible : Visibility.Collapsed;

    public static Visibility NullToCollapsed(ImageSource? value) =>
        value == null ? Visibility.Collapsed : Visibility.Visible;

    public void Show()
    {
        if (_flyout is not null && _triggerButton is not null)
        {
            _flyout.ShowAt(_triggerButton);
        }
    }

    public void Hide() => _flyout?.Hide();

    protected override void OnApplyTemplate()
    {
        DetachParts();
        base.OnApplyTemplate();

        _triggerButton = GetTemplateChild(TriggerButtonPartName) as Button;
        _flyout = _triggerButton?.Flyout;

        if (_flyout is null)
        {
            return;
        }

        ApplyFlyoutOptions();
        AttachFlyoutEvents();
    }

    private void AttachFlyoutEvents()
    {
        if (_flyout is not null)
        {
            _flyout.Opening += Flyout_Opening;
            _flyout.Opened += Flyout_Opened;
            _flyout.Closed += Flyout_Closed;
        }
    }

    private void AttachPartHandlers()
    {
        if (_searchTextBox is not null)
        {
            _searchTextBox.TextChanged -= SearchTextBox_TextChanged;
            _searchTextBox.KeyDown -= SearchTextBox_KeyDown;
            _searchTextBox.TextChanged += SearchTextBox_TextChanged;
            _searchTextBox.KeyDown += SearchTextBox_KeyDown;
        }

        if (_resultsList is not null)
        {
            _resultsList.SelectionChanged -= ResultsList_SelectionChanged;
            _resultsList.DoubleTapped -= ResultsList_DoubleTapped;
            _resultsList.SizeChanged -= ResultsList_SizeChanged;
            _resultsList.SelectionChanged += ResultsList_SelectionChanged;
            _resultsList.DoubleTapped += ResultsList_DoubleTapped;
            _resultsList.SizeChanged += ResultsList_SizeChanged;
        }

        if (_openButton is not null)
        {
            _openButton.Click -= OpenButton_Click;
            _openButton.Click += OpenButton_Click;
        }
    }

    private IEnumerable<DependencyObject> EnumerateFlyoutRoots()
    {
        if (XamlRoot is not null)
        {
            foreach (var popup in VisualTreeHelper.GetOpenPopupsForXamlRoot(XamlRoot))
            {
                if (popup.Child is not null)
                {
                    yield return popup.Child;
                }
            }
        }

        if ((_flyout as Flyout)?.Content is DependencyObject content)
        {
            yield return content;
        }
    }

    private void ResolveOpenedParts()
    {
        foreach (var root in EnumerateFlyoutRoots())
        {
            if (_searchTextBox is not null)
            {
                break;
            }

            Microsoft.UI.Xaml.Controls.TextBox? textBox = null;
            ListView? list = null;
            Button? openButton = null;
            FrameworkElement? noResults = null;
            FrameworkElement? flyoutRoot = null;

            foreach (var node in Descendants(root))
            {
                if (node is not FrameworkElement element)
                {
                    continue;
                }

                switch (element.Name)
                {
                    case SearchTextBoxPartName:
                        textBox = element as Microsoft.UI.Xaml.Controls.TextBox;
                        break;
                    case ResultsListPartName:
                        list = element as ListView;
                        break;
                    case OpenButtonPartName:
                        openButton = element as Button;
                        break;
                    case NoResultsPresenterPartName:
                        noResults = element;
                        break;
                    case FlyoutRootPartName:
                        flyoutRoot = element;
                        break;
                }
            }

            if (textBox is null)
            {
                continue;
            }

            _searchTextBox = textBox;
            _resultsList = list;
            _openButton = openButton;
            _noResultsPresenter = noResults;
            _flyoutRoot = flyoutRoot;
            break;
        }

        if (_searchTextBox is null)
        {
            return;
        }

        if (_resultsList is not null)
        {
            _resultsList.ItemsSource = Results;
            _resultsList.Tag = this;
        }

        _searchTextBox.Text = QueryText;

        ApplyFlyoutContentOptions();
        UpdateNoResultsState();
        AttachPartHandlers();
    }

    private static IEnumerable<DependencyObject> Descendants(DependencyObject root)
    {
        yield return root;

        foreach (var child in GetChildren(root))
        {
            foreach (var descendant in Descendants(child))
            {
                yield return descendant;
            }
        }
    }

    private static IEnumerable<DependencyObject> GetChildren(DependencyObject parent)
    {
        var yielded = false;

        if (parent is Panel panel)
        {
            foreach (var child in panel.Children)
            {
                yielded = true;
                yield return child;
            }
        }
        else if (parent is Border { Child: { } borderChild })
        {
            yielded = true;
            yield return borderChild;
        }
        else if (parent is ContentControl { Content: DependencyObject content })
        {
            yielded = true;
            yield return content;
        }

        if (yielded)
        {
            yield break;
        }

        var count = VisualTreeHelper.GetChildrenCount(parent);
        for (var i = 0; i < count; i++)
        {
            yield return VisualTreeHelper.GetChild(parent, i);
        }
    }

    private void DetachParts()
    {
        if (_flyout is not null)
        {
            _flyout.Opening -= Flyout_Opening;
            _flyout.Opened -= Flyout_Opened;
            _flyout.Closed -= Flyout_Closed;
        }

        if (_searchTextBox is not null)
        {
            _searchTextBox.TextChanged -= SearchTextBox_TextChanged;
            _searchTextBox.KeyDown -= SearchTextBox_KeyDown;
        }

        if (_resultsList is not null)
        {
            _resultsList.SelectionChanged -= ResultsList_SelectionChanged;
            _resultsList.DoubleTapped -= ResultsList_DoubleTapped;
            _resultsList.SizeChanged -= ResultsList_SizeChanged;
            _resultsList.ItemsSource = null;
            _resultsList.Tag = null;
        }

        if (_openButton is not null)
        {
            _openButton.Click -= OpenButton_Click;
        }

        _triggerButton = null;
        _flyout = null;
        _flyoutRoot = null;
        _searchTextBox = null;
        _resultsList = null;
        _openButton = null;
        _noResultsPresenter = null;
    }

    private static void OnQueryTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (SearchBox)d;
        control.UpdateNoResultsState();

        if (control._updatingQueryText || control._searchTextBox is null)
        {
            return;
        }

        var text = (string?)e.NewValue ?? string.Empty;
        if (control._searchTextBox.Text != text)
        {
            control._searchTextBox.Text = text;
        }
    }

    private static void OnFlyoutOptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((SearchBox)d).ApplyFlyoutOptions();
    }

    private static void OnFlyoutContentOptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((SearchBox)d).ApplyFlyoutContentOptions();
    }

    private void ApplyFlyoutOptions()
    {
        if (_flyout is null)
        {
            return;
        }

        _flyout.ShouldConstrainToRootBounds = ConstrainToAppBounds;
        _flyout.Placement = FlyoutPlacement == FlyoutPlacementMode.Auto
            ? FlyoutPlacementMode.Bottom
            : FlyoutPlacement;
    }

    private void ApplyFlyoutContentOptions()
    {
        if (_flyoutRoot is not null)
        {
            _flyoutRoot.Width = FlyoutWidth;
        }

        if (_resultsList is not null)
        {
            _resultsList.MaxHeight = ResultsMaxHeight;
        }

        if (_searchTextBox is not null)
        {
            _searchTextBox.PlaceholderText = PlaceholderText;
        }
    }

    private void Results_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateNoResultsState();
        QueueFlyoutLayoutRefresh();
    }

    private void ResultsList_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        QueueFlyoutLayoutRefresh();
    }

    private void QueueFlyoutLayoutRefresh()
    {
        if (_layoutRefreshQueued || _resultsList is null)
        {
            return;
        }

        _layoutRefreshQueued = true;

        DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () =>
        {
            _layoutRefreshQueued = false;
            RefreshFlyoutLayout();
            _flyoutRoot?.UpdateLayout();
        });
    }

    private void RefreshFlyoutLayout()
    {
        if (_resultsList is null)
        {
            return;
        }

        DependencyObject? current = _resultsList;

        while (current is not null)
        {
            if (current is UIElement element)
            {
                element.InvalidateMeasure();
            }

            current = VisualTreeHelper.GetParent(current);
        }
    }

    private void UpdateNoResultsState()
    {
        if (_noResultsPresenter is null)
        {
            return;
        }

        var show = Results.Count == 0 && !string.IsNullOrWhiteSpace(QueryText);
        _noResultsPresenter.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
    }

    private void Flyout_Opening(object? sender, object e)
    {
        if (_flyout is not null && FlyoutPlacement == FlyoutPlacementMode.Auto)
        {
            _flyout.Placement = ResolveAutoPlacement();
        }
    }

    private FlyoutPlacementMode ResolveAutoPlacement()
    {
        if (XamlRoot is null || _triggerButton is null)
        {
            return FlyoutPlacementMode.Bottom;
        }

        var origin = _triggerButton.TransformToVisual(null).TransformPoint(new Point(0, 0));
        var needed = ResultsMaxHeight + 130;
        var below = XamlRoot.Size.Height - (origin.Y + _triggerButton.ActualHeight);
        var above = origin.Y;

        if (below >= needed || below >= above)
        {
            return FlyoutPlacementMode.Bottom;
        }

        return FlyoutPlacementMode.Top;
    }

    private bool TryResolveOpenedParts()
    {
        if (_searchTextBox is null)
        {
            ResolveOpenedParts();
        }

        return _searchTextBox is not null;
    }

    private void FocusSearchBox()
    {
        _searchTextBox?.Focus(FocusState.Programmatic);
        _searchTextBox?.SelectAll();
    }

    private void Flyout_Opened(object? sender, object e)
    {
        if (TryResolveOpenedParts())
        {
            FocusSearchBox();
            return;
        }

        DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () =>
        {
            if (TryResolveOpenedParts())
            {
                FocusSearchBox();
            }
        });
    }

    private void Flyout_Closed(object? sender, object e)
    {
        if (!ShowPreviousSearch)
        {
            ClearQuery();
        }
    }

    private void ClearQuery()
    {
        _suppressQueryChanged = true;

        if (_searchTextBox is not null)
        {
            _searchTextBox.Text = string.Empty;
        }

        _updatingQueryText = true;
        QueryText = string.Empty;
        _updatingQueryText = false;
        _suppressQueryChanged = false;

        foreach (var item in Results)
        {
            item.IsSelected = false;
        }

        if (_resultsList is not null)
        {
            _resultsList.SelectedItem = null;
        }

        QueryChanged?.Invoke(this, new SearchEventArgs(string.Empty));
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_suppressQueryChanged || _searchTextBox is null)
        {
            return;
        }

        var text = _searchTextBox.Text;

        _updatingQueryText = true;
        QueryText = text;
        _updatingQueryText = false;

        QueryChanged?.Invoke(this, new SearchEventArgs(text));
    }

    private void SearchTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        switch (e.Key)
        {
            case VirtualKey.Down:
                MoveSelection(1);
                e.Handled = true;
                break;
            case VirtualKey.Up:
                MoveSelection(-1);
                e.Handled = true;
                break;
            case VirtualKey.Enter:
                Invoke(_resultsList?.SelectedItem as SearchResultItem);
                e.Handled = true;
                break;
            case VirtualKey.Escape:
                Hide();
                e.Handled = true;
                break;
        }
    }

    private void MoveSelection(int delta)
    {
        if (_resultsList is null || Results.Count == 0)
        {
            return;
        }

        var index = Math.Clamp(_resultsList.SelectedIndex + delta, 0, Results.Count - 1);
        _resultsList.SelectedIndex = index;
        _resultsList.ScrollIntoView(_resultsList.SelectedItem);
    }

    private void ResultsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        foreach (var removed in e.RemovedItems)
        {
            if (removed is SearchResultItem item)
            {
                item.IsSelected = false;
            }
        }

        foreach (var added in e.AddedItems)
        {
            if (added is SearchResultItem item)
            {
                item.IsSelected = true;
            }
        }
    }

    private void ResultsList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        Invoke(_resultsList?.SelectedItem as SearchResultItem);
    }

    private void OpenButton_Click(object sender, RoutedEventArgs e)
    {
        Invoke(_resultsList?.SelectedItem as SearchResultItem);
    }

    internal void InvokeItem(SearchResultItem item)
    {
        Invoke(item);
    }

    internal void ShowItemMenu(FrameworkElement anchor, SearchResultItem item)
    {
        IList<SearchMenuItem> source = item.MenuItems.Count > 0 ? item.MenuItems : DefaultMenuItems;
        if (source.Count == 0)
        {
            return;
        }

        var menu = new MenuFlyout { ShouldConstrainToRootBounds = ConstrainToAppBounds };

        foreach (var entry in source)
        {
            if (entry.IsSeparator)
            {
                menu.Items.Add(new MenuFlyoutSeparator());
                continue;
            }

            var menuItem = new MenuFlyoutItem { Text = entry.Text, Tag = entry.Tag };
            if (entry.IconSource is not null)
            {
                menuItem.Icon = entry.IconSource.CreateIconElement();
            }

            menuItem.Click += (_, _) =>
            {
                entry.Action?.Invoke(item);
                MenuItemInvoked?.Invoke(this, new SearchMenuItemEventArgs(item, entry));
            };

            menu.Items.Add(menuItem);
        }

        menu.ShowAt(anchor);
    }

    private void Invoke(SearchResultItem? item)
    {
        ResultInvoked?.Invoke(this, new SearchResultEventArgs(item, _searchTextBox?.Text ?? QueryText));
        Hide();
    }
}
