using Windows.Globalization;
using DayOfWeek = Windows.Globalization.DayOfWeek;

namespace DevWinUI;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract partial class DateTimeBase : Control
{
    public static TimeSpan? TimeNow => new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

    public bool IsGroupLabelVisible
    {
        get { return (bool)GetValue(IsGroupLabelVisibleProperty); }
        set { SetValue(IsGroupLabelVisibleProperty, value); }
    }

    public static readonly DependencyProperty IsGroupLabelVisibleProperty =
        DependencyProperty.Register(nameof(IsGroupLabelVisible), typeof(bool), typeof(DateTimeBase), new PropertyMetadata(false));
    public bool IsTodayHighlighted
    {
        get { return (bool)GetValue(IsTodayHighlightedProperty); }
        set { SetValue(IsTodayHighlightedProperty, value); }
    }

    public static readonly DependencyProperty IsTodayHighlightedProperty =
        DependencyProperty.Register(nameof(IsTodayHighlighted), typeof(bool), typeof(DateTimeBase), new PropertyMetadata(true));

    public CalendarViewDisplayMode DisplayMode
    {
        get { return (CalendarViewDisplayMode)GetValue(DisplayModeProperty); }
        set { SetValue(DisplayModeProperty, value); }
    }

    public static readonly DependencyProperty DisplayModeProperty =
        DependencyProperty.Register(nameof(DisplayMode), typeof(CalendarViewDisplayMode), typeof(DateTimeBase), new PropertyMetadata(CalendarViewDisplayMode.Month));

    public DayOfWeek FirstDayOfWeek
    {
        get { return (DayOfWeek)GetValue(FirstDayOfWeekProperty); }
        set { SetValue(FirstDayOfWeekProperty, value); }
    }

    public static readonly DependencyProperty FirstDayOfWeekProperty =
        DependencyProperty.Register(nameof(FirstDayOfWeek), typeof(DayOfWeek), typeof(DateTimeBase), new PropertyMetadata(DayOfWeek.Sunday));
    public string DayOfWeekFormat
    {
        get { return (string)GetValue(DayOfWeekFormatProperty); }
        set { SetValue(DayOfWeekFormatProperty, value); }
    }

    public static readonly DependencyProperty DayOfWeekFormatProperty =
        DependencyProperty.Register(nameof(DayOfWeekFormat), typeof(string), typeof(DateTimeBase), new PropertyMetadata(default(string)));
    public string CalendarIdentifier
    {
        get { return (string)GetValue(CalendarIdentifierProperty); }
        set { SetValue(CalendarIdentifierProperty, value); }
    }

    public static readonly DependencyProperty CalendarIdentifierProperty =
        DependencyProperty.Register(nameof(CalendarIdentifier), typeof(string), typeof(DateTimeBase), new PropertyMetadata(CalendarIdentifiers.Gregorian));
    public bool IsOutOfScopeEnabled
    {
        get { return (bool)GetValue(IsOutOfScopeEnabledProperty); }
        set { SetValue(IsOutOfScopeEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsOutOfScopeEnabledProperty =
        DependencyProperty.Register(nameof(IsOutOfScopeEnabled), typeof(bool), typeof(DateTimeBase), new PropertyMetadata(true));
    public DateTimeOffset MaxDate
    {
        get { return (DateTimeOffset)GetValue(MaxDateProperty); }
        set { SetValue(MaxDateProperty, value); }
    }

    public static readonly DependencyProperty MaxDateProperty =
        DependencyProperty.Register(nameof(MaxDate), typeof(DateTimeOffset), typeof(DateTimeBase), new PropertyMetadata(new DateTimeOffset(2124, 12, 31, 0, 0, 0, TimeSpan.Zero)));

    public DateTimeOffset MinDate
    {
        get { return (DateTimeOffset)GetValue(MinDateProperty); }
        set { SetValue(MinDateProperty, value); }
    }

    public static readonly DependencyProperty MinDateProperty =
        DependencyProperty.Register(nameof(MinDate), typeof(DateTimeOffset), typeof(DateTimeBase), new PropertyMetadata(new DateTimeOffset(1924, 1, 1, 0, 0, 0, TimeSpan.Zero)));
    public Thickness CalendarViewMargin
    {
        get { return (Thickness)GetValue(CalendarViewMarginProperty); }
        set { SetValue(CalendarViewMarginProperty, value); }
    }

    public static readonly DependencyProperty CalendarViewMarginProperty =
        DependencyProperty.Register(nameof(CalendarViewMargin), typeof(Thickness), typeof(DateTimeBase), new PropertyMetadata(default(Thickness)));
    public Thickness TimePickerMargin
    {
        get { return (Thickness)GetValue(TimePickerMarginProperty); }
        set { SetValue(TimePickerMarginProperty, value); }
    }

    public static readonly DependencyProperty TimePickerMarginProperty =
        DependencyProperty.Register(nameof(TimePickerMargin), typeof(Thickness), typeof(DateTimeBase), new PropertyMetadata(new Thickness(5)));

    public Style CalendarViewStyle
    {
        get { return (Style)GetValue(CalendarViewStyleProperty); }
        set { SetValue(CalendarViewStyleProperty, value); }
    }

    public static readonly DependencyProperty CalendarViewStyleProperty =
        DependencyProperty.Register(nameof(CalendarViewStyle), typeof(Style), typeof(DateTimeBase), new PropertyMetadata(default(Style)));

    public Thickness CalendarViewBorderThickness
    {
        get { return (Thickness)GetValue(CalendarViewBorderThicknessProperty); }
        set { SetValue(CalendarViewBorderThicknessProperty, value); }
    }

    public static readonly DependencyProperty CalendarViewBorderThicknessProperty =
        DependencyProperty.Register(nameof(CalendarViewBorderThickness), typeof(Thickness), typeof(DateTimeBase), new PropertyMetadata(new Thickness(1)));
}
