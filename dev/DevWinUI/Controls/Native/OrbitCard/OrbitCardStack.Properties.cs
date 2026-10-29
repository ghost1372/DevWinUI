namespace DevWinUI;

public sealed partial class OrbitCardStack
{
    public IReadOnlyList<OrbitCardItem> Items
    {
        get { return (IReadOnlyList<OrbitCardItem>)GetValue(ItemsProperty); }
        set { SetValue(ItemsProperty, value); }
    }

    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register(nameof(Items), typeof(IReadOnlyList<OrbitCardItem>), typeof(OrbitCardStack), new PropertyMetadata(Array.Empty<OrbitCardItem>(), (d, _) => ((OrbitCardStack)d).Rebuild()));

    public OrbitCardLayout Layout
    {
        get { return (OrbitCardLayout)GetValue(LayoutProperty); }
        set { SetValue(LayoutProperty, value); }
    }

    public static readonly DependencyProperty LayoutProperty =
        DependencyProperty.Register(nameof(Layout), typeof(OrbitCardLayout), typeof(OrbitCardStack), new PropertyMetadata(OrbitCardLayout.Arc, (d, _) => ((OrbitCardStack)d).Relayout(true)));

    public OrbitCardSize Size
    {
        get { return (OrbitCardSize)GetValue(SizeProperty); }
        set { SetValue(SizeProperty, value); }
    }

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(nameof(Size), typeof(OrbitCardSize), typeof(OrbitCardStack), new PropertyMetadata(OrbitCardSize.Medium, (d, _) => ((OrbitCardStack)d).Rebuild()));

    public double Spread
    {
        get { return (double)GetValue(SpreadProperty); }
        set { SetValue(SpreadProperty, value); }
    }

    public static readonly DependencyProperty SpreadProperty =
        DependencyProperty.Register(nameof(Spread), typeof(double), typeof(OrbitCardStack), new PropertyMetadata(168.0, (d, _) => ((OrbitCardStack)d).Relayout(true)));

    public double Lift
    {
        get { return (double)GetValue(LiftProperty); }
        set { SetValue(LiftProperty, value); }
    }

    public static readonly DependencyProperty LiftProperty =
        DependencyProperty.Register(nameof(Lift), typeof(double), typeof(OrbitCardStack), new PropertyMetadata(34.0, (d, _) => ((OrbitCardStack)d).Relayout(true)));

    public int? Value
    {
        get { return (int?)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(int?), typeof(OrbitCardStack), new PropertyMetadata(null, (d, _) => ((OrbitCardStack)d).Relayout(true)));

    public int DefaultValue
    {
        get { return (int)GetValue(DefaultValueProperty); }
        set { SetValue(DefaultValueProperty, value); }
    }

    public static readonly DependencyProperty DefaultValueProperty =
        DependencyProperty.Register(nameof(DefaultValue), typeof(int), typeof(OrbitCardStack), new PropertyMetadata(0));

    public bool? Open
    {
        get { return (bool?)GetValue(OpenProperty); }
        set { SetValue(OpenProperty, value); }
    }

    public static readonly DependencyProperty OpenProperty =
        DependencyProperty.Register(nameof(Open), typeof(bool?), typeof(OrbitCardStack), new PropertyMetadata(null, (d, _) => ((OrbitCardStack)d).OnOpenPropertyChanged()));

    public bool DefaultOpen
    {
        get { return (bool)GetValue(DefaultOpenProperty); }
        set { SetValue(DefaultOpenProperty, value); }
    }

    public static readonly DependencyProperty DefaultOpenProperty =
        DependencyProperty.Register(nameof(DefaultOpen), typeof(bool), typeof(OrbitCardStack), new PropertyMetadata(false));

    public bool IsAnimationEnabled
    {
        get { return (bool)GetValue(IsAnimationEnabledProperty); }
        set { SetValue(IsAnimationEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsAnimationEnabledProperty =
        DependencyProperty.Register(nameof(IsAnimationEnabled), typeof(bool), typeof(OrbitCardStack), new PropertyMetadata(true));

    public OrbitCardLabels Labels
    {
        get { return (OrbitCardLabels)GetValue(LabelsProperty); }
        set { SetValue(LabelsProperty, value); }
    }

    public static readonly DependencyProperty LabelsProperty =
        DependencyProperty.Register(nameof(Labels), typeof(OrbitCardLabels), typeof(OrbitCardStack), new PropertyMetadata(new OrbitCardLabels(), (d, _) => ((OrbitCardStack)d).Rebuild()));

    public Func<OrbitCardItem, int, UIElement>? ImageFactory { get; set; }
}
