using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Windows.System;
using DispatcherQueueTimer = Microsoft.UI.Dispatching.DispatcherQueueTimer;

namespace DevWinUI;

[TemplatePart(Name = CanvasPartName, Type = typeof(CanvasAnimatedControl))]
[ContentProperty(Name = nameof(Items))]
public sealed partial class MotionWall : Control
{
    private const string CanvasPartName = "PART_Canvas";
    private const float DegreesToRadians = MathF.PI / 180f;
    private const float StageOverscan = 2f;
    private const float MaskDpi = 48f;
    private const float CullMargin = 8f;
    private const float MaxPointerDegrees = 10f;
    private const int RebuildDelayMs = 150;
    private const double FallbackWidth = 640;
    private const double FallbackHeight = 360;

    private static readonly MotionWallItemInfo EmptyItem = new(MotionWallItemKind.Empty, null, null, null, null, null);

    private readonly object _sync = new();
    private readonly PointerEventHandler _movedHandler;
    private readonly PointerEventHandler _exitedHandler;
    private readonly List<MotionWallHighlight> _highlights = new();
    private readonly List<MotionWallLiftedTile> _lifted = new();
    private readonly List<Transform3DEffect> _liftEffects = new();

    private CanvasAnimatedControl? _canvas;
    private UIElement? _pointerRoot;
    private CanvasRenderTarget? _fadeMask;
    private CanvasImageBrush? _fadeBrush;
    private CanvasLinearGradientBrush? _edgeHorizontal;
    private CanvasLinearGradientBrush? _edgeVertical;
    private CanvasStrokeStyle? _lineStyle;
    private Transform3DEffect? _effect;
    private INotifyCollectionChanged? _sourceNotifier;
    private DispatcherQueueTimer? _rebuildTimer;
    private Brush? _trackedBackground;
    private long _backgroundColorToken;

    private volatile MotionWallCardSet? _cards;
    private volatile MotionWallSettings _settings;
    private volatile MotionWallItemInfo[] _items = Array.Empty<MotionWallItemInfo>();
    private volatile bool _hasPointer;
    private volatile bool _pointerOver;
    private volatile bool _hasFocus;
    private volatile bool _loaded;
    private volatile bool _offscreen;
    private volatile bool _animating = true;
    private volatile bool _rebuildAllowed;
    private volatile bool _building;
    private volatile bool _cardsDirty = true;
    private volatile float _px;
    private volatile float _py;
    private volatile int _hoverIndex = -1;
    private volatile int _focusIndex = -1;
    private int _redrawPending = 1;
    private int _buildVersion;

    private float _scheduledWidth;
    private float _scheduledHeight;
    private float _maskWidth;
    private float _maskHeight;
    private float _maskFade;
    private float _maskStrength;
    private float _overlayWidth;
    private float _overlayHeight;
    private float _overlayFade;
    private float _overlayStrength;
    private Color _overlayColor;

    private double[] _offsets = Array.Empty<double>();
    private float[] _rates = Array.Empty<float>();
    private float _par;
    private float _parTarget;
    private float _spotMix;
    private float _tiltDelta;
    private float _turnDelta;
    private Vector2 _spotPos;
    private bool _spotInit;
    private string? _tooltip;

    public event EventHandler<MotionWallTileEventArgs>? TileInvoked;

    public ObservableCollection<object?> Items { get; } = new();

    public MotionWall()
    {
        DefaultStyleKey = typeof(MotionWall);

        _movedHandler = OnRootPointerMoved;
        _exitedHandler = OnRootPointerExited;
        _settings = CreateSettings();

        Items.CollectionChanged += (_, _) =>
        {
            if (ItemsSource is null)
            {
                RefreshItems();
            }
        };

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        SizeChanged += OnSizeChanged;
        EffectiveViewportChanged += OnEffectiveViewportChanged;
        ActualThemeChanged += OnActualThemeChanged;

        RegisterPropertyChangedCallback(BackgroundProperty, OnBackgroundChanged);
        ApplyInteractive();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_canvas != null)
        {
            _canvas.Draw -= OnCanvasDraw;
            _canvas.Update -= OnCanvasUpdate;
            _canvas.CreateResources -= OnCanvasCreateResources;
        }

        _canvas = GetTemplateChild(CanvasPartName) as CanvasAnimatedControl;

        if (_canvas != null)
        {
            _canvas.Draw += OnCanvasDraw;
            _canvas.Update += OnCanvasUpdate;
            _canvas.CreateResources += OnCanvasCreateResources;

            ApplyFrameRate();
            OnBackgroundChanged(this, BackgroundProperty);
            UpdateRunState();
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double width = availableSize.Width;
        double height = availableSize.Height;

        if (double.IsInfinity(width)) width = MinWidth > 0 ? MinWidth : FallbackWidth;
        if (double.IsInfinity(height)) height = MinHeight > 0 ? MinHeight : FallbackHeight;

        var size = new Size(width, height);
        base.MeasureOverride(size);
        return size;
    }

    protected override void OnPointerEntered(PointerRoutedEventArgs e)
    {
        base.OnPointerEntered(e);
        _pointerOver = true;
        RequestRedraw();
    }

    protected override void OnPointerExited(PointerRoutedEventArgs e)
    {
        base.OnPointerExited(e);
        _pointerOver = false;
        RequestRedraw();
    }

    protected override void OnTapped(TappedRoutedEventArgs e)
    {
        base.OnTapped(e);

        if (IsInteractive)
        {
            InvokeItem(_hoverIndex);
        }
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
        base.OnGotFocus(e);

        if (!IsInteractive || FocusState == FocusState.Pointer)
        {
            return;
        }

        _hasFocus = true;

        if (_focusIndex < 0 && _items.Length > 0)
        {
            _focusIndex = 0;
        }

        RequestRedraw();
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        _hasFocus = false;
        RequestRedraw();
    }

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        base.OnKeyDown(e);

        int count = _items.Length;

        if (!IsInteractive || e.Handled || count == 0)
        {
            return;
        }

        switch (e.Key)
        {
            case VirtualKey.Left:
            case VirtualKey.Up:
                _focusIndex = Mod(_focusIndex - 1, count);
                break;
            case VirtualKey.Right:
            case VirtualKey.Down:
                _focusIndex = Mod(_focusIndex + 1, count);
                break;
            case VirtualKey.Home:
                _focusIndex = 0;
                break;
            case VirtualKey.End:
                _focusIndex = count - 1;
                break;
            case VirtualKey.Enter:
            case VirtualKey.Space:
                InvokeItem(_focusIndex);
                break;
            default:
                return;
        }

