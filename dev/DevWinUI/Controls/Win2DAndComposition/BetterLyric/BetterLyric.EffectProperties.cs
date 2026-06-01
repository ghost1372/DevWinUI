// https://github.com/jayfunc/BetterLyrics

namespace DevWinUI;

public partial class BetterLyric
{
    internal LyricsWordByWordEffectMode wordByWordEffectMode = LyricsWordByWordEffectMode.Auto;
    internal bool isLyricsBlurEffectEnabled = true;
    internal bool isLyricsFadeOutEffectEnabled = true;
    internal bool isLyricsOutOfSightEffectEnabled = true;
    internal bool isLyricsGlowEffectEnabled = true;
    internal LyricEffectScope lyricsGlowEffectScope = LyricEffectScope.LongDurationSyllable;
    internal int lyricsGlowEffectLongSyllableDuration = 700;
    internal bool isLyricsGlowEffectAmountAutoAdjust = true;
    internal int lyricsGlowEffectAmount = 8;
    internal bool isLyricsShadowEffectEnabled = false;
    internal bool isLyricsScaleEffectEnabled = true;
    internal int lyricsScaleEffectLongSyllableDuration = 700;
    internal bool isLyricsScaleEffectAmountAutoAdjust = true;
    internal int lyricsScaleEffectAmount = 115;
    internal bool isLyricsFloatAnimationEnabled = true;
    internal bool isLyricsFloatAnimationAmountAutoAdjust = true;
    internal int lyricsFloatAnimationAmount = 8;
    internal int lyricsFloatAnimationDuration = 450;
    internal AnimationEasingType lyricsScrollEasingType = AnimationEasingType.Quad;
    internal AnimationEaseMode lyricsScrollEasingMode = AnimationEaseMode.Out;
    internal int lyricsScrollDuration = 500;
    internal int lyricsScrollTopDuration = 500;
    internal int lyricsScrollBottomDuration = 500;
    internal int lyricsScrollTopDelay = 0;
    internal int lyricsScrollBottomDelay = 0;
    internal bool isFanLyricsEnabled = false;
    internal int fanLyricsAngle = 30;
    internal bool is3DLyricsEnabled = false;
    internal int lyrics3DXAngle = 30;
    internal int lyrics3DYAngle = 0;
    internal int lyrics3DZAngle = 0;
    internal bool lyrics3DAutoFitLayout = false;
    internal int lyrics3DDepth = 1000;
    internal bool isLyricsBrethingEffectEnabled = false;
    internal int lyricsBreathingIntensity = 80;
    public LyricsWordByWordEffectMode WordByWordEffectMode
    {
        get { return (LyricsWordByWordEffectMode)GetValue(WordByWordEffectModeProperty); }
        set { SetValue(WordByWordEffectModeProperty, value); }
    }

    public static readonly DependencyProperty WordByWordEffectModeProperty =
        DependencyProperty.Register(nameof(WordByWordEffectMode), typeof(LyricsWordByWordEffectMode), typeof(BetterLyric), new PropertyMetadata(LyricsWordByWordEffectMode.Auto, OnWordByWordEffectModeChanged));

