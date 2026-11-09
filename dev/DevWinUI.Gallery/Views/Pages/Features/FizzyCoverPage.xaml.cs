namespace DevWinUIGallery.Views;

public sealed partial class FizzyCoverPage : Page
{
    public ObservableCollection<FizzyTrigger> Triggers { get; set; } = new ObservableCollection<FizzyTrigger>(Enum.GetValues<FizzyTrigger>());
    public ObservableCollection<FizzyShape> Shapes { get; set; } = new ObservableCollection<FizzyShape>(Enum.GetValues<FizzyShape>());

    public FizzyCoverPage()
    {
        InitializeComponent();
    }

    public int ToInt(double value) => (int)value;
}
