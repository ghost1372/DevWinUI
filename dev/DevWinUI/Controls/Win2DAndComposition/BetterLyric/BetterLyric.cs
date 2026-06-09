// https://github.com/jayfunc/BetterLyrics

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

public partial class BetterLyric : Control
{
    private const string PART_Canvas = "PART_Canvas";
    private const string PART_ScrollViewer = "PART_ScrollViewer";

    private CanvasAnimatedControl canvas;
    private ScrollViewer _scrollViewer;

    public event EventHandler<int>? LineClicked;

    private readonly LyricsSynchronizer _synchronizer = new();
    private readonly LyricsRenderer _lyricsRenderer = new();
    private readonly EdgeFadeMaskRenderer _edgeFadeMaskRenderer = new();

    private readonly ValueTransition<Color> _immersiveBgColorTransition = new(
        initialValue: Colors.Black,
        defaultTotalDuration: 0.3f,
        interpolator: (from, to, progress) => ColorHelper.GetInterpolatedColor(progress, from, to)
    );
    private readonly ValueTransition<double> _immersiveBgOpacityTransition = new(
        initialValue: 1f,
        AnimationEasingHelper.GetInterpolatorByEasingType<double>(AnimationEasingType.Sine),
        defaultTotalDuration: 0.3f
    );
    
    private readonly ValueTransition<double> _canvasYScrollTransition = new(
        initialValue: 0f,
        AnimationEasingHelper.GetInterpolatorByEasingType<double>(AnimationEasingType.Sine),
        defaultTotalDuration: 0.3f
    );
    private readonly ValueTransition<double> _mouseYScrollTransition = new(
        initialValue: 0f,
        AnimationEasingHelper.GetInterpolatorByEasingType<double>(AnimationEasingType.Sine),
        defaultTotalDuration: 0.3f
    );


    private TimeSpan _songPositionWithOffset;
    private TimeSpan _songPosition;

    
    private int _mouseHoverLineIndex = -1;
    
    private List<RenderLyricsLine>? _renderLyricsLines = null;

    private DispatcherQueueTimer _layoutTimer;
    private DispatcherQueueTimer _scrollWheelDebounceTimer;
    private bool _isLayoutChanged = false;
    private bool _isNowPlayingPaletteChanged = false;

    private int _primaryPlayingLineIndex;
    private (int Start, int End) _visibleRange;
    private double _canvasTargetScrollOffset;

    public TimeSpan SongPosition => _songPosition;
    public double CurrentCanvasYScroll => _canvasYScrollTransition.Value;
    public double ActualLyricsHeight => CalculateActualHeight(_renderLyricsLines);
    public int CurrentHoveringLineIndex => _mouseHoverLineIndex;

    public BetterLyric()
    {
        DefaultStyleKey = typeof(BetterLyric);

        _layoutTimer = DispatcherQueue.CreateTimer();
        _scrollWheelDebounceTimer = DispatcherQueue.CreateTimer();
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        canvas = GetTemplateChild(PART_Canvas) as CanvasAnimatedControl;
        canvas.CreateResources -= OnCreateResources;
        canvas.CreateResources += OnCreateResources;
        canvas.Draw -= OnDraw;
        canvas.Draw += OnDraw;
        canvas.Update -= OnUpdate;
        canvas.Update += OnUpdate;

        _scrollViewer = GetTemplateChild(PART_ScrollViewer) as ScrollViewer;
        if (_scrollViewer != null)
        {
            _scrollViewer.PointerEntered -= OnScrollViewerPointerEntered;
            _scrollViewer.PointerEntered += OnScrollViewerPointerEntered;

            _scrollViewer.PointerExited -= OnScrollViewerPointerExited;
            _scrollViewer.PointerExited += OnScrollViewerPointerExited;
            
            _scrollViewer.PointerMoved -= OnScrollViewerPointerMoved;
            _scrollViewer.PointerMoved += OnScrollViewerPointerMoved;
            
            _scrollViewer.PointerPressed -= OnScrollViewerPointerPressed;
            _scrollViewer.PointerPressed += OnScrollViewerPointerPressed;
            
            _scrollViewer.PointerReleased -= OnScrollViewerPointerReleased;
            _scrollViewer.PointerReleased += OnScrollViewerPointerReleased;
            
            _scrollViewer.PointerWheelChanged -= OnScrollViewerPointerWheelChanged;
            _scrollViewer.PointerWheelChanged += OnScrollViewerPointerWheelChanged;
        }

        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;

        Unloaded -= OnUnloaded;
        Unloaded += OnUnloaded;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (isAutoLayoutEnabled)
        {
            renderLyricsStartX = 0;
            renderLyricsStartY = 0;
            renderLyricsWidth = ActualWidth;
            renderLyricsHeight = ActualHeight;

            if (lyrics3DAutoFitLayout)
            {
                (renderLyricsHeight, renderLyricsWidth) = (renderLyricsWidth, renderLyricsHeight);
                renderLyricsStartX += (renderLyricsHeight - renderLyricsWidth) / 2;
                renderLyricsStartY += (renderLyricsWidth - renderLyricsHeight) / 2;
            }

            RequestRelayout();
        }
    }

