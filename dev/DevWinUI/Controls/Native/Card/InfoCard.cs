namespace DevWinUI;

public partial class InfoCard : Control
{
    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(InfoCard), new PropertyMetadata(null));

    public string Description
    {
        get { return (string)GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(InfoCard), new PropertyMetadata(null));

    public string Icon
    {
        get { return (string)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(string), typeof(InfoCard), new PropertyMetadata(null));

    public InfoBadge InfoBadge
    {
        get { return (InfoBadge)GetValue(InfoBadgeProperty); }
        set { SetValue(InfoBadgeProperty, value); }
    }

    public static readonly DependencyProperty InfoBadgeProperty =
        DependencyProperty.Register(nameof(InfoBadge), typeof(InfoBadge), typeof(InfoCard), new PropertyMetadata(null));

    public InfoCard()
    {
        DefaultStyleKey = typeof(InfoCard);
    }
}
