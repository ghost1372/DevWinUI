using System.Diagnostics;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

[TemplatePart(Name = PartRoot, Type = typeof(Border))]
[TemplatePart(Name = PartSurface, Type = typeof(Grid))]
[TemplatePart(Name = PartSnapshot, Type = typeof(Image))]
[TemplatePart(Name = PartIconPresenter, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PartIconMark, Type = typeof(Path))]
[TemplatePart(Name = PartIconMarkOutline, Type = typeof(Path))]
[TemplatePart(Name = PartRingText, Type = typeof(TextBlock))]
[TemplatePart(Name = PartTitleBlock, Type = typeof(TextBlock))]
[TemplatePart(Name = PartSubTitleBlock, Type = typeof(TextBlock))]
[TemplatePart(Name = PartCloseButton, Type = typeof(Button))]
[TemplatePart(Name = PartContentPresenter, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PartProgressText, Type = typeof(TextBlock))]
[TemplatePart(Name = PartActionPresenter, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PartCountdownFill, Type = typeof(Border))]
[TemplatePart(Name = PartCountdownProbe, Type = typeof(Border))]
[TemplatePart(Name = PartCountdownText, Type = typeof(TextBlock))]
[TemplatePart(Name = PartSentText, Type = typeof(TextBlock))]
[TemplatePart(Name = PartContentHost, Type = typeof(Grid))]
[TemplatePart(Name = PartIndicatorTop, Type = typeof(Border))]
[TemplatePart(Name = PartIndicatorLeft, Type = typeof(Border))]
[TemplatePart(Name = PartIndicatorRight, Type = typeof(Border))]
[TemplatePart(Name = PartIndicatorBottom, Type = typeof(Border))]
public sealed partial class GrowlCard : ContentControl
{
    private const string PartRoot = "PART_Root";
    private const string PartSurface = "PART_Surface";
    private const string PartSnapshot = "PART_Snapshot";
    private const string PartIconPresenter = "PART_IconPresenter";
    private const string PartIconMark = "PART_IconMark";
    private const string PartIconMarkOutline = "PART_IconMarkOutline";
    private const string PartRingText = "PART_RingText";
    private const string PartTitleBlock = "PART_TitleBlock";
    private const string PartSubTitleBlock = "PART_SubTitleBlock";
    private const string PartCloseButton = "PART_CloseButton";
    private const string PartContentPresenter = "PART_ContentPresenter";
    private const string PartProgressText = "PART_ProgressText";
    private const string PartActionPresenter = "PART_ActionPresenter";
    private const string PartCountdownFill = "PART_CountdownFill";
    private const string PartCountdownProbe = "PART_CountdownProbe";
    private const string PartCountdownText = "PART_CountdownText";
    private const string PartSentText = "PART_SentText";
    private const string PartContentHost = "PART_ContentHost";
    private const string PartIndicatorTop = "PART_IndicatorTop";
    private const string PartIndicatorLeft = "PART_IndicatorLeft";
    private const string PartIndicatorRight = "PART_IndicatorRight";
    private const string PartIndicatorBottom = "PART_IndicatorBottom";

    private static readonly TimeSpan SentTimeRefreshInterval = TimeSpan.FromSeconds(30);

    private readonly Stopwatch _stopwatch = new();
    private readonly SolidColorBrush _indicatorBrush = new();
    private readonly SolidColorBrush _countdownBrush = new();
    private DispatcherQueueTimer? _timer;
    private Storyboard? _transition;
    private bool _isPointerOver;
    private bool _applyScheduled;
    private GrowlCardSeverity _appliedType;
    private bool _widthManaged;
    private bool _syncingOpen;
    private bool _countdownSuspended;
    private readonly DateTimeOffset _createdAt = DateTimeOffset.Now;
    private DispatcherQueueTimer? _sentTimer;
    private Func<DateTimeOffset, string>? _sentTimeFormatter;

    private Border? _root;
    private Grid? _surface;
    private Image? _snapshot;
    private ContentPresenter? _iconPresenter;
    private Path? _iconMark;
    private Path? _iconMarkOutline;
    private TextBlock? _ringText;
    private TextBlock? _titleBlock;
    private TextBlock? _subTitleBlock;
    private Button? _closeButton;
    private ContentPresenter? _contentPresenter;
    private TextBlock? _progressText;
    private ContentPresenter? _actionPresenter;
    private Border? _countdownFill;
    private ScaleTransform? _countdownScale;
    private Border? _countdownProbe;
    private TextBlock? _countdownText;
    private TextBlock? _sentText;
    private Grid? _contentHost;
    private Border? _indicatorTop;
    private Border? _indicatorLeft;
    private Border? _indicatorRight;
    private Border? _indicatorBottom;

