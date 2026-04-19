using Microsoft.Windows.ApplicationModel.WindowsAppRuntime;

namespace DevWinUIGallery.Views;

public sealed partial class HomeLandingPage : Page
{
    public string WASDKVersion { get; } = $"Windows App SDK {ReleaseInfo.Major}.{ReleaseInfo.Minor}+";
    public HomeLandingPage()
    {
        this.InitializeComponent();
    }
}

