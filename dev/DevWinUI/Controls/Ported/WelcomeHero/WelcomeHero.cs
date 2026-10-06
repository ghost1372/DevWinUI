using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;
using Windows.UI.ViewManagement;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_SceneHost), Type = typeof(Grid))]
[TemplatePart(Name = nameof(PART_LabelPill), Type = typeof(Border))]
[TemplatePart(Name = nameof(PART_LabelText), Type = typeof(TextBlock))]
public partial class WelcomeHero : Control
{
    private const string PART_SceneHost = "PART_SceneHost";
    private const string PART_LabelPill = "PART_LabelPill";
    private const string PART_LabelText = "PART_LabelText";

    private Grid sceneHost;
    private Border labelPill;
    private TextBlock labelText;

    private const float LabelGap = 6f;

    // Text reveal, in milliseconds.
    private const float IntroRevealStartMs = WelcomeHeroScene.TextRevealStartMs;
    private const float IntroRevealStaggerMs = 90f;
    private const float IntroRevealMs = 700f;
    private const float SettleRevealStartMs = 80f;
    private const float SettleRevealStaggerMs = 50f;
    private const float SettleRevealMs = 450f;
    private const float RevealDistance = 24f;

    private static readonly TimeSpan AssetLoadTimeout = TimeSpan.FromMilliseconds(1500);

    private readonly List<LoadedImageSurface> _iconSurfaces = [];
    private readonly UISettings _uiSettings = new();

    private WelcomeHeroLayout _layout;
    private WelcomeHeroScene _scene;
    private LoadedImageSurface _logoSurface;
    private DispatcherQueueTimer _loadTimeout;
    private InputCursor _handCursor;
    private WelcomeHeroPlayback _activePlayback;
    private int _pendingLoads;
    private bool _started;
    private int _hoveredTile = -1;
    private bool _hoveringLogo;

    /// <summary>
    /// Raised with the OOBE navigation tag of a tool when its tile is clicked.
    /// </summary>
    public event EventHandler<string> ModuleInvoked;

    /// <summary>
    /// Identifies the <see cref="Modules"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ModulesProperty =
        DependencyProperty.Register(nameof(Modules), typeof(IList<WelcomeHeroModule>), typeof(WelcomeHero), new PropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="LogoAsset"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LogoAssetProperty =
        DependencyProperty.Register(nameof(LogoAsset), typeof(string), typeof(WelcomeHero), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets how the hero enters the screen. Set this before the control is loaded.
    /// </summary>
    public WelcomeHeroPlayback Playback { get; set; } = WelcomeHeroPlayback.Intro;

    /// <summary>
    /// Gets or sets the modules shown as icon tiles in the hero. Each module provides its icon's
    /// full asset address, its OOBE navigation tag, and its display title.
    /// </summary>
    public IList<WelcomeHeroModule> Modules
    {
        get => (IList<WelcomeHeroModule>)GetValue(ModulesProperty);
        set => SetValue(ModulesProperty, value);
    }

    /// <summary>
    /// Gets or sets the full asset address of the logo shown at the apex of the hero.
    /// </summary>
    public string LogoAsset
    {
        get => (string)GetValue(LogoAssetProperty);
        set => SetValue(LogoAssetProperty, value);
    }

    /// <summary>
    /// Gets the page elements (title, description, ...) that rise into view in sync with the hero.
    /// </summary>
    public IList<UIElement> RevealTargets { get; } = [];

    /// <summary>
    /// Gets a value indicating whether Windows animations are turned on.
    /// </summary>
    public bool AnimationsEnabled => _uiSettings.AnimationsEnabled;

    private int IconTileCount => Math.Min(Modules?.Count ?? 0, _scene?.TileCount ?? 0);
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        sceneHost = GetTemplateChild(PART_SceneHost) as Grid;
        labelPill = GetTemplateChild(PART_LabelPill) as Border;
        labelText = GetTemplateChild(PART_LabelText) as TextBlock;

        Height = WelcomeHeroLayout.DefaultHeroHeight;

        Loaded -= OnLoaded;
        Loaded += OnLoaded;

        Unloaded -= OnUnloaded;
        Unloaded += OnUnloaded;

        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;

        ActualThemeChanged -= OnActualThemeChanged;
        ActualThemeChanged += OnActualThemeChanged;

        sceneHost.PointerCanceled -= SceneHost_PointerExited;
        sceneHost.PointerCanceled += SceneHost_PointerExited;

        sceneHost.PointerCaptureLost -= SceneHost_PointerExited;
        sceneHost.PointerCaptureLost += SceneHost_PointerExited;

        sceneHost.PointerExited -= SceneHost_PointerExited;
        sceneHost.PointerExited += SceneHost_PointerExited;

        sceneHost.PointerEntered -= SceneHost_PointerEntered;
        sceneHost.PointerEntered += SceneHost_PointerEntered;

        sceneHost.PointerMoved -= SceneHost_PointerMoved;
        sceneHost.PointerMoved += SceneHost_PointerMoved;

        sceneHost.Tapped -= SceneHost_Tapped;
        sceneHost.Tapped += SceneHost_Tapped;
    }

    private static Color GetResourceColor(string key, Color fallback) =>
        Application.Current.Resources.TryGetValue(key, out var value) && value is Color color ? color : fallback;
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_scene is not null)
        {
            return;
        }

