using System.ComponentModel;

namespace DevWinUI;


[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
public partial class IsEnabledTextBlock : Control
{
    public IsEnabledTextBlock()
    {
        this.DefaultStyleKey = typeof(IsEnabledTextBlock);
    }

    protected override void OnApplyTemplate()
    {
        IsEnabledChanged -= IsEnabledTextBlock_IsEnabledChanged;
        SetEnabledState();
        IsEnabledChanged += IsEnabledTextBlock_IsEnabledChanged;
        base.OnApplyTemplate();
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(IsEnabledTextBlock), new PropertyMetadata(null));

    [Localizable(true)]
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty IsTextSelectionEnabledProperty = DependencyProperty.Register(nameof(IsTextSelectionEnabled), typeof(bool), typeof(IsEnabledTextBlock), new PropertyMetadata(false));

    public bool IsTextSelectionEnabled
    {
        get => (bool)GetValue(IsTextSelectionEnabledProperty);
        set => SetValue(IsTextSelectionEnabledProperty, value);
    }

    private void IsEnabledTextBlock_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        SetEnabledState();
    }

    private void SetEnabledState()
    {
        VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", true);
    }
}
