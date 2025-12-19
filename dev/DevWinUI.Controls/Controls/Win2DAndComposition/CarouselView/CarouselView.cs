using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

public sealed partial class CarouselView : Control
{
    private const string PART_RootGrid = "PART_RootGrid";
    private const string PART_Canvas = "PART_Canvas";
    private const string PART_IndicatorRect = "PART_IndicatorRect";
    private const string PART_PipsPager = "PART_PipsPager";
    private const string PART_CarouselViewItem1 = "PART_CarouselViewItem1";
    private const string PART_CarouselViewItem2 = "PART_CarouselViewItem2";
    private const string PART_CarouselViewItem3 = "PART_CarouselViewItem3";
    private const string PART_CarouselViewItem4 = "PART_CarouselViewItem4";
    private const string PART_CarouselViewItem5 = "PART_CarouselViewItem5";

    private Grid rootGrid;
    private Grid canvas;
    private PipsPager pipsPager;
    private CarouselViewItem carouselViewItem1;
    private CarouselViewItem carouselViewItem2;
    private CarouselViewItem carouselViewItem3;
    private CarouselViewItem carouselViewItem4;
    private CarouselViewItem carouselViewItem5;

    public delegate void CarouselViewItemClickEventHandler(object sender, CarouselViewItemClickEventArgs e);
    public event CarouselViewItemClickEventHandler ItemClick;

    private Compositor compositor;
    private Visual touchAreaVisual, indicatorVisual;
    private List<Visual> itemVisualList;
    private ExpressionAnimation animation;
    private ExpressionAnimation animation_0;
    private ExpressionAnimation animation_1;
    private ExpressionAnimation animation_2;
    private ExpressionAnimation animation_3;
    private ExpressionAnimation animation_4;
    private ScalarKeyFrameAnimation _indicatorAnimation;
    private float _x;
    private int _selectedIndex;
    private DispatcherTimer _dispatcherTimer;
    private bool _isAnimationRunning = false;
    private List<CarouselViewItem> carouselViewItems;
    private bool ignorePipsChange;
    public CarouselView()
    {
        this.DefaultStyleKey = typeof(CarouselView);
        _selectedIndex = 2;
        _dispatcherTimer = new DispatcherTimer();
        _dispatcherTimer.Tick += _dispatcherTimer_Tick;
        _dispatcherTimer.Interval = this.AutoSwitchInterval;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        rootGrid = GetTemplateChild(PART_RootGrid) as Grid;
        canvas = GetTemplateChild(PART_Canvas) as Grid;
        pipsPager = GetTemplateChild(PART_PipsPager) as PipsPager;
        carouselViewItem1 = GetTemplateChild(PART_CarouselViewItem1) as CarouselViewItem;
        carouselViewItem2 = GetTemplateChild(PART_CarouselViewItem2) as CarouselViewItem;
        carouselViewItem3 = GetTemplateChild(PART_CarouselViewItem3) as CarouselViewItem;
        carouselViewItem4 = GetTemplateChild(PART_CarouselViewItem4) as CarouselViewItem;
        carouselViewItem5 = GetTemplateChild(PART_CarouselViewItem5) as CarouselViewItem;

        carouselViewItems = new List<CarouselViewItem>();

        carouselViewItems.Add(carouselViewItem1);
        carouselViewItems.Add(carouselViewItem2);
        carouselViewItems.Add(carouselViewItem3);
        carouselViewItems.Add(carouselViewItem4);
        carouselViewItems.Add(carouselViewItem5);

        var indiRect = this.GetTemplateChild(PART_IndicatorRect) as Rectangle;
        indicatorVisual = ElementCompositionPreview.GetElementVisual(indiRect);
        touchAreaVisual = ElementCompositionPreview.GetElementVisual(canvas);
        compositor = touchAreaVisual.Compositor;

        itemVisualList = new List<Visual>();

        foreach (var item in carouselViewItems)
        {
            itemVisualList.Add(ElementCompositionPreview.GetElementVisual(item));
            item.Tapped += (s, e) => { OnItemClick((s as CarouselViewItem).ItemSource); };
        }

        this.canvas.ManipulationMode = ManipulationModes.TranslateX;

        canvas.ManipulationStarted -= Canvas_ManipulationStarted;
        canvas.ManipulationStarted += Canvas_ManipulationStarted;

        canvas.ManipulationDelta -= Canvas_ManipulationDelta;
        canvas.ManipulationDelta += Canvas_ManipulationDelta;

        canvas.ManipulationCompleted -= Canvas_ManipulationCompleted;
        canvas.ManipulationCompleted += Canvas_ManipulationCompleted;

        canvas.PointerWheelChanged -= Canvas_PointerWheelChanged;
        canvas.PointerWheelChanged += Canvas_PointerWheelChanged;

        rootGrid.SizeChanged -= OnRootGridSizeChanged;
        rootGrid.SizeChanged += OnRootGridSizeChanged;

        pipsPager.SelectedIndexChanged -= OnPipsPagerSelectedIndexChanged;
        pipsPager.SelectedIndexChanged += OnPipsPagerSelectedIndexChanged;

        SetItemsImageSource(true);
        SetSelectedAppearance();

        if (this.IsAutoSwitchEnabled)
        {
            _dispatcherTimer.Start();
        }

        Unloaded -= OnUnloaded;
        Unloaded += OnUnloaded;
    }

