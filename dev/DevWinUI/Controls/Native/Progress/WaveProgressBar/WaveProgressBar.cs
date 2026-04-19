namespace DevWinUI;

[TemplatePart(Name = ElementWave, Type = typeof(FrameworkElement))]
[TemplatePart(Name = ElementClip, Type = typeof(FrameworkElement))]
public partial class WaveProgressBar : Control
{
    private const string ElementWave = "PART_Wave";

    private const string ElementClip = "PART_Clip";

    private FrameworkElement _waveElement;

    private const double TranslateTransformMinY = -20;

    private double _translateTransformYRange;

    private TranslateTransform _translateTransform;

    public double Minimum
    {
        get { return (double)GetValue(MinimumProperty); }
        set { SetValue(MinimumProperty, value); }
    }

    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(WaveProgressBar), new PropertyMetadata(0.0, OnMinimumChanged));

    public double Maximum
    {
        get { return (double)GetValue(MaximumProperty); }
        set { SetValue(MaximumProperty, value); }
    }

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(WaveProgressBar), new PropertyMetadata(100.0, OnMaximumChanged));

    public double Value
    {
        get { return (double)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(double), typeof(WaveProgressBar), new PropertyMetadata(0.0, OnValueChanged));

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (WaveProgressBar)d;
        ctl.UpdateWave((double)e.NewValue);
    }

    private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (WaveProgressBar)d;
        ctl.UpdateWave(ctl.Value);
    }

    private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (WaveProgressBar)d;
        ctl.UpdateWave(ctl.Value);
    }

    public bool IsVerySmall(double value) => Math.Abs(value) < 1E-06;

    private void UpdateWave(double value)
    {
        if (_translateTransform == null || IsVerySmall(Maximum)) return;
        var scale = 1 - value / Maximum;
        var y = _translateTransformYRange * scale + TranslateTransformMinY;
        _translateTransform.Y = y;
        _waveElement.RenderTransform = new TransformGroup
        {
            Children = { _translateTransform }
        };
        VisualStateManager.GoToState(this, "Normal", false);
        VisualStateManager.GoToState(this, "Indeterminate", true);
    }

    public WaveProgressBar()
    {
        this.DefaultStyleKey = typeof(WaveProgressBar);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _waveElement = GetTemplateChild(ElementWave) as FrameworkElement;
        var clipElement = GetTemplateChild(ElementClip) as FrameworkElement;

        if (_waveElement != null && clipElement != null)
        {
            var clipElementHeight = clipElement.Height;

            _translateTransform = new TranslateTransform
            {
                Y = clipElementHeight
            };
            _translateTransformYRange = clipElementHeight - TranslateTransformMinY;
            _waveElement.RenderTransform = new TransformGroup
            {
                Children = { _translateTransform }
            };

            UpdateWave(Value);
        }
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text), typeof(string), typeof(WaveProgressBar), new PropertyMetadata(default(string)));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty ShowTextProperty = DependencyProperty.Register(
        nameof(ShowText), typeof(bool), typeof(WaveProgressBar), new PropertyMetadata(true));

    public bool ShowText
    {
        get => (bool)GetValue(ShowTextProperty);
        set => SetValue(ShowTextProperty, value);
    }

    public static readonly DependencyProperty WaveFillProperty = DependencyProperty.Register(
        nameof(WaveFill), typeof(Brush), typeof(WaveProgressBar), new PropertyMetadata(default(Brush)));

    public Brush WaveFill
    {
        get => (Brush)GetValue(WaveFillProperty);
        set => SetValue(WaveFillProperty, value);
    }

    public static readonly DependencyProperty WaveThicknessProperty = DependencyProperty.Register(
        nameof(WaveThickness), typeof(double), typeof(WaveProgressBar), new PropertyMetadata(0.0));

    public double WaveThickness
    {
        get => (double)GetValue(WaveThicknessProperty);
        set => SetValue(WaveThicknessProperty, value);
    }

    public static readonly DependencyProperty WaveStrokeProperty = DependencyProperty.Register(
        nameof(WaveStroke), typeof(Brush), typeof(WaveProgressBar), new PropertyMetadata(default(Brush)));

    public Brush WaveStroke
    {
        get => (Brush)GetValue(WaveStrokeProperty);
        set => SetValue(WaveStrokeProperty, value);
    }
}
