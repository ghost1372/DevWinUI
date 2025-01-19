namespace DevWinUI;

[TemplatePart(Name = PART_CalendarView, Type = typeof(CalendarView))]
[TemplatePart(Name = PART_TimePicker, Type = typeof(TimePicker))]
[TemplatePart(Name = PART_Clock, Type = typeof(Clock))]
[TemplatePart(Name = PART_Root, Type = typeof(Grid))]
public partial class CalendarWithClock : DateTimeBase
{
    public ControlTemplate? TimePickerTemplate { get; set; }
    public ControlTemplate? AnalogClockTemplate { get; set; }

    private const string PART_CalendarView = "PART_CalendarView";
    private const string PART_TimePicker = "PART_TimePicker";
    private const string PART_Clock = "PART_Clock";
    private const string PART_Root = "PART_Root";

    public event EventHandler<DateTimeOffset> SelectedTimeChanged;

    private CalendarView calendarView;
    private TimePicker timePicker;
    private Clock clock;
    private Grid rootGrid;
    private bool isUpdating;
    public string SelectedDateFormatted
    {
        get
        {
            //return SelectedDateTime.ToString("dd/MM/yyyy");
            return SelectedDateTime.ToString("d");
        }
    }
    public string SelectedDateTimeString
    {
        get
        {
            //return $"{SelectedDateFormatted} - {SelectedTime?.ToString(@"hh\:mm\:ss")}";
            return $"{SelectedDateFormatted} - {SelectedDateTime.TimeOfDay.ToString(@"hh\:mm\:ss")}";

        }
    }

    public CalendarWithClock()
    {
        this.DefaultStyleKey = typeof(CalendarWithClock);

        if (Application.Current.Resources["CalendarWithTimePickerTemplate"] is ControlTemplate timePickerTemplate)
            TimePickerTemplate = timePickerTemplate;

        if (Application.Current.Resources["CalendarWithAnalogClockTemplate"] is ControlTemplate analogClockTemplate)
            AnalogClockTemplate = analogClockTemplate;
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        UpdateTemplate();

        rootGrid = GetTemplateChild(PART_Root) as Grid;
        timePicker = GetTemplateChild(PART_TimePicker) as TimePicker;
        clock = GetTemplateChild(PART_Clock) as Clock;
        calendarView = GetTemplateChild(PART_CalendarView) as CalendarView;
        if (calendarView != null)
        {
            calendarView.SelectedDatesChanged -= OnCalendarViewSelectedDatesChanged;
            calendarView.SelectedDatesChanged += OnCalendarViewSelectedDatesChanged;
        }

        if (timePicker != null)
        {
            timePicker.SelectedTimeChanged -= OnTimePickerSelectedTimeChanged;
            timePicker.SelectedTimeChanged += OnTimePickerSelectedTimeChanged;
            timePicker.SelectedTime = SelectedTime;
        }

        if (clock != null)
        {
            if (SelectedDateTime != default)
            {
                clock.SelectedTime = SelectedDateTime.DateTime;
            }

            clock.SelectedTimeChanged -= OnClockSelectedTimeChanged;
            clock.SelectedTimeChanged += OnClockSelectedTimeChanged;
        }

        UpdateCalendarView();
        UpdateGridRowsAndColumns(TimePickerDisplayMode);
        OnShowAccentBorderOnHeader(ShowAccentBorderOnHeader);
    }

    private void OnClockSelectedTimeChanged(object sender, DateTime e)
    {
        if (!isUpdating && clock.SelectedTime is DateTime selectedTime)
        {
            try
            {
                isUpdating = true;

                SelectedTime = new TimeSpan(selectedTime.Hour, selectedTime.Minute, selectedTime.Second);

                SelectedDateTime = new DateTimeOffset(
                    SelectedDateTime.Year, SelectedDateTime.Month, SelectedDateTime.Day,//original
                    selectedTime.Hour, selectedTime.Minute, selectedTime.Second,  //just updated.
                    SelectedDateTime.Offset); //original

                SelectedTimeChanged?.Invoke(this, SelectedDateTime);
            }
            finally
            {
                isUpdating = false;
            }
        }
    }

    private void OnCalendarViewSelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
    {
        if (!isUpdating && calendarView.SelectedDates.FirstOrDefault() is DateTimeOffset selectedDate)
        {
            try
            {
                isUpdating = true;
                SelectedDateTime = new DateTimeOffset(
                    selectedDate.Year, selectedDate.Month, selectedDate.Day,
                    SelectedTime?.Hours ?? 0, SelectedTime?.Minutes ?? 0, SelectedTime?.Seconds ?? 0,
                    selectedDate.Offset);

                SelectedTimeChanged?.Invoke(this, SelectedDateTime);
            }
            finally
            {
                isUpdating = false;
            }
        }
    }

