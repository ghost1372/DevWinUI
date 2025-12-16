namespace DevWinUIGallery.Views;

public sealed partial class ProfileControlPage : Page
{
    public BaseViewModel ViewModel { get; }
    private readonly List<Uri> _profiles;
    private int currentIndex;
    private int totalProfiles;
    public ProfileControlPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();

        _profiles = ViewModel.ProfileData.Select(x => PathHelper.GetFilePath(x.ImageUri)).ToList();
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
