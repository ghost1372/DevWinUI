namespace DevWinUI;
public partial class NullToVisibilityConverter : NullToObjectConverter
{
    public NullToVisibilityConverter()
    {
        NullValue = Visibility.Collapsed;
        NotNullValue = Visibility.Visible;
    }
}
