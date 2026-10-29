namespace DevWinUIGallery.Views;

public sealed partial class ScrambledTextPage : Page
{
    public ObservableCollection<ScrambledTextEffect> EffectItems { get; set; } = new ObservableCollection<ScrambledTextEffect>(Enum.GetValues<ScrambledTextEffect>());

    public ScrambledTextPage()
    {
        InitializeComponent();
    }
}