    private void OnTimePickerSelectedTimeChanged(TimePicker sender, TimePickerSelectedValueChangedEventArgs args)
    {
        if (!isUpdating && timePicker.SelectedTime is TimeSpan selectedTime)
        {
            try
            {
                isUpdating = true;

                SelectedTime = selectedTime;

                SelectedDateTime = new DateTimeOffset(
                    SelectedDateTime.Year, SelectedDateTime.Month, SelectedDateTime.Day,
                    selectedTime.Hours, selectedTime.Minutes, selectedTime.Seconds,
                    SelectedDateTime.Offset);

                SelectedTimeChanged?.Invoke(this, SelectedDateTime);
            }
            finally
            {
                isUpdating = false;
            }
        }
    }
    private void UpdateCalendarView()
    {
        if (calendarView != null)
        {
            try
            {
                isUpdating = true;
                calendarView.SelectedDates.Clear();
                calendarView.SelectedDates.Add(SelectedDateTime);
                calendarView.SetDisplayDate(SelectedDateTime);
            }
            finally
            {
                isUpdating = false;
            }
        }
    }
    private void UpdateDateTimeOffset()
    {
        if (SelectedTime.HasValue)
        {
            try
            {
                isUpdating = true;
                SelectedDateTime = new DateTimeOffset(
                    SelectedDateTime.Year, SelectedDateTime.Month, SelectedDateTime.Day,
                    SelectedTime.Value.Hours, SelectedTime.Value.Minutes, SelectedTime.Value.Seconds,
                    SelectedDateTime.Offset);
            }
            finally
            {
                isUpdating = false;
            }
        }
    }

    private void UpdateGridRowsAndColumns(TimePickerDisplayMode displayMode)
    {
        if (timePicker == null)
        {
            return;
        }

        if (rootGrid != null)
        {
            switch (displayMode)
            {
                case TimePickerDisplayMode.Top:
                    Grid.SetColumn(timePicker, 0);
                    Grid.SetColumnSpan(timePicker, 2);
                    Grid.SetRow(timePicker, 0);
                    Grid.SetRowSpan(timePicker, 1);

                    Grid.SetColumn(calendarView, 0);
                    Grid.SetColumnSpan(calendarView, 2);
                    Grid.SetRow(calendarView, 1);
                    Grid.SetRowSpan(calendarView, 1);

                    rootGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                    rootGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
                    rootGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Auto);
                    rootGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);

                    calendarView.CornerRadius = new CornerRadius(0, 0, 4, 4);
                    break;
                case TimePickerDisplayMode.Bottom:
                    Grid.SetColumn(timePicker, 0);
                    Grid.SetColumnSpan(timePicker, 2);
                    Grid.SetRow(timePicker, 1);
                    Grid.SetRowSpan(timePicker, 1);

                    Grid.SetColumn(calendarView, 0);
                    Grid.SetColumnSpan(calendarView, 2);
                    Grid.SetRow(calendarView, 0);
                    Grid.SetRowSpan(calendarView, 1);

                    rootGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                    rootGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
                    rootGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                    rootGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Auto);

                    calendarView.CornerRadius = new CornerRadius(4, 4, 0, 0);

                    break;
                case TimePickerDisplayMode.Right:
                    Grid.SetColumn(timePicker, 1);
                    Grid.SetColumnSpan(timePicker, 1);
                    Grid.SetRow(timePicker, 0);
                    Grid.SetRowSpan(timePicker, 2);

                    Grid.SetColumn(calendarView, 0);
                    Grid.SetColumnSpan(calendarView, 1);
                    Grid.SetRow(calendarView, 0);
                    Grid.SetRowSpan(calendarView, 2);

                    rootGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                    rootGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
                    rootGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                    rootGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Auto);

                    calendarView.CornerRadius = new CornerRadius(4, 0, 0, 4);

                    break;
                case TimePickerDisplayMode.Left:
                    Grid.SetColumn(timePicker, 0);
                    Grid.SetColumnSpan(timePicker, 1);
                    Grid.SetRow(timePicker, 0);
                    Grid.SetRowSpan(timePicker, 2);

                    Grid.SetColumn(calendarView, 1);
                    Grid.SetColumnSpan(calendarView, 1);
                    Grid.SetRow(calendarView, 0);
                    Grid.SetRowSpan(calendarView, 2);

                    rootGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
                    rootGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                    rootGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                    rootGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Auto);
                    calendarView.CornerRadius = new CornerRadius(0, 4, 4, 0);

                    break;
            }
        }
    }
    private void UpdateTemplate()
    {
        // Dynamically apply the correct ControlTemplate based on ClockMode
        Template = ClockMode switch
        {
            ClockMode.TimePicker => TimePickerTemplate,
            ClockMode.AnalogClock => AnalogClockTemplate,
            _ => Template
        };
    }

    private void OnShowAccentBorderOnHeader(bool showBorder)
    {
        if (clock == null || calendarView == null)
        {
            return;
        }

        CalendarViewAttach.SetShowBorder(calendarView, showBorder);

        if (showBorder)
        {
            clock.TitleBorderCornerRadius = new CornerRadius(0, 8, 8, 0);
            clock.HeaderMargin = new Thickness(0, 5, 4, 4);
            calendarView.BorderThickness = new Thickness(1, 1, 0, 1);
        }
        else
        {
            clock.TitleBorderCornerRadius = new CornerRadius(8);
            clock.HeaderMargin = new Thickness(4);
            calendarView.BorderThickness = new Thickness(1);
        }
    }

    public Clock GetClock()
    {
        return clock;
    }

    public TimePicker GetTimePicker()
    {
        return timePicker;
    }

    public CalendarView GetCalendarView()
    {
        return calendarView;
    }
}
