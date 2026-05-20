using Microsoft.Windows.ApplicationModel.WindowsAppRuntime;

namespace DevWinUIGallery.Views;

public sealed partial class HomeLandingPage : Page
{
    public string WASDKVersion { get; } = $"Windows App SDK 1.8+";
    public HomeLandingPage()
    {
        this.InitializeComponent();
    }
}

