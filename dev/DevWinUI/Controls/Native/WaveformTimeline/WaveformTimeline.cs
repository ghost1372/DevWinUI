using System.ComponentModel;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Waveform), Type = typeof(Canvas))]
[TemplatePart(Name = nameof(PART_Timeline), Type = typeof(Canvas))]
[TemplatePart(Name = nameof(PART_Repeat), Type = typeof(Canvas))]
[TemplatePart(Name = nameof(PART_Progress), Type = typeof(Canvas))]
public partial class WaveformTimeline : Control
{
    private const string PART_Waveform = "PART_Waveform";
    private const string PART_Repeat = "PART_Repeat";
    private const string PART_Progress = "PART_Progress";
    private const string PART_Timeline = "PART_Timeline";

    private Canvas waveformCanvas;
    private Canvas repeatCanvas;
    private Canvas timelineCanvas;
    private Canvas progressCanvas;

    private IWaveformPlayer soundPlayer;
    private readonly Path leftPath = new Path();
    private readonly Path rightPath = new Path();
    private readonly Line centerLine = new Line();
    private readonly Rectangle repeatRegion = new Rectangle();
    private readonly Line progressLine = new Line();
    private readonly Path progressIndicator = new Path();
    private readonly List<Line> timeLineTicks = new List<Line>();
    private readonly Rectangle timelineBackgroundRegion = new Rectangle();
    private readonly List<TextBlock> timestampTextBlocks = new List<TextBlock>();
    private bool isMouseDown;
    private Point mouseDownPoint;
    private Point currentPoint;
    private double startLoopRegion = -1;
    private double endLoopRegion = -1;

    private const int mouseMoveTolerance = 3;
    private const int indicatorTriangleWidth = 4;
    private const int majorTickHeight = 10;
    private const int minorTickHeight = 3;
    private const int timeStampMargin = 5;

    public WaveformTimeline()
    {
        DefaultStyleKey = typeof(WaveformTimeline);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (waveformCanvas != null)
        {
            waveformCanvas.Children.Clear();
        }

        if (timelineCanvas != null)
        {
            timelineCanvas.Children.Clear();
        }

        if (repeatCanvas != null)
        {
            repeatCanvas.Children.Clear();
        }

        if (progressCanvas != null)
        {
            progressCanvas.Children.Clear();
        }

        waveformCanvas = GetTemplateChild(PART_Waveform) as Canvas;
        waveformCanvas.CacheMode = new BitmapCache();

        // Used to make the transparent regions clickable.
        waveformCanvas.Background = new SolidColorBrush(Colors.Transparent);

        waveformCanvas.Children.Add(centerLine);
        waveformCanvas.Children.Add(leftPath);
        waveformCanvas.Children.Add(rightPath);

        timelineCanvas = GetTemplateChild(PART_Timeline) as Canvas;
        timelineCanvas.Children.Add(timelineBackgroundRegion);

        timelineCanvas.SizeChanged -= OnTimelineCanvasSizeChanged;
        timelineCanvas.SizeChanged += OnTimelineCanvasSizeChanged;

        repeatCanvas = GetTemplateChild(PART_Repeat) as Canvas;
        repeatCanvas.Children.Add(repeatRegion);

        progressCanvas = GetTemplateChild(PART_Progress) as Canvas;
        progressCanvas.Children.Add(progressIndicator);
        progressCanvas.Children.Add(progressLine);

        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateAllRegions();
    }

    public void RegisterSoundPlayer(IWaveformPlayer soundPlayer)
    {
        this.soundPlayer = soundPlayer;
        soundPlayer.PropertyChanged -= OnSoundPlayerPropertyChanged;
        soundPlayer.PropertyChanged += OnSoundPlayerPropertyChanged;
    }

