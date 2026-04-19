using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;

namespace DevWinUI;
public static partial class DragMoveAndResizeHelper
{
    /// <summary>
    /// Sets up drag movement functionality for a specified window and UI element.
    /// </summary>
    /// <param name="window">The window where the drag movement will be applied.</param>
    /// <param name="element">The UI element that will respond to drag movement actions.</param>
    public static void SetDragMove(this Microsoft.UI.Xaml.Window window, UIElement element)
    {
        SetDragMove(window, element, new DragMoveAndResizeInfo(DragMoveAndResizeMode.DragMove));
    }

    /// <summary>
    /// Subscribe <see cref="UIElement.PointerPressed"/>, <see cref="UIElement.PointerMoved"/>, <see cref="UIElement.PointerReleased"/>
    /// </summary>
    /// <param name="window"></param>
    /// <param name="element"></param>
    /// <param name="info"></param>
    public static void SetDragMove(this Microsoft.UI.Xaml.Window window, UIElement element, DragMoveAndResizeInfo info)
    {
        _pointerPressed = (o, e) => RootPointerPressed(o, e, info);
        _pointerMoved = (o, e) => RootPointerMoved(o, e, info, window);
        element.PointerPressed += _pointerPressed;
        element.PointerMoved += _pointerMoved;
        element.PointerReleased += RootPointerReleased;
    }

    /// <summary>
    /// Unsubscribe <see cref="UIElement.PointerPressed"/>, <see cref="UIElement.PointerMoved"/>, <see cref="UIElement.PointerReleased"/>
    /// </summary>
    /// <param name="element"></param>
    public static void UnsetDragMove(UIElement element)
    {
        element.PointerPressed -= _pointerPressed;
        element.PointerMoved -= _pointerMoved;
        element.PointerReleased -= RootPointerReleased;
    }

    private static PointerEventHandler _pointerPressed = null!;
    private static PointerEventHandler _pointerMoved = null!;

    private static System.Drawing.Point _lastPoint;
    private static PointerOperationType _type;
    private static InputSystemCursorShape _lastShape = InputSystemCursorShape.Arrow;

    private static readonly PropertyInfo _property = typeof(UIElement).GetProperty("ProtectedCursor", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.GetProperty | BindingFlags.SetProperty)!;

    [Flags]
    private enum PointerOperationType
    {
        /// <summary>
        /// Used to distinguish whether a button is pressed in the root control
        /// </summary>
        None = 0,

        /// <summary>
        /// Only available when <see cref="DragMoveAndResizeMode.Resize"/> is set
        /// </summary>
        Top = 1 << 0,

        /// <inheritdoc cref="Top"/>
        Bottom = 1 << 1,

        /// <inheritdoc cref="Top"/>
        Left = 1 << 2,

        /// <inheritdoc cref="Top"/>
        Right = 1 << 3,

        /// <inheritdoc cref="Top"/>
        LeftTop = Top | Left,

        /// <inheritdoc cref="Top"/>
        RightTop = Right | Top,

        /// <inheritdoc cref="Top"/>
        LeftBottom = Left | Bottom,

        /// <inheritdoc cref="Top"/>
        RightBottom = Right | Bottom,

        /// <summary>
        /// Only available when <see cref="DragMoveAndResizeMode.DragMove"/> is set
        /// </summary>
        Move = Top | Left | Right | Bottom
    }

    private static void RootPointerPressed(object sender, PointerRoutedEventArgs e, DragMoveAndResizeInfo info)
    {
        var frameworkElement = sender as FrameworkElement;
        var point = e.GetCurrentPoint(frameworkElement);
        var properties = point.Properties;

        if (!properties.IsLeftButtonPressed)
            return;

        var width = frameworkElement.ActualWidth;
        var height = frameworkElement.ActualHeight;
        var position = point.Position;

        if (info.Mode is DragMoveAndResizeMode.DragMove)
            _type = PointerOperationType.Move;
        else
        {
            _type = PointerOperationType.None;
            if (position.X < info.DraggableBorderThickness)
                _type |= PointerOperationType.Left;
            if (position.Y < info.DraggableBorderThickness)
                _type |= PointerOperationType.Top;
            if (width - position.X < info.DraggableBorderThickness)
                _type |= PointerOperationType.Right;
            if (height - position.Y < info.DraggableBorderThickness)
                _type |= PointerOperationType.Bottom;
            if (_type is PointerOperationType.None && info.Mode is not DragMoveAndResizeMode.Resize)
                _type = PointerOperationType.Move;
        }

        _ = frameworkElement.CapturePointer(e.Pointer);
        _ = PInvoke.GetCursorPos(out _lastPoint);
    }