    public GrowlCard()
    {
        DefaultStyleKey = typeof(GrowlCard);

        Tapped += OnCardTapped;

        Loaded += (_, _) =>
        {
            StartAutoClose();
            UpdateSentTime();
            Announce();
        };
        Unloaded += (_, _) =>
        {
            StopAutoClose();
            _sentTimer?.Stop();
            FinishTransition();
        };

        ActualThemeChanged += (_, _) =>
        {
            if (_transition is null)
            {
                _indicatorBrush.Color = ResolveIndicatorColor(_appliedType);
            }

            UpdateCountdownBrush();
        };

        PointerEntered += (_, _) =>
        {
            _isPointerOver = true;
            GoTo("PointerOver");
            UpdateSentTimeState();
            if (PauseAutoCloseOnPointerOver)
            {
                _stopwatch.Stop();
            }
        };

        PointerExited += (_, _) =>
        {
            _isPointerOver = false;
            GoTo("Normal");
            UpdateSentTimeState();
            if (_timer?.IsRunning == true && !_countdownSuspended)
            {
                _stopwatch.Start();
            }
        };

        RegisterPropertyChangedCallback(VisibilityProperty, (_, _) =>
        {
            if (!_syncingOpen)
            {
                IsOpen = Visibility == Visibility.Visible;
            }
        });
    }

    public Func<DateTimeOffset, string>? SentTimeFormatter
    {
        get { return _sentTimeFormatter; }
        set
        {
            _sentTimeFormatter = value;
            RequestApply();
        }
    }

    public event TypedEventHandler<GrowlCard, object?>? Opened;

    public event TypedEventHandler<GrowlCard, object?>? Closed;

    public event TypedEventHandler<GrowlCard, object?>? Invoked;

    public bool CloseOnInvoke { get; set; }

    private bool IsBanner => DisplayMode == GrowlCardDisplayMode.Banner;

    public void Open()
    {
        IsOpen = true;
    }

    internal Action<Action>? CloseAnimator { get; set; }

    private bool _isClosing;

    internal bool IsClosing => _isClosing;

