using Windows.System;

namespace DevWinUI;

public partial class GoToCard : Control
{
    private const string PART_Button = "PART_Button";
    private const string PART_CloseButton = "PART_CloseButton";
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

    public bool ShowCloseButton
    {
        get { return (bool)GetValue(ShowCloseButtonProperty); }
        set { SetValue(ShowCloseButtonProperty, value); }
    }

    public static readonly DependencyProperty ShowCloseButtonProperty =
        DependencyProperty.Register(nameof(ShowCloseButton), typeof(bool), typeof(GoToCard), new PropertyMetadata(true));

    public string LaunchUri
    {
        get { return (string)GetValue(LaunchUriProperty); }
        set { SetValue(LaunchUriProperty, value); }
    }

    public static readonly DependencyProperty LaunchUriProperty =
        DependencyProperty.Register(nameof(LaunchUri), typeof(string), typeof(GoToCard), new PropertyMetadata(null));

    public bool IsOpen
    {
        get { return (bool)GetValue(IsOpenProperty); }
        set { SetValue(IsOpenProperty, value); }
    }

    public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(GoToCard), new PropertyMetadata(true, OnIsOpenChanged));

    private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (GoToCard)d;
        if (ctl != null)
        {
            ctl.OnIsOpenChanged();
        }
    }

    public event EventHandler<RoutedEventArgs> ActionClick;
    public event EventHandler<RoutedEventArgs> CloseRequested;

    public GoToCard()
    {
        DefaultStyleKey = typeof(GoToCard);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        var btn = GetTemplateChild(PART_Button) as Button;
        btn.Click -= OnButtonClicked;
        btn.Click += OnButtonClicked;

        var btnClose = GetTemplateChild(PART_CloseButton) as Button;
        btnClose.Click -= OnCloseButtonClicked;
        btnClose.Click += OnCloseButtonClicked;
    }
    private void OnIsOpenChanged()
    {
        Visibility = IsOpen ? Visibility.Visible : Visibility.Collapsed;
    }
    private async void OnButtonClicked(object sender, RoutedEventArgs e)
    {
        ActionClick?.Invoke(this, e);

        if (!string.IsNullOrEmpty(LaunchUri))
        {
            _ = await Launcher.LaunchUriAsync(new Uri(LaunchUri));
        }
    }
    private async void OnCloseButtonClicked(object sender, RoutedEventArgs e)
    {
        CloseRequested?.Invoke(this, e);
        IsOpen = false;
    }
}
