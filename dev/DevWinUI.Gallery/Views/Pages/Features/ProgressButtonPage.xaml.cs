namespace DevWinUIGallery.Views;

public sealed partial class ProgressButtonPage : Page
{
    public ProgressButtonPage()
    {
        this.InitializeComponent();
    }

    private async void ProgressButton_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is not ProgressButton pb)
        {
            return;
        }

        if (pb.IsChecked == true && !pb.IsIndeterminate)
        {
            pb.Progress = 0;
            while (true)
            {
                pb.Progress += 1;
                await Task.Delay(50);
                if (pb.Progress == 100)
                {
                    pb.IsChecked = false;
                    break;
                }
            }
        }
    }
}
