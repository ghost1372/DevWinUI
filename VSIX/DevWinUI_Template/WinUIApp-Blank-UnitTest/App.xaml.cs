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
        Microsoft.VisualStudio.TestPlatform.TestExecutor.UnitTestClient.CreateDefaultUI();

        MainWindow = new Window();

        if (MainWindow.Content is not Frame rootFrame)
        {
            MainWindow.Content = rootFrame = new Frame();
        }

        rootFrame.Navigate(typeof(MainPage));
        MainWindow.Activate();
        
        UITestMethodAttribute.DispatcherQueue = MainWindow.DispatcherQueue;
        Microsoft.VisualStudio.TestPlatform.TestExecutor.UnitTestClient.Run(Environment.CommandLine);
    }
}

