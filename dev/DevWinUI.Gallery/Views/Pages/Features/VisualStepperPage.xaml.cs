namespace DevWinUIGallery.Views;

public sealed partial class VisualStepperPage : Page
{
    public VisualStepperPage()
    {
        InitializeComponent();
    }

    private void OnNext(object sender, RoutedEventArgs e) => Stepper.SelectedIndex++;

    private void OnBack(object sender, RoutedEventArgs e) => Stepper.SelectedIndex--;
}
