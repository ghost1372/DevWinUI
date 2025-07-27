namespace DevWinUIGallery.Views;

public sealed partial class ConfettiCannonPage : Page
{
    public ConfettiCannonPage()
    {
        InitializeComponent();
    }

    private void OnFireBasicClick(object sender, RoutedEventArgs e)
    {
        ConfettiCannonSample.FireBasic();
    }
    private void OnFireRandomClick(object sender, RoutedEventArgs e)
    {
        ConfettiCannonSample.FireRandomDirection();
    }
    private void OnFireRealisticClick(object sender, RoutedEventArgs e)
    {
        ConfettiCannonSample.FireRealistic();
    }
    private void OnFireFireworksClick(object sender, RoutedEventArgs e)
    {
        ConfettiCannonSample.FireFireworks();
    }
    private void OnFireStarsClick(object sender, RoutedEventArgs e)
    {
        ConfettiCannonSample.FireStars();
    }
    private void OnFireSnowClick(object sender, RoutedEventArgs e)
    {
        ConfettiCannonSample.FireSnow();
    }
    private void OnFireSchoolPrideClick(object sender, RoutedEventArgs e)
    {
        ConfettiCannonSample.FireSchoolPride();
    }
}
