using Windows.Foundation.Metadata;

namespace DevWinUI;

public partial class BannerView : FlipView
{
    private const string ScrollingHost = "ScrollingHost";
    private ScrollViewer scrollingHost;

    internal Compositor Compositor;

    private CompositionPropertySet scrollProps;
    private CompositionPropertySet props;
    private Visual thisVisual;
    private Visual panelVisual;
    private ExpressionAnimation perspectiveMatrixExp;
    private ExpressionAnimation panelPerspectiveExp;
    private CycleSelectionState CycleSelectionState;

    private const string IndexNode = "(target.Offset.X / target.Size.X)";

    private const string SelectedIndexNode = "(-scroll.Translation.X / target.Size.X)";

    private const string DistanceNode = "((scroll.Translation.X + target.Offset.X) / target.Size.X)";

    private static readonly string ClampedDistanceNode = $"Clamp({DistanceNode}, -1f, 1f)";

    protected bool IsLoaded { get; private set; }
    private DispatcherTimer timer;
    public BannerView()
    {
        this.DefaultStyleKey = typeof(BannerView);

        RegisterPropertyChangedCallback(FlipView.VerticalContentAlignmentProperty, VerticalContentAlignmentPropertyChanged);

        this.SelectionChanged -= OnSelectionChanged;
        this.SelectionChanged += OnSelectionChanged;
        this.Loaded -= OnLoaded;
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        IsLoaded = true;
        InitComposition();
        InitCycleSelectedIndex();
        UpdateSelectedIndex();

        if (ItemsPanelRoot is VirtualizingStackPanel vsp)
        {
            vsp.CleanUpVirtualizedItemEvent += (s, a) =>
            {
                if (a.UIElement is BannerViewItem container && container.IsCycleItemContainer)
                {
                    a.Cancel = true;
                }
            };
        }
    }

