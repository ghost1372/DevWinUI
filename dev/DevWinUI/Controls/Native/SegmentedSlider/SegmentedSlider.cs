using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_HorizontalThumb), Type = typeof(Thumb))]
[TemplatePart(Name = nameof(PART_ItemsRepeater), Type = typeof(ItemsRepeater))]
public partial class SegmentedSlider : Control
{
    private const string PART_ItemsRepeater = "PART_ItemsRepeater";
    private const string PART_HorizontalThumb = "PART_HorizontalThumb";

    private ItemsRepeater itemsRepeater;
    private Thumb horizontalThumb;

    public event EventHandler<SegmentChangedEventArgs>? SegmentChanged;
    public event EventHandler<double>? ValueChanged;
    public event EventHandler<TimeSpan>? SelectedTimeChanged;
    public event EventHandler? DragStarted;
    public event EventHandler? DragCompleted;

    private int _lastSegmentIndex = -1;
    private bool _isTemplateReady;
    private bool _isSyncing;
    private bool _isRefreshQueued;
    private IList<SegmentedSliderTimeInfo> internalSegments = new List<SegmentedSliderTimeInfo>();

    public SegmentedSlider()
    {
        DefaultStyleKey = typeof(SegmentedSlider);

        SegmentTitles = new ObservableCollection<string>();
        TimeSegments = new ObservableCollection<SegmentedSliderTimeInfo>();

        PointerPressed += OnPointerPressed;
        SizeChanged += OnSizeChanged;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (horizontalThumb != null)
        {
            horizontalThumb.DragStarted -= HorizontalThumb_DragStarted;
            horizontalThumb.DragCompleted -= HorizontalThumb_DragCompleted;
            horizontalThumb.DragDelta -= HorizontalThumb_DragDelta;
        }

        if (itemsRepeater != null)
        {
            itemsRepeater.ElementPrepared -= ItemsRepeater_ElementPrepared;
            itemsRepeater.ElementIndexChanged -= ItemsRepeater_ElementIndexChanged;
        }

        itemsRepeater = GetTemplateChild(PART_ItemsRepeater) as ItemsRepeater;
        horizontalThumb = GetTemplateChild(PART_HorizontalThumb) as Thumb;

        if (horizontalThumb != null)
        {
            horizontalThumb.DragStarted += HorizontalThumb_DragStarted;
            horizontalThumb.DragCompleted += HorizontalThumb_DragCompleted;
            horizontalThumb.DragDelta += HorizontalThumb_DragDelta;
        }

        if (itemsRepeater != null)
        {
            itemsRepeater.ElementPrepared += ItemsRepeater_ElementPrepared;
            itemsRepeater.ElementIndexChanged += ItemsRepeater_ElementIndexChanged;
        }

        _isTemplateReady = true;

        UpdateSegments();
        InitSelectedTime();
        UpdateThumbPosition();
        UpdateSegmentsFill();
    }

    private double GetThumbTravel()
    {
        return Math.Max(0, itemsRepeater.ActualWidth - horizontalThumb.ActualWidth);
    }

    private int GetSegmentCount()
    {
        return internalSegments.Count > 0 ? internalSegments.Count : SegmentCount;
    }

    private void QueueRefresh()
    {
        if (_isRefreshQueued || DispatcherQueue == null)
            return;

        _isRefreshQueued = DispatcherQueue.TryEnqueue(() =>
        {
            _isRefreshQueued = false;
            UpdateThumbPosition();
            UpdateSegmentsFill();
        });
    }

    private void OnPointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (itemsRepeater == null || horizontalThumb == null || Maximum <= 0)
            return;

        double x = e.GetCurrentPoint(itemsRepeater).Position.X;
        double maxX = GetThumbTravel();

        Value = maxX > 0 ? Math.Clamp(x, 0, maxX) / maxX * Maximum : 0;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (!_isTemplateReady || itemsRepeater == null || horizontalThumb == null)
            return;

