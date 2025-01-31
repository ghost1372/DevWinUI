namespace $safeprojectname$;

public partial class App : Application
{
    public new static App Current => (App)Application.Current;
    public static Window MainWindow = Window.Current;
    
    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();

        MainWindow.SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
        MainWindow.Activate();
    }
}

