using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

public partial class LoopPanel : Panel
{
    internal int PivotalChildIndex { get; private set; }

    private double _inertiaVelocity = 0;
    private const double Deceleration = 0.95; // tweak for faster/slower stopping
    private bool _isInertiaActive = false;
    private double _dragVelocity = 0;
    private DateTime _lastDragTime;
    private const double MinInertiaThreshold = 1.0; // below this, snap
    private bool _isDragging = false;
    private Point _lastDragPos;
    public LoopPanel()
    {
        PivotalChildIndex = -1;

        this.PointerWheelChanged += OnPointerWheelChanged;

        // Drag scrolling
        this.PointerPressed += OnPointerPressed;
        this.PointerMoved += OnPointerMoved;
        this.PointerReleased += OnPointerReleased;
        this.PointerCanceled += OnPointerCanceled;
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        _isDragging = true;
        _lastDragTime = DateTime.Now;

        _lastDragPos = e.GetCurrentPoint(this).Position;
        CapturePointer(e.Pointer);
        e.Handled = true;
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!_isDragging) return;

        Point current = e.GetCurrentPoint(this).Position;
        double delta = Orientation == Orientation.Horizontal
            ? _lastDragPos.X - current.X
            : _lastDragPos.Y - current.Y;

        Scroll(delta * DragScrollFactor);
        InvalidateArrange();

        // compute approximate velocity
        DateTime now = DateTime.Now;
        double milliseconds = (now - _lastDragTime).TotalMilliseconds;
        if (milliseconds > 0)
        {
            double v = delta / milliseconds * 16.6667;
            _dragVelocity = _dragVelocity * 0.35 + v * 0.65;
        }

