namespace DevWinUIGallery.Views;

public sealed partial class TypeFlowPage : Page
{
    public TypeFlowPage()
    {
        InitializeComponent();
    }

    private void OnSkip(object sender, RoutedEventArgs e) => Flow.Skip();

    private void ControlExample_ResetRequested(object sender, EventArgs e)
    {
        Flow.Restart();
    }
}
