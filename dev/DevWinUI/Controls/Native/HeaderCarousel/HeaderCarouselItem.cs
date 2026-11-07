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
    private Visual visual;
    private Compositor compositor;

    public HeaderCarouselItem()
    {
        DefaultStyleKey = typeof(HeaderCarouselItem);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
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

            ElementCompositionPreview.SetIsTranslationEnabled(_textPanel, true);

            _textPanelVisual.Opacity = 0;
            _textPanelVisual.Properties.InsertVector3("Translation", new Vector3(0, 200, 0));

            _textPanel.Visibility = Visibility.Collapsed;
        }

        InitializeShadow();
        AttachCardShadow();
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
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_shadowHost == null)
            return;

        _shadowHost.SizeChanged -= OnShadowHostSizeChanged;
        _shadowHost.SizeChanged += OnShadowHostSizeChanged;
        UpdateCardShadowSize(_shadowHost.ActualWidth, _shadowHost.ActualHeight);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_shadowHost != null)
            _shadowHost.SizeChanged -= OnShadowHostSizeChanged;
    }

    private void AttachCardShadow()
    {
        if (_shadowHost == null)
            return;

        _cardShadow = compositor.CreateDropShadow();
        _cardShadow.BlurRadius = 12f;
        _cardShadow.Opacity = 0.2f;
        _cardShadow.Color = Colors.Black;
        _cardShadow.Offset = Vector3.Zero;

        _cardShadowVisual = compositor.CreateSpriteVisual();
        _cardShadowVisual.Shadow = _cardShadow;

        UpdateCardShadowSize(_shadowHost.ActualWidth, _shadowHost.ActualHeight);

        ElementCompositionPreview.SetElementChildVisual(_shadowHost, _cardShadowVisual);

        _shadowHost.SizeChanged -= OnShadowHostSizeChanged;
        _shadowHost.SizeChanged += OnShadowHostSizeChanged;
    }

    private void OnShadowHostSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateCardShadowSize(e.NewSize.Width, e.NewSize.Height);
    }

    private void UpdateCardShadowSize(double width, double height)
    {
        if (_cardShadowVisual != null)
        {
            _cardShadowVisual.Size = new Vector2((float)width, (float)height);
        }
    }

    private void AnimateShowPanel()
    {
        if (_textPanel == null)
            return;

        _textPanelVisual.StopAnimation("Opacity");
        _textPanelVisual.Properties.StopAnimation("Translation");

        _textPanelVisual.Opacity = 0;
        _textPanelVisual.Properties.InsertVector3("Translation", new Vector3(0, 200, 0));
        _textPanel.Visibility = Visibility.Visible;
        _textPanel.Opacity = 1;

        var fadeIn = compositor.CreateScalarKeyFrameAnimation();
        fadeIn.InsertKeyFrame(1f, 1f);
        fadeIn.Duration = TimeSpan.FromMilliseconds(400);
        _textPanelVisual.StartAnimation("Opacity", fadeIn);

        var slideIn = compositor.CreateVector3KeyFrameAnimation();
        slideIn.InsertKeyFrame(1f, Vector3.Zero);
        slideIn.Duration = TimeSpan.FromMilliseconds(600);
        _textPanelVisual.Properties.StartAnimation("Translation", slideIn);
    }

    private void AnimateHidePanel()
    {
        if (_textPanel == null || _textPanel.Visibility != Visibility.Visible)
            return;

        _textPanelVisual.StopAnimation("Opacity");
        _textPanelVisual.Properties.StopAnimation("Translation");

        var fadeOut = compositor.CreateScalarKeyFrameAnimation();
        fadeOut.InsertKeyFrame(1f, 0f);
        fadeOut.Duration = TimeSpan.FromMilliseconds(350);

        var slideOut = compositor.CreateVector3KeyFrameAnimation();
        slideOut.InsertKeyFrame(1f, new Vector3(0, 200, 0));
        slideOut.Duration = TimeSpan.FromMilliseconds(600);

        var batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
        batch.Completed += (s, e) =>
        {
            if (!IsSelected && _textPanel != null)
            {
                _textPanel.Visibility = Visibility.Collapsed;
            }
        };

        _textPanelVisual.StartAnimation("Opacity", fadeOut);
        _textPanelVisual.Properties.StartAnimation("Translation", slideOut);

        batch.End();
    }

    private void PlaySelectAnimation()
    {
        if (_dropShadow == null)
            return;

        visual.StopAnimation("Scale");
        _dropShadow.StopAnimation(nameof(_dropShadow.Opacity));
        _dropShadow.StopAnimation(nameof(_dropShadow.BlurRadius));

        var scaleAnim = compositor.CreateVector3KeyFrameAnimation();
        scaleAnim.InsertKeyFrame(1f, new Vector3(1f, 1f, 1f));
        scaleAnim.Duration = TimeSpan.FromMilliseconds(600);
        visual.StartAnimation("Scale", scaleAnim);

        var opacityAnim = compositor.CreateScalarKeyFrameAnimation();
        opacityAnim.InsertKeyFrame(1f, 0.4f);
        opacityAnim.Duration = TimeSpan.FromMilliseconds(600);
        _dropShadow.StartAnimation(nameof(_dropShadow.Opacity), opacityAnim);

        var blurAnim = compositor.CreateScalarKeyFrameAnimation();
        blurAnim.InsertKeyFrame(1f, 24f);
        blurAnim.Duration = TimeSpan.FromMilliseconds(600);
        _dropShadow.StartAnimation(nameof(_dropShadow.BlurRadius), blurAnim);
    }

    private void PlayDeselectAnimation()
    {
        if (_dropShadow == null)
            return;

        visual.StopAnimation("Scale");
        _dropShadow.StopAnimation(nameof(_dropShadow.Opacity));
        _dropShadow.StopAnimation(nameof(_dropShadow.BlurRadius));

        var scaleAnim = compositor.CreateVector3KeyFrameAnimation();
        scaleAnim.InsertKeyFrame(1f, new Vector3(0.8f, 0.8f, 1f));
        scaleAnim.Duration = TimeSpan.FromMilliseconds(350);

        var opacityAnim = compositor.CreateScalarKeyFrameAnimation();
        opacityAnim.InsertKeyFrame(1f, 0.2f);
        opacityAnim.Duration = TimeSpan.FromMilliseconds(350);

        var blurAnim = compositor.CreateScalarKeyFrameAnimation();
        blurAnim.InsertKeyFrame(1f, 12f);
        blurAnim.Duration = TimeSpan.FromMilliseconds(350);

        var batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
        batch.Completed += (s, e) =>
        {
            if (!IsSelected)
            {
                Canvas.SetZIndex(this, 0);
            }
        };

        visual.StartAnimation("Scale", scaleAnim);
        _dropShadow.StartAnimation(nameof(_dropShadow.Opacity), opacityAnim);
        _dropShadow.StartAnimation(nameof(_dropShadow.BlurRadius), blurAnim);

        batch.End();
    }
}
