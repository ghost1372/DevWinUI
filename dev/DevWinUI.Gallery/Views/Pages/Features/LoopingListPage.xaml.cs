namespace DevWinUIGallery.Views;

public sealed partial class LoopingListPage : Page
{
    public LoopingListPage()
    {
        InitializeComponent();

        LoopingListSample.PrimaryItems = new List<string>
        {
            "USA", "Canada", "Germany", "Japan", "Australia",
            "France", "Brazil", "India", "Italy", "Spain",
            "Mexico", "China", "Russia", "Egypt", "South Africa"
        };

        LoopingListSample.SecondaryItems = new List<string>
        {
            "California", "Ontario", "Bavaria", "Tokyo", "New South Wales",
            "Île-de-France", "São Paulo", "Maharashtra", "Lombardy", "Catalonia",
            "Jalisco", "Guangdong", "Moscow Oblast", "Cairo Governorate", "Gauteng"
        };

        LoopingListSample.TertiaryItems = new List<string>
        {
            "Los Angeles", "Toronto", "Munich", "Shinjuku", "Sydney",
            "Paris", "São Paulo City", "Mumbai", "Milan", "Barcelona",
            "Guadalajara", "Guangzhou", "Moscow", "Cairo", "Johannesburg"
        };
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        LoopingListSample.ClearSelection();
    }
}
