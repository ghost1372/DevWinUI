// https://github.com/SuGar0218/SuGarToolkit.WinUI3

using System.Windows.Input;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_ExtendedHeaderArea), Type = typeof(Border))]
[TemplatePart(Name = nameof(PART_ExtendedHeader), Type = typeof(ContentControl))]
[TemplatePart(Name = nameof(PART_ContentDialogView), Type = typeof(ExtendedContentDialogView))]
[TemplateVisualState(GroupName = "ExtendedHeaderVisibilityStates", Name = "ExtendedHeaderVisibile")]
[TemplateVisualState(GroupName = "ExtendedHeaderVisibilityStates", Name = "ExtendedHeaderCollapsed")]
public partial class ExtendedContentDialogView : ContentControl, IExtendedContentDialog
{
    public ExtendedContentDialogView()
    {
        DefaultStyleKey = typeof(ExtendedContentDialogView);
        Loaded += OnLoaded;
    }

    #region DependencyProperty

    public object? ExtendedHeader
    {
        get => (object?) GetValue(ExtendedHeaderProperty);
        set => SetValue(ExtendedHeaderProperty, value);
    }

    public static readonly DependencyProperty ExtendedHeaderProperty = DependencyProperty.Register(
        nameof(ExtendedHeader),
        typeof(object),
        typeof(ExtendedContentDialogView),
        new PropertyMetadata(default(object), OnExtendedHeaderChanged)
    );

    private static void OnExtendedHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExtendedContentDialogView self = (ExtendedContentDialogView) d;
        self.DetermineExtendedHeaderVisibilityState();
    }

    public DataTemplate? ExtendedHeaderTemplate
    {
        get => (DataTemplate?) GetValue(ExtendedHeaderTemplateProperty);
        set => SetValue(ExtendedHeaderTemplateProperty, value);
    }

    public static readonly DependencyProperty ExtendedHeaderTemplateProperty = DependencyProperty.Register(
        nameof(ExtendedHeaderTemplate),
        typeof(DataTemplate),
        typeof(ExtendedContentDialogView),
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
        typeof(ExtendedContentDialogView),
        new PropertyMetadata(default(Brush))
    );

    #region DependencyProperty for inner ContentDialogView

    public object? Header
    {
        get => (object?) GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(object),
        typeof(ExtendedContentDialogView),
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
        typeof(ExtendedContentDialogView),
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
        typeof(ExtendedContentDialogView),
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
        typeof(ExtendedContentDialogView),
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
        typeof(ExtendedContentDialogView),
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
        typeof(ExtendedContentDialogView),
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
        typeof(ExtendedContentDialogView),
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
        typeof(ExtendedContentDialogView),
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
        typeof(ExtendedContentDialogView),
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
        typeof(ExtendedContentDialogView),
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
        typeof(ExtendedContentDialogView),
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
        typeof(ExtendedContentDialogView),
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
        typeof(ExtendedContentDialogView),
        new PropertyMetadata(true)
    );

    public Orientation ButtonOrientation
    {
        get => (Orientation) GetValue(ButtonOrientationProperty);
        set => SetValue(ButtonOrientationProperty, value);
    }

    public static readonly DependencyProperty ButtonOrientationProperty = DependencyProperty.Register(
        nameof(ButtonOrientation),
        typeof(Orientation),
        typeof(ExtendedContentDialogView),
        new PropertyMetadata(Orientation.Horizontal)
    );

    public Style? PrimaryButtonStyle
    {
        get => (Style) GetValue(PrimaryButtonStyleProperty);
        set => SetValue(PrimaryButtonStyleProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonStyleProperty = DependencyProperty.Register(
        nameof(PrimaryButtonStyle),
        typeof(Style),
        typeof(ExtendedContentDialogView),
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
        typeof(ExtendedContentDialogView),
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
        typeof(ExtendedContentDialogView),
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
        typeof(ExtendedContentDialogView),
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
        typeof(ExtendedContentDialogView),
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
        typeof(ExtendedContentDialogView),
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
        typeof(ExtendedContentDialogView),
        new PropertyMetadata(default(ContentDialogButton))
    );

    #endregion

    #endregion

    public event EventHandler? PrimaryButtonClick;
    public event EventHandler? SecondaryButtonClick;
    public event EventHandler? CloseButtonClick;

    public ContentDialogResult Result => PART_ContentDialogView?.Result ?? ContentDialogResult.None;

    public UIElement? ExtendedHeaderArea => PART_ExtendedHeaderArea;

    private Border? PART_ExtendedHeaderArea;
    private ContentControl? PART_ExtendedHeader;
    private ContentDialogView? PART_ContentDialogView;

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        PART_ExtendedHeaderArea = (Border) GetTemplateChild(nameof(PART_ExtendedHeaderArea));
        PART_ExtendedHeader = (ContentControl) GetTemplateChild(nameof(PART_ExtendedHeader));
        PART_ContentDialogView = (ContentDialogView) GetTemplateChild(nameof(PART_ContentDialogView));

        PART_ContentDialogView.PrimaryButtonClick += (sender, args) => PrimaryButtonClick?.Invoke(this, EventArgs.Empty);
        PART_ContentDialogView.SecondaryButtonClick += (sender, args) => SecondaryButtonClick?.Invoke(this, EventArgs.Empty);
        PART_ContentDialogView.CloseButtonClick += (sender, args) => CloseButtonClick?.Invoke(this, EventArgs.Empty);
        PART_ContentDialogView.MaxWidth = double.PositiveInfinity;
    }

    private void DetermineExtendedHeaderVisibilityState()
    {
        if (ExtendedHeader is null)
        {
            VisualStateManager.GoToState(this, "ExtendedHeaderCollapsed", false);
        }
        else
        {
            VisualStateManager.GoToState(this, "ExtendedHeaderVisibile", false);
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        DetermineExtendedHeaderVisibilityState();
    }
}
