using System.Collections;

namespace DevWinUI;
public partial class AccordionPanel
{
    public int SelectedIndex
    {
        get { return (int)GetValue(SelectedIndexProperty); }
        set { SetValue(SelectedIndexProperty, value); }
    }
    public static readonly DependencyProperty SelectedIndexProperty =
       DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(AccordionPanel), new PropertyMetadata(0, SelectedIndexChanged));
    private static void SelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as AccordionPanel;
        control.InvalidateMeasure();
    }
   
    public object ItemsSource
    {
        get { return (object)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(AccordionPanel), new PropertyMetadata(null, ItemsSourceChanged));
    private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as AccordionPanel;
        control.SetItemsSource(e.NewValue);
    }
    public DataTemplate ItemTemplate
    {
        get { return (DataTemplate)GetValue(ItemTemplateProperty); }
        set { SetValue(ItemTemplateProperty, value); }
    }

    public static readonly DependencyProperty ItemTemplateProperty =
        DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(AccordionPanel), new PropertyMetadata(null, ItemTemplateChanged));
    private static void ItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as AccordionPanel;
        control.SetItemTemplate(e.NewValue as DataTemplate);
    }
    public DataTemplate HeaderTemplate
    {
        get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
        set { SetValue(HeaderTemplateProperty, value); }
    }

    public static readonly DependencyProperty HeaderTemplateProperty =
        DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(AccordionPanel), new PropertyMetadata(null, HeaderTemplateChanged));

    private static void HeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as AccordionPanel;
        control.SetHeaderTemplate(e.NewValue as DataTemplate);
    }

    private static int MaxTabs
    {
        get { return 4; }
    }

    private void SetItemsSource(object items)
    {
        base.Children.Clear();

        if (items is IEnumerable enumerable)
        {
            // Convert to a list to support Count and Reverse
            var itemList = enumerable.Cast<object>().ToList();
            int n = itemList.Count - 1;

            foreach (var item in itemList.AsEnumerable().Reverse())
            {
                var control = new AccordionItem
                {
                    Index = n--,
                    Content = item,
                    ContentTemplate = this.ItemTemplate,
                    Header = item,
                    HeaderTemplate = this.HeaderTemplate
                };
                base.Children.Add(control);
            }
        }
    }
    private void SetItemTemplate(DataTemplate itemTemplate)
    {
        foreach (ContentControl item in base.Children)
        {
            item.ContentTemplate = itemTemplate;
        }
    }

    private void SetHeaderTemplate(DataTemplate headerTemplate)
    {
        foreach (AccordionItem item in base.Children)
        {
            item.HeaderTemplate = headerTemplate;
        }
    }
}
