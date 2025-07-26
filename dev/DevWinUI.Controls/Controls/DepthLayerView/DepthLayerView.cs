using System.Collections;
using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_CompositionHostPanel), Type = typeof(Grid))]
[ContentProperty(Name = nameof(Content))]
public partial class DepthLayerView : ContentControl
{
    private const string PART_CompositionHostPanel = "PART_CompositionHostPanel";
    private Grid compositionHostPanel;

    private Compositor _compositor;
    private ContainerVisual _rootVisual;
    private CompositionPropertySet _animationPropertySet;
    public event EventHandler<DepthLayerViewEventArgs> SelectedIndexChanged;
    public CompositionStretch Stretch
    {
        get { return (CompositionStretch)GetValue(StretchProperty); }
        set { SetValue(StretchProperty, value); }
    }

    public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register(nameof(Stretch), typeof(CompositionStretch), typeof(DepthLayerView), new PropertyMetadata(CompositionStretch.Uniform, OnVisualChanged));

    public IEnumerable Items
    {
        get { return (IEnumerable)GetValue(ItemsProperty); }
        set { SetValue(ItemsProperty, value); }
    }

    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register(nameof(Items), typeof(IEnumerable), typeof(DepthLayerView), new PropertyMetadata(null, OnVisualChanged));

