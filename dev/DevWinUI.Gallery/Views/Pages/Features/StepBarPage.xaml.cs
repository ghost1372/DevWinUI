namespace DevWinUIGallery.Views;

public sealed partial class StepBarPage : Page
{
    public StepBarPage()
    {
        this.InitializeComponent();
    }

    private void BtnNext_Click(object sender, RoutedEventArgs e)
    {
        StepBarSample.Next();
    }

    private void BtnPrev_Click(object sender, RoutedEventArgs e)
    {
        StepBarSample.Prev();
    }

    private void Cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (StepBarSample == null || Cmb == null)
        {
            return;
        }
        var item = Cmb.SelectedItem as ComboBoxItem;
        if (item != null)
        {
            var status = GeneralHelper.GetEnum<StepStatus>(item.Tag.ToString());
            StepBarSample.Status = status;
        }
    }

    private void CmbOrientation_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (StepBarSample == null || CmbOrientation == null)
        {
            return;
        }

        var item = CmbOrientation.SelectedItem as ComboBoxItem;
        if (item != null)
        {
            var orientation = GeneralHelper.GetEnum<Orientation>(item.Tag.ToString());
            StepBarSample.Orientation = orientation;
        }
    }

    private void CmbHeaderDisplayMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (StepBarSample == null || CmbHeaderDisplayMode == null)
        {
            return;
        }
        var item = CmbHeaderDisplayMode.SelectedItem as ComboBoxItem;
        if (item != null)
        {
            var headerDisplayMode = GeneralHelper.GetEnum<StepBarHeaderDisplayMode>(item.Tag.ToString());
            StepBarSample.HeaderDisplayMode = headerDisplayMode;
        }
    }
}
