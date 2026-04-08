

namespace DevWinUI;

public partial class Countdown
{
    private int _countdownSeconds = 0;
    private string _text;
    private CountdownState _state = CountdownState.Normal;
    private CanvasTextFormat _textFormat;
    private int _maxSeconds = 30;
    private Color _textForeground = Colors.White;

    public Color TextForeground
    {
        get { return (Color)GetValue(TextForegroundProperty); }
        set { SetValue(TextForegroundProperty, value); }
    }

    public static readonly DependencyProperty TextForegroundProperty =
        DependencyProperty.Register(nameof(TextForeground), typeof(Color), typeof(Countdown), new PropertyMetadata(Colors.White, (s, e) => (s as Countdown)._textForeground = (Color)e.NewValue));

    public int CountdownSeconds
    {
        get => (int)GetValue(CountdownSecondsProperty);
        set => SetValue(CountdownSecondsProperty, value);
    }
    public static readonly DependencyProperty CountdownSecondsProperty =
        DependencyProperty.Register(nameof(CountdownSeconds), typeof(int), typeof(Countdown), new PropertyMetadata(0, (s, e) => (s as Countdown)._countdownSeconds = (int)e.NewValue));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(Countdown), new PropertyMetadata(null, (s, e) => (s as Countdown)._text = (string)e.NewValue));

    public CountdownState State
    {
        get => (CountdownState)GetValue(AnswerStateProperty);
        set => SetValue(AnswerStateProperty, value);
    }

    public static readonly DependencyProperty AnswerStateProperty =
        DependencyProperty.Register(nameof(State), typeof(CountdownState), typeof(Countdown), new PropertyMetadata(CountdownState.Normal, (s, e) => (s as Countdown)._state = (CountdownState)e.NewValue));

    public CanvasTextFormat TextFormat
    {
        get { return (CanvasTextFormat)GetValue(TextFormatProperty); }
        set { SetValue(TextFormatProperty, value); }
    }

    public static readonly DependencyProperty TextFormatProperty =
        DependencyProperty.Register(nameof(TextFormat), typeof(CanvasTextFormat), typeof(Countdown), new PropertyMetadata(null, (s, e) => (s as Countdown)._textFormat = (CanvasTextFormat)e.NewValue));

    public int MaxSeconds
    {
        get => (int)GetValue(MaxSecondsProperty);
        set => SetValue(MaxSecondsProperty, value);
    }

    public static readonly DependencyProperty MaxSecondsProperty =
        DependencyProperty.Register(nameof(MaxSeconds), typeof(int), typeof(Countdown), new PropertyMetadata(30, (s, e) => (s as Countdown)._maxSeconds = (int)e.NewValue));

    public CanvasLinearGradientBrush BorderStrokeGradiantBrush
    {
        get { return (CanvasLinearGradientBrush)GetValue(BorderStrokeGradiantBrushProperty); }
        set { SetValue(BorderStrokeGradiantBrushProperty, value); }
    }

    public static readonly DependencyProperty BorderStrokeGradiantBrushProperty =
        DependencyProperty.Register(nameof(BorderStrokeGradiantBrush), typeof(CanvasLinearGradientBrush), typeof(Countdown), new PropertyMetadata(null, OnPropertyChanged));

    public Color BorderStrokeColor
    {
        get { return (Color)GetValue(BorderStrokeColorProperty); }
        set { SetValue(BorderStrokeColorProperty, value); }
    }

    public static readonly DependencyProperty BorderStrokeColorProperty =
        DependencyProperty.Register(nameof(BorderStrokeColor), typeof(Color), typeof(Countdown), new PropertyMetadata(Color.FromArgb(0xFF, 0x6E, 0xEF, 0xF8), OnPropertyChanged));

    public Color SuccessBorderStrokeColor
    {
        get { return (Color)GetValue(SuccessBorderStrokeColorProperty); }
        set { SetValue(SuccessBorderStrokeColorProperty, value); }
    }

    public static readonly DependencyProperty SuccessBorderStrokeColorProperty =
        DependencyProperty.Register(nameof(SuccessBorderStrokeColor), typeof(Color), typeof(Countdown), new PropertyMetadata(Color.FromArgb(0xFF, 0x42, 0xC9, 0xC5), OnPropertyChanged));

    public Color ErrorBorderStrokeColor
    {
        get { return (Color)GetValue(ErrorBorderStrokeColorProperty); }
        set { SetValue(ErrorBorderStrokeColorProperty, value); }
    }

    public static readonly DependencyProperty ErrorBorderStrokeColorProperty =
        DependencyProperty.Register(nameof(ErrorBorderStrokeColor), typeof(Color), typeof(Countdown), new PropertyMetadata(Color.FromArgb(0xFF, 0xDE, 0x01, 0x99), OnPropertyChanged));

    public IList<RadialGradientStopData> BackgroundGradientStops
    {
        get => (IList<RadialGradientStopData>)GetValue(BackgroundGradientStopsProperty);
        set => SetValue(BackgroundGradientStopsProperty, value);
    }

    public static readonly DependencyProperty BackgroundGradientStopsProperty =
        DependencyProperty.Register(nameof(BackgroundGradientStops), typeof(IList<RadialGradientStopData>), typeof(Countdown), new PropertyMetadata(null, OnPropertyChanged));

    public IList<RadialGradientStopData> SuccessBackgroundGradientStops
    {
        get => (IList<RadialGradientStopData>)GetValue(SuccessBackgroundGradientStopsProperty);
        set => SetValue(SuccessBackgroundGradientStopsProperty, value);
    }

    public static readonly DependencyProperty SuccessBackgroundGradientStopsProperty =
        DependencyProperty.Register(nameof(SuccessBackgroundGradientStops), typeof(IList<RadialGradientStopData>), typeof(Countdown), new PropertyMetadata(null, OnPropertyChanged));

    public IList<RadialGradientStopData> ErrorBackgroundGradientStops
    {
        get => (IList<RadialGradientStopData>)GetValue(ErrorBackgroundGradientStopsProperty);
        set => SetValue(ErrorBackgroundGradientStopsProperty, value);
    }

    public static readonly DependencyProperty ErrorBackgroundGradientStopsProperty =
        DependencyProperty.Register(nameof(ErrorBackgroundGradientStops), typeof(IList<RadialGradientStopData>), typeof(Countdown), new PropertyMetadata(null, OnPropertyChanged));

    public Color ClearColor
    {
        get { return (Color)GetValue(ClearColorProperty); }
        set { SetValue(ClearColorProperty, value); }
    }

    public static readonly DependencyProperty ClearColorProperty =
        DependencyProperty.Register(nameof(ClearColor), typeof(Color), typeof(Countdown), new PropertyMetadata(Colors.Transparent, OnClearColorChanged));

    private static void OnClearColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (Countdown)d;
        if (ctl != null)
        {
            ctl.UpdateClearColor();
        }
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (Countdown)d;
        if (ctl != null)
        {
            ctl.UpdateCanvas();
        }
    }
}
