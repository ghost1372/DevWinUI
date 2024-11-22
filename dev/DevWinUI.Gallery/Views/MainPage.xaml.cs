namespace DevWinUIGallery.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; }
    internal static MainPage Instance { get; private set; }
    private JsonNavigationViewService NavService;
    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        this.InitializeComponent();
        Instance = this;

        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);

        NavService = App.GetService<IJsonNavigationViewService>() as JsonNavigationViewService;
        if (NavService != null)
        {
            NavService
                .Initialize(NavView, NavFrame, NavigationPageMappings.PageDictionary)
                .ConfigureJsonFile("Assets/NavViewMenu/AppData.json")
                .ConfigureDefaultPage(typeof(HomeLandingPage))
                .ConfigureSettingsPage(typeof(SettingsPage))
                .ConfigureSectionPage(typeof(DemoSectionPage))
                .ConfigureAutoSuggestBox(HeaderAutoSuggestBox)
                .ConfigureBreadcrumbBar(JsonBreadCrumbNavigator, BreadcrumbPageMappings.PageDictionary);
        }
    }

    private void AppTitleBar_BackRequested(TitleBar sender, object args)
    {
        NavService?.GoBack();
    }

    private void AppTitleBar_PaneToggleRequested(TitleBar sender, object args)
    {
        NavView.IsPaneOpen = !NavView.IsPaneOpen;
    }

    private void NavFrame_Navigated(object sender, NavigationEventArgs e)
    {
        AppTitleBar.IsBackButtonVisible = NavFrame.CanGoBack;
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
}

