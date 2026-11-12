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
            if (isCycleItemContainer == value)
                return;

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
        this.SizeChanged += OnSizeChanged;
        this.Loaded += OnLoaded;
        this.Unloaded += OnUnloaded;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        shadowHost = GetTemplateChild(ShadowHost) as Canvas;
        backgroundRect = GetTemplateChild(BackgroundRect) as Rectangle;

        InitComposition();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        InitComposition();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        DisposeComposition();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateShadow();
    }

    private void IsSelectedPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        try
        {
            if (dropShadow != null)
            {
                dropShadow.BlurRadius = IsSelected ? 8f : 0f;
            }
        }
        catch (Exception ex) when (ex is ObjectDisposedException || ex is System.Runtime.InteropServices.COMException)
        {
        }
    }

    private void DisposeComposition()
    {
        try
        {
            if (shadowHost != null)
            {
                ElementCompositionPreview.SetElementChildVisual(shadowHost, null);
            }
        }
        catch (Exception ex) when (ex is ObjectDisposedException || ex is System.Runtime.InteropServices.COMException)
        {
        }

        var shadow = dropShadow;
        var animations = imps;
        dropShadow = null;
        imps = null;

        try
        {
            shadow?.Dispose();
            animations?.Dispose();
        }
        catch (Exception ex) when (ex is ObjectDisposedException || ex is System.Runtime.InteropServices.COMException)
        {
        }
    }

    private void InitComposition()
    {
        if (shadowHost == null || backgroundRect == null)
            return;

        DisposeComposition();

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
        if (dropShadow == null)
            return;

        try
        {
            dropShadow.ImplicitAnimations = IsCycleItemContainer ? null : imps;
        }
        catch (Exception ex) when (ex is ObjectDisposedException || ex is System.Runtime.InteropServices.COMException)
        {
        }
    }

    private void UpdateShadow()
    {
        if (dropShadow == null || backgroundRect == null || !IsLoaded)
            return;

        try
        {
            dropShadow.Mask = backgroundRect.GetAlphaMask();
        }
        catch (Exception ex) when (ex is ObjectDisposedException || ex is System.Runtime.InteropServices.COMException)
        {
        }
    }
}
