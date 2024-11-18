using Windows.System;

namespace DevWinUI;
public sealed partial class HeaderTile : UserControl
{
    public event EventHandler<RoutedEventArgs> OnItemClick;

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
    public object Source
    {
        get => (object)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }
    public string Link
    {
        get => (string)GetValue(LinkProperty);
        set => SetValue(LinkProperty, value);
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(HeaderTile), new PropertyMetadata(null));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register("Description", typeof(string), typeof(HeaderTile), new PropertyMetadata(null));

    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register("Source", typeof(object), typeof(HeaderTile), new PropertyMetadata(null));

    public static readonly DependencyProperty LinkProperty =
        DependencyProperty.Register("Link", typeof(string), typeof(HeaderTile), new PropertyMetadata(null));

    public HeaderTile()
    {
        this.InitializeComponent();
    }

    private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        OnItemClick?.Invoke(sender, e);
        if (Link != null)
        {
            await Launcher.LaunchUriAsync(new Uri(Link));
        }
    }
}

