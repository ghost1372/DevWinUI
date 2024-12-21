namespace DevWinUI;
public partial class CalendarWithClock
{
    public ClockMode ClockMode
    {
        get { return (ClockMode)GetValue(ClockModeProperty); }
        set { SetValue(ClockModeProperty, value); }
    }

    public static readonly DependencyProperty ClockModeProperty =
        DependencyProperty.Register(nameof(ClockMode), typeof(ClockMode), typeof(CalendarWithClock), new PropertyMetadata(ClockMode.AnalogClock, OnClockModeChanged));
    private static void OnClockModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CalendarWithClock control)
        {
            control.UpdateTemplate();
        }
    }
    public TimeSpan? SelectedTime
    {
        get { return (TimeSpan?)GetValue(SelectedTimeProperty); }
        set { SetValue(SelectedTimeProperty, value); }
    }

    public static readonly DependencyProperty SelectedTimeProperty =
        DependencyProperty.Register(nameof(SelectedTime), typeof(TimeSpan?), typeof(CalendarWithClock), new PropertyMetadata(TimeNow, OnSelectedTimeChanged));

    private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CalendarWithClock ctl && !ctl.isUpdating)
        {
            ctl.UpdateDateTimeOffset();

            // Sync TimePicker's SelectedTime with the new value
            if (ctl.timePicker != null)
            {
                ctl.timePicker.SelectedTime = ctl.SelectedTime;
            }
        }
    }

    public DateTimeOffset SelectedDate
    {
        get { return (DateTimeOffset)GetValue(SelectedDateProperty); }
        set { SetValue(SelectedDateProperty, value); }
    }
    public static readonly DependencyProperty SelectedDateProperty =
        DependencyProperty.Register(nameof(SelectedDate), typeof(DateTimeOffset), typeof(CalendarWithClock), new PropertyMetadata(DateTimeOffset.Now, OnSelectedDateChanged));
    private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CalendarWithClock ctl && !ctl.isUpdating)
        {
            ctl.UpdateCalendarView();
        }
    }

    public TimePickerDisplayMode TimePickerDisplayMode
    {
        get { return (TimePickerDisplayMode)GetValue(TimePickerDisplayModeProperty); }
        set { SetValue(TimePickerDisplayModeProperty, value); }
    }

    public static readonly DependencyProperty TimePickerDisplayModeProperty =
        DependencyProperty.Register(nameof(TimePickerDisplayMode), typeof(TimePickerDisplayMode), typeof(CalendarWithClock), new PropertyMetadata(TimePickerDisplayMode.Right, OnTimePickerDisplayModeChanged));

    private static void OnTimePickerDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CalendarWithClock)d;
        if (ctl != null)
        {
            ctl.UpdateGridRowsAndColumns((TimePickerDisplayMode)e.NewValue);
        }
    }

    public bool ShowAccentBorderOnHeader
    {
        get { return (bool)GetValue(ShowAccentBorderOnHeaderProperty); }
        set { SetValue(ShowAccentBorderOnHeaderProperty, value); }
    }

    public static readonly DependencyProperty ShowAccentBorderOnHeaderProperty =
        DependencyProperty.Register(nameof(ShowAccentBorderOnHeader), typeof(bool), typeof(CalendarWithClock), new PropertyMetadata(true, OnShowAccentBorderOnHeaderChanged));

    private static void OnShowAccentBorderOnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CalendarWithClock)d;
        if (ctl != null)
        {
            ctl.OnShowAccentBorderOnHeader((bool)e.NewValue);
        }
    }
}