        e.Handled = true;
        RequestRedraw();
    }

    private void InvokeItem(int index)
    {
        MotionWallItemInfo[] items = _items;

        if (index < 0 || index >= items.Length)
        {
            return;
        }

        object? original = items[index].Original;

        TileInvoked?.Invoke(this, new MotionWallTileEventArgs(original, index));

        if (original is MotionWallItem { Href: { } href })
        {
            _ = Launcher.LaunchUriAsync(href);
        }
    }

    private void ApplyInteractive()
    {
        bool on = IsInteractive;

        IsTabStop = on;
        IsHitTestVisible = on;

        if (!on)
        {
            _pointerOver = false;
            _hasFocus = false;
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _loaded = true;
        AttachPointer();
        InvalidateCards();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _loaded = false;
        _hasPointer = false;
        _pointerOver = false;
        _rebuildAllowed = false;
        _scheduledWidth = 0;
        _scheduledHeight = 0;
        _rebuildTimer?.Stop();
        Interlocked.Increment(ref _buildVersion);

        DetachPointer();

        if (_canvas != null)
        {
            _canvas.Paused = true;
        }

        ReplaceCards(null);

        lock (_sync)
        {
            DisposeGraphics();
        }
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        RequestRedraw();
    }

    private void OnEffectiveViewportChanged(FrameworkElement sender, EffectiveViewportChangedEventArgs e)
    {
        Rect view = e.EffectiveViewport;
        bool off = view.Width <= 0 || view.Height <= 0 ||
                   view.Right <= 0 || view.Bottom <= 0 ||
                   view.Left >= ActualWidth || view.Top >= ActualHeight;

        if (off == _offscreen) return;

        _offscreen = off;

        if (off) UpdateRunState();
        else RequestRedraw();
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        RefreshSettings();
        InvalidateCards();
    }

    private void AttachPointer()
    {
        DetachPointer();

        _pointerRoot = XamlRoot?.Content;
        _pointerRoot?.AddHandler(PointerMovedEvent, _movedHandler, true);
        _pointerRoot?.AddHandler(PointerPressedEvent, _movedHandler, true);
        _pointerRoot?.AddHandler(PointerExitedEvent, _exitedHandler, true);
    }

    private void DetachPointer()
    {
        if (_pointerRoot is null) return;

        _pointerRoot.RemoveHandler(PointerMovedEvent, _movedHandler);
        _pointerRoot.RemoveHandler(PointerPressedEvent, _movedHandler);
        _pointerRoot.RemoveHandler(PointerExitedEvent, _exitedHandler);
        _pointerRoot = null;
    }

    private void OnRootPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (_canvas is null) return;

        Point point = e.GetCurrentPoint(_canvas).Position;
        _px = (float)point.X;
        _py = (float)point.Y;
        _hasPointer = true;

        if (_settings.NeedsPointer)
        {
            RequestRedraw();
        }
    }

    private void OnRootPointerExited(object sender, PointerRoutedEventArgs e)
    {
        _hasPointer = false;
        RequestRedraw();
    }

    private void OnCanvasCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        lock (_sync)
        {
            DisposeGraphics();
        }

        ReplaceCards(null);
        InvalidateCards();
    }

    private void AttachItemsSource(object? oldValue, object? newValue)
    {
        if (oldValue is INotifyCollectionChanged oldNotifier)
        {
            oldNotifier.CollectionChanged -= OnItemsSourceCollectionChanged;
        }

        _sourceNotifier = newValue as INotifyCollectionChanged;

        if (_sourceNotifier != null)
        {
            _sourceNotifier.CollectionChanged += OnItemsSourceCollectionChanged;
        }

        RefreshItems();
    }

    private void OnItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RefreshItems();
    }

    private void RefreshItems()
    {
        IEnumerable<object?> source = ItemsSource is IEnumerable enumerable and not string
            ? enumerable.Cast<object?>()
            : Items;

        _items = source.Select(ParseItem).ToArray();

        if (_focusIndex >= _items.Length)
        {
            _focusIndex = _items.Length - 1;
        }

        InvalidateCards();
    }

    private static MotionWallItemInfo ParseItem(object? item)
    {
        switch (item)
        {
            case null:
                return EmptyItem;

            case MotionWallItem wallItem:
                if (!string.IsNullOrWhiteSpace(wallItem.Source) && ResolveUri(wallItem.Source) is { } itemUri)
                {
                    return new MotionWallItemInfo(MotionWallItemKind.Image, itemUri.AbsoluteUri, itemUri, wallItem.Text, wallItem.Title, item);
                }

                return string.IsNullOrEmpty(wallItem.Text)
                    ? EmptyItem with { Title = wallItem.Title, Original = item }
                    : new MotionWallItemInfo(MotionWallItemKind.Text, null, null, wallItem.Text, wallItem.Title, item);

            case string text:
                if (LooksLikeImage(text) && ResolveUri(text) is { } uri)
                {
                    return new MotionWallItemInfo(MotionWallItemKind.Image, uri.AbsoluteUri, uri, null, null, item);
                }

                return new MotionWallItemInfo(MotionWallItemKind.Text, null, null, text, null, item);

            default:
                return new MotionWallItemInfo(MotionWallItemKind.Text, null, null, Convert.ToString(item, System.Globalization.CultureInfo.InvariantCulture), null, item);
        }
    }

    private static bool LooksLikeImage(string value)
    {
        string s = value.Trim();

        if (s.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            s.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
            s.StartsWith("ms-appx:", StringComparison.OrdinalIgnoreCase) ||
            s.StartsWith("ms-appdata:", StringComparison.OrdinalIgnoreCase) ||
            s.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        int query = s.IndexOf('?');
        if (query >= 0) s = s[..query];

        return s.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
               s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
               s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
               s.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
               s.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
               s.EndsWith(".webp", StringComparison.OrdinalIgnoreCase) ||
               s.EndsWith(".tif", StringComparison.OrdinalIgnoreCase) ||
               s.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase);
    }

    private static Uri? ResolveUri(string value)
    {
        string s = value.Trim();

        if (Uri.TryCreate(s, UriKind.Absolute, out Uri? absolute) &&
            absolute.Scheme is "http" or "https" or "ms-appx" or "ms-appdata" or "file")
        {
            return absolute;
        }

        string path = s.TrimStart('/', '\\');
        return Uri.TryCreate("ms-appx:///" + path, UriKind.Absolute, out Uri? relative) ? relative : null;
    }

    private void RequestRedraw()
    {
        Interlocked.Exchange(ref _redrawPending, 1);

        if (DispatcherQueue.HasThreadAccess)
        {
            UpdateRunState();
        }
        else
        {
            DispatcherQueue.TryEnqueue(UpdateRunState);
        }
    }

    private void InvalidateCards()
    {
        Interlocked.Increment(ref _buildVersion);
        _cardsDirty = true;
        RequestRedraw();
    }

    private void UpdateRunState()
    {
        if (_canvas == null)
        {
            return;
        }

        bool run = _loaded && !_offscreen && (Volatile.Read(ref _redrawPending) != 0 || _animating);

        if (_canvas.Paused == run)
        {
            _canvas.Paused = !run;

            if (run)
            {
                _canvas.ResetElapsedTime();
            }
        }
    }

    private void ApplyFrameRate()
    {
        if (_canvas == null)
        {
            return;
        }

        int rate = MaxFrameRate;

        if (rate > 0)
        {
            _canvas.IsFixedTimeStep = true;
            _canvas.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / rate);
        }
        else
        {
            _canvas.IsFixedTimeStep = false;
        }
    }

    private void RefreshSettings()
    {
        _settings = CreateSettings();
    }

    private MotionWallSettings CreateSettings()
    {
        return new MotionWallSettings
        {
            Orientation = Orientation,
            Lanes = Lanes,
            AddEdgeLanes = AddEdgeLanes,
            ItemsPerLane = ItemsPerLane,
            Distribution = ItemDistribution,
            Gap = Gap,
            TileWidth = TileWidth,
            TileHeight = TileHeight,
            AspectRatio = TileAspectRatio,
            CornerRadius = TileCornerRadius,
            Speed = Speed,
            LoopDuration = LoopDuration,
            Direction = Direction,
            Variance = Variance,
            VarianceStyle = VarianceStyle,
            RandomStart = RandomStart,
            PauseOnHover = PauseOnHover,
            Tilt = Tilt,
            Turn = Turn,
            Roll = Roll,
            Perspective = Perspective,
            Depth = Depth,
            Interactive = IsInteractive,
            Parallax = Parallax,
            PointerTilt = PointerTilt,
            Spotlight = Spotlight,
            Dim = Dim,
            Grayscale = Grayscale,
            Lift = Lift,
            OverlayColor = ToColor(OverlayColor),
            OverlayOpacity = OverlayOpacity,
            ShowLines = ShowLines,
            LineColor = ToColor(LineBrush),
            TileColor = ToColor(TileBackground),
            TextColor = ToColor(TileForeground),
            FadeEdges = FadeEdges,
            EdgeFadeSize = EdgeFadeSize,
            EdgeFadeStrength = EdgeFadeStrength,
            EdgeFadeColor = EdgeFadeColor,
            BackgroundColor = ToColor(Background)
        };
    }

    private static Color ToColor(Brush? brush)
    {
        if (brush is SolidColorBrush solid)
        {
            Color color = solid.Color;
            return Color.FromArgb((byte)(color.A * Math.Clamp(solid.Opacity, 0, 1)), color.R, color.G, color.B);
        }

        return Colors.Transparent;
    }

    private void OnBackgroundChanged(DependencyObject sender, DependencyProperty property)
    {
        if (_trackedBackground is SolidColorBrush previous)
        {
            previous.UnregisterPropertyChangedCallback(SolidColorBrush.ColorProperty, _backgroundColorToken);
        }

        _trackedBackground = Background;

        if (_trackedBackground is SolidColorBrush current)
        {
            _backgroundColorToken = current.RegisterPropertyChangedCallback(SolidColorBrush.ColorProperty, (_, _) =>
            {
                RefreshSettings();
                RequestRedraw();
            });
        }

        RefreshSettings();
        RequestRedraw();
    }

    private void SetTooltip(string? text)
    {
        if (string.Equals(_tooltip, text, StringComparison.Ordinal))
        {
            return;
        }

        _tooltip = text;
        DispatcherQueue.TryEnqueue(() => ToolTipService.SetToolTip(this, string.IsNullOrEmpty(text) ? null : text));
    }

    #region Cards

    private void ScheduleCardRebuild()
    {
        if (_rebuildTimer == null)
        {
            _rebuildTimer = DispatcherQueue.CreateTimer();
            _rebuildTimer.Interval = TimeSpan.FromMilliseconds(RebuildDelayMs);
            _rebuildTimer.IsRepeating = false;
            _rebuildTimer.Tick += (_, _) =>
            {
                _rebuildAllowed = true;
                RequestRedraw();
            };
        }

        _rebuildTimer.Stop();
        _rebuildTimer.Start();
    }

    private void ReplaceCards(MotionWallCardSet? next)
    {
        MotionWallCardSet? previous;

        lock (_sync)
        {
            previous = _cards;
            _cards = next;
        }

        previous?.Dispose();
    }

    private async Task RebuildCardsAsync(float width, float height)
    {
        CanvasAnimatedControl? canvas = _canvas;

        if (canvas is null)
        {
            _building = false;
            return;
        }

        int version = Volatile.Read(ref _buildVersion);

        _cardsDirty = false;

        MotionWallItemInfo[] infos = _items;
        MotionWallSettings s = _settings;
        float dpi = canvas.Dpi;

        var owned = new List<MotionWallCard>();
        var byKey = new Dictionary<string, MotionWallCard>();
        var cards = new MotionWallCard[infos.Length];
        bool cancelled = false;

        try
        {
            for (int i = 0; i < infos.Length; i++)
            {
                MotionWallItemInfo info = infos[i];

                if (info.Key != null && byKey.TryGetValue(info.Key, out MotionWallCard? shared))
                {
                    cards[i] = shared;
                    continue;
                }

                CanvasBitmap? bitmap = info.Kind == MotionWallItemKind.Image && info.Uri != null
                    ? await LoadBitmapAsync(canvas, info.Uri)
                    : null;

                if (version != Volatile.Read(ref _buildVersion) || canvas != _canvas)
                {
                    bitmap?.Dispose();
                    cancelled = true;
                    break;
                }

                MotionWallCard card;

                try
                {
                    card = RenderCard(canvas, dpi, info, bitmap, width, height, s);
                }
                finally
                {
                    bitmap?.Dispose();
                }

                owned.Add(card);
                cards[i] = card;

                if (info.Key != null)
                {
                    byKey[info.Key] = card;
                }
            }
        }
        catch (Exception ex) when (ex is COMException or ObjectDisposedException)
        {
            cancelled = true;
        }
        finally
        {
            _building = false;
        }

        if (cancelled)
        {
            foreach (MotionWallCard card in owned)
            {
                card.Dispose();
            }

            RequestRedraw();
            return;
        }

        ReplaceCards(new MotionWallCardSet(cards, infos, owned, width, height));
        RequestRedraw();
    }

    private static async Task<CanvasBitmap?> LoadBitmapAsync(ICanvasResourceCreator creator, Uri uri)
    {
        try
        {
            return await CanvasBitmap.LoadAsync(creator, uri);
        }
        catch (Exception ex) when (ex is UriFormatException or ArgumentException or IOException or UnauthorizedAccessException or COMException)
        {
            return null;
        }
    }

    private static MotionWallCard RenderCard(ICanvasResourceCreator creator, float dpi, MotionWallItemInfo info, CanvasBitmap? bitmap, float width, float height, MotionWallSettings s)
    {
        CanvasRenderTarget image = RenderFace(creator, dpi, info, bitmap, width, height, s, false);
        CanvasRenderTarget? gray = null;

        if (s.Grayscale && bitmap != null)
        {
            try
            {
                gray = RenderFace(creator, dpi, info, bitmap, width, height, s, true);
            }
            catch
            {
                image.Dispose();
                throw;
            }
        }

        return new MotionWallCard(image, gray);
    }

    private static CanvasRenderTarget RenderFace(ICanvasResourceCreator creator, float dpi, MotionWallItemInfo info, CanvasBitmap? bitmap, float width, float height, MotionWallSettings s, bool gray)
    {
        float radius = (float)Math.Clamp(s.CornerRadius, 0, Math.Min(width, height) / 2f);

        var target = new CanvasRenderTarget(creator, width, height, dpi);

        using CanvasGeometry clip = CanvasGeometry.CreateRoundedRectangle(creator, 0, 0, width, height, radius, radius);
        using CanvasDrawingSession ds = target.CreateDrawingSession();

        ds.Clear(Colors.Transparent);

        using (ds.CreateLayer(1f, clip))
        {
            ds.FillRectangle(0, 0, width, height, s.TileColor);

            if (bitmap != null)
            {
                Size source = bitmap.Size;
                double scale = Math.Max(width / source.Width, height / source.Height);
                double sourceWidth = width / scale;
                double sourceHeight = height / scale;

                var sourceRect = new Rect((source.Width - sourceWidth) / 2, (source.Height - sourceHeight) / 2, sourceWidth, sourceHeight);
                var destRect = new Rect(0, 0, width, height);

                if (gray)
                {
                    using var effect = new GrayscaleEffect { Source = bitmap };
                    ds.DrawImage(effect, destRect, sourceRect, 1f, CanvasImageInterpolation.HighQualityCubic);
                }
                else
                {
                    ds.DrawImage(bitmap, destRect, sourceRect, 1f, CanvasImageInterpolation.HighQualityCubic);
                }
            }
            else if (!string.IsNullOrEmpty(info.Text))
            {
                DrawCenteredText(ds, info.Text, width, height, s.TextColor);
            }
        }

        return target;
    }

    private static void DrawCenteredText(CanvasDrawingSession ds, string text, float width, float height, Color color)
    {
        using var format = new CanvasTextFormat
        {
            FontSize = Math.Clamp(height * 0.18f, 10f, 40f),
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            HorizontalAlignment = CanvasHorizontalAlignment.Center,
            VerticalAlignment = CanvasVerticalAlignment.Center,
            WordWrapping = CanvasWordWrapping.Wrap,
            TrimmingGranularity = CanvasTextTrimmingGranularity.Character,
            TrimmingSign = CanvasTrimmingSign.Ellipsis
        };

        double inset = Math.Min(12d, width * 0.08);

        var rect = new Rect(inset, inset, Math.Max(1d, width - inset * 2d), Math.Max(1d, height - inset * 2d));
        ds.DrawText(text, rect, color, format);
    }

    #endregion

    #region Update

    private void OnCanvasUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        MotionWallSettings s = _settings;
        MotionWallCardSet? cards = _cards;
        Size size = sender.Size;
        float w = (float)size.Width;
        float h = (float)size.Height;

        if (!_loaded || w <= 1f || h <= 1f || cards == null || cards.Infos.Length == 0)
        {
            _animating = false;
            return;
        }

        float dt = Math.Clamp((float)args.Timing.ElapsedTime.TotalSeconds, 0.001f, 0.1f);
        MotionWallMetrics m = CreateMetrics(s, w, h, cards.Infos.Length);

        EnsureLaneState(s, m);

        float px = _px;
        float py = _py;
        bool has = _hasPointer;
        bool inside = has && px >= 0f && py >= 0f && px <= w && py <= h && (!s.Interactive || _pointerOver);
        bool paused = s.PauseOnHover && inside;
        bool unsettled = false;

        var pointer = new Vector2(px, py);

        if (has)
        {
            if (!_spotInit)
            {
                _spotPos = pointer;
                _spotInit = true;
            }

            _spotPos = Vector2.Lerp(_spotPos, pointer, 1f - MathF.Exp(-dt * 14f));
            _parTarget = Math.Clamp((m.Horizontal ? px / w : py / h) - 0.5f, -0.5f, 0.5f);
        }

        _spotMix += ((has ? 1f : 0f) - _spotMix) * MathF.Min(1f, dt * 6f);

        if (!has && _spotMix < 0.01f)
        {
            _spotInit = false;
        }

        _par += (_parTarget - _par) * (1f - MathF.Exp(-dt * 4f));

        if (s.Spotlight > 0 && has && (Vector2.Distance(_spotPos, pointer) > 0.5f || MathF.Abs(1f - _spotMix) > 0.01f)) unsettled = true;
        if (s.Spotlight > 0 && !has && _spotMix > 0.01f) unsettled = true;
        if (s.Parallax > 0 && MathF.Abs(_parTarget - _par) > 0.0005f) unsettled = true;

        float targetTilt = 0f;
        float targetTurn = 0f;

        if (inside && s.PointerTilt > 0)
        {
            float nx = Math.Clamp(px / w * 2f - 1f, -1f, 1f);
            float ny = Math.Clamp(py / h * 2f - 1f, -1f, 1f);
            float amount = (float)s.PointerTilt * MaxPointerDegrees * DegreesToRadians;

            targetTurn = nx * amount;
            targetTilt = -ny * amount;
        }

        float poseStep = 1f - MathF.Exp(-dt * 8f);
        _tiltDelta += (targetTilt - _tiltDelta) * poseStep;
        _turnDelta += (targetTurn - _turnDelta) * poseStep;

        if (MathF.Abs(targetTilt - _tiltDelta) > 0.0005f || MathF.Abs(targetTurn - _turnDelta) > 0.0005f) unsettled = true;

        MotionWallTileRef? hover = null;
        MotionWallTileRef? focus = null;

        if (s.Interactive)
        {
            Matrix4x4 transform = CreateTransform(s, m, w, h, _tiltDelta, _turnDelta);
            MotionWallHomography inverse = MotionWallHomography.Invert(transform);

            if (inside)
            {
                hover = HitTest(inverse, s, m, px, py);
            }

            if (_hasFocus && _focusIndex >= 0 && _focusIndex < cards.Infos.Length)
            {
                Rect visible = GetVisibleStageBounds(inverse, m, w, h);
                focus = FindTile(s, m, transform, visible, _focusIndex, w, h);
            }
        }

        int hoverIndex = hover is { } hoverTile ? ItemIndex(s, m, hoverTile.Lane, hoverTile.K) : -1;
        _hoverIndex = hoverIndex;
        SetTooltip(hoverIndex >= 0 ? cards.Infos[hoverIndex].Title : null);

        if (UpdateHighlights(hover, focus, dt))
        {
            unsettled = true;
        }

        float rateStep = 1f - MathF.Exp(-dt * 10f);
        bool moving = false;

        for (int lane = 0; lane < m.TotalLanes; lane++)
        {
            float target = paused || IsLaneHeld(lane) ? 0f : 1f;
            float rate = _rates[lane] + (target - _rates[lane]) * rateStep;

            if (MathF.Abs(target - rate) < 0.002f)
            {
                rate = target;
            }

            _rates[lane] = rate;

            float speed = LaneSpeed(s, m, lane);
            _offsets[lane] += LaneDirection(s, lane) * speed * rate * dt;

            if (rate > 0f && speed > 0f) moving = true;
            if (rate != target) unsettled = true;
        }

        _animating = moving || unsettled;
    }

    private void EnsureLaneState(MotionWallSettings s, in MotionWallMetrics m)
    {
        if (_offsets.Length == m.TotalLanes)
        {
            return;
        }

        int previous = _offsets.Length;

        Array.Resize(ref _offsets, m.TotalLanes);
        Array.Resize(ref _rates, m.TotalLanes);

        for (int i = previous; i < m.TotalLanes; i++)
        {
            double v = Math.Sin((i + 1) * 78.233) * 43758.5453;

            _offsets[i] = s.RandomStart ? (v - Math.Floor(v)) * m.Loop : 0d;
            _rates[i] = 1f;
        }
    }

    private bool UpdateHighlights(MotionWallTileRef? hover, MotionWallTileRef? focus, float dt)
    {
        foreach (MotionWallHighlight highlight in _highlights)
        {
            highlight.Target = 0f;
        }

        Touch(hover);
        Touch(focus);

        float step = 1f - MathF.Exp(-dt * 12f);
        bool changing = false;

        for (int i = _highlights.Count - 1; i >= 0; i--)
        {
            MotionWallHighlight highlight = _highlights[i];

            highlight.Value += (highlight.Target - highlight.Value) * step;

            if (MathF.Abs(highlight.Target - highlight.Value) < 0.002f)
            {
                highlight.Value = highlight.Target;
            }

            if (highlight.Target == 0f && highlight.Value == 0f)
            {
                _highlights.RemoveAt(i);
            }
            else if (highlight.Value != highlight.Target)
            {
                changing = true;
            }
        }

        return changing;
    }

    private void Touch(MotionWallTileRef? tile)
    {
        if (tile is not { } t)
        {
            return;
        }

        foreach (MotionWallHighlight highlight in _highlights)
        {
            if (highlight.Lane == t.Lane && highlight.K == t.K)
            {
                highlight.Target = 1f;
                return;
            }
        }

        _highlights.Add(new MotionWallHighlight { Lane = t.Lane, K = t.K, Target = 1f });
    }

    private bool IsLaneHeld(int lane)
    {
        foreach (MotionWallHighlight highlight in _highlights)
        {
            if (highlight.Lane == lane && highlight.Target > 0f)
            {
                return true;
            }
        }

        return false;
    }

    private float HighlightOf(int lane, int k)
    {
        for (int i = 0; i < _highlights.Count; i++)
        {
            MotionWallHighlight highlight = _highlights[i];

            if (highlight.Lane == lane && highlight.K == k)
            {
                return highlight.Value;
            }
        }

        return 0f;
    }

    private static float LaneDirection(MotionWallSettings s, int lane)
    {
        return s.Direction switch
        {
            MotionWallDirection.Start => -1f,
            MotionWallDirection.End => 1f,
            MotionWallDirection.AlternateReverse => lane % 2 == 0 ? 1f : -1f,
            _ => lane % 2 == 0 ? -1f : 1f
        };
    }

    private static float LaneSpeed(MotionWallSettings s, in MotionWallMetrics m, int lane)
    {
        float baseSpeed = s.LoopDuration > 0
            ? m.Loop / (float)s.LoopDuration
            : (float)Math.Max(0, s.Speed);

        float variance = (float)s.Variance;

        float factor = s.VarianceStyle == MotionWallVarianceStyle.Stepped
            ? 1f / (1f + variance * (lane % 3))
            : MathF.Max(1f + variance * MathF.Sin((lane + 1) * 12.9898f), 0.2f);

        return baseSpeed * factor;
    }

    private float LaneShift(MotionWallSettings s, in MotionWallMetrics m, int lane)
    {
        if (s.Parallax <= 0)
        {
            return 0f;
        }

        float sign = lane % 2 == 0 ? 1f : -1f;
        return _par * (float)s.Parallax * m.Travel * sign * (0.6f + 0.2f * (lane % 3));
    }

    private static int ItemIndex(MotionWallSettings s, in MotionWallMetrics m, int lane, int k)
    {
        int slot = Mod(k, m.LoopCount);

        return s.Distribution == MotionWallDistribution.Interleaved
            ? Mod(lane + slot * m.TotalLanes, m.ItemCount)
            : Mod(lane * m.Stride + slot, m.ItemCount);
    }

    private MotionWallTileRef? HitTest(in MotionWallHomography inverse, MotionWallSettings s, in MotionWallMetrics m, float sx, float sy)
    {
        if (_offsets.Length < m.TotalLanes || !inverse.TryApply(sx, sy, out float x, out float y))
        {
            return null;
        }

        float u = m.Horizontal ? x : y;
        float v = m.Horizontal ? y : x;

        int lane = (int)MathF.Floor(v / m.CellCross);

        if (lane < 0 || lane >= m.TotalLanes)
        {
            return null;
        }

        float vInside = v - lane * m.CellCross - m.Gap * 0.5f;

        if (vInside < 0f || vInside > m.TileCross)
        {
            return null;
        }

        double relative = u - (_offsets[lane] + LaneShift(s, m, lane));
        int k = (int)Math.Floor(relative / m.CellMain);
        double uInside = relative - k * (double)m.CellMain - m.Gap * 0.5;

        if (uInside < 0d || uInside > m.TileMain)
        {
            return null;
        }

        return new MotionWallTileRef(lane, k);
    }

    private MotionWallTileRef? FindTile(MotionWallSettings s, in MotionWallMetrics m, in Matrix4x4 transform, Rect visible, int itemIndex, float w, float h)
    {
        if (_offsets.Length < m.TotalLanes)
        {
            return null;
        }

        float uMin = (float)(m.Horizontal ? visible.Left : visible.Top);
        float uMax = (float)(m.Horizontal ? visible.Right : visible.Bottom);
        float vMin = (float)(m.Horizontal ? visible.Top : visible.Left);
        float vMax = (float)(m.Horizontal ? visible.Bottom : visible.Right);

        int firstLane = Math.Max(0, (int)MathF.Floor(vMin / m.CellCross));
        int lastLane = Math.Min(m.TotalLanes - 1, (int)MathF.Ceiling(vMax / m.CellCross) - 1);

        var center = new Vector2(w * 0.5f, h * 0.5f);
        float best = float.MaxValue;
        MotionWallTileRef? result = null;

        for (int lane = firstLane; lane <= lastLane; lane++)
        {
            double origin = _offsets[lane] + LaneShift(s, m, lane);
            int k = (int)Math.Floor((uMin - origin) / m.CellMain);

            for (; ; k++)
            {
                float u = (float)(k * (double)m.CellMain + origin);

                if (u >= uMax)
                {
                    break;
                }

                if (ItemIndex(s, m, lane, k) != itemIndex)
                {
                    continue;
                }

                float uc = u + m.CellMain * 0.5f;
                float vc = lane * m.CellCross + m.CellCross * 0.5f;

                Vector4 p = Vector4.Transform(new Vector4(m.Horizontal ? uc : vc, m.Horizontal ? vc : uc, 0f, 1f), transform);

                if (p.W <= 1e-4f)
                {
                    continue;
                }

                float distance = Vector2.DistanceSquared(new Vector2(p.X / p.W, p.Y / p.W), center);

                if (distance < best)
                {
                    best = distance;
                    result = new MotionWallTileRef(lane, k);
                }
            }
        }

        return result;
    }

    #endregion

    #region Layout

    private static bool IsAuto(double value)
    {
        return double.IsNaN(value) || value <= 0;
    }

    private static int Mod(int a, int n)
    {
        return ((a % n) + n) % n;
    }

    private static Color Tint(Color c, float alpha)
    {
        return Color.FromArgb((byte)Math.Clamp(c.A * alpha, 0f, 255f), c.R, c.G, c.B);
    }

    private static float GetFitExtent(MotionWallSettings s, float extent, bool horizontal)
    {
        float roll = MathF.Abs((float)s.Roll) * DegreesToRadians;
        float cross = MathF.Abs((float)(horizontal ? s.Tilt : s.Turn)) * DegreesToRadians;

        return extent * MathF.Cos(roll) / MathF.Max(0.2f, MathF.Cos(cross));
    }

    private static MotionWallMetrics CreateMetrics(MotionWallSettings s, float width, float height, int itemCount)
    {
        bool horizontal = s.Orientation == Orientation.Horizontal;
        int lanes = Math.Max(1, s.Lanes);
        int total = s.TotalLanes;
        float gap = (float)Math.Max(0, s.Gap);
        float aspect = (float)Math.Max(0.1, s.AspectRatio);
        float fit = GetFitExtent(s, horizontal ? height : width, horizontal) / lanes;

        float tileWidth;
        float tileHeight;

        if (horizontal)
        {
            if (IsAuto(s.TileHeight))
            {
                gap = MathF.Min(gap, fit * 0.5f);
                tileHeight = MathF.Max(1f, fit - gap);
            }
            else
            {
                tileHeight = (float)s.TileHeight;
            }

            tileWidth = IsAuto(s.TileWidth) ? tileHeight * aspect : (float)s.TileWidth;
        }
        else
        {
            if (IsAuto(s.TileWidth))
            {
                gap = MathF.Min(gap, fit * 0.5f);
                tileWidth = MathF.Max(1f, fit - gap);
            }
            else
            {
                tileWidth = (float)s.TileWidth;
            }

            tileHeight = IsAuto(s.TileHeight) ? tileWidth / aspect : (float)s.TileHeight;
        }

        int count = Math.Max(1, itemCount);

        int loop = s.ItemsPerLane > 0
            ? s.ItemsPerLane
            : s.Distribution == MotionWallDistribution.Interleaved
                ? (int)Math.Ceiling(count / (double)total)
                : count;

        float stageSize = MathF.Sqrt(width * width + height * height) * StageOverscan;
        float cross = total * ((horizontal ? tileHeight : tileWidth) + gap);

        return new MotionWallMetrics(
            horizontal,
            total,
            count,
            Math.Max(1, loop),
            s.ItemsPerLane > 0 ? s.ItemsPerLane : 3,
            tileWidth,
            tileHeight,
            gap,
            horizontal ? stageSize : cross,
            horizontal ? cross : stageSize,
            MathF.Max(300f, (horizontal ? width : height) * 0.4f));
    }

    private static Matrix4x4 CreateTransform(MotionWallSettings s, in MotionWallMetrics m, float width, float height, float tiltDelta, float turnDelta)
    {
        float distance = double.IsNaN(s.Perspective) ? MathF.Max(width, height) * 1.6f : (float)s.Perspective;

        Matrix4x4 projection = Matrix4x4.Identity;

        if (distance > 0f)
        {
            projection.M34 = -1f / distance;
        }

        return Matrix4x4.CreateTranslation(-m.StageWidth / 2f, -m.StageHeight / 2f, 0f)
            * Matrix4x4.CreateRotationZ((float)s.Roll * DegreesToRadians)
            * Matrix4x4.CreateRotationY((float)s.Turn * DegreesToRadians + turnDelta)
            * Matrix4x4.CreateRotationX((float)s.Tilt * DegreesToRadians + tiltDelta)
            * Matrix4x4.CreateTranslation(0f, 0f, -(float)s.Depth)
            * projection
            * Matrix4x4.CreateTranslation(width / 2f, height / 2f, 0f);
    }

    private static Rect GetVisibleStageBounds(in MotionWallHomography inverse, in MotionWallMetrics m, float width, float height)
    {
        var full = new Rect(0, 0, m.StageWidth, m.StageHeight);

        if (!inverse.IsValid)
        {
            return full;
        }

        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;
        bool positive = false;

        for (int n = 0; n < 4; n++)
        {
            float sx = n == 1 || n == 2 ? width : 0f;
            float sy = n >= 2 ? height : 0f;
            float w = inverse.W(sx, sy);

            if (MathF.Abs(w) < 1e-6f || (n > 0 && (w > 0) != positive))
            {
                return full;
            }

            positive = w > 0;

            inverse.TryApply(sx, sy, out float x, out float y);

            minX = MathF.Min(minX, x);
            maxX = MathF.Max(maxX, x);
            minY = MathF.Min(minY, y);
            maxY = MathF.Max(maxY, y);
        }

        float left = MathF.Max(0f, minX - CullMargin);
        float top = MathF.Max(0f, minY - CullMargin);
        float right = MathF.Min(m.StageWidth, maxX + CullMargin);
        float bottom = MathF.Min(m.StageHeight, maxY + CullMargin);

        if (right <= left || bottom <= top)
        {
            return full;
        }

        return new Rect(left, top, right - left, bottom - top);
    }

    #endregion

    #region Draw

    private void OnCanvasDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        Interlocked.Exchange(ref _redrawPending, 0);

        DrawFrame(sender, args.DrawingSession);

        if (!_animating)
        {
            DispatcherQueue.TryEnqueue(UpdateRunState);
        }
    }

    private void DrawFrame(ICanvasAnimatedControl sender, CanvasDrawingSession ds)
    {
        Size size = sender.Size;

        if (!_loaded || size.Width <= 0 || size.Height <= 0)
        {
            return;
        }

        float width = (float)size.Width;
        float height = (float)size.Height;

        MotionWallSettings s = _settings;

        lock (_sync)
        {
            MotionWallCardSet? cards = _cards;
            MotionWallMetrics m = CreateMetrics(s, width, height, cards?.Infos.Length ?? _items.Length);

            bool sizeChanged = cards == null
                || Math.Abs(cards.Width - m.TileWidth) > 0.5f
                || Math.Abs(cards.Height - m.TileHeight) > 0.5f;

            if (!sizeChanged)
            {
                _rebuildAllowed = false;
                _scheduledWidth = 0;
                _scheduledHeight = 0;
            }
            else if (cards != null
                && !_rebuildAllowed
                && (Math.Abs(_scheduledWidth - m.TileWidth) > 0.5f || Math.Abs(_scheduledHeight - m.TileHeight) > 0.5f))
            {
                _scheduledWidth = m.TileWidth;
                _scheduledHeight = m.TileHeight;

                DispatcherQueue.TryEnqueue(ScheduleCardRebuild);
            }

            if (!_building && (_cardsDirty || cards == null || (sizeChanged && _rebuildAllowed)))
            {
                _building = true;
                _rebuildAllowed = false;

                float tileWidth = m.TileWidth;
                float tileHeight = m.TileHeight;

                DispatcherQueue.TryEnqueue(() => _ = RebuildCardsAsync(tileWidth, tileHeight));
            }

            if (cards == null || cards.Infos.Length == 0 || _offsets.Length < m.TotalLanes)
            {
                return;
            }

            Matrix4x4 transform = CreateTransform(s, m, width, height, _tiltDelta, _turnDelta);
            MotionWallHomography inverse = MotionWallHomography.Invert(transform);
            Rect visible = GetVisibleStageBounds(inverse, m, width, height);

            _lifted.Clear();

            using var stage = new CanvasCommandList(sender);

            using (CanvasDrawingSession stageSession = stage.CreateDrawingSession())
            {
                DrawStage(stageSession, s, m, transform, visible, cards, width, height);
            }

            Transform3DEffect effect = _effect ??= new Transform3DEffect { InterpolationMode = CanvasImageInterpolation.Anisotropic };
            effect.Source = stage;
            effect.TransformMatrix = transform;

            var liftLists = new List<CanvasCommandList>();

            try
            {
                if (!s.FadeEdges || s.EdgeFadeStrength <= 0)
                {
                    DisposeMask();
                    DisposeOverlay();
                    DrawWorld();
                    return;
                }

                float fade = (float)Math.Clamp(s.EdgeFadeSize, 0.0, 0.5);
                float strength = (float)Math.Clamp(s.EdgeFadeStrength, 0.0, 1.0);

                Color? overlay = s.EdgeFadeColor;

                if (overlay == null && s.BackgroundColor.A == 255)
                {
                    overlay = s.BackgroundColor;
                }

                if (overlay is { } overlayColor)
                {
                    DisposeMask();
                    EnsureFadeOverlay(sender, width, height, fade, strength, overlayColor);

                    DrawWorld();
                    ds.FillRectangle(0, 0, width, height, _edgeHorizontal!);
                    ds.FillRectangle(0, 0, width, height, _edgeVertical!);
                    return;
                }

                DisposeOverlay();
                EnsureFadeMask(sender, width, height, fade, strength);

                using (ds.CreateLayer(_fadeBrush!))
                {
                    DrawWorld();
                }
            }
            finally
            {
                effect.Source = null;

                foreach (Transform3DEffect liftEffect in _liftEffects)
                {
                    liftEffect.Source = null;
                }

                foreach (CanvasCommandList list in liftLists)
                {
                    list.Dispose();
                }
            }

            void DrawWorld()
            {
                ds.DrawImage(effect);

                for (int i = 0; i < _lifted.Count; i++)
                {
                    MotionWallLiftedTile tile = _lifted[i];

                    var list = new CanvasCommandList(sender);
                    liftLists.Add(list);

                    using (CanvasDrawingSession session = list.CreateDrawingSession())
                    {
                        DrawTile(session, s, m, tile.Card, tile.X, tile.Y, cards.Width, cards.Height, tile.Alpha, tile.Light, tile.Overlay);
                    }

                    while (_liftEffects.Count <= i)
                    {
                        _liftEffects.Add(new Transform3DEffect { InterpolationMode = CanvasImageInterpolation.Anisotropic });
                    }

                    Transform3DEffect liftEffect = _liftEffects[i];
                    liftEffect.Source = list;
                    liftEffect.TransformMatrix = Matrix4x4.CreateTranslation(0f, 0f, tile.Lift) * transform;

                    ds.DrawImage(liftEffect);
                }
            }
        }
    }

    private void DrawStage(CanvasDrawingSession ds, MotionWallSettings s, in MotionWallMetrics m, in Matrix4x4 transform, Rect visible, MotionWallCardSet set, float w, float h)
    {
        bool horizontal = m.Horizontal;

        float uMin = (float)(horizontal ? visible.Left : visible.Top);
        float uMax = (float)(horizontal ? visible.Right : visible.Bottom);
        float vMin = (float)(horizontal ? visible.Top : visible.Left);
        float vMax = (float)(horizontal ? visible.Bottom : visible.Right);

        float cellMain = m.CellMain;
        float cellCross = m.CellCross;
        float halfGap = m.Gap * 0.5f;

        int firstLane = Math.Max(0, (int)MathF.Floor(vMin / cellCross));
        int lastLane = Math.Min(m.TotalLanes - 1, (int)MathF.Ceiling(vMax / cellCross) - 1);

        float spotRadius = (float)s.Spotlight * MathF.Max(w, h) * 0.35f;
        bool spotOn = s.Spotlight > 0 && _spotMix > 0.001f && spotRadius > 1f;
        bool dimOn = s.Dim > 0 && (s.Spotlight > 0 || s.Interactive);
        float dim = (float)Math.Clamp(s.Dim, 0, 1);
        float overlayBase = (float)Math.Clamp(s.OverlayOpacity, 0, 1);
        bool liftOn = s.Interactive && s.Lift > 0;
        bool showLines = s.ShowLines;

        _lineStyle ??= new CanvasStrokeStyle { CustomDashStyle = new float[] { 4, 4 } };

        if (showLines)
        {
            for (int lane = firstLane; lane <= lastLane + 1; lane++)
            {
                float v = lane * cellCross;
                DrawLine(ds, horizontal, uMin, v, uMax, v, s.LineColor, _lineStyle);
            }
        }

        for (int lane = firstLane; lane <= lastLane; lane++)
        {
            double origin = _offsets[lane] + LaneShift(s, m, lane);
            float laneTop = lane * cellCross;
            float v0 = laneTop + halfGap;
            int k = (int)Math.Floor((uMin - origin) / cellMain);

            for (; ; k++)
            {
                float u = (float)(k * (double)cellMain + origin);

                if (u >= uMax)
                {
                    break;
                }

                if (showLines)
                {
                    DrawLine(ds, horizontal, u, laneTop, u, laneTop + cellCross, s.LineColor, _lineStyle);
                }

                float u0 = u + halfGap;
                float x = horizontal ? u0 : v0;
                float y = horizontal ? v0 : u0;

                MotionWallCard card = set.Cards[ItemIndex(s, m, lane, k)];
                float highlight = HighlightOf(lane, k);
                float spot = 0f;

                if (spotOn)
                {
                    Vector4 p = Vector4.Transform(new Vector4(x + m.TileWidth * 0.5f, y + m.TileHeight * 0.5f, 0f, 1f), transform);

                    if (p.W > 1e-4f)
                    {
                        float q = Math.Clamp(1f - Vector2.Distance(new Vector2(p.X / p.W, p.Y / p.W), _spotPos) / spotRadius, 0f, 1f);
                        spot = q * q * (3f - 2f * q) * _spotMix;
                    }
                }

                float light = MathF.Max(spot, highlight);
                float alpha = dimOn ? 1f - dim * (1f - light) : 1f;
                float overlay = overlayBase * (1f - light);

                if (liftOn && highlight > 0.002f)
                {
                    _lifted.Add(new MotionWallLiftedTile(card, x, y, alpha, light, overlay, (float)s.Lift * highlight));
                    continue;
                }

                DrawTile(ds, s, m, card, x, y, set.Width, set.Height, alpha, light, overlay);
            }
        }
    }

    private static void DrawTile(CanvasDrawingSession ds, MotionWallSettings s, in MotionWallMetrics m, MotionWallCard card, float x, float y, float sourceWidth, float sourceHeight, float alpha, float light, float overlay)
    {
        float tw = m.TileWidth;
        float th = m.TileHeight;
        bool scaled = Math.Abs(sourceWidth - tw) > 0.5f || Math.Abs(sourceHeight - th) > 0.5f;

        if (s.Grayscale && card.Gray != null)
        {
            DrawImage(ds, card.Gray, x, y, tw, th, sourceWidth, sourceHeight, scaled, alpha);

            if (light > 0.01f)
            {
                DrawImage(ds, card.Image, x, y, tw, th, sourceWidth, sourceHeight, scaled, alpha * light);
            }
        }
        else
        {
            DrawImage(ds, card.Image, x, y, tw, th, sourceWidth, sourceHeight, scaled, alpha);
        }

        if (overlay > 0.003f && s.OverlayColor.A > 0)
        {
            float radius = (float)Math.Clamp(s.CornerRadius, 0, Math.Min(tw, th) / 2f);
            ds.FillRoundedRectangle(x, y, tw, th, radius, radius, Tint(s.OverlayColor, overlay));
        }
    }

    private static void DrawImage(CanvasDrawingSession ds, CanvasRenderTarget image, float x, float y, float width, float height, float sourceWidth, float sourceHeight, bool scaled, float opacity)
    {
        var source = new Rect(0, 0, sourceWidth, sourceHeight);

        if (scaled)
        {
            ds.DrawImage(image, new Rect(x, y, width, height), source, opacity, CanvasImageInterpolation.Linear);
        }
        else
        {
            ds.DrawImage(image, x, y, source, opacity);
        }
    }

    private static void DrawLine(CanvasDrawingSession ds, bool horizontal, float u0, float v0, float u1, float v1, Color color, CanvasStrokeStyle style)
    {
        if (horizontal)
        {
            ds.DrawLine(u0, v0, u1, v1, color, 1f, style);
        }
        else
        {
            ds.DrawLine(v0, u0, v1, u1, color, 1f, style);
        }
    }

    private void DisposeGraphics()
    {
        DisposeMask();
        DisposeOverlay();

        _effect?.Dispose();
        _effect = null;

        foreach (Transform3DEffect liftEffect in _liftEffects)
        {
            liftEffect.Dispose();
        }

        _liftEffects.Clear();

        _lineStyle?.Dispose();
        _lineStyle = null;
    }

    private void DisposeMask()
    {
        _fadeBrush?.Dispose();
        _fadeBrush = null;

        _fadeMask?.Dispose();
        _fadeMask = null;
    }

    private void DisposeOverlay()
    {
        _edgeHorizontal?.Dispose();
        _edgeHorizontal = null;

        _edgeVertical?.Dispose();
        _edgeVertical = null;
    }

    private void EnsureFadeOverlay(ICanvasResourceCreator creator, float width, float height, float fade, float strength, Color color)
    {
        if (_edgeHorizontal != null && _overlayWidth == width && _overlayHeight == height && _overlayFade == fade && _overlayStrength == strength && _overlayColor == color)
        {
            return;
        }

        DisposeOverlay();

        Color solid = Color.FromArgb((byte)(strength * 255f), color.R, color.G, color.B);

        _edgeHorizontal = CreateEdgeBrush(creator, new Vector2(0, 0), new Vector2(width, 0), fade, solid);
        _edgeVertical = CreateEdgeBrush(creator, new Vector2(0, 0), new Vector2(0, height), fade, solid);
        _overlayWidth = width;
        _overlayHeight = height;
        _overlayFade = fade;
        _overlayStrength = strength;
        _overlayColor = color;
    }

    private static CanvasLinearGradientBrush CreateEdgeBrush(ICanvasResourceCreator creator, Vector2 start, Vector2 end, float fade, Color color)
    {
        Color clear = Color.FromArgb(0, color.R, color.G, color.B);

        var stops = new[]
        {
            new CanvasGradientStop { Position = 0f, Color = color },
            new CanvasGradientStop { Position = Math.Max(fade, 0.0001f), Color = clear },
            new CanvasGradientStop { Position = Math.Min(1f - fade, 0.9999f), Color = clear },
            new CanvasGradientStop { Position = 1f, Color = color }
        };

        return new CanvasLinearGradientBrush(creator, stops)
        {
            StartPoint = start,
            EndPoint = end
        };
    }

    private void EnsureFadeMask(ICanvasResourceCreator creator, float width, float height, float fade, float strength)
    {
        if (_fadeBrush != null && _maskWidth == width && _maskHeight == height && _maskFade == fade && _maskStrength == strength)
        {
            return;
        }

        DisposeMask();

        using var horizontal = new CanvasCommandList(creator);
        using var vertical = new CanvasCommandList(creator);

        using (CanvasDrawingSession session = horizontal.CreateDrawingSession())
        using (CanvasLinearGradientBrush brush = CreateFadeBrush(creator, new Vector2(0, 0), new Vector2(width, 0), fade, strength))
        {
            session.FillRectangle(0, 0, width, height, brush);
        }

        using (CanvasDrawingSession session = vertical.CreateDrawingSession())
        using (CanvasLinearGradientBrush brush = CreateFadeBrush(creator, new Vector2(0, 0), new Vector2(0, height), fade, strength))
        {
            session.FillRectangle(0, 0, width, height, brush);
        }

        using var product = new ArithmeticCompositeEffect
        {
            Source1 = horizontal,
            Source2 = vertical,
            MultiplyAmount = 1f,
            Source1Amount = 0f,
            Source2Amount = 0f,
            Offset = 0f
        };

        _fadeMask = new CanvasRenderTarget(creator, width, height, MaskDpi);

        using (CanvasDrawingSession ds = _fadeMask.CreateDrawingSession())
        {
            ds.Clear(Colors.Transparent);
            ds.DrawImage(product);
        }

        _fadeBrush = new CanvasImageBrush(creator, _fadeMask);
        _maskWidth = width;
        _maskHeight = height;
        _maskFade = fade;
        _maskStrength = strength;
    }

    private static CanvasLinearGradientBrush CreateFadeBrush(ICanvasResourceCreator creator, Vector2 start, Vector2 end, float fade, float strength)
    {
        Color edge = Color.FromArgb((byte)((1f - strength) * 255f), 0, 0, 0);

        var stops = new[]
        {
            new CanvasGradientStop { Position = 0f, Color = edge },
            new CanvasGradientStop { Position = Math.Max(fade, 0.0001f), Color = Colors.Black },
            new CanvasGradientStop { Position = Math.Min(1f - fade, 0.9999f), Color = Colors.Black },
            new CanvasGradientStop { Position = 1f, Color = edge }
        };

        return new CanvasLinearGradientBrush(creator, stops)
        {
            StartPoint = start,
            EndPoint = end
        };
    }

    #endregion
}
