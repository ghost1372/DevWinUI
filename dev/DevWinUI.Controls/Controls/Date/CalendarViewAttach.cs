namespace DevWinUI;
internal partial class CalendarViewAttach
{
    public static readonly DependencyProperty ShowBorderProperty =
    DependencyProperty.RegisterAttached(
        "ShowBorder",
        typeof(bool),
        typeof(CalendarViewAttach),
        new PropertyMetadata(false, OnShowBorderChanged));

    public static bool GetShowBorder(DependencyObject obj) =>
        (bool)obj.GetValue(ShowBorderProperty);

    public static void SetShowBorder(DependencyObject obj, bool value) =>
        obj.SetValue(ShowBorderProperty, value);

    private static void OnShowBorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CalendarView calendarView && calendarView != null)
        {
            FrameworkElement headerBorder = DependencyObjectEx.FindDescendant(calendarView, "PART_HeaderBorder");
            FrameworkElement topBorder = DependencyObjectEx.FindDescendant(calendarView, "PART_TopBorder");

            var value = (bool)e.NewValue;
            calendarView.Loaded -= CalendarView_Loaded;
            calendarView.Loaded += CalendarView_Loaded;


            void CalendarView_Loaded(object sender, RoutedEventArgs e)
            {
                if (headerBorder == null && topBorder == null)
                {
                    headerBorder = DependencyObjectEx.FindDescendant(calendarView, "PART_HeaderBorder");
                    topBorder = DependencyObjectEx.FindDescendant(calendarView, "PART_TopBorder");
                    UpdateBorders();
                }
            }

            void UpdateBorders()
            {
                if (headerBorder != null)
                {
                    headerBorder.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                }
                if (topBorder != null)
                {
                    if (value)
                    {
                        topBorder.Height = 0;
                    }
                    else
                    {
                        topBorder.Height = 1;
                    }
                }
            }
            UpdateBorders();
        }
    }
}