        _activePlayback = AnimationsEnabled ? Playback : WelcomeHeroPlayback.Static;
        _layout = new WelcomeHeroLayout((float)Height);

        var scale = (float)(XamlRoot?.RasterizationScale ?? 1.0);
        var iconPixels = Math.Ceiling(WelcomeHeroLayout.IconSize * WelcomeHeroScene.MaxMagnification * scale);
        var logoPixels = Math.Ceiling(WelcomeHeroLayout.LogoSize * 1.2f * scale);

        foreach (var module in Modules ?? [])
        {
            _iconSurfaces.Add(LoadSurface(module.Asset, iconPixels));
        }

        _logoSurface = LoadSurface(LogoAsset, logoPixels);

        var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        _scene = new WelcomeHeroScene(compositor, _layout, _iconSurfaces, _logoSurface, ResolvePalette());
        _scene.SetViewport(new Vector2((float)sceneHost.ActualWidth, (float)sceneHost.ActualHeight), scale);
        ElementCompositionPreview.SetElementChildVisual(sceneHost, _scene.Root);

        if (_activePlayback == WelcomeHeroPlayback.Static)
        {
            _scene.ShowFinalState();
            _started = true;
            return;
        }

        _scene.EnablePointerEffects(ElementCompositionPreview.GetPointerPositionPropertySet(sceneHost));
        _scene.HideAll();
        foreach (var target in RevealTargets)
        {
            ElementCompositionPreview.GetElementVisual(target).Opacity = 0f;
        }

        // Start once every icon is decoded, so nothing pops in mid-flight; never wait forever though.
        _loadTimeout = DispatcherQueue.CreateTimer();
        _loadTimeout.Interval = AssetLoadTimeout;
        _loadTimeout.IsRepeating = false;
        _loadTimeout.Tick += (_, _) => Start();
        _loadTimeout.Start();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _loadTimeout?.Stop();
        _loadTimeout = null;

        ElementCompositionPreview.SetElementChildVisual(sceneHost, null);
        _scene?.Dispose();
        _scene = null;

        foreach (var surface in _iconSurfaces)
        {
            surface.LoadCompleted -= OnSurfaceLoadCompleted;
            surface.Dispose();
        }

        _iconSurfaces.Clear();
        if (_logoSurface is not null)
        {
            _logoSurface.LoadCompleted -= OnSurfaceLoadCompleted;
            _logoSurface.Dispose();
            _logoSurface = null;
        }

        foreach (var target in RevealTargets)
        {
            var visual = ElementCompositionPreview.GetElementVisual(target);
            visual.StopAnimation("Opacity");
            visual.Opacity = 1f;
        }

