namespace DevWinUI;

/// <summary>
/// Displays an optional icon, title and subtitle. Each part is hidden while its value is null.
/// </summary>
public partial class Empty : Control
{
    public IconElement Icon
    {
        get => (IconElement)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(IconElement), typeof(Empty), new PropertyMetadata(null));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(Empty), new PropertyMetadata(null));

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(Empty), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets whether the empty state is shown. The control is collapsed while false.
    /// </summary>
    public bool IsEmpty
    {
        get => (bool)GetValue(IsEmptyProperty);
        set => SetValue(IsEmptyProperty, value);
    }

    public static readonly DependencyProperty IsEmptyProperty =
        DependencyProperty.Register(nameof(IsEmpty), typeof(bool), typeof(Empty), new PropertyMetadata(true, OnIsEmptyChanged));

    private static void OnIsEmptyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Empty)d).UpdateVisibility();
    }

    public Empty()
    {
        this.DefaultStyleKey = typeof(Empty);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        this.Visibility = IsEmpty ? Visibility.Visible : Visibility.Collapsed;
    }
}
