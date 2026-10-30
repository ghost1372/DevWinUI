namespace DevWinUI;

public sealed partial class ScrambledText
{
    private const string DefaultScrambleChars = ".:";

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(ScrambledText), new PropertyMetadata(string.Empty, OnTextChanged));

    public ScrambledTextEffect Effect
    {
        get { return (ScrambledTextEffect)GetValue(EffectProperty); }
        set { SetValue(EffectProperty, value); }
    }

    public static readonly DependencyProperty EffectProperty =
        DependencyProperty.Register(nameof(Effect), typeof(ScrambledTextEffect), typeof(ScrambledText), new PropertyMetadata(ScrambledTextEffect.Scramble, OnEffectChanged));

    public double Radius
    {
        get { return (double)GetValue(RadiusProperty); }
        set { SetValue(RadiusProperty, value); }
    }

    public static readonly DependencyProperty RadiusProperty =
        DependencyProperty.Register(nameof(Radius), typeof(double), typeof(ScrambledText), new PropertyMetadata(100.0));

    public double Strength
    {
        get { return (double)GetValue(StrengthProperty); }
        set { SetValue(StrengthProperty, value); }
    }

    public static readonly DependencyProperty StrengthProperty =
        DependencyProperty.Register(nameof(Strength), typeof(double), typeof(ScrambledText), new PropertyMetadata(1.0));

    public double Trail
    {
        get { return (double)GetValue(TrailProperty); }
        set { SetValue(TrailProperty, value); }
    }

    public static readonly DependencyProperty TrailProperty =
        DependencyProperty.Register(nameof(Trail), typeof(double), typeof(ScrambledText), new PropertyMetadata(0.6));

    public double Duration
    {
        get { return (double)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(double), typeof(ScrambledText), new PropertyMetadata(1.2));

    public double Speed
    {
        get { return (double)GetValue(SpeedProperty); }
        set { SetValue(SpeedProperty, value); }
    }

    public static readonly DependencyProperty SpeedProperty =
        DependencyProperty.Register(nameof(Speed), typeof(double), typeof(ScrambledText), new PropertyMetadata(0.5));

    public bool ClickBurst
    {
        get { return (bool)GetValue(ClickBurstProperty); }
        set { SetValue(ClickBurstProperty, value); }
    }

    public static readonly DependencyProperty ClickBurstProperty =
        DependencyProperty.Register(nameof(ClickBurst), typeof(bool), typeof(ScrambledText), new PropertyMetadata(true));

    public string ScrambleChars
    {
        get { return (string)GetValue(ScrambleCharsProperty); }
        set { SetValue(ScrambleCharsProperty, value); }
    }

    public static readonly DependencyProperty ScrambleCharsProperty =
        DependencyProperty.Register(nameof(ScrambleChars), typeof(string), typeof(ScrambledText), new PropertyMetadata(DefaultScrambleChars, OnScrambleCharsChanged));

    public Brush? ScrambleColor
    {
        get { return (Brush?)GetValue(ScrambleColorProperty); }
        set { SetValue(ScrambleColorProperty, value); }
    }

    public static readonly DependencyProperty ScrambleColorProperty =
        DependencyProperty.Register(nameof(ScrambleColor), typeof(Brush), typeof(ScrambledText), new PropertyMetadata(null, OnScrambleColorChanged));

    public bool Intro
    {
        get { return (bool)GetValue(IntroProperty); }
        set { SetValue(IntroProperty, value); }
    }

    public static readonly DependencyProperty IntroProperty =
        DependencyProperty.Register(nameof(Intro), typeof(bool), typeof(ScrambledText), new PropertyMetadata(true));

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((ScrambledText)d).BuildLetters();

    private static void OnEffectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((ScrambledText)d).ResetLetters();

    private static void OnScrambleCharsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((ScrambledText)d).UpdateScrambleGlyphs();

    private static void OnScrambleColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((ScrambledText)d).RefreshScrambleColor();
}
