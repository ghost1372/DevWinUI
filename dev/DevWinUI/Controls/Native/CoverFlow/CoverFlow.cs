using System.ComponentModel;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace DevWinUI;

public partial class CoverFlow : ItemsControl, INotifyPropertyChanged
{
    private const string PART_ItemsPresenter = "PART_ItemsPresenter";
    private ItemsPresenter itemsPresenter;

    public delegate void SelectedItemChangedEvent(CoverFlowEventArgs e);

    public event PropertyChangedEventHandler PropertyChanged;
    public event SelectedItemChangedEvent SelectedItemChanged;

    private Dictionary<object, CoverFlowItem> _objectToItemContainer;
    private List<CoverFlowItem> items;
    private readonly HashSet<CoverFlowItem> _itemSet = new();

    // Reused for the clip instead of creating a new geometry on every arrange pass.
    private readonly RectangleGeometry _visibleArea = new RectangleGeometry();
    private Rect _lastClip = Rect.Empty;

    private struct LayoutMetrics
    {
        public double Width;
        public double K;
        public double L;
        public double R;
        public double Z;
        public double Scale;
        public int Selected;
        public int Count;
    }

    private double manipulationDistance = 0.0;
    private bool isManipulationActive = true;

    private int selectedIndex;
    private Duration duration;

    public CoverFlow()
    {
        DefaultStyleKey = typeof(CoverFlow);
        items = new List<CoverFlowItem>();
        SingleItemDuration = new Duration(TimeSpan.FromMilliseconds(600));
        PageDuration = new Duration(TimeSpan.FromMilliseconds(900));
        duration = SingleItemDuration;
        EasingFunction = new CubicEase();
    }

    public int SelectedIndex
    {
        get { return selectedIndex; }
        set { IndexSelected(value); }
    }

    public object SelectedItem
    {
        get
        {
            return items.Count > 0 && SelectedIndex >= 0 && SelectedIndex < items.Count ? items[SelectedIndex].Content : null;
        }
        set
        {
            CoverFlowItem o = GetItemContainerForObject(value);
            if (o != null)
                SelectedIndex = items.IndexOf(o);
        }
    }

    protected void OnItemSelected(object sender, EventArgs e)
    {
        CoverFlowItem item = sender as CoverFlowItem;
        if (item == null)
            return;
        int index = items.IndexOf(item);
        if (index >= 0)
            IndexSelected(index);
    }

    private void IndexSelected(int index)
    {
        IndexSelected(index, true);
    }

