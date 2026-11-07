using System.Diagnostics.CodeAnalysis;

namespace DevWinUI;

public partial class ElementGroup : ItemsControl
{
    private const double ElementCornerRadius = 4;

    private readonly Dictionary<Control, long> _visibilityCallbacks = new();

    public Orientation Orientation
    {
        get { return (Orientation)GetValue(OrientationProperty); }
        set { SetValue(OrientationProperty, value); }
    }

    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(ElementGroup), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

    private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ElementGroup)d).UpdateOrientation();
    }

    public ElementGroup()
    {
        DefaultStyleKey = typeof(ElementGroup);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        UpdateOrientation();
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ItemsPanelTemplate))]
    private void UpdateOrientation()
    {
        var key = Orientation == Orientation.Horizontal
            ? "ElementGroupHorizontalItemsPanel"
            : "ElementGroupVerticalItemsPanel";

        ItemsPanel = Application.Current.Resources[key] as ItemsPanelTemplate;

        if (ItemsPanelRoot != null)
        {
            LayoutUpdated -= OnPanelLayoutUpdated;
            LayoutUpdated += OnPanelLayoutUpdated;
        }

        UpdateElements();
    }

    private void OnPanelLayoutUpdated(object sender, object e)
    {
        LayoutUpdated -= OnPanelLayoutUpdated;

        UpdateVisibilityEventHandlers();
        UpdateElements();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        UpdateElements();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateVisibilityEventHandlers();
        UpdateElements();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        ClearVisibilityEventHandlers();
    }

    protected override void OnItemsChanged(object e)
    {
        base.OnItemsChanged(e);

        UpdateVisibilityEventHandlers();
        UpdateElements();
    }

    private void ClearVisibilityEventHandlers()
    {
        foreach (var entry in _visibilityCallbacks)
        {
            entry.Key.UnregisterPropertyChangedCallback(VisibilityProperty, entry.Value);
        }

        _visibilityCallbacks.Clear();
    }

    private void UpdateVisibilityEventHandlers()
    {
        if (ItemsPanelRoot is not Panel panel)
            return;

        ClearVisibilityEventHandlers();

        foreach (var child in panel.Children)
        {
            if (child is Control element)
            {
                _visibilityCallbacks[element] = element.RegisterPropertyChangedCallback(VisibilityProperty, OnChildVisibilityChanged);
            }
        }
    }

    private void OnChildVisibilityChanged(DependencyObject sender, DependencyProperty dp)
    {
        UpdateElements();
    }

    private void UpdateElements()
    {
        if (ItemsPanelRoot is not Panel panel)
            return;

        int totalItems = 0;
        foreach (var child in panel.Children)
        {
            if (child is Control { Visibility: Visibility.Visible })
            {
                totalItems++;
            }
        }

        int index = 0;
        foreach (var child in panel.Children)
        {
            if (child is Control { Visibility: Visibility.Visible } element)
            {
                ApplyElementShape(element, index++, totalItems);
            }
        }
    }

    private void ApplyElementShape(Control element, int index, int totalItems)
    {
        if (totalItems == 1)
        {
            element.CornerRadius = new CornerRadius(ElementCornerRadius);
            element.BorderThickness = new Thickness(1);
            return;
        }

        double first = index == 0 ? ElementCornerRadius : 0;
        double last = index == totalItems - 1 ? ElementCornerRadius : 0;
        bool isLast = last > 0;

        if (Orientation == Orientation.Horizontal)
        {
            element.CornerRadius = new CornerRadius(first, last, last, first);
            element.BorderThickness = new Thickness(1, 1, isLast ? 1 : 0, 1);
        }
        else
        {
            element.CornerRadius = new CornerRadius(first, first, last, last);
            element.BorderThickness = new Thickness(1, 1, 1, isLast ? 1 : 0);
        }
    }
}
