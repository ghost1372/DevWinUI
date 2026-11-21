namespace DevWinUIGallery.Models;

public class DigitalSegmentOption
{
    public string Name { get; set; } = string.Empty;
    public SegmentChar Value { get; set; } = null!;

    public override string ToString() => Name;
}
