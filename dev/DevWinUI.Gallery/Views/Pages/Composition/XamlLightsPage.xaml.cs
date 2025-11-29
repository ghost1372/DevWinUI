namespace DevWinUIGallery.Views;

public sealed partial class XamlLightsPage : Page
{
    public XamlLightsPage()
    {
        InitializeComponent();

        AmbGrid.Lights.Add(new AmbLight());
        HoverGrid.Lights.Add(new HoverLight());
        RippleGrid.Lights.Add(new RippleLight());
        AllGrid.Lights.Add(new AmbLight());
        AllGrid.Lights.Add(new RippleLight());
        AllGrid.Lights.Add(new HoverLight());
    }
}
