namespace DevWinUIGallery.Views;

public sealed partial class AccordionPage : Page
{
    public BaseViewModel ViewModel { get;}
    public AccordionPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }
}
