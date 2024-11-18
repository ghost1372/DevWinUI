namespace DevWinUIGallery.Views;

public sealed partial class PinBoxPage : Page
{
    public PinBoxPage()
    {
        this.InitializeComponent();
    }

    public Orientation GetOrientation(int index)
    {
        return (Orientation)index;
    }

    public PasswordRevealMode GetPasswordRevealMode(int index)
    {
        return (PasswordRevealMode)index;
    }

    public PinBoxFocusMode GetFocusMode(int index)
    {
        return (PinBoxFocusMode)index;
    }

    public HorizontalAlignment GetHorizontalAlignment(int index)
    {
        return (HorizontalAlignment)index;
    }

    private void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (pinBox != null)
        {
            pinBox.PasswordLength = Convert.ToInt32(args.NewValue);
        }
    }

    private void Slider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (pinBox != null)
        {
            pinBox.ItemSpacing = e.NewValue;
        }
    }
}
