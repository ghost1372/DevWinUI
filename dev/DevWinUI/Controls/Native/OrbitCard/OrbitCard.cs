using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;

namespace DevWinUI;

public sealed partial class OrbitCard : ContentControl
{
    internal readonly CompositeTransform PoseTransform = new();
    internal Storyboard? Running;

    static readonly string[] PaletteStates = { "Palette0", "Palette1", "Palette2", "Palette3", "Palette4", "Palette5" };

    readonly ImageBrush _imageBrush = new() { Stretch = Stretch.UniformToFill };

    Border _image = null!;
    Border _initials = null!;
    TextBlock _initialsText = null!;
    Border _stat = null!;
    TextBlock _statText = null!;
    Button _link = null!;
    TextBlock _name = null!;
    TextBlock _role = null!;
    TextBlock _description = null!;
    Uri? _imageUri;
    int _paletteState = -1;
    bool _templated;
    bool _linkTabStop;

    public OrbitCard()
    {
        DefaultStyleKey = typeof(OrbitCard);
        RenderTransformOrigin = new Point(0.5, 0.5);
        RenderTransform = PoseTransform;
    }

    internal bool LinkTabStop
    {
        get { return _linkTabStop; }
        set
        {
            _linkTabStop = value;
            if (_templated) _link.IsTabStop = value;
        }
    }

    protected override void OnApplyTemplate()
    {
        if (_templated) _link.Click -= OnLinkClick;

        base.OnApplyTemplate();

        _image = (Border)GetTemplateChild("PART_Image");
        _initials = (Border)GetTemplateChild("PART_Initials");
        _initialsText = (TextBlock)GetTemplateChild("PART_InitialsText");
        _stat = (Border)GetTemplateChild("PART_Stat");
        _statText = (TextBlock)GetTemplateChild("PART_StatText");
        _link = (Button)GetTemplateChild("PART_Link");
        _name = (TextBlock)GetTemplateChild("PART_Name");
        _role = (TextBlock)GetTemplateChild("PART_Role");
        _description = (TextBlock)GetTemplateChild("PART_Description");

        _image.Background = _imageBrush;
        _link.Click += OnLinkClick;
        _templated = true;
        _paletteState = -1;
        Apply();
    }

    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);
        Apply();
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new OrbitCardPeer(this);

    void OnLinkClick(object sender, RoutedEventArgs e)
    {
        if (Item?.Href is { } href) _ = Windows.System.Launcher.LaunchUriAsync(href);
    }

    void Apply()
    {
        if (!_templated || Item is not { } it) return;

        AutomationProperties.SetName(this, string.IsNullOrEmpty(it.Role) ? it.Name : $"{it.Name}, {it.Role}");

        bool slot = Content is not null;
        bool image = !slot && it.Image is not null;

        _image.Visibility = image ? Visibility.Visible : Visibility.Collapsed;
        UpdateImage(image ? it.Image : null);

        _initials.Visibility = !slot && !image ? Visibility.Visible : Visibility.Collapsed;
        _initialsText.Text = it.GetInitials();

        int palette = (PaletteIndex % PaletteStates.Length + PaletteStates.Length) % PaletteStates.Length;
        if (palette != _paletteState)
        {
            _paletteState = palette;
            VisualStateManager.GoToState(this, PaletteStates[palette], false);
        }

        _stat.Visibility = string.IsNullOrEmpty(it.Stat) ? Visibility.Collapsed : Visibility.Visible;
        _statText.Text = it.Stat ?? string.Empty;

        _link.Visibility = it.Href is null ? Visibility.Collapsed : Visibility.Visible;
        _link.IsTabStop = _linkTabStop;
        AutomationProperties.SetName(_link, $"{LinkLabel} {it.Name}");

        _name.Text = it.Name;
        _role.Text = it.Role ?? string.Empty;
        _role.Visibility = string.IsNullOrEmpty(it.Role) ? Visibility.Collapsed : Visibility.Visible;
        _description.Text = it.Description ?? string.Empty;
        _description.Visibility = ShowDescription && !string.IsNullOrEmpty(it.Description) ? Visibility.Visible : Visibility.Collapsed;
    }

    void UpdateImage(Uri? uri)
    {
        if (Equals(_imageUri, uri)) return;
        _imageUri = uri;

        if (uri is null)
        {
            _imageBrush.ImageSource = null;
            return;
        }

        var source = new BitmapImage();
        if (!double.IsNaN(Width) && Width > 0)
            source.DecodePixelWidth = (int)Math.Ceiling(Width * (XamlRoot?.RasterizationScale ?? 1.0));
        source.UriSource = uri;
        _imageBrush.ImageSource = source;
    }
}