    private static void OnWordByWordEffectModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.wordByWordEffectMode = (LyricsWordByWordEffectMode)e.NewValue;
    }


    public bool IsLyricsBlurEffectEnabled
    {
        get { return (bool)GetValue(IsLyricsBlurEffectEnabledProperty); }
        set { SetValue(IsLyricsBlurEffectEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsLyricsBlurEffectEnabledProperty =
        DependencyProperty.Register(nameof(IsLyricsBlurEffectEnabled), typeof(bool), typeof(BetterLyric), new PropertyMetadata(true, OnIsLyricsBlurEffectEnabledChanged));

    private static void OnIsLyricsBlurEffectEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isLyricsBlurEffectEnabled = (bool)e.NewValue;
        ctl.RequestRelayout();
    }


    public bool IsLyricsFadeOutEffectEnabled
    {
        get { return (bool)GetValue(IsLyricsFadeOutEffectEnabledProperty); }
        set { SetValue(IsLyricsFadeOutEffectEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsLyricsFadeOutEffectEnabledProperty =
        DependencyProperty.Register(nameof(IsLyricsFadeOutEffectEnabled), typeof(bool), typeof(BetterLyric), new PropertyMetadata(true, OnIsLyricsFadeOutEffectEnabledChanged));

    private static void OnIsLyricsFadeOutEffectEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isLyricsFadeOutEffectEnabled = (bool)e.NewValue;
        ctl.RequestRelayout();
    }


    public bool IsLyricsOutOfSightEffectEnabled
    {
        get { return (bool)GetValue(IsLyricsOutOfSightEffectEnabledProperty); }
        set { SetValue(IsLyricsOutOfSightEffectEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsLyricsOutOfSightEffectEnabledProperty =
        DependencyProperty.Register(nameof(IsLyricsOutOfSightEffectEnabled), typeof(bool), typeof(BetterLyric), new PropertyMetadata(true, OnIsLyricsOutOfSightEffectEnabledChanged));

    private static void OnIsLyricsOutOfSightEffectEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isLyricsOutOfSightEffectEnabled = (bool)e.NewValue;
        ctl.RequestRelayout();
    }


    public bool IsLyricsGlowEffectEnabled
    {
        get { return (bool)GetValue(IsLyricsGlowEffectEnabledProperty); }
        set { SetValue(IsLyricsGlowEffectEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsLyricsGlowEffectEnabledProperty =
        DependencyProperty.Register(nameof(IsLyricsGlowEffectEnabled), typeof(bool), typeof(BetterLyric), new PropertyMetadata(true, OnIsLyricsGlowEffectEnabledChanged));

    private static void OnIsLyricsGlowEffectEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isLyricsGlowEffectEnabled = (bool)e.NewValue;
    }


    public LyricEffectScope LyricsGlowEffectScope
    {
        get { return (LyricEffectScope)GetValue(LyricsGlowEffectScopeProperty); }
        set { SetValue(LyricsGlowEffectScopeProperty, value); }
    }

    public static readonly DependencyProperty LyricsGlowEffectScopeProperty =
        DependencyProperty.Register(nameof(LyricsGlowEffectScope), typeof(LyricEffectScope), typeof(BetterLyric), new PropertyMetadata(LyricEffectScope.LongDurationSyllable, OnLyricsGlowEffectScopeChanged));

    private static void OnLyricsGlowEffectScopeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsGlowEffectScope = (LyricEffectScope)e.NewValue;
    }


    public int LyricsGlowEffectLongSyllableDuration
    {
        get { return (int)GetValue(LyricsGlowEffectLongSyllableDurationProperty); }
        set { SetValue(LyricsGlowEffectLongSyllableDurationProperty, value); }
    }

    public static readonly DependencyProperty LyricsGlowEffectLongSyllableDurationProperty =
        DependencyProperty.Register(nameof(LyricsGlowEffectLongSyllableDuration), typeof(int), typeof(BetterLyric), new PropertyMetadata(700, OnLyricsGlowEffectLongSyllableDurationChanged));

    private static void OnLyricsGlowEffectLongSyllableDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsGlowEffectLongSyllableDuration = (int)e.NewValue;
    }


    public bool IsLyricsGlowEffectAmountAutoAdjust
    {
        get { return (bool)GetValue(IsLyricsGlowEffectAmountAutoAdjustProperty); }
        set { SetValue(IsLyricsGlowEffectAmountAutoAdjustProperty, value); }
    }

    public static readonly DependencyProperty IsLyricsGlowEffectAmountAutoAdjustProperty =
        DependencyProperty.Register(nameof(IsLyricsGlowEffectAmountAutoAdjust), typeof(bool), typeof(BetterLyric), new PropertyMetadata(true, OnIsLyricsGlowEffectAmountAutoAdjustChanged));

    private static void OnIsLyricsGlowEffectAmountAutoAdjustChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isLyricsGlowEffectAmountAutoAdjust = (bool)e.NewValue;
    }


    public int LyricsGlowEffectAmount
    {
        get { return (int)GetValue(LyricsGlowEffectAmountProperty); }
        set { SetValue(LyricsGlowEffectAmountProperty, value); }
    }

    public static readonly DependencyProperty LyricsGlowEffectAmountProperty =
        DependencyProperty.Register(nameof(LyricsGlowEffectAmount), typeof(int), typeof(BetterLyric), new PropertyMetadata(8, OnLyricsGlowEffectAmountChanged));

    private static void OnLyricsGlowEffectAmountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsGlowEffectAmount = (int)e.NewValue;
    }


    public bool IsLyricsShadowEffectEnabled
    {
        get { return (bool)GetValue(IsLyricsShadowEffectEnabledProperty); }
        set { SetValue(IsLyricsShadowEffectEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsLyricsShadowEffectEnabledProperty =
        DependencyProperty.Register(nameof(IsLyricsShadowEffectEnabled), typeof(bool), typeof(BetterLyric), new PropertyMetadata(false, OnIsLyricsShadowEffectEnabledChanged));

    private static void OnIsLyricsShadowEffectEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isLyricsShadowEffectEnabled = (bool)e.NewValue;
    }


    public bool IsLyricsScaleEffectEnabled
    {
        get { return (bool)GetValue(IsLyricsScaleEffectEnabledProperty); }
        set { SetValue(IsLyricsScaleEffectEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsLyricsScaleEffectEnabledProperty =
        DependencyProperty.Register(nameof(IsLyricsScaleEffectEnabled), typeof(bool), typeof(BetterLyric), new PropertyMetadata(true, OnIsLyricsScaleEffectEnabledChanged));

    private static void OnIsLyricsScaleEffectEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isLyricsScaleEffectEnabled = (bool)e.NewValue;
    }


    public int LyricsScaleEffectLongSyllableDuration
    {
        get { return (int)GetValue(LyricsScaleEffectLongSyllableDurationProperty); }
        set { SetValue(LyricsScaleEffectLongSyllableDurationProperty, value); }
    }

    public static readonly DependencyProperty LyricsScaleEffectLongSyllableDurationProperty =
        DependencyProperty.Register(nameof(LyricsScaleEffectLongSyllableDuration), typeof(int), typeof(BetterLyric), new PropertyMetadata(700, OnLyricsScaleEffectLongSyllableDurationChanged));

    private static void OnLyricsScaleEffectLongSyllableDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsScaleEffectLongSyllableDuration = (int)e.NewValue;
    }


    public bool IsLyricsScaleEffectAmountAutoAdjust
    {
        get { return (bool)GetValue(IsLyricsScaleEffectAmountAutoAdjustProperty); }
        set { SetValue(IsLyricsScaleEffectAmountAutoAdjustProperty, value); }
    }

    public static readonly DependencyProperty IsLyricsScaleEffectAmountAutoAdjustProperty =
        DependencyProperty.Register(nameof(IsLyricsScaleEffectAmountAutoAdjust), typeof(bool), typeof(BetterLyric), new PropertyMetadata(true, OnIsLyricsScaleEffectAmountAutoAdjustChanged));

    private static void OnIsLyricsScaleEffectAmountAutoAdjustChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isLyricsScaleEffectAmountAutoAdjust = (bool)e.NewValue;
    }


    public int LyricsScaleEffectAmount
    {
        get { return (int)GetValue(LyricsScaleEffectAmountProperty); }
        set { SetValue(LyricsScaleEffectAmountProperty, value); }
    }

    public static readonly DependencyProperty LyricsScaleEffectAmountProperty =
        DependencyProperty.Register(nameof(LyricsScaleEffectAmount), typeof(int), typeof(BetterLyric), new PropertyMetadata(115, OnLyricsScaleEffectAmountChanged));

    private static void OnLyricsScaleEffectAmountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsScaleEffectAmount = (int)e.NewValue;
    }


    public bool IsLyricsFloatAnimationEnabled
    {
        get { return (bool)GetValue(IsLyricsFloatAnimationEnabledProperty); }
        set { SetValue(IsLyricsFloatAnimationEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsLyricsFloatAnimationEnabledProperty =
        DependencyProperty.Register(nameof(IsLyricsFloatAnimationEnabled), typeof(bool), typeof(BetterLyric), new PropertyMetadata(true, OnIsLyricsFloatAnimationEnabledChanged));

    private static void OnIsLyricsFloatAnimationEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isLyricsFloatAnimationEnabled = (bool)e.NewValue;
    }


    public bool IsLyricsFloatAnimationAmountAutoAdjust
    {
        get { return (bool)GetValue(IsLyricsFloatAnimationAmountAutoAdjustProperty); }
        set { SetValue(IsLyricsFloatAnimationAmountAutoAdjustProperty, value); }
    }

    public static readonly DependencyProperty IsLyricsFloatAnimationAmountAutoAdjustProperty =
        DependencyProperty.Register(nameof(IsLyricsFloatAnimationAmountAutoAdjust), typeof(bool), typeof(BetterLyric), new PropertyMetadata(true, OnIsLyricsFloatAnimationAmountAutoAdjustChanged));

    private static void OnIsLyricsFloatAnimationAmountAutoAdjustChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isLyricsFloatAnimationAmountAutoAdjust = (bool)e.NewValue;
    }


    public int LyricsFloatAnimationAmount
    {
        get { return (int)GetValue(LyricsFloatAnimationAmountProperty); }
        set { SetValue(LyricsFloatAnimationAmountProperty, value); }
    }

    public static readonly DependencyProperty LyricsFloatAnimationAmountProperty =
        DependencyProperty.Register(nameof(LyricsFloatAnimationAmount), typeof(int), typeof(BetterLyric), new PropertyMetadata(8, OnLyricsFloatAnimationAmountChanged));

    private static void OnLyricsFloatAnimationAmountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsFloatAnimationAmount = (int)e.NewValue;
    }


    public int LyricsFloatAnimationDuration
    {
        get { return (int)GetValue(LyricsFloatAnimationDurationProperty); }
        set { SetValue(LyricsFloatAnimationDurationProperty, value); }
    }

    public static readonly DependencyProperty LyricsFloatAnimationDurationProperty =
        DependencyProperty.Register(nameof(LyricsFloatAnimationDuration), typeof(int), typeof(BetterLyric), new PropertyMetadata(450, OnLyricsFloatAnimationDurationChanged));

    private static void OnLyricsFloatAnimationDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsFloatAnimationDuration = (int)e.NewValue;
    }

    public AnimationEasingType LyricsScrollEasingType
    {
        get { return (AnimationEasingType)GetValue(LyricsScrollEasingTypeProperty); }
        set { SetValue(LyricsScrollEasingTypeProperty, value); }
    }

    public static readonly DependencyProperty LyricsScrollEasingTypeProperty =
        DependencyProperty.Register(nameof(LyricsScrollEasingType), typeof(AnimationEasingType), typeof(BetterLyric), new PropertyMetadata(AnimationEasingType.Quad, OnLyricsScrollEasingTypeChanged));

    private static void OnLyricsScrollEasingTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsScrollEasingType = (AnimationEasingType)e.NewValue;
    }


    public AnimationEaseMode LyricsScrollEasingMode
    {
        get { return (AnimationEaseMode)GetValue(LyricsScrollEasingModeProperty); }
        set { SetValue(LyricsScrollEasingModeProperty, value); }
    }

    public static readonly DependencyProperty LyricsScrollEasingModeProperty =
        DependencyProperty.Register(nameof(LyricsScrollEasingMode), typeof(AnimationEaseMode), typeof(BetterLyric), new PropertyMetadata(AnimationEaseMode.Out, OnLyricsScrollEasingModeChanged));

    private static void OnLyricsScrollEasingModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsScrollEasingMode = (AnimationEaseMode)e.NewValue;
    }


    public int LyricsScrollDuration
    {
        get { return (int)GetValue(LyricsScrollDurationProperty); }
        set { SetValue(LyricsScrollDurationProperty, value); }
    }

    public static readonly DependencyProperty LyricsScrollDurationProperty =
        DependencyProperty.Register(nameof(LyricsScrollDuration), typeof(int), typeof(BetterLyric), new PropertyMetadata(500, OnLyricsScrollDurationChanged));

    private static void OnLyricsScrollDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsScrollDuration = (int)e.NewValue;
        ctl.RequestRelayout();
    }


    public int LyricsScrollTopDuration
    {
        get { return (int)GetValue(LyricsScrollTopDurationProperty); }
        set { SetValue(LyricsScrollTopDurationProperty, value); }
    }

    public static readonly DependencyProperty LyricsScrollTopDurationProperty =
        DependencyProperty.Register(nameof(LyricsScrollTopDuration), typeof(int), typeof(BetterLyric), new PropertyMetadata(500, OnLyricsScrollTopDurationChanged));

    private static void OnLyricsScrollTopDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsScrollTopDuration = (int)e.NewValue;
        ctl.RequestRelayout();
    }


    public int LyricsScrollBottomDuration
    {
        get { return (int)GetValue(LyricsScrollBottomDurationProperty); }
        set { SetValue(LyricsScrollBottomDurationProperty, value); }
    }

    public static readonly DependencyProperty LyricsScrollBottomDurationProperty =
        DependencyProperty.Register(nameof(LyricsScrollBottomDuration), typeof(int), typeof(BetterLyric), new PropertyMetadata(500, OnLyricsScrollBottomDurationChanged));

    private static void OnLyricsScrollBottomDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsScrollBottomDuration = (int)e.NewValue;
        ctl.RequestRelayout();
    }


    public int LyricsScrollTopDelay
    {
        get { return (int)GetValue(LyricsScrollTopDelayProperty); }
        set { SetValue(LyricsScrollTopDelayProperty, value); }
    }

    public static readonly DependencyProperty LyricsScrollTopDelayProperty =
        DependencyProperty.Register(nameof(LyricsScrollTopDelay), typeof(int), typeof(BetterLyric), new PropertyMetadata(0, OnLyricsScrollTopDelayChanged));

    private static void OnLyricsScrollTopDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsScrollTopDelay = (int)e.NewValue;
        ctl.RequestRelayout();
    }


    public int LyricsScrollBottomDelay
    {
        get { return (int)GetValue(LyricsScrollBottomDelayProperty); }
        set { SetValue(LyricsScrollBottomDelayProperty, value); }
    }

    public static readonly DependencyProperty LyricsScrollBottomDelayProperty =
        DependencyProperty.Register(nameof(LyricsScrollBottomDelay), typeof(int), typeof(BetterLyric), new PropertyMetadata(0, OnLyricsScrollBottomDelayChanged));

    private static void OnLyricsScrollBottomDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsScrollBottomDelay = (int)e.NewValue;
        ctl.RequestRelayout();
    }


    public bool IsFanLyricsEnabled
    {
        get { return (bool)GetValue(IsFanLyricsEnabledProperty); }
        set { SetValue(IsFanLyricsEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsFanLyricsEnabledProperty =
        DependencyProperty.Register(nameof(IsFanLyricsEnabled), typeof(bool), typeof(BetterLyric), new PropertyMetadata(false, OnIsFanLyricsEnabledChanged));

    private static void OnIsFanLyricsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isFanLyricsEnabled = (bool)e.NewValue;
        ctl.RequestRelayout();
    }


    public int FanLyricsAngle
    {
        get { return (int)GetValue(FanLyricsAngleProperty); }
        set { SetValue(FanLyricsAngleProperty, value); }
    }

    public static readonly DependencyProperty FanLyricsAngleProperty =
        DependencyProperty.Register(nameof(FanLyricsAngle), typeof(int), typeof(BetterLyric), new PropertyMetadata(30, OnFanLyricsAngleChanged));

    private static void OnFanLyricsAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.fanLyricsAngle = (int)e.NewValue;
        ctl.RequestRelayout();
    }


    public bool Is3DLyricsEnabled
    {
        get { return (bool)GetValue(Is3DLyricsEnabledProperty); }
        set { SetValue(Is3DLyricsEnabledProperty, value); }
    }

    public static readonly DependencyProperty Is3DLyricsEnabledProperty =
        DependencyProperty.Register(nameof(Is3DLyricsEnabled), typeof(bool), typeof(BetterLyric), new PropertyMetadata(false, OnIs3DLyricsEnabledChanged));

    private static void OnIs3DLyricsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.is3DLyricsEnabled = (bool)e.NewValue;
    }


    public int Lyrics3DXAngle
    {
        get { return (int)GetValue(Lyrics3DXAngleProperty); }
        set { SetValue(Lyrics3DXAngleProperty, value); }
    }

    public static readonly DependencyProperty Lyrics3DXAngleProperty =
        DependencyProperty.Register(nameof(Lyrics3DXAngle), typeof(int), typeof(BetterLyric), new PropertyMetadata(30, OnLyrics3DXAngleChanged));

    private static void OnLyrics3DXAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyrics3DXAngle = (int)e.NewValue;
        ctl.RequestRelayout();
    }


    public int Lyrics3DYAngle
    {
        get { return (int)GetValue(Lyrics3DYAngleProperty); }
        set { SetValue(Lyrics3DYAngleProperty, value); }
    }

    public static readonly DependencyProperty Lyrics3DYAngleProperty =
        DependencyProperty.Register(nameof(Lyrics3DYAngle), typeof(int), typeof(BetterLyric), new PropertyMetadata(0, OnLyrics3DYAngleChanged));

    private static void OnLyrics3DYAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyrics3DYAngle = (int)e.NewValue;
        ctl.RequestRelayout();
    }


    public int Lyrics3DZAngle
    {
        get { return (int)GetValue(Lyrics3DZAngleProperty); }
        set { SetValue(Lyrics3DZAngleProperty, value); }
    }

    public static readonly DependencyProperty Lyrics3DZAngleProperty =
        DependencyProperty.Register(nameof(Lyrics3DZAngle), typeof(int), typeof(BetterLyric), new PropertyMetadata(0, OnLyrics3DZAngleChanged));

    private static void OnLyrics3DZAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyrics3DZAngle = (int)e.NewValue;
        ctl.RequestRelayout();
    }


    public bool Lyrics3DAutoFitLayout
    {
        get { return (bool)GetValue(Lyrics3DAutoFitLayoutProperty); }
        set { SetValue(Lyrics3DAutoFitLayoutProperty, value); }
    }

    public static readonly DependencyProperty Lyrics3DAutoFitLayoutProperty =
        DependencyProperty.Register(nameof(Lyrics3DAutoFitLayout), typeof(bool), typeof(BetterLyric), new PropertyMetadata(false, OnLyrics3DAutoFitLayoutChanged));

    private static void OnLyrics3DAutoFitLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyrics3DAutoFitLayout = (bool)e.NewValue;
    }

    public int Lyrics3DDepth
    {
        get { return (int)GetValue(Lyrics3DDepthProperty); }
        set { SetValue(Lyrics3DDepthProperty, value); }
    }

    public static readonly DependencyProperty Lyrics3DDepthProperty =
        DependencyProperty.Register(nameof(Lyrics3DDepth), typeof(int), typeof(BetterLyric), new PropertyMetadata(1000, OnLyrics3DDepthChanged));

    private static void OnLyrics3DDepthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyrics3DDepth = (int)e.NewValue;
        ctl.RequestRelayout();
    }


    public bool IsLyricsBrethingEffectEnabled
    {
        get { return (bool)GetValue(IsLyricsBrethingEffectEnabledProperty); }
        set { SetValue(IsLyricsBrethingEffectEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsLyricsBrethingEffectEnabledProperty =
        DependencyProperty.Register(nameof(IsLyricsBrethingEffectEnabled), typeof(bool), typeof(BetterLyric), new PropertyMetadata(false, OnIsLyricsBrethingEffectEnabledChanged));

    private static void OnIsLyricsBrethingEffectEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isLyricsBrethingEffectEnabled = (bool)e.NewValue;
    }


    public int LyricsBreathingIntensity
    {
        get { return (int)GetValue(LyricsBreathingIntensityProperty); }
        set { SetValue(LyricsBreathingIntensityProperty, value); }
    }

    public static readonly DependencyProperty LyricsBreathingIntensityProperty =
        DependencyProperty.Register(nameof(LyricsBreathingIntensity), typeof(int), typeof(BetterLyric), new PropertyMetadata(80, OnLyricsBreathingIntensityChanged));

    private static void OnLyricsBreathingIntensityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsBreathingIntensity = (int)e.NewValue;
    }
}
