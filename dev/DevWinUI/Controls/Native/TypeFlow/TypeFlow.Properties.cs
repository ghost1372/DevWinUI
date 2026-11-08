namespace DevWinUI;

public sealed partial class TypeFlow
{
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(TypeFlow), new PropertyMetadata(string.Empty, OnTextChanged));

    public double BaseDelay
    {
        get { return (double)GetValue(BaseDelayProperty); }
        set { SetValue(BaseDelayProperty, value); }
    }

    public static readonly DependencyProperty BaseDelayProperty =
        DependencyProperty.Register(nameof(BaseDelay), typeof(double), typeof(TypeFlow), new PropertyMetadata(40.0));

    public double PunctuationDelay
    {
        get { return (double)GetValue(PunctuationDelayProperty); }
        set { SetValue(PunctuationDelayProperty, value); }
    }

    public static readonly DependencyProperty PunctuationDelayProperty =
        DependencyProperty.Register(nameof(PunctuationDelay), typeof(double), typeof(TypeFlow), new PropertyMetadata(260.0));

    public bool AutoStart
    {
        get { return (bool)GetValue(AutoStartProperty); }
        set { SetValue(AutoStartProperty, value); }
    }

    public static readonly DependencyProperty AutoStartProperty =
        DependencyProperty.Register(nameof(AutoStart), typeof(bool), typeof(TypeFlow), new PropertyMetadata(true));

    public TextWrapping TextWrapping
    {
        get { return (TextWrapping)GetValue(TextWrappingProperty); }
        set { SetValue(TextWrappingProperty, value); }
    }

    public static readonly DependencyProperty TextWrappingProperty =
        DependencyProperty.Register(nameof(TextWrapping), typeof(TextWrapping), typeof(TypeFlow), new PropertyMetadata(TextWrapping.Wrap));

    public TextAlignment TextAlignment
    {
        get { return (TextAlignment)GetValue(TextAlignmentProperty); }
        set { SetValue(TextAlignmentProperty, value); }
    }

    public static readonly DependencyProperty TextAlignmentProperty =
        DependencyProperty.Register(nameof(TextAlignment), typeof(TextAlignment), typeof(TypeFlow), new PropertyMetadata(TextAlignment.Left));

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (TypeFlow)d;
        if (!control.IsLoaded || control._body is null) return;

        if (control.AutoStart) control.Restart();
        else control.SetProgress(control.Text ?? string.Empty, 0);
    }
}
