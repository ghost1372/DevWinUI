namespace DevWinUI;
public partial class DevWinUI : ResourceDictionary
{
    public DevWinUI()
    {
        Source = new Uri("ms-appx:///DevWinUI.Controls/Themes/Generic.xaml", UriKind.Absolute);
    }
}
