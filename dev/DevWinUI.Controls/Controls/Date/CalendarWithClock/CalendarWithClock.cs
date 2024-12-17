namespace DevWinUI;
[TemplatePart(Name = nameof(PART_CalendarView), Type = typeof(CalendarView))]
[TemplatePart(Name = nameof(PART_TimePicker), Type = typeof(TimePicker))]
[TemplatePart(Name = nameof(PART_Root), Type = typeof(Grid))]
public partial class CalendarWithClock : DateTimeBase
{
    private readonly string PART_CalendarView = "PART_CalendarView";
    private readonly string PART_TimePicker = "PART_TimePicker";
    private readonly string PART_Root = "PART_Root";
    private CalendarView calendarView;
    private TimePicker timePicker;
    private Grid rootGrid;
    private bool isUpdating;
    public string SelectedDateFormatted
    {
        get
        {
            return SelectedDate.ToString("dd/MM/yyyy");
        }
    }
    public string SelectedDateTime
    {
        get
        {
            return $"{SelectedDateFormatted} - {SelectedTime?.ToString(@"hh\:mm\:ss")}";
        }
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        rootGrid = GetTemplateChild(nameof(PART_Root)) as Grid;
        timePicker = GetTemplateChild(nameof(PART_TimePicker)) as TimePicker;
        calendarView = GetTemplateChild(nameof(PART_CalendarView)) as CalendarView;
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
        UpdateCalendarView();
        UpdateGridRowsAndColumns(TimePickerDisplayMode);
    }

    private void OnCalendarViewSelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
    {
        if (!isUpdating && calendarView.SelectedDates.FirstOrDefault() is DateTimeOffset selectedDate)
        {
            try
            {
                isUpdating = true;
                SelectedDate = new DateTimeOffset(
                    selectedDate.Year, selectedDate.Month, selectedDate.Day,
                    SelectedTime?.Hours ?? 0, SelectedTime?.Minutes ?? 0, SelectedTime?.Seconds ?? 0,
                    selectedDate.Offset);
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

                // Update SelectedTime
                SelectedTime = selectedTime;

                // Update SelectedDate with new time while preserving the date and time zone offset
                SelectedDate = new DateTimeOffset(
                    SelectedDate.Year, SelectedDate.Month, SelectedDate.Day,
                    selectedTime.Hours, selectedTime.Minutes, selectedTime.Seconds,
                    SelectedDate.Offset);
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
                calendarView.SelectedDates.Add(SelectedDate);
                calendarView.SetDisplayDate(SelectedDate);
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
                SelectedDate = new DateTimeOffset(
                    SelectedDate.Year, SelectedDate.Month, SelectedDate.Day,
                    SelectedTime.Value.Hours, SelectedTime.Value.Minutes, SelectedTime.Value.Seconds,
                    SelectedDate.Offset);
            }
            finally
            {
                isUpdating = false;
            }
        }
    }

    private void UpdateGridRowsAndColumns(TimePickerDisplayMode displayMode)
    {
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
}
