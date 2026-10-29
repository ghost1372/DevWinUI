namespace DevWinUI;

public sealed partial class Reasoning
{
    public bool Thinking
    {
        get { return (bool)GetValue(ThinkingProperty); }
        set { SetValue(ThinkingProperty, value); }
    }

    public static readonly DependencyProperty ThinkingProperty =
        DependencyProperty.Register(nameof(Thinking), typeof(bool), typeof(Reasoning), new PropertyMetadata(false, (d, _) => ((Reasoning)d).OnThinkingChanged()));

    public double Duration
    {
        get { return (double)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(double), typeof(Reasoning), new PropertyMetadata(double.NaN, (d, _) => ((Reasoning)d).UpdateLabel(true)));

    public bool? Open
    {
        get { return (bool?)GetValue(OpenProperty); }
        set { SetValue(OpenProperty, value); }
    }

    public static readonly DependencyProperty OpenProperty =
        DependencyProperty.Register(nameof(Open), typeof(bool?), typeof(Reasoning), new PropertyMetadata(null, (d, _) => ((Reasoning)d).ApplyOpen(true)));

    public bool DefaultOpen
    {
        get { return (bool)GetValue(DefaultOpenProperty); }
        set { SetValue(DefaultOpenProperty, value); }
    }

    public static readonly DependencyProperty DefaultOpenProperty =
        DependencyProperty.Register(nameof(DefaultOpen), typeof(bool), typeof(Reasoning), new PropertyMetadata(false));

    public ReasoningKind Kind
    {
        get { return (ReasoningKind)GetValue(KindProperty); }
        set { SetValue(KindProperty, value); }
    }

    public static readonly DependencyProperty KindProperty =
        DependencyProperty.Register(nameof(Kind), typeof(ReasoningKind), typeof(Reasoning), new PropertyMetadata(ReasoningKind.Outline, (d, _) => ((Reasoning)d).ApplyVariant()));

    public string ThinkingLabel
    {
        get { return (string)GetValue(ThinkingLabelProperty); }
        set { SetValue(ThinkingLabelProperty, value); }
    }

    public static readonly DependencyProperty ThinkingLabelProperty =
        DependencyProperty.Register(nameof(ThinkingLabel), typeof(string), typeof(Reasoning), new PropertyMetadata("Thinking", (d, _) => ((Reasoning)d).UpdateLabel(false)));

    public string CompletedLabel
    {
        get { return (string)GetValue(CompletedLabelProperty); }
        set { SetValue(CompletedLabelProperty, value); }
    }

    public static readonly DependencyProperty CompletedLabelProperty =
        DependencyProperty.Register(nameof(CompletedLabel), typeof(string), typeof(Reasoning), new PropertyMetadata("Thought", (d, _) => ((Reasoning)d).UpdateLabel(false)));

    public string CompletedDurationLabel
    {
        get { return (string)GetValue(CompletedDurationLabelProperty); }
        set { SetValue(CompletedDurationLabelProperty, value); }
    }

    public static readonly DependencyProperty CompletedDurationLabelProperty =
        DependencyProperty.Register(nameof(CompletedDurationLabel), typeof(string), typeof(Reasoning), new PropertyMetadata("Thought for {0}", (d, _) => ((Reasoning)d).UpdateLabel(false)));

    public object? Body
    {
        get { return GetValue(BodyProperty); }
        set { SetValue(BodyProperty, value); }
    }

    public static readonly DependencyProperty BodyProperty =
        DependencyProperty.Register(nameof(Body), typeof(object), typeof(Reasoning), new PropertyMetadata(null, (d, _) => ((Reasoning)d).OnBodyChanged()));

    public Func<double, string>? FormatDuration { get; set; }
}
