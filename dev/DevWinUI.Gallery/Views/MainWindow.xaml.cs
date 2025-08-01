using Microsoft.UI.Xaml.Input;

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

        new ModernSystemMenu(this);

        var NavService = App.GetService<IJsonNavigationService>() as JsonNavigationService;
        if (NavService != null)
        {
            NavService.Initialize(NavView, NavFrame, NavigationPageMappings.PageDictionary)
                .ConfigureDefaultPage(typeof(HomeLandingPage))
                .ConfigureSettingsPage(typeof(SettingsPage))
                .ConfigureSectionPage(typeof(DemoSectionPage))
                .ConfigureJsonFile("Assets/NavViewMenu/AppData.json", OrderItemsType.AscendingBoth)
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

    private void OnIconPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        AppHelper.PostMessage(App.Hwnd, (uint)NativeValues.WindowMessage.WM_SYSCOMMAND, 0xF090, IntPtr.Zero);
    }
}

