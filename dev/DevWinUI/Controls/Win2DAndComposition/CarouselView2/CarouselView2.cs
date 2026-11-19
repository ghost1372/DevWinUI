using System.Diagnostics;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Media3D;
using EF = DevWinUI.ExpressionFunctions;

namespace DevWinUI;

public partial class CarouselView2 : ContentControl, ICarouselView2
{
    public event EventHandler? ItemsCreated;
    public event EventHandler? ItemsLoaded;
    public event EventHandler? CarouselMovingStateChanged;
    public event EventHandler? SelectionAnimationComplete;
    public event EventHandler? SelectionChanged;
    private readonly Grid _root;
    private volatile bool _inputAllowed;
    private double _width;
    private double _height;
    private readonly DispatcherTimer _delayedRefreshTimer;
    private readonly DispatcherTimer _delayedZIndexUpdateTimer;
    private CancellationTokenSource _cancelTokenSource = new();
    protected int _maxItemWidth;
    protected int _maxItemHeight;
    private int _previousSelectedIndex;
    private volatile float _scrollValue = 0;
    private volatile float _scrollSnapshot = 0;
    private volatile int _carouselInsertPosition;
    protected Visual _dynamicGridVisual = null!;
    private Grid _dynamicContainerGrid = null!;
    private Grid _itemsLayerGrid = null!;
    private Canvas _hitbox = null!;
    private volatile float _currentWheelTick;
    private volatile float _currentWheelTickOffset;
    private long _currentColumnYPosTick;
    private long _currentRowXPosTick;
    private bool _manipulationStarted;
    private bool _selectedIndexSetInternally;
    private bool _deltaDirectionIsReverse;
    protected bool _uIItemsCreated;
    private volatile int _elementsToLoadCount;
    private Dictionary<Visual, float> _activeVerticalScrollAnimations = [];
    private Dictionary<Visual, float> _activeHorizontalScrollAnimations = [];
    protected bool IsXaxisNavigation
    {
        get
        {
            if (CarouselType == CarouselView2CarouselTypes.Wheel)
            {
                switch (WheelAlignment)
                {
                    case CarouselView2WheelAlignments.Bottom:
                    case CarouselView2WheelAlignments.Top:
                        return true;
                    case CarouselView2WheelAlignments.Left:
                    case CarouselView2WheelAlignments.Right:
                        return false;
                }
            }
            else if (CarouselType == CarouselView2CarouselTypes.Row)
            {
                return true;
            }
            else if (CarouselType == CarouselView2CarouselTypes.Column)
            {
                return false;
            }
            return true;
        }
    }

    private int ItemsToScale
    {
        get
        {
            if (AdditionalItemsToScale > Density / 2)
            {
                return Density / 2;
            }
            return AdditionalItemsToScale;
        }
    }

    private int ItemsToWarp
    {
        get
        {
            if (AdditionalItemsToWarp > Density / 2)
            {
                return Density / 2;
            }
            return AdditionalItemsToWarp;
        }
    }

    protected bool UseFliptych
    {
        get
        {
            return FliptychDegrees > 1 || FliptychDegrees < -1;
        }
    }

    private int DisplaySelectedIndex
    {
        get
        {
            return (_carouselInsertPosition + (Density / 2)) % Density;
        }
    }

    private float Degrees
    {
        get
        {
            return 360.0f / Density;
        }
    }
    private int CurrentStartIndexForwards
    {
        get
        {
            return Items != null ? (CurrentStartIndexBackwards + (Density - 1)) % Items.Count : 0;
        }
    }

    private int CurrentStartIndexBackwards
    {
        get
        {
            return Items != null ? Modulus((SelectedIndex - (Density / 2)), Items.Count) : 0;
        }
    }

    public bool IsCarouselMoving { get; private set; }

    public bool AreItemsLoaded { get; set; }

    public object? SelectedItem { get; private set; }

    public FrameworkElement? SelectedItemElement { get; private set; }

    public IList<object>? Items { get; private set; }

    public int WheelSize
    {
        get
        {
            var maxDimension = (_height > _width) ? _height : _width;
            return Convert.ToInt32(maxDimension);
        }
    }

    public CarouselView2()
    {
        _root = new Grid();
        Content = _root;
        _delayedRefreshTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        _delayedRefreshTimer.Tick += DelayedRefreshTimer_Tick;
        _delayedZIndexUpdateTimer = new DispatcherTimer();
        _delayedZIndexUpdateTimer.Tick += DelayedZIndexUpdateTimer_Tick;
        _root.Background = new SolidColorBrush(Colors.Transparent);

        PointerWheelChanged -= Carousel_PointerWheelChanged;
        PointerWheelChanged += Carousel_PointerWheelChanged;

        KeyDown -= Carousel_KeyDown;
        KeyDown += Carousel_KeyDown;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Refresh();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _delayedRefreshTimer.Stop();
        _delayedZIndexUpdateTimer.Stop();
    }

