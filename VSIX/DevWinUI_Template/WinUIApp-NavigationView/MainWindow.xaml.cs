using Microsoft.UI.Windowing;

namespace $safeprojectname$.Views;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

        ((OverlappedPresenter)AppWindow.Presenter).PreferredMinimumWidth = 800;
        ((OverlappedPresenter)AppWindow.Presenter).PreferredMinimumHeight = 600;

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
