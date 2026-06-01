// https://github.com/jayfunc/BetterLyrics

namespace DevWinUI;

public partial class BetterLyric
{
    private bool isTranslationEnabled = true;
    private bool isPhoneticEnabled = true;

    public bool IsTranslationEnabled
    {
        get { return (bool)GetValue(IsTranslationEnabledProperty); }
        set { SetValue(IsTranslationEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsTranslationEnabledProperty =
        DependencyProperty.Register(nameof(IsTranslationEnabled), typeof(bool), typeof(BetterLyric), new PropertyMetadata(true, OnIsTranslationEnabledChanged));
    private static void OnIsTranslationEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isTranslationEnabled = (bool)e.NewValue;
    }

    public bool IsPhoneticEnabled
    {
        get { return (bool)GetValue(IsPhoneticEnabledProperty); }
        set { SetValue(IsPhoneticEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsPhoneticEnabledProperty =
        DependencyProperty.Register(nameof(IsPhoneticEnabled), typeof(bool), typeof(BetterLyric), new PropertyMetadata(false, OnIsPhoneticEnabledChanged));

    private static void OnIsPhoneticEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isPhoneticEnabled = (bool)e.NewValue;
    }
}
