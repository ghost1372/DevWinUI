namespace DevWinUIGallery.Views;

public sealed partial class BruteForcePage : Page
{
    public BruteForcePage()
    {
        InitializeComponent();
        Loaded += Page_Loaded;
        Unloaded += Page_Unloaded;
    }

    private bool _canHandle;

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        _canHandle = true;

        var random = new Random();

        while (_canHandle)
        {
            BruteForceSample.Reset();
            BruteForceSample2.Reset();

            int warningIndex = random.Next(TxtPassword.Text.Length);

            int errorIndex;
            do
            {
                errorIndex = random.Next(TxtPassword.Text.Length);
            }
            while (errorIndex == warningIndex);

            for (int i = 0; i < TxtPassword.Text.Length && _canHandle; i++)
            {
                await Task.Delay(random.Next(300, 600));

                if (!_canHandle)
                    break;

                var state = i switch
                {
                    _ when i == warningIndex => BruteForceState.Warning,
                    _ when i == errorIndex => BruteForceState.Error,
                    _ => BruteForceState.Success
                };

                BruteForceSample.UpdateCharacterState(i, state);
                BruteForceSample2.UpdateCharacterState(i, state);
            }

            if (_canHandle)
                await Task.Delay(3000);
        }
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        _canHandle = false;
    }
}
