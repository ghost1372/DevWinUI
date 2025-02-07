namespace DevWinUIGallery.Views;

public sealed partial class MainWindow : Window
{
    public MainViewModel ViewModel { get; }
    internal static MainWindow Instance { get; private set; }
    public MainWindow()
    {
        ViewModel = App.GetService<MainViewModel>();
        this.InitializeComponent();
        Instance = this;

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;
        
        var NavService = App.GetService<IJsonNavigationService>() as JsonNavigationService;
        if (NavService != null)
        {
            NavService.Initialize(NavView, NavFrame, NavigationPageMappings.PageDictionary)
                .ConfigureDefaultPage(typeof(HomeLandingPage))
                .ConfigureSettingsPage(typeof(SettingsPage))
                .ConfigureSectionPage(typeof(DemoSectionPage))
                .ConfigureJsonFile("Assets/NavViewMenu/AppData.json")
                .ConfigureAutoSuggestBox(HeaderAutoSuggestBox)
                .ConfigureTitleBar(AppTitleBar)
                .ConfigureBreadcrumbBar(BreadCrumbNav, BreadcrumbPageMappings.PageDictionary);
        }
    }

    private void ThemeButton_Click(object sender, RoutedEventArgs e)
    {
        ThemeService.ChangeThemeWithoutSave(App.MainWindow);
    }

    private void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        AutoSuggestBoxHelper.OnITitleBarAutoSuggestBoxTextChangedEvent(sender, args, NavFrame);
    }

    private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        AutoSuggestBoxHelper.OnITitleBarAutoSuggestBoxQuerySubmittedEvent(sender, args, NavFrame);
    }

    private void KeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        HeaderAutoSuggestBox.Focus(FocusState.Programmatic);
    }
}

