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

    public event EventHandler<double>? ValueChanged;
    public event EventHandler<TimeSpan>? SelectedTimeChanged;
    public event EventHandler? DragStarted;
    public event EventHandler? DragCompleted;

    private bool _isTemplateReady;
    private bool _isSyncing;
    private IList<SegmentedSliderTimeInfo> internalSegments = new List<SegmentedSliderTimeInfo>();

    public SegmentedSlider()
    {
        DefaultStyleKey = typeof(SegmentedSlider);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        itemsRepeater = GetTemplateChild(PART_ItemsRepeater) as ItemsRepeater;
        horizontalThumb = GetTemplateChild(PART_HorizontalThumb) as Thumb;

        horizontalThumb.DragStarted -= HorizontalThumb_DragStarted;
        horizontalThumb.DragStarted += HorizontalThumb_DragStarted;
        horizontalThumb.DragCompleted -= HorizontalThumb_DragCompleted;
        horizontalThumb.DragCompleted += HorizontalThumb_DragCompleted;
        horizontalThumb.DragDelta -= HorizontalThumb_DragDelta;
        horizontalThumb.DragDelta += HorizontalThumb_DragDelta;

        PointerPressed -= OnPointerPressed;
        PointerPressed += OnPointerPressed;

        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;

        _isTemplateReady = true;

        UpdateSegments();
        InitSelectedTime();
        UpdateThumbPosition();
        UpdateSegmentsFill();
        UpdateTitleHorizontalAlignment();
    }
    private void OnPointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (itemsRepeater == null || horizontalThumb == null || Maximum <= 0)
            return;

        // Get pointer position relative to the repeater (track)
        var point = e.GetCurrentPoint(itemsRepeater);
        double x = point.Position.X;

        double trackWidth = itemsRepeater.ActualWidth;
        double maxX = Math.Max(0, trackWidth - horizontalThumb.ActualWidth);

        x = Math.Max(0, Math.Min(x, maxX));

        Value = (x / maxX) * Maximum;

        // Move thumb and update fill
        UpdateThumbPosition();
        UpdateSegmentsFill();
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
        if (itemsRepeater == null || horizontalThumb == null)
            return;

        if (Maximum > 0)
        {
            double trackWidth = itemsRepeater.ActualWidth;

            var transform = horizontalThumb.RenderTransform as TranslateTransform;
            if (transform == null)
            {
                transform = new TranslateTransform();
                horizontalThumb.RenderTransform = transform;
            }

            double currentX = transform.X;
            currentX += e.HorizontalChange;

            double maxX = trackWidth - horizontalThumb.ActualWidth;
            currentX = Math.Max(0, Math.Min(currentX, maxX));

            Value = (currentX / maxX) * Maximum;
            UpdateSegmentsFill();

            transform.X = currentX;

            ValueChanged?.Invoke(this, Value);
        }
    }

    private void InitSelectedTime()
    {
        if (TotalTime == TimeSpan.Zero || Maximum <= 0)
            return;

        // Temporarily disable sync callbacks
        bool oldSync = _isSyncing;
        _isSyncing = true;

        try
        {
            if (internalSegments.Count > 0)
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

        if (Maximum > 0)
        {
            double trackWidth = itemsRepeater.ActualWidth;
            double maxX = trackWidth - horizontalThumb.ActualWidth;

            double thumbX = (Value / Maximum) * maxX;

            var transform = horizontalThumb.RenderTransform as TranslateTransform;
            if (transform == null)
            {
                transform = new TranslateTransform();
                horizontalThumb.RenderTransform = transform;
            }

            transform.X = thumbX;
        }
    }
    private void UpdateSegmentsFill()
    {
        if (itemsRepeater == null)
            return;

        double trackWidth = itemsRepeater.ActualWidth;

        double fillPixels = 0;
        int segmentCount = 0;
        double totalSpacing = 0;
        double usableWidth = 0;

        if (internalSegments.Count > 0 && TotalTime > TimeSpan.Zero)
        {
            segmentCount = internalSegments.Count;
            totalSpacing = (segmentCount - 1) * Spacing;
            usableWidth = trackWidth - totalSpacing;

            TimeSpan currentTime = TimeSpan.FromMilliseconds((Value / Maximum) * TotalTime.TotalMilliseconds);

            for (int i = 0; i < internalSegments.Count; i++)
            {
                var element = itemsRepeater.GetOrCreateElement(i);
                if (element is Grid hostGrid && hostGrid.Children.Count > 1)
                {
                    var fillRect = hostGrid.Children[1] as Rectangle;
                    if (fillRect != null)
                    {
                        double segmentStart = internalSegments[i].StartTime.TotalMilliseconds / TotalTime.TotalMilliseconds;
                        double segmentEnd = internalSegments[i].EndTime.TotalMilliseconds / TotalTime.TotalMilliseconds;

                        double segmentPixelStart = segmentStart * usableWidth + i * Spacing;
                        double segmentPixelEnd = segmentEnd * usableWidth + i * Spacing;

                        if (currentTime.TotalMilliseconds >= internalSegments[i].EndTime.TotalMilliseconds)
                            fillRect.Width = segmentPixelEnd - segmentPixelStart;
                        else if (currentTime.TotalMilliseconds <= internalSegments[i].StartTime.TotalMilliseconds)
                            fillRect.Width = 0;
                        else
                            fillRect.Width = (currentTime.TotalMilliseconds - internalSegments[i].StartTime.TotalMilliseconds) /
                                             (internalSegments[i].EndTime - internalSegments[i].StartTime).TotalMilliseconds *
                                             (segmentPixelEnd - segmentPixelStart);
                    }
                }
            }
        }
        else
        {
            segmentCount = SegmentCount;
            totalSpacing = (segmentCount - 1) * Spacing;
            usableWidth = trackWidth - totalSpacing;

            double valueRatio = Value / Maximum;
            fillPixels = valueRatio * usableWidth;

            for (int i = 0; i < segmentCount; i++)
            {
                var element = itemsRepeater.GetOrCreateElement(i);
                if (element is Grid hostGrid && hostGrid.Children.Count > 1)
                {
                    var fillRect = hostGrid.Children[1] as Rectangle;
                    if (fillRect != null)
                    {
                        double segmentWidth = usableWidth / segmentCount;
                        double segmentFill = Math.Min(fillPixels - i * segmentWidth, segmentWidth);
                        fillRect.Width = Math.Max(0, segmentFill);
                    }
                }
            }
        }
    }
    private void UpdateSegments()
    {
        if (itemsRepeater == null)
            return;

        List<string> labels = new List<string>();
        List<double> segmentWidths = new List<double>();

        if (internalSegments.Count > 0 && TotalTime > TimeSpan.Zero)
        {
            // Time-based segments
            foreach (var seg in internalSegments)
            {
                labels.Add(seg.Title);

                double widthRatio = (seg.EndTime - seg.StartTime).TotalMilliseconds / TotalTime.TotalMilliseconds;
                segmentWidths.Add(widthRatio);
            }
        }
        else
        {
            // Default logic with SegmentCount
            if (SegmentTitles != null && SegmentTitles.Count > 0)
            {
                for (int i = 0; i < SegmentCount; i++)
                {
                    if (i < SegmentTitles.Count)
                        labels.Add(SegmentTitles[i]);
                    else
                        labels.Add(null);
                }
            }
            else
            {
                for (int i = 0; i < SegmentCount; i++)
                    labels.Add(null);
            }

            for (int i = 0; i < SegmentCount; i++)
                segmentWidths.Add(1.0 / SegmentCount);
        }

        itemsRepeater.ItemsSource = labels;
        itemsRepeater.UpdateLayout();

        if (itemsRepeater.Layout is HorizontalSimpleGridLayout layout)
        {
            layout.ColumnWidths.Clear();

            // Apply column widths (like GridLength Stars)
            for (int i = 0; i < segmentWidths.Count; i++)
                layout.ColumnWidths.Add(new GridLength(segmentWidths[i], GridUnitType.Star));

            // Force layout recalculation
            itemsRepeater.InvalidateMeasure();
            itemsRepeater.InvalidateArrange();
        }

        UpdateSegmentLabelsVisibility();
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

            // Update Value to match SelectedTime
            var newValue = (newTime.TotalMilliseconds / TotalTime.TotalMilliseconds) * Maximum;
            if (Math.Abs(Value - newValue) > 0.001)
                Value = newValue;

            // Update the property itself in case it was out of bounds
            if (SelectedTime != newTime)
                SetValue(SelectedTimeProperty, newTime);
        }
        finally
        {
            _isSyncing = false;
        }

        SelectedTimeChanged?.Invoke(this, newTime);
    }
    private void OnValueChanged()
    {
        if (_isSyncing || Maximum <= 0)
            return;

        _isSyncing = true;

        if (internalSegments.Count > 0 && TotalTime > TimeSpan.Zero)
        {
            SelectedTime = TimeSpan.FromMilliseconds((Value / Maximum) * TotalTime.TotalMilliseconds);
        }

        _isSyncing = false;

        if (_isTemplateReady)
        {
            UpdateThumbPosition();
            UpdateSegmentsFill();
        }

        ValueChanged?.Invoke(this, Value);
    }
    private void NormalizeSegments()
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
            var start = seg.StartTime;
            var end = seg.EndTime;

            // Fix backward overlap or incorrect start (e.g. user entered 0:20:0 instead of 0:30:0)
            if (start < lastEnd)
                start = lastEnd;

            // Fix EndTimeSpan greater than TotalTimeSpan
            if (end > TotalTime)
                end = TotalTime;

            // Fix missing gap between previous end and current start (insert a gap segment)
            if (start > lastEnd)
            {
                normalized.Add(new SegmentedSliderTimeInfo
                {
                    StartTime = lastEnd,
                    EndTime = start,
                    Title = string.Empty
                });
            }

            // Ignore invalid zero-length or reversed segments
            if (end > start)
            {
                normalized.Add(new SegmentedSliderTimeInfo
                {
                    StartTime = start,
                    EndTime = end,
                    Title = seg.Title
                });
                lastEnd = end;
            }
        }

        // If last segment doesn't reach TotalTimeSpan, add a final segment to cover the end
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
    
    private void UpdateSegmentLabelsVisibility()
    {
        if (itemsRepeater == null)
            return;

        int itemCount = itemsRepeater.ItemsSource is System.Collections.ICollection collection
            ? collection.Count
            : 0;

        for (int i = 0; i < itemCount; i++)
        {
            var element = itemsRepeater.GetOrCreateElement(i);
            if (element is Grid hostGrid)
            {
                if (hostGrid.Children.Count > 1)
                {
                    var textBlock = hostGrid.Children[2];
                    if (textBlock != null)
                    {
                        switch (TitleVisibility)
                        {
                            case SegmentedSliderTitleVisibility.Collapsed:
                                textBlock.Visibility = Visibility.Collapsed;
                                break;
                            case SegmentedSliderTitleVisibility.AlwaysVisible:
                                textBlock.Visibility = Visibility.Visible;
                                break;

                            case SegmentedSliderTitleVisibility.VisibleCurrentSegment:
                                textBlock.Visibility = i == CurrentSegmentIndex()
                                    ? Visibility.Visible
                                    : Visibility.Collapsed;
                                break;

                            case SegmentedSliderTitleVisibility.VisibleCurrentAndPrevious:
                                textBlock.Visibility = i <= CurrentSegmentIndex()
                                    ? Visibility.Visible
                                    : Visibility.Collapsed;
                                break;
                        }
                    }
                }
            }
        }
    }
    private void UpdateTitleHorizontalAlignment()
    {
        var segments = GetSegments();
        if (segments == null)
            return;

        foreach (var item in segments)
        {
            item.TitleTextBlock.HorizontalAlignment = TitleHorizontalAlignment;
        }
    }
    private int CurrentSegmentIndex()
    {
        if (Maximum == 0 || SegmentCount == 0)
            return 0;

        double ratio = Value / Maximum;
        int index = (int)Math.Floor(ratio * SegmentCount);
        return Math.Min(index, SegmentCount - 1);
    }

    public ItemsRepeater GetItemsRepeater()
    {
        return itemsRepeater;
    }

    public List<SegmentedSliderSegment> GetSegments()
    {
        if (itemsRepeater == null || itemsRepeater.ItemsSource is not System.Collections.ICollection collection || collection.Count == 0)
            return null;

        List<SegmentedSliderSegment> segments = new List<SegmentedSliderSegment>();
        int itemCount = collection.Count;

        for (int i = 0; i < itemCount; i++)
        {
            var element = itemsRepeater.GetOrCreateElement(i);
            if (element is Grid hostGrid)
            {
                if (hostGrid.Children.Count > 1)
                {
                    var rect = hostGrid.Children[0] as Rectangle;
                    var fillRect = hostGrid.Children[1] as Rectangle;
                    var label = hostGrid.Children[2] as TextBlock;
                    segments.Add(new SegmentedSliderSegment
                    {
                        TrackRectangle = rect,
                        FillSegmentRectangle = fillRect,
                        TitleTextBlock = label
                    });
                }
            }
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

            // Find both rectangles
            Rectangle? trackRect = null;
            Rectangle? fillRect = null;

            if (hostGrid.Children.Count >= 2)
            {
                trackRect = hostGrid.Children[0] as Rectangle;
                fillRect = hostGrid.Children[1] as Rectangle;
            }

            if (trackRect == null || fillRect == null)
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
                bool isLastMiddle = (i == itemCount - 2);
                ApplyMiddleSegmentClip(trackRect, isLastMiddle);
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
