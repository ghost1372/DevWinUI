using Microsoft.UI.Xaml.Media;
using WinRT;

namespace DevWinUIGallery.Views;
public sealed partial class UacStyleDialogWindowPage : Page
{
    public ObservableCollection<BackdropType> BackdropItems { get; set; } = new ObservableCollection<BackdropType>(Enum.GetValues<BackdropType>());
    public ObservableCollection<FlowDirection> FlowDirectionItems { get; set; } = new ObservableCollection<FlowDirection>(Enum.GetValues<FlowDirection>());
    public ObservableCollection<InfoBarSeverity> SeverityItems { get; set; } = new ObservableCollection<InfoBarSeverity>(Enum.GetValues<InfoBarSeverity>());

    public UacStyleDialogWindowPage()
    {
        InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        var underlayBackdrop = CmbUnderlayBackdrops.SelectedItem.As<BackdropType>();
        var flow = CmbFlow.SelectedItem.As<FlowDirection>();
        var severity = CmbSeverity.SelectedItem.As<InfoBarSeverity>();

        UacStyleDialogWindow dialog = new()
        {
            Header = txtTitle.Text,
            ExtendedHeader = txtTitle.Text,
            Content = txtContent.Text,
            PrimaryButtonContent = "PrimaryButtonText",
            SecondaryButtonContent = "SecondaryButtonText",
            CloseButtonContent = "CloseButtonText",
            Owner = MainWindow.Instance,
            HasTitleBar = TGHasTitleBar.IsOn,
            CanResize = TGIsResizable.IsOn,
            FlowDirection = flow,
            Severity = severity
        };

        switch (underlayBackdrop)
        {
            case BackdropType.None:
                dialog.SystemBackdrop = null;
                break;
            case BackdropType.Mica:
                dialog.SystemBackdrop = new MicaBackdrop();
                break;
            case BackdropType.MicaAlt:
                dialog.SystemBackdrop = new MicaBackdrop() { Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt };
                break;
            case BackdropType.Acrylic:
                dialog.SystemBackdrop = new AcrylicSystemBackdrop();
                break;
            case BackdropType.AcrylicThin:
                dialog.SystemBackdrop = new AcrylicSystemBackdrop(Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicKind.Thin);
                break;
            case BackdropType.Transparent:
                dialog.SystemBackdrop = new TransparentBackdrop();
                break;
        }

        dialog.ShowDialog();
    }
}
