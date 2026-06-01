// https://github.com/jayfunc/BetterLyrics

namespace DevWinUI;

public partial class BetterLyric
{
    private double renderLyricsStartX = 0;
    private double renderLyricsStartY = 0;
    private double renderLyricsWidth = 0;
    private double renderLyricsHeight = 0;
    private bool isLyricsVisible = false;
    private Point mousePosition = new(0, 0);
    private bool isMouseInLyricsArea = false;
    private bool isMousePressing = false;
    private bool isMouseScrolling = false;
    private bool isMouseScrollingChanged = false;
    private TimeSpan currentPosition = TimeSpan.Zero;
    private LyricData? currentLyricsData = null;
    private bool currentIsPlaying = true;
    private int positionOffset = 0;
    private int timelineSyncThreshold = 0;
    private bool isScrollViewerEnabled = true;
    private bool isAutoLayoutEnabled = true;
    public double LyricsStartX
    {
        get { return (double)GetValue(LyricsStartXProperty); }
        set { SetValue(LyricsStartXProperty, value); }
    }

    public static readonly DependencyProperty LyricsStartXProperty =
        DependencyProperty.Register(nameof(LyricsStartX), typeof(double), typeof(BetterLyric), new PropertyMetadata(0.0, OnLyricsStartXChanged));

