namespace DevWinUI;

public sealed partial class KeyCharPresenter : Control
{
    public KeyCharPresenter()
    {
        DefaultStyleKey = typeof(KeyCharPresenter);
    }

    public object Content
    {
        get => (object)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(KeyCharPresenter), new PropertyMetadata(default(string)));
}
