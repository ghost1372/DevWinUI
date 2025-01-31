namespace $safeprojectname$;

public partial class App : Application
{
    public new static App Current => (App)Application.Current;
    public static Window MainWindow = Window.Current;
    public JsonNavigationService NavService { get; set; }
    public IThemeService ThemeService { get; set; }

    public App()
    {
        this.InitializeComponent();
        NavService = new JsonNavigationService();
    }

    protected $OnLaunchedAsyncKeyword$override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();
       
        ThemeService = new ThemeService(MainWindow);

        MainWindow.Title = MainWindow.AppWindow.Title = ProcessInfoHelper.ProductNameAndVersion;
        MainWindow.AppWindow.SetIcon("Assets/AppIcon.ico");$ConfigLogger$

        MainWindow.Activate();$Windows11ContextMenuInitializer$$UnhandeledException$
    }
}

