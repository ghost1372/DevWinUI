namespace DevWinUIGallery.Views;

public sealed partial class CountdownPage : Page
{
    public ObservableCollection<CountdownState> CountdownState { get; set; } = new ObservableCollection<CountdownState>(Enum.GetValues<CountdownState>());

    public CountdownPage()
    {
        InitializeComponent();
    }
}
