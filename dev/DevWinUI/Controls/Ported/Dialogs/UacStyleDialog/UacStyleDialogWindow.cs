// https://github.com/SuGar0218/SuGarToolkit.WinUI3

using System.Windows.Input;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_UacStyleDialogView), Type = typeof(UacStyleDialogView))]
public partial class UacStyleDialogWindow : DialogWindowBase, IExtendedContentDialog
{
    public UacStyleDialogWindow()
    {
        DefaultStyleKey = typeof(UacStyleDialogWindow);
        Loaded += OnLoaded;
    }

    #region DependencyProperty

    public InfoBarSeverity Severity
    {
        get => (InfoBarSeverity) GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }

    public static readonly DependencyProperty SeverityProperty = DependencyProperty.Register(
        nameof(Severity),
        typeof(InfoBarSeverity),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(InfoBarSeverity), OnSeverityChanged)
    );

    public object? ExtendedHeader
    {
        get => (object?) GetValue(ExtendedHeaderProperty);
        set => SetValue(ExtendedHeaderProperty, value);
    }

    public static readonly DependencyProperty ExtendedHeaderProperty = DependencyProperty.Register(
        nameof(ExtendedHeader),
        typeof(object),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(object))
    );

    public DataTemplate? ExtendedHeaderTemplate
    {
        get => (DataTemplate?) GetValue(ExtendedHeaderTemplateProperty);
        set => SetValue(ExtendedHeaderTemplateProperty, value);
    }

    public static readonly DependencyProperty ExtendedHeaderTemplateProperty = DependencyProperty.Register(
        nameof(ExtendedHeaderTemplate),
        typeof(DataTemplate),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(DataTemplate))
    );

    public Brush? ExtendedHeaderBackground
    {
        get => (Brush?) GetValue(ExtendedHeaderBackgroundProperty);
        set => SetValue(ExtendedHeaderBackgroundProperty, value);
    }

    public static readonly DependencyProperty ExtendedHeaderBackgroundProperty = DependencyProperty.Register(
        nameof(ExtendedHeaderBackground),
        typeof(Brush),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(Brush))
    );

    public object? Header
    {
        get => (object?) GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(object),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(object))
    );

    public DataTemplate? HeaderTemplate
    {
        get => (DataTemplate?) GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
        nameof(HeaderTemplate),
        typeof(DataTemplate),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(DataTemplate))
    );

    public object? PrimaryButtonContent
    {
        get => (object?) GetValue(PrimaryButtonContentProperty);
        set => SetValue(PrimaryButtonContentProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonContentProperty = DependencyProperty.Register(
        nameof(PrimaryButtonContent),
        typeof(object),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(object))
    );

    public DataTemplate? PrimaryButtonTemplate
    {
        get => (DataTemplate?) GetValue(PrimaryButtonTemplateProperty);
        set => SetValue(PrimaryButtonTemplateProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonTemplateProperty = DependencyProperty.Register(
        nameof(PrimaryButtonTemplate),
        typeof(DataTemplate),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(DataTemplate))
    );

    public object? SecondaryButtonContent
    {
        get => (object?) GetValue(SecondaryButtonContentProperty);
        set => SetValue(SecondaryButtonContentProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonContentProperty = DependencyProperty.Register(
        nameof(SecondaryButtonContent),
        typeof(object),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(object))
    );

    public DataTemplate? SecondaryButtonTemplate
    {
        get => (DataTemplate?) GetValue(SecondaryButtonTemplateProperty);
        set => SetValue(SecondaryButtonTemplateProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonTemplateProperty = DependencyProperty.Register(
        nameof(SecondaryButtonTemplate),
        typeof(DataTemplate),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(DataTemplate))
    );

    public object? CloseButtonContent
    {
        get => (object?) GetValue(CloseButtonContentProperty);
        set => SetValue(CloseButtonContentProperty, value);
    }

    public static readonly DependencyProperty CloseButtonContentProperty = DependencyProperty.Register(
        nameof(CloseButtonContent),
        typeof(object),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(object))
    );

    public DataTemplate? CloseButtonTemplate
    {
        get => (DataTemplate?) GetValue(CloseButtonTemplateProperty);
        set => SetValue(CloseButtonTemplateProperty, value);
    }

    public static readonly DependencyProperty CloseButtonTemplateProperty = DependencyProperty.Register(
        nameof(CloseButtonTemplate),
        typeof(DataTemplate),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(DataTemplate))
    );

    public ICommand? PrimaryButtonCommand
    {
        get => (ICommand?) GetValue(PrimaryButtonCommandProperty);
        set => SetValue(PrimaryButtonCommandProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonCommandProperty = DependencyProperty.Register(
        nameof(PrimaryButtonCommand),
        typeof(ICommand),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(ICommand))
    );

    public ICommand? SecondaryButtonCommand
    {
        get => (ICommand?) GetValue(SecondaryButtonCommandProperty);
        set => SetValue(SecondaryButtonCommandProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonCommandProperty = DependencyProperty.Register(
        nameof(SecondaryButtonCommand),
        typeof(ICommand),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(ICommand))
    );

    public ICommand? CloseButtonCommand
    {
        get => (ICommand?) GetValue(CloseButtonCommandProperty);
        set => SetValue(CloseButtonCommandProperty, value);
    }

    public static readonly DependencyProperty CloseButtonCommandProperty = DependencyProperty.Register(
        nameof(CloseButtonCommand),
        typeof(ICommand),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(ICommand))
    );

    public bool IsPrimaryButtonEnabled
    {
        get => (bool) GetValue(IsPrimaryButtonEnabledProperty);
        set => SetValue(IsPrimaryButtonEnabledProperty, value);
    }

    public static readonly DependencyProperty IsPrimaryButtonEnabledProperty = DependencyProperty.Register(
        nameof(IsPrimaryButtonEnabled),
        typeof(bool),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(true)
    );

    public bool IsSecondaryButtonEnabled
    {
        get => (bool) GetValue(IsSecondaryButtonEnabledProperty);
        set => SetValue(IsSecondaryButtonEnabledProperty, value);
    }

    public static readonly DependencyProperty IsSecondaryButtonEnabledProperty = DependencyProperty.Register(
        nameof(IsSecondaryButtonEnabled),
        typeof(bool),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(true)
    );

    public Style? PrimaryButtonStyle
    {
        get => (Style) GetValue(PrimaryButtonStyleProperty);
        set => SetValue(PrimaryButtonStyleProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonStyleProperty = DependencyProperty.Register(
        nameof(PrimaryButtonStyle),
        typeof(Style),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(Style))
    );

    public Style? SecondaryButtonStyle
    {
        get => (Style) GetValue(SecondaryButtonStyleProperty);
        set => SetValue(SecondaryButtonStyleProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonStyleProperty = DependencyProperty.Register(
        nameof(SecondaryButtonStyle),
        typeof(Style),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(Style))
    );

    public Style? CloseButtonStyle
    {
        get => (Style) GetValue(CloseButtonStyleProperty);
        set => SetValue(CloseButtonStyleProperty, value);
    }

    public static readonly DependencyProperty CloseButtonStyleProperty = DependencyProperty.Register(
        nameof(CloseButtonStyle),
        typeof(Style),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(Style))
    );

    public string? PrimaryButtonAccessKey
    {
        get => (string?) GetValue(PrimaryButtonAccessKeyProperty);
        set => SetValue(PrimaryButtonAccessKeyProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonAccessKeyProperty = DependencyProperty.Register(
        nameof(PrimaryButtonAccessKey),
        typeof(string),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(string))
    );

    public string? SecondaryButtonAccessKey
    {
        get => (string?) GetValue(SecondaryButtonAccessKeyProperty);
        set => SetValue(SecondaryButtonAccessKeyProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonAccessKeyProperty = DependencyProperty.Register(
        nameof(SecondaryButtonAccessKey),
        typeof(string),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(string))
    );

    public string? CloseButtonAccessKey
    {
        get => (string?) GetValue(CloseButtonAccessKeyProperty);
        set => SetValue(CloseButtonAccessKeyProperty, value);
    }

    public static readonly DependencyProperty CloseButtonAccessKeyProperty = DependencyProperty.Register(
        nameof(CloseButtonAccessKey),
        typeof(string),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(string))
    );

    public ContentDialogButton DefaultButton
    {
        get => (ContentDialogButton) GetValue(DefaultButtonProperty);
        set => SetValue(DefaultButtonProperty, value);
    }

    public static readonly DependencyProperty DefaultButtonProperty = DependencyProperty.Register(
        nameof(DefaultButton),
        typeof(ContentDialogButton),
        typeof(UacStyleDialogWindow),
        new PropertyMetadata(default(ContentDialogButton))
    );

    #endregion

    public ContentDialogResult Result => PART_UacStyleDialogView?.Result ?? ContentDialogResult.None;

    public event EventHandler? PrimaryButtonClick;
    public event EventHandler? SecondaryButtonClick;
    public event EventHandler? CloseButtonClick;

    private UacStyleDialogView? PART_UacStyleDialogView;

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        PART_UacStyleDialogView = (UacStyleDialogView) GetTemplateChild(nameof(PART_UacStyleDialogView));
        PART_UacStyleDialogView.PrimaryButtonClick += (sender, args) => PrimaryButtonClick?.Invoke(this, EventArgs.Empty);
        PART_UacStyleDialogView.SecondaryButtonClick += (sender, args) => SecondaryButtonClick?.Invoke(this, EventArgs.Empty);
        PART_UacStyleDialogView.CloseButtonClick += (sender, args) => CloseButtonClick?.Invoke(this, EventArgs.Empty);
    }

    public static readonly string ErrorSeverityStyleKey = "UacStyleDialogWindowErrorSeverityStyle";
    public static readonly string WarningSeverityStyleKey = "UacStyleDialogWindowWarningSeverityStyle";
    public static readonly string SuccessSeverityStyleKey = "UacStyleDialogWindowSuccessSeverityStyle";
    public static readonly string InformationalSeverityStyleKey = "UacStyleDialogWindowInformationalSeverityStyle";

    private static void OnSeverityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        UacStyleDialogWindow self = (UacStyleDialogWindow) d;
        InfoBarSeverity severity = (InfoBarSeverity) e.NewValue;
        self.PART_UacStyleDialogView?.Severity = severity;
        string styleKey = severity switch
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

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        //PART_UacStyleDialogView.MaxWidth = double.PositiveInfinity;
    }
}