    private void OnSoundPlayerPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "SelectionBegin":
                startLoopRegion = soundPlayer.SelectionBegin.TotalSeconds;
                UpdateRepeatRegion();
                break;
            case "SelectionEnd":
                endLoopRegion = soundPlayer.SelectionEnd.TotalSeconds;
                UpdateRepeatRegion();
                break;
            case "WaveformData":
                UpdateWaveform();
                break;
            case "ChannelPosition":
                UpdateProgressIndicator();
                break;
            case "ChannelLength":
                startLoopRegion = -1;
                endLoopRegion = -1;
                UpdateAllRegions();
                break;
        }
    }

    private void OnTimelineCanvasSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateTimeline();
    }

    protected override void OnPointerPressed(PointerRoutedEventArgs e)
    {
        base.OnPointerPressed(e);

        var pointerPoint = e.GetCurrentPoint(waveformCanvas);
        if (pointerPoint.Properties.IsLeftButtonPressed)
        {
            CapturePointer(e.Pointer);
            isMouseDown = true;
            mouseDownPoint = pointerPoint.Position;
        }
    }
    protected override void OnPointerReleased(PointerRoutedEventArgs e)
    {
        base.OnPointerReleased(e);

        var pointerPoint = e.GetCurrentPoint(waveformCanvas);
        if (!isMouseDown)
            return;

        bool updateRepeatRegion = false;
        isMouseDown = false;
        ReleasePointerCapture(e.Pointer);

        var currentPoint = pointerPoint.Position;

        if (Math.Abs(currentPoint.X - mouseDownPoint.X) < mouseMoveTolerance)
        {
            if (PointInRepeatRegion(mouseDownPoint))
            {
                double position = (currentPoint.X / ActualWidth) * soundPlayer.ChannelLength;
                soundPlayer.ChannelPosition = Math.Min(soundPlayer.ChannelLength, Math.Max(0, position));
            }
            else
            {
                soundPlayer.SelectionBegin = TimeSpan.Zero;
                soundPlayer.SelectionEnd = TimeSpan.Zero;
                double position = (currentPoint.X / ActualWidth) * soundPlayer.ChannelLength;
                soundPlayer.ChannelPosition = Math.Min(soundPlayer.ChannelLength, Math.Max(0, position));
                startLoopRegion = -1;
                endLoopRegion = -1;
                updateRepeatRegion = true;
            }
        }
        else
        {
            soundPlayer.SelectionBegin = TimeSpan.FromSeconds(startLoopRegion);
            soundPlayer.SelectionEnd = TimeSpan.FromSeconds(endLoopRegion);
            double position = startLoopRegion;
            soundPlayer.ChannelPosition = Math.Min(soundPlayer.ChannelLength, Math.Max(0, position));
            updateRepeatRegion = true;
        }

        if (updateRepeatRegion)
            UpdateRepeatRegion();
    }

    protected override void OnPointerMoved(PointerRoutedEventArgs e)
    {
        currentPoint = e.GetCurrentPoint(waveformCanvas).Position;

        if (isMouseDown && AllowRepeatRegions)
        {
            if (Math.Abs(currentPoint.X - mouseDownPoint.X) > mouseMoveTolerance)
            {
                if (mouseDownPoint.X < currentPoint.X)
                {
                    startLoopRegion = (mouseDownPoint.X / RenderSize.Width) * soundPlayer.ChannelLength;
                    endLoopRegion = (currentPoint.X / RenderSize.Width) * soundPlayer.ChannelLength;
                }
                else
                {
                    startLoopRegion = (currentPoint.X / RenderSize.Width) * soundPlayer.ChannelLength;
                    endLoopRegion = (mouseDownPoint.X / RenderSize.Width) * soundPlayer.ChannelLength;
                }
            }
            else
            {
                startLoopRegion = -1;
                endLoopRegion = -1;
            }
            UpdateRepeatRegion();
        }
    }

    private bool PointInRepeatRegion(Point point)
    {
        if (soundPlayer.ChannelLength == 0)
            return false;

        double regionLeft = (soundPlayer.SelectionBegin.TotalSeconds / soundPlayer.ChannelLength) * RenderSize.Width;
        double regionRight = (soundPlayer.SelectionEnd.TotalSeconds / soundPlayer.ChannelLength) * RenderSize.Width;

        return (point.X >= regionLeft && point.X < regionRight);
    }

    private void UpdateAllRegions()
    {
        UpdateRepeatRegion();
        CreateProgressIndicator();
        UpdateTimeline();
        UpdateWaveform();
    }

    private void UpdateRepeatRegion()
    {
        if (soundPlayer == null || repeatCanvas == null)
            return;

        double startPercent = startLoopRegion / soundPlayer.ChannelLength;
        double startXLocation = startPercent * repeatCanvas.RenderSize.Width;
        double endPercent = endLoopRegion / soundPlayer.ChannelLength;
        double endXLocation = endPercent * repeatCanvas.RenderSize.Width;

        if (soundPlayer.ChannelLength == 0 ||
            endXLocation <= startXLocation)
        {
            repeatRegion.Width = 0;
            repeatRegion.Height = 0;
            return;
        }

        repeatRegion.Margin = new Thickness(startXLocation, 0, 0, 0);
        repeatRegion.Width = endXLocation - startXLocation;
        repeatRegion.Height = repeatCanvas.RenderSize.Height;
    }

    private void UpdateTimeline()
    {
        if (soundPlayer == null || timelineCanvas == null)
            return;

        foreach (TextBlock textblock in timestampTextBlocks)
        {
            timelineCanvas.Children.Remove(textblock);
        }
        timestampTextBlocks.Clear();

        foreach (Line line in timeLineTicks)
        {
            timelineCanvas.Children.Remove(line);
        }
        timeLineTicks.Clear();

        double bottomLoc = timelineCanvas.RenderSize.Height - 1;

        timelineBackgroundRegion.Width = timelineCanvas.RenderSize.Width;
        timelineBackgroundRegion.Height = timelineCanvas.RenderSize.Height;

        double minorTickDuration = 1.00d; // Major tick = 5 seconds, Minor tick = 1.00 second
        double majorTickDuration = 5.00d;
        if (soundPlayer.ChannelLength >= 120.0d) // Major tick = 1 minute, Minor tick = 15 seconds.
        {
            minorTickDuration = 15.0d;
            majorTickDuration = 60.0d;
        }
        else if (soundPlayer.ChannelLength >= 60.0d) // Major tick = 30 seconds, Minor tick = 5.0 seconds.
        {
            minorTickDuration = 5.0d;
            majorTickDuration = 30.0d;
        }
        else if (soundPlayer.ChannelLength >= 30.0d) // Major tick = 10 seconds, Minor tick = 2.0 seconds.
        {
            minorTickDuration = 2.0d;
            majorTickDuration = 10.0d;
        }

        if (soundPlayer.ChannelLength < minorTickDuration)
            return;

        int minorTickCount = (int)(soundPlayer.ChannelLength / minorTickDuration);
        for (int i = 1; i <= minorTickCount; i++)
        {
            Line timelineTick = new Line()
            {
                Stroke = TimelineTickBrush,
                StrokeThickness = 1.0d
            };
            if (i % (majorTickDuration / minorTickDuration) == 0) // Draw Large Ticks and Timestamps at minute marks
            {
                double xLocation = ((i * minorTickDuration) / soundPlayer.ChannelLength) * timelineCanvas.RenderSize.Width;

                bool drawTextBlock = false;
                double lastTimestampEnd;
                if (timestampTextBlocks.Count != 0)
                {
                    TextBlock lastTextBlock = timestampTextBlocks[timestampTextBlocks.Count - 1];
                    lastTimestampEnd = lastTextBlock.Margin.Left + lastTextBlock.ActualWidth;
                }
                else
                    lastTimestampEnd = 0;

                if (xLocation > lastTimestampEnd + timeStampMargin)
                    drawTextBlock = true;

                // Flag that we're at the end of the timeline such 
                // that there is not enough room for the text to draw.
                bool isAtEndOfTimeline = (timelineCanvas.RenderSize.Width - xLocation < 28.0d);

                if (drawTextBlock)
                {
                    timelineTick.X1 = xLocation;
                    timelineTick.Y1 = bottomLoc;
                    timelineTick.X2 = xLocation;
                    timelineTick.Y2 = bottomLoc - majorTickHeight;

                    if (isAtEndOfTimeline)
                        continue;

                    TimeSpan timeSpan = TimeSpan.FromSeconds(i * minorTickDuration);
                    TextBlock timestampText = new TextBlock()
                    {
                        Margin = new Thickness(xLocation + 2, 0, 0, 0),
                        FontFamily = this.FontFamily,
                        FontStyle = this.FontStyle,
                        FontWeight = this.FontWeight,
                        FontStretch = this.FontStretch,
                        FontSize = this.FontSize,
                        Foreground = this.Foreground,
                        Text = (timeSpan.TotalHours >= 1.0d) ? string.Format(@"{0:hh\:mm\:ss}", timeSpan) : string.Format(@"{0:mm\:ss}", timeSpan)
                    };
                    timestampTextBlocks.Add(timestampText);
                    timelineCanvas.Children.Add(timestampText);
                    UpdateLayout(); // Needed so that we know the width of the textblock.
                }
                else // If still on the text block, draw a minor tick mark instead of a major.
                {
                    timelineTick.X1 = xLocation;
                    timelineTick.Y1 = bottomLoc;
                    timelineTick.X2 = xLocation;
                    timelineTick.Y2 = bottomLoc - minorTickHeight;
                }
            }
            else // Draw small ticks
            {
                double xLocation = ((i * minorTickDuration) / soundPlayer.ChannelLength) * timelineCanvas.RenderSize.Width;
                timelineTick.X1 = xLocation;
                timelineTick.Y1 = bottomLoc;
                timelineTick.X2 = xLocation;
                timelineTick.Y2 = bottomLoc - minorTickHeight;
            }
            timeLineTicks.Add(timelineTick);
            timelineCanvas.Children.Add(timelineTick);
        }
    }

    private void CreateProgressIndicator()
    {
        if (soundPlayer == null || timelineCanvas == null || progressCanvas == null)
            return;

        const double xLocation = 0.0d;

        progressLine.X1 = xLocation;
        progressLine.X2 = xLocation;
        progressLine.Y1 = timelineCanvas.RenderSize.Height;
        progressLine.Y2 = progressCanvas.RenderSize.Height;

        PolyLineSegment indicatorPolySegment = new PolyLineSegment();
        indicatorPolySegment.Points.Add(new Point(xLocation, timelineCanvas.RenderSize.Height));
        indicatorPolySegment.Points.Add(new Point(xLocation - indicatorTriangleWidth, timelineCanvas.RenderSize.Height - indicatorTriangleWidth));
        indicatorPolySegment.Points.Add(new Point(xLocation + indicatorTriangleWidth, timelineCanvas.RenderSize.Height - indicatorTriangleWidth));
        indicatorPolySegment.Points.Add(new Point(xLocation, timelineCanvas.RenderSize.Height));
        PathGeometry indicatorGeometry = new PathGeometry();
        PathFigure indicatorFigure = new PathFigure();
        indicatorFigure.Segments.Add(indicatorPolySegment);
        indicatorGeometry.Figures.Add(indicatorFigure);

        progressIndicator.Data = indicatorGeometry;
        UpdateProgressIndicator();
    }

    private void UpdateProgressIndicator()
    {
        if (soundPlayer == null || progressCanvas == null)
            return;

        double xLocation = 0.0d;
        if (soundPlayer.ChannelLength != 0)
        {
            double progressPercent = soundPlayer.ChannelPosition / soundPlayer.ChannelLength;
            xLocation = progressPercent * progressCanvas.RenderSize.Width;
        }
        progressLine.Margin = new Thickness(xLocation, 0, 0, 0);
        progressIndicator.Margin = new Thickness(xLocation, 0, 0, 0);
    }

    private void UpdateWaveform()
    {
        const double minValue = 0;
        const double maxValue = 1.5;
        const double dbScale = (maxValue - minValue);

        if (soundPlayer == null || soundPlayer.WaveformData == null || waveformCanvas == null ||
            waveformCanvas.RenderSize.Width < 1 || waveformCanvas.RenderSize.Height < 1)
            return;

        double leftRenderHeight;
        double rightRenderHeight;

        int pointCount = (int)(soundPlayer.WaveformData.Length / 2.0d);
        double pointThickness = waveformCanvas.RenderSize.Width / pointCount;
        double waveformSideHeight = waveformCanvas.RenderSize.Height / 2.0d;
        double centerHeight = waveformSideHeight;

        if (CenterLineBrush != null)
        {
            centerLine.X1 = 0;
            centerLine.X2 = waveformCanvas.RenderSize.Width;
            centerLine.Y1 = centerHeight;
            centerLine.Y2 = centerHeight;
        }

        if (soundPlayer.WaveformData != null && soundPlayer.WaveformData.Length > 1)
        {
            PolyLineSegment leftWaveformPolyLine = new PolyLineSegment();
            leftWaveformPolyLine.Points.Add(new Point(0, centerHeight));

            PolyLineSegment rightWaveformPolyLine = new PolyLineSegment();
            rightWaveformPolyLine.Points.Add(new Point(0, centerHeight));

            double xLocation = 0.0d;
            for (int i = 0; i < soundPlayer.WaveformData.Length; i += 2)
            {
                xLocation = (i / 2) * pointThickness;
                leftRenderHeight = ((soundPlayer.WaveformData[i] - minValue) / dbScale) * waveformSideHeight;
                leftWaveformPolyLine.Points.Add(new Point(xLocation, centerHeight - leftRenderHeight));
                rightRenderHeight = ((soundPlayer.WaveformData[i + 1] - minValue) / dbScale) * waveformSideHeight;
                rightWaveformPolyLine.Points.Add(new Point(xLocation, centerHeight + rightRenderHeight));
            }

            leftWaveformPolyLine.Points.Add(new Point(xLocation, centerHeight));
            leftWaveformPolyLine.Points.Add(new Point(0, centerHeight));
            rightWaveformPolyLine.Points.Add(new Point(xLocation, centerHeight));
            rightWaveformPolyLine.Points.Add(new Point(0, centerHeight));

            PathGeometry leftGeometry = new PathGeometry();
            PathFigure leftPathFigure = new PathFigure();
            leftPathFigure.Segments.Add(leftWaveformPolyLine);
            leftGeometry.Figures.Add(leftPathFigure);
            PathGeometry rightGeometry = new PathGeometry();
            PathFigure rightPathFigure = new PathFigure();
            rightPathFigure.Segments.Add(rightWaveformPolyLine);
            rightGeometry.Figures.Add(rightPathFigure);

            leftPath.Data = leftGeometry;
            rightPath.Data = rightGeometry;
        }
        else
        {
            leftPath.Data = null;
            rightPath.Data = null;
        }
    }
}