    private static void RootPointerMoved(object sender, PointerRoutedEventArgs e, DragMoveAndResizeInfo info, Microsoft.UI.Xaml.Window window)
    {
        var frameworkElement = sender as FrameworkElement;
        var pointer = e.GetCurrentPoint(frameworkElement);

        #region Cursor shape

        var position = pointer.Position;
        var width = frameworkElement.ActualWidth;
        var height = frameworkElement.ActualHeight;

        var pointerShape = InputSystemCursorShape.Arrow;

        var presenter = (OverlappedPresenter)window.AppWindow.Presenter;

        if (presenter.State is not OverlappedPresenterState.Maximized && info.Mode.HasFlag(DragMoveAndResizeMode.Resize))
        {
            var left = position._x < info.MinOffset;
            var top = position._y < info.MinOffset;
            var right = width - position._x < info.MinOffset;
            var bottom = height - position._y < info.MinOffset;
            pointerShape = 0 switch
            {
                0 when (left && top) || (right && bottom) => InputSystemCursorShape.SizeNorthwestSoutheast,
                0 when (left && bottom) || (right && top) => InputSystemCursorShape.SizeNortheastSouthwest,
                0 when left || right => InputSystemCursorShape.SizeWestEast,
                0 when top || bottom => InputSystemCursorShape.SizeNorthSouth,
                _ => pointerShape
            };
        }

        if (pointerShape != _lastShape)
        {
            (_property.GetValue(sender) as InputCursor)?.Dispose();
            _property.SetValue(sender, InputSystemCursor.Create(pointerShape));
            _lastShape = pointerShape;
        }

        #endregion

        var properties = pointer.Properties;
        if (!properties.IsLeftButtonPressed || _type is PointerOperationType.None)
            return;

        _ = PInvoke.GetCursorPos(out var point);

        var xOffset = point.X - _lastPoint.X;
        var yOffset = point.Y - _lastPoint.Y;
        var offset = Vector2.DistanceSquared(Vector2.Zero, new(xOffset, yOffset));

        if (offset < info.MinOffset)
            return;

        if (presenter.State is OverlappedPresenterState.Maximized)
        {
            if (_type is PointerOperationType.Move)
            {
                var originalSize = window.AppWindow.Size;
                presenter.Restore();
                var size = window.AppWindow.Size;
                var rate = 1 - (double)size.Width / originalSize.Width;
                window.AppWindow.Move(new((int)(point.X * rate), (int)(point.Y * rate)));
            }
        }
        else
        {
            var xPos = window.AppWindow.Position.X;
            var yPos = window.AppWindow.Position.Y;
            var xSize = window.AppWindow.Size.Width;
            var ySize = window.AppWindow.Size.Height;

            if (_type.HasFlag(PointerOperationType.Top))
            {
                yPos += yOffset;
                var newYSize = ySize - yOffset;
                if (info.MinSize.Y < newYSize && newYSize < info.MaxSize.Y)
                    ySize = newYSize;
            }
            if (_type.HasFlag(PointerOperationType.Bottom))
            {
                var newYSize = ySize + yOffset;
                if (info.MinSize.Y < newYSize && newYSize < info.MaxSize.Y)
                    ySize = newYSize;
                else
                    yPos += yOffset;
            }
            if (_type.HasFlag(PointerOperationType.Left))
            {
                xPos += xOffset;
                var newXSize = xSize - xOffset;
                if (info.MinSize.X < newXSize && newXSize < info.MaxSize.X)
                    xSize = newXSize;
            }
            if (_type.HasFlag(PointerOperationType.Right))
            {
                var newXSize = xSize + xOffset;
                if (info.MinSize.X < newXSize && newXSize < info.MaxSize.X)
                    xSize = newXSize;
                else
                    xPos += xOffset;
            }

            window.AppWindow.MoveAndResize(new(xPos, yPos, xSize, ySize));
        }

        _lastPoint.X = point.X;
        _lastPoint.Y = point.Y;
    }

    private static void RootPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        (sender as UIElement).ReleasePointerCaptures();
        _type = PointerOperationType.None;
    }
}

