// https://github.com/jayfunc/BetterLyrics

namespace DevWinUI;

public partial class BetterLyric
{
    internal bool isDynamicLyricsFontSize = true;
    internal int phoneticLyricsFontSize = 12;
    internal int originalLyricsFontSize = 24;
    internal int translatedLyricsFontSize = 12;
    internal int phoneticLyricsOpacity = 60;
    internal int playedOriginalLyricsOpacity = 100;
    internal int unplayedOriginalLyricsOpacity = 30;
    internal int translatedLyricsOpacity = 60;
    internal LyricsTextAlignmentType lyricsAlignmentType = LyricsTextAlignmentType.Left;
    internal LyricsContentOrientation lyricsLineContentOrientation = LyricsContentOrientation.Vertical;
    internal bool autoWrap = true;
    internal int lyricsFontStrokeWidth = 0;
    internal Color nonCurrentLineFillColor = Colors.White;
    internal Color playedCurrentLineFillColor = Colors.White;
    internal Color unplayedCurrentLineFillColor = Colors.White;
    internal Color playedTextStrokeColor = Colors.White;
    internal Color unplayedTextStrokeColor = Colors.White;
    internal LyricFontWeight lyricsFontWeight = LyricFontWeight.Bold;
    internal double lyricsLineOverallSpacingFactor = 0.5;
    internal double lyricsLineInnerSpacingFactor = 0.1;
    internal string lyricsWesternFontFamily = "Arial";
    internal string lyricsCJKFontFamily = "Arial";
    internal bool isRightToLeftLyric = false;
    internal int playingLineTopOffset = 50;

    public bool IsDynamicLyricsFontSize
    {
        get { return (bool)GetValue(IsDynamicLyricsFontSizeProperty); }
        set { SetValue(IsDynamicLyricsFontSizeProperty, value); }
    }

    public static readonly DependencyProperty IsDynamicLyricsFontSizeProperty =
        DependencyProperty.Register(nameof(IsDynamicLyricsFontSize), typeof(bool), typeof(BetterLyric), new PropertyMetadata(true, OnIsDynamicLyricsFontSizeChanged));

    private static void OnIsDynamicLyricsFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isDynamicLyricsFontSize = (bool)e.NewValue;
        ctl.RequestRelayout();
    }


    public int PhoneticLyricsFontSize
    {
        get { return (int)GetValue(PhoneticLyricsFontSizeProperty); }
        set { SetValue(PhoneticLyricsFontSizeProperty, value); }
    }

    public static readonly DependencyProperty PhoneticLyricsFontSizeProperty =
        DependencyProperty.Register(nameof(PhoneticLyricsFontSize), typeof(int), typeof(BetterLyric), new PropertyMetadata(12, OnPhoneticLyricsFontSizeChanged));

    private static void OnPhoneticLyricsFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.phoneticLyricsFontSize = (int)e.NewValue;
        ctl.RequestRelayout();
    }


    public int OriginalLyricsFontSize
    {
        get { return (int)GetValue(OriginalLyricsFontSizeProperty); }
        set { SetValue(OriginalLyricsFontSizeProperty, value); }
    }

    public static readonly DependencyProperty OriginalLyricsFontSizeProperty =
        DependencyProperty.Register(nameof(OriginalLyricsFontSize), typeof(int), typeof(BetterLyric), new PropertyMetadata(24, OnOriginalLyricsFontSizeChanged));

    private static void OnOriginalLyricsFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.originalLyricsFontSize = (int)e.NewValue;
        ctl.RequestRelayout();
    }


    public int TranslatedLyricsFontSize
    {
        get { return (int)GetValue(TranslatedLyricsFontSizeProperty); }
        set { SetValue(TranslatedLyricsFontSizeProperty, value); }
    }

    public static readonly DependencyProperty TranslatedLyricsFontSizeProperty =
        DependencyProperty.Register(nameof(TranslatedLyricsFontSize), typeof(int), typeof(BetterLyric), new PropertyMetadata(12, OnTranslatedLyricsFontSizeChanged));

    private static void OnTranslatedLyricsFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.translatedLyricsFontSize = (int)e.NewValue;
        ctl.RequestRelayout();
    }


    public int PhoneticLyricsOpacity
    {
        get { return (int)GetValue(PhoneticLyricsOpacityProperty); }
        set { SetValue(PhoneticLyricsOpacityProperty, value); }
    }

    public static readonly DependencyProperty PhoneticLyricsOpacityProperty =
        DependencyProperty.Register(nameof(PhoneticLyricsOpacity), typeof(int), typeof(BetterLyric), new PropertyMetadata(60, OnPhoneticLyricsOpacityChanged));

    private static void OnPhoneticLyricsOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.phoneticLyricsOpacity = (int)e.NewValue;
        ctl.RequestRelayout();
    }


    public int PlayedOriginalLyricsOpacity
    {
        get { return (int)GetValue(PlayedOriginalLyricsOpacityProperty); }
        set { SetValue(PlayedOriginalLyricsOpacityProperty, value); }
    }

    public static readonly DependencyProperty PlayedOriginalLyricsOpacityProperty =
        DependencyProperty.Register(nameof(PlayedOriginalLyricsOpacity), typeof(int), typeof(BetterLyric), new PropertyMetadata(100, OnPlayedOriginalLyricsOpacityChanged));

    private static void OnPlayedOriginalLyricsOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.playedOriginalLyricsOpacity = (int)e.NewValue;
        ctl.RequestRelayout();
    }

    public int UnplayedOriginalLyricsOpacity
    {
        get { return (int)GetValue(UnplayedOriginalLyricsOpacityProperty); }
        set { SetValue(UnplayedOriginalLyricsOpacityProperty, value); }
    }

    public static readonly DependencyProperty UnplayedOriginalLyricsOpacityProperty =
        DependencyProperty.Register(nameof(UnplayedOriginalLyricsOpacity), typeof(int), typeof(BetterLyric), new PropertyMetadata(30, OnUnplayedOriginalLyricsOpacityChanged));

    private static void OnUnplayedOriginalLyricsOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.unplayedOriginalLyricsOpacity = (int)e.NewValue;
        ctl.RequestRelayout();
    }


    public int TranslatedLyricsOpacity
    {
        get { return (int)GetValue(TranslatedLyricsOpacityProperty); }
        set { SetValue(TranslatedLyricsOpacityProperty, value); }
    }

    public static readonly DependencyProperty TranslatedLyricsOpacityProperty =
        DependencyProperty.Register(nameof(TranslatedLyricsOpacity), typeof(int), typeof(BetterLyric), new PropertyMetadata(60, OnTranslatedLyricsOpacityChanged));

    private static void OnTranslatedLyricsOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.translatedLyricsOpacity = (int)e.NewValue;
        ctl.RequestRelayout();
    }


    public LyricsTextAlignmentType LyricsAlignmentType
    {
        get { return (LyricsTextAlignmentType)GetValue(LyricsAlignmentTypeProperty); }
        set { SetValue(LyricsAlignmentTypeProperty, value); }
    }

    public static readonly DependencyProperty LyricsAlignmentTypeProperty =
        DependencyProperty.Register(nameof(LyricsAlignmentType), typeof(LyricsTextAlignmentType), typeof(BetterLyric), new PropertyMetadata(LyricsTextAlignmentType.Left, OnLyricsAlignmentTypeChanged));

    private static void OnLyricsAlignmentTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsAlignmentType = (LyricsTextAlignmentType)e.NewValue;
        ctl.RequestRelayout();
    }


    public LyricsContentOrientation LyricsLineContentOrientation
    {
        get { return (LyricsContentOrientation)GetValue(LyricsLineContentOrientationProperty); }
        set { SetValue(LyricsLineContentOrientationProperty, value); }
    }

    public static readonly DependencyProperty LyricsLineContentOrientationProperty =
        DependencyProperty.Register(nameof(LyricsLineContentOrientation), typeof(LyricsContentOrientation), typeof(BetterLyric), new PropertyMetadata(LyricsContentOrientation.Vertical, OnLyricsLineContentOrientationChanged));

    private static void OnLyricsLineContentOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsLineContentOrientation = (LyricsContentOrientation)e.NewValue;
        ctl.RequestRelayout();
    }

    public bool AutoWrap
    {
        get { return (bool)GetValue(AutoWrapProperty); }
        set { SetValue(AutoWrapProperty, value); }
    }

    public static readonly DependencyProperty AutoWrapProperty =
        DependencyProperty.Register(nameof(AutoWrap), typeof(bool), typeof(BetterLyric), new PropertyMetadata(true, OnAutoWrapChanged));

    private static void OnAutoWrapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.autoWrap = (bool)e.NewValue;
        ctl.RequestRelayout();
    }

    public int LyricsFontStrokeWidth
    {
        get { return (int)GetValue(LyricsFontStrokeWidthProperty); }
        set { SetValue(LyricsFontStrokeWidthProperty, value); }
    }

    public static readonly DependencyProperty LyricsFontStrokeWidthProperty =
        DependencyProperty.Register(nameof(LyricsFontStrokeWidth), typeof(int), typeof(BetterLyric), new PropertyMetadata(0, OnLyricsFontStrokeWidthChanged));

    private static void OnLyricsFontStrokeWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsFontStrokeWidth = (int)e.NewValue;
        ctl.RequestRelayout();
    }


    public Color NonCurrentLineFillColor
    {
        get { return (Color)GetValue(NonCurrentLineFillColorProperty); }
        set { SetValue(NonCurrentLineFillColorProperty, value); }
    }

    public static readonly DependencyProperty NonCurrentLineFillColorProperty =
        DependencyProperty.Register(nameof(NonCurrentLineFillColor), typeof(Color), typeof(BetterLyric), new PropertyMetadata(Colors.White, OnNonCurrentLineFillColorChanged));

    private static void OnNonCurrentLineFillColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.nonCurrentLineFillColor = (Color)e.NewValue;
    }
    
    public Color PlayedCurrentLineFillColor
    {
        get { return (Color)GetValue(PlayedCurrentLineFillColorProperty); }
        set { SetValue(PlayedCurrentLineFillColorProperty, value); }
    }

    public static readonly DependencyProperty PlayedCurrentLineFillColorProperty =
        DependencyProperty.Register(nameof(PlayedCurrentLineFillColor), typeof(Color), typeof(BetterLyric), new PropertyMetadata(Colors.White, OnPlayedCurrentLineFillColorChanged));

    private static void OnPlayedCurrentLineFillColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.playedCurrentLineFillColor = (Color)e.NewValue;
    }

   
    public Color UnplayedCurrentLineFillColor
    {
        get { return (Color)GetValue(UnplayedCurrentLineFillColorProperty); }
        set { SetValue(UnplayedCurrentLineFillColorProperty, value); }
    }

    public static readonly DependencyProperty UnplayedCurrentLineFillColorProperty =
        DependencyProperty.Register(nameof(UnplayedCurrentLineFillColor), typeof(Color), typeof(BetterLyric), new PropertyMetadata(Colors.White, OnUnplayedCurrentLineFillColorChanged));

    private static void OnUnplayedCurrentLineFillColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.unplayedCurrentLineFillColor = (Color)e.NewValue;
    }

    
    public Color PlayedTextStrokeColor
    {
        get { return (Color)GetValue(PlayedTextStrokeColorProperty); }
        set { SetValue(PlayedTextStrokeColorProperty, value); }
    }

    public static readonly DependencyProperty PlayedTextStrokeColorProperty =
        DependencyProperty.Register(nameof(PlayedTextStrokeColor), typeof(Color), typeof(BetterLyric), new PropertyMetadata(Colors.White, OnPlayedTextStrokeColorChanged));

    private static void OnPlayedTextStrokeColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.playedTextStrokeColor = (Color)e.NewValue;
    }
    
    public Color UnplayedTextStrokeColor
    {
        get { return (Color)GetValue(UnplayedTextStrokeColorProperty); }
        set { SetValue(UnplayedTextStrokeColorProperty, value); }
    }

    public static readonly DependencyProperty UnplayedTextStrokeColorProperty =
        DependencyProperty.Register(nameof(UnplayedTextStrokeColor), typeof(Color), typeof(BetterLyric), new PropertyMetadata(Colors.White, OnUnplayedTextStrokeColorChanged));

    private static void OnUnplayedTextStrokeColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.unplayedTextStrokeColor = (Color)e.NewValue;
    }
   
    public LyricFontWeight LyricsFontWeight
    {
        get { return (LyricFontWeight)GetValue(LyricsFontWeightProperty); }
        set { SetValue(LyricsFontWeightProperty, value); }
    }

    public static readonly DependencyProperty LyricsFontWeightProperty =
        DependencyProperty.Register(nameof(LyricsFontWeight), typeof(LyricFontWeight), typeof(BetterLyric), new PropertyMetadata(LyricFontWeight.Bold, OnLyricsFontWeightChanged));

    private static void OnLyricsFontWeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsFontWeight = (LyricFontWeight)e.NewValue;
        ctl.RequestRelayout();
    }


    public double LyricsLineOverallSpacingFactor
    {
        get { return (double)GetValue(LyricsLineOverallSpacingFactorProperty); }
        set { SetValue(LyricsLineOverallSpacingFactorProperty, value); }
    }

    public static readonly DependencyProperty LyricsLineOverallSpacingFactorProperty =
        DependencyProperty.Register(nameof(LyricsLineOverallSpacingFactor), typeof(double), typeof(BetterLyric), new PropertyMetadata(0.5, OnLyricsLineOverallSpacingFactorChanged));

    private static void OnLyricsLineOverallSpacingFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsLineOverallSpacingFactor = (double)e.NewValue;
        ctl.RequestRelayout();
    }


    public double LyricsLineInnerSpacingFactor
    {
        get { return (double)GetValue(LyricsLineInnerSpacingFactorProperty); }
        set { SetValue(LyricsLineInnerSpacingFactorProperty, value); }
    }

    public static readonly DependencyProperty LyricsLineInnerSpacingFactorProperty =
        DependencyProperty.Register(nameof(LyricsLineInnerSpacingFactor), typeof(double), typeof(BetterLyric), new PropertyMetadata(0.1, OnLyricsLineInnerSpacingFactorChanged));

    private static void OnLyricsLineInnerSpacingFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsLineInnerSpacingFactor = (double)e.NewValue;
        ctl.RequestRelayout();
    }


    public string LyricsWesternFontFamily
    {
        get { return (string)GetValue(LyricsWesternFontFamilyProperty); }
        set { SetValue(LyricsWesternFontFamilyProperty, value); }
    }

    public static readonly DependencyProperty LyricsWesternFontFamilyProperty =
        DependencyProperty.Register(nameof(LyricsWesternFontFamily), typeof(string), typeof(BetterLyric), new PropertyMetadata("Arial", OnLyricsWesternFontFamilyChanged));

    private static void OnLyricsWesternFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsWesternFontFamily = (string)e.NewValue;
        ctl.RequestRelayout();
    }


    public string LyricsCJKFontFamily
    {
        get { return (string)GetValue(LyricsCJKFontFamilyProperty); }
        set { SetValue(LyricsCJKFontFamilyProperty, value); }
    }

    public static readonly DependencyProperty LyricsCJKFontFamilyProperty =
        DependencyProperty.Register(nameof(LyricsCJKFontFamily), typeof(string), typeof(BetterLyric), new PropertyMetadata("Arial", OnLyricsCJKFontFamilyChanged));

    private static void OnLyricsCJKFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.lyricsCJKFontFamily = (string)e.NewValue;
        ctl.RequestRelayout();
    }


    public bool IsRightToLeftLyric
    {
        get { return (bool)GetValue(IsRightToLeftLyricProperty); }
        set { SetValue(IsRightToLeftLyricProperty, value); }
    }

    public static readonly DependencyProperty IsRightToLeftLyricProperty =
        DependencyProperty.Register(nameof(IsRightToLeftLyric), typeof(bool), typeof(BetterLyric), new PropertyMetadata(false, OnIsRightToLeftLyricChanged));

    private static void OnIsRightToLeftLyricChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isRightToLeftLyric = (bool)e.NewValue;
        ctl.RequestRelayout();
    }


    public int PlayingLineTopOffset
    {
        get { return (int)GetValue(PlayingLineTopOffsetProperty); }
        set { SetValue(PlayingLineTopOffsetProperty, value); }
    }

    public static readonly DependencyProperty PlayingLineTopOffsetProperty =
        DependencyProperty.Register(nameof(PlayingLineTopOffset), typeof(int), typeof(BetterLyric), new PropertyMetadata(50, OnPlayingLineTopOffsetChanged));

    private static void OnPlayingLineTopOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.playingLineTopOffset = (int)e.NewValue;
        ctl.RequestRelayout();
    }
}
