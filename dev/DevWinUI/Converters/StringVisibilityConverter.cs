namespace DevWinUI;

public partial class StringVisibilityConverter : EmptyStringToObjectConverter
{
    public StringVisibilityConverter()
    {
        NotEmptyValue = Visibility.Visible;
        EmptyValue = Visibility.Collapsed;
    }
}
