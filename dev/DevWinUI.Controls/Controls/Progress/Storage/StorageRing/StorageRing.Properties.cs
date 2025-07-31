// Copyright (c) Files Community
// Licensed under the MIT License.

namespace DevWinUI;

public partial class StorageRing
{
	public double ValueRingThickness
	{
		get { return (double)GetValue(ValueRingThicknessProperty); }
		set { SetValue(ValueRingThicknessProperty, value); }
	}

	public static readonly DependencyProperty ValueRingThicknessProperty =
		DependencyProperty.Register(nameof(ValueRingThickness), typeof(double), typeof(StorageRing), new PropertyMetadata(0.0, OnValueRingThicknessChanged));

    private static void OnValueRingThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StorageRing)d;
		    if (ctl != null)
		    {
            ctl.UpdateRingThickness((double)e.NewValue, false);
            ctl.UpdateRings();
        }
    }

    public double TrackRingThickness
    {
		get { return (double)GetValue(TrackRingThicknessProperty); }
		set { SetValue(TrackRingThicknessProperty, value); }
	}

	public static readonly DependencyProperty TrackRingThicknessProperty =
		DependencyProperty.Register(nameof(TrackRingThickness), typeof(double), typeof(StorageRing), new PropertyMetadata(0.0, OnTrackRingThicknessChanged));

    private static void OnTrackRingThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StorageRing)d;
        if (ctl != null)
        {
            ctl.UpdateRingThickness((double)e.NewValue, true);
            ctl.UpdateRings();
        }
    }

    public double MinAngle
    {
		get { return (double)GetValue(MinAngleProperty); }
		set { SetValue(MinAngleProperty, value); }
	}

	public static readonly DependencyProperty MinAngleProperty =
		DependencyProperty.Register(nameof(MinAngle), typeof(double), typeof(StorageRing), new PropertyMetadata(0.0, OnMinAngleChanged));

    private static void OnMinAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StorageRing)d;
        if (ctl != null)
        {
            ctl.UpdateValues(ctl.Value, ctl._oldValue, false, -1.0);
            ctl.UpdateNormalizedAngles((double)e.NewValue, ctl.MaxAngle);
            ctl.UpdateRings();
        }
    }

    public double MaxAngle
    {
		get { return (double)GetValue(MaxAngleProperty); }
		set { SetValue(MaxAngleProperty, value); }
	}

	public static readonly DependencyProperty MaxAngleProperty =
		DependencyProperty.Register(nameof(MaxAngle), typeof(double), typeof(StorageRing), new PropertyMetadata(360.0, OnMaxAngleChanged));

    private static void OnMaxAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StorageRing)d;
        if (ctl != null)
        {
            ctl.UpdateValues(ctl.Value, ctl._oldValue, false, -1.0);
            ctl.UpdateNormalizedAngles(ctl.MinAngle, (double)e.NewValue);
            ctl.UpdateRings();
        }
    }

    public double StartAngle
    {
		get { return (double)GetValue(StartAngleProperty); }
		set { SetValue(StartAngleProperty, value); }
	}

	public static readonly DependencyProperty StartAngleProperty =
		DependencyProperty.Register(nameof(StartAngle), typeof(double), typeof(StorageRing), new PropertyMetadata(0.0, OnStartAngleChanged));

    private static void OnStartAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StorageRing)d;
        if (ctl != null)
        {
            ctl.UpdateValues(ctl.Value, ctl._oldValue, false, -1.0);
            ctl.UpdateNormalizedAngles(ctl.MinAngle, (double)e.NewValue);
            ctl.ValidateStartAngle((double)e.NewValue);
            ctl.UpdateRings();
        }
    }

    public double Percent
    {
		get { return (double)GetValue(PercentProperty); }
		set { SetValue(PercentProperty, value); }
	}

	public static readonly DependencyProperty PercentProperty =
		DependencyProperty.Register(nameof(Percent), typeof(double), typeof(StorageRing), new PropertyMetadata(0.0));

	public double PercentCaution
    {
		get { return (double)GetValue(PercentCautionProperty); }
		set { SetValue(PercentCautionProperty, value); }
	}

	public static readonly DependencyProperty PercentCautionProperty =
		DependencyProperty.Register(nameof(PercentCaution), typeof(double), typeof(StorageRing), new PropertyMetadata(75.01, OnPercentCautionOrCriticalChanged));

    private static void OnPercentCautionOrCriticalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StorageRing)d;
        if (ctl != null)
        {
            ctl.UpdateValues(ctl.Value, ctl._oldValue, false, -1.0);
            ctl.UpdateVisualState();
            ctl.UpdateRings();
        }
    }

    public double PercentCritical
    {
		get { return (double)GetValue(PercentCriticalProperty); }
		set { SetValue(PercentCriticalProperty, value); }
	}

	public static readonly DependencyProperty PercentCriticalProperty =
		DependencyProperty.Register(nameof(PercentCritical), typeof(double), typeof(StorageRing), new PropertyMetadata(90.01, OnPercentCautionOrCriticalChanged));

	public double ValueAngle
    {
		get { return (double)GetValue(ValueAngleProperty); }
		set { SetValue(ValueAngleProperty, value); }
	}

	public static readonly DependencyProperty ValueAngleProperty =
		DependencyProperty.Register(nameof(ValueAngle), typeof(double), typeof(StorageRing), new PropertyMetadata(0.0));

	public double AdjustedSize
    {
		get { return (double)GetValue(AdjustedSizeProperty); }
		set { SetValue(AdjustedSizeProperty, value); }
	}

	public static readonly DependencyProperty AdjustedSizeProperty =
		DependencyProperty.Register(nameof(AdjustedSize), typeof(double), typeof(StorageRing), new PropertyMetadata(16.0));

	/// <inheritdoc/>
	protected override void OnValueChanged(double oldValue, double newValue)
	{
		base.OnValueChanged(oldValue, newValue);

		UpdateValues(newValue, oldValue, false, -1.0);
		UpdateRings();
	}

	/// <inheritdoc/>
	protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
	{
		base.OnMinimumChanged(oldMinimum, newMinimum);

		UpdateRings();
	}

	/// <inheritdoc/>
	protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
	{
		base.OnMaximumChanged(oldMaximum, newMaximum);

		UpdateRings();
	}
}
