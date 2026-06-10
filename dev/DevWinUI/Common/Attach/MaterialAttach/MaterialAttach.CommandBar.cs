namespace DevWinUI;

public static partial class MaterialAttach
{
    private static void HandleCommandBar(CommandBar commandBar, bool newValue)
    {
        if (newValue)
        {
            var scrollViewerStyle = Application.Current.Resources["ScrollViewerWithSystemBackdropElementStyle"] as Style;
            commandBar.Resources[typeof(ScrollViewer)] = scrollViewerStyle;
        }
        else
        {
            var scrollViewerStyle = Application.Current.Resources["DefaultScrollViewerStyle"] as Style;
            commandBar.Resources[typeof(ScrollViewer)] = scrollViewerStyle;
        }
    }
}