        _lastDragPos = current;
        _lastDragTime = now;
        e.Handled = true;
    }

    private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (!_isDragging) return;

        _isDragging = false;
        ReleasePointerCapture(e.Pointer);
        e.Handled = true;

        if (IsInertiaEnabled && Math.Abs(_dragVelocity) > MinInertiaThreshold)
        {
            _inertiaVelocity = _dragVelocity;
            StartInertia();
        }
    }

    private void OnPointerCanceled(object sender, PointerRoutedEventArgs e)
    {
        if (!_isDragging) return;

        _isDragging = false;
        ReleasePointerCapture(e.Pointer);
        e.Handled = true;
    }

    private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        var delta = e.GetCurrentPoint(this).Properties.MouseWheelDelta;
        double scrollAmount = -delta * MouseWheelScrollFactor;


        if (IsInertiaEnabled)
        {
            _inertiaVelocity = scrollAmount;
            StartInertia();
        }
        else
        {
            Scroll(scrollAmount);
            InvalidateArrange();

        }

        e.Handled = true;
    }
    private void OnInertiaRendering(object sender, object e)
    {
        if (Math.Abs(_inertiaVelocity) < MinInertiaThreshold)
        {
            // freeze offset so no more scrolling occurs
            _inertiaVelocity = 0;

            // force a dedicated async snap (after layout settles)
            DispatcherQueue.TryEnqueue(() =>
            {
                SnapToNearestChild();
                StopInertia();

            });

            return;
        }

        Scroll(_inertiaVelocity);
        InvalidateArrange();

        _inertiaVelocity *= Deceleration;
    }

    private void SnapToNearestChild()
    {
        if (Children.Count == 0) return;

        int nearest = (int)Math.Round(Offset);
        Offset = nearest;
        InvalidateArrange();
    }

    private void StartInertia()
    {
        if (_isInertiaActive) return;

        _isInertiaActive = true;
        CompositionTarget.Rendering += OnInertiaRendering;
    }

    private void StopInertia()
    {
        if (!_isInertiaActive) return;

        _isInertiaActive = false;
        CompositionTarget.Rendering -= OnInertiaRendering;
    }



    private static bool IsRelativeOffsetValid(object value)
    {
        double v = (double)value;
        return (!double.IsNaN(v)
            && !double.IsPositiveInfinity(v)
            && !double.IsNegativeInfinity(v)
            && v >= 0.0d && v <= 1.0d);
    }


    public void Scroll(double viewportUnits)
    {
        int childCount = Children.Count;
        if (childCount == 0) return;

        // determine the new Offset value required to move the specified viewport units
        double newOffset = Offset;
        int childIndex = PivotalChildIndex;
        bool isHorizontal = (Orientation == Orientation.Horizontal);
        bool isForwardMovement = (viewportUnits > 0);
        int directionalFactor = isForwardMovement ? 1 : -1;
        double remainingExtent = Math.Abs(viewportUnits);
        UIElement child = Children[childIndex];
        double childExtent = isHorizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
        double fractionalOffset = (Offset > 0) ? Offset - Math.Truncate(Offset) : 1.0d - Math.Truncate(Offset) + Offset;
        double childRemainingExtent = isForwardMovement ? childExtent - fractionalOffset * childExtent : fractionalOffset * childExtent;
        if (LoopPanelHelper.LessThanOrVirtuallyEqual(childRemainingExtent, remainingExtent))
        {
            newOffset = Math.Round(isForwardMovement ? newOffset + 1 - fractionalOffset : newOffset - fractionalOffset);
            remainingExtent -= childRemainingExtent;
            while (LoopPanelHelper.StrictlyGreaterThan(remainingExtent, 0.0d))
            {
                childIndex = isForwardMovement ? (childIndex + 1) % childCount : (childIndex == 0 ? childCount - 1 : childIndex - 1);
                child = Children[childIndex];
                childExtent = isHorizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
                if (LoopPanelHelper.LessThanOrVirtuallyEqual(childExtent, remainingExtent))
                {
                    newOffset += 1.0d * directionalFactor;
                    remainingExtent -= childExtent;
                }
                else
                {
                    newOffset += remainingExtent * directionalFactor / childExtent;
                    remainingExtent = 0.0d;
                }
            }
        }
        else
        {
            newOffset += remainingExtent * directionalFactor / childExtent;
            remainingExtent = 0.0d;
        }
        Offset = newOffset;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        UIElementCollection children = Children;
        bool isHorizontal = (Orientation == Orientation.Horizontal);
        Rect childRect = new Rect();
        double childExtent = 0.0;
        int childCount = children.Count;
        Rect controlBounds = new Rect(new Point(0, 0), finalSize);
        double nextEdge = 0, priorEdge = 0;
        int nextIndex = 0, priorIndex = 0;
        PivotalChildIndex = -1;

        if (childCount > 0)
        {
            double adjustedOffset = Offset % childCount;
            if (adjustedOffset < 0)
                adjustedOffset = (adjustedOffset + childCount) % childCount;

            PivotalChildIndex = (int)adjustedOffset;

            nextIndex = (PivotalChildIndex + 1) % childCount;
            priorIndex = (PivotalChildIndex == 0) ? childCount - 1 : PivotalChildIndex - 1;

            UIElement child = children[PivotalChildIndex];
            if (isHorizontal)
            {
                childExtent = child.DesiredSize.Width;
                childRect.X = finalSize.Width * RelativeOffset - childExtent * (adjustedOffset - Math.Truncate(adjustedOffset));
                childRect.Width = childExtent;
                childRect.Height = Math.Max(finalSize.Height, child.DesiredSize.Height);
                nextEdge = childRect.X + childExtent + Spacing;
                priorEdge = childRect.X - Spacing;
            }
            else
            {
                childExtent = child.DesiredSize.Height;
                childRect.Y = finalSize.Height * RelativeOffset - childExtent * (adjustedOffset - Math.Truncate(adjustedOffset));
                childRect.Height = childExtent;
                childRect.Width = Math.Max(finalSize.Width, child.DesiredSize.Width);
                nextEdge = childRect.Y + childExtent + Spacing;
                priorEdge = childRect.Y - Spacing;
            }
            child.Arrange(childRect);
        }

        bool isNextFull = false, isPriorFull = false;

        for (int i = 1; i < childCount; i++)
        {
            bool isArrangingNext = (i % 2 == 1);

            if (isArrangingNext && isNextFull && !isPriorFull) isArrangingNext = false;
            else if (!isArrangingNext && isPriorFull && !isNextFull) isArrangingNext = true;

            int childIndex = isArrangingNext ? nextIndex : priorIndex;
            if (isArrangingNext) nextIndex = (nextIndex + 1) % childCount;
            else priorIndex = (priorIndex > 0) ? priorIndex - 1 : childCount - 1;

            UIElement child = children[childIndex];
            bool childArranged = false;

            if (!(isNextFull && isPriorFull))
            {
                if (isHorizontal)
                {
                    childExtent = child.DesiredSize.Width;
                    childRect.X = isArrangingNext ? nextEdge : priorEdge - childExtent;
                    childRect.Width = childExtent;
                    childRect.Height = Math.Max(finalSize.Height, child.DesiredSize.Height);
                }
                else
                {
                    childExtent = child.DesiredSize.Height;
                    childRect.Y = isArrangingNext ? nextEdge : priorEdge - childExtent;
                    childRect.Height = childExtent;
                    childRect.Width = Math.Max(finalSize.Width, child.DesiredSize.Width);
                }

                Rect intersection = new Rect(new Point(0, 0), finalSize);
                intersection.Intersect(childRect);

                if (!intersection.IsEmpty && intersection.Width * intersection.Height > 1.0e-10)
                {
                    if (isHorizontal)
                    {
                        if (isArrangingNext) nextEdge = childRect.X + childExtent + Spacing;
                        else priorEdge = childRect.X - Spacing;
                    }
                    else
                    {
                        if (isArrangingNext) nextEdge = childRect.Y + childExtent + Spacing;
                        else priorEdge = childRect.Y - Spacing;
                    }

                    child.Arrange(childRect);
                    childArranged = true;
                }
                else
                {
                    if (isArrangingNext) isNextFull = true;
                    else isPriorFull = true;
                }
            }

            if (!childArranged)
            {
                if (isArrangingNext && isNextFull)
                {
                    if (isHorizontal)
                    {
                        childRect.X = nextEdge;
                        nextEdge = childRect.X + child.DesiredSize.Width + Spacing;
                    }
                    else
                    {
                        childRect.Y = nextEdge;
                        nextEdge = childRect.Y + child.DesiredSize.Height + Spacing;
                    }
                    childRect.Width = 0;
                    childRect.Height = 0;
                    child.Arrange(childRect);
                }
                else if (!isArrangingNext && isPriorFull)
                {
                    if (isHorizontal)
                    {
                        childRect.X = priorEdge - child.DesiredSize.Width;
                        priorEdge = childRect.X - Spacing;
                    }
                    else
                    {
                        childRect.Y = priorEdge - child.DesiredSize.Height;
                        priorEdge = childRect.Y - Spacing;
                    }
                    childRect.Width = 0;
                    childRect.Height = 0;
                    child.Arrange(childRect);
                }
            }
        }

        return finalSize;
    }
    private int _lastChildrenCount = -1;

    protected override Size MeasureOverride(Size availableSize)
    {
        if (_lastChildrenCount != Children.Count)
        {
            int oldCount = _lastChildrenCount;
            int newCount = Children.Count;
            _lastChildrenCount = newCount;

            if (_isMeasured && !_inMeasure)
            {
                HandleChildrenChanged(oldCount, newCount);
            }
        }

        Size desiredSize = new Size();
        _inMeasure = true;
        try
        {
            UIElementCollection children = Children;
            bool isHorizontal = (Orientation == Orientation.Horizontal);
            Size childSize = availableSize;

            if (isHorizontal)
                childSize.Width = double.PositiveInfinity;
            else
                childSize.Height = double.PositiveInfinity;

            for (int i = 0, count = children.Count; i < count; ++i)
            {
                UIElement child = children[i];
                child.Measure(childSize);

                Size childDesiredSize = child.DesiredSize;
                if (isHorizontal)
                {
                    desiredSize.Width += childDesiredSize.Width;
                    if (i > 0) desiredSize.Width += Spacing; // add spacing between items
                    desiredSize.Height = Math.Max(desiredSize.Height, childDesiredSize.Height);
                }
                else
                {
                    desiredSize.Height += childDesiredSize.Height;
                    if (i > 0) desiredSize.Height += Spacing;
                    desiredSize.Width = Math.Max(desiredSize.Width, childDesiredSize.Width);
                }
            }

            if (isHorizontal)
                desiredSize.Width = Math.Min(availableSize.Width, desiredSize.Width);
            else
                desiredSize.Height = Math.Min(availableSize.Height, desiredSize.Height);

            _isMeasured = true;
        }
        finally
        {
            _inMeasure = false;
        }
        return desiredSize;
    }

    protected override void OnBringIntoViewRequested(BringIntoViewRequestedEventArgs e)
    {
        base.OnBringIntoViewRequested(e);

        if (!this.BringChildrenIntoView) return;

        // The event has a TargetElement property in WinUI
        var target = e.TargetElement as DependencyObject;
        if (target == null || target == this) return;

        UIElement child = null;
        DependencyObject current = target;
        while (current != null)
        {
            if ((current is UIElement ui)
                && this.Children.Contains(ui))
            {
                child = ui;
                break;
            }
            current = VisualTreeHelper.GetParent(current);
            if (current == this) break;
        }

        if (child != null && this.Children.Contains(child))
        {
            e.Handled = true;

            // determine if the child needs to be brought into view
            GeneralTransform childTransform = child.TransformToVisual(this);
            Rect childRect;
            try
            {
                // TransformBounds exists on GeneralTransform in many Windows/XAML implementations.
                childRect = childTransform.TransformBounds(new Rect(new Point(0, 0), child.RenderSize));
            }
            catch
            {
                // If TransformBounds is not available in the exact runtime, fallback to transforming corners
                Point topLeft = childTransform.TransformPoint(new Point(0, 0));
                Point bottomRight = childTransform.TransformPoint(new Point(child.RenderSize.Width, child.RenderSize.Height));
                childRect = new Rect(topLeft, bottomRight);
            }

            Rect controlRect = new Rect(new Point(0, 0), this.RenderSize);
            Rect intersection = new Rect(new Point(0, 0), this.RenderSize);
            intersection.Intersect(childRect);

            // if the intersection is different than the child rect, it is either not visible 
            // or only partially visible, so adjust the Offset to bring it into view
            if (!LoopPanelHelper.AreVirtuallyEqual(childRect, intersection))
            {
                if (!intersection.IsEmpty)
                {
                    // the child is already partially visible, so just scroll it into view
                    lp_ScrollFromBringIntoView(this, childRect, intersection);
                }
                else
                {
                    // the child is not visible at all
                    lp_ScrollFromBringIntoView(this, childRect, intersection, fullyOutside: true);
                }
            }
        }
    }

    private static void lp_ScrollFromBringIntoView(LoopPanel lp, Rect childRect, Rect intersection, bool fullyOutside = false)
    {
        if (!fullyOutside)
        {
            lp.Scroll((lp.Orientation == Orientation.Horizontal)
                ? (LoopPanelHelper.AreVirtuallyEqual(childRect.X, intersection.X)
                    ? childRect.Width - intersection.Width + Math.Min(0, lp.RenderSize.Width - childRect.Width)
                    : childRect.X - intersection.X)
                : (LoopPanelHelper.AreVirtuallyEqual(childRect.Y, intersection.Y)
                    ? childRect.Height - intersection.Height + Math.Min(0, lp.RenderSize.Height - childRect.Height)
                    : childRect.Y - intersection.Y));
        }
        else
        {
            lp.Scroll((lp.Orientation == Orientation.Horizontal)
                ? (LoopPanelHelper.StrictlyLessThan(childRect.Right, 0.0d)
                    ? childRect.X
                    : childRect.Right - lp.RenderSize.Width + Math.Min(0, lp.RenderSize.Width - childRect.Width))
                : (LoopPanelHelper.StrictlyLessThan(childRect.Bottom, 0.0d)
                    ? childRect.Y
                    : childRect.Bottom - lp.RenderSize.Height + Math.Min(0, lp.RenderSize.Height - childRect.Height)));
        }
    }

    private void HandleChildrenChanged(int oldCount, int newCount)
    {
        if (oldCount == 0) return; // nothing to compare yet

        // do not process during layout
        if (_inMeasure || !_isMeasured) return;

        int childCount = Children.Count;

        if (childCount == 0)
            return;

        if (newCount > oldCount)
        {
            // Simulate CollectionChange.ItemInserted
            double adjustedOffset = Offset % childCount;
            if (adjustedOffset < 0)
            {
                adjustedOffset += childCount;
            }

            int newPivotalChildIndex = (int)adjustedOffset;

            if (newPivotalChildIndex != PivotalChildIndex)
            {
                if (childCount > 1)  // <-- safety check
                {
                    Offset += ((int)Offset) / (childCount - 1) +
                              (Offset < 0 ? -1 : 0);
                }
            }
        }
        else if (newCount < oldCount)
        {
            // Simulate CollectionChange.ItemRemoved
            Offset -= ((int)Offset) / childCount +
                      (Offset < 0 ? -1 : 0);
        }
    }

    private bool _isMeasured = false;
    private bool _inMeasure = false;
}
