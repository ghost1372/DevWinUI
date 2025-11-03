namespace DevWinUI;

public partial class LoopingList
{
    public DataTemplate HeaderTemplate
    {
        get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
        set { SetValue(HeaderTemplateProperty, value); }
    }

    public static readonly DependencyProperty HeaderTemplateProperty =
        DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(LoopingList), new PropertyMetadata(null));

    public object Header
    {
        get { return (object)GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(object), typeof(LoopingList), new PropertyMetadata(null));

    public IList<string> PrimaryItems
    {
        get { return (IList<string>)GetValue(PrimaryItemsProperty); }
        set { SetValue(PrimaryItemsProperty, value); }
    }

    public static readonly DependencyProperty PrimaryItemsProperty =
        DependencyProperty.Register(nameof(PrimaryItems), typeof(IList<string>), typeof(LoopingList), new PropertyMetadata(null));

    public IList<string> SecondaryItems
    {
        get { return (IList<string>)GetValue(SecondaryItemsProperty); }
        set { SetValue(SecondaryItemsProperty, value); }
    }

    public static readonly DependencyProperty SecondaryItemsProperty =
        DependencyProperty.Register(nameof(SecondaryItems), typeof(IList<string>), typeof(LoopingList), new PropertyMetadata(null));

    public IList<string> TertiaryItems
    {
        get { return (IList<string>)GetValue(TertiaryItemsProperty); }
        set { SetValue(TertiaryItemsProperty, value); }
    }

    public static readonly DependencyProperty TertiaryItemsProperty =
        DependencyProperty.Register(nameof(TertiaryItems), typeof(IList<string>), typeof(LoopingList), new PropertyMetadata(null));

    public int PrimarySelectedIndex
    {
        get { return (int)GetValue(PrimarySelectedIndexProperty); }
        set { SetValue(PrimarySelectedIndexProperty, value); }
    }

    public static readonly DependencyProperty PrimarySelectedIndexProperty =
        DependencyProperty.Register(nameof(PrimarySelectedIndex), typeof(int), typeof(LoopingList), new PropertyMetadata(-1));

    public int SecondarySelectedIndex
    {
        get { return (int)GetValue(SecondarySelectedIndexProperty); }
        set { SetValue(SecondarySelectedIndexProperty, value); }
    }

    public static readonly DependencyProperty SecondarySelectedIndexProperty =
        DependencyProperty.Register(nameof(SecondarySelectedIndex), typeof(int), typeof(LoopingList), new PropertyMetadata(-1));

    public int TertiarySelectedIndex
    {
        get { return (int)GetValue(TertiarySelectedIndexProperty); }
        set { SetValue(TertiarySelectedIndexProperty, value); }
    }

    public static readonly DependencyProperty TertiarySelectedIndexProperty =
        DependencyProperty.Register(nameof(TertiarySelectedIndex), typeof(int), typeof(LoopingList), new PropertyMetadata(-1));

    public string PrimarySelectedItem
    {
        get { return (string)GetValue(PrimarySelectedItemProperty); }
        set { SetValue(PrimarySelectedItemProperty, value); }
    }

    public static readonly DependencyProperty PrimarySelectedItemProperty =
        DependencyProperty.Register(nameof(PrimarySelectedItem), typeof(string), typeof(LoopingList), new PropertyMetadata(null));

    public string SecondarySelectedItem
    {
        get { return (string)GetValue(SecondarySelectedItemProperty); }
        set { SetValue(SecondarySelectedItemProperty, value); }
    }

    public static readonly DependencyProperty SecondarySelectedItemProperty =
        DependencyProperty.Register(nameof(SecondarySelectedItem), typeof(string), typeof(LoopingList), new PropertyMetadata(null));

    public string TertiarySelectedItem
    {
        get { return (string)GetValue(TertiarySelectedItemProperty); }
        set { SetValue(TertiarySelectedItemProperty, value); }
    }

    public static readonly DependencyProperty TertiarySelectedItemProperty =
        DependencyProperty.Register(nameof(TertiarySelectedItem), typeof(string), typeof(LoopingList), new PropertyMetadata(null));

    public string PrimaryPlaceholderText
    {
        get { return (string)GetValue(PrimaryPlaceholderTextProperty); }
        set { SetValue(PrimaryPlaceholderTextProperty, value); }
    }

    public static readonly DependencyProperty PrimaryPlaceholderTextProperty =
        DependencyProperty.Register(nameof(PrimaryPlaceholderText), typeof(string), typeof(LoopingList), new PropertyMetadata(null, OnUpdatePlaceholderText));

    private static void OnUpdatePlaceholderText(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (LoopingList)d;
        if (ctl != null)
        {
            ctl.UpdatePlaceholderText();
        }
    }

    public string SecondaryPlaceholderText
    {
        get { return (string)GetValue(SecondaryPlaceholderTextProperty); }
        set { SetValue(SecondaryPlaceholderTextProperty, value); }
    }

    public static readonly DependencyProperty SecondaryPlaceholderTextProperty =
        DependencyProperty.Register(nameof(SecondaryPlaceholderText), typeof(string), typeof(LoopingList), new PropertyMetadata(null, OnUpdatePlaceholderText));

    public string TertiaryPlaceholderText
    {
        get { return (string)GetValue(TertiaryPlaceholderTextProperty); }
        set { SetValue(TertiaryPlaceholderTextProperty, value); }
    }

    public static readonly DependencyProperty TertiaryPlaceholderTextProperty =
        DependencyProperty.Register(nameof(TertiaryPlaceholderText), typeof(string), typeof(LoopingList), new PropertyMetadata(null, OnUpdatePlaceholderText));
}
