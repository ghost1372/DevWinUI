namespace DevWinUI;

public partial class BannerView : FlipView
{
    private const string ScrollingHost = "ScrollingHost";

    private const string DistanceNode = "((scroll.Translation.X + target.Offset.X) / target.Size.X)";
    private const string ClampedDistanceNode = $"Clamp({DistanceNode}, -1f, 1f)";
    private const string ScaleExpression = $"1 - abs({ClampedDistanceNode}) * 0.1f";
    private const string OffsetExpression = $"-{ClampedDistanceNode} * (prop.ItemsSpacing + prop.PerspectiveSpacing)";
    private const string CenterPointExpression = $"Vector3((1 - {ClampedDistanceNode}) * target.Size.X * 0.5, prop.CenterPointY, 0f)";
    private const string RotationExpression = $"0.2 * Pi * {ClampedDistanceNode}";

    private readonly DispatcherTimer timer = new();
    private ScrollViewer scrollingHost;

    internal Compositor Compositor;

    private CompositionPropertySet scrollProps;
    private CompositionPropertySet props;
    private Visual thisVisual;
    private Visual panelVisual;
    private ExpressionAnimation perspectiveMatrixExp;
    private ExpressionAnimation panelPerspectiveExp;
    private CycleSelectionState CycleSelectionState;

    public BannerView()
    {
        this.DefaultStyleKey = typeof(BannerView);

        RegisterPropertyChangedCallback(FlipView.VerticalContentAlignmentProperty, VerticalContentAlignmentPropertyChanged);

        timer.Interval = Interval;
        timer.Tick += Timer_Tick;

        SelectionChanged += OnSelectionChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        InitComposition();
        InitCycleSelectedIndex();
        UpdateSelectedIndex();

        if (ItemsPanelRoot is VirtualizingStackPanel vsp)
        {
            vsp.CleanUpVirtualizedItemEvent -= OnCleanUpVirtualizedItem;
            vsp.CleanUpVirtualizedItemEvent += OnCleanUpVirtualizedItem;
        }

        OnAutoShuffleChanged();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        timer.Stop();

        if (ItemsPanelRoot is VirtualizingStackPanel vsp)
        {
            vsp.CleanUpVirtualizedItemEvent -= OnCleanUpVirtualizedItem;
        }

        DisposeComposition();
    }

    private void DisposeComposition()
    {
        var perspective = perspectiveMatrixExp;
        var panelPerspective = panelPerspectiveExp;
        var properties = props;

        perspectiveMatrixExp = null;
        panelPerspectiveExp = null;
        props = null;
        scrollProps = null;
        thisVisual = null;
        panelVisual = null;

        try
        {
            perspective?.Dispose();
            panelPerspective?.Dispose();
            properties?.Dispose();
        }
        catch (Exception ex) when (ex is ObjectDisposedException || ex is System.Runtime.InteropServices.COMException)
        {
        }
    }

    private void OnCleanUpVirtualizedItem(object sender, CleanUpVirtualizedItemEventArgs e)
    {
        if (e.UIElement is BannerViewItem container && container.IsCycleItemContainer)
        {
            e.Cancel = true;
        }
    }

