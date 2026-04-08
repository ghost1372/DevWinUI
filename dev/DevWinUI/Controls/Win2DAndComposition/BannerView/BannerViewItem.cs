using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

public partial class BannerViewItem : FlipViewItem
{
    private const string ShadowHost = "ShadowHost";
    private const string BackgroundRect = "BackgroundRect";
    private Canvas shadowHost;
    private Rectangle backgroundRect;

    private DropShadow dropShadow;
    private ImplicitAnimationCollection imps;
    internal Compositor Compositor;
    private bool isCycleItemContainer;

    public bool IsCycleItemContainer
    {
        get => isCycleItemContainer;
        set
        {
            isCycleItemContainer = value;
            UpdateShadowAnimation();
        }
    }

    public double RadiusX
    {
        get { return (double)GetValue(RadiusXProperty); }
        set { SetValue(RadiusXProperty, value); }
    }

    public static readonly DependencyProperty RadiusXProperty =
        DependencyProperty.Register(nameof(RadiusX), typeof(double), typeof(BannerViewItem), new PropertyMetadata(8.0d));

    public double RadiusY
    {
        get { return (double)GetValue(RadiusYProperty); }
        set { SetValue(RadiusYProperty, value); }
    }

    public static readonly DependencyProperty RadiusYProperty =
        DependencyProperty.Register(nameof(RadiusY), typeof(double), typeof(BannerViewItem), new PropertyMetadata(8.0d));

    public BannerViewItem()
    {
        this.DefaultStyleKey = typeof(BannerViewItem);
        RegisterPropertyChangedCallback(FlipViewItem.IsSelectedProperty, IsSelectedPropertyChanged);
        this.SizeChanged += (s, a) => UpdateShadow();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        shadowHost = GetTemplateChild(ShadowHost) as Canvas;
        backgroundRect = GetTemplateChild(BackgroundRect) as Rectangle;

        InitComposition();
    }

    private void IsSelectedPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (dropShadow != null)
        {
            dropShadow.BlurRadius = IsSelected ? 8f : 0f;
        }
    }

    private void InitComposition()
    {
        Compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        var visual = Compositor.CreateSpriteVisual();
        var sizebind = Compositor.CreateExpressionAnimation("rect.Size");
        sizebind.SetReferenceParameter("rect", ElementCompositionPreview.GetElementVisual(backgroundRect));
        visual.StartAnimation("Size", sizebind);

        dropShadow = Compositor.CreateDropShadow();
        dropShadow.Color = Colors.Black;
        dropShadow.Opacity = 1f;
        dropShadow.Offset = new Vector3(0f, 2f, 0f);
        dropShadow.BlurRadius = IsSelected ? 8f : 0f;

        imps = Compositor.CreateImplicitAnimationCollection();
        var blur_an = Compositor.CreateScalarKeyFrameAnimation();
        blur_an.InsertExpressionKeyFrame(1f, "this.FinalValue");
        blur_an.Duration = TimeSpan.FromSeconds(0.2d);
        blur_an.Target = "BlurRadius";
        imps["BlurRadius"] = blur_an;

        visual.Shadow = dropShadow;

        ElementCompositionPreview.SetElementChildVisual(shadowHost, visual);
        UpdateShadow();
        UpdateShadowAnimation();
    }

    private void UpdateShadowAnimation()
    {
        if (imps == null) return;
        if (IsCycleItemContainer)
        {
            dropShadow.ImplicitAnimations = null;
        }
        else
        {
            dropShadow.ImplicitAnimations = imps;
        }
    }

    private void UpdateShadow()
    {
        if (dropShadow == null) return;
        dropShadow.Mask = backgroundRect.GetAlphaMask();
    }
}
