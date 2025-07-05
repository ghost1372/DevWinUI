using Microsoft.UI.Xaml.Automation;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_TextPanel), Type = typeof(StackPanel))]
[TemplatePart(Name = nameof(PART_ShadowHost), Type = typeof(Grid))]
public partial class HeaderCarouselItem : Button
{
    private const string PART_TextPanel = "PART_TextPanel";
    private const string PART_ShadowHost = "PART_ShadowHost";

    private StackPanel _textPanel;
    private DropShadow _cardShadow;
    private DropShadow _dropShadow;
    private SpriteVisual _cardShadowVisual;
    private FrameworkElement _shadowHost;
    private Visual _textPanelVisual;
    private Compositor _textPanelCompositor;
    private Visual visual;
    private Compositor compositor;

    public HeaderCarouselItem()
    {
        this.DefaultStyleKey = typeof(HeaderCarouselItem);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        visual = ElementCompositionPreview.GetElementVisual(this);
        compositor = visual.Compositor;

        _textPanel = GetTemplateChild(PART_TextPanel) as StackPanel;
        _shadowHost = GetTemplateChild(PART_ShadowHost) as FrameworkElement;

        if (_textPanel is not null)
        {
            _textPanelVisual = ElementCompositionPreview.GetElementVisual(_textPanel);
            _textPanelCompositor = _textPanelVisual.Compositor;

            ElementCompositionPreview.SetIsTranslationEnabled(_textPanel, true);

            _textPanelVisual.Opacity = 0;
            _textPanelVisual.Properties.InsertVector3("Translation", new Vector3(0, 200, 0));

            _textPanel.Visibility = Visibility.Collapsed;
        }

        InitializeShadow();
        AttachCardShadow();

        Unloaded -= HeaderTile_Unloaded;
        Unloaded += HeaderTile_Unloaded;
    }
    private void SetAccessibleName()
    {
        if (!string.IsNullOrEmpty(Header))
        {
            AutomationProperties.SetName(this, Header);
        }
    }
    private void OnIsSelectedChanged()
    {
        if (IsSelected)
        {
            Canvas.SetZIndex(this, 10);
            VisualStateManager.GoToState(this, "Selected", true);
            AnimateShowPanel();
            PlaySelectAnimation();
        }
        else
        {
            VisualStateManager.GoToState(this, "NotSelected", true);
            AnimateHidePanel();
            PlayDeselectAnimation();
        }
    }
    private void InitializeShadow()
    {
        _dropShadow = compositor.CreateDropShadow();
        _dropShadow.Opacity = 0.2f;
        _dropShadow.BlurRadius = 12f;

        var shadowVisual = compositor.CreateSpriteVisual();
        shadowVisual.Shadow = _dropShadow;
        shadowVisual.Size = visual.Size;

        ElementCompositionPreview.SetElementChildVisual(this, shadowVisual);
    }
    private void HeaderTile_Unloaded(object sender, RoutedEventArgs e)
    {
        DetachCardShadow();
    }
    private void DetachCardShadow()
    {
        if (_shadowHost != null)
        {
            ElementCompositionPreview.SetElementChildVisual(_shadowHost, null);
            _shadowHost.SizeChanged -= OnShadowHostSizeChanged;
        }

        _cardShadowVisual = null;
        _cardShadow = null;
        _shadowHost = null;
    }