    private void OnPipsPagerSelectedIndexChanged(PipsPager sender, PipsPagerSelectedIndexChangedEventArgs args)
    {
        if (ignorePipsChange)
            return;

        int newIndex = sender.SelectedPageIndex;
        int oldIndex = SelectedIndex;

        if (newIndex == oldIndex)
            return;

        if (IsNext(oldIndex, newIndex))
            GotoNext();
        else
            GotoPrevious();
    }
    private bool IsNext(int oldIndex, int newIndex)
    {
        // User clicked forward direction
        return newIndex > oldIndex ||
               (oldIndex == pipsPager.NumberOfPages - 1 && newIndex == 0);
    }
    private void OnRootGridSizeChanged(object sender, SizeChangedEventArgs e)
    {
        MeasureItemsPosition(_selectedIndex);
        canvas.Clip = new RectangleGeometry() { Rect = RectHelper.FromCoordinatesAndDimensions(0, 0, (float)e.NewSize.Width, (float)e.NewSize.Height) };
    }

    private void OnItemClick(ICarouselViewItemSource e)
    {
        if (e == null) return;
        ItemClick?.Invoke(this, new CarouselViewItemClickEventArgs(e));
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _dispatcherTimer?.Stop();
        _dispatcherTimer = null;
    }

    private void PrepareAnimations()
    {
        animation = compositor.CreateExpressionAnimation("touch.Offset.X");
        animation.SetReferenceParameter("touch", indicatorVisual);

        animation_0 = compositor.CreateExpressionAnimation("touch.Offset.X+self");
        float offsestX_0 = itemVisualList[0].Offset.X;
        animation_0.SetScalarParameter("self", offsestX_0);
        animation_0.SetReferenceParameter("touch", indicatorVisual);

        animation_1 = compositor.CreateExpressionAnimation("touch.Offset.X+self");
        float offsestX_1 = itemVisualList[1].Offset.X;
        animation_1.SetScalarParameter("self", offsestX_1);
        animation_1.SetReferenceParameter("touch", indicatorVisual);

        animation_2 = compositor.CreateExpressionAnimation("touch.Offset.X+self");
        float offsestX_2 = itemVisualList[2].Offset.X;
        animation_2.SetScalarParameter("self", offsestX_2);
        animation_2.SetReferenceParameter("touch", indicatorVisual);

        animation_3 = compositor.CreateExpressionAnimation("touch.Offset.X+self");
        float offsestX_3 = itemVisualList[3].Offset.X;
        animation_3.SetScalarParameter("self", offsestX_3);
        animation_3.SetReferenceParameter("touch", indicatorVisual);

        animation_4 = compositor.CreateExpressionAnimation("touch.Offset.X+self");
        float offsestX_4 = itemVisualList[4].Offset.X;
        animation_4.SetScalarParameter("self", offsestX_4);
        animation_4.SetReferenceParameter("touch", indicatorVisual);
    }