    public void Close()
    {
        if (CloseAnimator is null || !IsOpen || !IsLoaded)
        {
            IsOpen = false;
            return;
        }

        if (_isClosing)
        {
            return;
        }

        _isClosing = true;
        StopAutoClose();

        CloseAnimator(() =>
        {
            _isClosing = false;
            IsOpen = false;
        });
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        FinishTransition();

        if (_closeButton is not null)
        {
            _closeButton.Click -= OnCloseButtonClick;
        }

        _root = GetTemplateChild(PartRoot) as Border;
        _surface = GetTemplateChild(PartSurface) as Grid;
        GrowlStrings.SetDefaults(_surface?.Resources);
        _snapshot = GetTemplateChild(PartSnapshot) as Image;
        _iconPresenter = GetTemplateChild(PartIconPresenter) as ContentPresenter;
        _iconMark = GetTemplateChild(PartIconMark) as Path;
        _iconMarkOutline = GetTemplateChild(PartIconMarkOutline) as Path;
        _ringText = GetTemplateChild(PartRingText) as TextBlock;
        _titleBlock = GetTemplateChild(PartTitleBlock) as TextBlock;
        _subTitleBlock = GetTemplateChild(PartSubTitleBlock) as TextBlock;
        _closeButton = GetTemplateChild(PartCloseButton) as Button;
        _contentPresenter = GetTemplateChild(PartContentPresenter) as ContentPresenter;
        _progressText = GetTemplateChild(PartProgressText) as TextBlock;
        _actionPresenter = GetTemplateChild(PartActionPresenter) as ContentPresenter;
        _countdownFill = GetTemplateChild(PartCountdownFill) as Border;
        _countdownProbe = GetTemplateChild(PartCountdownProbe) as Border;
        _countdownText = GetTemplateChild(PartCountdownText) as TextBlock;
        _sentText = GetTemplateChild(PartSentText) as TextBlock;
        _contentHost = GetTemplateChild(PartContentHost) as Grid;
        _indicatorTop = GetTemplateChild(PartIndicatorTop) as Border;
        _indicatorLeft = GetTemplateChild(PartIndicatorLeft) as Border;
        _indicatorRight = GetTemplateChild(PartIndicatorRight) as Border;
        _indicatorBottom = GetTemplateChild(PartIndicatorBottom) as Border;

        if (_closeButton is not null)
        {
            _closeButton.Click += OnCloseButtonClick;
            AutomationProperties.SetName(_closeButton, GrowlStrings.Close);
        }

        foreach (var indicator in new[] { _indicatorTop, _indicatorLeft, _indicatorRight, _indicatorBottom })
        {
            if (indicator is not null)
            {
                indicator.Background = _indicatorBrush;
            }
        }

        if (_countdownFill is not null)
        {
            _countdownScale = new ScaleTransform();
            _countdownFill.RenderTransform = _countdownScale;
        }

        _countdownTextValue = null;

        Commit();
        ResetCountdown();
    }

    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);
        RequestApply();
    }

    private void ApplyIsOpen(bool isOpen)
    {
        if (_syncingOpen)
        {
            return;
        }

        _syncingOpen = true;
        try
        {
            if (isOpen)
            {
                Visibility = Visibility.Visible;
                StartAutoClose();
                Opened?.Invoke(this, null);
            }
            else
            {
                StopAutoClose();
                FinishTransition();
                Visibility = Visibility.Collapsed;
                Closed?.Invoke(this, null);
            }
        }
        finally
        {
            _syncingOpen = false;
        }
    }

    private static string FormatAge(TimeSpan age)
    {
        if (age < TimeSpan.FromMinutes(1))
        {
            return GrowlStrings.AgeNow;
        }

        if (age < TimeSpan.FromHours(1))
        {
            return GrowlStrings.AgeMinutes((int)age.TotalMinutes);
        }

        if (age < TimeSpan.FromDays(1))
        {
            return GrowlStrings.AgeHours((int)age.TotalHours);
        }

        if (age < TimeSpan.FromDays(7))
        {
            return GrowlStrings.AgeDays((int)age.TotalDays);
        }

        if (age < TimeSpan.FromDays(30))
        {
            return GrowlStrings.AgeWeeks((int)(age.TotalDays / 7));
        }

        if (age < TimeSpan.FromDays(365))
        {
            return GrowlStrings.AgeMonths((int)(age.TotalDays / 30));
        }

        return GrowlStrings.AgeYears((int)(age.TotalDays / 365));
    }

    private void UpdateSentTime()
    {
        if (_sentText is not null)
        {
            _sentText.Text = FormatSentTime();
        }

        UpdateSentTimer();
    }

    private void UpdateSentTimeState()
    {
        GoTo(SentTimeVisibility switch
        {
            GrowlCardSentTimeVisibility.Visible => "SentTimeVisible",
            GrowlCardSentTimeVisibility.OnHover => _isPointerOver ? "SentTimeVisible" : "SentTimeOnHoverHidden",
            _ => "SentTimeCollapsed"
        });
    }

    private void UpdateSentTimer()
    {
        if (SentTimeVisibility == GrowlCardSentTimeVisibility.Hidden || !IsLoaded)
        {
            _sentTimer?.Stop();
            return;
        }

        _sentTimer ??= DispatcherQueue.CreateTimer();
        _sentTimer.Interval = SentTimeRefreshInterval;
        _sentTimer.Tick -= OnSentTimerTick;
        _sentTimer.Tick += OnSentTimerTick;
        _sentTimer.Start();
    }

    private void OnSentTimerTick(DispatcherQueueTimer sender, object args)
    {
        if (_sentText is not null)
        {
            _sentText.Text = FormatSentTime();
        }
    }

    private string FormatSentTime()
    {
        var sent = SentAt ?? _createdAt;

        return _sentTimeFormatter?.Invoke(sent) ?? FormatAge(DateTimeOffset.Now - sent);
    }

    private void OnCardTapped(object sender, TappedRoutedEventArgs e)
    {
        if (e.Handled || IsInteractiveSource(e.OriginalSource as DependencyObject))
        {
            return;
        }

        Invoked?.Invoke(this, null);

        if (CloseOnInvoke)
        {
            Close();
        }
    }

    private bool IsInteractiveSource(DependencyObject? source)
    {
        while (source is not null && !ReferenceEquals(source, this))
        {
            if (source is ButtonBase or TextBox or PasswordBox or RichEditBox or AutoSuggestBox or NumberBox or ComboBox or ToggleSwitch or Slider or ListViewBase or CalendarDatePicker or DatePicker or TimePicker or RatingControl)
            {
                return true;
            }

            source = VisualTreeHelper.GetParent(source);
        }

        return false;
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

}
