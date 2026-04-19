using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Header), Type = typeof(Grid))]
[TemplatePart(Name = nameof(PART_BackgroundRectangle), Type = typeof(Rectangle))]
[TemplatePart(Name = nameof(PART_OverlayRectangle), Type = typeof(Rectangle))]
[TemplatePart(Name = nameof(PART_Profile), Type = typeof(ContentPresenter))]
[TemplatePart(Name = nameof(PART_Subtitle), Type = typeof(ContentPresenter))]
[TemplatePart(Name = nameof(PART_Description), Type = typeof(ContentPresenter))]
[TemplatePart(Name = nameof(PART_TextContainer), Type = typeof(StackPanel))]
[TemplatePart(Name = nameof(PART_Footer), Type = typeof(ContentPresenter))]
public partial class ShyHeader : Control
{
    private const string PART_Header = "PART_Header";
    private const string PART_BackgroundRectangle = "PART_BackgroundRectangle";
    private const string PART_OverlayRectangle = "PART_OverlayRectangle";
    private const string PART_Profile = "PART_Profile";
    private const string PART_Subtitle = "PART_Subtitle";
    private const string PART_Description = "PART_Description";
    private const string PART_TextContainer = "PART_TextContainer";
    private const string PART_Footer = "PART_Footer";

    private Grid _headerGrid;
    private Rectangle _backgroundRectangle;
    private Rectangle _overlayRectangle;
    private ContentPresenter _profilePresenter;
    private ContentPresenter _subtitlePresenter;
    private ContentPresenter _descriptionPresenter;
    private StackPanel _textContainer;
    private ContentPresenter _footerPresenter;

    private SpriteVisual _blurredBackgroundImageVisual;
    private CompositionPropertySet _props;
    private CompositionPropertySet _scrollerPropertySet;
    private Compositor _compositor;

    public Brush OverlayBrush
    {
        get { return (Brush)GetValue(OverlayBrushProperty); }
        set { SetValue(OverlayBrushProperty, value); }
    }

    public static readonly DependencyProperty OverlayBrushProperty =
        DependencyProperty.Register(nameof(OverlayBrush), typeof(Brush), typeof(ShyHeader), new PropertyMetadata(null));


    public double BackgroundImageOpacity
    {
        get { return (double)GetValue(BackgroundImageOpacityProperty); }
        set { SetValue(BackgroundImageOpacityProperty, value); }
    }

    public static readonly DependencyProperty BackgroundImageOpacityProperty =
        DependencyProperty.Register(nameof(BackgroundImageOpacity), typeof(double), typeof(ShyHeader), new PropertyMetadata(0.6));

    public object Title
    {
        get { return (object)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(object), typeof(ShyHeader), new PropertyMetadata(null, OnVisualChanged));

