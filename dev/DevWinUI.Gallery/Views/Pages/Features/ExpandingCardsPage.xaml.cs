namespace DevWinUIGallery.Views;

public sealed partial class ExpandingCardsPage : Page
{
    public ExpandingCardsPage()
    {
        InitializeComponent();
    }

    private static int ToInt(double value) => (int)value;

    private static CornerRadius ToCornerRadius(double value) => new CornerRadius(value);
}
