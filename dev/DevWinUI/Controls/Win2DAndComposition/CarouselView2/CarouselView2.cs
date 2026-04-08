using System.Diagnostics;
using Microsoft.UI.Xaml.Media.Media3D;
using EF = DevWinUI.ExpressionFunctions;
using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

public partial class CarouselView2 : ContentControl, ICarouselView2
{
    public event EventHandler ItemsCreated;
    public event EventHandler ItemsLoaded;
    public event EventHandler CarouselMovingStateChanged;
    public event EventHandler SelectionAnimationComplete;
    public event EventHandler SelectionChanged;
    Grid _root;
    volatile bool _inputAllowed;
    double _width;
    double _height;
    DispatcherTimer _delayedRefreshTimer;
    DispatcherTimer _delayedZIndexUpdateTimer = new DispatcherTimer();
    CancellationTokenSource _cancelTokenSource;
    protected int _maxItemWidth;
    protected int _maxItemHeight;
    int _previousSelectedIndex;
    volatile float _scrollValue = 0;
    volatile float _scrollSnapshot = 0;
    volatile int _carouselInsertPosition;
    protected Visual _dynamicGridVisual;
    Grid _dynamicContainerGrid;
    Grid _itemsLayerGrid;
    Canvas _hitbox;
    volatile float _currentWheelTick;
    volatile float _currentWheelTickOffset;
    long _currentColumnYPosTick;
    long _currentRowXPosTick;
    bool _manipulationStarted;
    bool _selectedIndexSetInternally;
    bool _deltaDirectionIsReverse;
    protected bool _uIItemsCreated;
    volatile int _elementsToLoadCount;
    Dictionary<Visual, float> _activeVerticalScrollAnimations;
    Dictionary<Visual, float> _activeHorizontalScrollAnimations;
    protected bool isXaxisNavigation
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

    int itemsToScale
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

    int itemsToWarp
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

    protected bool useFliptych
    {
        get
        {
            return this.FliptychDegrees > 1 || this.FliptychDegrees < -1;
        }
    }

    int displaySelectedIndex
    {
        get
        {
            return (_carouselInsertPosition + (Density / 2)) % Density;
        }
    }

    float degrees
    {
        get
        {
            return 360.0f / Density;
        }
    }
    int currentStartIndexForwards
    {
        get
        {
            return Items != null ? (currentStartIndexBackwards + (Density - 1)) % Items.Count() : 0;
        }
    }

    int currentStartIndexBackwards
    {
        get
        {
            return Items != null ? Modulus((SelectedIndex - (Density / 2)), Items.Count()) : 0;
        }
    }

    public bool IsCarouselMoving { get; private set; }

    public bool AreItemsLoaded { get; set; }

    public object SelectedItem { get; private set; }

    public FrameworkElement SelectedItemElement { get; private set; }

    public IList<object> Items { get; private set; }

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
        this.Content = _root;
        _delayedRefreshTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        _delayedRefreshTimer.Tick += _delayedRefreshTimer_Tick;
        _delayedZIndexUpdateTimer = new DispatcherTimer();
        _delayedZIndexUpdateTimer.Tick += _delayedZIndexUpdateTimer_Tick;
        _root.Background = new SolidColorBrush(Colors.Transparent);

        PointerWheelChanged -= Carousel_PointerWheelChanged;
        PointerWheelChanged += Carousel_PointerWheelChanged;

        KeyDown -= Carousel_KeyDown;
        KeyDown += Carousel_KeyDown;
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

    void Refresh()
    {
        _inputAllowed = false;
        AreItemsLoaded = false;
        _delayedRefreshTimer.Start();
    }

