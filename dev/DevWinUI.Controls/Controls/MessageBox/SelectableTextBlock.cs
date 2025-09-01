namespace DevWinUI;

internal sealed partial class SelectableTextBlock : ContentControl
{
    public SelectableTextBlock()
    {
        DefaultStyleKey = typeof(SelectableTextBlock);
    }

    public TextWrapping TextWrapping
    {
        get { return (TextWrapping)GetValue(TextWrappingProperty); }
        set { SetValue(TextWrappingProperty, value); }
    }

    public static readonly DependencyProperty TextWrappingProperty =
        DependencyProperty.Register(nameof(TextWrapping), typeof(TextWrapping), typeof(SelectableTextBlock), new PropertyMetadata(default(TextWrapping)));

    public string? Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(SelectableTextBlock),
        new PropertyMetadata(default(string), (d, e) =>
        {
            SelectableTextBlock self = (SelectableTextBlock)d;
            self.Content = e.NewValue;
        })
    );
}
