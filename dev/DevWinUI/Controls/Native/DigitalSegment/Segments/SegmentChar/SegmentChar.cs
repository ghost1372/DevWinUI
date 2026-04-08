using System.Text;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

public abstract partial class SegmentChar : Control
{
    public event PointerEventHandler SegmentPointerPressed;
    public event PointerEventHandler SegmentPointerReleased;
    public event PointerEventHandler SegmentPointerEntered;
    public event PointerEventHandler SegmentPointerExited;
    public event PointerEventHandler SegmentPointerMoved;

    public event EventHandler<SegmentEventArgs> PatternChanged;

    private ScalarKeyFrameAnimation _blinkAnimation;
    protected Shape ColonTop => Segments?.FirstOrDefault(s => s.Name == DigitalSegmentHelper.COLON_TOP_KEY).Segment;
    protected Shape ColonBottom => Segments?.FirstOrDefault(s => s.Name == DigitalSegmentHelper.COLON_BOTTOM_KEY).Segment;

    public abstract SegmentChar Clone();

    protected List<(string Name, Shape Segment)> Segments { get; set; }

    private void StartColonBlink()
    {
        if (ColonTop == null || ColonBottom == null) return;

        var visualTop = ElementCompositionPreview.GetElementVisual(ColonTop);
        var visualBottom = ElementCompositionPreview.GetElementVisual(ColonBottom);

        visualTop.StartAnimation(nameof(visualTop.Opacity), _blinkAnimation);
        visualBottom.StartAnimation(nameof(visualBottom.Opacity), _blinkAnimation);
    }

    private void StopColonBlink()
    {
        if (ColonTop == null || ColonBottom == null) return;

        var visualTop = ElementCompositionPreview.GetElementVisual(ColonTop);
        var visualBottom = ElementCompositionPreview.GetElementVisual(ColonBottom);

        visualTop.StopAnimation(nameof(visualTop.Opacity));
        visualBottom.StopAnimation(nameof(visualBottom.Opacity));

        // Reset to visible
        visualTop.Opacity = 1;
        visualBottom.Opacity = 1;
    }
    protected virtual void InitColonAnimation()
    {
        var _compositor = CompositionTarget.GetCompositorForCurrentThread();

        _blinkAnimation = _compositor.CreateScalarKeyFrameAnimation();
        _blinkAnimation.InsertKeyFrame(0.0f, 1f);
        _blinkAnimation.InsertKeyFrame(0.5f, 0f);
        _blinkAnimation.InsertKeyFrame(1.0f, 1f);
        _blinkAnimation.Duration = TimeSpan.FromSeconds(1);
        _blinkAnimation.IterationBehavior = AnimationIterationBehavior.Forever;

        if (IsColonBlink)
            StartColonBlink();
    }

    protected abstract IReadOnlyDictionary<string, string> PatternTable { get; }
    protected abstract string DefaultPattern { get; }
    protected virtual int Columns {  get; set; }
    protected virtual int Rows {  get; set; }
    protected virtual void ReDrawMatrix() { }
    protected abstract void CollectSegments();

    protected virtual void UpdateCharacter()
    {
        if (Segments == null || Segments.Count == 0)
            CollectSegments();

        var key = Character ?? string.Empty;

        if (!PatternTable.TryGetValue(key, out string pattern))
        {
            if (!PatternTable.TryGetValue(key.ToUpperInvariant(), out pattern))
            {
                pattern = DefaultPattern;
            }
        }

        ApplyPattern(pattern);
    }

    protected void ApplyPattern(string pattern)
    {
        if (Segments == null || Segments.Count == 0)
            return;

        if (pattern.Length != Segments.Count)
            throw new ArgumentException($"Pattern must be {Segments.Count} chars long.");

        for (int i = 0; i < Segments.Count; i++)
        {
            var (key, seg) = Segments[i];
            if (seg == null) continue;

            bool active = pattern[i] == '1';

            seg.Tag = active;

            if (key.Equals(DigitalSegmentHelper.COLON_TOP_KEY) || key.Equals(DigitalSegmentHelper.COLON_BOTTOM_KEY))
            {
                seg.Fill = active ? ColonForeground : ColonBackground;
            }
            else
            {
                seg.Fill = active ? SegmentForeground : SegmentBackground;
            }
        }
    }

    private void UnRegisterPointerEvent()
    {
        if (Segments == null || Segments.Count == 0)
            return;

        foreach (var item in Segments)
        {
            item.Segment.PointerPressed -= OnPointerPressed;
            item.Segment.PointerEntered -= OnPointerEntered;
            item.Segment.PointerExited -= OnPointerExited;
            item.Segment.PointerMoved -= OnPointerMoved;
            item.Segment.PointerReleased -= OnPointerReleased;
        }
    }
    public void RegisterPointerEvents()
    {
        if (Segments == null || Segments.Count == 0)
            return;

        foreach (var item in Segments)
        {
            if (item.Segment != null)
            {
                item.Segment.PointerPressed -= OnPointerPressed;
                item.Segment.PointerPressed += OnPointerPressed;

                item.Segment.PointerEntered -= OnPointerEntered;
                item.Segment.PointerEntered += OnPointerEntered;

                item.Segment.PointerExited -= OnPointerExited;
                item.Segment.PointerExited -= OnPointerExited;

                item.Segment.PointerMoved -= OnPointerMoved;
                item.Segment.PointerMoved -= OnPointerMoved;

                item.Segment.PointerReleased -= OnPointerReleased;
                item.Segment.PointerReleased -= OnPointerReleased;
            }
        }
    }

    private void OnPointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        SegmentPointerExited?.Invoke(sender, e);
    }
    private void OnPointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        SegmentPointerMoved?.Invoke(sender, e);
    }
    private void OnPointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        SegmentPointerReleased?.Invoke(sender, e);

    }
    private void OnPointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        SegmentPointerEntered?.Invoke(sender, e);
    }
    private void OnPointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        SegmentPointerPressed?.Invoke(sender, e);
    }
    public void ToggleSegmentState(Shape shape)
    {
        bool isActive = (shape.Tag as bool?) ?? false;
        isActive = !isActive;

        SetSegmentState(shape, isActive);
    }
    public void SetSegmentState(Shape shape, bool isActive)
    {
        shape.Tag = isActive;

        if (shape.Equals(DigitalSegmentHelper.COLON_TOP_KEY) || shape.Equals(DigitalSegmentHelper.COLON_BOTTOM_KEY))
        {
            shape.Fill = isActive ? ColonForeground : ColonBackground;
        }
        else
        {
            shape.Fill = isActive ? SegmentForeground : SegmentBackground;
        }
        string pattern = GetCurrentPattern();
        PatternChanged?.Invoke(this, new SegmentEventArgs { Pattern = pattern });
    }
    private string GetCurrentPattern()
    {
        var sb = new StringBuilder();

        foreach (var (name, seg) in Segments)
        {
            bool active = (seg?.Tag as bool?) == true;
            sb.Append(active ? "1" : "0");
        }

        return sb.ToString();
    }

    public void SetPattern(string pattern) => ApplyPattern(pattern);

    protected SegmentChar()
    {
        ActualThemeChanged -= SegmentChar_ActualThemeChanged;
        ActualThemeChanged += SegmentChar_ActualThemeChanged;
    }

    private void SegmentChar_ActualThemeChanged(FrameworkElement sender, object args)
    {
        UpdateCharacter();
    }
}
