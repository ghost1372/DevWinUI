namespace DevWinUIGallery.Views;

public sealed partial class DropdownColorPickerPage : Page
{
    public DropdownColorPickerPage()
    {
        InitializeComponent();
    }

    private void DropdownColorPickerSample_ColorChanged(object sender, DropdownColorPickerColorChangedEventArgs e)
    {
        TxtResult.Text = e.Color.ToString();
    }

    private void DropdownColorPickerSample2_ColorChanged(object sender, DropdownColorPickerColorChangedEventArgs e)
    {
        TxtResult2.Text = e.Color.ToString();
    }
}