    private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (DepthLayerView)d;
        if (ctl != null)
        {
            ctl.CreateCompositionScene();
        }
    }

    public int SelectedIndex
    {
        get { return (int)GetValue(SelectedIndexProperty); }
        set { SetValue(SelectedIndexProperty, value); }
    }

    public static readonly DependencyProperty SelectedIndexProperty =
        DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(DepthLayerView), new PropertyMetadata(-1, OnSelectedIndexChanged));

    private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (DepthLayerView)d;
        if (ctl != null)
        {
            ctl.OnSelectedIndexChanged((int)e.OldValue, (int)e.NewValue);
        }
    }

    private void OnSelectedIndexChanged(int oldValue, int newValue)
    {
        SelectedIndexChanged?.Invoke(this, new DepthLayerViewEventArgs(oldValue, newValue));
        if (_animationPropertySet != null)
        {
            var currentZAnimation = _animationPropertySet.Compositor.CreateScalarKeyFrameAnimation();
            currentZAnimation.InsertKeyFrame(1, SelectedIndex);
            currentZAnimation.Duration = TimeSpan.FromSeconds(1.2f);
            _animationPropertySet.StartAnimation("currentZ", currentZAnimation);
        }
    }

    public DepthLayerView()
    {
        DefaultStyleKey = typeof(DepthLayerView);
        _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        compositionHostPanel = GetTemplateChild(PART_CompositionHostPanel) as Grid;

        compositionHostPanel.LayoutUpdated -= OnCompositionHostPanelLayoutUpdated;
        compositionHostPanel.LayoutUpdated += OnCompositionHostPanelLayoutUpdated;

        Loaded -= OnLoaded;
        Loaded += OnLoaded;
    }

    private void OnCompositionHostPanelLayoutUpdated(object sender, object e)
    {
        UpdateVisualLayout();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        CreateCompositionScene();
    }

    public void CreateCompositionScene()
    {
        if (compositionHostPanel == null || Items == null)
        {
            return;
        }

        _animationPropertySet = _compositor.CreatePropertySet();
        _animationPropertySet.InsertScalar("currentZ", SelectedIndex);

        var layerEffectDesc = new GaussianBlurEffect
        {
            Name = "blur",
            BorderMode = EffectBorderMode.Hard,
            BlurAmount = 0,
            Source = new SaturationEffect
            {
                Name = "saturation",
                Saturation = 1,
                Source = new CompositionEffectSourceParameter("source")
            }
        };

        var layerEffectFactory = _compositor.CreateEffectFactory(layerEffectDesc,
            new[] { "blur.BlurAmount", "saturation.Saturation" });

        _rootVisual = _compositor.CreateContainerVisual();
        ElementCompositionPreview.SetElementChildVisual(compositionHostPanel, _rootVisual);

        var itemList = Items.Cast<DepthLayerViewItem>().ToList();
        for (int layerIndex = 0; layerIndex < itemList.Count; ++layerIndex)
        {
            var layer = itemList[layerIndex];
            CreateVisuals(layer);

            _rootVisual.Children.InsertAtBottom(layer.LayerVisual);
            layer.LayerVisual.Effect = layerEffectFactory.CreateBrush();
            SetupLayerAnimations(layer, layerIndex);
        }

        UpdateVisualLayout();
    }
    public void CreateVisuals(DepthLayerViewItem layer)
    {
        var layerVisual = _compositor.CreateLayerVisual();
        var itemContainerVisual = _compositor.CreateContainerVisual();
        layerVisual.Children.InsertAtTop(itemContainerVisual);

        var itemVisual = _compositor.CreateSpriteVisual();
        var _surface = LoadedImageSurface.StartLoadFromUri(layer.ImageUri);
        var brush = _compositor.CreateSurfaceBrush(_surface);
        brush.Stretch = Stretch;
        itemVisual.Brush = brush;

        itemVisual.Size = new Vector2((float)ActualWidth, (float)ActualHeight);

        var centerX = (itemContainerVisual.Size.X - itemVisual.Size.X) / 2;
        var centerY = (itemContainerVisual.Size.Y - itemVisual.Size.Y) / 2;
        itemVisual.Offset = new Vector3((float)centerX, (float)centerY, 0);

        itemContainerVisual.Children.InsertAtTop(itemVisual);

        layer.LayerVisual = layerVisual;
        layer.ItemContainerVisual = itemContainerVisual;
    }

    public void UpdateVisualLayout()
    {
        if (_rootVisual == null || compositionHostPanel == null)
            return;

        var itemList = Items.Cast<DepthLayerViewItem>().ToList();

        foreach (var layer in itemList)
        {
            var layerVisual = layer.LayerVisual;
            var itemContainer = layer.ItemContainerVisual;

            if (layerVisual != null)
            {
                layerVisual.Size = new Vector2(
                    (float)compositionHostPanel.ActualWidth,
                    (float)compositionHostPanel.ActualHeight);

                itemContainer.Offset = new Vector3(
                    (float)compositionHostPanel.ActualWidth / 2,
                    (float)compositionHostPanel.ActualHeight / 2, 0);

                var itemVisual = itemContainer.Children.FirstOrDefault() as SpriteVisual;
                if (itemVisual != null)
                {
                    itemVisual.Size = new Vector2(
                        (float)ActualWidth,
                        (float)ActualHeight);

                    var centerX = (itemContainer.Size.X - itemVisual.Size.X) / 2;
                    var centerY = (itemContainer.Size.Y - itemVisual.Size.Y) / 2;
                    itemVisual.Offset = new Vector3((float)centerX, (float)centerY, 0);
                }
            }
        }
    }

    private void SetupLayerAnimations(DepthLayerViewItem layer, float layerZ)
    {
        var layerVisual = layer.LayerVisual;
        var itemContainerVisual = layer.ItemContainerVisual;

        var compositor = layerVisual.Compositor;
        var animationPropertySet = _animationPropertySet;
        var layerZScalar = layerZ;

        var opacityAndSaturationAnimation = compositor.CreateExpressionAnimation("max(0, 1 - abs(currentZ - targetZ))");
        opacityAndSaturationAnimation.SetReferenceParameter("props", animationPropertySet);
        opacityAndSaturationAnimation.SetScalarParameter("targetZ", layerZScalar);
        opacityAndSaturationAnimation.SetScalarParameter("currentZ", 0);
        opacityAndSaturationAnimation.Expression = "max(0, 1 - abs(props.currentZ - targetZ))";

        layerVisual.StartAnimation("Opacity", opacityAndSaturationAnimation);
        layerVisual.Effect.Properties.StartAnimation("saturation.Saturation", opacityAndSaturationAnimation);

        var scaleAnimation = compositor.CreateExpressionAnimation("Vector3(pow(3.5, targetZ - props.currentZ), pow(3.5, targetZ - props.currentZ), 1)");
        scaleAnimation.SetReferenceParameter("props", animationPropertySet);
        scaleAnimation.SetScalarParameter("targetZ", layerZScalar);

        itemContainerVisual.StartAnimation("Scale", scaleAnimation);

        var blurAmountAnimation = compositor.CreateExpressionAnimation("abs(props.currentZ - targetZ) * 10");
        blurAmountAnimation.SetReferenceParameter("props", animationPropertySet);
        blurAmountAnimation.SetScalarParameter("targetZ", layerZScalar);

        layerVisual.Effect.Properties.StartAnimation("blur.BlurAmount", blurAmountAnimation);
    }
}
