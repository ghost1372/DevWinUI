namespace DevWinUI;
internal partial class TextBlockTemplateSelector : DataTemplateSelector
{
    public DataTemplate TextBlockTemplate { get; set; }
    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item == null) return null;

        if (item is string)
        {
            return TextBlockTemplate;
        }
        else
        {
            return base.SelectTemplateCore(item, container);
        }
    }
}
