namespace DevWinUI;

public partial class ConfettiCannonOptions
{
    private static readonly List<string> DefaultColors =
    [
        "#26ccff",
        "#a25afd",
        "#ff5e7e",
        "#88ff5a",
        "#fcff42",
        "#ffa62d",
        "#ff36ff"
    ];

    public int ParticleCount { get; set; } = 50;

    public double Angle { get; set; } = 90;

    public double Spread { get; set; } = 45;

    public double StartVelocity { get; set; } = 45;

    public double Decay { get; set; } = 0.9;

    public double Gravity { get; set; } = 1;

    public double Drift { get; set; }

    public int Ticks { get; set; } = 200;

    public Point Origin { get; set; } = new(0.5, 0.5);

    public List<string> Shapes { get; set; } = new() { "square", "circle" };

    public List<Color> Colors { get; set; }

    public double Scalar { get; set; } = 1;

    public bool Flat { get; set; }

    public double? OvalScalar { get; set; } // for circle

    public List<Color> GetFinalColors()
    {
        if (Colors is { Count: > 0 })
            return Colors;

        return DefaultColors.Select(ColorHelper.GetColorFromHex).ToList();
    }
}
