using Windows.System;

namespace DevWinUI;

public partial class GoToCard : Control
{
    public IconSource Icon
    {
        get { return (IconSource)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(IconSource), typeof(GoToCard), new PropertyMetadata(null));

    public bool ShowIcon
    {
        get { return (bool)GetValue(ShowIconProperty); }
        set { SetValue(ShowIconProperty, value); }
    }

    public static readonly DependencyProperty ShowIconProperty =
        DependencyProperty.Register(nameof(ShowIcon), typeof(bool), typeof(GoToCard), new PropertyMetadata(true));

    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(GoToCard), new PropertyMetadata(null));

    public string ActionTitle
    {
        get { return (string)GetValue(ActionTitleProperty); }
        set { SetValue(ActionTitleProperty, value); }
    }

    public static readonly DependencyProperty ActionTitleProperty =
        DependencyProperty.Register(nameof(ActionTitle), typeof(string), typeof(GoToCard), new PropertyMetadata(null));

    public IconSource ActionIcon
    {
        get { return (IconSource)GetValue(ActionIconProperty); }
        set { SetValue(ActionIconProperty, value); }
    }

    public static readonly DependencyProperty ActionIconProperty =
        DependencyProperty.Register(nameof(ActionIcon), typeof(IconSource), typeof(GoToCard), new PropertyMetadata(new FontIconSource() { Glyph = "\uF0AF" }));

    public bool ShowActionIcon
    {
        get { return (bool)GetValue(ShowActionIconProperty); }
        set { SetValue(ShowActionIconProperty, value); }
    }

    public static readonly DependencyProperty ShowActionIconProperty =
        DependencyProperty.Register(nameof(ShowActionIcon), typeof(bool), typeof(GoToCard), new PropertyMetadata(true));

    public bool ShowActionButton
    {
        get { return (bool)GetValue(ShowActionButtonProperty); }
        set { SetValue(ShowActionButtonProperty, value); }
    }

    public static readonly DependencyProperty ShowActionButtonProperty =
        DependencyProperty.Register(nameof(ShowActionButton), typeof(bool), typeof(GoToCard), new PropertyMetadata(true));

    public string LaunchUri
    {
        get { return (string)GetValue(LaunchUriProperty); }
        set { SetValue(LaunchUriProperty, value); }
    }

    public static readonly DependencyProperty LaunchUriProperty =
        DependencyProperty.Register(nameof(LaunchUri), typeof(string), typeof(GoToCard), new PropertyMetadata(null));


    public EventHandler<RoutedEventArgs> ActionClick;

    public GoToCard()
    {
        DefaultStyleKey = typeof(GoToCard);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        var btn = GetTemplateChild("PART_Button") as Button;
        btn.Click -= OnButtonClicked;
        btn.Click += OnButtonClicked;
    }

    private async void OnButtonClicked(object sender, RoutedEventArgs e)
    {
        ActionClick?.Invoke(this, e);

        if (!string.IsNullOrEmpty(LaunchUri))
        {
            _ = await Launcher.LaunchUriAsync(new Uri(LaunchUri));
        }
    }
}
