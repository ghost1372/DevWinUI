namespace DevWinUIGallery.Views;

public sealed partial class TextHighlightPage : Page
{
    public ObservableCollection<TextHighlightTrigger> TriggerItems { get; set; } = new ObservableCollection<TextHighlightTrigger>(Enum.GetValues<TextHighlightTrigger>());

    public TextHighlightPage()
    {
        InitializeComponent();
    }

    private bool? GetDrawn(int index) => index switch
    {
        1 => true,
        2 => false,
        _ => null
    };
}
