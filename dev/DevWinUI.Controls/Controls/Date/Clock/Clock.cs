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
    private ClockRadioButton _buttonAm;

    private ClockRadioButton _buttonPm;

    private Border _borderTitle;

    private Line _minuteHandLine;

    private ClockRadioButton _currentButton;

    private RotateTransform _rotateTransformClock;

    private CirclePanel _circlePanel;

    private List<ClockRadioButton> _radioButtonList;

    private TextBlock _blockTime;
    private Grid _grid;

    private int _secValue;
    protected bool AppliedTemplate;
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
    protected override void OnApplyTemplate()
    {
        AppliedTemplate = false;
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

        _radioButtonList = new List<ClockRadioButton>();
        for (var i = 0; i < 12; i++)
        {
            var num = i + 1;
            var button = new ClockRadioButton
            {
                Num = num,
                Content = num.ToString()
            };
            button.Checked -= Button_Checked;
            button.Checked += Button_Checked;
            _radioButtonList.Add(button);
            _circlePanel.Children.Add(button);
        }

        AppliedTemplate = true;
        SelectedTime = DateTime.Now;
        Update(SelectedTime);
    }

    private void Button_Checked(object sender, RoutedEventArgs e)
    {
        _currentButton = e.OriginalSource as ClockRadioButton;
        if (_currentButton != null)
        {
            Update();
        }
    }

    private void OnGridPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        var pointerPoint = e.GetCurrentPoint(_grid);
        if (pointerPoint.Properties.IsLeftButtonPressed)
        {
            var position = pointerPoint.Position;
            var value = ArithmeticHelper.CalAngle(new Point(85, 85), position) + 90;
            if (value < 0)
            {
                value = value + 360;
            }
            value = value - value % 6;
            _rotateTransformClock.Angle = value;
            Update();
        }
    }

    private void OnGridPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        var value = (int)_rotateTransformClock.Angle;
        var delta = e.GetCurrentPoint(_grid).Properties.MouseWheelDelta;

        if (delta < 0)
        {
            value += 6;
        }
        else
        {
            value -= 6;
        }
        if (value < 0)
        {
            value = value + 360;
        }
        _rotateTransformClock.Angle = value;

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
        if (!AppliedTemplate) return;
        var hValue = _currentButton.Num;
        if (_buttonPm.IsChecked == true)
        {
            hValue += 12;
            if (hValue == 24) hValue = 12;
        }
        else if (hValue == 12)
        {
            hValue = 0;
        }
        if (hValue == 12 && _buttonAm.IsChecked == true)
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
        if (!AppliedTemplate) return;
        var h = time.Hour;
        var m = time.Minute;

        if (h >= 12)
        {
            _buttonPm.IsChecked = true;
            _buttonAm.IsChecked = false;
        }
        else
        {
            _buttonPm.IsChecked = false;
            _buttonAm.IsChecked = true;
        }

        _rotateTransformClock.Angle = m * 6;

        var hRest = h % 12;
        if (hRest == 0) hRest = 12;
        var ctl = _radioButtonList[hRest - 1];
        ctl.IsChecked = true;

        _secValue = time.Second;
        Update();
    }

    private DateTime GetDisplayTime()
    {
        var hValue = _currentButton.Num;
        if (_buttonPm.IsChecked == true)
        {
            hValue += 12;
            if (hValue == 24) hValue = 12;
        }
        else if (hValue == 12)
        {
            hValue = 0;
        }
        var now = DateTime.Now;
        return new DateTime(now.Year, now.Month, now.Day, hValue, (int)Math.Abs(_rotateTransformClock.Angle) % 360 / 6, _secValue);
    }

    private void ButtonAm_OnClick(object sender, RoutedEventArgs e) => Update();

    private void ButtonPm_OnClick(object sender, RoutedEventArgs e) => Update();
}
