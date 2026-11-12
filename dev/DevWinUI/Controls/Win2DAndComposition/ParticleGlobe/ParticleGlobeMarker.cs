namespace DevWinUI;

public sealed partial class ParticleGlobeMarker
{
    public ParticleGlobeMarker() { }

    public ParticleGlobeMarker(double latitude, double longitude, string label = null, Color? color = null)
    {
        Latitude = latitude;
        Longitude = longitude;
        Label = label;
        Color = color;
    }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Label { get; set; }

    public Color? Color { get; set; }

    public object Tag { get; set; }

    internal Vector3 ToVector()
    {
        float phi = (float)(Math.Clamp(Latitude, -90.0, 90.0) * Math.PI / 180.0);
        float lam = (float)(Longitude * Math.PI / 180.0);
        return new Vector3(MathF.Cos(phi) * MathF.Sin(lam), MathF.Sin(phi), MathF.Cos(phi) * MathF.Cos(lam));
    }

    public static ObservableCollection<ParticleGlobeMarker> CreateDefaults() => new()
    {
        new(40.7, -74.0, "New York"),   new(51.5, -0.1, "London"),     new(50.1, 8.7, "Frankfurt"),   new(35.7, 139.7, "Tokyo"),
        new(-33.9, 151.2, "Sydney"),    new(-23.5, -46.6, "São Paulo"), new(-33.9, 18.4, "Cape Town"), new(19.1, 72.9, "Mumbai"),
        new(1.35, 103.8, "Singapore"),  new(25.2, 55.3, "Dubai"),      new(34.0, -118.2, "Los Angeles"), new(55.8, 37.6, "Moscow"),
        new(6.5, 3.4, "Lagos"),
                new(35.69, 51.39, "Tehran"),
    };
}
