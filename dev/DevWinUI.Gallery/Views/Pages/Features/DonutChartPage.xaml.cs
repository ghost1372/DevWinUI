namespace DevWinUIGallery.Views;

public sealed partial class DonutChartPage : Page
{
    public ObservableCollection<DonutChartDatum> Data { get; } = new()
    {
        new DonutChartDatum("Desktop", 4820),
        new DonutChartDatum("Mobile", 3260),
        new DonutChartDatum("Tablet", 1240),
        new DonutChartDatum("Other", 680),
    };

    public DonutChartPage()
    {
        InitializeComponent();
    }

    private void AddSlice_Click(object sender, RoutedEventArgs e)
    {
        Data.Add(new DonutChartDatum($"Item {Data.Count + 1}", Random.Shared.Next(200, 1500)));
    }

    private void RemoveSlice_Click(object sender, RoutedEventArgs e)
    {
        if (Data.Count > 0)
        {
            Data.RemoveAt(Data.Count - 1);
        }
    }
}
