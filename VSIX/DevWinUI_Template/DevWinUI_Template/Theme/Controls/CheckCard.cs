using System.Windows;

namespace DevWinUI_Template;

public class CheckCard : Card
{
    static CheckCard()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckCard), new FrameworkPropertyMetadata(typeof(CheckCard)));
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(CheckCard),
        new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(
        nameof(Subtitle),
        typeof(string),
        typeof(CheckCard),
        new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(object),
        typeof(CheckCard),
        new PropertyMetadata(null));

    public static readonly DependencyProperty TagCornerRadiusProperty = DependencyProperty.Register(
        nameof(TagCornerRadius),
        typeof(CornerRadius),
        typeof(CheckCard),
        new PropertyMetadata(default(CornerRadius)));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public object Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public CornerRadius TagCornerRadius
    {
        get => (CornerRadius)GetValue(TagCornerRadiusProperty);
        set => SetValue(TagCornerRadiusProperty, value);
    }
}
