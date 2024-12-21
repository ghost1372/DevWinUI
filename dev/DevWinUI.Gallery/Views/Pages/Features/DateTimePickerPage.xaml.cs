namespace DevWinUIGallery.Views;

public sealed partial class DateTimePickerPage : Page
{
    public DateTimePickerPage()
    {
        this.InitializeComponent();
    }

    private void RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        var tag = (sender as RadioButton).Tag;
        var displayMode = GeneralHelper.GetEnum<TimePickerDisplayMode>(tag?.ToString());
        DateTimePickerSample2.TimePickerDisplayMode = displayMode;
    }
}