    void LoadNewCarousel()
    {
        _uIItemsCreated = false;
        _width = Double.IsNaN(Width) ? ActualWidth : Width;
        _height = Double.IsNaN(Height) ? ActualHeight : Height;
        _cancelTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancelTokenSource.Token;
        CreateContainers();
        _activeVerticalScrollAnimations = new Dictionary<Visual, float>();
        _activeHorizontalScrollAnimations = new Dictionary<Visual, float>();
        _elementsToLoadCount = Density;
        var items = new FrameworkElement[Density];
        if (Items != null && Items.Count() > 0)
        {
            try
            {
                _itemsLayerGrid.Children.Clear();

                for (int i = 0; i < Density; i++)
                {
                    _itemsLayerGrid.Children.Add(new ContentControl());
                }

                for (int i = 0; i < Density / 2; i++)
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
        for (int i = 0; i < Density / 2; i++)
        {
            var idx1 = (i + (Density / 2)) % Density;
            var idx2 = Density - 1 - idx1;

            StartExpressionItemAnimations(items[idx1], idx1);
            StartExpressionItemAnimations(items[idx2], idx2);
        }
    }

    FrameworkElement AddCarouselItemToUI(int idx)
    {
        int playlistIdx = (idx + this.currentStartIndexBackwards) % Items.Count();
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

    void CreateContainers()
    {
        _root.Children.Clear();
        _currentRowXPosTick = 0;
        _currentWheelTick = 0;
        _carouselInsertPosition = 0;

        _dynamicContainerGrid = new Grid { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        _dynamicContainerGrid.Width = _width;
        _dynamicContainerGrid.Height = _height;
        _dynamicGridVisual = ElementCompositionPreview.GetElementVisual(_dynamicContainerGrid);
        _dynamicGridVisual.CenterPoint = new Vector3(Convert.ToSingle(_width / 2), Convert.ToSingle(_height / 2), 0);
        CarouselRotationAngle = _dynamicGridVisual.RotationAngleInDegrees;
        _itemsLayerGrid = new Grid { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        _dynamicContainerGrid.Children.Add(_itemsLayerGrid);
        _root.Children.Add(_dynamicContainerGrid);
        if (_hitbox != null)
        {
            _hitbox.ManipulationStarted -= _hitbox_ManipulationStarted;
            _hitbox.ManipulationCompleted -= _hitbox_ManipulationCompleted;
            _hitbox.ManipulationDelta -= _hitbox_ManipulationDelta;
        }
        if (UseGestures)
        {
            _hitbox = new Canvas();
            _hitbox.Background = new SolidColorBrush(Colors.Transparent);
            _hitbox.HorizontalAlignment = HorizontalAlignment.Center;
            _hitbox.VerticalAlignment = VerticalAlignment.Center;
            _hitbox.ManipulationMode = ManipulationModes.All;
            _hitbox.ManipulationStarted += _hitbox_ManipulationStarted;
            _hitbox.ManipulationCompleted += _hitbox_ManipulationCompleted;
            _hitbox.ManipulationDelta += _hitbox_ManipulationDelta;
            _root.Children.Add(_hitbox);
        }

        ElementCompositionPreview.SetIsTranslationEnabled(_root, true);
    }

    private void _hitbox_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
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

    private void _hitbox_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        StopManipulationMode();
    }

    private void _hitbox_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
    {
        StartManipulationMode();
    }

    void SetHitboxSize()
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
                    if (WarpIntensity != 0 && WarpCurve != 0 && this.SelectedItemElement != null)
                    {
                        _hitbox.Width += (int)(Math.Abs((-WarpCurve) + Math.Abs(WarpIntensity)));
                    }
                    break;
                case CarouselView2CarouselTypes.Row:
                    _hitbox.Width = _width;
                    _hitbox.Height = _maxItemHeight * SelectedItemScale;
                    if (WarpIntensity != 0 && WarpCurve != 0 && this.SelectedItemElement != null)
                    {
                        _hitbox.Height += (int)(Math.Abs((-WarpCurve) + Math.Abs(WarpIntensity)));
                    }
                    break;
            }

        }
    }

    private void _delayedRefreshTimer_Tick(object sender, object e)
    {
        _delayedRefreshTimer.Stop();
        LoadNewCarousel();
    }

    private void _delayedZIndexUpdateTimer_Tick(object sender, object e)
    {
        _delayedZIndexUpdateTimer.Stop();
        UpdateZIndices();
    }

    private void StartWheelSnapToAnimation(Visual visual, float degrees)
    {
        if (NavigationSpeed != 0)
        {
            int duration = (NavigationSpeed < 500) ? NavigationSpeed : 500;
            var animationRotate = visual.Compositor.CreateScalarKeyFrameAnimation();
            animationRotate.InsertKeyFrame(1f, degrees);
            animationRotate.Duration = TimeSpan.FromMilliseconds(duration);
            visual.StartAnimation("RotationAngleInDegrees", animationRotate);
        }
    }

    private void ClearImplicitOffsetAnimations(float xDiff, float yDiff, bool clearAll = false)
    {
        for (int i = (Density - 1); i > -1; i--)
        {
            int idx = Modulus(((Density - 1) - i), Density);
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
        var compositor = visual.Compositor;
        ScalarNode distanceAsPercentOfSelectionAreaThreshold = null;
        ScalarNode scaleThresholdDistanceRaw = null;
        BooleanNode distanceIsNegativeValue = null;
        BooleanNode isWithinScaleThreshold = null;
        Vector3Node offset = null;
        float selectionAreaThreshold = 0;

        if (CarouselType == CarouselView2CarouselTypes.Wheel && slotNum != null)
        {
            selectionAreaThreshold = itemsToScale == 0 ? degrees : degrees * itemsToScale;

            var slotDegrees = ((slotNum + (Density / 2)) % Density) * degrees;

            float wheelDegreesWhenItemIsSelected = Convert.ToSingle(slotDegrees);

            using (ScalarNode wheelAngle = _dynamicGridVisual.GetReference().RotationAngleInDegrees)
            {
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

                using (ScalarNode distanceToZero = EF.Abs(scaleThresholdDistanceRaw))
                using (ScalarNode distanceTo360 = 360 - distanceToZero)
                using (BooleanNode isClosestToZero = distanceToZero <= distanceTo360)
                using (ScalarNode distanceInDegrees = EF.Conditional(isClosestToZero, distanceToZero, distanceTo360))
                {
                    distanceAsPercentOfSelectionAreaThreshold = distanceInDegrees / selectionAreaThreshold;
                    switch (WheelAlignment)
                    {
                        case CarouselView2WheelAlignments.Top:
                        case CarouselView2WheelAlignments.Bottom:
                            distanceIsNegativeValue = EF.Abs(scaleThresholdDistanceRaw) < 180;
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
            }
        }
        else if (CarouselType == CarouselView2CarouselTypes.Row || CarouselType == CarouselView2CarouselTypes.Column)
        {
            selectionAreaThreshold = isXaxisNavigation ? AdditionalItemsToScale * (_maxItemWidth + ItemGap) : AdditionalItemsToScale * (_maxItemHeight + ItemGap);
            if (selectionAreaThreshold == 0)
            {
                selectionAreaThreshold = isXaxisNavigation ? _maxItemWidth + ItemGap : _maxItemHeight + ItemGap;
            }

            offset = visual.GetReference().Offset;
            scaleThresholdDistanceRaw = this.isXaxisNavigation ? offset.X / selectionAreaThreshold : offset.Y / selectionAreaThreshold;

            distanceAsPercentOfSelectionAreaThreshold = EF.Abs(scaleThresholdDistanceRaw);

            distanceIsNegativeValue = scaleThresholdDistanceRaw < 0;
            isWithinScaleThreshold = isXaxisNavigation ? offset.X > -selectionAreaThreshold & offset.X < selectionAreaThreshold : offset.Y > -selectionAreaThreshold & offset.Y < selectionAreaThreshold;

            StartScaleAnimation(element, distanceAsPercentOfSelectionAreaThreshold, isWithinScaleThreshold);

            if (WarpIntensity != 0)
            {
                var warpItemsthreshold = isXaxisNavigation ? AdditionalItemsToWarp * (_maxItemWidth + ItemGap) : AdditionalItemsToWarp * (_maxItemHeight + ItemGap);
                if (warpItemsthreshold == 0)
                {
                    warpItemsthreshold = isXaxisNavigation ? _maxItemWidth + ItemGap : _maxItemHeight + ItemGap;
                }
                using (ScalarNode warpThresholdDistanceRaw = this.isXaxisNavigation ? offset.X / warpItemsthreshold : offset.Y / warpItemsthreshold)
                using (ScalarNode distanceAsPercentOfWarpThreshold = EF.Abs(warpThresholdDistanceRaw))
                using (BooleanNode isWithinWarpThreshold = isXaxisNavigation ? offset.X > -warpItemsthreshold & offset.X < warpItemsthreshold : offset.Y > -warpItemsthreshold & offset.Y < warpItemsthreshold)
                using (ScalarNode y = WarpIntensity - (distanceAsPercentOfWarpThreshold * WarpIntensity))
                using (ScalarNode WarpOffset = Convert.ToSingle(-WarpCurve) * warpThresholdDistanceRaw * warpThresholdDistanceRaw + WarpIntensity)
                using (ScalarNode finalWarpValue = EF.Conditional(isWithinWarpThreshold, y * EF.Abs(y) * (float)WarpCurve, 0))
                {
                    if (isXaxisNavigation)
                    {
                        visual.StartAnimation("Translation.Y", finalWarpValue);
                    }
                    else
                    {
                        visual.StartAnimation("Translation.X", finalWarpValue);
                    }
                }
            }

        }

        if (useFliptych && CarouselType != CarouselView2CarouselTypes.Wheel) 
        {
            UIElement child = null;

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
                var fliptychDegrees = isXaxisNavigation ? Convert.ToSingle(FliptychDegrees) : Convert.ToSingle(-FliptychDegrees);
                if (CarouselType == CarouselView2CarouselTypes.Wheel) { fliptychDegrees *= -1; }
                childVisual.RotationAxis = isXaxisNavigation ? new Vector3(0, 1, 0) : new Vector3(1, 0, 0);
                childVisual.CenterPoint = new Vector3(_maxItemWidth / 2, _maxItemHeight / 2, 0);
                using (ScalarNode rotatedValue = EF.Conditional(distanceIsNegativeValue, fliptychDegrees, -fliptychDegrees))
                using (ScalarNode finalValue = EF.Conditional(isWithinScaleThreshold, distanceAsPercentOfSelectionAreaThreshold * rotatedValue, rotatedValue))
                {
                    childVisual.StartAnimation(nameof(childVisual.RotationAngleInDegrees), finalValue);
                }
            }
        }
        SetColorAnimation(distanceAsPercentOfSelectionAreaThreshold, isWithinScaleThreshold, element);

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
            using (ScalarNode scalePercent = scaleRange * (1 - distanceAsPercentOfScaleThreshold) + 1)
            using (ScalarNode finalScaleValue = ExpressionFunctions.Conditional(isWithinScaleThreshold, scalePercent, 1))
            {
                visual.StartAnimation("Scale.X", finalScaleValue);
                visual.StartAnimation("Scale.Y", finalScaleValue);
            }

        }
    }

    private void SetColorAnimation(ScalarNode distanceAsPercentOfScaleThreshold, BooleanNode isWithinScaleThreshold, FrameworkElement fe)
    {
        var descendants = fe.FindDescendants();
        List<FrameworkElement> elements = new List<FrameworkElement>();
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
            var props = t.GetProperties();
            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(CompositionColorBrush) || prop.PropertyType == typeof(CompositionLinearGradientBrush))
                {
                    if (SelectedItemForegroundBrush is SolidColorBrush solidColorBrush)
                    {
                        if (prop.GetValue(element) is CompositionColorBrush compositionSolid)
                        {
                            using (ColorNode deselectedColor = ExpressionFunctions.ColorRgb(compositionSolid.Color.A,
                               compositionSolid.Color.R, compositionSolid.Color.G, compositionSolid.Color.B))
                            using (ColorNode selectedColor = ExpressionFunctions.ColorRgb(solidColorBrush.Color.A,
                               solidColorBrush.Color.R, solidColorBrush.Color.G, solidColorBrush.Color.B))
                            using (ColorNode colorLerp = ExpressionFunctions.ColorLerp(selectedColor, deselectedColor, distanceAsPercentOfScaleThreshold))
                            using (ColorNode finalColorExp = ExpressionFunctions.Conditional(isWithinScaleThreshold, colorLerp, deselectedColor))
                            {
                                compositionSolid.StartAnimation("Color", finalColorExp);
                            }

                        }
                        else if (prop.GetValue(element) is CompositionLinearGradientBrush compositionGradient)
                        {
                            for (int i = 0; i < compositionGradient.ColorStops.Count; i++)
                            {
                                using (CompositionColorGradientStop targetStop = compositionGradient.ColorStops[i])
                                using (ColorNode deselectedColor = ExpressionFunctions.ColorRgb(targetStop.Color.A, targetStop.Color.R, targetStop.Color.G, targetStop.Color.B))
                                using (ColorNode selectedColor = ExpressionFunctions.ColorRgb(solidColorBrush.Color.A, solidColorBrush.Color.R, solidColorBrush.Color.G, solidColorBrush.Color.B))
                                using (ColorNode colorLerp = ExpressionFunctions.ColorLerp(selectedColor, deselectedColor, distanceAsPercentOfScaleThreshold))
                                using (ColorNode finalColorExp = ExpressionFunctions.Conditional(isWithinScaleThreshold, colorLerp, deselectedColor))
                                {
                                    targetStop.StartAnimation("Color", finalColorExp);
                                }
                            }
                        }
                    }
                    else if (SelectedItemForegroundBrush is LinearGradientBrush linearGradientBrush && prop.GetValue(element) is CompositionLinearGradientBrush compositionGradient)
                    {
                        if (linearGradientBrush.GradientStops.Count == compositionGradient.ColorStops.Count)
                        {
                            for (int i = 0; i < compositionGradient.ColorStops.Count; i++)
                            {
                                using (CompositionColorGradientStop targetStop = compositionGradient.ColorStops[i])
                                using (ColorNode deselectedColor = ExpressionFunctions.ColorRgb(targetStop.Color.A, targetStop.Color.R, targetStop.Color.G, targetStop.Color.B))
                                {
                                    GradientStop sourceStop = linearGradientBrush.GradientStops[i];
                                    using (ColorNode selectedColor = ExpressionFunctions.ColorRgb(sourceStop.Color.A, sourceStop.Color.R, sourceStop.Color.G, sourceStop.Color.B))
                                    using (ColorNode colorLerp = ExpressionFunctions.ColorLerp(selectedColor, deselectedColor, distanceAsPercentOfScaleThreshold))
                                    using (ColorNode finalColorExp = ExpressionFunctions.Conditional(isWithinScaleThreshold, colorLerp, deselectedColor))
                                    {
                                        targetStop.StartAnimation("Color", finalColorExp);
                                    }
                                }
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
            ImplicitAnimationCollection implicitAnimations = visual.Compositor.CreateImplicitAnimationCollection();
            var animation = visual.Compositor.CreateScalarKeyFrameAnimation();
            animation.InsertExpressionKeyFrame(1f, "this.FinalValue");
            animation.Target = "RotationAngleInDegrees";
            animation.Duration = TimeSpan.FromMilliseconds(NavigationSpeed);
            implicitAnimations["RotationAngleInDegrees"] = animation;
            visual.ImplicitAnimations = implicitAnimations;
        }
    }

    private void RemoveImplicitWheelRotationAnimation(Visual visual)
    {
        if (visual.ImplicitAnimations != null)
        {
            visual.ImplicitAnimations.Clear();
        }
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

    void UpdateCarouselVerticalScrolling(float newValue)
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

            for (int i = (Density - 1); i > -1; i--)
            {
                int idx = Modulus(((Density - 1) - i), Density);
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

    void UpdateCarouselHorizontalScrolling(float newValue)
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

            for (int i = (Density - 1); i > -1; i--)
            {
                int idx = Modulus(((Density - 1) - i), Density);
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

    void UpdateWheelRotation(float newValue)
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
                    _currentWheelTickOffset = degrees / 2;
                }
                else
                {
                    _currentWheelTickOffset = -degrees / 2;
                }
            }
            else if ((_deltaDirectionIsReverse && _scrollValue > 0) || (!_deltaDirectionIsReverse && _scrollValue < 0))
            {
                _deltaDirectionIsReverse = !_deltaDirectionIsReverse;
                if (_deltaDirectionIsReverse)
                {
                    _currentWheelTickOffset += degrees;
                }
                else
                {
                    _currentWheelTickOffset -= degrees;
                }
            }

            _dynamicGridVisual.RotationAngleInDegrees = newValue;
            while (newValue > _currentWheelTick + degrees + _currentWheelTickOffset)
            {
                _currentWheelTick += degrees;
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
            while (newValue < _currentWheelTick - degrees + _currentWheelTickOffset)
            {
                _currentWheelTick -= degrees;
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

    async Task StopCarouselMoving()
    {
        var selectedIdx = Modulus(((Density - 1) - (displaySelectedIndex)), Density);
        if (CarouselType == CarouselView2CarouselTypes.Wheel)
        {
            StartWheelSnapToAnimation(_dynamicGridVisual, _currentWheelTick);
            _currentWheelTick = _currentWheelTick % 360;
            _dynamicGridVisual.RotationAngleInDegrees = _currentWheelTick;
            CarouselRotationAngle = _currentWheelTick;
        }

        var offsetVertical = _maxItemHeight + ItemGap;
        var offsetHorizontal = _maxItemWidth + ItemGap;

        for (int i = -((Density / 2) - 1); i <= (Density / 2); i++)
        {
            int j = Modulus((selectedIdx + i), Density);
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

    FrameworkElement CreateCarouselItemElement(int i, int playlistIdx)
    {
        if (ItemTemplate != null)
        {
            int w = 0;
            int h = 0;
            FrameworkElement element = ItemTemplate.LoadContent() as FrameworkElement;
            if (ItemContentStyle != null)
            {
                var descendants = element.FindDescendants();
                var targetTypeChild = descendants.Where(x => x.GetType() == ItemContentStyle.TargetType).FirstOrDefault() as FrameworkElement;
                if (targetTypeChild != null)
                {
                    Convert.ChangeType(targetTypeChild, ItemContentStyle.TargetType);
                    targetTypeChild.Style = ItemContentStyle;
                    targetTypeChild.Tag = true;
                    if (Double.IsNaN(targetTypeChild.Width) || Double.IsNaN(targetTypeChild.Height))
                    {
                        if (!Double.IsInfinity(targetTypeChild.MaxWidth) && !Double.IsInfinity(targetTypeChild.MaxHeight))
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
            element.DataContext = Items[playlistIdx];
            if (Double.IsNaN(element.Height) || Double.IsNaN(element.Width))
            {
                if (!Double.IsInfinity(element.MaxWidth) && !Double.IsInfinity(element.MaxHeight))
                {
                    w = Convert.ToInt32(element.MaxWidth);
                    h = Convert.ToInt32(element.MaxHeight);
                }
                else if (element is UserControl uc && uc.Content is FrameworkElement childElement)
                {
                    if (Double.IsNaN(childElement.Width) || Double.IsNaN(childElement.Height))
                    {
                        if (!Double.IsInfinity(childElement.MaxWidth) && !Double.IsInfinity(childElement.MaxHeight))
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
            if (_uIItemsCreated)
            {
                for (int i = (Density - 1); i > -1; i--)
                {
                    int playlistIdx = (i + this.currentStartIndexBackwards) % Items.Count();
                    var context = Items[playlistIdx];
                    FrameworkElement itemElement = null;
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

    void PositionElement(FrameworkElement element, int index, float elementWidth, float elementHeight)
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

    List<Visual> UpdateItemInCarouselSlot(int carouselIdx, int sourceIdx, bool loFi)
    {
        var output = new List<Visual>();
        int idx = Modulus(((Density - 1) - carouselIdx), Density);
        if (_itemsLayerGrid != null && _itemsLayerGrid.Children[idx] is FrameworkElement element)
        {
            element.DataContext = Items[sourceIdx];

            if (CarouselType != CarouselView2CarouselTypes.Wheel)
            {
                double translateX = 0;
                double translateY = 0;
                var elementVisual = ElementCompositionPreview.GetElementVisual(element);
                UIElement precedingItemElement = loFi ? _itemsLayerGrid.Children[Modulus(idx - 1, Density)] : _itemsLayerGrid.Children[(idx + 1) % Density];
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
            SelectedIndex = reverse ? Modulus(SelectedIndex - 1, Items.Count()) : (SelectedIndex + 1) % Items.Count();
            ChangeSelection(reverse ? currentStartIndexBackwards : currentStartIndexForwards, reverse);
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

    void ChangeSelection(int startIdx, bool reverse)
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
        var count = Items != null && Items.Count >= 0 ? Items.Count() : 0;
        if (count > 0)
        {
            var distance = ModularDistance(_previousSelectedIndex, SelectedIndex, count);
            bool goForward = false;

            if (Carousel2Ext.Mod(_previousSelectedIndex + distance, count) == SelectedIndex)
            {
                goForward = true;
            }

            var steps = distance > Density ? Density : distance;

            if (goForward)
            {
                var startIdx = Modulus((SelectedIndex + 1 - steps - (Density / 2)), count);
                for (int i = 0; i < steps; i++)
                {
                    ChangeSelection((startIdx + i + (Density - 1)) % Items.Count(), false);
                }
            }
            else
            {
                var startIdx = Modulus(SelectedIndex - 1 + steps - (Density / 2), count);
                for (int i = 0; i < steps; i++)
                {
                    ChangeSelection(Carousel2Ext.Mod(startIdx - i, count), true);
                }
            }

            UpdateZIndices();
        }

    }

    public void AnimateSelection()
    {
        if (this.SelectedItemElement is FrameworkElement selectedItemContent)
        {
            Storyboard sb = null;
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

    private void SelectionAnimation_Completed(object sender, object e)
    {
        if (sender is Storyboard sb)
        {
            sb.Completed -= SelectionAnimation_Completed;
        }
        OnSelectionAnimationComplete();
    }

    private void RotateWheel(bool clockwise)
    {
        float endAngle = (clockwise) ? degrees : -degrees;
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

    void ScrollVerticalColumn(bool scrollUp, List<Visual> dontAnimate)
    {
        long scrollAmount = (scrollUp) ? -(_maxItemHeight + ItemGap) : (_maxItemHeight + ItemGap);
        for (int i = (Density - 1); i > -1; i--)
        {
            int idx = Modulus(((Density - 1) - i), Density);
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
                    finalValue = _activeVerticalScrollAnimations.ContainsKey(itemElementVisual)
                        ? _activeVerticalScrollAnimations[itemElementVisual] + scrollAmount : itemElementVisual.Offset.Y + scrollAmount;
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

    void ScrollHorizontalRow(bool scrollLeft, List<Visual> dontAnimate)
    {
        long scrollAmount = (scrollLeft) ? -(_maxItemWidth + ItemGap) : (_maxItemWidth + ItemGap);
        for (int i = (Density - 1); i > -1; i--)
        {
            int idx = Modulus(((Density - 1) - i), Density);
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
                    finalValue = _activeHorizontalScrollAnimations.ContainsKey(itemElementVisual)
                        ? _activeHorizontalScrollAnimations[itemElementVisual] + scrollAmount : itemElementVisual.Offset.X + scrollAmount;
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

    void ScrollHorizontalRowOLD(bool scrollLeft)
    {
        long endPosition = (scrollLeft) ? -(_maxItemWidth + ItemGap) : (_maxItemWidth + ItemGap);
        for (int i = (Density - 1); i > -1; i--)
        {
            int idx = Modulus(((Density - 1) - i), Density);
            if (_itemsLayerGrid != null && _itemsLayerGrid.Children[idx] is FrameworkElement itemElement)
            {
                var itemElementVisual = ElementCompositionPreview.GetElementVisual(itemElement);
                var currentX = itemElementVisual.Offset.X;
                var currentY = itemElementVisual.Offset.Y;
                itemElementVisual.Offset = new System.Numerics.Vector3(currentX + endPosition, currentY, 0);
            }
        }
    }
    void UpdateZIndices()
    {
        if (Items != null && _itemsLayerGrid != null)
        {
            for (int i = -(Density / 2); i < (Density / 2); i++)
            {
                var slot = Modulus(((Density - 1) - (displaySelectedIndex + i)), Density);
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
        switch (CarouselType)
        {
            default:
                switch (WheelAlignment)
                {
                    default:
                        return -(Math.Sin(DegreesToRadians(degrees * i))) * (WheelSize / 2);
                    case CarouselView2WheelAlignments.Left:
                        return (Math.Sin(DegreesToRadians(360 - (degrees * i)))) * (WheelSize / 2);
                    case CarouselView2WheelAlignments.Top:
                        return (Math.Sin(DegreesToRadians(360 - (degrees * ((i + (Density / 4)) % Density))))) * (WheelSize / 2);
                    case CarouselView2WheelAlignments.Bottom:
                        return -(Math.Sin(DegreesToRadians(360 - (degrees * ((i + (Density / 4)) % Density))))) * (WheelSize / 2);
                }
            case CarouselView2CarouselTypes.Column:
                return ((_maxItemHeight + ItemGap) * (i - (Density / 2)));
            case CarouselView2CarouselTypes.Row:
                return 0;
        }
    }
    protected double GetOffsetX(int i)
    {
        switch (CarouselType)
        {
            default:
                switch (WheelAlignment)
                {
                    default:
                        return (Math.Cos(DegreesToRadians(degrees * i))) * (WheelSize / 2);
                    case CarouselView2WheelAlignments.Left:
                        return -(Math.Cos(DegreesToRadians(360 - (degrees * i)))) * (WheelSize / 2);
                    case CarouselView2WheelAlignments.Top:
                        return (Math.Cos(DegreesToRadians(360 - (degrees * ((i + (Density / 4)) % Density))))) * (WheelSize / 2);
                    case CarouselView2WheelAlignments.Bottom:
                        return (Math.Cos(DegreesToRadians(360 - (degrees * ((i + (Density / 4)) % Density))))) * (WheelSize / 2);
                }
            case CarouselView2CarouselTypes.Row:
                return ((_maxItemWidth + ItemGap) * (i - (Density / 2)));
            case CarouselView2CarouselTypes.Column:
                return 0;
        }
    }

    private int GetRotation(int i)
    {
        if (CarouselType == CarouselView2CarouselTypes.Wheel)
        {
            switch (WheelAlignment)
            {
                default:
                    return Convert.ToInt32(((360 - (degrees * i)) + 180) % 360);
                case CarouselView2WheelAlignments.Left:
                case CarouselView2WheelAlignments.Bottom:
                    return Convert.ToInt32(((degrees * i) + 180) % 360);
            }
        }
        else
        {
            return 0;
        }
    }

    void OnItemsCreated()
    {
        AreItemsLoaded = true;
        EventHandler handler = ItemsCreated;
        if (handler != null)
            handler(this, null);
    }


    void OnItemsLoaded()
    {
        AreItemsLoaded = true;
        EventHandler handler = ItemsLoaded;
        if (handler != null)
            handler(this, null);
    }

    void OnCarouselMovingStateChanged()
    {
        EventHandler handler = CarouselMovingStateChanged;
        if (handler != null)
            handler(this, null);
    }

    void OnSelectionAnimationComplete()
    {
        EventHandler handler = SelectionAnimationComplete;
        if (handler != null)
            handler(this, null);
    }

    void OnSelectionChanged()
    {
        EventHandler handler = SelectionChanged;
        if (handler != null)
            handler(this, null);
    }

    private static void OnSelectedIndexChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == SelectedIndexProperty)
        {
            var control = dependencyObject as CarouselView2;
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
        var control = dependencyObject as CarouselView2;
        control.Refresh();
    }

    internal static int Modulus(int num, int mod)
    {
        int result = num % mod;
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
        int r = x % m;
        return r < 0 ? r + m : r;
    }
}
