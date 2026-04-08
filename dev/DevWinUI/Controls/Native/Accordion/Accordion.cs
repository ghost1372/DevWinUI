namespace DevWinUI;

[TemplatePart(Name = nameof(PART_RectangleGeometry), Type = typeof(RectangleGeometry))]
public partial class Accordion : Control
{
    private const string PART_RectangleGeometry = "PART_RectangleGeometry";

    private RectangleGeometry _clip = null;
    
    public Accordion()
    {
        this.DefaultStyleKey = typeof(Accordion);
        this.SizeChanged += OnSizeChanged;
    }

    public int SelectedIndex
    {
        get { return (int)GetValue(SelectedIndexProperty); }
        set { SetValue(SelectedIndexProperty, value); }
    }

    public static readonly DependencyProperty SelectedIndexProperty =
        DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(Accordion), new PropertyMetadata(0));

    public object ItemsSource
    {
        get { return (object)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(Accordion), new PropertyMetadata(null));

    public DataTemplate ItemTemplate
    {
        get { return (DataTemplate)GetValue(ItemTemplateProperty); }
        set { SetValue(ItemTemplateProperty, value); }
    }

    public static readonly DependencyProperty ItemTemplateProperty =
        DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(Accordion), new PropertyMetadata(null));

    public DataTemplate HeaderTemplate
    {
        get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
        set { SetValue(HeaderTemplateProperty, value); }
    }

    public static readonly DependencyProperty HeaderTemplateProperty =
        DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(Accordion), new PropertyMetadata(null));

    protected override void OnApplyTemplate()
    {
        _clip = base.GetTemplateChild(PART_RectangleGeometry) as RectangleGeometry;

        base.OnApplyTemplate();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _clip.Rect = new Rect(new Point(), e.NewSize);
    }
}
