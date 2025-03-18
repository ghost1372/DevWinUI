using Microsoft.UI.Windowing;

namespace $safeprojectname$.Views;

public sealed partial class MainWindow : Window
{
    public MainViewModel ViewModel { get; }
    public MainWindow()
    {
        ViewModel = App.GetService<MainViewModel>();
        this.InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

        ((OverlappedPresenter)AppWindow.Presenter).PreferredMinimumWidth = 800;
        ((OverlappedPresenter)AppWindow.Presenter).PreferredMinimumHeight = 600;
    }
}

