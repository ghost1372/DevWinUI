// https://github.com/SuGar0218/SuGarToolkit.WinUI3

namespace DevWinUI;

public sealed partial class UacStyleDialogView : ExtendedContentDialogView, IExtendedContentDialog
{
    public UacStyleDialogView()
    {
        DefaultStyleKey = typeof(UacStyleDialogView);
    }

    public InfoBarSeverity Severity
    {
        get => (InfoBarSeverity) GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }

    public static readonly DependencyProperty SeverityProperty = DependencyProperty.Register(
        nameof(Severity),
        typeof(InfoBarSeverity),
        typeof(UacStyleDialogView),
        new PropertyMetadata(default(InfoBarSeverity), OnSeverityChanged)
    );

    public static readonly string ErrorSeverityStyleKey = "UacStyleDialogViewErrorSeverityStyle";
    public static readonly string WarningSeverityStyleKey = "UacStyleDialogViewWarningSeverityStyle";
    public static readonly string SuccessSeverityStyleKey = "UacStyleDialogViewSuccessSeverityStyle";
    public static readonly string InformationalSeverityStyleKey = "UacStyleDialogViewInformationalSeverityStyle";

    private static void OnSeverityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        UacStyleDialogView self = (UacStyleDialogView) d;
        string styleKey = (InfoBarSeverity) e.NewValue switch
        {
            InfoBarSeverity.Informational => InformationalSeverityStyleKey,
            InfoBarSeverity.Success => SuccessSeverityStyleKey,
            InfoBarSeverity.Warning => WarningSeverityStyleKey,
            InfoBarSeverity.Error => ErrorSeverityStyleKey,
            _ => InformationalSeverityStyleKey
        };
        if (Application.Current.Resources.TryGetValue(styleKey, out object resource) && resource is Style style)
        {
            self.Style = style;
        }
    }
}
