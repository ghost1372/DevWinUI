namespace DevWinUI;

internal partial class HaloCircleHelper
{
    public HaloCircleHelper(Size size)
    {
        Radius = Math.Min(size.Width, size.Height) / 2;
        Center = new Point(size.Width / 2, size.Height / 2);
    }

    public double Radius { get; set; }
    public Point Center { get; set; }

    public Point PointAt(double angle)
    {
        return new Point(
            Center.X + Radius * Math.Sin(angle.ToRadians()),
            Center.Y - Radius * Math.Cos(angle.ToRadians())
        );
    }

    public Size Size()
    {
        return new Size(Radius, Radius);
    }
}
