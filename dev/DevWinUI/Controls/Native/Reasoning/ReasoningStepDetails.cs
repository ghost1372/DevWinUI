using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[ContentProperty(Name = nameof(Text))]
public sealed partial class ReasoningStepDetails : Control
{
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(ReasoningStepDetails), new PropertyMetadata(""));

    public ReasoningStepDetails()
    {
        DefaultStyleKey = typeof(ReasoningStepDetails);
    }
}
