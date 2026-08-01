using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;
[TemplatePart(Name = ElementButtonAm, Type = typeof(RadioButton))]
[TemplatePart(Name = ElementButtonPm, Type = typeof(RadioButton))]
[TemplatePart(Name = ElementBorderTitle, Type = typeof(Border))]
[TemplatePart(Name = ElementSecondHandLine, Type = typeof(Line))]
[TemplatePart(Name = ElementMinuteHandLine, Type = typeof(Line))]
[TemplatePart(Name = ElementPanelNum, Type = typeof(CirclePanel))]
[TemplatePart(Name = ElementTimeStr, Type = typeof(TextBlock))]
public partial class RealClock : Control
{
    private const string ElementButtonAm = "PART_ButtonAm";
    private const string ElementButtonPm = "PART_ButtonPm";
    private const string ElementBorderTitle = "PART_BorderTitle";
    private const string ElementMinuteHandLine = "PART_MinuteHand";
    private const string ElementSecondHandLine = "PART_SecondHand";
    private const string ElementPanelNum = "PART_PanelNum";
    private const string ElementTimeStr = "PART_TimeStr";

    private ClockRadioButton _buttonAm;

    private ClockRadioButton _buttonPm;

    private Line _secondHandLine;
    private Line _minuteHandLine;

    private RotateTransform _secondHandRotateTransform;
    private RotateTransform _rotateTransformClock;

    private CirclePanel _circlePanel;

    private List<ClockRadioButton> _hourButtonList;

    private TextBlock _blockTime;

    private DispatcherTimer _timer;

    protected bool isTemplateApplied;

    public RealClock()
    {
        this.DefaultStyleKey = typeof(RealClock);
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    protected override void OnApplyTemplate()
    {
        isTemplateApplied = false;

        base.OnApplyTemplate();

        _buttonAm = GetTemplateChild(ElementButtonAm) as ClockRadioButton;
        _buttonPm = GetTemplateChild(ElementButtonPm) as ClockRadioButton;
        _secondHandLine = GetTemplateChild(ElementSecondHandLine) as Line;
        _minuteHandLine = GetTemplateChild(ElementMinuteHandLine) as Line;
        _circlePanel = GetTemplateChild(ElementPanelNum) as CirclePanel;
        _blockTime = GetTemplateChild(ElementTimeStr) as TextBlock;

        if (!CheckNull()) return;

        _rotateTransformClock = new RotateTransform();
        _minuteHandLine.RenderTransform = _rotateTransformClock;

        _secondHandRotateTransform = new RotateTransform();
        _secondHandLine.RenderTransform = _secondHandRotateTransform;

        _hourButtonList = new List<ClockRadioButton>();
        _circlePanel.Children.Clear();
        for (var i = 0; i < 12; i++)
        {
            var num = i + 1;
            var hourButton = new ClockRadioButton
            {
                Num = num,
                Content = num.ToString(),
                IsHitTestVisible = false
            };
            _hourButtonList.Add(hourButton);
            _circlePanel.Children.Add(hourButton);
        }

        isTemplateApplied = true;

        Update();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Update();

        if (_timer == null)
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };

            _timer.Tick += OnTimerTick;
        }

        _timer.Start();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _timer?.Stop();
    }

    private void OnTimerTick(object sender, object e)
    {
        Update();
    }

    private bool CheckNull()
    {
        if (_buttonPm == null || _buttonAm == null ||
            _secondHandLine == null || _minuteHandLine == null || _circlePanel == null ||
            _blockTime == null) return false;

        return true;
    }

    internal void Update()
    {
        if (!isTemplateApplied) return;

        var time = TimeZoneInfo.ConvertTime(DateTime.Now, ResolveTimeZone());

        if (time.Hour >= 12)
        {
            _buttonPm.IsChecked = true;
            _buttonAm.IsChecked = false;
        }
        else
        {
            _buttonPm.IsChecked = false;
            _buttonAm.IsChecked = true;
        }

        _secondHandRotateTransform.Angle = (float)(time.Second * 6 + time.Millisecond * 0.006);

        _rotateTransformClock.Angle = (float)(time.Minute * 6 + time.Second * 0.1);

        var hour12 = time.Hour % 12;
        if (hour12 == 0) hour12 = 12;

        _hourButtonList[hour12 - 1].IsChecked = true;

        _blockTime.Text = time.ToString(TimeFormat);
    }
    private TimeZoneInfo ResolveTimeZone()
    {
        if (string.IsNullOrEmpty(TimeZoneId)) return TimeZoneInfo.Local;

        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
        }
        catch (Exception)
        {
            return TimeZoneInfo.Local;
        }
    }
}
