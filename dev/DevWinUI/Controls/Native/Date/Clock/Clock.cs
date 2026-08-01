using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;
[TemplatePart(Name = ElementButtonAm, Type = typeof(RadioButton))]
[TemplatePart(Name = ElementButtonPm, Type = typeof(RadioButton))]
[TemplatePart(Name = ElementBorderTitle, Type = typeof(Border))]
[TemplatePart(Name = ElementMinuteHandLine, Type = typeof(Line))]
[TemplatePart(Name = ElementPanelNum, Type = typeof(CirclePanel))]
[TemplatePart(Name = ElementTimeStr, Type = typeof(TextBlock))]
[TemplatePart(Name = ElementGrid, Type = typeof(Grid))]
[TemplatePart(Name = ElementMinuteFocus, Type = typeof(ContentControl))]
public partial class Clock : Control
{
    private const string ElementButtonAm = "PART_ButtonAm";
    private const string ElementButtonPm = "PART_ButtonPm";
    private const string ElementBorderTitle = "PART_BorderTitle";
    private const string ElementMinuteHandLine = "PART_MinuteHand";
    private const string ElementPanelNum = "PART_PanelNum";
    private const string ElementTimeStr = "PART_TimeStr";
    private const string ElementGrid = "PART_Grid";
    private const string ElementMinuteFocus = "PART_MinuteFocus";

    public event EventHandler<DateTime> SelectedTimeChanged;

    private ClockRadioButton _buttonAm;

    private ClockRadioButton _buttonPm;

    private Border _borderTitle;

    private Line _minuteHandLine;

    private ClockRadioButton _selectedHourButton;

    private RotateTransform _rotateTransformClock;

    private CirclePanel _circlePanel;

    private List<ClockRadioButton> _hourButtonList;

    private TextBlock _blockTime;
    private Grid _grid;
    private ContentControl _minuteFocus;

