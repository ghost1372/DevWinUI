namespace DevWinUI;

public partial class NavigationServiceEx
{
    private bool _useBreadcrumbBar;
    private bool _autoManageBackButtonVisibility;
    private BreadcrumbNavigator _mainBreadcrumb { get; set; }
    private NavigationView? _navigationView;
    public IList<object>? MenuItems => _navigationView?.MenuItems;
    public IList<object>? FooterMenuItems => _navigationView?.FooterMenuItems;
    private IList<object>? AllMenuItems => MenuItems.Concat(FooterMenuItems).ToList();
    public object? SettingsItem => _navigationView?.SettingsItem;

    private Type _defaultPage;
    private Type _settingsPage;

    private object? _lastParameterUsed;
    private Frame? _frame;
    public event NavigatedEventHandler? FrameNavigated;

    private bool _isTitlebarConfigured;
    private TitleBar _titleBar;
    public IDelegateCommand NavigateToCommand { get; }
}
