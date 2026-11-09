namespace DevWinUIGallery.Views;

public sealed partial class TypeFlowPage : Page
{
    public TypeFlowPage()
    {
        InitializeComponent();
    }

    private void OnRestart(object sender, RoutedEventArgs e) => Flow.Restart();

    private void OnSkip(object sender, RoutedEventArgs e) => Flow.Skip();
}
