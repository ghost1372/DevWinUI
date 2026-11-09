namespace DevWinUI;

public sealed partial class HoldToConfirm
{
    public event TypedEventHandler<HoldToConfirm, object?>? Confirm;

    public string Label
    {
        get { return (string)GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(HoldToConfirm), new PropertyMetadata("Hold to confirm", OnTextChanged));

    public string ConfirmedLabel
    {
        get { return (string)GetValue(ConfirmedLabelProperty); }
        set { SetValue(ConfirmedLabelProperty, value); }
    }

    public static readonly DependencyProperty ConfirmedLabelProperty =
        DependencyProperty.Register(nameof(ConfirmedLabel), typeof(string), typeof(HoldToConfirm), new PropertyMetadata("Confirmed", OnTextChanged));

    public HoldToConfirmKind Kind
    {
        get { return (HoldToConfirmKind)GetValue(KindProperty); }
        set { SetValue(KindProperty, value); }
    }

    public static readonly DependencyProperty KindProperty =
        DependencyProperty.Register(nameof(Kind), typeof(HoldToConfirmKind), typeof(HoldToConfirm), new PropertyMetadata(HoldToConfirmKind.Default, OnVisualStateChanged));

    public HoldToConfirmSize Size
    {
        get { return (HoldToConfirmSize)GetValue(SizeProperty); }
        set { SetValue(SizeProperty, value); }
    }

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(nameof(Size), typeof(HoldToConfirmSize), typeof(HoldToConfirm), new PropertyMetadata(HoldToConfirmSize.Default, OnVisualStateChanged));

    public double Duration
    {
        get { return (double)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(double), typeof(HoldToConfirm), new PropertyMetadata(1200.0, OnDurationChanged));

    public bool? Confirmed
    {
        get { return (bool?)GetValue(ConfirmedProperty); }
        set { SetValue(ConfirmedProperty, value); }
    }

    public static readonly DependencyProperty ConfirmedProperty =
        DependencyProperty.Register(nameof(Confirmed), typeof(bool?), typeof(HoldToConfirm), new PropertyMetadata(null, OnConfirmedChanged));

    public string IconGlyph
    {
        get { return (string)GetValue(IconGlyphProperty); }
        set { SetValue(IconGlyphProperty, value); }
    }

    public static readonly DependencyProperty IconGlyphProperty =
        DependencyProperty.Register(nameof(IconGlyph), typeof(string), typeof(HoldToConfirm), new PropertyMetadata(string.Empty, OnIconChanged));

    public bool IsAnimationEnabled
    {
        get { return (bool)GetValue(IsAnimationEnabledProperty); }
        set { SetValue(IsAnimationEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsAnimationEnabledProperty =
        DependencyProperty.Register(nameof(IsAnimationEnabled), typeof(bool), typeof(HoldToConfirm), new PropertyMetadata(true));

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((HoldToConfirm)d).UpdateText();

    private static void OnVisualStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((HoldToConfirm)d).UpdateVisualStates();

    private static void OnDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((HoldToConfirm)d).UpdateAutomation();

    private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((HoldToConfirm)d).UpdateIcon();

    private static void OnConfirmedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var self = (HoldToConfirm)d;
        if (e.NewValue is bool done) self.ApplyDone(done, self.IsLoaded);
    }
}
