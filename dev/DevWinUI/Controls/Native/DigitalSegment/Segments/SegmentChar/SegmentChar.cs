using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

public abstract partial class SegmentChar : Control
{
    private static readonly object ActiveTag = true;
    private static readonly object InactiveTag = false;

    private bool deferVisualUpdates;
    private bool requiresCharacterUpdate;
    private bool requiresMatrixRedraw;

    private List<(string Name, Shape Segment)> segments;
    private Shape colonTop;
    private Shape colonBottom;
    private ScalarKeyFrameAnimation _blinkAnimation;

    public event PointerEventHandler SegmentPointerPressed;
    public event PointerEventHandler SegmentPointerReleased;
    public event PointerEventHandler SegmentPointerEntered;
    public event PointerEventHandler SegmentPointerExited;
    public event PointerEventHandler SegmentPointerMoved;

    public event EventHandler<SegmentEventArgs> PatternChanged;

    public abstract SegmentChar Clone();

    protected Shape ColonTop => colonTop;
    protected Shape ColonBottom => colonBottom;

    protected List<(string Name, Shape Segment)> Segments
    {
        get { return segments; }
        set
        {
            segments = value;
            CacheColonShapes();
        }
    }

    protected abstract IReadOnlyDictionary<string, string> PatternTable { get; }
    protected abstract string DefaultPattern { get; }
    protected virtual int Columns { get; set; }
    protected virtual int Rows { get; set; }
    protected abstract void CollectSegments();

    /// <summary>
    /// Rebuilds the dot matrix (if needed) and re-applies the current character. No-op for non matrix characters.
    /// </summary>
    protected virtual void RefreshMatrix() { }

    protected SegmentChar()
    {
        ActualThemeChanged += SegmentChar_ActualThemeChanged;
    }

    private void CacheColonShapes()
    {
        colonTop = null;
        colonBottom = null;

        if (segments == null)
            return;

        foreach (var (name, segment) in segments)
        {
            if (name == DigitalSegmentHelper.COLON_TOP_KEY)
                colonTop = segment;
            else if (name == DigitalSegmentHelper.COLON_BOTTOM_KEY)
                colonBottom = segment;
        }
    }

    private void StartColonBlink()
    {
        if (_blinkAnimation == null || colonTop == null || colonBottom == null)
            return;

        var visualTop = ElementCompositionPreview.GetElementVisual(colonTop);
        var visualBottom = ElementCompositionPreview.GetElementVisual(colonBottom);

        visualTop.StartAnimation(nameof(visualTop.Opacity), _blinkAnimation);
        visualBottom.StartAnimation(nameof(visualBottom.Opacity), _blinkAnimation);
    }

    private void StopColonBlink()
    {
        if (colonTop == null || colonBottom == null)
            return;

        var visualTop = ElementCompositionPreview.GetElementVisual(colonTop);
        var visualBottom = ElementCompositionPreview.GetElementVisual(colonBottom);

        visualTop.StopAnimation(nameof(visualTop.Opacity));
        visualBottom.StopAnimation(nameof(visualBottom.Opacity));

        // Reset to visible
        visualTop.Opacity = 1;
        visualBottom.Opacity = 1;
    }

    protected virtual void InitColonAnimation()
    {
        var compositor = CompositionTarget.GetCompositorForCurrentThread();

        _blinkAnimation = compositor.CreateScalarKeyFrameAnimation();
        _blinkAnimation.InsertKeyFrame(0.0f, 1f);
        _blinkAnimation.InsertKeyFrame(0.5f, 0f);
        _blinkAnimation.InsertKeyFrame(1.0f, 1f);
        _blinkAnimation.Duration = TimeSpan.FromSeconds(1);
        _blinkAnimation.IterationBehavior = AnimationIterationBehavior.Forever;

        if (IsColonBlink)
            StartColonBlink();
    }

    internal void SynchronizeAppearanceFrom(DigitalSegment control)
    {
        deferVisualUpdates = true;

        Angle = control.Angle;
        ColonBackground = control.ColonBackground;
        ColonForeground = control.ColonForeground;
        SegmentForeground = control.SegmentForeground;
        SegmentBackground = control.SegmentBackground;
        Stroke = control.Stroke;
        StrokeThickness = control.StrokeThickness;
        IsColonBlink = control.IsColonBlink;
        MatrixDotGap = control.MatrixDotGap;
        IsMatrixSquare = control.IsMatrixSquare;
        MatrixDotSize = control.MatrixDotSize;

        deferVisualUpdates = false;

        if (requiresMatrixRedraw)
        {
            // RefreshMatrix also re-applies the character.
            requiresMatrixRedraw = false;
            requiresCharacterUpdate = false;
            RefreshMatrix();
        }
        else if (requiresCharacterUpdate)
        {
            requiresCharacterUpdate = false;
            UpdateCharacter();
        }
    }

