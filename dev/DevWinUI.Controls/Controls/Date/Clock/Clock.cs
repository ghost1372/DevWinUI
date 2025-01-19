using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;

namespace DevWinUI;
[TemplatePart(Name = ElementButtonAm, Type = typeof(RadioButton))]
[TemplatePart(Name = ElementButtonPm, Type = typeof(RadioButton))]
[TemplatePart(Name = ElementBorderTitle, Type = typeof(Border))]
[TemplatePart(Name = ElementMinuteHandLine, Type = typeof(Line))]
[TemplatePart(Name = ElementPanelNum, Type = typeof(CirclePanel))]
[TemplatePart(Name = ElementTimeStr, Type = typeof(TextBlock))]
[TemplatePart(Name = ElementGrid, Type = typeof(Grid))]
public partial class Clock : Control
{
    private const string ElementButtonAm = "PART_ButtonAm";
    private const string ElementButtonPm = "PART_ButtonPm";
    private const string ElementBorderTitle = "PART_BorderTitle";
    private const string ElementMinuteHandLine = "PART_MinuteHand";
    private const string ElementPanelNum = "PART_PanelNum";
    private const string ElementTimeStr = "PART_TimeStr";
    private const string ElementGrid = "PART_Grid";

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

    private int _secValue;
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
    }
    protected override void OnApplyTemplate()
    {
        isTemplateApplied = false;
        if (_buttonAm != null)
        {
            _buttonAm.Click -= ButtonAm_OnClick;
        }

        if (_buttonPm != null)
        {
            _buttonPm.Click -= ButtonPm_OnClick;
        }

        if (_borderTitle != null)
        {
            _borderTitle.PointerWheelChanged -= OnBorderTitlePointerWheelChanged;
        }

        if (_grid != null)
        {
            _grid.PointerWheelChanged -= OnGridPointerWheelChanged;
            _grid.PointerMoved -= OnGridPointerMoved;
        }

        base.OnApplyTemplate();

        _buttonAm = GetTemplateChild(ElementButtonAm) as ClockRadioButton;
        _buttonPm = GetTemplateChild(ElementButtonPm) as ClockRadioButton;
        _borderTitle = GetTemplateChild(ElementBorderTitle) as Border;
        _minuteHandLine = GetTemplateChild(ElementMinuteHandLine) as Line;
        _circlePanel = GetTemplateChild(ElementPanelNum) as CirclePanel;
        _blockTime = GetTemplateChild(ElementTimeStr) as TextBlock;
        _grid = GetTemplateChild(ElementGrid) as Grid;

        if (!CheckNull()) return;

        _buttonAm.Click += ButtonAm_OnClick;
        _buttonPm.Click += ButtonPm_OnClick;
        _borderTitle.PointerWheelChanged += OnBorderTitlePointerWheelChanged;

        _grid.PointerWheelChanged += OnGridPointerWheelChanged;

        _grid.PointerMoved += OnGridPointerMoved;

        _rotateTransformClock = new RotateTransform();
        _minuteHandLine.RenderTransform = _rotateTransformClock;

        _hourButtonList = new List<ClockRadioButton>();
        for (var i = 0; i < 12; i++)
        {
            var num = i + 1;
            var hourButton = new ClockRadioButton
            {
                Num = num,
                Content = num.ToString()
            };
            hourButton.Checked -= HourButton_Checked;
            hourButton.Checked += HourButton_Checked;
            _hourButtonList.Add(hourButton);
            _circlePanel.Children.Add(hourButton);
        }

        isTemplateApplied = true;
        //SelectedTime = DateTime.Now; //Commented out, as the SelectedTime already defaults to DateTime.Now, and this interferes with externally set DateTime.
        Update(SelectedTime);
    }

    private void HourButton_Checked(object sender, RoutedEventArgs e)
    {
        _selectedHourButton = e.OriginalSource as ClockRadioButton;
        if (_selectedHourButton != null)
        {
            Update();
        }
    }

    private void OnGridPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        var pointerPoint = e.GetCurrentPoint(_grid);
        if (pointerPoint.Properties.IsLeftButtonPressed)
        {

            var originalSource = e.OriginalSource as FrameworkElement;
            if (originalSource is TextBlock) //When clicking on a number, don't move the minute hand.
            {
                return;
            }

            var position = pointerPoint.Position;
            var minuteAngle = ArithmeticHelper.CalAngle(new Point(85, 85), position) + 90;
            if (minuteAngle < 0)
            {
                minuteAngle = minuteAngle + 360;
            }
            minuteAngle = minuteAngle - minuteAngle % 6;
            _rotateTransformClock.Angle = minuteAngle;
            Update();
        }
    }

    private void OnGridPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        var minuteAngle = (int)_rotateTransformClock.Angle;
        var delta = e.GetCurrentPoint(_grid).Properties.MouseWheelDelta;

        if (delta < 0)
        {
            minuteAngle += 6;
        }
        else
        {
            minuteAngle -= 6;
        }
        if (minuteAngle < 0)
        {
            minuteAngle = minuteAngle + 360;
        }
        _rotateTransformClock.Angle = minuteAngle;

        Update();
        e.Handled = true;
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

        if (_blockTime != null)
        {
            SelectedTime = GetDisplayTime();
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

        _rotateTransformClock.Angle = minutes * 6;

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
        var now = DateTime.Now; //Remember, this does not overwrite CalendarWithClock/DateTimePicker's SelectedDateTime, so no need to switch to TimeOnly and break users' apps.
        return new DateTime(now.Year, now.Month, now.Day, hourValue, minuteValue, _secValue);
    }

    private void ButtonAm_OnClick(object sender, RoutedEventArgs e) => Update();

    private void ButtonPm_OnClick(object sender, RoutedEventArgs e) => Update();
}
