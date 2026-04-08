using Windows.System;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_HyperLink), Type = typeof(HyperlinkButton))]
public partial class HeaderTile : Control
{
    private const string PART_HyperLink = "PART_HyperLink";
    private HyperlinkButton _HyperLinkButton;

    public event EventHandler<RoutedEventArgs> OnItemClick;

    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(HeaderTile), new PropertyMetadata(null));

    public string Description
    {
        get { return (string)GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(HeaderTile), new PropertyMetadata(null));

    public object Source
    {
        get { return (object)GetValue(SourceProperty); }
        set { SetValue(SourceProperty, value); }
    }

    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source), typeof(object), typeof(HeaderTile), new PropertyMetadata(null));

    public string Link
    {
        get { return (string)GetValue(LinkProperty); }
        set { SetValue(LinkProperty, value); }
    }

    public static readonly DependencyProperty LinkProperty =
        DependencyProperty.Register(nameof(Link), typeof(string), typeof(HeaderTile), new PropertyMetadata(null));

    public HeaderTile()
    {
        DefaultStyleKey = typeof(HeaderTile);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _HyperLinkButton = GetTemplateChild(PART_HyperLink) as HyperlinkButton;

        _HyperLinkButton.Click -= OnHyperLinkButtonClick;
        _HyperLinkButton.Click += OnHyperLinkButtonClick;
    }

    private async void OnHyperLinkButtonClick(object sender, RoutedEventArgs e)
    {
        OnItemClick?.Invoke(sender, e);
        if (Link != null)
        {
            await Launcher.LaunchUriAsync(new Uri(Link));
        }
    }
}
