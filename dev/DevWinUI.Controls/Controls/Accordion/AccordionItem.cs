namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Header), Type = typeof(ContentControl))]
public partial class AccordionItem : ContentControl
{
    private const string PART_Header = "PART_Header";
    private ContentControl _header = null;
    public int Index { get; set; }
    public bool IsUp { get; set; }
    public double HeaderHeight
    {
        get
        {
            if (_header != null)
            {
                return _header.DesiredSize.Height;
            }
            return 0.0;
        }
    }
    public object Header
    {
        get { return (object)GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(object), typeof(AccordionItem), new PropertyMetadata(null));
    public DataTemplate HeaderTemplate
    {
        get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
        set { SetValue(HeaderTemplateProperty, value); }
    }

    public static readonly DependencyProperty HeaderTemplateProperty =
        DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(AccordionItem), new PropertyMetadata(null));

    public AccordionItem()
    {
        this.DefaultStyleKey = typeof(AccordionItem);
    }

    protected override void OnApplyTemplate()
    {
        _header = GetTemplateChild(PART_Header) as ContentControl;

        base.OnApplyTemplate();
    }
}
