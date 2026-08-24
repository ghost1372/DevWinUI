using System.Windows;

namespace DevWinUI_Template;

public class RadioCard : Card
{
    static RadioCard()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(RadioCard), new FrameworkPropertyMetadata(typeof(RadioCard)));
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(RadioCard),
        new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(
        nameof(Subtitle),
        typeof(string),
        typeof(RadioCard),
        new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SeverityProperty = DependencyProperty.Register(
        nameof(Severity),
        typeof(RadioCardSeverity),
        typeof(RadioCard),
        new PropertyMetadata(RadioCardSeverity.Info));

    public static readonly DependencyProperty TagCornerRadiusProperty = DependencyProperty.Register(
        nameof(TagCornerRadius),
        typeof(CornerRadius),
        typeof(RadioCard),
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

    public RadioCardSeverity Severity
    {
        get => (RadioCardSeverity)GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }

    public CornerRadius TagCornerRadius
    {
        get => (CornerRadius)GetValue(TagCornerRadiusProperty);
        set => SetValue(TagCornerRadiusProperty, value);
    }
}
