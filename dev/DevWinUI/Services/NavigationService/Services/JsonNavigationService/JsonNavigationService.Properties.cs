using Microsoft.Windows.ApplicationModel.Resources;

namespace DevWinUI;

public partial class JsonNavigationService
{
    private bool _useBreadcrumbBar;
    private bool _allowDuplication;
    private BreadcrumbNavigator _mainBreadcrumb { get; set; }
    private NavigationView? _navigationView;
    private AutoSuggestBox? _autoSuggestBox;
    public IList<object>? MenuItems => _navigationView?.MenuItems;
    public IList<object>? FooterMenuItems => _navigationView?.FooterMenuItems;
    private IList<object>? AllMenuItems => MenuItems.Concat(FooterMenuItems).ToList();
    public object? SettingsItem => _navigationView?.SettingsItem;
    public ResourceManager ResourceManager { get; set; }
    public ResourceContext ResourceContext { get; set; }

    private Type _defaultPage;
    private Type _sectionPage;
    private Type _settingsPage;
    private string _fontFamilyForGlyph;
    private string _autoSuggestBoxNotFoundString;
    private string _autoSuggestBoxNotFoundImagePath;
    public string JsonFilePath;

    private object? _lastParameterUsed;
    private Frame? _frame;
    public event NavigatedEventHandler? FrameNavigated;

    public Window Window { get; set; }
    private bool _isBackNavigation = false;
    private bool _isTitlebarConfigured;
    private TitleBar _titleBar;
}
