namespace $safeprojectname$;

public partial class App : Application
{
    public static Window MainWindow = Window.Current;
    public IThemeService ThemeService { get; set; }
    public IJsonNavigationService NavigationService { get; set; }
    public new static App Current => (App)Application.Current;
    public IJsonNavigationService GetNavigationService => NavigationService;
    public IThemeService GetThemeService => ThemeService;

    public App()
    {
        this.InitializeComponent();
        NavigationService = new JsonNavigationService();
    }

    protected $OnLaunchedAsyncKeyword$override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new Window();
       
        if (MainWindow.Content is not Frame rootFrame)
        {
            MainWindow.Content = rootFrame = new Frame();
        }

        ThemeService = new ThemeService(MainWindow);

        rootFrame.Navigate(typeof(MainPage));

        MainWindow.Title = MainWindow.AppWindow.Title = ProcessInfoHelper.ProductNameAndVersion;
        MainWindow.AppWindow.SetIcon("Assets/AppIcon.ico");$ConfigLogger$

        MainWindow.Activate();$Windows11ContextMenuInitializer$$UnhandeledException$
    }
}

