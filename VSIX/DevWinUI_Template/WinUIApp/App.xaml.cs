﻿namespace $safeprojectname$;

public partial class App : Application
{
    public new static App Current => (App)Application.Current;
    public static Window MainWindow = Window.Current;
    public static IntPtr Hwnd => WinRT.Interop.WindowNative.GetWindowHandle(MainWindow);
    public IThemeService ThemeService { get; set; }

    public App()
    {
        this.InitializeComponent();$BoostStartup$
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();

        MainWindow.Title = MainWindow.AppWindow.Title = ProcessInfoHelper.ProductNameAndVersion;
        MainWindow.AppWindow.SetIcon("Assets/AppIcon.ico");

        ThemeService = new ThemeService(MainWindow);

        MainWindow.Activate();

        InitializeApp();
    }

    private $AsyncKeyword$void InitializeApp()
    {
        $Windows11ContextMenuInitializer$$ConfigLogger$$UnhandeledException$
    }
}

