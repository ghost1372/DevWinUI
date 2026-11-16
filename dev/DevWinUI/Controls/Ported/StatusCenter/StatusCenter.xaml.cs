// Copyright (c) Files Community
// Licensed under the MIT License.

using System.Collections;
using System.ComponentModel;

namespace DevWinUI;

public sealed partial class StatusCenter : Control
{
    private static readonly StatusCenterItemTemplates ItemTemplateResources = new();
    private static readonly DataTemplate StatusCenterItemDataTemplate = (DataTemplate)ItemTemplateResources["StatusCenterItemTemplate"];

    private StatusCenterViewModel _viewModel = new();

    internal StatusCenterViewModel ViewModel
    {
        get => _viewModel;
        set
        {
            if (_viewModel == value)
                return;

            if (_isViewModelSubscribed)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
                _isViewModelSubscribed = false;
            }

            _viewModel = value;
            StatusCenterItems = value.StatusCenterItems;
            RefreshHasAnyItem();

            if (IsLoaded)
            {
                _viewModel.WindowHandle = WindowHelper.GetWindowHandle(this);
                _isViewModelSubscribed = true;
                _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }
    }

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

    public SizeUnitTypes SizeUnitFormat
    {
        get => _viewModel.SizeUnitFormat;
        set => _viewModel.SizeUnitFormat = value;
    }

    public StatusCenterItem AddCard(
        ReturnResult status,
        string header,
        string subHeader,
        string message,
        StatusCenterItemIconKind iconKind,
        bool canProvideProgress = true,
        long itemsCount = 0,
        long totalSize = 0,
        Func<StatusCenterItem, StatusCenterItemProgressModel, (string Header, string SubHeader, string Message)>? update = null)
        => _viewModel.AddCard(status, header, subHeader, message, iconKind, canProvideProgress, itemsCount, totalSize, update);

    public bool RemoveItem(StatusCenterItem item)
        => _viewModel.RemoveItem(item);

    public void RemoveAllCompletedItems()
        => _viewModel.RemoveAllCompletedItems();

    public static void UpdateCardStrings(StatusCenterItem card, StatusCenterItemProgressModel? progressValue = null)
        => card.UpdateCardStrings(progressValue);

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
