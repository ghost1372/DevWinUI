namespace $safeprojectname$.Views;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);
        App.Current.NavService
            .Initialize(NavView, NavFrame, NavigationPageMappings.PageDictionary)$ConfigDefaultPages$
            .ConfigureJsonFile("Assets/NavViewMenu/AppData.json")
            .ConfigureTitleBar(AppTitleBar)
            .ConfigureBreadcrumbBar(BreadCrumbNav, BreadcrumbPageMappings.PageDictionary);
    }

    private void ThemeButton_Click(object sender, RoutedEventArgs e)
    {
        ThemeService.ChangeThemeWithoutSave(App.MainWindow);
    }
}