    private void OnScrollViewerPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (isScrollViewerEnabled)
        {
            isMouseInLyricsArea = true;
        }
    }

    private void OnScrollViewerPointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (isScrollViewerEnabled)
        {
            isMouseInLyricsArea = false;
        }
    }

    private void OnScrollViewerPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (isScrollViewerEnabled)
        {
            mousePosition = e.GetCurrentPoint(_scrollViewer).Position;
        }
    } 

    private void OnScrollViewerPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (isScrollViewerEnabled)
        {
            isMousePressing = true;
        }
    }

    private void OnScrollViewerPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (isScrollViewerEnabled)
        {
            isMousePressing = false;

            if (_mouseHoverLineIndex >= 0)
                LineClicked?.Invoke(this, _mouseHoverLineIndex);
        }
    }

    private void OnScrollViewerPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        if (isScrollViewerEnabled)
        {
            isMouseScrolling = true;
            isMouseScrollingChanged = true;

            var delta = e.GetCurrentPoint(_scrollViewer).Properties.MouseWheelDelta;
            var value = _mouseYScrollTransition.TargetValue + delta;
            value = Math.Clamp(value, -_canvasYScrollTransition.Value - CalculateActualHeight(_renderLyricsLines), -_canvasYScrollTransition.Value);
            _mouseYScrollTransition.Start(value);

            _scrollWheelDebounceTimer.Debounce(() =>
            {
                _mouseYScrollTransition.Start(0);
                isMouseScrolling = false;
            }, TimeSpan.FromSeconds(3));
        }
    }

    private void OnUnloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        canvas.Draw -= OnDraw;
        canvas.Update -= OnUpdate;
        canvas.CreateResources -= OnCreateResources;

        if (_scrollViewer != null)
        {
            _scrollViewer.PointerEntered -= OnScrollViewerPointerEntered;
            _scrollViewer.PointerExited -= OnScrollViewerPointerExited;
            _scrollViewer.PointerMoved -= OnScrollViewerPointerMoved;
            _scrollViewer.PointerPressed -= OnScrollViewerPointerPressed;
            _scrollViewer.PointerReleased -= OnScrollViewerPointerReleased;
            _scrollViewer.PointerWheelChanged -= OnScrollViewerPointerWheelChanged;
        }

        SizeChanged -= OnSizeChanged;

        canvas.Paused = true;
        canvas.RemoveFromVisualTree();
        canvas = null;

        _lyricsRenderer.Dispose();
        DisposeRenderLyricsLines();
        _edgeFadeMaskRenderer.Dispose();
    }

    private void OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        var ds = args.DrawingSession;

        DrawCoreWithEdgeFeatheringHandled(sender, ds);
    }
    private void DrawCoreWithEdgeFeatheringHandled(ICanvasAnimatedControl sender, CanvasDrawingSession ds)
    {
        DrawCore(sender, ds);
    }
    private void DrawCore(ICanvasAnimatedControl sender, CanvasDrawingSession ds)
    {

        if (isLyricsVisible)
        {
            _lyricsRenderer.Draw(sender, ds);
        }
    }

    private void OnCreateResources(CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
    {
        RequestRelayout();
        TriggerRelayout();
    }

    private void OnUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        var lyricsData = currentLyricsData;

        TimeSpan elapsedTime = args.Timing.ElapsedTime;

        _immersiveBgOpacityTransition.Update(elapsedTime);
        _immersiveBgColorTransition.Update(elapsedTime);

        UpdatePlaybackState(elapsedTime);

        TriggerRelayout();

        #region UpdatePlayingLineIndex

        int primaryPlayingIndex = _synchronizer.GetCurrentLineIndex(_songPositionWithOffset.TotalMilliseconds, _renderLyricsLines);
        bool isPrimaryPlayingLineChanged = primaryPlayingIndex != _primaryPlayingLineIndex;
        _primaryPlayingLineIndex = primaryPlayingIndex;

        #endregion

        #region UpdateTargetScrollOffset

        if (isPrimaryPlayingLineChanged || _isLayoutChanged)
        {
            var targetScroll = CalculateTargetScrollOffset(_renderLyricsLines, _primaryPlayingLineIndex);
            if (targetScroll.HasValue) _canvasTargetScrollOffset = targetScroll.Value;

            if (_isLayoutChanged)
            {
                _canvasYScrollTransition.JumpTo(_canvasTargetScrollOffset);
            }
            else
            {
                _canvasYScrollTransition.SetDurationMs(lyricsScrollDuration);
                _canvasYScrollTransition.SetInterpolator(AnimationEasingHelper.GetInterpolatorByEasingType<double>(lyricsScrollEasingType, lyricsScrollEasingMode));
                _canvasYScrollTransition.Start(_canvasTargetScrollOffset);
            }
        }
        _canvasYScrollTransition.Update(elapsedTime);

        #endregion

        _mouseYScrollTransition.Update(elapsedTime);

        _mouseHoverLineIndex = FindMouseHoverLineIndex(
            _renderLyricsLines,
            isMouseInLyricsArea,
            mousePosition,
            _canvasYScrollTransition.Value + _mouseYScrollTransition.Value,
            renderLyricsHeight,
            playingLineTopOffset / 100.0
        );

        _visibleRange = CalculateVisibleRange(
            _renderLyricsLines,
            _canvasYScrollTransition.Value + _mouseYScrollTransition.Value,
            renderLyricsStartY,
            renderLyricsHeight,
            sender.Size.Height,
            playingLineTopOffset / 100.0
        );

        var maxRange = CalculateMaxRange(_renderLyricsLines);

        UpdateLines(
            _renderLyricsLines,
            isMouseScrolling ? maxRange.Start : _visibleRange.Start,
            isMouseScrolling ? maxRange.End : _visibleRange.End,
            _primaryPlayingLineIndex,
            renderLyricsWidth,
            renderLyricsHeight,
            _canvasTargetScrollOffset,
            playingLineTopOffset / 100.0,            
            _canvasYScrollTransition,            
            elapsedTime,
            isMouseScrolling,
            _isLayoutChanged,
            isPrimaryPlayingLineChanged,
            isMouseScrollingChanged,
            _isNowPlayingPaletteChanged,
            _songPositionWithOffset.TotalMilliseconds
        );

        isMouseScrollingChanged = false;
        _isNowPlayingPaletteChanged = false;

        if (isLyricsVisible)
        {
            _lyricsRenderer.CalculateLyrics3DMatrix(_isLayoutChanged);
        }

        _isLayoutChanged = false;
        
        if (isLyricsVisible)
        {
            _lyricsRenderer.MouseHoverLineIndex = _mouseHoverLineIndex;
            _lyricsRenderer.IsMousePressing = isMousePressing;
            _lyricsRenderer.StartVisibleLineIndex = _visibleRange.Start;
            _lyricsRenderer.EndVisibleLineIndex = _visibleRange.End;
            _lyricsRenderer.UserScrollOffset = _mouseYScrollTransition.Value;
            _lyricsRenderer.LyricsX = renderLyricsStartX;
            _lyricsRenderer.LyricsY = renderLyricsStartY;
            _lyricsRenderer.LyricsWidth = renderLyricsWidth;
            _lyricsRenderer.LyricsHeight = renderLyricsHeight;
            _lyricsRenderer.IsLyricsVisible = isLyricsVisible;
            _lyricsRenderer.PlayingLineTopOffsetFactor = playingLineTopOffset / 100.0;
            _lyricsRenderer.CurrentProgressMs = _songPositionWithOffset.TotalMilliseconds;
            _lyricsRenderer.RenderConfig = new LyricsRenderConfig(wordByWordEffectMode, lyricsLineContentOrientation, is3DLyricsEnabled, autoWrap, isLyricsBrethingEffectEnabled, isLyricsFloatAnimationEnabled, isLyricsGlowEffectEnabled, isLyricsScaleEffectEnabled, isRightToLeftLyric, isFanLyricsEnabled, fanLyricsAngle, lyricsFontStrokeWidth, playingLineTopOffset, lyrics3DXAngle, lyrics3DYAngle, lyrics3DZAngle, lyrics3DDepth);
            _lyricsRenderer.RenderLyricsLines = _renderLyricsLines;
            _lyricsRenderer.Update(sender, 0, lyricsBreathingIntensity);
        }
    }

    private void UpdatePlaybackState(TimeSpan elapsedTime)
    {
        if (currentIsPlaying)
        {
            _songPosition += elapsedTime;
            _songPositionWithOffset = _songPosition + TimeSpan.FromMilliseconds(positionOffset);
        }
    }

    private void TriggerRelayout()
    {
        if (!_isLayoutChanged) return;

        DisposeRenderLyricsLines();
        _renderLyricsLines = currentLyricsData?.LyricsLines.Select(x => new RenderLyricsLine(x)).ToList();

        EnsureRenderLyricsLinesPreservedAnimation();

        if (_renderLyricsLines == null) return;

        CalculateLanes(_renderLyricsLines);

        MeasureAndArrange(
            resourceCreator: canvas,
            lines: _renderLyricsLines,
            canvasWidth: canvas.Size.Width,
            canvasHeight: canvas.Size.Height,
            lyricsWidth: renderLyricsWidth,
            lyricsHeight: renderLyricsHeight
        );
    }

    private void RequestRelayout()
    {
        _layoutTimer.Debounce(() =>
        {
            _isLayoutChanged = true;
        }, TimeSpan.FromMilliseconds(400));
    }

    private void ResetPlaybackState()
    {
        _songPosition = TimeSpan.Zero;
    }

    private void DisposeRenderLyricsLines()
    {
        if (_renderLyricsLines != null)
        {
            foreach (var item in _renderLyricsLines)
            {
                item.DisposeTextGeometry();
                item.DisposeTextLayout();
                item.DisposeCaches();
            }
            _renderLyricsLines = null;
        }
    }

    private void UpdateCurrentPosition()
    {
        var diff = Math.Abs(_songPosition.TotalMilliseconds - currentPosition.TotalMilliseconds);

        if (diff >= timelineSyncThreshold)
        {
            _songPosition = currentPosition;
        }

        if (diff >= timelineSyncThreshold + 5000)
        {
            RequestRelayout();
        }
    }

    public void ApplyLayoutFromContainer(FrameworkElement lyricsContainer, FrameworkElement root)
    {
        var transform = lyricsContainer.TransformToVisual(root);
        var localRect = new Windows.Foundation.Rect(0, 0, this.ActualWidth, this.ActualHeight);
        var relativeRect = transform.TransformBounds(localRect);

        renderLyricsStartX = relativeRect.X;
        renderLyricsStartY = relativeRect.Y;
        renderLyricsWidth = lyricsContainer.ActualWidth;
        renderLyricsHeight = lyricsContainer.ActualHeight;

        if (lyrics3DAutoFitLayout)
        {
            (renderLyricsHeight, renderLyricsWidth) = (renderLyricsWidth, renderLyricsHeight);
            renderLyricsStartX += (renderLyricsHeight - renderLyricsWidth) / 2;
            renderLyricsStartY += (renderLyricsWidth - renderLyricsHeight) / 2;
        }
    }

    private void EnsureRenderLyricsLinesPreservedAnimation()
    {
        if (_renderLyricsLines == null) return;

        if (!isLyricsScaleEffectEnabled && !isLyricsGlowEffectEnabled) return;

        int animationPadding = (int)BetterLyricTimeSpanHelper.AnimationDuration.TotalMilliseconds;
        int longSyllableThreshold = Math.Max(
            lyricsScaleEffectLongSyllableDuration,
            lyricsGlowEffectLongSyllableDuration
        );

        var lines = _renderLyricsLines;
        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            if (line == null) continue;

            bool isLastLine = i + 1 >= lines.Count;
            var nextLineStartMs = isLastLine ? (int)currentLyricsData.Duration.TotalMilliseconds : lines[i + 1].StartMs;

            bool isLongSyllable = line.PrimaryRenderSyllables.LastOrDefault()?.DurationMs > longSyllableThreshold;

            if (line.EndMs.HasValue && isLongSyllable)
            {
                if (isLastLine || line.EndMs > nextLineStartMs)
                {
                    line.EndMs += animationPadding;
                }
                else
                {
                    int targetEndMs = line.EndMs.Value + animationPadding;

                    line.EndMs = Math.Max(line.EndMs.Value, Math.Min(targetEndMs, nextLineStartMs));
                }
            }
        }
    }
}
