using WinRT;

namespace DevWinUIGallery.Views;

public sealed partial class ElementGroupPage : Page
{
    public ElementGroupPage()
    {
        this.InitializeComponent();
    }

    private void CmbOrientation_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ElementGroupSample != null && ElementGroupSample2 != null && ElementGroupSample3 != null)
        {
            var item = CmbOrientation.SelectedItem.As<ComboBoxItem>();
            if (item.Tag is not string tag)
            {
                return;
            }

            var orientation = GeneralHelper.GetEnum<Orientation>(tag);
            ElementGroupSample.Orientation = orientation;
            ElementGroupSample2.Orientation = orientation;
            ElementGroupSample3.Orientation = orientation;
        }
    }
}
