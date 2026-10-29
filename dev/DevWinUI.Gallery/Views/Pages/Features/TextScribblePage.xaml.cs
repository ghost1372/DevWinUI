namespace DevWinUIGallery.Views;

public sealed partial class TextScribblePage : Page
{
    public ObservableCollection<TextScribbleType> TypeItems { get; set; } = new ObservableCollection<TextScribbleType>(Enum.GetValues<TextScribbleType>());
    public ObservableCollection<TextScribbleTrigger> TriggerItems { get; set; } = new ObservableCollection<TextScribbleTrigger>(Enum.GetValues<TextScribbleTrigger>());

    public TextScribblePage()
    {
        InitializeComponent();
    }

    private bool? GetDrawn(int index) => index switch
    {
        1 => true,
        2 => false,
        _ => null
    };

    private int? GetIterations(double value) => double.IsNaN(value) ? null : (int)value;
}
