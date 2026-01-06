using DevWinUI.Gallery;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Input;
using WinRT;

namespace DevWinUIGallery.Views;

public sealed partial class MainWindow : Window
{
    public MainViewModel ViewModel { get; }
    internal static MainWindow Instance { get; private set; }
    public SystemTrayIcon TrayIcon { get; set; }
    public MainWindow()
    {
        ViewModel = App.GetService<MainViewModel>();
        this.InitializeComponent();
        Instance = this;

        Closed += OnMainWindowClosed;
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

    private void OnMainWindowClosed(object sender, WindowEventArgs args)
    {
        RemoveTrayIcon();
    }

    private void ThemeButton_Click(object sender, RoutedEventArgs e)
    {
        App.Current.ThemeService.SetElementThemeWithoutSaveAsync();
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

    public void AddTrayIcon(string toolTip)
    {
        uint iconId = 123;
        if (TrayIcon is null)
        {
            var icon = WindowHelper.GetWindowIcon(this);
            TrayIcon = new SystemTrayIcon(iconId, icon, toolTip);
            TrayIcon.LeftClick += OnTrayIconLeftClick;
            TrayIcon.RightClick += OnTrayIconRightClick;
        }
        TrayIcon.IsVisible = true;
    }

    public void RemoveTrayIcon()
    {
        if (TrayIcon is not null)
        {
            TrayIcon.LeftClick -= OnTrayIconLeftClick;
            TrayIcon.RightClick -= OnTrayIconRightClick;
            TrayIcon.IsVisible = false;
            TrayIcon.Dispose();
            TrayIcon = null;
        }
    }

    private void OnTrayIconRightClick(SystemTrayIcon sender, SystemTrayIconEventArgs args)
    {
        var flyout = new MenuFlyout();
        flyout.Items.Add(new MenuFlyoutItem() { Text = "DevWinUI", IsEnabled = false });
        flyout.Items.Add(new MenuFlyoutItem() { Text = "Open DevWinUI Gallery" });
        flyout.Items.Add(new MenuFlyoutSeparator());
        flyout.Items.Add(new MenuFlyoutItem() { Text = "Exit" });

        ((MenuFlyoutItem)flyout.Items.Last()).Click += (s, e) => this.Close();

        var submenu = new MenuFlyoutSubItem() { Text = "About" };
        submenu.Items.Add(new MenuFlyoutItem() { Text = "GitHub" });
        submenu.Items.Add(new MenuFlyoutItem() { Text = "Docs" });
        flyout.Items.Add(submenu);

        args.Flyout = flyout;
    }

    private void OnTrayIconLeftClick(SystemTrayIcon sender, SystemTrayIconEventArgs args)
    {
        AppWindow.Presenter.As<OverlappedPresenter>().Restore();
        WindowHelper.SetForegroundWindow(this);
    }
}