    private void SetItemsImageSource(bool isinitial = false)
    {
        if (ItemImageSource == null || carouselViewItems == null || carouselViewItems.Count == 0 || pipsPager == null)
            return;

        int count = ItemImageSource.Count;
        if (SelectedIndex < 0)
            SelectedIndex = 0;
        int sindex = SelectedIndex;
        int sindex_0, sindex_1, sindex_2, sindex_3, sindex_4;
        sindex_0 = sindex - 2 < 0 ? (sindex - 2) + count : sindex - 2;
        sindex_1 = (sindex - 1) < 0 ? (sindex - 1) + count : sindex - 1;
        sindex_2 = sindex;
        sindex_3 = (sindex + 1) > count - 1 ? (sindex + 1) - count : sindex + 1;
        sindex_4 = (sindex + 2) > count - 1 ? (sindex + 2) - count : sindex + 2;

        // if count=1 all the sindexes = 0
        if (count == 1)
        {
            sindex_0 = sindex_1 = sindex_2 = sindex_3 = sindex_4 = 0;
        }
        // get the UIelement indexes
        int index_0, index_1, index_2, index_3, index_4;
        index_0 = _selectedIndex - 2;
        if (index_0 < 0) index_0 = index_0 + 5;
        index_1 = _selectedIndex - 1;
        if (index_1 < 0) index_1 = index_1 + 5;
        index_2 = _selectedIndex;
        index_3 = _selectedIndex + 1;
        if (index_3 > 4) index_3 = index_3 - 5;
        index_4 = _selectedIndex + 2;
        if (index_4 > 4) index_4 = index_4 - 5;

        pipsPager.NumberOfPages = count;

        carouselViewItems[index_0].ItemSource = ItemImageSource[sindex_0];
        carouselViewItems[index_1].ItemSource = ItemImageSource[sindex_1];
        carouselViewItems[index_2].ItemSource = ItemImageSource[sindex_2];
        carouselViewItems[index_3].ItemSource = ItemImageSource[sindex_3];
        carouselViewItems[index_4].ItemSource = ItemImageSource[sindex_4];

        ignorePipsChange = true;
        pipsPager.SelectedPageIndex = SelectedIndex;
        ignorePipsChange = false;
    }

