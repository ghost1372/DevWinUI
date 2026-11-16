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

    private string GetPersian(DateTimeOffset? date) => date is { } value ? RelativeDate.Get(value).ToString("fa-IR") : string.Empty;

    private string GetDefault(DateTimeOffset? date) => date is { } value ? RelativeDate.Get(value).ToString() : string.Empty;
}
