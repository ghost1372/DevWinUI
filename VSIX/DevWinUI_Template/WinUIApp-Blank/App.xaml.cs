namespace $safeprojectname$;

public partial class App : Application
{
    public static Window MainWindow = Window.Current;
    
    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new Window();

        if (MainWindow.Content is not Frame rootFrame)
        {
            MainWindow.Content = rootFrame = new Frame();
        }

        rootFrame.Navigate(typeof(MainPage));
        MainWindow.Activate();
    }
}