    private void SetSelectedAppearance()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == _selectedIndex)
            {
                carouselViewItems[i].BlackMaskOpacity = 0.0;
            }
            else
            {
                carouselViewItems[i].BlackMaskOpacity = 0.3;
            }
        }
    }

    private void Canvas_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
    {
        _dispatcherTimer.Stop();
        itemVisualList[0].StopAnimation("Offset.X");
        itemVisualList[1].StopAnimation("Offset.X");
        itemVisualList[2].StopAnimation("Offset.X");
        itemVisualList[3].StopAnimation("Offset.X");
        itemVisualList[4].StopAnimation("Offset.X");
        _x = 0.0f;
        indicatorVisual.Offset = new Vector3(_x, 0.0f, 0.0f);

        PrepareAnimations();

        itemVisualList[0].StartAnimation("Offset.X", animation_0);
        itemVisualList[1].StartAnimation("Offset.X", animation_1);
        itemVisualList[2].StartAnimation("Offset.X", animation_2);
        itemVisualList[3].StartAnimation("Offset.X", animation_3);
        itemVisualList[4].StartAnimation("Offset.X", animation_4);
    }
    private void Canvas_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        _x += (float)e.Delta.Translation.X;

        indicatorVisual.Offset = new Vector3(_x, 0.0f, 0.0f);

    }

    private void Canvas_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        double containerWidth = canvas.ActualWidth;
        double itemWidth = carouselViewItems[0].ActualWidth;
        double cleft = (containerWidth - itemWidth) / 2;
        double threshold = itemWidth / 8;
        int oldSelectedIndex = _selectedIndex;
        var cha = indicatorVisual.Offset.X;
        if (cha <= -threshold)
        {
            _selectedIndex = _selectedIndex + 1;
            if (_selectedIndex > 4)
            {
                _selectedIndex = _selectedIndex - 5;
            }
            var k = SelectedIndex + 1;
            SelectedIndex = k > ItemImageSource.Count - 1 ? k - ItemImageSource.Count : k;
        }
        if (cha >= threshold)
        {
            _selectedIndex = _selectedIndex - 1;
            if (_selectedIndex < 0) _selectedIndex = _selectedIndex + 5;

            var k = SelectedIndex - 1;
            SelectedIndex = k < 0 ? k + ItemImageSource.Count : k;
        }

        MeasureItemsPosition(_selectedIndex, oldSelectedIndex);
        if (IsAutoSwitchEnabled)
            _dispatcherTimer.Start();
    }



    static ContainerVisual GetVisual(UIElement element)
    {
        var hostVisual = ElementCompositionPreview.GetElementVisual(element);
        ContainerVisual root = hostVisual.Compositor.CreateContainerVisual();
        ElementCompositionPreview.SetElementChildVisual(element, root);
        return root;
    }

    /// <summary>
    /// key function of the carousel logic, before calling this, should comfirm:
    /// 1. indicator's Offset.X is correct
    /// 2. set the _selectedIndex and SelectedIndex to new index
    /// note: _selectedIndex and SelectedIndex have diffrent meaning
    /// </summary>
    /// <param name="index"></param>
    /// <param name="oldindex"></param>
    private void MeasureItemsPosition(int index, int oldindex = -1)
    {
        // Set the itemwidth
        // if the container width is larger than ItemWidtn, itemwidth = ItemWidth
        // else itemwidth = container
        double containerWidth = canvas.ActualWidth;
        double itemWidth = carouselViewItems[0].ActualWidth;
        if (containerWidth < this.ItemWidth)
        {
            foreach (var item in carouselViewItems)
            {
                item.Width = containerWidth;
            }
        }
        else if (itemWidth < ItemWidth)
        {
            foreach (var item in carouselViewItems)
            {
                item.Width = ItemWidth;
            }
        }
        itemWidth = carouselViewItems[0].Width;
        double LLeft, Cleft, Rleft;
        Cleft = (containerWidth - itemWidth) / 2;
        LLeft = -(itemWidth - Cleft);
        Rleft = containerWidth - Cleft;

        int index_0, index_1, index_2, index_3, index_4;
        index_0 = index - 2;
        if (index_0 < 0) index_0 = index_0 + 5;
        index_1 = index - 1;
        if (index_1 < 0) index_1 = index_1 + 5;
        index_2 = index;
        index_3 = index + 1;
        if (index_3 > 4) index_3 = index_3 - 5;
        index_4 = index + 2;
        if (index_4 > 4) index_4 = index_4 - 5;

        // for initialing only, or no need to enable animiations
        if (oldindex == -1)
        {
            itemVisualList[index_0].Offset = new Vector3((float)(LLeft - itemWidth), 0, 0);
            itemVisualList[index_1].Offset = new Vector3((float)LLeft, 0, 0);
            itemVisualList[index_2].Offset = new Vector3((float)Cleft, 0, 0);
            itemVisualList[index_3].Offset = new Vector3((float)(Rleft), 0, 0);
            itemVisualList[index_4].Offset = new Vector3((float)(Rleft + itemWidth), 0, 0);
            return;
        }

        int diff = index - oldindex;
        double duration = 500;
        // new selected item equals to current item
        if (diff == 0)
        {
            _indicatorAnimation = compositor.CreateScalarKeyFrameAnimation();
            _indicatorAnimation.InsertKeyFrame(1.0f, 0.0f);
            _indicatorAnimation.Duration = TimeSpan.FromMilliseconds(duration);
        }
        // new selected item is the right item of current item
        if (diff == 1 || diff < -1)
        {
            _indicatorAnimation = compositor.CreateScalarKeyFrameAnimation();
            _indicatorAnimation.InsertKeyFrame(1.0f, (float)-itemWidth);
            _indicatorAnimation.Duration = TimeSpan.FromMilliseconds(duration);
        }
        // new selected item is the left one of current item
        if (diff == -1 || diff > 1)
        {
            _indicatorAnimation = compositor.CreateScalarKeyFrameAnimation();
            _indicatorAnimation.InsertKeyFrame(1.0f, (float)itemWidth);
            _indicatorAnimation.Duration = TimeSpan.FromMilliseconds(duration);
        }

        _isAnimationRunning = true;

        // Start the indicator animiation
        var backScopedBatch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
        indicatorVisual.StartAnimation("Offset.X", _indicatorAnimation);
        backScopedBatch.End();
        backScopedBatch.Completed += (ss, ee) =>
        {
            // reset the firt and last item's postion
            itemVisualList[index_0].Offset = new Vector3((float)(LLeft - itemWidth), 0, 0);
            itemVisualList[index_1].Offset = new Vector3((float)LLeft, 0, 0);
            itemVisualList[index_2].Offset = new Vector3((float)Cleft, 0, 0);
            itemVisualList[index_3].Offset = new Vector3((float)(Rleft), 0, 0);
            itemVisualList[index_4].Offset = new Vector3((float)(Rleft + itemWidth), 0, 0);

            // Change item's imagesources
            SetItemsImageSource();
            // Set Selected Item's appearance
            SetSelectedAppearance();
            // reset animation running flas
            _isAnimationRunning = false;
        };

        // set the dispatcherTimer
        if (IsAutoSwitchEnabled)
        {
            _dispatcherTimer.Start();
        }
        else if (_dispatcherTimer.IsEnabled)
        {
            _dispatcherTimer.Stop();
        }
    }

    private void Canvas_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        // Handle the event
        e.Handled = true;

        if (_isAnimationRunning)
        {
            return;
        }

        // Stop the Animation and reset the postion of indicator
        itemVisualList[0].StopAnimation("Offset.X");
        itemVisualList[1].StopAnimation("Offset.X");
        itemVisualList[2].StopAnimation("Offset.X");
        itemVisualList[3].StopAnimation("Offset.X");
        itemVisualList[4].StopAnimation("Offset.X");
        _x = 0.0f;
        indicatorVisual.Offset = new Vector3(_x, 0.0f, 0.0f);

        // Processing the PointerWheelChanged event by mouse
        var pointerpoint = e.GetCurrentPoint(canvas);
        var mousewheeldelta = pointerpoint.Properties.MouseWheelDelta;

        int newindex = _selectedIndex;
        // MouseWheelDelta:
        // A positive value indicates that the wheel was rotated forward (away from the user) or tilted to the right;
        // A negative value indicates that the wheel was rotated backward (toward the user) or tilted to the left.
        if (mousewheeldelta > 0)
        {
            // get the index of last one
            newindex = _selectedIndex - 1;
            if (newindex < 0)
            {
                newindex = newindex + 5;
            }
            // Change the SelectedIndex
            var k = SelectedIndex - 1;
            SelectedIndex = k < 0 ? k + ItemImageSource.Count : k;
        }
        else if (mousewheeldelta < 0)
        {
            // get the index of next one
            newindex = _selectedIndex + 1;
            if (newindex > 4)
            {
                newindex = newindex - 5;
            }
            // Change the SelectedIndex
            var k = SelectedIndex + 1;
            SelectedIndex = k > ItemImageSource.Count - 1 ? k - ItemImageSource.Count : k;
        }
        PrepareAnimations();
        itemVisualList[0].StartAnimation("Offset.X", animation_0);
        itemVisualList[1].StartAnimation("Offset.X", animation_1);
        itemVisualList[2].StartAnimation("Offset.X", animation_2);
        itemVisualList[3].StartAnimation("Offset.X", animation_3);
        itemVisualList[4].StartAnimation("Offset.X", animation_4);
        // Changed the _selectedIndex
        int oldindex = _selectedIndex;
        _selectedIndex = newindex;
        MeasureItemsPosition(newindex, oldindex);
    }

    private void _dispatcherTimer_Tick(object sender, object e)
    {
        // Implement autoswitch
        if (this.IsAutoSwitchEnabled)
        {
            GotoNext();
        }
        else
        {
            _dispatcherTimer.Stop();
        }
    }

    public void GotoNext()
    {
        // Avoid null crash
        if (ItemImageSource == null || ItemImageSource.Count == 0)
        {
            return;
        }
        // Stop the Animation and reset the postion of indicator
        itemVisualList[0].StopAnimation("Offset.X");
        itemVisualList[1].StopAnimation("Offset.X");
        itemVisualList[2].StopAnimation("Offset.X");
        itemVisualList[3].StopAnimation("Offset.X");
        itemVisualList[4].StopAnimation("Offset.X");
        _x = 0.0f;
        indicatorVisual.Offset = new Vector3(_x, 0.0f, 0.0f);

        int newindex = _selectedIndex;
        // get the index of next one
        newindex = _selectedIndex + 1;
        if (newindex > 4)
        {
            newindex = newindex - 5;
        }
        // Change the SelectedIndex
        var k = SelectedIndex + 1;
        SelectedIndex = k > ItemImageSource.Count - 1 ? k - ItemImageSource.Count : k;

        PrepareAnimations();
        itemVisualList[0].StartAnimation("Offset.X", animation_0);
        itemVisualList[1].StartAnimation("Offset.X", animation_1);
        itemVisualList[2].StartAnimation("Offset.X", animation_2);
        itemVisualList[3].StartAnimation("Offset.X", animation_3);
        itemVisualList[4].StartAnimation("Offset.X", animation_4);
        // Changed the _selectedIndex
        int oldindex = _selectedIndex;
        _selectedIndex = newindex;
        MeasureItemsPosition(newindex, oldindex);
    }

    public void GotoPrevious()
    {
        // Avoid null crash
        if (ItemImageSource == null || ItemImageSource.Count == 0)
        {
            return;
        }
        // Stop the Animation and reset the postion of indicator
        itemVisualList[0].StopAnimation("Offset.X");
        itemVisualList[1].StopAnimation("Offset.X");
        itemVisualList[2].StopAnimation("Offset.X");
        itemVisualList[3].StopAnimation("Offset.X");
        itemVisualList[4].StopAnimation("Offset.X");
        _x = 0.0f;
        indicatorVisual.Offset = new Vector3(_x, 0.0f, 0.0f);

        int newindex = _selectedIndex;
        // get the index of last one
        newindex = _selectedIndex - 1;
        if (newindex < 0)
        {
            newindex = newindex + 5;
        }
        // Change the SelectedIndex
        var k = SelectedIndex - 1;
        SelectedIndex = k < 0 ? k + ItemImageSource.Count : k;

        PrepareAnimations();
        itemVisualList[0].StartAnimation("Offset.X", animation_0);
        itemVisualList[1].StartAnimation("Offset.X", animation_1);
        itemVisualList[2].StartAnimation("Offset.X", animation_2);
        itemVisualList[3].StartAnimation("Offset.X", animation_3);
        itemVisualList[4].StartAnimation("Offset.X", animation_4);
        // Changed the _selectedIndex
        int oldindex = _selectedIndex;
        _selectedIndex = newindex;
        MeasureItemsPosition(newindex, oldindex);
    }
}