        _pendingLoads = 0;
        _started = false;
        _hoveredTile = -1;
        _hoveringLogo = false;
        ProtectedCursor = null;
    }

    private LoadedImageSurface LoadSurface(string assetUri, double pixels)
    {
        var surface = LoadedImageSurface.StartLoadFromUri(new Uri(assetUri), new Size(pixels, pixels));
        surface.LoadCompleted += OnSurfaceLoadCompleted;
        _pendingLoads++;
        return surface;
    }

    private void OnSurfaceLoadCompleted(LoadedImageSurface sender, LoadedImageSourceLoadCompletedEventArgs args)
    {
        sender.LoadCompleted -= OnSurfaceLoadCompleted;
        if (--_pendingLoads <= 0)
        {
            Start();
        }
    }

    private void Start()
    {
        if (_started || _scene is null)
        {
            return;
        }

        _started = true;
        _loadTimeout?.Stop();

        if (_activePlayback == WelcomeHeroPlayback.Intro)
        {
            _scene.PlayIntro();
            RevealContent(IntroRevealStartMs, IntroRevealStaggerMs, IntroRevealMs);
        }
        else
        {
            _scene.PlaySettle();
            RevealContent(SettleRevealStartMs, SettleRevealStaggerMs, SettleRevealMs);
        }
    }

    private void RevealContent(float startMs, float staggerMs, float durationMs)
    {
        var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        var easeOut = compositor.CreateCubicBezierEasingFunction(new Vector2(0.16f, 1f), new Vector2(0.3f, 1f));
        var linear = compositor.CreateLinearEasingFunction();

        for (var i = 0; i < RevealTargets.Count; i++)
        {
            var target = RevealTargets[i];
            var delay = TimeSpan.FromMilliseconds(startMs + (i * staggerMs));
            var duration = TimeSpan.FromMilliseconds(durationMs);

            ElementCompositionPreview.SetIsTranslationEnabled(target, true);
            var visual = ElementCompositionPreview.GetElementVisual(target);

            var fade = compositor.CreateScalarKeyFrameAnimation();
            fade.InsertKeyFrame(0f, 0f, linear);
            fade.InsertKeyFrame(1f, 1f, easeOut);
            fade.Duration = duration;
            fade.DelayTime = delay;
            fade.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
            visual.StartAnimation("Opacity", fade);

            var rise = compositor.CreateVector3KeyFrameAnimation();
            rise.InsertKeyFrame(0f, new Vector3(0f, RevealDistance, 0f), linear);
            rise.InsertKeyFrame(1f, Vector3.Zero, easeOut);
            rise.Duration = duration;
            rise.DelayTime = delay;
            rise.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
            visual.StartAnimation("Translation", rise);
        }
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _scene?.SetViewport(new Vector2((float)sceneHost.ActualWidth, (float)sceneHost.ActualHeight), (float)(XamlRoot?.RasterizationScale ?? 1.0));
        HideLabel();
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        _scene?.ApplyPalette(ResolvePalette(), animate: _activePlayback != WelcomeHeroPlayback.Static);
    }

    private WelcomeHeroPalette ResolvePalette()
    {
        if (new AccessibilitySettings().HighContrast)
        {
            return WelcomeHeroPalette.HighContrast(
                GetResourceColor("SystemColorWindowTextColor", Microsoft.UI.Colors.White),
                GetResourceColor("SystemColorHighlightColor", Microsoft.UI.Colors.Cyan));
        }

        return ActualTheme == ElementTheme.Light ? WelcomeHeroPalette.Light : WelcomeHeroPalette.Dark;
    }

    private void SceneHost_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (_activePlayback != WelcomeHeroPlayback.Static)
        {
            _scene?.SetPointerActive(true);
        }

        UpdateHover(e);
    }

    private void SceneHost_PointerMoved(object sender, PointerRoutedEventArgs e) => UpdateHover(e);

    private void SceneHost_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (_activePlayback != WelcomeHeroPlayback.Static)
        {
            _scene?.SetPointerActive(false);
        }

        SetHover(-1, false);
    }

    private void SceneHost_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (_scene is null)
        {
            return;
        }

        var (tile, logo) = HitTest(e.GetPosition(sceneHost));
        if (tile >= 0)
        {
            ModuleInvoked?.Invoke(this, Modules[tile].NavigationTag);
        }
        else if (logo && _started && _activePlayback != WelcomeHeroPlayback.Static)
        {
            // Easter egg: power it on again.
            _activePlayback = WelcomeHeroPlayback.Intro;
            _scene.PlayIntro();
        }
    }

    private void UpdateHover(PointerRoutedEventArgs e)
    {
        if (_scene is null)
        {
            return;
        }

        var (tile, logo) = HitTest(e.GetCurrentPoint(sceneHost).Position);
        SetHover(tile, logo);
    }

    private (int Tile, bool Logo) HitTest(Point position)
    {
        var fromApex = new Vector2((float)position.X, (float)position.Y) - _scene.Apex;
        var tile = _layout.HitTestSlot(fromApex, IconTileCount);
        return (tile, tile < 0 && WelcomeHeroLayout.HitTestLogo(fromApex));
    }

    private void SetHover(int tile, bool logo)
    {
        if (tile == _hoveredTile && logo == _hoveringLogo)
        {
            return;
        }

        _hoveredTile = tile;
        _hoveringLogo = logo;
        _scene?.SetHighlightedTile(tile);

        if (tile >= 0 || logo)
        {
            _handCursor ??= InputSystemCursor.Create(InputSystemCursorShape.Hand);
            ProtectedCursor = _handCursor;
        }
        else
        {
            ProtectedCursor = null;
        }

        if (tile >= 0)
        {
            ShowLabel(tile);
        }
        else
        {
            HideLabel();
        }
    }
    private void ShowLabel(int tile)
    {
        var module = Modules[tile];
        var name = module.Title;
        labelText.Text = string.IsNullOrEmpty(name) ? module.Asset : name;
        labelPill.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        var labelSize = labelPill.DesiredSize;

        var center = _scene.Apex + _layout.Slots[tile].Offset;
        var reach = (WelcomeHeroLayout.TileSize / 2f * WelcomeHeroScene.MaxMagnification) + LabelGap;
        var left = Math.Clamp(center.X - ((float)labelSize.Width / 2f), 4f, Math.Max(4f, (float)ActualWidth - (float)labelSize.Width - 4f));
        var top = center.Y - reach - (float)labelSize.Height;
        var above = top >= 2f;
        if (!above)
        {
            top = center.Y + reach;
        }

        var wasVisible = labelPill.Opacity > 0;
        Canvas.SetLeft(labelPill, left);
        Canvas.SetTop(labelPill, top);
        if (!wasVisible)
        {
            // Start slightly towards the tile, then rise into place.
            labelPill.TranslationTransition = null;
            labelPill.Translation = new Vector3(0f, above ? 6f : -6f, 0f);
            labelPill.TranslationTransition = new Vector3Transition { Duration = TimeSpan.FromMilliseconds(180) };
        }

        labelPill.Translation = Vector3.Zero;
        labelPill.Opacity = 1;
    }

    private void HideLabel()
    {
        labelPill.Opacity = 0;
    }
}
