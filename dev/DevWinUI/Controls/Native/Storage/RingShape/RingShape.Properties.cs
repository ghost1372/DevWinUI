// Copyright (c) Files Community
// Licensed under the MIT License.

namespace DevWinUI;

public partial class RingShape
{
	public double StartAngle
	{
		get { return (double)GetValue(StartAngleProperty); }
		set { SetValue(StartAngleProperty, value); }
	}

	public static readonly DependencyProperty StartAngleProperty =
		DependencyProperty.Register(nameof(StartAngle), typeof(double), typeof(RingShape), new PropertyMetadata(0.0, OnStartAngleChanged));

    private static void OnStartAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RingShape)d;
		    if (ctl != null)
		    {
			    ctl.StartAngleChanged();
		    }
    }

    public double EndAngle
    {
		    get { return (double)GetValue(EndAngleProperty); }
		    set { SetValue(EndAngleProperty, value); }
	    }

	    public static readonly DependencyProperty EndAngleProperty =
		    DependencyProperty.Register(nameof(EndAngle), typeof(double), typeof(RingShape), new PropertyMetadata(90.0, OnEndAngleChanged));

    private static void OnEndAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RingShape)d;
        if (ctl != null)
        {
            ctl.EndAngleChanged();
        }
    }

    public SweepDirection SweepDirection
    {
		    get { return (SweepDirection)GetValue(SweepDirectionProperty); }
		    set { SetValue(SweepDirectionProperty, value); }
	    }

	    public static readonly DependencyProperty SweepDirectionProperty =
		    DependencyProperty.Register(nameof(SweepDirection), typeof(SweepDirection), typeof(RingShape), new PropertyMetadata(SweepDirection.Clockwise, OnSweepDirectionChanged));

    private static void OnSweepDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RingShape)d;
        if (ctl != null)
        {
            ctl.SweepDirectionChanged();
        }
    }

    public double MinAngle
    {
		    get { return (double)GetValue(MinAngleProperty); }
		    set { SetValue(MinAngleProperty, value); }
	    }

	    public static readonly DependencyProperty MinAngleProperty =
		    DependencyProperty.Register(nameof(MinAngle), typeof(double), typeof(RingShape), new PropertyMetadata(0.0, OnMinAngleChanged));

    private static void OnMinAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RingShape)d;
        if (ctl != null)
        {
            ctl.MinMaxAngleChanged(false);
        }
    }

    public double MaxAngle
    {
		    get { return (double)GetValue(MaxAngleProperty); }
		    set { SetValue(MaxAngleProperty, value); }
	    }

	    public static readonly DependencyProperty MaxAngleProperty =
		    DependencyProperty.Register(nameof(MaxAngle), typeof(double), typeof(RingShape), new PropertyMetadata(360.0, OnMaxAngleChanged));

    private static void OnMaxAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RingShape)d;
        if (ctl != null)
        {
            ctl.MinMaxAngleChanged(true);
        }
    }

    public double RadiusWidth
    {
		    get { return (double)GetValue(RadiusWidthProperty); }
		    set { SetValue(RadiusWidthProperty, value); }
	    }

	    public static readonly DependencyProperty RadiusWidthProperty =
		    DependencyProperty.Register(nameof(RadiusWidth), typeof(double), typeof(RingShape), new PropertyMetadata(0.0, OnRadiusHeightChanged));

    private static void OnRadiusHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RingShape)d;
        if (ctl != null)
        {
            ctl.RadiusHeightChanged();
        }
    }

    public double RadiusHeight
    {
		    get { return (double)GetValue(RadiusHeightProperty); }
		    set { SetValue(RadiusHeightProperty, value); }
	    }

	    public static readonly DependencyProperty RadiusHeightProperty =
		    DependencyProperty.Register(nameof(RadiusHeight), typeof(double), typeof(RingShape), new PropertyMetadata(0.0, OnRadiusWidthChanged));

    private static void OnRadiusWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RingShape)d;
        if (ctl != null)
        {
            ctl.RadiusWidthChanged();
        }
    }

    public bool IsCircle
    {
		    get { return (bool)GetValue(IsCircleProperty); }
		    set { SetValue(IsCircleProperty, value); }
	    }

	    public static readonly DependencyProperty IsCircleProperty =
		    DependencyProperty.Register(nameof(IsCircle), typeof(bool), typeof(RingShape), new PropertyMetadata(false, OnIsCircleChanged));

    private static void OnIsCircleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RingShape)d;
        if (ctl != null)
        {
            ctl.IsCircleChanged();
        }
    }

    public Point Center
    {
		get { return (Point)GetValue(CenterProperty); }
		set { SetValue(CenterProperty, value); }
	}

	public static readonly DependencyProperty CenterProperty =
		DependencyProperty.Register(nameof(Center), typeof(Point), typeof(RingShape), new PropertyMetadata(new Point()));

	public double ActualRadiusWidth
{
		get { return (double)GetValue(ActualRadiusWidthProperty); }
		set { SetValue(ActualRadiusWidthProperty, value); }
	}

	public static readonly DependencyProperty ActualRadiusWidthProperty =
		DependencyProperty.Register(nameof(ActualRadiusWidth), typeof(double), typeof(RingShape), new PropertyMetadata(0.0));

	public double ActualRadiusHeight
{
		get { return (double)GetValue(ActualRadiusHeightProperty); }
		set { SetValue(ActualRadiusHeightProperty, value); }
	}

	public static readonly DependencyProperty ActualRadiusHeightProperty =
		DependencyProperty.Register(nameof(ActualRadiusHeight), typeof(double), typeof(RingShape), new PropertyMetadata(0.0));
}
