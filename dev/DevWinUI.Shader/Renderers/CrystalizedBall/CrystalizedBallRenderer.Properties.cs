using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace DevWinUI;

public partial class CrystalizedBallRenderer
{
    public CrystalizedBallPreset Preset
    {
        get { return (CrystalizedBallPreset)GetValue(PresetProperty); }
        set { SetValue(PresetProperty, value); }
    }

    public static readonly DependencyProperty PresetProperty =
        DependencyProperty.Register(nameof(Preset), typeof(CrystalizedBallPreset), typeof(CrystalizedBallRenderer), new PropertyMetadata(CrystalizedBallPreset.Plasma, OnPresetChanged));

    private static void OnPresetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((CrystalizedBallRenderer)d).Rebuild();
    }

    public Color BallColor
    {
        get { return (Color)GetValue(BallColorProperty); }
        set { SetValue(BallColorProperty, value); }
    }

    public static readonly DependencyProperty BallColorProperty =
        DependencyProperty.Register(nameof(BallColor), typeof(Color), typeof(CrystalizedBallRenderer), new PropertyMetadata(Colors.Transparent, OnSettingsChanged));

    public CrystalizedBallTheme Theme
    {
        get { return (CrystalizedBallTheme)GetValue(ThemeProperty); }
        set { SetValue(ThemeProperty, value); }
    }

    public static readonly DependencyProperty ThemeProperty =
        DependencyProperty.Register(nameof(Theme), typeof(CrystalizedBallTheme), typeof(CrystalizedBallRenderer), new PropertyMetadata(CrystalizedBallTheme.Auto, OnSettingsChanged));

    public Brush? BallBrush
    {
        get { return (Brush?)GetValue(BallBrushProperty); }
        set { SetValue(BallBrushProperty, value); }
    }

    public static readonly DependencyProperty BallBrushProperty =
        DependencyProperty.Register(nameof(BallBrush), typeof(Brush), typeof(CrystalizedBallRenderer), new PropertyMetadata(null, OnBallBrushChanged));

    private static void OnBallBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((CrystalizedBallRenderer)d).Rebuild();
    }

    public double BallSize
    {
        get { return (double)GetValue(BallSizeProperty); }
        set { SetValue(BallSizeProperty, value); }
    }

    public static readonly DependencyProperty BallSizeProperty =
        DependencyProperty.Register(nameof(BallSize), typeof(double), typeof(CrystalizedBallRenderer), new PropertyMetadata(0.7, OnSettingsChanged));

    public int Strands
    {
        get { return (int)GetValue(StrandsProperty); }
        set { SetValue(StrandsProperty, value); }
    }

    public static readonly DependencyProperty StrandsProperty =
        DependencyProperty.Register(nameof(Strands), typeof(int), typeof(CrystalizedBallRenderer), new PropertyMetadata(-1, OnSettingsChanged));

    public double Crackle
    {
        get { return (double)GetValue(CrackleProperty); }
        set { SetValue(CrackleProperty, value); }
    }

    public static readonly DependencyProperty CrackleProperty =
        DependencyProperty.Register(nameof(Crackle), typeof(double), typeof(CrystalizedBallRenderer), new PropertyMetadata(double.NaN, OnSettingsChanged));

    public double Flares
    {
        get { return (double)GetValue(FlaresProperty); }
        set { SetValue(FlaresProperty, value); }
    }

    public static readonly DependencyProperty FlaresProperty =
        DependencyProperty.Register(nameof(Flares), typeof(double), typeof(CrystalizedBallRenderer), new PropertyMetadata(double.NaN, OnSettingsChanged));

    public double Glow
    {
        get { return (double)GetValue(GlowProperty); }
        set { SetValue(GlowProperty, value); }
    }

    public static readonly DependencyProperty GlowProperty =
        DependencyProperty.Register(nameof(Glow), typeof(double), typeof(CrystalizedBallRenderer), new PropertyMetadata(double.NaN, OnSettingsChanged));

    public double Sparks
    {
        get { return (double)GetValue(SparksProperty); }
        set { SetValue(SparksProperty, value); }
    }

    public static readonly DependencyProperty SparksProperty =
        DependencyProperty.Register(nameof(Sparks), typeof(double), typeof(CrystalizedBallRenderer), new PropertyMetadata(double.NaN, OnSettingsChanged));

    public int ParticleCount
    {
        get { return (int)GetValue(ParticleCountProperty); }
        set { SetValue(ParticleCountProperty, value); }
    }

    public static readonly DependencyProperty ParticleCountProperty =
        DependencyProperty.Register(nameof(ParticleCount), typeof(int), typeof(CrystalizedBallRenderer), new PropertyMetadata(-1, OnSettingsChanged));

    public double Fill
    {
        get { return (double)GetValue(FillProperty); }
        set { SetValue(FillProperty, value); }
    }

    public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register(nameof(Fill), typeof(double), typeof(CrystalizedBallRenderer), new PropertyMetadata(double.NaN, OnSettingsChanged));

    public CrystalizedBallMotion Motion
    {
        get { return (CrystalizedBallMotion)GetValue(MotionProperty); }
        set { SetValue(MotionProperty, value); }
    }

    public static readonly DependencyProperty MotionProperty =
        DependencyProperty.Register(nameof(Motion), typeof(CrystalizedBallMotion), typeof(CrystalizedBallRenderer), new PropertyMetadata(CrystalizedBallMotion.Auto, OnSettingsChanged));

    public CrystalizedBallParticleShape ParticleShape
    {
        get { return (CrystalizedBallParticleShape)GetValue(ParticleShapeProperty); }
        set { SetValue(ParticleShapeProperty, value); }
    }

    public static readonly DependencyProperty ParticleShapeProperty =
        DependencyProperty.Register(nameof(ParticleShape), typeof(CrystalizedBallParticleShape), typeof(CrystalizedBallRenderer), new PropertyMetadata(CrystalizedBallParticleShape.Auto, OnSettingsChanged));

    public double Depth
    {
        get { return (double)GetValue(DepthProperty); }
        set { SetValue(DepthProperty, value); }
    }

    public static readonly DependencyProperty DepthProperty =
        DependencyProperty.Register(nameof(Depth), typeof(double), typeof(CrystalizedBallRenderer), new PropertyMetadata(double.NaN, OnSettingsChanged));

    public double Sway
    {
        get { return (double)GetValue(SwayProperty); }
        set { SetValue(SwayProperty, value); }
    }

    public static readonly DependencyProperty SwayProperty =
        DependencyProperty.Register(nameof(Sway), typeof(double), typeof(CrystalizedBallRenderer), new PropertyMetadata(double.NaN, OnSettingsChanged));

    public double Twinkle
    {
        get { return (double)GetValue(TwinkleProperty); }
        set { SetValue(TwinkleProperty, value); }
    }

    public static readonly DependencyProperty TwinkleProperty =
        DependencyProperty.Register(nameof(Twinkle), typeof(double), typeof(CrystalizedBallRenderer), new PropertyMetadata(double.NaN, OnSettingsChanged));

    public double Haze
    {
        get { return (double)GetValue(HazeProperty); }
        set { SetValue(HazeProperty, value); }
    }

    public static readonly DependencyProperty HazeProperty =
        DependencyProperty.Register(nameof(Haze), typeof(double), typeof(CrystalizedBallRenderer), new PropertyMetadata(double.NaN, OnSettingsChanged));

    public double Speed
    {
        get { return (double)GetValue(SpeedProperty); }
        set { SetValue(SpeedProperty, value); }
    }

    public static readonly DependencyProperty SpeedProperty =
        DependencyProperty.Register(nameof(Speed), typeof(double), typeof(CrystalizedBallRenderer), new PropertyMetadata(1.0, OnSettingsChanged));

    public double DustSpeed
    {
        get { return (double)GetValue(DustSpeedProperty); }
        set { SetValue(DustSpeedProperty, value); }
    }

    public static readonly DependencyProperty DustSpeedProperty =
        DependencyProperty.Register(nameof(DustSpeed), typeof(double), typeof(CrystalizedBallRenderer), new PropertyMetadata(double.NaN, OnSettingsChanged));

    public bool Interactive
    {
        get { return (bool)GetValue(InteractiveProperty); }
        set { SetValue(InteractiveProperty, value); }
    }

    public static readonly DependencyProperty InteractiveProperty =
        DependencyProperty.Register(nameof(Interactive), typeof(bool), typeof(CrystalizedBallRenderer), new PropertyMetadata(true, OnSettingsChanged));

    public double HoverStrength
    {
        get { return (double)GetValue(HoverStrengthProperty); }
        set { SetValue(HoverStrengthProperty, value); }
    }

    public static readonly DependencyProperty HoverStrengthProperty =
        DependencyProperty.Register(nameof(HoverStrength), typeof(double), typeof(CrystalizedBallRenderer), new PropertyMetadata(0.7, OnSettingsChanged));

    public bool Intro
    {
        get { return (bool)GetValue(IntroProperty); }
        set { SetValue(IntroProperty, value); }
    }

    public static readonly DependencyProperty IntroProperty =
        DependencyProperty.Register(nameof(Intro), typeof(bool), typeof(CrystalizedBallRenderer), new PropertyMetadata(true, OnSettingsChanged));

    public bool IsPaused
    {
        get { return (bool)GetValue(IsPausedProperty); }
        set { SetValue(IsPausedProperty, value); }
    }

    public static readonly DependencyProperty IsPausedProperty =
        DependencyProperty.Register(nameof(IsPaused), typeof(bool), typeof(CrystalizedBallRenderer), new PropertyMetadata(false, OnSettingsChanged));
}
