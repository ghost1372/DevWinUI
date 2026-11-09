namespace DevWinUI;

public sealed partial class GrowlCard
{
    public Brush? IconBackground
    {
        get { return (Brush?)GetValue(IconBackgroundProperty); }
        set { SetValue(IconBackgroundProperty, value); }
    }

    public static readonly DependencyProperty IconBackgroundProperty =
        DependencyProperty.Register(nameof(IconBackground), typeof(Brush), typeof(GrowlCard), new PropertyMetadata(null, OnIconBrushChanged));

    public Brush? IconForeground
    {
        get { return (Brush?)GetValue(IconForegroundProperty); }
        set { SetValue(IconForegroundProperty, value); }
    }

    public static readonly DependencyProperty IconForegroundProperty =
        DependencyProperty.Register(nameof(IconForeground), typeof(Brush), typeof(GrowlCard), new PropertyMetadata(null, OnIconBrushChanged));

    public Brush? IconBorderBrush
    {
        get { return (Brush?)GetValue(IconBorderBrushProperty); }
        set { SetValue(IconBorderBrushProperty, value); }
    }

    public static readonly DependencyProperty IconBorderBrushProperty =
        DependencyProperty.Register(nameof(IconBorderBrush), typeof(Brush), typeof(GrowlCard), new PropertyMetadata(null, OnIconBrushChanged));

    public GrowlCardIconStyle IconStyle
    {
        get { return (GrowlCardIconStyle)GetValue(IconStyleProperty); }
        set { SetValue(IconStyleProperty, value); }
    }

    public static readonly DependencyProperty IconStyleProperty =
        DependencyProperty.Register(nameof(IconStyle), typeof(GrowlCardIconStyle), typeof(GrowlCard), new PropertyMetadata(GrowlCardIconStyle.Filled, OnIconBrushChanged));

    public bool ShowIconContainer
    {
        get { return (bool)GetValue(ShowIconContainerProperty); }
        set { SetValue(ShowIconContainerProperty, value); }
    }

    public static readonly DependencyProperty ShowIconContainerProperty =
        DependencyProperty.Register(nameof(ShowIconContainer), typeof(bool), typeof(GrowlCard), new PropertyMetadata(true, OnIconBrushChanged));

    public GrowlCardSentTimeVisibility SentTimeVisibility
    {
        get { return (GrowlCardSentTimeVisibility)GetValue(SentTimeVisibilityProperty); }
        set { SetValue(SentTimeVisibilityProperty, value); }
    }

    public static readonly DependencyProperty SentTimeVisibilityProperty =
        DependencyProperty.Register(nameof(SentTimeVisibility), typeof(GrowlCardSentTimeVisibility), typeof(GrowlCard), new PropertyMetadata(GrowlCardSentTimeVisibility.Hidden, OnVisualChanged));

    public DateTimeOffset? SentAt
    {
        get { return (DateTimeOffset?)GetValue(SentAtProperty); }
        set { SetValue(SentAtProperty, value); }
    }

