namespace DevWinUIGallery.Views;

public sealed partial class WelcomeHeroPage3 : Page
{
    public WelcomeHeroPage3()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        var navigationTag = e.Parameter as string;
        ModuleTitleText.Text = string.IsNullOrEmpty(navigationTag) ? "Module" : navigationTag;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
        {
            Frame.GoBack();
        }
    }
}
