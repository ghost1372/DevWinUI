namespace DevWinUIGallery.Views;

public sealed partial class UnderlineAnimationPage : Page
{
    public ObservableCollection<UnderlineKind> KindItems { get; set; } = new ObservableCollection<UnderlineKind>(Enum.GetValues<UnderlineKind>());

    public UnderlineAnimationPage()
    {
        InitializeComponent();
    }

    private void OnTabClick(object? sender, object e)
    {
        foreach (var tab in TabsPanel.Children.OfType<UnderlineAnimation>())
        {
            tab.Active = ReferenceEquals(tab, sender);
        }
    }
}
