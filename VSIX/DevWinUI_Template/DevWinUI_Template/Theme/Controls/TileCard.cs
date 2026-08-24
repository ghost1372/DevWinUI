using System.Windows;

namespace DevWinUI_Template;

public class TileCard : Card
{
    static TileCard()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TileCard), new FrameworkPropertyMetadata(typeof(TileCard)));
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(TileCard),
        new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(
        nameof(Subtitle),
        typeof(string),
        typeof(TileCard),
        new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(object),
        typeof(TileCard),
        new PropertyMetadata(null));

    public static readonly DependencyProperty ActiveIconProperty = DependencyProperty.Register(
        nameof(ActiveIcon),
        typeof(object),
        typeof(TileCard),
        new PropertyMetadata(null));

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

    public object ActiveIcon
    {
        get => GetValue(ActiveIconProperty);
        set => SetValue(ActiveIconProperty, value);
    }
}
