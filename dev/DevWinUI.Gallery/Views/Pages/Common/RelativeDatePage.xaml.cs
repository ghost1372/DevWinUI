namespace DevWinUIGallery.Views;

public sealed partial class RelativeDatePage : Page
{
    public RelativeDatePage()
    {
        this.InitializeComponent();
        Loaded += RelativeDatePage_Loaded;
    }

    private void RelativeDatePage_Loaded(object sender, RoutedEventArgs e)
    {
        DatePickerSample.SelectedDate = DateTime.Now;
    }

    private void DatePickerSample_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
    {
        TextBlockSample.Text = RelativeDate.Get(args.NewDate.Value).ToString("fa-IR");
        TextBlockSample2.Text = RelativeDate.Get(args.NewDate.Value).ToString();
    }
}