        UpdateThumbPosition();
        UpdateSegmentsFill();
    }

    private void HorizontalThumb_DragStarted(object sender, DragStartedEventArgs e)
    {
        DragStarted?.Invoke(this, EventArgs.Empty);
    }

    private void HorizontalThumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        DragCompleted?.Invoke(this, EventArgs.Empty);
    }

    private void HorizontalThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (itemsRepeater == null || horizontalThumb == null || Maximum <= 0)
            return;

        double maxX = GetThumbTravel();
        double currentX = (horizontalThumb.RenderTransform as TranslateTransform)?.X ?? 0;

        Value = maxX > 0 ? Math.Clamp(currentX + e.HorizontalChange, 0, maxX) / maxX * Maximum : 0;
    }

    private void OnSegmentTitlesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        UpdateSegments();
    }

    private void OnTimeSegmentsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        NormalizeTimeSegments();
        UpdateSegments();
    }

    private void ItemsRepeater_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        UpdateThumbPosition();

        if (args.Element is Grid hostGrid)
        {
            UpdateSingleSegmentFill(hostGrid);
        }

        QueueRefresh();
    }

    private void ItemsRepeater_ElementIndexChanged(ItemsRepeater sender, ItemsRepeaterElementIndexChangedEventArgs args)
    {
        UpdateThumbPosition();
        UpdateSegmentsFill();
    }

    private void InitSelectedTime()
    {
        if (TotalTime == TimeSpan.Zero || Maximum <= 0 || internalSegments.Count == 0)
            return;

        bool oldSync = _isSyncing;
        _isSyncing = true;

        try
        {
            if (SelectedTime > TotalTime)
                SelectedTime = TotalTime;
            else if (SelectedTime < TimeSpan.Zero)
                SelectedTime = TimeSpan.Zero;

            if (SelectedTime > TimeSpan.Zero && Value == 0)
            {
                Value = (SelectedTime.TotalMilliseconds / TotalTime.TotalMilliseconds) * Maximum;
                SelectedTimeChanged?.Invoke(this, SelectedTime);
            }
            else if (Value > 0 && SelectedTime == TimeSpan.Zero)
            {
                SelectedTime = TimeSpan.FromMilliseconds((Value / Maximum) * TotalTime.TotalMilliseconds);
                SelectedTimeChanged?.Invoke(this, SelectedTime);
            }
        }
        finally
        {
            _isSyncing = oldSync;
        }
    }

    private void UpdateThumbPosition()
    {
        if (itemsRepeater == null || horizontalThumb == null)
            return;

        var transform = horizontalThumb.RenderTransform as TranslateTransform;

        if (Maximum > 0)
        {
            if (transform == null)
            {
                transform = new TranslateTransform();
                horizontalThumb.RenderTransform = transform;
            }

            double maxX = GetThumbTravel();
            transform.X = maxX > 0 ? (Value / Maximum) * maxX : 0;
        }
        else if (transform != null)
        {
            transform.X = 0;
        }
    }

    private static bool TryGetRectangles(Grid hostGrid, out Rectangle trackRect, out Rectangle fillRect)
    {
        trackRect = null;
        fillRect = null;

        if (hostGrid.Children.Count < 2)
            return false;

        trackRect = hostGrid.Children[0] as Rectangle;
        fillRect = hostGrid.Children[1] as Rectangle;
        return trackRect != null && fillRect != null;
    }

    private static void ApplyFill(Rectangle fillRect, double totalFillPixels, double segmentStart, double segmentWidth)
    {
        double width = Math.Clamp(totalFillPixels - segmentStart, 0, segmentWidth);
        if (fillRect.Width != width)
        {
            fillRect.Width = width;
        }
    }

    private void UpdateSingleSegmentFill(Grid hostGrid)
    {
        if (itemsRepeater == null || Maximum <= 0)
            return;

        if (!TryGetRectangles(hostGrid, out var trackRect, out var fillRect))
            return;

        double trackWidth = itemsRepeater.ActualWidth;
        if (trackWidth <= 0)
            return;

        double segmentStart = 0;
        try
        {
            segmentStart = hostGrid.TransformToVisual(itemsRepeater).TransformPoint(new Point(0, 0)).X;
        }
        catch
        {
        }

        ApplyFill(fillRect, (Value / Maximum) * trackWidth, segmentStart, trackRect.ActualWidth);
    }

    private void UpdateSegmentsFill()
    {
        if (itemsRepeater == null || Maximum <= 0)
            return;

        double trackWidth = itemsRepeater.ActualWidth;
        if (trackWidth <= 0)
            return;

        int count = GetSegmentCount();
        if (count <= 0)
            return;

        double totalFillPixels = (Value / Maximum) * trackWidth;
        double filledSoFar = 0;

        for (int i = 0; i < count; i++)
        {
            if (itemsRepeater.TryGetElement(i) is not Grid hostGrid || !TryGetRectangles(hostGrid, out var trackRect, out var fillRect))
                continue;

            double segmentWidth = trackRect.ActualWidth;
            ApplyFill(fillRect, totalFillPixels, filledSoFar + (i * Spacing), segmentWidth);
            filledSoFar += segmentWidth;
        }
    }

    private void UpdateSegments()
    {
        if (itemsRepeater == null)
            return;

        var labels = new List<string>();
        var segmentWidths = new List<double>();

        if (internalSegments.Count > 0 && TotalTime > TimeSpan.Zero)
        {
            foreach (var seg in internalSegments)
            {
                labels.Add(seg.Title);
                segmentWidths.Add((seg.EndTime - seg.StartTime).TotalMilliseconds / TotalTime.TotalMilliseconds);
            }
        }
        else
        {
            for (int i = 0; i < SegmentCount; i++)
            {
                labels.Add(SegmentTitles != null && i < SegmentTitles.Count ? SegmentTitles[i] : null);
                segmentWidths.Add(1.0 / SegmentCount);
            }
        }

        itemsRepeater.ItemsSource = labels;
        itemsRepeater.UpdateLayout();

        if (itemsRepeater.Layout is HorizontalSimpleGridLayout layout)
        {
            layout.ColumnWidths.Clear();

            foreach (var width in segmentWidths)
                layout.ColumnWidths.Add(new GridLength(width, GridUnitType.Star));

            if (Maximum > 0)
            {
                if (Value < 0)
                    Value = 0;
                else if (Value > Maximum)
                    Value = Maximum;
            }

            QueueRefresh();
            itemsRepeater.InvalidateMeasure();
            itemsRepeater.InvalidateArrange();
        }

        UpdateSegmentLabelsVisibility();
        UpdateTitleHorizontalAlignment();
        _lastSegmentIndex = -1;
    }

    private void OnSelectedTimeChanged(TimeSpan newTime)
    {
        if (internalSegments.Count == 0)
            return;

        if (_isSyncing || TotalTime == TimeSpan.Zero || Maximum <= 0)
            return;

        _isSyncing = true;
        try
        {
            if (newTime < TimeSpan.Zero)
                newTime = TimeSpan.Zero;
            else if (newTime > TotalTime)
                newTime = TotalTime;

            var newValue = (newTime.TotalMilliseconds / TotalTime.TotalMilliseconds) * Maximum;
            if (Math.Abs(Value - newValue) > 0.001)
                Value = newValue;

            if (SelectedTime != newTime)
                SetValue(SelectedTimeProperty, newTime);
        }
        finally
        {
            _isSyncing = false;
        }

        SelectedTimeChanged?.Invoke(this, newTime);
    }

    private void OnMaximumChanged()
    {
        if (!_isTemplateReady)
            return;

        if (Maximum > 0 && Value > Maximum)
        {
            Value = Maximum;
            return;
        }

        UpdateThumbPosition();
        UpdateSegmentsFill();
    }

    private void OnValueChanged()
    {
        if (Maximum <= 0)
            return;

        if (Value < 0)
        {
            Value = 0;
            return;
        }

        if (Value > Maximum)
        {
            Value = Maximum;
            return;
        }

        if (!_isSyncing && internalSegments.Count > 0 && TotalTime > TimeSpan.Zero)
        {
            _isSyncing = true;
            try
            {
                SelectedTime = TimeSpan.FromMilliseconds((Value / Maximum) * TotalTime.TotalMilliseconds);
            }
            finally
            {
                _isSyncing = false;
            }
        }

        if (_isTemplateReady)
        {
            UpdateThumbPosition();
            UpdateSegmentsFill();

            int currentIndex = CurrentSegmentIndex();
            if (currentIndex != _lastSegmentIndex)
            {
                _lastSegmentIndex = currentIndex;

                var currentSegment = GetSegment(currentIndex);
                if (currentSegment != null)
                {
                    var timeInfo = currentIndex < internalSegments.Count ? internalSegments[currentIndex] : null;
                    SegmentChanged?.Invoke(this, new SegmentChangedEventArgs(currentIndex, currentSegment, timeInfo));
                }

                UpdateSegmentLabelsVisibility();
            }
        }

        ValueChanged?.Invoke(this, Value);
    }

    private void NormalizeTimeSegments()
    {
        if (TimeSegments == null || TimeSegments.Count == 0 || TotalTime == TimeSpan.Zero)
        {
            internalSegments = new List<SegmentedSliderTimeInfo>();
            return;
        }

        var normalized = new List<SegmentedSliderTimeInfo>();
        TimeSpan lastEnd = TimeSpan.Zero;

        foreach (var seg in TimeSegments.OrderBy(s => s.StartTime))
        {
            var start = seg.StartTime < lastEnd ? lastEnd : seg.StartTime;
            var end = seg.EndTime > TotalTime ? TotalTime : seg.EndTime;

            if (end <= start)
                continue;

            if (start > lastEnd)
            {
                normalized.Add(new SegmentedSliderTimeInfo
                {
                    StartTime = lastEnd,
                    EndTime = start,
                    Title = string.Empty
                });
            }

            normalized.Add(new SegmentedSliderTimeInfo
            {
                StartTime = start,
                EndTime = end,
                Title = seg.Title
            });
            lastEnd = end;
        }

        if (lastEnd < TotalTime)
        {
            normalized.Add(new SegmentedSliderTimeInfo
            {
                StartTime = lastEnd,
                EndTime = TotalTime,
                Title = string.Empty
            });
        }

        internalSegments = normalized;
    }

    private IEnumerable<(int Index, Grid Grid)> EnumerateSegmentGrids()
    {
        if (itemsRepeater?.ItemsSource is not System.Collections.ICollection collection)
            yield break;

        for (int i = 0; i < collection.Count; i++)
        {
            if (itemsRepeater.GetOrCreateElement(i) is Grid hostGrid)
                yield return (i, hostGrid);
        }
    }

    private void UpdateSegmentLabelsVisibility()
    {
        if (itemsRepeater == null)
            return;

        var visibility = TitleVisibility;
        int currentIndex = visibility is SegmentedSliderTitleVisibility.VisibleCurrentSegment or SegmentedSliderTitleVisibility.VisibleCurrentAndPrevious
            ? CurrentSegmentIndex()
            : -1;

        foreach (var (index, hostGrid) in EnumerateSegmentGrids())
        {
            if (hostGrid.Children.Count <= 2)
                continue;

            hostGrid.Children[2].Visibility = visibility switch
            {
                SegmentedSliderTitleVisibility.Collapsed => Visibility.Collapsed,
                SegmentedSliderTitleVisibility.AlwaysVisible => Visibility.Visible,
                SegmentedSliderTitleVisibility.VisibleCurrentSegment => index == currentIndex ? Visibility.Visible : Visibility.Collapsed,
                SegmentedSliderTitleVisibility.VisibleCurrentAndPrevious => index <= currentIndex ? Visibility.Visible : Visibility.Collapsed,
                _ => hostGrid.Children[2].Visibility
            };
        }
    }

    private void UpdateTitleHorizontalAlignment()
    {
        foreach (var (_, hostGrid) in EnumerateSegmentGrids())
        {
            if (hostGrid.Children.Count > 2 && hostGrid.Children[2] is TextBlock title)
                title.HorizontalAlignment = TitleHorizontalAlignment;
        }
    }

    private int CurrentSegmentIndex()
    {
        if (Maximum <= 0)
            return 0;

        double ratio = Value / Maximum;

        if (internalSegments.Count > 0 && TotalTime > TimeSpan.Zero)
        {
            TimeSpan currentTime = TimeSpan.FromMilliseconds(ratio * TotalTime.TotalMilliseconds);

            for (int i = 0; i < internalSegments.Count; i++)
            {
                var seg = internalSegments[i];
                if (currentTime >= seg.StartTime && currentTime < seg.EndTime)
                    return i;
            }

            return internalSegments.Count - 1;
        }

        if (SegmentCount <= 0)
            return 0;

        return Math.Min((int)Math.Floor(ratio * SegmentCount), SegmentCount - 1);
    }

    public ItemsRepeater GetItemsRepeater()
    {
        return itemsRepeater;
    }

    private static SegmentedSliderSegment CreateSegment(Grid hostGrid)
    {
        return new SegmentedSliderSegment
        {
            TrackRectangle = hostGrid.Children[0] as Rectangle,
            FillSegmentRectangle = hostGrid.Children[1] as Rectangle,
            TitleTextBlock = hostGrid.Children[2] as TextBlock
        };
    }

    private SegmentedSliderSegment GetSegment(int index)
    {
        if (itemsRepeater?.ItemsSource is not System.Collections.ICollection collection || index < 0 || index >= collection.Count)
            return null;

        return itemsRepeater.GetOrCreateElement(index) is Grid hostGrid && hostGrid.Children.Count > 2
            ? CreateSegment(hostGrid)
            : null;
    }

    public List<SegmentedSliderSegment> GetSegments()
    {
        if (itemsRepeater?.ItemsSource is not System.Collections.ICollection { Count: > 0 })
            return null;

        var segments = new List<SegmentedSliderSegment>();

        foreach (var (_, hostGrid) in EnumerateSegmentGrids())
        {
            if (hostGrid.Children.Count > 2)
                segments.Add(CreateSegment(hostGrid));
        }

        return segments;
    }

    // not set yet
    private void SetSegmentRectanglesStyle()
    {
        if (itemsRepeater == null || itemsRepeater.ItemsSource is not System.Collections.ICollection collection || collection.Count == 0)
            return;

        int itemCount = collection.Count;

        for (int i = 0; i < itemCount; i++)
        {
            var element = itemsRepeater.GetOrCreateElement(i);
            if (element is not Grid hostGrid)
                continue;

            if (!TryGetRectangles(hostGrid, out var trackRect, out _))
                continue;

            if (i == 0)
            {
                ApplyFirstSegmentClip(trackRect);
            }
            else if (i == itemCount - 1)
            {
                ApplyLastSegmentClip(trackRect);
            }
            else
            {
                ApplyMiddleSegmentClip(trackRect, i == itemCount - 2);
            }
        }
    }

    private static void ApplyFirstSegmentClip(Rectangle rect)
    {
    }

    private static void ApplyMiddleSegmentClip(Rectangle rect, bool isLastItem)
    {
    }

    private static void ApplyLastSegmentClip(Rectangle rect)
    {
    }
}
