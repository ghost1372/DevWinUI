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
    private bool _isRenderingSubscribed;
    private int _selectedHourIndex = -1;
    private bool? _isPm;
    private string _lastTimeText;
    private string _resolvedTimeZoneId;
    private TimeZoneInfo _resolvedTimeZone = TimeZoneInfo.Local;

    protected bool isTemplateApplied;

    public RealClock()
    {
        DefaultStyleKey = typeof(RealClock);
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

        _selectedHourIndex = -1;
        _isPm = null;
        _lastTimeText = null;

        isTemplateApplied = true;
        Update();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Update();
        StartRenderingUpdates();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        StopRenderingUpdates();
    }

    private void StartRenderingUpdates()
    {
        if (_isRenderingSubscribed) return;

        CompositionTarget.Rendering += OnCompositionTargetRendering;
        _isRenderingSubscribed = true;
    }

    private void StopRenderingUpdates()
    {
        if (!_isRenderingSubscribed) return;

        CompositionTarget.Rendering -= OnCompositionTargetRendering;
        _isRenderingSubscribed = false;
    }

    private void OnCompositionTargetRendering(object sender, object e)
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

        var time = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ResolveTimeZone());

        var isPm = time.Hour >= 12;
        if (_isPm != isPm)
        {
            _buttonPm.IsChecked = isPm;
            _buttonAm.IsChecked = !isPm;
            _isPm = isPm;
        }

        var second = time.TimeOfDay.TotalSeconds % 60;
        _secondHandRotateTransform.Angle = second * 6.0;

        var minute = time.TimeOfDay.TotalMinutes % 60;
        _rotateTransformClock.Angle = minute * 6.0;

        var hour12 = time.Hour % 12;
        if (hour12 == 0) hour12 = 12;

        var hourIndex = hour12 - 1;
        if (hourIndex != _selectedHourIndex)
        {
            if (_selectedHourIndex >= 0 && _selectedHourIndex < _hourButtonList.Count)
            {
                _hourButtonList[_selectedHourIndex].IsChecked = false;
            }

            _hourButtonList[hourIndex].IsChecked = true;
            _selectedHourIndex = hourIndex;
        }

        var timeText = time.ToString(TimeFormat);
        if (!string.Equals(_lastTimeText, timeText, StringComparison.Ordinal))
        {
            _blockTime.Text = timeText;
            _lastTimeText = timeText;
        }
    }

    private TimeZoneInfo ResolveTimeZone()
    {
        if (string.IsNullOrWhiteSpace(TimeZoneId))
        {
            _resolvedTimeZoneId = null;
            _resolvedTimeZone = TimeZoneInfo.Local;
            return _resolvedTimeZone;
        }

        if (string.Equals(_resolvedTimeZoneId, TimeZoneId, StringComparison.Ordinal))
        {
            return _resolvedTimeZone;
        }

        try
        {
            _resolvedTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
        }
        catch (Exception)
        {
            _resolvedTimeZone = TimeZoneInfo.Local;
        }

        _resolvedTimeZoneId = TimeZoneId;
        return _resolvedTimeZone;
    }
}
