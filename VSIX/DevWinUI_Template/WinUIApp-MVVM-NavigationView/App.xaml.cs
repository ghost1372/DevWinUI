namespace $safeprojectname$;

public partial class App : Application
{
    public new static App Current => (App)Application.Current;
    public static Window MainWindow = Window.Current;
    public IServiceProvider Services { get; }
    public IJsonNavigationService NavService => GetService<IJsonNavigationService>();
    public IThemeService ThemeService => GetService<IThemeService>();

    public static T GetService<T>() where T : class
    {
        if ((App.Current as App)!.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public App()
    {
        Services = ConfigureServices();
        this.InitializeComponent();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<IJsonNavigationService, JsonNavigationService>();

        services.AddTransient<MainViewModel>();$Services$

        return services.BuildServiceProvider();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();

        MainWindow.Title = MainWindow.AppWindow.Title = ProcessInfoHelper.ProductNameAndVersion;
        MainWindow.AppWindow.SetIcon("Assets/AppIcon.ico");

        ThemeService.AutoInitialize(MainWindow).ConfigureTintColor();

        MainWindow.Activate();

        InitializeApp();
    }

    private $AsyncKeyword$void InitializeApp()
    {
        $PackagedAppTaskId$$Windows11ContextMenuMVVMInitializer$$ConfigLogger$$UnhandeledException$
    }
}

