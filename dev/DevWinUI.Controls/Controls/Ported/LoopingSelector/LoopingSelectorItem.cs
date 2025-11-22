namespace DevWinUI;

public partial class LoopingSelectorItem : DependencyObject
{
    public string PrimaryText
    {
        get => (string)GetValue(PrimaryTextProperty);
        set => SetValue(PrimaryTextProperty, value);
    }

    public static DependencyProperty PrimaryTextProperty { get; } =
        DependencyProperty.Register(nameof(PrimaryText), typeof(string), typeof(LoopingSelectorItem), new PropertyMetadata("default"));

    public override string ToString()
    {
        return PrimaryText;
    }
}
