namespace $safeprojectname$.Views;

public sealed partial class AboutUsSettingPage : Page
{
    public AboutUsSettingViewModel ViewModel { get; }

    public AboutUsSettingPage()
    {
        ViewModel = App.GetService<AboutUsSettingViewModel>();
        this.InitializeComponent();
    }
}