    private int _secValue;
    private bool _isDraggingHand;
    private bool _isUpdatingSelectedTime;
    protected bool isTemplateApplied;
    private int SecValue
    {
        get => _secValue;
        set
        {
            if (value < 0)
            {
                _secValue = 59;
            }
            else if (value > 59)
            {
                _secValue = 0;
            }
            else
            {
                _secValue = value;
            }
        }
    }
    public Clock()
    {
        this.DefaultStyleKey = typeof(Clock);
        //The DP default DateTime.Now is evaluated once at class load and shared by all instances; set a per-instance value.
        SelectedTime = DateTime.Now;
    }
    protected override void OnApplyTemplate()
    {
        isTemplateApplied = false;
        if (_buttonAm != null)
        {
            _buttonAm.Click -= ButtonAm_OnClick;
            _buttonAm.PreviewKeyDown -= OnAmPmKeyDown;
        }

        if (_buttonPm != null)
        {
            _buttonPm.Click -= ButtonPm_OnClick;
            _buttonPm.PreviewKeyDown -= OnAmPmKeyDown;
        }

        if (_borderTitle != null)
        {
            _borderTitle.PointerWheelChanged -= OnBorderTitlePointerWheelChanged;
            _borderTitle.Tapped -= OnBorderTitleTapped;
        }

        if (_minuteFocus != null)
        {
            _minuteFocus.KeyDown -= OnMinuteFocusKeyDown;
            _minuteFocus.GotFocus -= OnMinuteFocusGotFocus;
            _minuteFocus.LostFocus -= OnMinuteFocusLostFocus;
        }

        if (_grid != null)
        {
            _grid.PointerWheelChanged -= OnGridPointerWheelChanged;
            _grid.PointerMoved -= OnGridPointerMoved;
            _grid.PointerPressed -= OnGridPointerPressed;
            _grid.PointerReleased -= OnGridPointerReleased;
            _grid.PointerCaptureLost -= OnGridPointerReleased;
            _grid.PointerCanceled -= OnGridPointerReleased;
        }

        base.OnApplyTemplate();

        _buttonAm = GetTemplateChild(ElementButtonAm) as ClockRadioButton;
        _buttonPm = GetTemplateChild(ElementButtonPm) as ClockRadioButton;
        _borderTitle = GetTemplateChild(ElementBorderTitle) as Border;
        _minuteHandLine = GetTemplateChild(ElementMinuteHandLine) as Line;
        _circlePanel = GetTemplateChild(ElementPanelNum) as CirclePanel;
        _blockTime = GetTemplateChild(ElementTimeStr) as TextBlock;
        _grid = GetTemplateChild(ElementGrid) as Grid;
        _minuteFocus = GetTemplateChild(ElementMinuteFocus) as ContentControl;

        if (!CheckNull()) return;

        _buttonAm.Click += ButtonAm_OnClick;
        _buttonPm.Click += ButtonPm_OnClick;
        //PreviewKeyDown: RadioButton handles arrow keys itself (group navigation), so KeyDown never fires for them.
        _buttonAm.PreviewKeyDown += OnAmPmKeyDown;
        _buttonPm.PreviewKeyDown += OnAmPmKeyDown;
        _borderTitle.PointerWheelChanged += OnBorderTitlePointerWheelChanged;
        _borderTitle.Tapped += OnBorderTitleTapped;

        if (_minuteFocus != null)
        {
            _minuteFocus.KeyDown += OnMinuteFocusKeyDown;
            _minuteFocus.GotFocus += OnMinuteFocusGotFocus;
            _minuteFocus.LostFocus += OnMinuteFocusLostFocus;
        }

        _grid.PointerWheelChanged += OnGridPointerWheelChanged;

        _grid.PointerMoved += OnGridPointerMoved;

        _grid.PointerPressed += OnGridPointerPressed;
        _grid.PointerReleased += OnGridPointerReleased;
        _grid.PointerCaptureLost += OnGridPointerReleased;
        _grid.PointerCanceled += OnGridPointerReleased;

        _rotateTransformClock = new RotateTransform();
        _minuteHandLine.RenderTransform = _rotateTransformClock;

        _hourButtonList = new List<ClockRadioButton>();
        for (var i = 0; i < 12; i++)
        {
            var num = i + 1;
            var hourButton = new ClockRadioButton
            {
                Num = num,
                Content = num.ToString(),
                TabIndex = 1,
                IsTabStop = false
            };
            hourButton.Checked -= HourButton_Checked;
            hourButton.Checked += HourButton_Checked;
            hourButton.PreviewKeyDown -= HourButton_KeyDown;
            hourButton.PreviewKeyDown += HourButton_KeyDown;
            _hourButtonList.Add(hourButton);
            _circlePanel.Children.Add(hourButton);
        }

        isTemplateApplied = true;

        //As the SelectedTime already defaults to DateTime.Now, and this interferes with externally set DateTime.
        //SelectedTime = DateTime.Now;

        Update(SelectedTime);
    }

    private void HourButton_Checked(object sender, RoutedEventArgs e)
    {
        _selectedHourButton = e.OriginalSource as ClockRadioButton;
        if (_selectedHourButton != null)
        {
            //Keep the hour group a single tab stop: only the selected hour button is tabbable.
            foreach (var hourButton in _hourButtonList)
            {
                hourButton.IsTabStop = ReferenceEquals(hourButton, _selectedHourButton);
            }
            Update();
        }
    }

    private void HourButton_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        //Left/Up move clockwise (increase), Right/Down counter-clockwise.
        var step = e.Key switch
        {
            Windows.System.VirtualKey.Up or Windows.System.VirtualKey.Left => 1,
            Windows.System.VirtualKey.Down or Windows.System.VirtualKey.Right => -1,
            _ => 0
        };
        if (step == 0 || _selectedHourButton == null) return;