    private void AttachCardShadow()
    {
        if (_shadowHost == null)
            return;

        var hostVisual = ElementCompositionPreview.GetElementVisual(_shadowHost);
        var compositor = hostVisual.Compositor;

        _cardShadow = compositor.CreateDropShadow();
        _cardShadow.BlurRadius = 12f;
        _cardShadow.Opacity = 0.2f;
        _cardShadow.Color = Colors.Black;
        _cardShadow.Offset = new Vector3(0, 0, 0);

        _cardShadowVisual = compositor.CreateSpriteVisual();
        _cardShadowVisual.Shadow = _cardShadow;
        _cardShadowVisual.Size = new Vector2((float)_shadowHost.ActualWidth, (float)_shadowHost.ActualHeight);

        ElementCompositionPreview.SetElementChildVisual(_shadowHost, _cardShadowVisual);

        _shadowHost.SizeChanged += OnShadowHostSizeChanged;
    }
    private void OnShadowHostSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_cardShadowVisual != null)
        {
            _cardShadowVisual.Size = new Vector2((float)e.NewSize.Width, (float)e.NewSize.Height);
        }
    }

    private void AnimateShowPanel()
    {
        if (_textPanel == null) return;

        _textPanel.Visibility = Visibility.Visible;
        _textPanel.Opacity = 1;
        ElementCompositionPreview.SetIsTranslationEnabled(_textPanel, true);

        // Opacity animation
        var fadeIn = _textPanelCompositor.CreateScalarKeyFrameAnimation();
        fadeIn.InsertKeyFrame(1f, 1f);
        fadeIn.Duration = TimeSpan.FromMilliseconds(400);
        _textPanelVisual.StartAnimation("Opacity", fadeIn);

        // Translation animation
        var slideIn = _textPanelCompositor.CreateVector3KeyFrameAnimation();
        slideIn.InsertKeyFrame(1f, Vector3.Zero);
        slideIn.Duration = TimeSpan.FromMilliseconds(600);
        _textPanelVisual.Properties.StartAnimation("Translation", slideIn);
    }

    private void AnimateHidePanel()
    {
        if (_textPanel == null) return;

        var fadeOut = _textPanelCompositor.CreateScalarKeyFrameAnimation();
        fadeOut.InsertKeyFrame(1f, 0f);
        fadeOut.Duration = TimeSpan.FromMilliseconds(350);

        var slideOut = _textPanelCompositor.CreateVector3KeyFrameAnimation();
        slideOut.InsertKeyFrame(1f, new Vector3(0, 200, 0));
        slideOut.Duration = TimeSpan.FromMilliseconds(600);

        var batch = _textPanelCompositor.CreateScopedBatch(CompositionBatchTypes.Animation);
        batch.Completed += (s, e) =>
        {
            _textPanel.Visibility = Visibility.Collapsed;
        };

        _textPanelVisual.StartAnimation("Opacity", fadeOut);
        _textPanelVisual.Properties.StartAnimation("Translation", slideOut);

        batch.End();
    }

    private void PlaySelectAnimation()
    {
        // Animate scale to 1.0
        var scaleAnim = compositor.CreateVector3KeyFrameAnimation();
        scaleAnim.InsertKeyFrame(1f, new Vector3(1f, 1f, 1f));
        scaleAnim.Duration = TimeSpan.FromMilliseconds(600);
        visual.StartAnimation("Scale", scaleAnim);

        // Animate shadow opacity to 0.4
        var opacityAnim = compositor.CreateScalarKeyFrameAnimation();
        opacityAnim.InsertKeyFrame(1f, 0.4f);
        opacityAnim.Duration = TimeSpan.FromMilliseconds(600);
        _dropShadow.StartAnimation(nameof(_dropShadow.Opacity), opacityAnim);

        // Animate shadow blur radius to 24
        var blurAnim = compositor.CreateScalarKeyFrameAnimation();
        blurAnim.InsertKeyFrame(1f, 24f);
        blurAnim.Duration = TimeSpan.FromMilliseconds(600);
        _dropShadow.StartAnimation(nameof(_dropShadow.BlurRadius), blurAnim);
    }

    private void PlayDeselectAnimation()
    {
        // Scale animation to 0.8
        var scaleAnim = compositor.CreateVector3KeyFrameAnimation();
        scaleAnim.InsertKeyFrame(1f, new Vector3(0.8f, 0.8f, 1f));
        scaleAnim.Duration = TimeSpan.FromMilliseconds(350);

        // Shadow opacity animation to 0.2
        var opacityAnim = compositor.CreateScalarKeyFrameAnimation();
        opacityAnim.InsertKeyFrame(1f, 0.2f);
        opacityAnim.Duration = TimeSpan.FromMilliseconds(350);

        // Shadow blur radius animation to 12
        var blurAnim = compositor.CreateScalarKeyFrameAnimation();
        blurAnim.InsertKeyFrame(1f, 12f);
        blurAnim.Duration = TimeSpan.FromMilliseconds(350);

        var batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
        batch.Completed += (s, e) =>
        {
            Canvas.SetZIndex(this, 0);
        };

        // Start animations while batch is active
        visual.StartAnimation("Scale", scaleAnim);
        _dropShadow.StartAnimation(nameof(_dropShadow.Opacity), opacityAnim);
        _dropShadow.StartAnimation(nameof(_dropShadow.BlurRadius), blurAnim);

        batch.End();
    }
}