    private static void OnLyricsStartXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.renderLyricsStartX = Convert.ToDouble(e.NewValue);
        ctl.RequestRelayout();
    }

    public double LyricsStartY
    {
        get { return (double)GetValue(LyricsStartYProperty); }
        set { SetValue(LyricsStartYProperty, value); }
    }

    public static readonly DependencyProperty LyricsStartYProperty =
        DependencyProperty.Register(nameof(LyricsStartY), typeof(double), typeof(BetterLyric), new PropertyMetadata(0.0, OnLyricsStartYChanged));

    private static void OnLyricsStartYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.renderLyricsStartY = Convert.ToDouble(e.NewValue);
        ctl.RequestRelayout();
    }

    public double LyricsWidth
    {
        get { return (double)GetValue(LyricsWidthProperty); }
        set { SetValue(LyricsWidthProperty, value); }
    }

    public static readonly DependencyProperty LyricsWidthProperty =
        DependencyProperty.Register(nameof(LyricsWidth), typeof(double), typeof(BetterLyric), new PropertyMetadata(0.0, OnLyricsWidthChanged));

    private static void OnLyricsWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.renderLyricsWidth = Convert.ToDouble(e.NewValue);
        ctl.RequestRelayout();
    }

    public double LyricsHeight
    {
        get { return (double)GetValue(LyricsHeightProperty); }
        set { SetValue(LyricsHeightProperty, value); }
    }

    public static readonly DependencyProperty LyricsHeightProperty =
        DependencyProperty.Register(nameof(LyricsHeight), typeof(double), typeof(BetterLyric), new PropertyMetadata(0.0, OnLyricsHeightChanged));

    private static void OnLyricsHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.renderLyricsHeight = Convert.ToDouble(e.NewValue);
        ctl.RequestRelayout();
    }

    public bool IsLyricsVisible
    {
        get { return (bool)GetValue(IsLyricsVisibleProperty); }
        set { SetValue(IsLyricsVisibleProperty, value); }
    }

    public static readonly DependencyProperty IsLyricsVisibleProperty =
        DependencyProperty.Register(nameof(IsLyricsVisible), typeof(bool), typeof(BetterLyric), new PropertyMetadata(false, OnIsLyricsVisibleChanged));

    private static void OnIsLyricsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isLyricsVisible = (bool)e.NewValue;
        ctl.RequestRelayout();
    }

    public double MouseScrollOffset
    {
        get { return (double)GetValue(MouseScrollOffsetProperty); }
        set { SetValue(MouseScrollOffsetProperty, value); }
    }

    public static readonly DependencyProperty MouseScrollOffsetProperty =
        DependencyProperty.Register(nameof(MouseScrollOffset), typeof(double), typeof(BetterLyric), new PropertyMetadata(0.0, OnMouseScrollOffsetChanged));

    private static void OnMouseScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl._mouseYScrollTransition.Start(Convert.ToDouble(e.NewValue));
    }

    public Point MousePosition
    {
        get { return (Point)GetValue(MousePositionProperty); }
        set { SetValue(MousePositionProperty, value); }
    }

    public static readonly DependencyProperty MousePositionProperty =
        DependencyProperty.Register(nameof(MousePosition), typeof(Point), typeof(BetterLyric), new PropertyMetadata(new Point(0, 0), OnMousePositionChanged));

    private static void OnMousePositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.mousePosition = (Point)e.NewValue;
    }

    public bool IsMouseInLyricsArea
    {
        get { return (bool)GetValue(IsMouseInLyricsAreaProperty); }
        set { SetValue(IsMouseInLyricsAreaProperty, value); }
    }

    public static readonly DependencyProperty IsMouseInLyricsAreaProperty =
        DependencyProperty.Register(nameof(IsMouseInLyricsArea), typeof(bool), typeof(BetterLyric), new PropertyMetadata(false, OnIsMouseInLyricsAreaChanged));

    private static void OnIsMouseInLyricsAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isMouseInLyricsArea = (bool)e.NewValue;
    }

    public bool IsMousePressing
    {
        get { return (bool)GetValue(IsMousePressingProperty); }
        set { SetValue(IsMousePressingProperty, value); }
    }

    public static readonly DependencyProperty IsMousePressingProperty =
        DependencyProperty.Register(nameof(IsMousePressing), typeof(bool), typeof(BetterLyric), new PropertyMetadata(false, OnIsMousePressingChanged));

    private static void OnIsMousePressingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isMousePressing = (bool)e.NewValue;
    }

    public bool IsMouseScrolling
    {
        get { return (bool)GetValue(IsMouseScrollingProperty); }
        set { SetValue(IsMouseScrollingProperty, value); }
    }

    public static readonly DependencyProperty IsMouseScrollingProperty =
        DependencyProperty.Register(nameof(IsMouseScrolling), typeof(bool), typeof(BetterLyric), new PropertyMetadata(false, OnIsMouseScrollingChanged));

    private static void OnIsMouseScrollingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        var newValue = (bool)e.NewValue;
        var oldValue = (bool)e.OldValue;
        ctl.isMouseScrolling = newValue;
        if (newValue != oldValue)
        {
            ctl.isMouseScrollingChanged = true;
        }
    }
    public TimeSpan CurrentPosition
    {
        get { return (TimeSpan)GetValue(CurrentPositionProperty); }
        set { SetValue(CurrentPositionProperty, value); }
    }

    public static readonly DependencyProperty CurrentPositionProperty =
        DependencyProperty.Register(nameof(CurrentPosition), typeof(TimeSpan), typeof(BetterLyric), new PropertyMetadata(TimeSpan.Zero, OnCurrentPositionChanged));

    private static void OnCurrentPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.currentPosition = (TimeSpan)e.NewValue;
        ctl.UpdateCurrentPosition();
    }

    public LyricData? CurrentLyricsData
    {
        get { return (LyricData?)GetValue(CurrentLyricsDataProperty); }
        set { SetValue(CurrentLyricsDataProperty, value); }
    }

    public static readonly DependencyProperty CurrentLyricsDataProperty =
        DependencyProperty.Register(nameof(CurrentLyricsData), typeof(LyricData), typeof(BetterLyric), new PropertyMetadata(null, OnCurrentLyricsDataChanged));

    private static void OnCurrentLyricsDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.currentLyricsData = (LyricData?)e.NewValue;
        ctl.RequestRelayout();
    }

    public bool CurrentIsPlaying
    {
        get { return (bool)GetValue(CurrentIsPlayingProperty); }
        set { SetValue(CurrentIsPlayingProperty, value); }
    }

    public static readonly DependencyProperty CurrentIsPlayingProperty =
        DependencyProperty.Register(nameof(CurrentIsPlaying), typeof(bool), typeof(BetterLyric), new PropertyMetadata(true, OnCurrentIsPlayingChanged));

    private static void OnCurrentIsPlayingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.currentIsPlaying = (bool)e.NewValue;
    }

    public int PositionOffset
    {
        get { return (int)GetValue(PositionOffsetProperty); }
        set { SetValue(PositionOffsetProperty, value); }
    }

    public static readonly DependencyProperty PositionOffsetProperty =
        DependencyProperty.Register(nameof(PositionOffset), typeof(int), typeof(BetterLyric), new PropertyMetadata(0, OnPositionOffsetChanged));

    private static void OnPositionOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.positionOffset = (int)e.NewValue;
    }
    public int TimelineSyncThreshold
    {
        get { return (int)GetValue(TimelineSyncThresholdProperty); }
        set { SetValue(TimelineSyncThresholdProperty, value); }
    }

    public static readonly DependencyProperty TimelineSyncThresholdProperty =
        DependencyProperty.Register(nameof(TimelineSyncThreshold), typeof(int), typeof(BetterLyric), new PropertyMetadata(0, OnTimelineSyncThresholdChanged));

    private static void OnTimelineSyncThresholdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.timelineSyncThreshold = (int)e.NewValue;
    }

    public bool IsAutoLayoutEnabled
    {
        get { return (bool)GetValue(IsAutoLayoutEnabledProperty); }
        set { SetValue(IsAutoLayoutEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsAutoLayoutEnabledProperty =
        DependencyProperty.Register(nameof(IsAutoLayoutEnabled), typeof(bool), typeof(BetterLyric), new PropertyMetadata(true, OnIsAutoLayoutEnabledChanged));

    private static void OnIsAutoLayoutEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isAutoLayoutEnabled = (bool)e.NewValue;
    }

    public bool IsScrollViewerEnabled
    {
        get { return (bool)GetValue(IsScrollViewerEnabledProperty); }
        set { SetValue(IsScrollViewerEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsScrollViewerEnabledProperty =
        DependencyProperty.Register(nameof(IsScrollViewerEnabled), typeof(bool), typeof(BetterLyric), new PropertyMetadata(true, OnIsScrollViewerEnabledChanged));

    private static void OnIsScrollViewerEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BetterLyric)d;
        ctl.isScrollViewerEnabled = (bool)e.NewValue;
    }
}