    private void OnCharacterAppearanceChanged()
    {
        if (deferVisualUpdates)
        {
            requiresCharacterUpdate = true;
            return;
        }

        UpdateCharacter();
    }

    private void OnMatrixAppearanceChanged()
    {
        if (deferVisualUpdates)
        {
            requiresMatrixRedraw = true;
            return;
        }

        RefreshMatrix();
    }

    protected virtual void UpdateCharacter()
    {
        if (segments == null || segments.Count == 0)
            CollectSegments();

        ApplyPattern(GetPatternForCharacter(Character));
    }

    /// <summary>
    /// Finds the pattern for a character: exact match, then upper case, then the default pattern.
    /// </summary>
    protected string GetPatternForCharacter(string character)
    {
        var key = character ?? string.Empty;

        if (!PatternTable.TryGetValue(key, out string pattern) &&
            !PatternTable.TryGetValue(key.ToUpperInvariant(), out pattern))
        {
            pattern = DefaultPattern;
        }

        return pattern;
    }

    protected void ApplyPattern(string pattern)
    {
        if (segments == null || segments.Count == 0)
            return;

        if (pattern.Length != segments.Count)
            throw new ArgumentException($"Pattern must be {segments.Count} chars long.");

        for (int i = 0; i < segments.Count; i++)
        {
            var seg = segments[i].Segment;
            if (seg == null)
                continue;

            bool active = pattern[i] == '1';
            bool isColon = ReferenceEquals(seg, colonTop) || ReferenceEquals(seg, colonBottom);

            seg.Tag = active ? ActiveTag : InactiveTag;
            seg.Fill = isColon
                ? (active ? ColonForeground : ColonBackground)
                : (active ? SegmentForeground : SegmentBackground);
        }
    }

    private void UnregisterPointerEvents()
    {
        if (segments == null || segments.Count == 0)
            return;

        foreach (var item in segments)
        {
            if (item.Segment == null)
                continue;

            item.Segment.PointerPressed -= OnPointerPressed;
            item.Segment.PointerEntered -= OnPointerEntered;
            item.Segment.PointerExited -= OnPointerExited;
            item.Segment.PointerMoved -= OnPointerMoved;
            item.Segment.PointerReleased -= OnPointerReleased;
        }
    }

    public void RegisterPointerEvents()
    {
        if (segments == null || segments.Count == 0)
            return;

        foreach (var item in segments)
        {
            var segment = item.Segment;
            if (segment == null)
                continue;

            segment.PointerPressed -= OnPointerPressed;
            segment.PointerPressed += OnPointerPressed;

            segment.PointerEntered -= OnPointerEntered;
            segment.PointerEntered += OnPointerEntered;

            segment.PointerExited -= OnPointerExited;
            segment.PointerExited += OnPointerExited;

            segment.PointerMoved -= OnPointerMoved;
            segment.PointerMoved += OnPointerMoved;

            segment.PointerReleased -= OnPointerReleased;
            segment.PointerReleased += OnPointerReleased;
        }
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs e) => SegmentPointerExited?.Invoke(sender, e);
    private void OnPointerMoved(object sender, PointerRoutedEventArgs e) => SegmentPointerMoved?.Invoke(sender, e);
    private void OnPointerReleased(object sender, PointerRoutedEventArgs e) => SegmentPointerReleased?.Invoke(sender, e);
    private void OnPointerEntered(object sender, PointerRoutedEventArgs e) => SegmentPointerEntered?.Invoke(sender, e);
    private void OnPointerPressed(object sender, PointerRoutedEventArgs e) => SegmentPointerPressed?.Invoke(sender, e);

    public void ToggleSegmentState(Shape shape)
    {
        bool isActive = (shape.Tag as bool?) ?? false;
        SetSegmentState(shape, !isActive);
    }

    public void SetSegmentState(Shape shape, bool isActive)
    {
        shape.Tag = isActive ? ActiveTag : InactiveTag;

        if (ReferenceEquals(shape, colonTop) || ReferenceEquals(shape, colonBottom))
        {
            shape.Fill = isActive ? ColonForeground : ColonBackground;
        }
        else
        {
            shape.Fill = isActive ? SegmentForeground : SegmentBackground;
        }

        // Building the pattern string is only worth it when somebody listens.
        PatternChanged?.Invoke(this, new SegmentEventArgs(GetCurrentPattern()));
    }

    private string GetCurrentPattern()
    {
        if (segments == null || segments.Count == 0)
            return string.Empty;

        return string.Create(segments.Count, segments, static (span, list) =>
        {
            for (int i = 0; i < span.Length; i++)
                span[i] = list[i].Segment?.Tag is true ? '1' : '0';
        });
    }

    public void SetPattern(string pattern) => ApplyPattern(pattern);

    private void SegmentChar_ActualThemeChanged(FrameworkElement sender, object args)
    {
        UpdateCharacter();
    }
}
