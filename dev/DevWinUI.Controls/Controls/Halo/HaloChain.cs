//https://github.com/benthorner/radial_controls

namespace DevWinUI;

public partial class HaloChain : HaloRing
{
    public double Tension
    {
        get { return (double)GetValue(TensionProperty); }
        set { SetValue(TensionProperty, value); }
    }
    public static readonly DependencyProperty TensionProperty =
        DependencyProperty.Register(nameof(Tension), typeof(double), typeof(HaloChain), new PropertyMetadata(0.5, Refresh));

    public double Angle
    {
        get { return (double)GetValue(AngleProperty); }
        set { SetValue(AngleProperty, value); }
    }
    public static new readonly DependencyProperty AngleProperty =
        DependencyProperty.Register(nameof(Angle), typeof(double), typeof(HaloChain), new PropertyMetadata(0.0, Refresh));

    public double Offset
    {
        get { return (double)GetValue(OffsetProperty); }
        set { SetValue(OffsetProperty, value); }
    }
    public static new readonly DependencyProperty OffsetProperty =
        DependencyProperty.Register(nameof(Offset), typeof(double), typeof(HaloChain), new PropertyMetadata(0.0, Refresh));

    public double Spacing
    {
        get { return (double)GetValue(SpacingProperty); }
        set { SetValue(SpacingProperty, value); }
    }
    public static readonly DependencyProperty SpacingProperty =
        DependencyProperty.Register(nameof(Spacing), typeof(double), typeof(HaloChain), new PropertyMetadata(0.0, Refresh));

    protected override Size ArrangeOverride(Size finalSize)
    {
        var radius = Math.Min(finalSize.Width, finalSize.Height) / 2;

        var angle = -(Tension % 1) * TotalAngle(radius) + Angle;

        foreach (var link in Children)
        {
            angle += EnterAngle(link, radius);
            link.SetValue(HaloRing.AngleProperty, angle);

            angle += ExitAngle(link, radius);
            link.SetValue(HaloRing.OffsetProperty, Offset);
        }

        return base.ArrangeOverride(new Size(radius * 2, radius * 2));
    }

    private static void Refresh(object o, DependencyPropertyChangedEventArgs e)
    {
        ((HaloChain)o).InvalidateMeasure();
        ((HaloChain)o).UpdateLayout();
    }

    private double TotalAngle(double radius)
    {
        return Children.Sum(link =>
        {
            return EnterAngle(link, radius) + ExitAngle(link, radius);
        });
    }

    private double EnterAngle(UIElement link, double radius)
    {
        if (Children.First() == link) return 0.0;
        return HalfAngle(link.DesiredSize, radius) + Spacing / 2;
    }

    private double ExitAngle(UIElement link, double radius)
    {
        if (Children.Last() == link) return 0.0;
        return HalfAngle(link.DesiredSize, radius) + Spacing / 2;
    }

    private double HalfAngle(Size size, double radius)
    {
        var thickness = (double)GetValue(Halo.ThicknessProperty);

        var width = new HaloVector(
            Math.Cos(Offset.ToRadians()) * size.Width,
            Math.Sin(Offset.ToRadians()) * size.Height
        ).Length;

        var height = new HaloVector(
            Math.Sin(Offset.ToRadians()) * size.Width,
            Math.Cos(Offset.ToRadians()) * size.Height
        ).Length;

        return Math.Atan2(
            width / 2, radius - thickness / 2 - height / 2
        ).ToDegrees();
    }
}
