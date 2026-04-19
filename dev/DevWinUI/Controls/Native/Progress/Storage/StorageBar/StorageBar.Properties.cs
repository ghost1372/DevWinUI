// Copyright (c) Files Community
// Licensed under the MIT License.

namespace DevWinUI;

public partial class StorageBar
{
	public double ValueBarHeight
    {
		get { return (double)GetValue(ValueBarHeightProperty); }
		set { SetValue(ValueBarHeightProperty, value); }
	}

	public static readonly DependencyProperty ValueBarHeightProperty =
		DependencyProperty.Register(nameof(ValueBarHeight), typeof(double), typeof(StorageBar), new PropertyMetadata(6.0, OnDPChanged));

    private static void OnDPChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StorageBar)d;
		    if (ctl != null)
		    {
			    ctl.UpdateControl();
        }
    }

    public double TrackBarHeight
    {
		get { return (double)GetValue(TrackBarHeightProperty); }
		set { SetValue(TrackBarHeightProperty, value); }
	}

	public static readonly DependencyProperty TrackBarHeightProperty =
		DependencyProperty.Register(nameof(TrackBarHeight), typeof(double), typeof(StorageBar), new PropertyMetadata(3.0, OnDPChanged));

	public BarShapes BarShape
    {
		get { return (BarShapes)GetValue(BarShapeProperty); }
		set { SetValue(BarShapeProperty, value); }
	}

	public static readonly DependencyProperty BarShapeProperty =
		DependencyProperty.Register(nameof(BarShape), typeof(BarShapes), typeof(StorageBar), new PropertyMetadata(BarShapes.Round, OnDPChanged));

	public double Percent
    {
		get { return (double)GetValue(PercentProperty); }
		set { SetValue(PercentProperty, value); }
	}

	public static readonly DependencyProperty PercentProperty =
		DependencyProperty.Register(nameof(Percent), typeof(double), typeof(StorageBar), new PropertyMetadata(0.0));

	public double PercentCaution
    {
		get { return (double)GetValue(PercentCautionProperty); }
		set { SetValue(PercentCautionProperty, value); }
	}

	public static readonly DependencyProperty PercentCautionProperty =
		DependencyProperty.Register(nameof(PercentCaution), typeof(double), typeof(StorageBar), new PropertyMetadata(75.1, OnDPChanged));

	public double PercentCritical
    {
		get { return (double)GetValue(PercentCriticalProperty); }
		set { SetValue(PercentCriticalProperty, value); }
	}

	public static readonly DependencyProperty PercentCriticalProperty =
		DependencyProperty.Register(nameof(PercentCritical), typeof(double), typeof(StorageBar), new PropertyMetadata(89.9, OnDPChanged));

	/// <inheritdoc/>
	protected override void OnValueChanged(double oldValue, double newValue)
	{
		base.OnValueChanged(oldValue, newValue);

		UpdateValue(Value, _oldValue, false, -1.0);
	}

	/// <inheritdoc/>
	protected override void OnMaximumChanged(double oldValue, double newValue)
	{
		base.OnMaximumChanged(oldValue, newValue);

		UpdateValue(oldValue, newValue, false, -1.0);
	}

	/// <inheritdoc/>
	protected override void OnMinimumChanged(double oldValue, double newValue)
	{
		base.OnMinimumChanged(oldValue, newValue);

		UpdateValue(oldValue, newValue, false, -1.0);
	}
}
