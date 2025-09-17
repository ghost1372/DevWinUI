using System.Diagnostics.CodeAnalysis;

namespace DevWinUI;
public partial class ElementGroup : ItemsControl
{
    private Dictionary<Control, long> _visibilityCallbacks = new();

    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(ElementGroup), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }
    private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ElementGroup)d;
        if (ctl != null)
        {
            ctl.UpdateOrientation();
        }
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ItemsPanelTemplate))]
    private void UpdateOrientation()
    {
        if (Orientation == Orientation.Horizontal)
        {
            ItemsPanel = Application.Current.Resources["ElementGroupHorizontalItemsPanel"] as ItemsPanelTemplate;
        }
        else
        {
            ItemsPanel = Application.Current.Resources["ElementGroupVerticalItemsPanel"] as ItemsPanelTemplate;
        }
        UpdateElements();
    }

    public ElementGroup()
    {
        this.DefaultStyleKey = typeof(ElementGroup);

        UpdateOrientation();
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        UpdateElements();
        Loaded -= OnLoaded;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateVisibilityEventHandlers();
        UpdateElements();
    }

    protected override void OnItemsChanged(object e)
    {
        base.OnItemsChanged(e);

        UpdateVisibilityEventHandlers();
        UpdateElements();
    }
    private void UpdateVisibilityEventHandlers()
    {
        if (ItemsPanelRoot is Panel panel)
        {
            foreach (var entry in _visibilityCallbacks)
            {
                entry.Key.UnregisterPropertyChangedCallback(VisibilityProperty, entry.Value);
            }
            _visibilityCallbacks.Clear();

            foreach (var child in panel.Children)
            {
                if (child is Control element)
                {
                    long token = element.RegisterPropertyChangedCallback(VisibilityProperty, OnChildVisibilityChanged);
                    _visibilityCallbacks[element] = token;
                }
            }
        }
    }
    private void OnChildVisibilityChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (dp == VisibilityProperty)
        {
            UpdateElements();
        }
    }
    private void UpdateElements()
    {
        if (ItemsPanelRoot is Panel panel)
        {
            var visibleChildren = panel.Children
                .OfType<Control>()
                .Where(c => c.Visibility == Visibility.Visible)
                .ToList();

            int totalItems = visibleChildren.Count;

            if (totalItems == 1)
            {
                if (visibleChildren[0] is Control element)
                {
                    element.CornerRadius = new CornerRadius(4);
                    element.BorderThickness = new Thickness(1);
                }
                return;
            }

            for (int i = 0; i < totalItems; i++)
            {
                if (visibleChildren[i] is Control element)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        SetStackPanelHorizontalItems(i, totalItems, element);
                    }
                    else
                    {
                        SetStackPanelVerticalItems(i, totalItems, element);
                    }
                }
            }
        }
    }

    private void SetStackPanelVerticalItems(int index, int totalItems, Control element)
    {
        if (index == 0)
        {
            element.CornerRadius = new CornerRadius(4, 4, 0, 0);
            element.BorderThickness = new Thickness(1, 1, 1, 0);
        }
        else if (index == totalItems - 1)
        {
            element.CornerRadius = new CornerRadius(0, 0, 4, 4);
            element.BorderThickness = new Thickness(1, 1, 1, 1);
        }
        else
        {
            element.CornerRadius = new CornerRadius(0);
            element.BorderThickness = new Thickness(1, 1, 1, 0);
        }
    }
    private void SetStackPanelHorizontalItems(int index, int totalItems, Control element)
    {
        if (index == 0)
        {
            element.CornerRadius = new CornerRadius(4, 0, 0, 4);
            element.BorderThickness = new Thickness(1, 1, 0, 1);
        }
        else if (index == totalItems - 1)
        {
            element.CornerRadius = new CornerRadius(0, 4, 4, 0);
            element.BorderThickness = new Thickness(1, 1, 1, 1);
        }
        else
        {
            element.CornerRadius = new CornerRadius(0);
            element.BorderThickness = new Thickness(1, 1, 0, 1);
        }
    }
}