    private void VerticalContentAlignmentPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        UpdateCenterPoint();
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IsCycleEnable())
        {
            var list = GetCycleItemsSource();
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
        return ItemsSource.GetType().GetGenericTypeDefinition() == typeof(CycleCollection<>);
    }

    protected ICycleCollection GetCycleItemsSource()
    {
        return (ICycleCollection)ItemsSource;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        scrollingHost = GetTemplateChild(ScrollingHost) as ScrollViewer;
        scrollingHost.ViewChanged -= ScrollingHost_ViewChanged;
        scrollingHost.ViewChanged += ScrollingHost_ViewChanged;

        timer = new DispatcherTimer();
        timer.Interval = Interval;
        timer.Tick -= Timer_Tick;
        timer.Tick += Timer_Tick;

        Unloaded -= OnUnloaded;
        Unloaded += OnUnloaded;

        OnAutoShuffleChanged();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        timer?.Stop();
        timer = null;
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
        timer?.Start();
    }
    private void Timer_Tick(object sender, object e)
    {
        switch (ShiftingDirection)
        {
            case CarouselShiftingDirection.Forward:
                SelectedIndex++;
                break;
            case CarouselShiftingDirection.Backward:
                SelectedIndex--;
                break;
        }
    }

    private void ScrollingHost_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        if (!e.IsIntermediate)
        {
            if (!IsLoaded) return;
            if (!IsCycleEnable()) return;
            if (CycleSelectionState != CycleSelectionState.None)
            {
                CycleSelectionState = CycleSelectionState.None;
                UpdateSelectedIndex();
            }
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
            if (IsCycleItem(item))
            {
                container.IsCycleItemContainer = true;
            }
            else
            {
                container.IsCycleItemContainer = false;
            }
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
        if (IsCycleEnable())
        {
            var list = GetCycleItemsSource();
            return list.IsCycleItem(item);
        }
        return false;
    }

    private void InitComposition()
    {
        Compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        scrollProps = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollingHost);
        thisVisual = ElementCompositionPreview.GetElementVisual(this);
        panelVisual = ElementCompositionPreview.GetElementVisual(ItemsPanelRoot);
        props = Compositor.CreatePropertySet();
        props.InsertMatrix4x4("perspective", Matrix4x4.Identity);
        props.InsertScalar("ItemsSpacing", 0f);
        props.InsertScalar("PerspectiveSpacing", 20f);
        props.InsertScalar("CenterPointY", 0f);

        UpdateCenterPoint();

        for (int i = 0; i < ItemsPanelRoot.Children.Count; i++)
        {
            CreateAnimation(ItemsPanelRoot.Children[i]);
        }

        UpdateScale();
        UpdatePerspective();
        UpdateItemSpacing();
    }

    private void UpdateCenterPoint()
    {
        if (props != null)
        {
            ExpressionAnimation ex = null;
            switch (VerticalContentAlignment)
            {
                case VerticalAlignment.Top:
                    ex = Compositor.CreateExpressionAnimation("0f");
                    break;
                case VerticalAlignment.Bottom:
                    ex = Compositor.CreateExpressionAnimation("target.Size.Y");
                    break;
                default:
                    ex = Compositor.CreateExpressionAnimation("target.Size.Y / 2");
                    break;
            }
            ex.SetReferenceParameter("target", ElementCompositionPreview.GetElementVisual(this));
            props.StartAnimation("CenterPointY", ex);
        }
    }

    private void CreateAnimation(UIElement element)
    {
        if (scrollProps == null) return;

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

        var itemScaleExp = Compositor.CreateExpressionAnimation($"1 - abs({ClampedDistanceNode}) * 0.1f");
        itemScaleExp.SetReferenceParameter("scroll", scrollProps);
        itemScaleExp.SetReferenceParameter("target", visual);

        visual.StartAnimation("Scale.X", itemScaleExp);
        visual.StartAnimation("Scale.Y", itemScaleExp);
    }

    private void CreateOffsetAnimation(UIElement element)
    {
        var visual = ElementCompositionPreview.GetElementVisual(element);

        var itemOffsetExp = Compositor.CreateExpressionAnimation($"-{ClampedDistanceNode} * (prop.ItemsSpacing + prop.PerspectiveSpacing)");
        itemOffsetExp.SetReferenceParameter("prop", props);
        itemOffsetExp.SetReferenceParameter("scroll", scrollProps);
        itemOffsetExp.SetReferenceParameter("target", visual);

        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 4))
        {
            ElementCompositionPreview.SetIsTranslationEnabled(element, true);
            visual.StartAnimation("Translation.X", itemOffsetExp);
        }
        else if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3))
        {
            var child = VisualTreeHelper.GetChild(element, 0) as UIElement;
            if (child == null)
            {
                ((FrameworkElement)element).Loaded += _Loaded;
            }
            else
            {
                _Loaded(null, null);
            }

            void _Loaded(object sender, RoutedEventArgs e)
            {
                child = VisualTreeHelper.GetChild(element, 0) as UIElement;
                if (child != null)
                {
                    ElementCompositionPreview.GetElementVisual(child).StartAnimation("Offset.X", itemOffsetExp);
                }
            }
        }
    }


    private void CreateCenterPointAnimation(UIElement element)
    {
        var visual = ElementCompositionPreview.GetElementVisual(element);

        var itemCenterPointExp = Compositor.CreateExpressionAnimation($"Vector3((1 - {ClampedDistanceNode}) * target.Size.X * 0.5, prop.CenterPointY, 0f)");
        itemCenterPointExp.SetReferenceParameter("prop", props);
        itemCenterPointExp.SetReferenceParameter("scroll", scrollProps);
        itemCenterPointExp.SetReferenceParameter("target", visual);

        visual.StartAnimation("CenterPoint", itemCenterPointExp);
    }

    private void CreateRotationAnimation(UIElement element)
    {
        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 4))
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);

            var itemRotationExp = Compositor.CreateExpressionAnimation($"0.2 * Pi * {ClampedDistanceNode}");
            itemRotationExp.SetReferenceParameter("scroll", scrollProps);
            itemRotationExp.SetReferenceParameter("target", visual);

            visual.RotationAxis = new Vector3(0f, 1f, 0f);

            visual.StartAnimation("RotationAngle", itemRotationExp);
        }
    }

    private void InitCycleSelectedIndex()
    {
        if (IsCycleEnable())
        {
            var list = GetCycleItemsSource();
            if (list.Count > 2)
            {
                SelectedIndex = list.ConvertFromItemIndex(0);
                OnCycleSelectionChanged(new SelectionChangedEventArgs(new object[] { }, new object[] { list[SelectedIndex] }));
            }
        }
    }

    private void UpdateSelectedIndex()
    {
        if (IsCycleEnable())
        {
            int index = -1;
            var list = GetCycleItemsSource();
            if (list.IsCycleItem(SelectedIndex))
            {
                index = list.ConvertFromItemIndex(list.ConvertToItemIndex(SelectedIndex));
            }

            if (index != -1)
            {
                SelectedIndex = index;
                UpdateLayout();
            }

            OnCycleSelectionChanged(new SelectionChangedEventArgs(new object[] { }, new object[] { SelectedIndex }));
        }
    }


    private void UpdateItemSpacing()
    {
        if (props != null)
        {
            props.InsertScalar("ItemsSpacing", (float)ItemsSpacing);
        }
    }

    private void UpdateScale()
    {
        if (ItemsPanelRoot != null)
        {
            if (IsScaleEnable)
            {
                foreach (var item in ItemsPanelRoot.Children)
                {
                    CreateScaleAnimation(item);
                }
            }
            else
            {
                foreach (var item in ItemsPanelRoot.Children)
                {
                    var visual = ElementCompositionPreview.GetElementVisual(item);
                    visual.StopAnimation("Scale.X");
                    visual.StopAnimation("Scale.Y");
                    visual.Scale = Vector3.One;
                }
            }
        }
    }

    private void UpdatePerspective()
    {
        if (props == null) return;

        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 4) && IsPerspectiveEnable)
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
                panelPerspectiveExp.SetReferenceParameter("panel", panelVisual);
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

            panelVisual.StopAnimation("TransformMatrix");
            props.StopAnimation("perspective");

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