    public static readonly DependencyProperty SentAtProperty =
        DependencyProperty.Register(nameof(SentAt), typeof(DateTimeOffset?), typeof(GrowlCard), new PropertyMetadata(null, OnVisualChanged));

    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(GrowlCard), new PropertyMetadata(string.Empty, OnVisualChanged));

    public string SubTitle
    {
        get { return (string)GetValue(SubTitleProperty); }
        set { SetValue(SubTitleProperty, value); }
    }

    public static readonly DependencyProperty SubTitleProperty =
        DependencyProperty.Register(nameof(SubTitle), typeof(string), typeof(GrowlCard), new PropertyMetadata(string.Empty, OnVisualChanged));

    public IconElement? Icon
    {
        get { return (IconElement?)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(IconElement), typeof(GrowlCard), new PropertyMetadata(null, OnVisualChanged));

    public object? ActionContent
    {
        get { return (object?)GetValue(ActionContentProperty); }
        set { SetValue(ActionContentProperty, value); }
    }

    public static readonly DependencyProperty ActionContentProperty =
        DependencyProperty.Register(nameof(ActionContent), typeof(object), typeof(GrowlCard), new PropertyMetadata(null, OnVisualChanged));

    public bool IsProgressIndeterminate
    {
        get { return (bool)GetValue(IsProgressIndeterminateProperty); }
        set { SetValue(IsProgressIndeterminateProperty, value); }
    }

    public static readonly DependencyProperty IsProgressIndeterminateProperty =
        DependencyProperty.Register(nameof(IsProgressIndeterminate), typeof(bool), typeof(GrowlCard), new PropertyMetadata(false, OnVisualChanged));

    public double Progress
    {
        get { return (double)GetValue(ProgressProperty); }
        set { SetValue(ProgressProperty, value); }
    }

    public static readonly DependencyProperty ProgressProperty =
        DependencyProperty.Register(nameof(Progress), typeof(double), typeof(GrowlCard), new PropertyMetadata(0d, OnProgressChanged));

    public double ProgressMaximum
    {
        get { return (double)GetValue(ProgressMaximumProperty); }
        set { SetValue(ProgressMaximumProperty, value); }
    }

    public static readonly DependencyProperty ProgressMaximumProperty =
        DependencyProperty.Register(nameof(ProgressMaximum), typeof(double), typeof(GrowlCard), new PropertyMetadata(100d, OnProgressChanged));

    public bool IsProgressError
    {
        get { return (bool)GetValue(IsProgressErrorProperty); }
        set { SetValue(IsProgressErrorProperty, value); }
    }

    public static readonly DependencyProperty IsProgressErrorProperty =
        DependencyProperty.Register(nameof(IsProgressError), typeof(bool), typeof(GrowlCard), new PropertyMetadata(false, OnProgressStatusChanged));

    public bool IsProgressPaused
    {
        get { return (bool)GetValue(IsProgressPausedProperty); }
        set { SetValue(IsProgressPausedProperty, value); }
    }

    public static readonly DependencyProperty IsProgressPausedProperty =
        DependencyProperty.Register(nameof(IsProgressPaused), typeof(bool), typeof(GrowlCard), new PropertyMetadata(false, OnProgressStatusChanged));

    public GrowlCardProgressType ProgressType
    {
        get { return (GrowlCardProgressType)GetValue(ProgressTypeProperty); }
        set { SetValue(ProgressTypeProperty, value); }
    }

    public static readonly DependencyProperty ProgressTypeProperty =
        DependencyProperty.Register(nameof(ProgressType), typeof(GrowlCardProgressType), typeof(GrowlCard), new PropertyMetadata(GrowlCardProgressType.Bar, OnVisualChanged));

    public bool IsOpen
    {
        get { return (bool)GetValue(IsOpenProperty); }
        set { SetValue(IsOpenProperty, value); }
    }

    public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(GrowlCard), new PropertyMetadata(true, OnIsOpenChanged));

    public GrowlCardAutoClose AutoClose
    {
        get { return (GrowlCardAutoClose)GetValue(AutoCloseProperty); }
        set { SetValue(AutoCloseProperty, value); }
    }

    public static readonly DependencyProperty AutoCloseProperty =
        DependencyProperty.Register(nameof(AutoClose), typeof(GrowlCardAutoClose), typeof(GrowlCard), new PropertyMetadata(GrowlCardAutoClose.None, OnAutoCloseChanged));

    public TimeSpan AutoCloseDuration
    {
        get { return (TimeSpan)GetValue(AutoCloseDurationProperty); }
        set { SetValue(AutoCloseDurationProperty, value); }
    }

    public static readonly DependencyProperty AutoCloseDurationProperty =
        DependencyProperty.Register(nameof(AutoCloseDuration), typeof(TimeSpan), typeof(GrowlCard), new PropertyMetadata(TimeSpan.FromSeconds(5), OnAutoCloseChanged));

    public bool ShowCountdown
    {
        get { return (bool)GetValue(ShowCountdownProperty); }
        set { SetValue(ShowCountdownProperty, value); }
    }

    public static readonly DependencyProperty ShowCountdownProperty =
        DependencyProperty.Register(nameof(ShowCountdown), typeof(bool), typeof(GrowlCard), new PropertyMetadata(true, OnVisualChanged));

    public bool ShowCountdownText
    {
        get { return (bool)GetValue(ShowCountdownTextProperty); }
        set { SetValue(ShowCountdownTextProperty, value); }
    }

    public static readonly DependencyProperty ShowCountdownTextProperty =
        DependencyProperty.Register(nameof(ShowCountdownText), typeof(bool), typeof(GrowlCard), new PropertyMetadata(true, OnVisualChanged));

    public bool PauseAutoCloseOnPointerOver
    {
        get { return (bool)GetValue(PauseAutoCloseOnPointerOverProperty); }
        set { SetValue(PauseAutoCloseOnPointerOverProperty, value); }
    }

    public static readonly DependencyProperty PauseAutoCloseOnPointerOverProperty =
        DependencyProperty.Register(nameof(PauseAutoCloseOnPointerOver), typeof(bool), typeof(GrowlCard), new PropertyMetadata(true));

    public bool ShowIcon
    {
        get { return (bool)GetValue(ShowIconProperty); }
        set { SetValue(ShowIconProperty, value); }
    }

    public static readonly DependencyProperty ShowIconProperty =
        DependencyProperty.Register(nameof(ShowIcon), typeof(bool), typeof(GrowlCard), new PropertyMetadata(true, OnVisualChanged));

    public bool ShowTitle
    {
        get { return (bool)GetValue(ShowTitleProperty); }
        set { SetValue(ShowTitleProperty, value); }
    }

    public static readonly DependencyProperty ShowTitleProperty =
        DependencyProperty.Register(nameof(ShowTitle), typeof(bool), typeof(GrowlCard), new PropertyMetadata(true, OnVisualChanged));

    public bool ShowSubTitle
    {
        get { return (bool)GetValue(ShowSubTitleProperty); }
        set { SetValue(ShowSubTitleProperty, value); }
    }

    public static readonly DependencyProperty ShowSubTitleProperty =
        DependencyProperty.Register(nameof(ShowSubTitle), typeof(bool), typeof(GrowlCard), new PropertyMetadata(true, OnVisualChanged));

    public bool ShowContent
    {
        get { return (bool)GetValue(ShowContentProperty); }
        set { SetValue(ShowContentProperty, value); }
    }

    public static readonly DependencyProperty ShowContentProperty =
        DependencyProperty.Register(nameof(ShowContent), typeof(bool), typeof(GrowlCard), new PropertyMetadata(true, OnVisualChanged));

    public bool ShowCloseButton
    {
        get { return (bool)GetValue(ShowCloseButtonProperty); }
        set { SetValue(ShowCloseButtonProperty, value); }
    }

    public static readonly DependencyProperty ShowCloseButtonProperty =
        DependencyProperty.Register(nameof(ShowCloseButton), typeof(bool), typeof(GrowlCard), new PropertyMetadata(true, OnVisualChanged));

    public bool ShowActionContent
    {
        get { return (bool)GetValue(ShowActionContentProperty); }
        set { SetValue(ShowActionContentProperty, value); }
    }

    public static readonly DependencyProperty ShowActionContentProperty =
        DependencyProperty.Register(nameof(ShowActionContent), typeof(bool), typeof(GrowlCard), new PropertyMetadata(true, OnVisualChanged));

    public bool ShowProgress
    {
        get { return (bool)GetValue(ShowProgressProperty); }
        set { SetValue(ShowProgressProperty, value); }
    }

    public static readonly DependencyProperty ShowProgressProperty =
        DependencyProperty.Register(nameof(ShowProgress), typeof(bool), typeof(GrowlCard), new PropertyMetadata(false, OnVisualChanged));

    public bool ShowProgressText
    {
        get { return (bool)GetValue(ShowProgressTextProperty); }
        set { SetValue(ShowProgressTextProperty, value); }
    }

    public static readonly DependencyProperty ShowProgressTextProperty =
        DependencyProperty.Register(nameof(ShowProgressText), typeof(bool), typeof(GrowlCard), new PropertyMetadata(true, OnVisualChanged));

    public bool ShowIndicator
    {
        get { return (bool)GetValue(ShowIndicatorProperty); }
        set { SetValue(ShowIndicatorProperty, value); }
    }

    public static readonly DependencyProperty ShowIndicatorProperty =
        DependencyProperty.Register(nameof(ShowIndicator), typeof(bool), typeof(GrowlCard), new PropertyMetadata(true, OnIndicatorChanged));

    public bool ShowGradientBackground
    {
        get { return (bool)GetValue(ShowGradientBackgroundProperty); }
        set { SetValue(ShowGradientBackgroundProperty, value); }
    }

    public static readonly DependencyProperty ShowGradientBackgroundProperty =
        DependencyProperty.Register(nameof(ShowGradientBackground), typeof(bool), typeof(GrowlCard), new PropertyMetadata(false, OnGradientChanged));

    public GrowlCardGradientIntensity GradientIntensity
    {
        get { return (GrowlCardGradientIntensity)GetValue(GradientIntensityProperty); }
        set { SetValue(GradientIntensityProperty, value); }
    }

    public static readonly DependencyProperty GradientIntensityProperty =
        DependencyProperty.Register(nameof(GradientIntensity), typeof(GrowlCardGradientIntensity), typeof(GrowlCard), new PropertyMetadata(GrowlCardGradientIntensity.Normal, OnGradientChanged));

    public GrowlCardIndicatorPlacement IndicatorPlacement
    {
        get { return (GrowlCardIndicatorPlacement)GetValue(IndicatorPlacementProperty); }
        set { SetValue(IndicatorPlacementProperty, value); }
    }

    public static readonly DependencyProperty IndicatorPlacementProperty =
        DependencyProperty.Register(nameof(IndicatorPlacement), typeof(GrowlCardIndicatorPlacement), typeof(GrowlCard), new PropertyMetadata(GrowlCardIndicatorPlacement.Top, OnIndicatorChanged));

    public double IndicatorThickness
    {
        get { return (double)GetValue(IndicatorThicknessProperty); }
        set { SetValue(IndicatorThicknessProperty, value); }
    }

    public static readonly DependencyProperty IndicatorThicknessProperty =
        DependencyProperty.Register(nameof(IndicatorThickness), typeof(double), typeof(GrowlCard), new PropertyMetadata(3d));

    public GrowlCardSeverity Severity
    {
        get { return (GrowlCardSeverity)GetValue(SeverityProperty); }
        set { SetValue(SeverityProperty, value); }
    }

    public static readonly DependencyProperty SeverityProperty =
        DependencyProperty.Register(nameof(Severity), typeof(GrowlCardSeverity), typeof(GrowlCard), new PropertyMetadata(GrowlCardSeverity.Attention, OnVisualChanged));

    public GrowlCardCountdownSeverity CountdownSeverity
    {
        get { return (GrowlCardCountdownSeverity)GetValue(CountdownSeverityProperty); }
        set { SetValue(CountdownSeverityProperty, value); }
    }

    public static readonly DependencyProperty CountdownSeverityProperty =
        DependencyProperty.Register(nameof(CountdownSeverity), typeof(GrowlCardCountdownSeverity), typeof(GrowlCard), new PropertyMetadata(GrowlCardCountdownSeverity.Default, OnCountdownSeverityChanged));

    public GrowlCardCountdownPlacement CountdownPlacement
    {
        get { return (GrowlCardCountdownPlacement)GetValue(CountdownPlacementProperty); }
        set { SetValue(CountdownPlacementProperty, value); }
    }

    public static readonly DependencyProperty CountdownPlacementProperty =
        DependencyProperty.Register(nameof(CountdownPlacement), typeof(GrowlCardCountdownPlacement), typeof(GrowlCard), new PropertyMetadata(GrowlCardCountdownPlacement.Default, OnIndicatorChanged));

    public GrowlCardDisplayMode DisplayMode
    {
        get { return (GrowlCardDisplayMode)GetValue(DisplayModeProperty); }
        set { SetValue(DisplayModeProperty, value); }
    }

    public static readonly DependencyProperty DisplayModeProperty =
        DependencyProperty.Register(nameof(DisplayMode), typeof(GrowlCardDisplayMode), typeof(GrowlCard), new PropertyMetadata(GrowlCardDisplayMode.Card, OnDisplayModeChanged));

    public GrowlCardColorStyle ColorStyle
    {
        get { return (GrowlCardColorStyle)GetValue(ColorStyleProperty); }
        set { SetValue(ColorStyleProperty, value); }
    }

    public static readonly DependencyProperty ColorStyleProperty =
        DependencyProperty.Register(nameof(ColorStyle), typeof(GrowlCardColorStyle), typeof(GrowlCard), new PropertyMetadata(GrowlCardColorStyle.Solid, OnColorStyleChanged));

    public GrowlCardCountdownColorStyle CountdownColorStyle
    {
        get { return (GrowlCardCountdownColorStyle)GetValue(CountdownColorStyleProperty); }
        set { SetValue(CountdownColorStyleProperty, value); }
    }

    public static readonly DependencyProperty CountdownColorStyleProperty =
        DependencyProperty.Register(nameof(CountdownColorStyle), typeof(GrowlCardCountdownColorStyle), typeof(GrowlCard), new PropertyMetadata(GrowlCardCountdownColorStyle.Default, OnCountdownColorStyleChanged));

    private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GrowlCard)d).ApplyIsOpen((bool)e.NewValue);
    }

    private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GrowlCard)d).RequestApply();
    }

    private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GrowlCard)d).UpdateProgressText();
    }

    private static void OnProgressStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GrowlCard)d).UpdateRingStatus();
    }

    private static void OnGradientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GrowlCard)d).UpdateGradient();
    }

    private static void OnAutoCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var card = (GrowlCard)d;
        card.RequestApply();
        card.StartAutoClose();
    }

    private static void OnIndicatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GrowlCard)d).UpdateIndicator();
    }

    private static void OnIconBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GrowlCard)d).UpdateIconStates();
    }

    private static void OnCountdownSeverityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var card = (GrowlCard)d;

        card.UpdateCountdownBrush();
        card.UpdateCountdownFill();
    }

    private static void OnColorStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var card = (GrowlCard)d;

        card.UpdateIconStates();
        card.UpdateIndicatorFill();
        card.UpdateCountdownFill();
    }

    private static void OnCountdownColorStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GrowlCard)d).UpdateCountdownFill();
    }

    private static void OnDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GrowlCard)d).UpdateDisplayMode();
    }

}
