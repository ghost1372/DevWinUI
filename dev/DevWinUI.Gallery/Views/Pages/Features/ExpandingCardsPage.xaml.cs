namespace DevWinUIGallery.Views;

public sealed partial class ExpandingCardsPage : Page
{
    public ExpandingCardsPage()
    {
        InitializeComponent();
    }

    private int ToInt(double value) => (int)value;

    private CornerRadius ToCornerRadius(double value) => new CornerRadius(value);
}
