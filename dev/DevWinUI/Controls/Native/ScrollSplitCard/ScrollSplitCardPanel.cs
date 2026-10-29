namespace DevWinUI;

public sealed class ScrollSplitCardPanel : Control
{
    public ScrollSplitCardItem? Item
    {
        get { return (ScrollSplitCardItem?)GetValue(ItemProperty); }
        set { SetValue(ItemProperty, value); }
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(nameof(Item), typeof(ScrollSplitCardItem), typeof(ScrollSplitCardPanel), new PropertyMetadata(null, (d, _) => ((ScrollSplitCardPanel)d).Apply()));

    public ScrollSplitCardTone Tone
    {
        get { return (ScrollSplitCardTone)GetValue(ToneProperty); }
        set { SetValue(ToneProperty, value); }
    }

    public static readonly DependencyProperty ToneProperty =
        DependencyProperty.Register(nameof(Tone), typeof(ScrollSplitCardTone), typeof(ScrollSplitCardPanel), new PropertyMetadata(ScrollSplitCardTone.Card, (d, _) => ((ScrollSplitCardPanel)d).Apply()));

    public int Index
    {
        get { return (int)GetValue(IndexProperty); }
        set { SetValue(IndexProperty, value); }
    }

    public static readonly DependencyProperty IndexProperty =
        DependencyProperty.Register(nameof(Index), typeof(int), typeof(ScrollSplitCardPanel), new PropertyMetadata(0, (d, _) => ((ScrollSplitCardPanel)d).Apply()));

    public double TitleFontSize
    {
        get { return (double)GetValue(TitleFontSizeProperty); }
        set { SetValue(TitleFontSizeProperty, value); }
    }

    public static readonly DependencyProperty TitleFontSizeProperty =
        DependencyProperty.Register(nameof(TitleFontSize), typeof(double), typeof(ScrollSplitCardPanel), new PropertyMetadata(16.0));

    public double BodyFontSize
    {
        get { return (double)GetValue(BodyFontSizeProperty); }
        set { SetValue(BodyFontSizeProperty, value); }
    }

    public static readonly DependencyProperty BodyFontSizeProperty =
        DependencyProperty.Register(nameof(BodyFontSize), typeof(double), typeof(ScrollSplitCardPanel), new PropertyMetadata(12.5));

    static readonly string[] PaletteStates = { "Palette0", "Palette1", "Palette2" };

    internal readonly CompositeTransform PoseTransform = new();
    internal readonly PlaneProjection FrontProjection = new();
    internal readonly PlaneProjection BackProjection = new() { RotationY = -180 };
    internal readonly CompositeTransform ImageTransform = new();

    readonly ImageBrush _imageBrush;

    Border _front = null!;
    Border _image = null!;
    Border _back = null!;
    TextBlock _eyebrow = null!;
    TextBlock _title = null!;
    TextBlock _description = null!;
    BitmapImage? _bitmap;
    CornerRadius _corners;
    bool _templated, _faceUp;

    public ScrollSplitCardPanel()
    {
        DefaultStyleKey = typeof(ScrollSplitCardPanel);
        RenderTransformOrigin = new Point(0.5, 0.5);
        RenderTransform = PoseTransform;
        _imageBrush = new ImageBrush { Stretch = Stretch.Fill, RelativeTransform = ImageTransform };
    }

    internal bool FaceUp
    {
        get { return _faceUp; }
        set
        {
            if (_faceUp == value) return;
            _faceUp = value;
            ApplyFace();
        }
    }

    internal CornerRadius FrontCorners
    {
        get { return _corners; }
        set
        {
            if (_corners.Equals(value)) return;
            _corners = value;
            ApplyCorners();
        }
    }

    internal void SetImage(BitmapImage? bitmap)
    {
        _bitmap = bitmap;
        ApplyImage();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _front = (Border)GetTemplateChild("PART_Front");
        _image = (Border)GetTemplateChild("PART_Image");
        _back = (Border)GetTemplateChild("PART_Back");
        _eyebrow = (TextBlock)GetTemplateChild("PART_Eyebrow");
        _title = (TextBlock)GetTemplateChild("PART_Title");
        _description = (TextBlock)GetTemplateChild("PART_Description");

        _front.Projection = FrontProjection;
        _back.Projection = BackProjection;
        _templated = true;

        ApplyImage();
        ApplyFace();
        ApplyCorners();
        Apply();
    }

    void ApplyImage()
    {
        if (!_templated) return;

        _imageBrush.ImageSource = _bitmap;
        _image.Background = _bitmap is null ? null : _imageBrush;
    }

    void ApplyFace()
    {
        if (!_templated) return;

        _front.Opacity = _faceUp ? 0 : 1;
        _back.Opacity = _faceUp ? 1 : 0;
    }

    void ApplyCorners()
    {
        if (!_templated) return;

        _front.CornerRadius = _corners;
        _image.CornerRadius = _corners;
    }

    void Apply()
    {
        if (!_templated) return;

        int palette = (Index % PaletteStates.Length + PaletteStates.Length) % PaletteStates.Length;
        VisualStateManager.GoToState(this, Tone.ToString(), false);
        VisualStateManager.GoToState(this, Tone == ScrollSplitCardTone.Chart ? PaletteStates[palette] : "NoPalette", false);

        if (Item is not { } item) return;

        _eyebrow.Text = (item.Eyebrow ?? string.Empty).ToUpperInvariant();
        _eyebrow.Visibility = string.IsNullOrEmpty(item.Eyebrow) ? Visibility.Collapsed : Visibility.Visible;
        _title.Text = item.Title;
        _description.Text = item.Description;
        _description.Visibility = string.IsNullOrEmpty(item.Description) ? Visibility.Collapsed : Visibility.Visible;
    }
}
