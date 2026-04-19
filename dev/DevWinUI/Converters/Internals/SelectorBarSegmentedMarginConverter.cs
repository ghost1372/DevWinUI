namespace DevWinUI;
internal partial class SelectorBarSegmentedMarginConverter : DependencyObject, IValueConverter
{
    /// <summary>
    /// Identifies the <see cref="LeftItemMargin"/> property.
    /// </summary>
    public static readonly DependencyProperty LeftItemMarginProperty =
        DependencyProperty.Register(nameof(LeftItemMargin), typeof(Thickness), typeof(SelectorBarSegmentedMarginConverter), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the margin of the first item
    /// </summary>
    public Thickness LeftItemMargin
    {
        get { return (Thickness)GetValue(LeftItemMarginProperty); }
        set { SetValue(LeftItemMarginProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MiddleItemMargin"/> property.
    /// </summary>
    public static readonly DependencyProperty MiddleItemMarginProperty =
        DependencyProperty.Register(nameof(MiddleItemMargin), typeof(Thickness), typeof(SelectorBarSegmentedMarginConverter), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the margin of any middle item
    /// </summary>
    public Thickness MiddleItemMargin
    {
        get { return (Thickness)GetValue(MiddleItemMarginProperty); }
        set { SetValue(MiddleItemMarginProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="RightItemMargin"/> property.
    /// </summary>
    public static readonly DependencyProperty RightItemMarginProperty =
        DependencyProperty.Register(nameof(RightItemMargin), typeof(Thickness), typeof(SelectorBarSegmentedMarginConverter), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the margin of the last item
    /// </summary>
    public Thickness RightItemMargin
    {
        get { return (Thickness)GetValue(RightItemMarginProperty); }
        set { SetValue(RightItemMarginProperty, value); }
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var segmentedItem = value as SelectorBarItem;
        if (segmentedItem != null)
        {
            ItemsRepeater parent = segmentedItem.Parent as ItemsRepeater;

            if (parent != null)
            {
                IEnumerable<object> itemsSource = (IEnumerable<object>)parent.ItemsSource;

                var index = parent.GetElementIndex(segmentedItem);
                if (index == 0)
                {
                    return LeftItemMargin;
                }
                else if (index == itemsSource.Count() - 1)
                {
                    return RightItemMargin;
                }
                else
                {
                    return MiddleItemMargin;
                }
            }
        }
        return new Thickness(3);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
}
