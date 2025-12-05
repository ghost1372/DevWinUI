namespace DevWinUIGallery.Views;

public sealed partial class ProfileControlPage : Page
{
    private readonly List<Uri> _profiles;
    private int currentIndex;
    private int totalProfiles;
    public ProfileControlPage()
    {
        InitializeComponent();

        _profiles = new List<Uri>
        {
            PathHelper.GetFilePath(new Uri("ms-appx:///Assets/Others/p1.jpg")),
            PathHelper.GetFilePath(new Uri("ms-appx:///Assets/Others/p2.jpg")),
            PathHelper.GetFilePath(new Uri("ms-appx:///Assets/Others/p3.jpg")),
            PathHelper.GetFilePath(new Uri("ms-appx:///Assets/Others/p4.jpg")),
            PathHelper.GetFilePath(new Uri("ms-appx:///Assets/Others/p5.jpg")),
            PathHelper.GetFilePath(new Uri("ms-appx:///Assets/Others/p6.jpg")),
        };

        currentIndex = 0;
        totalProfiles = _profiles.Count;

        profile.Source = _profiles[currentIndex];
    }

    private void BtnPrev_Click(object sender, RoutedEventArgs e)
    {
        currentIndex = (--currentIndex + totalProfiles) % totalProfiles;
        profile.Source = _profiles[currentIndex];
    }

    private void BtnNext_Click(object sender, RoutedEventArgs e)
    {
        currentIndex = (++currentIndex) % totalProfiles;
        profile.Source = _profiles[currentIndex];
    }
}
