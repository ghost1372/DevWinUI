using Microsoft.Windows.ApplicationModel.Resources;

namespace DevWinUI;

public partial class JsonNavigationService
{
    private bool _useBreadcrumbBar;
    private bool _autoManageBackButtonVisibility;
    private bool _deferNavigation = true;
    private BreadcrumbNavigator _mainBreadcrumb { get; set; }
    private NavigationView? _navigationView;
    private AutoSuggestBox? _autoSuggestBox;
    public IList<object>? MenuItems => _navigationView?.MenuItems;
    public IList<object>? FooterMenuItems => _navigationView?.FooterMenuItems;
    public object? SettingsItem => _navigationView?.SettingsItem;

    private Type _defaultPage;
    private Type _sectionPage;
    private Type _settingsPage;
    private string _fontFamilyForGlyph;
    private string _autoSuggestBoxNotFoundString;
    private string _autoSuggestBoxNotFoundImagePath;
    private OrderItemsType _orderItems;
    private PathType _pathType;
    private ResourceManager _resourceManager;
    private ResourceContext _resourceContext;
    public string JsonFilePath;

    private readonly Dictionary<string, (NavigationViewItem Item, NavigationViewItem? Parent)> _itemMap = new();
    private bool _suppressSelectionNavigation;
    private string? _pendingSelectionId;
    private Type? _pendingSelectionPageType;
    private object? _lastParameterUsed;
    private int _jsonConfigurationVersion;
    private Task _jsonLoadTask = Task.CompletedTask;
    private long _pendingClearNavigationSequence;
    private readonly List<(long Id, Type PageType, object? Parameter)> _pendingClearNavigations = new();
    public event NavigatedEventHandler? FrameNavigated;

    private bool _isTitlebarConfigured;
    private TitleBar _titleBar;

    public IDelegateCommand NavigateToCommand { get; }
}
