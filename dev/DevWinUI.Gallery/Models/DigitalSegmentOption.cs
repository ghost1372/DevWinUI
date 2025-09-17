namespace DevWinUIGallery.Models;

public class DigitalSegmentOption
{
    public string Name { get; set; }
    public SegmentChar Value { get; set; }

    public override string ToString() => Name;
}
