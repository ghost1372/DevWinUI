namespace DevWinUIGallery.Views;

public sealed partial class AnimatedImagePage : Page
{
    public BaseViewModel ViewModel { get; }
    private DispatcherTimer _timer;
    private readonly Random _random = new();
    public AnimatedImagePage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        ViewModel.AnimatedImageImagePath = ViewModel.LandscapeData[0].ImageUri;

        InitializeComponent();
    }
    public void InitAnimatedImageTimer()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2)
        };
        _timer.Tick -= Timer_Tick;
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }
    public void StopTimer()
    {
        _timer?.Stop();
        _timer = null;
    }
    private void Timer_Tick(object sender, object e)
    {
        var index = _random.Next(ViewModel.LandscapeData.Count);
        ViewModel.AnimatedImageImagePath = ViewModel.LandscapeData[index].ImageUri;
    }
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        InitAnimatedImageTimer();
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        StopTimer();
    }
}
