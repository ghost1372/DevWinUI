using WinRT.Interop;

namespace DevWinUIGallery.Views;

public sealed partial class ModalWindowPage : Page
{
    public ModalWindowPage()
    {
        this.InitializeComponent();
    }

    private void OnCreateModalWindowClick(object sender, RoutedEventArgs args)
    {
        var window = new SampleModalWindow(WindowNative.GetWindowHandle(App.MainWindow));
        window.Activate();
    }
}