    private void Carousel_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (AreItemsLoaded && IsKeyboardNavigationEnabled)
        {
            try
            {
                var key = e.Key;
                switch (key)
                {
                    case Windows.System.VirtualKey.Up:
                    case Windows.System.VirtualKey.Down:
                        switch (CarouselType)
                        {
                            case CarouselView2CarouselTypes.Wheel:
                                switch (WheelAlignment)
                                {
                                    case CarouselView2WheelAlignments.Right:
                                    case CarouselView2WheelAlignments.Left:
                                        ChangeSelection(key == Windows.System.VirtualKey.Up);
                                        break;
                                }
                                break;
                            case CarouselView2CarouselTypes.Column:
                                ChangeSelection(key == Windows.System.VirtualKey.Up);
                                break;
                        }
                        break;
                    case Windows.System.VirtualKey.Left:
                    case Windows.System.VirtualKey.Right:
                        switch (CarouselType)
                        {
                            case CarouselView2CarouselTypes.Wheel:
                                switch (WheelAlignment)
                                {
                                    case CarouselView2WheelAlignments.Top:
                                    case CarouselView2WheelAlignments.Bottom:
                                        ChangeSelection(key == Windows.System.VirtualKey.Left);
                                        break;
                                }
                                break;
                            case CarouselView2CarouselTypes.Row:
                                ChangeSelection(key == Windows.System.VirtualKey.Left);
                                break;
                        }
                        break;
                    case Windows.System.VirtualKey.Enter:
                        AnimateSelection();
                        break;
                }
            }
            catch (Exception)
            {
            }
        }
    }

    private void Carousel_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        if (AreItemsLoaded && IsPointerWheelEnabled)
        {
            try
            {
                var point = e.GetCurrentPoint(this);
                switch (point.Properties.MouseWheelDelta)
                {
                    case 120:
                        ChangeSelection(true);
                        break;
                    case -120:
                        ChangeSelection(false);
                        break;
                }
            }
            catch (Exception)
            {
            }
        }
    }

    private void Refresh()
    {
        _inputAllowed = false;
        AreItemsLoaded = false;
        _delayedRefreshTimer.Stop();

        if (IsLoaded)
        {
            _delayedRefreshTimer.Start();
        }
    }

    private void LoadNewCarousel()
    {
        _uIItemsCreated = false;
        _width = double.IsNaN(Width) ? ActualWidth : Width;
        _height = double.IsNaN(Height) ? ActualHeight : Height;
        _cancelTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancelTokenSource.Token;
        CreateContainers();
        _activeVerticalScrollAnimations = [];
        _activeHorizontalScrollAnimations = [];
        _elementsToLoadCount = Density;
        var items = new FrameworkElement[Density];
        if (Items != null && Items.Count > 0)
        {
            try
            {
                _itemsLayerGrid.Children.Clear();

                for (var i = 0; i < Density; i++)
                {
                    _itemsLayerGrid.Children.Add(new ContentControl());
                }

                for (var i = 0; i < Density / 2; i++)
                {
                    var idx1 = (i + (Density / 2)) % Density;
                    var idx2 = Density - 1 - idx1;
                    var fe1 = AddCarouselItemToUI(idx1);
                    items[idx1] = fe1;
                    if (cancellationToken.IsCancellationRequested) { break; }
                    var fe2 = AddCarouselItemToUI(idx2);
                    if (cancellationToken.IsCancellationRequested) { break; }
                    items[idx2] = fe2;
                }

                _uIItemsCreated = true;
                if (cancellationToken.IsCancellationRequested) { return; }

                UpdateZIndices();
                SetHitboxSize();
                InitializeAnimations(items);
                OnItemsCreated();
            }
            catch (Exception ex)
            {
                _cancelTokenSource.Cancel();
                Trace.WriteLine(ex.Message);
            }
        }
        _inputAllowed = true;
    }

    protected virtual void InitializeAnimations(FrameworkElement[] items)
    {
        for (var i = 0; i < Density / 2; i++)
        {
            var idx1 = (i + (Density / 2)) % Density;
            var idx2 = Density - 1 - idx1;

            StartExpressionItemAnimations(items[idx1], idx1);
            StartExpressionItemAnimations(items[idx2], idx2);
        }
    }

    private FrameworkElement AddCarouselItemToUI(int idx)
    {
        var playlistIdx = (idx + CurrentStartIndexBackwards) % Items!.Count;
        var itemElement = CreateCarouselItemElement(idx, playlistIdx);
        if (itemElement != null)
        {
            itemElement.Loaded += ItemElement_Loaded;
            _itemsLayerGrid.Children[Density - 1 - idx] = itemElement;
        }
        else
        {
            throw new Exception("Failed to create Carousel Item");
        }
        return itemElement;
    }

    private void ItemElement_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement element)
        {
            element.Loaded -= ItemElement_Loaded;
            _elementsToLoadCount--;
            if (_elementsToLoadCount < 1)
            {
                OnItemsLoaded();
            }
        }
    }

    private void CreateContainers()
    {
        _root.Children.Clear();
        _currentRowXPosTick = 0;
        _currentWheelTick = 0;
        _carouselInsertPosition = 0;

        _dynamicContainerGrid = new Grid
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Width = _width,
            Height = _height
        };
        _dynamicGridVisual = ElementCompositionPreview.GetElementVisual(_dynamicContainerGrid);
        _dynamicGridVisual.CenterPoint = new Vector3(Convert.ToSingle(_width / 2), Convert.ToSingle(_height / 2), 0);
        CarouselRotationAngle = _dynamicGridVisual.RotationAngleInDegrees;
        _itemsLayerGrid = new Grid { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        _dynamicContainerGrid.Children.Add(_itemsLayerGrid);
        _root.Children.Add(_dynamicContainerGrid);
        if (_hitbox != null)
        {
            _hitbox.ManipulationStarted -= Hitbox_ManipulationStarted;
            _hitbox.ManipulationCompleted -= Hitbox_ManipulationCompleted;
            _hitbox.ManipulationDelta -= Hitbox_ManipulationDelta;
        }
        if (UseGestures)
        {
            _hitbox = new Canvas
            {
                Background = new SolidColorBrush(Colors.Transparent),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                ManipulationMode = ManipulationModes.All
            };
            _hitbox.ManipulationStarted += Hitbox_ManipulationStarted;
            _hitbox.ManipulationCompleted += Hitbox_ManipulationCompleted;
            _hitbox.ManipulationDelta += Hitbox_ManipulationDelta;
            _root.Children.Add(_hitbox);
        }

        ElementCompositionPreview.SetIsTranslationEnabled(_root, true);
    }

    private void Hitbox_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        if (AreItemsLoaded && ItemsSource != null)
        {
            double value = 0;
            switch (CarouselType)
            {
                case CarouselView2CarouselTypes.Wheel:
                    switch (WheelAlignment)
                    {
                        case CarouselView2WheelAlignments.Right:
                            value = -(e.Delta.Translation.Y / 4);
                            break;
                        case CarouselView2WheelAlignments.Left:
                            value = e.Delta.Translation.Y / 4;
                            break;
                        case CarouselView2WheelAlignments.Top:
                            value = -(e.Delta.Translation.X / 4);
                            break;
                        case CarouselView2WheelAlignments.Bottom:
                            value = e.Delta.Translation.X / 4;
                            break;
                    }
                    CarouselRotationAngle += Convert.ToSingle(value);
                    break;
                case CarouselView2CarouselTypes.Column:
                    value = e.Cumulative.Translation.Y * 2;
                    CarouselPositionY = Convert.ToSingle(value);
                    break;
                case CarouselView2CarouselTypes.Row:
                    value = e.Cumulative.Translation.X * 2;
                    CarouselPositionX = Convert.ToSingle(value);
                    break;
            }
        }
    }

    private void Hitbox_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        StopManipulationMode();
    }

    private void Hitbox_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
    {
        StartManipulationMode();
    }

    private void SetHitboxSize()
    {
        if (UseGestures)
        {
            switch (CarouselType)
            {
                default:
                    _hitbox.Width = _width;
                    _hitbox.Height = _height;
                    break;
                case CarouselView2CarouselTypes.Wheel:
                    float ws = 0;
                    switch (WheelAlignment)
                    {
                        case CarouselView2WheelAlignments.Bottom:
                        case CarouselView2WheelAlignments.Top:
                            ws = WheelSize + (_maxItemHeight * Convert.ToSingle(SelectedItemScale));
                            break;
                        case CarouselView2WheelAlignments.Left:
                        case CarouselView2WheelAlignments.Right:
                            ws = WheelSize + (_maxItemWidth * Convert.ToSingle(SelectedItemScale));
                            break;
                    }
                    _hitbox.Width = ws;
                    _hitbox.Height = ws;
                    break;
                case CarouselView2CarouselTypes.Column:
                    _hitbox.Height = _height;
                    _hitbox.Width = _maxItemWidth * SelectedItemScale;
                    if (WarpIntensity != 0 && WarpCurve != 0 && SelectedItemElement != null)
                    {
                        _hitbox.Width += (int)(Math.Abs((-WarpCurve) + Math.Abs(WarpIntensity)));
                    }
                    break;
                case CarouselView2CarouselTypes.Row:
                    _hitbox.Width = _width;
                    _hitbox.Height = _maxItemHeight * SelectedItemScale;
                    if (WarpIntensity != 0 && WarpCurve != 0 && SelectedItemElement != null)
                    {
                        _hitbox.Height += (int)(Math.Abs((-WarpCurve) + Math.Abs(WarpIntensity)));
                    }
                    break;
            }

        }
    }

    private void DelayedRefreshTimer_Tick(object? sender, object e)
    {
        _delayedRefreshTimer.Stop();

        if (!IsLoaded)
            return;

        var width = double.IsNaN(Width) ? ActualWidth : Width;
        var height = double.IsNaN(Height) ? ActualHeight : Height;

        if (width <= 0 || height <= 0)
        {
            _delayedRefreshTimer.Start();
            return;
        }

        LoadNewCarousel();
    }

    private void DelayedZIndexUpdateTimer_Tick(object? sender, object e)
    {
        _delayedZIndexUpdateTimer.Stop();
        UpdateZIndices();
    }

    private void StartWheelSnapToAnimation(Visual visual, float degrees)
    {
        if (NavigationSpeed != 0)
        {
            var duration = (NavigationSpeed < 500) ? NavigationSpeed : 500;
            var animationRotate = visual.Compositor.CreateScalarKeyFrameAnimation();
            animationRotate.InsertKeyFrame(1f, degrees);
            animationRotate.Duration = TimeSpan.FromMilliseconds(duration);
            visual.StartAnimation("RotationAngleInDegrees", animationRotate);
        }
    }

    private void ClearImplicitOffsetAnimations(float xDiff, float yDiff, bool clearAll = false)
    {
        for (var i = (Density - 1); i > -1; i--)
        {
            var idx = Modulus(((Density - 1) - i), Density);
            if (_itemsLayerGrid.Children[idx] is FrameworkElement itemElement)
            {
                var itemElementVisual = ElementCompositionPreview.GetElementVisual(itemElement);
                if (itemElementVisual.ImplicitAnimations != null)
                {
                    if (clearAll)
                    {
                        itemElementVisual.ImplicitAnimations.Clear();
                    }
                    else
                    {
                        itemElementVisual.ImplicitAnimations.Remove("Offset");
                    }
                }
                itemElementVisual.Offset = new Vector3(itemElementVisual.Offset.X + xDiff, itemElementVisual.Offset.Y + yDiff, itemElementVisual.Offset.Z);
            }
        }
    }

    protected void StartExpressionItemAnimations(FrameworkElement element, int? slotNum)
    {
        if (element == null) { return; }
        var visual = ElementCompositionPreview.GetElementVisual(element);
        ScalarNode? distanceAsPercentOfSelectionAreaThreshold = null;
        ScalarNode? scaleThresholdDistanceRaw = null;
        BooleanNode? distanceIsNegativeValue = null;
        BooleanNode? isWithinScaleThreshold = null;
        Vector3Node? offset = null;
        float selectionAreaThreshold;

        if (CarouselType == CarouselView2CarouselTypes.Wheel && slotNum != null)
        {
            selectionAreaThreshold = ItemsToScale == 0 ? Degrees : Degrees * ItemsToScale;

            var slotDegrees = ((slotNum + (Density / 2)) % Density) * Degrees;

            var wheelDegreesWhenItemIsSelected = Convert.ToSingle(slotDegrees);

            using var wheelAngle = _dynamicGridVisual.GetReference().RotationAngleInDegrees;
            switch (WheelAlignment)
            {
                case CarouselView2WheelAlignments.Right:
                    scaleThresholdDistanceRaw = EF.Mod((wheelAngle - wheelDegreesWhenItemIsSelected), 360);
                    break;
                case CarouselView2WheelAlignments.Top:
                    scaleThresholdDistanceRaw = EF.Mod((wheelAngle - wheelDegreesWhenItemIsSelected), 360);
                    break;
                case CarouselView2WheelAlignments.Left:
                    scaleThresholdDistanceRaw = EF.Mod((wheelAngle + wheelDegreesWhenItemIsSelected), 360);
                    break;
                case CarouselView2WheelAlignments.Bottom:
                    scaleThresholdDistanceRaw = EF.Mod((wheelAngle + wheelDegreesWhenItemIsSelected), 360);
                    break;
            }

            using var distanceToZero = EF.Abs(scaleThresholdDistanceRaw!);
            using var distanceTo360 = 360 - distanceToZero;
            using var isClosestToZero = distanceToZero <= distanceTo360;
            using var distanceInDegrees = EF.Conditional(isClosestToZero, distanceToZero, distanceTo360);
            distanceAsPercentOfSelectionAreaThreshold = distanceInDegrees / selectionAreaThreshold;
            switch (WheelAlignment)
            {
                case CarouselView2WheelAlignments.Top:
                case CarouselView2WheelAlignments.Bottom:
                    distanceIsNegativeValue = EF.Abs(scaleThresholdDistanceRaw!) < 180;
                    break;
                case CarouselView2WheelAlignments.Left:
                    distanceIsNegativeValue = EF.Abs(EF.Mod((wheelAngle + wheelDegreesWhenItemIsSelected - 90), 360)) < 180;
                    break;
                case CarouselView2WheelAlignments.Right:
                    distanceIsNegativeValue = EF.Abs(EF.Mod((wheelAngle - wheelDegreesWhenItemIsSelected + 90), 360)) < 180;
                    break;
            }

            isWithinScaleThreshold = distanceInDegrees < selectionAreaThreshold;
            StartScaleAnimation(element, distanceAsPercentOfSelectionAreaThreshold, isWithinScaleThreshold);
        }
        else if (CarouselType == CarouselView2CarouselTypes.Row || CarouselType == CarouselView2CarouselTypes.Column)
        {
            selectionAreaThreshold = IsXaxisNavigation ? AdditionalItemsToScale * (_maxItemWidth + ItemGap) : AdditionalItemsToScale * (_maxItemHeight + ItemGap);
            if (selectionAreaThreshold == 0)
            {
                selectionAreaThreshold = IsXaxisNavigation ? _maxItemWidth + ItemGap : _maxItemHeight + ItemGap;
            }

            offset = visual.GetReference().Offset;
            scaleThresholdDistanceRaw = IsXaxisNavigation ? offset.X / selectionAreaThreshold : offset.Y / selectionAreaThreshold;

            distanceAsPercentOfSelectionAreaThreshold = EF.Abs(scaleThresholdDistanceRaw);

            distanceIsNegativeValue = scaleThresholdDistanceRaw < 0;
            isWithinScaleThreshold = IsXaxisNavigation ? offset.X > -selectionAreaThreshold & offset.X < selectionAreaThreshold : offset.Y > -selectionAreaThreshold & offset.Y < selectionAreaThreshold;

            StartScaleAnimation(element, distanceAsPercentOfSelectionAreaThreshold, isWithinScaleThreshold);

            if (WarpIntensity != 0)
            {
                var warpItemsthreshold = IsXaxisNavigation ? AdditionalItemsToWarp * (_maxItemWidth + ItemGap) : AdditionalItemsToWarp * (_maxItemHeight + ItemGap);
                if (warpItemsthreshold == 0)
                {
                    warpItemsthreshold = IsXaxisNavigation ? _maxItemWidth + ItemGap : _maxItemHeight + ItemGap;
                }
                using var warpThresholdDistanceRaw = IsXaxisNavigation ? offset.X / warpItemsthreshold : offset.Y / warpItemsthreshold;
                using var distanceAsPercentOfWarpThreshold = EF.Abs(warpThresholdDistanceRaw);
                using var isWithinWarpThreshold = IsXaxisNavigation ? offset.X > -warpItemsthreshold & offset.X < warpItemsthreshold : offset.Y > -warpItemsthreshold & offset.Y < warpItemsthreshold;
                using var y = WarpIntensity - (distanceAsPercentOfWarpThreshold * WarpIntensity);
                using var WarpOffset = Convert.ToSingle(-WarpCurve) * warpThresholdDistanceRaw * warpThresholdDistanceRaw + WarpIntensity;
                using var finalWarpValue = EF.Conditional(isWithinWarpThreshold, y * EF.Abs(y) * (float)WarpCurve, 0);
                if (IsXaxisNavigation)
                {
                    visual.StartAnimation("Translation.Y", finalWarpValue);
                }
                else
                {
                    visual.StartAnimation("Translation.X", finalWarpValue);
                }
            }

        }

        if (UseFliptych && CarouselType != CarouselView2CarouselTypes.Wheel)
        {
            UIElement? child = null;

            if (element.Transform3D == null)
            {
                element.Transform3D = new PerspectiveTransform3D { Depth = 1000 };
            }
            if (element is UserControl uc && uc.Content is UIElement ucElement)
            {
                child = ucElement;
            }
            else if (element is ContentControl cc && cc.Content is UIElement ccElement)
            {
                child = ccElement;
            }
            else if (element is Grid g && g.Children.Count == 1 && g.Children.First() is UIElement gElement)
            {
                child = gElement;
            }
            else if (element is Panel p && p.Children.Count == 1 && p.Children.First() is UIElement pElement)
            {
                child = pElement;
            }
            else if (element is Canvas c && c.Children.Count == 1 && c.Children.First() is UIElement cElement)
            {
                child = cElement;
            }

            if (child != null)
            {
                var childVisual = ElementCompositionPreview.GetElementVisual(child);
                var fliptychDegrees = IsXaxisNavigation ? Convert.ToSingle(FliptychDegrees) : Convert.ToSingle(-FliptychDegrees);
                if (CarouselType == CarouselView2CarouselTypes.Wheel) { fliptychDegrees *= -1; }
                childVisual.RotationAxis = IsXaxisNavigation ? new Vector3(0, 1, 0) : new Vector3(1, 0, 0);
                childVisual.CenterPoint = new Vector3(_maxItemWidth / 2, _maxItemHeight / 2, 0);
                using var rotatedValue = EF.Conditional(distanceIsNegativeValue!, fliptychDegrees, -fliptychDegrees);
                using var finalValue = EF.Conditional(isWithinScaleThreshold!, distanceAsPercentOfSelectionAreaThreshold! * rotatedValue, rotatedValue);
                childVisual.StartAnimation(nameof(childVisual.RotationAngleInDegrees), finalValue);
            }
        }
        SetColorAnimation(distanceAsPercentOfSelectionAreaThreshold!, isWithinScaleThreshold!, element);

        offset?.Dispose();
        distanceAsPercentOfSelectionAreaThreshold?.Dispose();
        scaleThresholdDistanceRaw?.Dispose();
        distanceIsNegativeValue?.Dispose();
        isWithinScaleThreshold?.Dispose();
    }

    private void StartScaleAnimation(UIElement element, ScalarNode distanceAsPercentOfScaleThreshold, BooleanNode isWithinScaleThreshold)
    {
        if (SelectedItemScale != 1)
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);
            var scaleRange = (float)SelectedItemScale - 1;
            using var scalePercent = scaleRange * (1 - distanceAsPercentOfScaleThreshold) + 1;
            using var finalScaleValue = ExpressionFunctions.Conditional(isWithinScaleThreshold, scalePercent, 1);
            visual.StartAnimation("Scale.X", finalScaleValue);
            visual.StartAnimation("Scale.Y", finalScaleValue);

        }
    }

    private void SetColorAnimation(ScalarNode distanceAsPercentOfScaleThreshold, BooleanNode isWithinScaleThreshold, FrameworkElement fe)
    {
        var descendants = fe.FindDescendants();
        List<FrameworkElement> elements = [];
        foreach (var descendant in descendants)
        {
            if (descendant is FrameworkElement cfe)
            {
                elements.Add(cfe);
            }
        }
        elements.Add(fe);
        foreach (var element in elements)
        {
            var t = element.GetType();
#pragma warning disable IL2075 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.
            var props = t.GetProperties();
#pragma warning restore IL2075 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.
            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(CompositionColorBrush) || prop.PropertyType == typeof(CompositionLinearGradientBrush))
                {
                    if (SelectedItemForegroundBrush is SolidColorBrush solidColorBrush)
                    {
                        if (prop.GetValue(element) is CompositionColorBrush compositionSolid)
                        {
                            using var deselectedColor = ExpressionFunctions.ColorRgb(compositionSolid.Color.A,
                               compositionSolid.Color.R, compositionSolid.Color.G, compositionSolid.Color.B);
                            using var selectedColor = ExpressionFunctions.ColorRgb(solidColorBrush.Color.A,
                               solidColorBrush.Color.R, solidColorBrush.Color.G, solidColorBrush.Color.B);
                            using var colorLerp = ExpressionFunctions.ColorLerp(selectedColor, deselectedColor, distanceAsPercentOfScaleThreshold);
                            using var finalColorExp = ExpressionFunctions.Conditional(isWithinScaleThreshold, colorLerp, deselectedColor);
                            compositionSolid.StartAnimation("Color", finalColorExp);

                        }
                        else if (prop.GetValue(element) is CompositionLinearGradientBrush compositionGradient)
                        {
                            for (var i = 0; i < compositionGradient.ColorStops.Count; i++)
                            {
                                using var targetStop = compositionGradient.ColorStops[i];
                                using var deselectedColor = ExpressionFunctions.ColorRgb(targetStop.Color.A, targetStop.Color.R, targetStop.Color.G, targetStop.Color.B);
                                using var selectedColor = ExpressionFunctions.ColorRgb(solidColorBrush.Color.A, solidColorBrush.Color.R, solidColorBrush.Color.G, solidColorBrush.Color.B);
                                using var colorLerp = ExpressionFunctions.ColorLerp(selectedColor, deselectedColor, distanceAsPercentOfScaleThreshold);
                                using var finalColorExp = ExpressionFunctions.Conditional(isWithinScaleThreshold, colorLerp, deselectedColor);
                                targetStop.StartAnimation("Color", finalColorExp);
                            }
                        }
                    }
                    else if (SelectedItemForegroundBrush is LinearGradientBrush linearGradientBrush && prop.GetValue(element) is CompositionLinearGradientBrush compositionGradient)
                    {
                        if (linearGradientBrush.GradientStops.Count == compositionGradient.ColorStops.Count)
                        {
                            for (var i = 0; i < compositionGradient.ColorStops.Count; i++)
                            {
                                using var targetStop = compositionGradient.ColorStops[i];
                                using var deselectedColor = ExpressionFunctions.ColorRgb(targetStop.Color.A, targetStop.Color.R, targetStop.Color.G, targetStop.Color.B);
                                var sourceStop = linearGradientBrush.GradientStops[i];
                                using var selectedColor = ExpressionFunctions.ColorRgb(sourceStop.Color.A, sourceStop.Color.R, sourceStop.Color.G, sourceStop.Color.B);
                                using var colorLerp = ExpressionFunctions.ColorLerp(selectedColor, deselectedColor, distanceAsPercentOfScaleThreshold);
                                using var finalColorExp = ExpressionFunctions.Conditional(isWithinScaleThreshold, colorLerp, deselectedColor);
                                targetStop.StartAnimation("Color", finalColorExp);
                            }
                        }
                    }
                }
            }
        }
    }

    private void AddStandardImplicitItemAnimation(Visual visual)
    {
        AddStandardImplicitItemAnimation(visual, NavigationSpeed, false);
    }

    protected virtual void AddStandardImplicitItemAnimation(Visual visual, int durationMilliseconds, bool rotation)
    {
        if (NavigationSpeed != 0)
        {
            if (visual.ImplicitAnimations == null)
            {
                visual.ImplicitAnimations = visual.Compositor.CreateImplicitAnimationCollection();
            }

            var scaleAnimation = visual.Compositor.CreateVector3KeyFrameAnimation();
            var scaleEasing = scaleAnimation.Compositor.CreateLinearEasingFunction();
            scaleAnimation.InsertExpressionKeyFrame(1f, "this.FinalValue", scaleEasing);
            scaleAnimation.Target = nameof(visual.Scale);
            scaleAnimation.Duration = TimeSpan.FromMilliseconds(durationMilliseconds);
            if (!visual.ImplicitAnimations.ContainsKey(nameof(visual.Scale)))
            {
                visual.ImplicitAnimations[nameof(visual.Scale)] = scaleAnimation;
            }

            var offsetAnimation = visual.Compositor.CreateVector3KeyFrameAnimation();
            var offsetEasing = offsetAnimation.Compositor.CreateLinearEasingFunction();
            offsetAnimation.InsertExpressionKeyFrame(1f, "this.FinalValue", offsetEasing);
            offsetAnimation.Target = nameof(visual.Offset);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(durationMilliseconds);
            if (!visual.ImplicitAnimations.ContainsKey(nameof(visual.Offset)))
            {
                visual.ImplicitAnimations[nameof(visual.Offset)] = offsetAnimation;
            }

            var opacityAnimation = visual.Compositor.CreateScalarKeyFrameAnimation();
            var opacityEasing = opacityAnimation.Compositor.CreateLinearEasingFunction();
            opacityAnimation.InsertExpressionKeyFrame(1f, "this.FinalValue", opacityEasing);
            opacityAnimation.Target = nameof(visual.Opacity);
            opacityAnimation.Duration = TimeSpan.FromMilliseconds(durationMilliseconds);
            if (!visual.ImplicitAnimations.ContainsKey(nameof(visual.Opacity)))
            {
                visual.ImplicitAnimations[nameof(visual.Opacity)] = opacityAnimation;
            }

            if (rotation)
            {
                var rotateAnimation = visual.Compositor.CreateScalarKeyFrameAnimation();
                var rotationEasing = rotateAnimation.Compositor.CreateLinearEasingFunction();
                rotateAnimation.InsertExpressionKeyFrame(1f, "this.FinalValue", rotationEasing);
                rotateAnimation.Target = nameof(visual.RotationAngleInDegrees);
                rotateAnimation.Duration = TimeSpan.FromMilliseconds(durationMilliseconds);
                if (!visual.ImplicitAnimations.ContainsKey(nameof(visual.RotationAngleInDegrees)))
                {
                    visual.ImplicitAnimations[nameof(visual.RotationAngleInDegrees)] = rotateAnimation;
                }
            }
        }
    }

    private void AddImplicitWheelRotationAnimation(Visual visual)
    {
        if (NavigationSpeed > 0)
        {
            var implicitAnimations = visual.Compositor.CreateImplicitAnimationCollection();
            var animation = visual.Compositor.CreateScalarKeyFrameAnimation();
            animation.InsertExpressionKeyFrame(1f, "this.FinalValue");
            animation.Target = "RotationAngleInDegrees";
            animation.Duration = TimeSpan.FromMilliseconds(NavigationSpeed);
            implicitAnimations["RotationAngleInDegrees"] = animation;
            visual.ImplicitAnimations = implicitAnimations;
        }
    }

    private static void RemoveImplicitWheelRotationAnimation(Visual visual)
    {
        visual.ImplicitAnimations?.Clear();
    }

    public void StartManipulationMode()
    {
        _manipulationStarted = true;
        IsCarouselMoving = true;
        RemoveImplicitWheelRotationAnimation(_dynamicGridVisual);
        OnCarouselMovingStateChanged();
    }

    public async void StopManipulationMode()
    {
        IsCarouselMoving = false;
        await StopCarouselMoving().ConfigureAwait(false);
        OnCarouselMovingStateChanged();
    }

    private void UpdateCarouselVerticalScrolling(float newValue)
    {
        if (Items != null && _itemsLayerGrid.Children.Count == Density)
        {
            _scrollValue = newValue - _scrollSnapshot;
            _scrollSnapshot = newValue;
            var threshold = _maxItemHeight + ItemGap;

            if (_manipulationStarted)
            {
                _manipulationStarted = false;
                _deltaDirectionIsReverse = _scrollValue < 0;
                if (newValue > 0)
                {
                    Interlocked.Add(ref _currentColumnYPosTick, -threshold / 2);
                }
                else if (newValue < 0)
                {
                    Interlocked.Add(ref _currentColumnYPosTick, threshold / 2);
                }
            }

            else if ((_deltaDirectionIsReverse && _scrollValue > 0) || (!_deltaDirectionIsReverse && _scrollValue < 0))
            {
                _deltaDirectionIsReverse = !_deltaDirectionIsReverse;
                if (_deltaDirectionIsReverse)
                {
                    Interlocked.Add(ref _currentColumnYPosTick, threshold);
                }
                else
                {
                    Interlocked.Add(ref _currentColumnYPosTick, -threshold);
                }
            }

            for (var i = (Density - 1); i > -1; i--)
            {
                var idx = Modulus(((Density - 1) - i), Density);
                if (_itemsLayerGrid != null && _itemsLayerGrid.Children[idx] is FrameworkElement itemElement)
                {
                    var itemElementVisual = ElementCompositionPreview.GetElementVisual(itemElement);
                    itemElementVisual.Offset = new Vector3(itemElementVisual.Offset.X, itemElementVisual.Offset.Y + _scrollValue, itemElementVisual.Offset.Z);
                }
            }

            while (newValue > _currentColumnYPosTick + threshold)
            {
                ChangeSelection(true);
                Interlocked.Add(ref _currentColumnYPosTick, threshold);
            }

            while (newValue < _currentColumnYPosTick - threshold)
            {
                ChangeSelection(false);
                Interlocked.Add(ref _currentColumnYPosTick, -threshold);
            }
        }
    }

    private void UpdateCarouselHorizontalScrolling(float newValue)
    {
        if (Items != null && _itemsLayerGrid.Children.Count == Density)
        {
            _scrollValue = newValue - _scrollSnapshot;
            _scrollSnapshot = newValue;
            var threshold = _maxItemWidth + ItemGap;

            if (_manipulationStarted)
            {
                _manipulationStarted = false;
                _deltaDirectionIsReverse = _scrollValue < 0;
                if (newValue > 0)
                {
                    Interlocked.Add(ref _currentRowXPosTick, -threshold / 2);
                }
                else if (newValue < 0)
                {
                    Interlocked.Add(ref _currentRowXPosTick, threshold / 2);
                }
            }

            else if ((_deltaDirectionIsReverse && _scrollValue > 0) || (!_deltaDirectionIsReverse && _scrollValue < 0))
            {
                _deltaDirectionIsReverse = !_deltaDirectionIsReverse;
                if (_deltaDirectionIsReverse)
                {
                    Interlocked.Add(ref _currentRowXPosTick, threshold);
                }
                else
                {
                    Interlocked.Add(ref _currentRowXPosTick, -threshold);
                }
            }

            for (var i = (Density - 1); i > -1; i--)
            {
                var idx = Modulus(((Density - 1) - i), Density);
                if (_itemsLayerGrid != null && _itemsLayerGrid.Children[idx] is FrameworkElement itemElement)
                {
                    var itemElementVisual = ElementCompositionPreview.GetElementVisual(itemElement);
                    itemElementVisual.Offset = new Vector3(itemElementVisual.Offset.X + _scrollValue, itemElementVisual.Offset.Y, itemElementVisual.Offset.Z);
                }
            }

            while (newValue > _currentRowXPosTick + threshold)
            {
                ChangeSelection(true);
                Interlocked.Add(ref _currentRowXPosTick, threshold);
            }

            while (newValue < _currentRowXPosTick - threshold)
            {
                ChangeSelection(false);
                Interlocked.Add(ref _currentRowXPosTick, -threshold);
            }

        }
    }

    private void UpdateWheelRotation(float newValue)
    {
        if (Items != null && _itemsLayerGrid.Children.Count == Density)
        {
            _scrollValue = newValue - _scrollSnapshot;
            _scrollSnapshot = newValue;
            if (_manipulationStarted)
            {
                _manipulationStarted = false;
                _deltaDirectionIsReverse = _scrollValue < 0;
                if (_deltaDirectionIsReverse)
                {
                    _currentWheelTickOffset = Degrees / 2;
                }
                else
                {
                    _currentWheelTickOffset = -Degrees / 2;
                }
            }
            else if ((_deltaDirectionIsReverse && _scrollValue > 0) || (!_deltaDirectionIsReverse && _scrollValue < 0))
            {
                _deltaDirectionIsReverse = !_deltaDirectionIsReverse;
                if (_deltaDirectionIsReverse)
                {
                    _currentWheelTickOffset += Degrees;
                }
                else
                {
                    _currentWheelTickOffset -= Degrees;
                }
            }

            _dynamicGridVisual.RotationAngleInDegrees = newValue;
            while (newValue > _currentWheelTick + Degrees + _currentWheelTickOffset)
            {
                _currentWheelTick += Degrees;
                switch (WheelAlignment)
                {
                    case CarouselView2WheelAlignments.Right:
                    case CarouselView2WheelAlignments.Top:
                        ChangeSelection(false);
                        break;
                    case CarouselView2WheelAlignments.Left:
                    case CarouselView2WheelAlignments.Bottom:
                        ChangeSelection(true);
                        break;
                }
            }
            while (newValue < _currentWheelTick - Degrees + _currentWheelTickOffset)
            {
                _currentWheelTick -= Degrees;
                switch (WheelAlignment)
                {
                    case CarouselView2WheelAlignments.Right:
                    case CarouselView2WheelAlignments.Top:
                        ChangeSelection(true);
                        break;
                    case CarouselView2WheelAlignments.Left:
                    case CarouselView2WheelAlignments.Bottom:
                        ChangeSelection(false);
                        break;
                }
            }
        }
    }

    private async Task StopCarouselMoving()
    {
        var selectedIdx = Modulus(((Density - 1) - (DisplaySelectedIndex)), Density);
        if (CarouselType == CarouselView2CarouselTypes.Wheel)
        {
            StartWheelSnapToAnimation(_dynamicGridVisual, _currentWheelTick);
            _currentWheelTick %= 360;
            _dynamicGridVisual.RotationAngleInDegrees = _currentWheelTick;
            CarouselRotationAngle = _currentWheelTick;
        }

        var offsetVertical = _maxItemHeight + ItemGap;
        var offsetHorizontal = _maxItemWidth + ItemGap;

        for (var i = -((Density / 2) - 1); i <= (Density / 2); i++)
        {
            var j = Modulus((selectedIdx + i), Density);
            if (_itemsLayerGrid != null && _itemsLayerGrid.Children[j] is FrameworkElement itemElement)
            {
                var itemElementVisual = ElementCompositionPreview.GetElementVisual(itemElement);
                if (CarouselType == CarouselView2CarouselTypes.Column)
                {
                    var currentX = itemElementVisual.Offset.X;
                    itemElementVisual.Offset = new System.Numerics.Vector3(currentX, offsetVertical * -i, (Density - Math.Abs(i)));
                }
                else if (CarouselType == CarouselView2CarouselTypes.Row)
                {
                    var currentY = itemElementVisual.Offset.Y;
                    itemElementVisual.Offset = new System.Numerics.Vector3(offsetHorizontal * -i, currentY, (Density - Math.Abs(i)));
                }
            }
        }
        CarouselPositionY = 0;
        _currentColumnYPosTick = 0;
        CarouselPositionX = 0;
        _currentRowXPosTick = 0;
        _scrollValue = 0;
        _scrollSnapshot = 0;
    }

    private FrameworkElement? CreateCarouselItemElement(int i, int playlistIdx)
    {
        if (ItemTemplate != null)
        {
            var w = 0;
            var h = 0;
            if (ItemTemplate.LoadContent() is not FrameworkElement element)
            {
                return null;
            }
            if (ItemContentStyle != null)
            {
                var descendants = element.FindDescendants();
                var targetTypeChild = descendants.Where(x => x.GetType() == ItemContentStyle.TargetType).FirstOrDefault() as FrameworkElement;
                if (targetTypeChild != null)
                {
                    Convert.ChangeType(targetTypeChild, ItemContentStyle.TargetType);
                    targetTypeChild.Style = ItemContentStyle;
                    targetTypeChild.Tag = true;
                    if (double.IsNaN(targetTypeChild.Width) || double.IsNaN(targetTypeChild.Height))
                    {
                        if (!double.IsInfinity(targetTypeChild.MaxWidth) && !double.IsInfinity(targetTypeChild.MaxHeight))
                        {
                            w = Convert.ToInt32(targetTypeChild.MaxWidth + targetTypeChild.Margin.Left + targetTypeChild.Margin.Right);
                            h = Convert.ToInt32(targetTypeChild.MaxHeight + targetTypeChild.Margin.Top + targetTypeChild.Margin.Bottom);
                        }
                    }
                    else
                    {
                        w = Convert.ToInt32(targetTypeChild.Width + targetTypeChild.Margin.Left + targetTypeChild.Margin.Right);
                        h = Convert.ToInt32(targetTypeChild.Height + targetTypeChild.Margin.Top + targetTypeChild.Margin.Bottom);
                    }
                }
            }
            element.DataContext = Items![playlistIdx];
            if (double.IsNaN(element.Height) || double.IsNaN(element.Width))
            {
                if (!double.IsInfinity(element.MaxWidth) && !double.IsInfinity(element.MaxHeight))
                {
                    w = Convert.ToInt32(element.MaxWidth);
                    h = Convert.ToInt32(element.MaxHeight);
                }
                else if (element is UserControl uc && uc.Content is FrameworkElement childElement)
                {
                    if (double.IsNaN(childElement.Width) || double.IsNaN(childElement.Height))
                    {
                        if (!double.IsInfinity(childElement.MaxWidth) && !double.IsInfinity(childElement.MaxHeight))
                        {
                            w = Convert.ToInt32(childElement.MaxWidth + childElement.Margin.Left + childElement.Margin.Right);
                            h = Convert.ToInt32(childElement.MaxHeight + childElement.Margin.Top + childElement.Margin.Bottom);
                        }
                    }
                    else
                    {
                        w = Convert.ToInt32(childElement.Width + childElement.Margin.Left + childElement.Margin.Right);
                        h = Convert.ToInt32(childElement.Height + childElement.Margin.Top + childElement.Margin.Bottom);
                    }
                }
            }
            else
            {
                w = Convert.ToInt32(element.Width + element.Margin.Left + element.Margin.Right);
                h = Convert.ToInt32(element.Height + element.Margin.Top + element.Margin.Bottom);
            }

            if (_maxItemWidth < element.MaxWidth) { _maxItemWidth = w; }
            if (_maxItemHeight < element.MaxHeight) { _maxItemHeight = h; }
            PositionElement(element, i, (float)w, (float)h);
            return element;
        }
        return null;
    }

    private void Element_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 0 && e.NewSize.Height > 0 && sender is FrameworkElement element)
        {
            element.SizeChanged -= Element_SizeChanged;
            if (_maxItemHeight < element.ActualHeight) { _maxItemHeight = Convert.ToInt32(element.ActualHeight); }
            if (_maxItemWidth < element.ActualWidth) { _maxItemWidth = Convert.ToInt32(element.ActualWidth); }
            if (_uIItemsCreated && Items != null)
            {
                for (var i = (Density - 1); i > -1; i--)
                {
                    var playlistIdx = (i + CurrentStartIndexBackwards) % Items.Count;
                    var context = Items[playlistIdx];
                    FrameworkElement? itemElement = null;
                    foreach (var child in _itemsLayerGrid.Children)
                    {
                        if (child is FrameworkElement childElement && childElement.DataContext == context)
                        {
                            itemElement = childElement;
                            break;
                        }
                    }
                    if (itemElement != null)
                    {
                        PositionElement(itemElement, i, (float)itemElement.ActualWidth, (float)itemElement.ActualHeight);
                    }
                }
                OnItemsLoaded();
            }
        }
    }

    private void PositionElement(FrameworkElement element, int index, float elementWidth, float elementHeight)
    {
        var elementVisual = ElementCompositionPreview.GetElementVisual(element);
        elementVisual.CenterPoint = new Vector3((elementWidth / 2), (elementHeight / 2), 0);
        var offsetX = GetOffsetX(index);
        var offsetY = GetOffsetY(index);
        elementVisual.Offset = new Vector3(Convert.ToSingle(offsetX), Convert.ToSingle(offsetY), 0);
        if (CarouselType == CarouselView2CarouselTypes.Wheel)
        {
            elementVisual.RotationAngleInDegrees = GetRotation(index);
        }
    }

    private List<Visual> UpdateItemInCarouselSlot(int carouselIdx, int sourceIdx, bool loFi)
    {
        var output = new List<Visual>();
        var idx = Modulus(((Density - 1) - carouselIdx), Density);
        if (_itemsLayerGrid != null && _itemsLayerGrid.Children[idx] is FrameworkElement element)
        {
            element.DataContext = Items![sourceIdx];

            if (CarouselType != CarouselView2CarouselTypes.Wheel)
            {
                double translateX = 0;
                double translateY = 0;
                var elementVisual = ElementCompositionPreview.GetElementVisual(element);
                var precedingItemElement = loFi ? _itemsLayerGrid.Children[Modulus(idx - 1, Density)] : _itemsLayerGrid.Children[(idx + 1) % Density];
                var precedingItemElementVisual = ElementCompositionPreview.GetElementVisual(precedingItemElement);
                output.Add(elementVisual);

                switch (CarouselType)
                {
                    case CarouselView2CarouselTypes.Row:
                        if (loFi)
                        {
                            translateX = IsCarouselMoving ? precedingItemElementVisual.Offset.X - (_maxItemWidth + ItemGap)
                                : translateX - (((Density / 2) * (_maxItemWidth + ItemGap)) + _maxItemWidth + ItemGap);

                        }
                        else
                        {
                            translateX = IsCarouselMoving ? precedingItemElementVisual.Offset.X + _maxItemWidth + ItemGap :
                                (Density / 2) * (_maxItemWidth + ItemGap);
                        }
                        break;
                    case CarouselView2CarouselTypes.Column:
                        if (loFi)
                        {
                            translateY = IsCarouselMoving ? precedingItemElementVisual.Offset.Y - (_maxItemHeight + ItemGap) :
                                translateY - (((Density / 2) * (_maxItemHeight + ItemGap)) + _maxItemHeight + ItemGap);
                        }
                        else
                        {
                            translateY = IsCarouselMoving ? precedingItemElementVisual.Offset.Y + _maxItemHeight + ItemGap :
                                (Density / 2) * (_maxItemHeight + ItemGap);
                        }
                        break;
                }
                elementVisual.Offset = new System.Numerics.Vector3((float)translateX, (float)translateY, 0);

                if (!IsCarouselMoving)
                {
                    var precedingItemZIndex = Canvas.GetZIndex(precedingItemElement);
                    Canvas.SetZIndex(element, precedingItemZIndex - 1);
                }

            }
        }
        return output;
    }

    public void ChangeSelection(bool reverse)
    {
        if (_inputAllowed)
        {
            _selectedIndexSetInternally = true;
            SelectedIndex = reverse ? Modulus(SelectedIndex - 1, Items!.Count) : (SelectedIndex + 1) % Items!.Count;
            ChangeSelection(reverse ? CurrentStartIndexBackwards : CurrentStartIndexForwards, reverse);
            if (NavigationSpeed > 1 && ZIndexUpdateWaitsForAnimation)
            {
                _delayedZIndexUpdateTimer.Interval = TimeSpan.FromMilliseconds(NavigationSpeed / 2);
                if (_delayedZIndexUpdateTimer.IsEnabled)
                {
                    UpdateZIndices();
                }
                _delayedZIndexUpdateTimer.Start();
            }
            else
            {
                UpdateZIndices();
            }
            _selectedIndexSetInternally = false;
        }
    }

    private void ChangeSelection(int startIdx, bool reverse)
    {
        _carouselInsertPosition = reverse ? Modulus((_carouselInsertPosition - 1), Density) : (_carouselInsertPosition + 1) % Density;
        var carouselIdx = reverse ? _carouselInsertPosition : Modulus((_carouselInsertPosition - 1), Density);
        InsertNewCarouselItem(startIdx, carouselIdx, !reverse, reverse);
    }

    private void InsertNewCarouselItem(int startIdx, int carouselIdx, bool scrollbackwards, bool loFi)
    {
        if (!IsCarouselMoving)
        {
            switch (CarouselType)
            {
                default:
                    switch (WheelAlignment)
                    {
                        default:
                            RotateWheel(!loFi);
                            break;
                        case CarouselView2WheelAlignments.Left:
                        case CarouselView2WheelAlignments.Bottom:
                            RotateWheel(loFi);
                            break;
                    }
                    UpdateItemInCarouselSlot(carouselIdx, startIdx, loFi);
                    break;
                case CarouselView2CarouselTypes.Column:
                    var shiftedVItems = UpdateItemInCarouselSlot(carouselIdx, startIdx, loFi);
                    ScrollVerticalColumn(scrollbackwards, shiftedVItems);
                    break;
                case CarouselView2CarouselTypes.Row:
                    var shiftedHItems = UpdateItemInCarouselSlot(carouselIdx, startIdx, loFi);
                    ScrollHorizontalRow(scrollbackwards, shiftedHItems);
                    break;
            }
        }
        else
        {
            UpdateItemInCarouselSlot(carouselIdx, startIdx, loFi);
        }
    }

    public void AnimateToSelectedIndex()
    {
        var count = Items != null && Items.Count >= 0 ? Items.Count : 0;
        if (count > 0)
        {
            var distance = ModularDistance(_previousSelectedIndex, SelectedIndex, count);
            var goForward = false;

            if (Carousel2Ext.Mod(_previousSelectedIndex + distance, count) == SelectedIndex)
            {
                goForward = true;
            }

            var steps = distance > Density ? Density : distance;

            if (goForward)
            {
                var startIdx = Modulus((SelectedIndex + 1 - steps - (Density / 2)), count);
                for (var i = 0; i < steps; i++)
                {
                    ChangeSelection((startIdx + i + (Density - 1)) % count, false);
                }
            }
            else
            {
                var startIdx = Modulus(SelectedIndex - 1 + steps - (Density / 2), count);
                for (var i = 0; i < steps; i++)
                {
                    ChangeSelection(Carousel2Ext.Mod(startIdx - i, count), true);
                }
            }

            UpdateZIndices();
        }

    }

    public void AnimateSelection()
    {
        if (SelectedItemElement is FrameworkElement selectedItemContent)
        {
            Storyboard? sb = null;
            if (selectedItemContent.Resources.ContainsKey("SelectionAnimation"))
            {
                sb = selectedItemContent.Resources["SelectionAnimation"] as Storyboard;
            }
            else if (selectedItemContent.Parent is FrameworkElement parent && parent.Resources.ContainsKey("SelectionAnimation"))
            {
                sb = parent.Resources["SelectionAnimation"] as Storyboard;
            }
            if (sb != null)
            {
                sb.Completed += SelectionAnimation_Completed;
                sb.Begin();
            }

        }
    }

    private void SelectionAnimation_Completed(object? sender, object e)
    {
        if (sender is Storyboard sb)
        {
            sb.Completed -= SelectionAnimation_Completed;
        }
        OnSelectionAnimationComplete();
    }

    private void RotateWheel(bool clockwise)
    {
        var endAngle = (clockwise) ? Degrees : -Degrees;
        var newVal = (float)CarouselRotationAngle + endAngle;
        var rotationAnimation = _dynamicGridVisual.Compositor.CreateScalarKeyFrameAnimation();
        var navSpeed = NavigationSpeed > 0 ? NavigationSpeed : 1;
        rotationAnimation.Duration = TimeSpan.FromMilliseconds(navSpeed);
        rotationAnimation.InsertKeyFrame(1f, newVal);
        rotationAnimation.StopBehavior = AnimationStopBehavior.LeaveCurrentValue;
        CarouselRotationAngle = newVal;
        _currentWheelTick += endAngle;
        _dynamicGridVisual.StartAnimation("RotationAngleInDegrees", rotationAnimation);
    }

    private void ScrollVerticalColumn(bool scrollUp, List<Visual> dontAnimate)
    {
        long scrollAmount = (scrollUp) ? -(_maxItemHeight + ItemGap) : (_maxItemHeight + ItemGap);
        for (var i = (Density - 1); i > -1; i--)
        {
            var idx = Modulus(((Density - 1) - i), Density);
            if (_itemsLayerGrid != null && _itemsLayerGrid.Children[idx] is FrameworkElement itemElement)
            {
                var itemElementVisual = ElementCompositionPreview.GetElementVisual(itemElement);
                float finalValue;
                if (dontAnimate.Contains(itemElementVisual))
                {
                    finalValue = itemElementVisual.Offset.Y + scrollAmount;
                }
                else
                {
                    finalValue = _activeVerticalScrollAnimations.TryGetValue(itemElementVisual, out var activeValue)
                        ? activeValue + scrollAmount : itemElementVisual.Offset.Y + scrollAmount;
                }
                var scrollAnimation = itemElementVisual.Compositor.CreateScalarKeyFrameAnimation();
                scrollAnimation.StopBehavior = AnimationStopBehavior.SetToFinalValue;
                var duration = NavigationSpeed > 0 ? NavigationSpeed : 1;
                scrollAnimation.Duration = TimeSpan.FromMilliseconds(duration);
                scrollAnimation.InsertKeyFrame(1f, finalValue);
                itemElementVisual.StartAnimation("Offset.Y", scrollAnimation);
                _activeVerticalScrollAnimations[itemElementVisual] = finalValue;
            }
        }
    }

    private void ScrollHorizontalRow(bool scrollLeft, List<Visual> dontAnimate)
    {
        long scrollAmount = (scrollLeft) ? -(_maxItemWidth + ItemGap) : (_maxItemWidth + ItemGap);
        for (var i = (Density - 1); i > -1; i--)
        {
            var idx = Modulus(((Density - 1) - i), Density);
            if (_itemsLayerGrid != null && _itemsLayerGrid.Children[idx] is FrameworkElement itemElement)
            {
                var itemElementVisual = ElementCompositionPreview.GetElementVisual(itemElement);
                float finalValue;
                if (dontAnimate.Contains(itemElementVisual))
                {
                    finalValue = itemElementVisual.Offset.X + scrollAmount;
                }
                else
                {
                    finalValue = _activeHorizontalScrollAnimations.TryGetValue(itemElementVisual, out var activeValue)
                        ? activeValue + scrollAmount : itemElementVisual.Offset.X + scrollAmount;
                }
                var scrollAnimation = itemElementVisual.Compositor.CreateScalarKeyFrameAnimation();
                scrollAnimation.StopBehavior = AnimationStopBehavior.SetToFinalValue;
                var duration = NavigationSpeed > 0 ? NavigationSpeed : 1;
                scrollAnimation.Duration = TimeSpan.FromMilliseconds(duration);
                scrollAnimation.InsertKeyFrame(1f, finalValue);
                itemElementVisual.StartAnimation("Offset.X", scrollAnimation);
                _activeHorizontalScrollAnimations[itemElementVisual] = finalValue;
            }
        }
    }

    private void ScrollHorizontalRowOLD(bool scrollLeft)
    {
        long endPosition = (scrollLeft) ? -(_maxItemWidth + ItemGap) : (_maxItemWidth + ItemGap);
        for (var i = (Density - 1); i > -1; i--)
        {
            var idx = Modulus(((Density - 1) - i), Density);
            if (_itemsLayerGrid != null && _itemsLayerGrid.Children[idx] is FrameworkElement itemElement)
            {
                var itemElementVisual = ElementCompositionPreview.GetElementVisual(itemElement);
                var currentX = itemElementVisual.Offset.X;
                var currentY = itemElementVisual.Offset.Y;
                itemElementVisual.Offset = new System.Numerics.Vector3(currentX + endPosition, currentY, 0);
            }
        }
    }
    private void UpdateZIndices()
    {
        if (Items != null && _itemsLayerGrid != null)
        {
            for (var i = -(Density / 2); i < (Density / 2); i++)
            {
                var slot = Modulus(((Density - 1) - (DisplaySelectedIndex + i)), Density);
                Canvas.SetZIndex((UIElement)_itemsLayerGrid.Children[slot], 10000 - Math.Abs(i));
                if (i == 0)
                {
                    SelectedItemElement = _itemsLayerGrid.Children[slot] as FrameworkElement;
                }
            }
        }
    }

    protected double GetOffsetY(int i)
    {
        return CarouselType switch
        {
            CarouselView2CarouselTypes.Column => (_maxItemHeight + ItemGap) * (i - (Density / 2)),
            CarouselView2CarouselTypes.Row => 0,
            _ => WheelAlignment switch
            {
                CarouselView2WheelAlignments.Left => Math.Sin(DegreesToRadians(360 - (Degrees * i))) * (WheelSize / 2),
                CarouselView2WheelAlignments.Top => Math.Sin(DegreesToRadians(360 - (Degrees * ((i + (Density / 4)) % Density)))) * (WheelSize / 2),
                CarouselView2WheelAlignments.Bottom => -Math.Sin(DegreesToRadians(360 - (Degrees * ((i + (Density / 4)) % Density)))) * (WheelSize / 2),
                _ => -Math.Sin(DegreesToRadians(Degrees * i)) * (WheelSize / 2)
            }
        };
    }
    protected double GetOffsetX(int i)
    {
        return CarouselType switch
        {
            CarouselView2CarouselTypes.Row => (_maxItemWidth + ItemGap) * (i - (Density / 2)),
            CarouselView2CarouselTypes.Column => 0,
            _ => WheelAlignment switch
            {
                CarouselView2WheelAlignments.Left => -Math.Cos(DegreesToRadians(360 - (Degrees * i))) * (WheelSize / 2),
                CarouselView2WheelAlignments.Top => Math.Cos(DegreesToRadians(360 - (Degrees * ((i + (Density / 4)) % Density)))) * (WheelSize / 2),
                CarouselView2WheelAlignments.Bottom => Math.Cos(DegreesToRadians(360 - (Degrees * ((i + (Density / 4)) % Density)))) * (WheelSize / 2),
                _ => Math.Cos(DegreesToRadians(Degrees * i)) * (WheelSize / 2)
            }
        };
    }

    private int GetRotation(int i)
    {
        if (CarouselType == CarouselView2CarouselTypes.Wheel)
        {
            return WheelAlignment switch
            {
                CarouselView2WheelAlignments.Left or CarouselView2WheelAlignments.Bottom => Convert.ToInt32(((Degrees * i) + 180) % 360),
                _ => Convert.ToInt32(((360 - (Degrees * i)) + 180) % 360)
            };
        }
        else
        {
            return 0;
        }
    }

    private void OnItemsCreated()
    {
        AreItemsLoaded = true;
        ItemsCreated?.Invoke(this, EventArgs.Empty);
    }


    private void OnItemsLoaded()
    {
        AreItemsLoaded = true;
        ItemsLoaded?.Invoke(this, EventArgs.Empty);
    }

    private void OnCarouselMovingStateChanged()
    {
        CarouselMovingStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnSelectionAnimationComplete()
    {
        SelectionAnimationComplete?.Invoke(this, EventArgs.Empty);
    }

    private void OnSelectionChanged()
    {
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    private static void OnSelectedIndexChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == SelectedIndexProperty)
        {
            if (dependencyObject is not CarouselView2 control)
            {
                return;
            }
            if (e.OldValue is int oldVal)
            {
                control._previousSelectedIndex = oldVal;
            }
            if (e.NewValue is int newVal)
            {
                if (!control._selectedIndexSetInternally)
                {
                    control.AnimateToSelectedIndex();
                }
                if (control.Items != null && newVal >= 0 && newVal < control.Items.Count)
                {
                    control.SelectedItem = control.Items[newVal];
                }
                control.OnSelectionChanged();
            }
        }
    }

    private static void OnCaptionPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is CarouselView2 control)
        {
            control.Refresh();
        }
    }

    internal static int Modulus(int num, int mod)
    {
        var result = num % mod;
        if (result < 0)
        {
            result += mod;
        }
        return result;
    }

    internal static int ModularDistance(int a, int b, int m)
    {
        return System.Math.Min(Carousel2Ext.Mod(a - b, m), Carousel2Ext.Mod(b - a, m));
    }

    internal static double DegreesToRadians(double degrees)
    {
        return (degrees * (Math.PI / 180));
    }
}

internal static partial class Carousel2Ext
{
    internal static int Mod(this int x, int m)
    {
        if (m < 0) m = -m;
        var r = x % m;
        return r < 0 ? r + m : r;
    }
}
