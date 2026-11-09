namespace DevWinUI;

public sealed partial class BankCard
{
    public string BankName
    {
        get { return (string)GetValue(BankNameProperty); }
        set { SetValue(BankNameProperty, value); }
    }

    public static readonly DependencyProperty BankNameProperty =
        DependencyProperty.Register(nameof(BankName), typeof(string), typeof(BankCard), new PropertyMetadata(string.Empty, OnVisualPropertyChanged));

    public string CardNumber
    {
        get { return (string)GetValue(CardNumberProperty); }
        set { SetValue(CardNumberProperty, value); }
    }

    public static readonly DependencyProperty CardNumberProperty =
        DependencyProperty.Register(nameof(CardNumber), typeof(string), typeof(BankCard), new PropertyMetadata(string.Empty, OnVisualPropertyChanged));

    public string CardHolder
    {
        get { return (string)GetValue(CardHolderProperty); }
        set { SetValue(CardHolderProperty, value); }
    }

    public static readonly DependencyProperty CardHolderProperty =
        DependencyProperty.Register(nameof(CardHolder), typeof(string), typeof(BankCard), new PropertyMetadata(string.Empty, OnVisualPropertyChanged));

    public string Expiration
    {
        get { return (string)GetValue(ExpirationProperty); }
        set { SetValue(ExpirationProperty, value); }
    }

    public static readonly DependencyProperty ExpirationProperty =
        DependencyProperty.Register(nameof(Expiration), typeof(string), typeof(BankCard), new PropertyMetadata(string.Empty, OnVisualPropertyChanged));

    public string Cvv2
    {
        get { return (string)GetValue(Cvv2Property); }
        set { SetValue(Cvv2Property, value); }
    }

    public static readonly DependencyProperty Cvv2Property =
        DependencyProperty.Register(nameof(Cvv2), typeof(string), typeof(BankCard), new PropertyMetadata(string.Empty, OnVisualPropertyChanged));

    public bool MaskNumber
    {
        get { return (bool)GetValue(MaskNumberProperty); }
        set { SetValue(MaskNumberProperty, value); }
    }

    public static readonly DependencyProperty MaskNumberProperty =
        DependencyProperty.Register(nameof(MaskNumber), typeof(bool), typeof(BankCard), new PropertyMetadata(false, OnVisualPropertyChanged));

    public bool ShowCvv
    {
        get { return (bool)GetValue(ShowCvvProperty); }
        set { SetValue(ShowCvvProperty, value); }
    }

    public static readonly DependencyProperty ShowCvvProperty =
        DependencyProperty.Register(nameof(ShowCvv), typeof(bool), typeof(BankCard), new PropertyMetadata(false, OnVisualPropertyChanged));

    public bool ShowChip
    {
        get { return (bool)GetValue(ShowChipProperty); }
        set { SetValue(ShowChipProperty, value); }
    }

    public static readonly DependencyProperty ShowChipProperty =
        DependencyProperty.Register(nameof(ShowChip), typeof(bool), typeof(BankCard), new PropertyMetadata(true, OnVisualPropertyChanged));

    public ImageSource? CardLogo
    {
        get { return (ImageSource?)GetValue(CardLogoProperty); }
        set { SetValue(CardLogoProperty, value); }
    }

    public static readonly DependencyProperty CardLogoProperty =
        DependencyProperty.Register(nameof(CardLogo), typeof(ImageSource), typeof(BankCard), new PropertyMetadata(null, OnVisualPropertyChanged));

    public ImageSource? BackgroundImage
    {
        get { return (ImageSource?)GetValue(BackgroundImageProperty); }
        set { SetValue(BackgroundImageProperty, value); }
    }

    public static readonly DependencyProperty BackgroundImageProperty =
        DependencyProperty.Register(nameof(BackgroundImage), typeof(ImageSource), typeof(BankCard), new PropertyMetadata(null, OnVisualPropertyChanged));

    public Brush? ChipBackground
    {
        get { return (Brush?)GetValue(ChipBackgroundProperty); }
        set { SetValue(ChipBackgroundProperty, value); }
    }

    public static readonly DependencyProperty ChipBackgroundProperty =
        DependencyProperty.Register(nameof(ChipBackground), typeof(Brush), typeof(BankCard), new PropertyMetadata(null));

    public Brush? LabelForeground
    {
        get { return (Brush?)GetValue(LabelForegroundProperty); }
        set { SetValue(LabelForegroundProperty, value); }
    }

    public static readonly DependencyProperty LabelForegroundProperty =
        DependencyProperty.Register(nameof(LabelForeground), typeof(Brush), typeof(BankCard), new PropertyMetadata(null));

    public Brush? GlareBrush
    {
        get { return (Brush?)GetValue(GlareBrushProperty); }
        set { SetValue(GlareBrushProperty, value); }
    }

    public static readonly DependencyProperty GlareBrushProperty =
        DependencyProperty.Register(nameof(GlareBrush), typeof(Brush), typeof(BankCard),
            new PropertyMetadata(null, (d, e) => ((BankCard)d).ApplyGlareBrush()));

    private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((BankCard)d).UpdateVisuals();
    }
}