    private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ShyHeader)d;
        if (ctl != null)
        {
            ctl.SetupCompositionAnimations();
        }
    }

    public object Subtitle
    {
        get => (object)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }
    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(nameof(Subtitle), typeof(object), typeof(ShyHeader), new PropertyMetadata(null, OnVisualChanged));

    public object Description
    {
        get => (object)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(object), typeof(ShyHeader), new PropertyMetadata(null, OnVisualChanged));

    public ImageSource BackgroundImage
    {
        get => (ImageSource)GetValue(BackgroundImageProperty);
        set => SetValue(BackgroundImageProperty, value);
    }
    public static readonly DependencyProperty BackgroundImageProperty =
        DependencyProperty.Register(nameof(BackgroundImage), typeof(ImageSource), typeof(ShyHeader), new PropertyMetadata(null));

    public object Profile
    {
        get => (object)GetValue(ProfileImageProperty);
        set => SetValue(ProfileImageProperty, value);
    }
    public static readonly DependencyProperty ProfileImageProperty =
        DependencyProperty.Register(nameof(Profile), typeof(object), typeof(ShyHeader), new PropertyMetadata(null, OnVisualChanged));

    public object Footer
    {
        get => (object)GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }
    public static readonly DependencyProperty FooterProperty =
        DependencyProperty.Register(nameof(Footer), typeof(object), typeof(ShyHeader), new PropertyMetadata(null, OnVisualChanged));

    public ShyHeader()
    {
        this.DefaultStyleKey = typeof(ShyHeader);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _headerGrid = GetTemplateChild(PART_Header) as Grid;
        _backgroundRectangle = GetTemplateChild(PART_BackgroundRectangle) as Rectangle;
        _overlayRectangle = GetTemplateChild(PART_OverlayRectangle) as Rectangle;
        _profilePresenter = GetTemplateChild(PART_Profile) as ContentPresenter;
        _subtitlePresenter = GetTemplateChild(PART_Subtitle) as ContentPresenter;
        _descriptionPresenter = GetTemplateChild(PART_Description) as ContentPresenter;
        _textContainer = GetTemplateChild(PART_TextContainer) as StackPanel;
        _footerPresenter = GetTemplateChild(PART_Footer) as ContentPresenter;

        SetupCompositionAnimations();
        SetZIndex();

        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;
    }

    private void SetZIndex()
    {
        var parent = VisualTreeHelper.GetParent(this) as UIElement;
        var container = parent != null ? VisualTreeHelper.GetParent(parent) as UIElement : null;

        if (container != null)
        {
            Canvas.SetZIndex(container, 1);
        }
    }
    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_blurredBackgroundImageVisual != null && _overlayRectangle != null)
        {
            _blurredBackgroundImageVisual.Size = new Vector2(
                (float)_overlayRectangle.ActualWidth,
                (float)_overlayRectangle.ActualHeight);
        }
    }

    private void SetupCompositionAnimations()
    {
        var scrollViewer = this.FindAscendant<ScrollViewer>();
        if (scrollViewer == null) return;

        _scrollerPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);
        _compositor = _scrollerPropertySet.Compositor;

        _props = _compositor.CreatePropertySet();
        _props.InsertScalar("progress", 0);
        _props.InsertScalar("clampSize", 150);
        _props.InsertScalar("scaleFactor", 0.7f);

        var blurEffect = new GaussianBlurEffect
        {
            Name = "blur",
            BlurAmount = 0.0f,
            BorderMode = EffectBorderMode.Hard,
            Source = new CompositionEffectSourceParameter("source")
        };

        var blurBrush = _compositor.CreateEffectFactory(blurEffect, new[] { "blur.BlurAmount" }).CreateBrush();
        blurBrush.SetSourceParameter("source", _compositor.CreateBackdropBrush());

        _blurredBackgroundImageVisual = _compositor.CreateSpriteVisual();
        _blurredBackgroundImageVisual.Brush = blurBrush;
        _blurredBackgroundImageVisual.Size = new Vector2((float)_overlayRectangle.ActualWidth, (float)_overlayRectangle.ActualHeight);
        ElementCompositionPreview.SetElementChildVisual(_overlayRectangle, _blurredBackgroundImageVisual);

        var progressAnimation = _compositor.CreateExpressionAnimation("Clamp(-scroll.Translation.Y / props.clampSize, 0, 1)");
        progressAnimation.SetReferenceParameter("scroll", _scrollerPropertySet);
        progressAnimation.SetReferenceParameter("props", _props);
        _props.StartAnimation("progress", progressAnimation);

        var blurAnimation = _compositor.CreateExpressionAnimation("Lerp(0, 15, props.progress)");
        blurAnimation.SetReferenceParameter("props", _props);
        _blurredBackgroundImageVisual.Brush.Properties.StartAnimation("blur.BlurAmount", blurAnimation);

        var headerVisual = ElementCompositionPreview.GetElementVisual(_headerGrid);
        var headerTranslationAnimation = _compositor.CreateExpressionAnimation("props.progress < 1 ? 0 : -scroll.Translation.Y - props.clampSize");
        headerTranslationAnimation.SetReferenceParameter("scroll", _scrollerPropertySet);
        headerTranslationAnimation.SetReferenceParameter("props", _props);
        headerVisual.StartAnimation("Offset.Y", headerTranslationAnimation);

        var headerScaleAnimation = _compositor.CreateExpressionAnimation("Lerp(1, 1.25, Clamp(scroll.Translation.Y / 50, 0, 1))");
        headerScaleAnimation.SetReferenceParameter("scroll", _scrollerPropertySet);
        headerVisual.StartAnimation("Scale.X", headerScaleAnimation);
        headerVisual.StartAnimation("Scale.Y", headerScaleAnimation);
        headerVisual.CenterPoint = new Vector3((float)(_headerGrid.ActualWidth / 2), (float)_headerGrid.ActualHeight, 0);

        var photoVisual = ElementCompositionPreview.GetElementVisual(_backgroundRectangle);
        var photoOpacityAnimation = _compositor.CreateExpressionAnimation("1 - props.progress");
        photoOpacityAnimation.SetReferenceParameter("props", _props);
        photoVisual.StartAnimation("Opacity", photoOpacityAnimation);

        var profileVisual = ElementCompositionPreview.GetElementVisual(_profilePresenter);
        var profileScaleAnimation = _compositor.CreateExpressionAnimation("Lerp(1, props.scaleFactor, props.progress)");
        profileScaleAnimation.SetReferenceParameter("props", _props);
        profileVisual.StartAnimation("Scale.X", profileScaleAnimation);
        profileVisual.StartAnimation("Scale.Y", profileScaleAnimation);

        var descriptionVisual = ElementCompositionPreview.GetElementVisual(_descriptionPresenter);
        var subtitleVisual = ElementCompositionPreview.GetElementVisual(_subtitlePresenter);

        var textOpacityAnimation = _compositor.CreateExpressionAnimation("Clamp(1 - (props.progress * 2), 0, 1)");
        textOpacityAnimation.SetReferenceParameter("props", _props);
        descriptionVisual.StartAnimation("Opacity", textOpacityAnimation);
        subtitleVisual.StartAnimation("Opacity", textOpacityAnimation);

        descriptionVisual.StartAnimation("Scale.X", profileScaleAnimation);
        descriptionVisual.StartAnimation("Scale.Y", profileScaleAnimation);
        subtitleVisual.StartAnimation("Scale.X", profileScaleAnimation);
        subtitleVisual.StartAnimation("Scale.Y", profileScaleAnimation);

        var textVisual = ElementCompositionPreview.GetElementVisual(_textContainer);
        var footerVisual = ElementCompositionPreview.GetElementVisual(_footerPresenter);

        var textOffsetAnimation = _compositor.CreateExpressionAnimation("props.progress * 100");
        textOffsetAnimation.SetReferenceParameter("props", _props);
        textVisual.StartAnimation("Offset.Y", textOffsetAnimation);

        var footerOffsetAnimation = _compositor.CreateExpressionAnimation("props.progress * -100");
        footerOffsetAnimation.SetReferenceParameter("props", _props);
        footerVisual.StartAnimation("Offset.Y", footerOffsetAnimation);
    }
}
