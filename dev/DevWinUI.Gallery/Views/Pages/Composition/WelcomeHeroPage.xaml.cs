namespace DevWinUIGallery.Views;

public sealed partial class WelcomeHeroPage : Page
{
    internal static WelcomeHeroPage Instance { get; private set; }
    public static bool WelcomeIntroPlayed { get; set; }
    public WelcomeHeroPage()
    {
        WelcomeIntroPlayed = false;
        InitializeComponent();
        Instance = this;

        MainFrame.Navigate(typeof(WelcomeHeroPage2));
    }

    public void NavigateToModule(string moduleTag)
    {
        MainFrame.Navigate(typeof(WelcomeHeroPage3), moduleTag);
    }
}
