using Windows.UI;

namespace DevWinUI;

public sealed partial class DonutChartDatum
{
    public DonutChartDatum()
    {
    }

    public DonutChartDatum(string label, double value, Color? color = null)
    {
        Label = label;
        Value = value;
        Color = color;
    }

    public string Label { get; set; } = string.Empty;

    public double Value { get; set; }

    public Color? Color { get; set; }
}
