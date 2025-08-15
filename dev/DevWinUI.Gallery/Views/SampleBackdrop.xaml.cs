namespace DevWinUIGallery.Views;

public sealed partial class SampleBackdrop : Window
{
    public SampleBackdrop()
    {
        this.InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SystemBackdrop = new MicaSystemBackdrop();
    }

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SampleTxt == null)
        {
            return;
        }
        var item = CmbBackdrop.SelectedItem as ComboBoxItem;
        switch (item.Tag)
        {
            case "None":
                SystemBackdrop = null;
                SampleTxt.Text = "window.SystemBackdrop = null;";
                break;
            case "Mica":
                SystemBackdrop = new MicaSystemBackdrop();
                SampleTxt.Text = "window.SystemBackdrop = new MicaSystemBackdrop();";
                break;
            case "MicaAlt":
                SystemBackdrop = new MicaSystemBackdrop(Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt);
                SampleTxt.Text = "window.SystemBackdrop = new MicaSystemBackdrop(MicaKind.BaseAlt);";
                break;
            case "Acrylic":
                SystemBackdrop = new AcrylicSystemBackdrop();
                SampleTxt.Text = "window.SystemBackdrop = new AcrylicSystemBackdrop();";
                break;
            case "AcrylicThin":
                SystemBackdrop = new AcrylicSystemBackdrop(Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicKind.Thin);
                SampleTxt.Text = "window.SystemBackdrop = new AcrylicSystemBackdrop(DesktopAcrylicKind.Thin);";
                break;
            case "Transparent":
                SystemBackdrop = new TransparentBackdrop();
                SampleTxt.Text = "window.SystemBackdrop = new TransparentBackdrop();";
                break;
        }
    }
}
