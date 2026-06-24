using WinRT;

namespace DevWinUIGallery.Views;

public sealed partial class TextBlockHelperPage : Page
{
    public ObservableCollection<TextBlockSlideDirection> TextBlockSlideDirectionItems { get; set; } = new ObservableCollection<TextBlockSlideDirection>(Enum.GetValues<TextBlockSlideDirection>());
    private readonly string[] _demoTexts =
    {
        "Welcome to DevWinUI",
        "Modern WinUI Controls",
        "Smooth Text Animations",
        "Beautiful UI Components",
        "Fast and Lightweight",
        "Customizable Styles",
        "Built for Windows Apps",
        "Fluent Design Ready",
        "Easy to Integrate",
        "Open Source and Free"
    };
    private CancellationTokenSource? _cts;
    public TextBlockHelperPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _cts = new CancellationTokenSource();
        _ = StartDemoAsync(_cts.Token);
    }

    private async Task StartDemoAsync(CancellationToken token)
    {
        int index = 0;

        while (!token.IsCancellationRequested)
        {
            var direction = CmbType.SelectedItem.As<TextBlockSlideDirection>();
            await TextBlockHelper.AnimateTextChangeAsync(TextBlockSample, _demoTexts[index], direction);
            index = (index + 1) % _demoTexts.Length;

            try
            {
                await Task.Delay(500, token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}
