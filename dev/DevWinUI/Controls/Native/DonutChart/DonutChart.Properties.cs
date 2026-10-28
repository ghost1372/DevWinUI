using System.Collections.Generic;
using Microsoft.UI.Xaml;

namespace DevWinUI;

public sealed partial class DonutChart
{
    public IEnumerable<DonutChartDatum>? Data
    {
        get { return (IEnumerable<DonutChartDatum>?)GetValue(DataProperty); }
        set { SetValue(DataProperty, value); }
    }

    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register(nameof(Data), typeof(IEnumerable<DonutChartDatum>), typeof(DonutChart), new PropertyMetadata(null, OnDataChanged));

    public double Diameter
    {
        get { return (double)GetValue(DiameterProperty); }
        set { SetValue(DiameterProperty, value); }
    }

    public static readonly DependencyProperty DiameterProperty =
        DependencyProperty.Register(nameof(Diameter), typeof(double), typeof(DonutChart), new PropertyMetadata(220d, OnGeometryChanged));

    public double Thickness
    {
        get { return (double)GetValue(ThicknessProperty); }
        set { SetValue(ThicknessProperty, value); }
    }

    public static readonly DependencyProperty ThicknessProperty =
        DependencyProperty.Register(nameof(Thickness), typeof(double), typeof(DonutChart), new PropertyMetadata(28d, OnGeometryChanged));

    public double Gap
    {
        get { return (double)GetValue(GapProperty); }
        set { SetValue(GapProperty, value); }
    }

    public static readonly DependencyProperty GapProperty =
        DependencyProperty.Register(nameof(Gap), typeof(double), typeof(DonutChart), new PropertyMetadata(0.03d, OnGeometryChanged));

    public double SliceCornerRadius
    {
        get { return (double)GetValue(SliceCornerRadiusProperty); }
        set { SetValue(SliceCornerRadiusProperty, value); }
    }

    public static readonly DependencyProperty SliceCornerRadiusProperty =
        DependencyProperty.Register(nameof(SliceCornerRadius), typeof(double), typeof(DonutChart), new PropertyMetadata(4d, OnGeometryChanged));

    public string CenterLabel
    {
        get { return (string)GetValue(CenterLabelProperty); }
        set { SetValue(CenterLabelProperty, value); }
    }

    public static readonly DependencyProperty CenterLabelProperty =
        DependencyProperty.Register(nameof(CenterLabel), typeof(string), typeof(DonutChart), new PropertyMetadata("Total"));

    public bool ShowLegend
    {
        get { return (bool)GetValue(ShowLegendProperty); }
        set { SetValue(ShowLegendProperty, value); }
    }

    public static readonly DependencyProperty ShowLegendProperty =
        DependencyProperty.Register(nameof(ShowLegend), typeof(bool), typeof(DonutChart), new PropertyMetadata(true, OnShowLegendChanged));

    public double Progress
    {
        get { return (double)GetValue(ProgressProperty); }
        set { SetValue(ProgressProperty, value); }
    }

    public static readonly DependencyProperty ProgressProperty =
        DependencyProperty.Register(nameof(Progress), typeof(double), typeof(DonutChart), new PropertyMetadata(0d, OnProgressChanged));
}