    private void IndexSelected(int index, bool layoutChildren)
    {
        if (items.Count > 0 && index >= 0 && index < items.Count)
        {
            selectedIndex = index;
            if (layoutChildren)
                LayoutChildren();

            CoverFlowEventArgs e = new CoverFlowEventArgs(index, items[index].Content);

            if (SelectedItemChanged != null)
                SelectedItemChanged(e);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedIndex"));
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedItem"));
            }
        }
    }
    private double k { get { return SpaceBetweenSelectedItemAndItems; } }

    private double l { get { return SpaceBetweenItems; } }

    private double r { get { return RotationAngle; } }

    private double z { get { return ZDistance; } }

    protected CoverFlowItem GetItemContainerForObject(object value)
    {
        CoverFlowItem item = value as CoverFlowItem;
        if (item == null && value != null)
        {
            this.ObjectToItemContainer.TryGetValue(value, out item);
        }
        return item;
    }

    protected Dictionary<object, CoverFlowItem> ObjectToItemContainer
    {
        get
        {
            if (this._objectToItemContainer == null)
            {
                this._objectToItemContainer = new Dictionary<object, CoverFlowItem>();
            }
            return this._objectToItemContainer;
        }
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        CoverFlowItem item = new CoverFlowItem();
        return item;
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return (item is CoverFlowItem);
    }

    private static void OnValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as CoverFlow).LayoutChildren();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        itemsPresenter = GetTemplateChild(PART_ItemsPresenter) as ItemsPresenter;

        this.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateRailsX;

        // OnApplyTemplate can run more than once; don't stack the handlers.
        this.ManipulationStarted -= OnManipulationStarted;
        this.ManipulationStarted += OnManipulationStarted;
        this.ManipulationDelta -= OnManipulationDelta;
        this.ManipulationDelta += OnManipulationDelta;
    }

    protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
    {
        int wheelDelta = e.GetCurrentPoint(null).Properties.MouseWheelDelta;
        if (wheelDelta > 0 && this.SelectedIndex > 0)
            PreviousItem();
        else if (wheelDelta < 0 && this.SelectedIndex < Items.Count - 1)
            NextItem();

        base.OnPointerWheelChanged(e);
    }

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        // Arrows and US keyboard.

        if (e.Key == VirtualKey.Right || e.Key == VirtualKey.D)
        {
            NextItem();
            e.Handled = true;
        }
        else if (e.Key == VirtualKey.Left || e.Key == VirtualKey.A)
        {
            PreviousItem();
            e.Handled = true;
        }
        else if (e.Key == VirtualKey.PageDown || e.Key == VirtualKey.S || e.Key == VirtualKey.Down)
        {
            NextPage();
            e.Handled = true;
        }
        else if (e.Key == VirtualKey.PageUp || e.Key == VirtualKey.W || e.Key == VirtualKey.Up)
        {
            PreviousPage();
            e.Handled = true;
        }
        else if (e.Key == VirtualKey.Home || e.Key == VirtualKey.Q)
        {
            First();
            e.Handled = true;
        }
        else if (e.Key == VirtualKey.End || e.Key == VirtualKey.E)
        {
            Last();
            e.Handled = true;
        }
    }

    private void OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
    {
        manipulationDistance = 0.0;
        isManipulationActive = true;
    }

    private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        manipulationDistance += e.Delta.Translation.X;

        if (isManipulationActive || manipulationDistance < -ManipulationThreshold || manipulationDistance > ManipulationThreshold)
        {
            manipulationDistance = 0.0;
            isManipulationActive = false;

            // TODO: find a way to give focus to the control, so that keyboard manipulation is restored.

            if (e.Delta.Translation.X < 0 && this.SelectedIndex < Items.Count - 1)
                NextItem();
            else if (e.Delta.Translation.X > 0 && this.SelectedIndex > 0)
                PreviousItem();
        }
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);

        if (element is not CoverFlowItem container)
            return;

        ObjectToItemContainer[item] = container;

        if (_itemSet.Add(container))
        {
            items.Add(container);
            container.ItemSelected += OnItemSelected;
            container.SizeChanged += OnItemSizeChanged;
        }

        if (items.Count == 1)
            IndexSelected(0, false);
    }


    protected void OnItemSizeChanged(object sender, SizeChangedEventArgs e)
    {
        CoverFlowItem item = sender as CoverFlowItem;
        int index = items.IndexOf(item);
        if (index < 0)
            return;

        LayoutChild(item, index);
    }

    protected override void ClearContainerForItemOverride(DependencyObject element, object item)
    {
        base.ClearContainerForItemOverride(element, item);

        if (element is not CoverFlowItem container)
            return;

        ObjectToItemContainer.Remove(item);
        items.Remove(container);
        _itemSet.Remove(container);

        // Keep the selection valid, otherwise SelectedItem would index past the end.
        if (selectedIndex >= items.Count)
            selectedIndex = Math.Max(0, items.Count - 1);

        container.ItemSelected -= OnItemSelected;
        container.SizeChanged -= OnItemSizeChanged;
    }


    private LayoutMetrics GetLayoutMetrics()
    {
        // Read the dependency properties once per layout pass instead of once per item.
        return new LayoutMetrics
        {
            Width = itemsPresenter.ActualWidth,
            K = k,
            L = l,
            R = r,
            Z = z,
            Scale = Scale,
            Selected = SelectedIndex,
            Count = items.Count
        };
    }

    private static double GetTargetX(in LayoutMetrics m, int b, double mu, double itemWidth)
    {
        return (m.Width / 2 + ((double)b * m.L + (m.K * mu))) - (itemWidth / 2);
    }

    protected void LayoutChildren()
    {
        if (itemsPresenter == null)
            return;

        var m = GetLayoutMetrics();
        for (int i = 0; i < items.Count; i++)
        {
            LayoutChild(items[i], i, m);
        }
    }

    protected void LayoutChild(CoverFlowItem item, int index)
    {
        if (itemsPresenter == null)
            return;

        LayoutChild(item, index, GetLayoutMetrics());
    }

    private void LayoutChild(CoverFlowItem item, int index, in LayoutMetrics m)
    {
        int b = index - m.Selected;
        double mu = Math.Sign(b);
        double x = GetTargetX(m, b, mu, item.ActualWidth);

        double s = mu == 0 ? 1 : m.Scale;

        int zindex = m.Count - Math.Abs(b);

        if (((x + item.ActualWidth) < 0 || x > m.Width)
            && ((item.X + item.ActualWidth) < 0 || item.X > m.Width)
            && !((x + item.ActualWidth) < 0 && item.X > m.Width)
            && !((item.X + item.ActualWidth) < 0 && x > m.Width))
        {
            item.SetValues(x, zindex, m.R * mu, m.Z * Math.Abs(mu), s, duration, EasingFunction, false);
        }
        else
        {
            item.SetValues(x, zindex, m.R * mu, m.Z * Math.Abs(mu), s, duration, EasingFunction, true);
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (itemsPresenter == null)
            return finalSize;

        Size size = base.ArrangeOverride(finalSize);

        double presenterHeight = itemsPresenter.ActualHeight;
        Rect clip = new Rect(0, 0, itemsPresenter.ActualWidth, presenterHeight);
        if (clip != _lastClip)
        {
            _visibleArea.Rect = clip;
            _lastClip = clip;
        }
        if (!ReferenceEquals(itemsPresenter.Clip, _visibleArea))
        {
            itemsPresenter.Clip = _visibleArea;
        }

        foreach (CoverFlowItem item in items)
        {
            if (item.Height != presenterHeight)
            {
                item.Height = presenterHeight;
            }
        }

        var m = GetLayoutMetrics();

        for (int index = 0; index < items.Count; index++)
        {
            CoverFlowItem item = items[index];
            int b = index - m.Selected;
            double mu = Math.Sign(b);
            double x = GetTargetX(m, b, mu, item.ActualWidth);

            double s = mu == 0 ? 1 : m.Scale;

            item.X = x;
            item.YRotation = m.R * mu;
            item.ZOffset = m.Z * Math.Abs(mu);
            item.Scale = s;

            ////Use scale factor for opacity
            //item.Opacity = s > 0.9 ? 1.0d : 0.5d;
        }

        return size;
    }

    public void NextItem()
    {
        if (SelectedIndex < items.Count - 1)
        {
            duration = SingleItemDuration;
            SelectedIndex++;
        }
    }

    public void PreviousItem()
    {
        if (SelectedIndex > 0)
        {
            duration = SingleItemDuration;
            SelectedIndex--;
        }
    }

    public void NextPage()
    {
        if (SelectedIndex == items.Count - 1)
            return;

        duration = PageDuration;
        int i = GetPageCount();
        if (SelectedIndex + i >= items.Count)
            SelectedIndex = items.Count - 1;
        else
            SelectedIndex += i;
    }

    public void PreviousPage()
    {
        if (SelectedIndex == 0)
            return;
        duration = PageDuration;
        int i = GetPageCount();
        if (SelectedIndex - i < 0)
            SelectedIndex = 0;
        else
            SelectedIndex -= i;
    }

    protected int GetPageCount()
    {
        if (itemsPresenter == null)
            return 1;

        double m = itemsPresenter.ActualWidth / 2;
        m -= k;
        if (l <= 0)
            return 1;

        // At least one item, otherwise a narrow control would page backwards or not at all.
        return Math.Max(1, (int)(m / l));
    }

    public void First()
    {
        if (items.Count == 0)
            return;
        duration = PageDuration;
        SelectedIndex = 0;
    }

    public void Last()
    {
        if (items.Count == 0)
            return;
        duration = PageDuration;
        SelectedIndex = items.Count - 1;
    }

    public void UpdatePositions()
    {
        LayoutChildren();
    }

    public void UpdatePositions(object value)
    {
        CoverFlowItem item = GetItemContainerForObject(value);
        if (item == null)
            return;

        int index = items.IndexOf(item);
        LayoutChild(item, index);
    }
}
