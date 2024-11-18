namespace DevWinUIGallery.Views;
public sealed partial class WatermarkPage : Page
{
    public WatermarkPage()
    {
        this.InitializeComponent();
    }

    private void nbHorizSpace_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (nbHorizSpace != null && watermark1 != null)
        {
            watermark1.HorizontalSpacing = (int)args.NewValue;
        }
    }

    private void nbVerSpace_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (nbVerSpace != null && watermark1 != null)
        {
            watermark1.VerticalSpacing = (int)args.NewValue;
        }
    }

    private void nbHorizSpace2_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (nbHorizSpace2 != null && watermark2 != null)
        {
            watermark2.HorizontalSpacing = (int)args.NewValue;
        }
    }

    private void nbVerSpace2_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (nbVerSpace2 != null && watermark2 != null)
        {
            watermark2.VerticalSpacing = (int)args.NewValue;
        }
    }
}
