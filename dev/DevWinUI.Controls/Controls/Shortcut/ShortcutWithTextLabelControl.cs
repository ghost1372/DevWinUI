namespace DevWinUI;
public partial class ShortcutWithTextLabel : Control
{
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(ShortcutWithTextLabel), new PropertyMetadata(default(string)));

    public List<object> Keys
    {
        get => (List<object>)GetValue(KeysProperty);
        set => SetValue(KeysProperty, value);
    }

    public static readonly DependencyProperty KeysProperty =
        DependencyProperty.Register(nameof(Keys), typeof(List<object>), typeof(ShortcutWithTextLabel), new PropertyMetadata(default(string)));

}