    private void VerticalContentAlignmentPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        UpdateCenterPoint();
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ItemsSource is ICycleCollection list)
        {
            if (list.IsHeader(SelectedIndex))
            {
                CycleSelectionState = CycleSelectionState.Footer;
            }
            else if (list.IsFooter(SelectedIndex))
            {
                CycleSelectionState = CycleSelectionState.Header;
            }
        }
    }

    protected bool IsCycleEnable()
    {
        return ItemsSource is ICycleCollection;
    }

    protected ICycleCollection GetCycleItemsSource()
    {
        return (ICycleCollection)ItemsSource;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (scrollingHost != null)
        {
            scrollingHost.ViewChanged -= ScrollingHost_ViewChanged;
        }

        scrollingHost = GetTemplateChild(ScrollingHost) as ScrollViewer;

        if (scrollingHost != null)
        {
            scrollingHost.ViewChanged += ScrollingHost_ViewChanged;
        }
    }

    public void StopShuffle()
    {
        timer.Stop();
    }

    public void PlayShuffleForward()
    {
        if (!AutoShuffle)
            return;

        ShiftingDirection = CarouselShiftingDirection.Forward;
        StartShuffleInternal();
    }

    public void PlayShuffleBackward()
    {
        if (!AutoShuffle)
            return;

        ShiftingDirection = CarouselShiftingDirection.Backward;
        StartShuffleInternal();
    }

    private void StartShuffleInternal()
    {
        if (IsLoaded)
        {
            timer.Start();
        }
    }

    private void Timer_Tick(object sender, object e)
    {
        int count = Items.Count;
        if (count == 0)
            return;

        int next = SelectedIndex + (ShiftingDirection == CarouselShiftingDirection.Forward ? 1 : -1);

        if (next < 0 || next >= count)
        {
            if (IsCycleEnable())
                return;

            next = (next % count + count) % count;
        }

        SelectedIndex = next;
    }

    private void ScrollingHost_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        if (e.IsIntermediate || !IsLoaded || !IsCycleEnable())
            return;

        if (CycleSelectionState != CycleSelectionState.None)
        {
            CycleSelectionState = CycleSelectionState.None;
            UpdateSelectedIndex();
        }
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        return new BannerViewItem();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is BannerViewItem;
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        if (element is BannerViewItem container)
        {
            container.IsCycleItemContainer = IsCycleItem(item);
        }

        CreateAnimation((UIElement)element);
        base.PrepareContainerForItemOverride(element, item);
    }

    protected override void ClearContainerForItemOverride(DependencyObject element, object item)
    {
        if (element is BannerViewItem container)
        {
            container.IsCycleItemContainer = false;
        }

        base.ClearContainerForItemOverride(element, item);
    }

    private bool IsCycleItem(object item)
    {
        return ItemsSource is ICycleCollection list && list.IsCycleItem(item);
    }

    private void InitComposition()
    {
        if (scrollingHost == null || ItemsPanelRoot == null)
            return;

        DisposeComposition();

        Compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        scrollProps = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollingHost);
        thisVisual = ElementCompositionPreview.GetElementVisual(this);
        panelVisual = ElementCompositionPreview.GetElementVisual(ItemsPanelRoot);

        props = Compositor.CreatePropertySet();
        props.InsertMatrix4x4("perspective", Matrix4x4.Identity);
        props.InsertScalar("ItemsSpacing", (float)ItemsSpacing);
        props.InsertScalar("PerspectiveSpacing", 20f);
        props.InsertScalar("CenterPointY", 0f);

        UpdateCenterPoint();

        foreach (var child in ItemsPanelRoot.Children)
        {
            CreateOffsetAnimation(child);
            CreateCenterPointAnimation(child);
        }

        UpdateScale();
        UpdatePerspective();
    }

    private void UpdateCenterPoint()
    {
        if (props == null)
            return;

        var expression = VerticalContentAlignment switch
        {
            VerticalAlignment.Top => "0f",
            VerticalAlignment.Bottom => "target.Size.Y",
            _ => "target.Size.Y / 2"
        };

        var ex = Compositor.CreateExpressionAnimation(expression);
        ex.SetReferenceParameter("target", thisVisual);
        props.StartAnimation("CenterPointY", ex);
    }

    private void CreateAnimation(UIElement element)
    {
        if (scrollProps == null)
            return;

        CreateOffsetAnimation(element);
        CreateCenterPointAnimation(element);

        if (IsScaleEnable)
        {
            CreateScaleAnimation(element);
        }

        if (IsPerspectiveEnable)
        {
            CreateRotationAnimation(element);
        }
    }

    private void CreateScaleAnimation(UIElement element)
    {
        var visual = ElementCompositionPreview.GetElementVisual(element);

        var itemScaleExp = Compositor.CreateExpressionAnimation(ScaleExpression);
        itemScaleExp.SetReferenceParameter("scroll", scrollProps);
        itemScaleExp.SetReferenceParameter("target", visual);

        visual.StartAnimation("Scale.X", itemScaleExp);
        visual.StartAnimation("Scale.Y", itemScaleExp);
    }

    private void CreateOffsetAnimation(UIElement element)
    {
        var visual = ElementCompositionPreview.GetElementVisual(element);

        var itemOffsetExp = Compositor.CreateExpressionAnimation(OffsetExpression);
        itemOffsetExp.SetReferenceParameter("prop", props);
        itemOffsetExp.SetReferenceParameter("scroll", scrollProps);
        itemOffsetExp.SetReferenceParameter("target", visual);

        ElementCompositionPreview.SetIsTranslationEnabled(element, true);
        visual.StartAnimation("Translation.X", itemOffsetExp);
    }

    private void CreateCenterPointAnimation(UIElement element)
    {
        var visual = ElementCompositionPreview.GetElementVisual(element);

        var itemCenterPointExp = Compositor.CreateExpressionAnimation(CenterPointExpression);
        itemCenterPointExp.SetReferenceParameter("prop", props);
        itemCenterPointExp.SetReferenceParameter("scroll", scrollProps);
        itemCenterPointExp.SetReferenceParameter("target", visual);

        visual.StartAnimation("CenterPoint", itemCenterPointExp);
    }

    private void CreateRotationAnimation(UIElement element)
    {
        var visual = ElementCompositionPreview.GetElementVisual(element);

        var itemRotationExp = Compositor.CreateExpressionAnimation(RotationExpression);
        itemRotationExp.SetReferenceParameter("scroll", scrollProps);
        itemRotationExp.SetReferenceParameter("target", visual);

        visual.RotationAxis = new Vector3(0f, 1f, 0f);
        visual.StartAnimation("RotationAngle", itemRotationExp);
    }

    private void InitCycleSelectedIndex()
    {
        if (ItemsSource is ICycleCollection list && list.Count > 2)
        {
            SelectedIndex = list.ConvertFromItemIndex(0);
            RaiseCycleSelectionChanged();
        }
    }

    private void UpdateSelectedIndex()
    {
        if (ItemsSource is not ICycleCollection list)
            return;

        if (list.IsCycleItem(SelectedIndex))
        {
            int index = list.ConvertFromItemIndex(list.ConvertToItemIndex(SelectedIndex));

            if (index != -1)
            {
                SelectedIndex = index;
                UpdateLayout();
            }
        }

        RaiseCycleSelectionChanged();
    }

    private void RaiseCycleSelectionChanged()
    {
        if (SelectedItem != null)
        {
            OnCycleSelectionChanged(new SelectionChangedEventArgs(Array.Empty<object>(), new object[] { SelectedItem }));
        }
    }

    private void UpdateItemSpacing()
    {
        props?.InsertScalar("ItemsSpacing", (float)ItemsSpacing);
    }

    private void UpdateScale()
    {
        if (scrollProps == null || ItemsPanelRoot == null)
            return;

        foreach (var item in ItemsPanelRoot.Children)
        {
            if (IsScaleEnable)
            {
                CreateScaleAnimation(item);
            }
            else
            {
                var visual = ElementCompositionPreview.GetElementVisual(item);
                visual.StopAnimation("Scale.X");
                visual.StopAnimation("Scale.Y");
                visual.Scale = Vector3.One;
            }
        }
    }

    private void UpdatePerspective()
    {
        if (props == null || ItemsPanelRoot == null)
            return;

        if (IsPerspectiveEnable)
        {
            props.InsertScalar("PerspectiveSpacing", 0f);

            if (perspectiveMatrixExp == null)
            {
                perspectiveMatrixExp = Compositor.CreateExpressionAnimation("Matrix4x4(1.0f, 0.0f, 0.0f, 0.0f," +
                                                                            "0.0f, 1.0f, 0.0f, 0.0f," +
                                                                            "0.0f, 0.0f, 1.0f, -1.0f / target.Size.X," +
                                                                            "0.0f, 0.0f, 0.0f, 1.0f)");
                perspectiveMatrixExp.SetReferenceParameter("target", thisVisual);
            }

            if (panelPerspectiveExp == null)
            {
                panelPerspectiveExp = Compositor.CreateExpressionAnimation("Matrix4x4.CreateFromTranslation(Vector3((scroll.Translation.X - target.Size.X / 2), -target.Size.Y / 2 , 0f)) * " +
                                                                           "prop.perspective * " +
                                                                           "Matrix4x4.CreateFromTranslation(Vector3((target.Size.X / 2 - scroll.Translation.X), target.Size.Y / 2, 0f))");
                panelPerspectiveExp.SetReferenceParameter("scroll", scrollProps);
                panelPerspectiveExp.SetReferenceParameter("target", thisVisual);
                panelPerspectiveExp.SetReferenceParameter("prop", props);
            }

            props.StartAnimation("perspective", perspectiveMatrixExp);
            panelVisual.StartAnimation("TransformMatrix", panelPerspectiveExp);

            foreach (var item in ItemsPanelRoot.Children)
            {
                CreateRotationAnimation(item);
            }
        }
        else
        {
            props.InsertScalar("PerspectiveSpacing", 20f);

            props.StopAnimation("perspective");
            props.InsertMatrix4x4("perspective", Matrix4x4.Identity);
            panelVisual.StopAnimation("TransformMatrix");
            panelVisual.TransformMatrix = Matrix4x4.Identity;

            foreach (var item in ItemsPanelRoot.Children)
            {
                var visual = ElementCompositionPreview.GetElementVisual(item);
                visual.StopAnimation("RotationAngle");
                visual.RotationAngle = 0f;
            }
        }
    }

    public event SelectionChangedEventHandler CycleSelectionChanged;

    protected void OnCycleSelectionChanged(SelectionChangedEventArgs e)
    {
        CycleSelectionChanged?.Invoke(this, e);
    }

    private void OnAutoShuffleChanged()
    {
        if (AutoShuffle)
            StartShuffleInternal();
        else
            StopShuffle();
    }
}