        var index = (_selectedHourButton.Num - 1 + step + 12) % 12;
        var next = _hourButtonList[index];
        next.IsChecked = true;
        next.Focus(FocusState.Keyboard);
        e.Handled = true;
    }

    private void OnMinuteFocusKeyDown(object sender, KeyRoutedEventArgs e)
    {
        //Left/Up move clockwise (increase), Right/Down counter-clockwise.
        var step = e.Key switch
        {
            Windows.System.VirtualKey.Up or Windows.System.VirtualKey.Left => 1,
            Windows.System.VirtualKey.Down or Windows.System.VirtualKey.Right => -1,
            _ => 0
        };
        if (step == 0) return;

        StepMinutes(step);
        e.Handled = true;
    }

    private void OnAmPmKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key is not (Windows.System.VirtualKey.Up or Windows.System.VirtualKey.Right or
            Windows.System.VirtualKey.Down or Windows.System.VirtualKey.Left)) return;

        var other = ReferenceEquals(sender, _buttonAm) ? _buttonPm : _buttonAm;
        other.IsChecked = true;
        //Update() sets IsTabStop from the checked state; Focus() fails silently while IsTabStop is false.
        Update();
        other.Focus(FocusState.Keyboard);
        e.Handled = true;
    }

    private void OnMinuteFocusGotFocus(object sender, RoutedEventArgs e)
    {
        if (_minuteFocus.FocusState != FocusState.Keyboard) return;

        _minuteHandLine.Stroke = new SolidColorBrush(ActualTheme == ElementTheme.Dark ? Colors.White : Colors.Black);
        _minuteHandLine.StrokeThickness = 4;
    }

    private void OnMinuteFocusLostFocus(object sender, RoutedEventArgs e)
    {
        //Setting a local value severed the TemplateBinding for good, so restore the bound values explicitly.
        _minuteHandLine.Stroke = MinuteHandBackground;
        _minuteHandLine.StrokeThickness = 2;
    }

    internal void FocusSelectedHour()
    {
        //Deferred so it runs after the flyout's own initial-focus pass (and after the first-open template apply).
        DispatcherQueue.TryEnqueue(() => _selectedHourButton?.Focus(FocusState.Keyboard));
    }

    private void OnGridPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        //Only drag the minute hand when the press starts on the clock face, not on an hour button.
        _isDraggingHand = !IsInsideHourButton(e.OriginalSource as DependencyObject);
    }

    private void OnGridPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        _isDraggingHand = false;
    }

    private bool IsInsideHourButton(DependencyObject source)
    {
        while (source != null && source != _grid)
        {
            if (source is ClockRadioButton)
            {
                return true;
            }
            source = VisualTreeHelper.GetParent(source);
        }
        return false;
    }

    private void OnGridPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        var pointerPoint = e.GetCurrentPoint(_grid);
        if (pointerPoint.Properties.IsLeftButtonPressed)
        {
            if (!_isDraggingHand)
            {
                return;
            }

            var position = pointerPoint.Position;
            var minuteAngle = ArithmeticHelper.CalAngle(new Point(85, 85), position) + 90;
            if (minuteAngle < 0)
            {
                minuteAngle = minuteAngle + 360;
            }
            minuteAngle = minuteAngle - minuteAngle % (6 * MinuteIncrement);
            _rotateTransformClock.Angle = minuteAngle;
            Update();
        }
    }

    private void OnGridPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        var delta = e.GetCurrentPoint(_grid).Properties.MouseWheelDelta;
        StepMinutes(delta < 0 ? 1 : -1);
        e.Handled = true;
    }

    private void StepMinutes(int direction)
    {
        var minuteAngle = (int)_rotateTransformClock.Angle;
        minuteAngle += direction * 6 * MinuteIncrement;
        if (minuteAngle < 0)
        {
            minuteAngle = minuteAngle + 360;
        }
        _rotateTransformClock.Angle = minuteAngle;

        Update();
    }

    private void OnBorderTitlePointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        var delta = e.GetCurrentPoint(_borderTitle).Properties.MouseWheelDelta;

        if (delta < 0)
        {
            SecValue--;
            Update();
        }
        else
        {
            SecValue++;
            Update();
        }
        e.Handled = true;
    }

    private bool CheckNull()
    {
        if (_buttonPm == null || _buttonAm == null || _grid == null ||
            _borderTitle == null || _minuteHandLine == null || _circlePanel == null ||
            _blockTime == null) return false;

        return true;
    }

    private void Update()
    {
        if (!isTemplateApplied) return;
        var hourValue = _selectedHourButton.Num;
        if (_buttonPm.IsChecked == true)
        {
            hourValue += 12;
            if (hourValue == 24) hourValue = 12;
        }
        else if (hourValue == 12)
        {
            hourValue = 0;
        }
        if (hourValue == 12 && _buttonAm.IsChecked == true)
        {
            _buttonPm.IsChecked = true;
            _buttonAm.IsChecked = false;
        }

        //Keep am/pm a single tab stop: only the checked button is tabbable.
        _buttonAm.IsTabStop = _buttonAm.IsChecked == true;
        _buttonPm.IsTabStop = _buttonPm.IsChecked == true;

        if (_blockTime != null)
        {
            try
            {
                _isUpdatingSelectedTime = true;
                SelectedTime = GetDisplayTime();
            }
            finally
            {
                _isUpdatingSelectedTime = false;
            }
            _blockTime.Text = SelectedTime.ToString(TimeFormat);
        }
    }

    internal void Update(DateTime time)
    {
        if (!isTemplateApplied) return;
        var hour24 = time.Hour;
        var minutes = time.Minute;

        if (hour24 >= 12)
        {
            _buttonPm.IsChecked = true;
            _buttonAm.IsChecked = false;
        }
        else
        {
            _buttonPm.IsChecked = false;
            _buttonAm.IsChecked = true;
        }

        var snappedMinutes = (minutes / MinuteIncrement) * MinuteIncrement;
        _rotateTransformClock.Angle = snappedMinutes * 6;

        var hour12 = hour24 % 12;
        if (hour12 == 0) hour12 = 12;
        var ctl = _hourButtonList[hour12 - 1];
        ctl.IsChecked = true;

        _secValue = time.Second;
        Update();
    }

    private DateTime GetDisplayTime()
    {
        var hourValue = _selectedHourButton.Num;
        var minuteValue = (int)Math.Abs(_rotateTransformClock.Angle) % 360 / 6;
        if (_buttonPm.IsChecked == true)
        {
            hourValue += 12;
            if (hourValue == 24) hourValue = 12;
        }
        else if (hourValue == 12)
        {
            hourValue = 0;
        }

        //Remember, this does not overwrite CalendarWithClock/DateTimePicker's SelectedDateTime, so no need to switch to TimeOnly and break users' apps.
        var now = DateTime.Now;
        return new DateTime(now.Year, now.Month, now.Day, hourValue, minuteValue, _secValue);
    }

    private void ButtonAm_OnClick(object sender, RoutedEventArgs e) => Update();

    private void ButtonPm_OnClick(object sender, RoutedEventArgs e) => Update();

    private void OnBorderTitleTapped(object sender, TappedRoutedEventArgs e)
    {
        ShowTimePickerFlyout();
    }

    internal void ShowTimePickerFlyout()
    {
        if (!isTemplateApplied || _borderTitle == null)
            return;

        var flyout = new TimePickerFlyout
        {
            Time = new TimeSpan(SelectedTime.Hour, SelectedTime.Minute, SelectedTime.Second),
            MinuteIncrement = MinuteIncrement
        };

        flyout.Closed += (s, args) =>
        {
            if (flyout.Time is TimeSpan ts)
            {
                Update(new DateTime(
                    SelectedTime.Year, SelectedTime.Month, SelectedTime.Day,
                    ts.Hours, ts.Minutes, ts.Seconds));
            }
        };

        flyout.ShowAt(_borderTitle);
    }
}
