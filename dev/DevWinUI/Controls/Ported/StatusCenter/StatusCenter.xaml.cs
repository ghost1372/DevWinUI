// Copyright (c) Files Community
// Licensed under the MIT License.

using System.Collections;
using System.ComponentModel;

namespace DevWinUI;

public sealed partial class StatusCenter : Control
{
    private static readonly StatusCenterItemTemplates ItemTemplateResources = new();
    private static readonly DataTemplate StatusCenterItemDataTemplate = (DataTemplate)ItemTemplateResources["StatusCenterItemTemplate"];

    private readonly StatusCenterViewModel _viewModel = StatusCenterViewModel.Instance;

    private Button? _closeAllItemsButton;
    private bool _isViewModelSubscribed;

    public static readonly DependencyProperty StatusCenterItemsProperty =
        DependencyProperty.Register(nameof(StatusCenterItems), typeof(IEnumerable), typeof(StatusCenter), new PropertyMetadata(null));

    public IEnumerable? StatusCenterItems
    {
        get => (IEnumerable?)GetValue(StatusCenterItemsProperty);
        private set => SetValue(StatusCenterItemsProperty, value);
    }

    public static readonly DependencyProperty HasAnyItemProperty =
        DependencyProperty.Register(nameof(HasAnyItem), typeof(bool), typeof(StatusCenter), new PropertyMetadata(false));

    public bool HasAnyItem
    {
        get => (bool)GetValue(HasAnyItemProperty);
        private set => SetValue(HasAnyItemProperty, value);
    }

    public static readonly DependencyProperty EmptyStateVisibilityProperty =
        DependencyProperty.Register(nameof(EmptyStateVisibility), typeof(Visibility), typeof(StatusCenter), new PropertyMetadata(Visibility.Visible));

    public Visibility EmptyStateVisibility
    {
        get => (Visibility)GetValue(EmptyStateVisibilityProperty);
        private set => SetValue(EmptyStateVisibilityProperty, value);
    }

    public StatusCenter()
    {
        DefaultStyleKey = typeof(StatusCenter);
        Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new("ms-appx:///DevWinUI/Controls/Ported/StatusCenter/Styles/StatusCenterStyles.xaml") });

        StatusCenterItems = _viewModel.StatusCenterItems;
        RefreshHasAnyItem();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _closeAllItemsButton?.Click -= CloseAllItemsButton_Click;
        _closeAllItemsButton = GetTemplateChild("CloseAllItemsButton") as Button;
        _closeAllItemsButton?.Click += CloseAllItemsButton_Click;

        if (GetTemplateChild("StatusCenterItemsListView") is ListView itemsListView)
            itemsListView.ItemTemplate = StatusCenterItemDataTemplate;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _viewModel.WindowHandle = WindowHelper.GetWindowHandle(this);
        RefreshHasAnyItem();

        if (_isViewModelSubscribed)
            return;

        _isViewModelSubscribed = true;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (!_isViewModelSubscribed)
            return;

        _isViewModelSubscribed = false;
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(StatusCenterViewModel.HasAnyItem))
            RefreshHasAnyItem();
    }

    private void RefreshHasAnyItem()
    {
        HasAnyItem = _viewModel.HasAnyItem;
        EmptyStateVisibility = HasAnyItem ? Visibility.Collapsed : Visibility.Visible;
    }

    private void CloseAllItemsButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.RemoveAllCompletedItems();
    }
}
