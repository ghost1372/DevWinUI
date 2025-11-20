using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_PathFigure), Type = typeof(PathFigure))]
[TemplatePart(Name = nameof(PART_ArcSegment), Type = typeof(ArcSegment))]
[TemplatePart(Name = nameof(PART_LineSegment), Type = typeof(LineSegment))]
[TemplatePart(Name = nameof(PART_Elipse), Type = typeof(Ellipse))]
public partial class ArcProgress : Control
{
    private const string PART_PathFigure = "PART_PathFigure";
    private const string PART_ArcSegment = "PART_ArcSegment";
    private const string PART_LineSegment = "PART_LineSegment";
    private const string PART_Elipse = "PART_Elipse";

    private Storyboard storyboard;
    private PathFigure pathFigure;
    private ArcSegment arcSegment;
    private LineSegment lineSegment;
    private Ellipse ellipse;

    private double _radius;
    private double _centerX;
    private double _centerY;

    public ArcProgressInitialPosition InitialPosition
    {
        get { return (ArcProgressInitialPosition)GetValue(InitialPositionProperty); }
        set { SetValue(InitialPositionProperty, value); }
    }

    public static readonly DependencyProperty InitialPositionProperty =
        DependencyProperty.Register(nameof(InitialPosition), typeof(ArcProgressInitialPosition), typeof(ArcProgress), new PropertyMetadata(ArcProgressInitialPosition.Top));

    public ArcProgressFillAnimationState FillStatus
    {
        get { return (ArcProgressFillAnimationState)GetValue(FillStatusProperty); }
        set { SetValue(FillStatusProperty, value); }
    }

    public static readonly DependencyProperty FillStatusProperty =
        DependencyProperty.Register(nameof(FillStatus), typeof(ArcProgressFillAnimationState), typeof(ArcProgress), new PropertyMetadata(ArcProgressFillAnimationState.UnFilling, OnFillStatusChanged));

    private static void OnFillStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ArcProgress)d;
        ctl?.UpdateDirection();
    }

    public SweepDirection SweepDirection
    {
        get { return (SweepDirection)GetValue(DirectionProperty); }
        set { SetValue(DirectionProperty, value); }
    }

    public static readonly DependencyProperty DirectionProperty =
        DependencyProperty.Register(nameof(SweepDirection), typeof(SweepDirection), typeof(ArcProgress), new PropertyMetadata(SweepDirection.Clockwise, OnDirectionChanged));

    private static void OnDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ArcProgress)d;
        ctl?.UpdateDirection();
    }

    private void UpdateDirection()
    {
        switch (FillStatus)
        {
            case ArcProgressFillAnimationState.Filling:
                FlowDirection = SweepDirection == SweepDirection.Clockwise ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;
                break;
            case ArcProgressFillAnimationState.UnFilling:
                FlowDirection = SweepDirection == SweepDirection.Clockwise ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                break;
        }
    }

    internal double InternalPercentage
    {
        get { return (double)GetValue(InternalPercentageProperty); }
        set { SetValue(InternalPercentageProperty, value); }
    }

    internal static readonly DependencyProperty InternalPercentageProperty =
        DependencyProperty.Register(nameof(InternalPercentage), typeof(double), typeof(ArcProgress), new PropertyMetadata(0.0, OnInternalPercentageChanged));

    private static void OnInternalPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ArcProgress)d;
        if (ctl != null)
        {
            ctl.DrawArcProgress((double)e.NewValue);
        }
    }

    public double Percentage
    {
        get { return (double)GetValue(PercentageProperty); }
        set { SetValue(PercentageProperty, value); }
    }

    public static readonly DependencyProperty PercentageProperty =
        DependencyProperty.Register(nameof(Percentage), typeof(double), typeof(ArcProgress), new PropertyMetadata(100.0, OnPercentageChanged));

    private static void OnPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ArcProgress)d;
        if (ctl != null)
        {
            ctl.DrawArcProgress((double)e.NewValue);
        }
    }
    public ArcProgress()
    {
        DefaultStyleKey = typeof(ArcProgress);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        pathFigure = GetTemplateChild(PART_PathFigure) as PathFigure;
        arcSegment = GetTemplateChild(PART_ArcSegment) as ArcSegment;
        lineSegment = GetTemplateChild(PART_LineSegment) as LineSegment;
        ellipse = GetTemplateChild(PART_Elipse) as Ellipse;

        UpdateDirection();
        DrawArcProgress(Percentage);

        Loaded -= OnLoaded;
        Loaded += OnLoaded;

        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        DrawArcProgress(Percentage);
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _centerX = ActualWidth / 2;
        _centerY = ActualHeight / 2;
        _radius = Math.Min(_centerX, _centerY);
    }

    public void StartFillAnimation(TimeSpan duration)
    {
        double startValue = FillStatus == ArcProgressFillAnimationState.Filling ? 0 : 100;
        double endValue = FillStatus == ArcProgressFillAnimationState.Filling ? 100 : 0;

        var animation = new DoubleAnimation
        {
            From = startValue,
            To = endValue,
            Duration = new Duration(duration),
            EnableDependentAnimation = true
        };

        animation.Completed += (s, e) =>
        {
            InternalPercentage = endValue;
        };

        storyboard = new Storyboard();
        Storyboard.SetTarget(animation, this);
        Storyboard.SetTargetProperty(animation, "InternalPercentage");

        storyboard.Children.Add(animation);
        storyboard.Begin();
    }
    public void PauseFillAnimation()
    {
        if (storyboard != null)
        {
            storyboard.Pause();
        }
    }

    public void ResumeFillAnimation()
    {
        if (storyboard != null)
        {
            storyboard.Resume();
        }
    }
    public void DrawArcProgress(double percentage)
    {
        double angle = Math.Min(Math.Max(percentage, 0), 100) * 3.6;
        DrawArcSlice(angle);
    }

    public void DrawArcSlice(double angle)
    {
        if (pathFigure == null || arcSegment == null || lineSegment == null || ellipse == null)
        {
            return;
        }

        if (angle >= 360)
        {
            angle = 359;
            ellipse.Visibility = Visibility.Visible;
        }
        else
        {
            ellipse.Visibility = Visibility.Collapsed;
        }

        double startAngle = 0;

        switch (InitialPosition)
        {
            case ArcProgressInitialPosition.Top:
                startAngle = 0;
                break;
            case ArcProgressInitialPosition.MiddleRight:
                startAngle = 90;
                break;
            case ArcProgressInitialPosition.Bottom:
                startAngle = 180;
                break;
            case ArcProgressInitialPosition.MiddleLeft:
                startAngle = 270;
                break;
        }

        double endAngle = startAngle + angle;

        double startRadians = startAngle * Math.PI / 180.0;
        double endRadians = endAngle * Math.PI / 180.0;

        double startX = _centerX + _radius * Math.Sin(startRadians);
        double startY = _centerY - _radius * Math.Cos(startRadians);
        double endX = _centerX + _radius * Math.Sin(endRadians);
        double endY = _centerY - _radius * Math.Cos(endRadians);

        pathFigure.StartPoint = new Windows.Foundation.Point(startX, startY);
        arcSegment.Point = new Windows.Foundation.Point(endX, endY);
        arcSegment.Size = new Windows.Foundation.Size(_radius, _radius);
        arcSegment.IsLargeArc = angle > 180;
        arcSegment.SweepDirection = SweepDirection.Clockwise;
        lineSegment.Point = new Windows.Foundation.Point(_centerX, _centerY);
    }
}
