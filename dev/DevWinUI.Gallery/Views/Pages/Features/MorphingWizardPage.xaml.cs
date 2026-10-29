namespace DevWinUIGallery.Views;

public sealed partial class MorphingWizardPage : Page
{
    public MorphingWizardPage()
    {
        InitializeComponent();
    }

    private void Sample_StepChanged(object? sender, int index)
    {
        StatusText.Text = $"Step {index + 1}";
    }

    private void Sample_Completed(object? sender, EventArgs e)
    {
        StatusText.Text = $"Completed at {DateTime.Now:T}";
    }

    private void GoToStep3_Click(object sender, RoutedEventArgs e)
    {
        Sample.GoTo(2);
    }
}
