using Microsoft.UI.Xaml.Shapes;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

public sealed partial class SixteenSegmentChar : SegmentChar
{
    protected override IReadOnlyDictionary<string, string> PatternTable => StandardSixteenSegmentPattern.Patterns;
    protected override string DefaultPattern => StandardSixteenSegmentPattern.DefaultPattern;

    public SixteenSegmentChar()
    {
        DefaultStyleKey = typeof(SixteenSegmentChar);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        CollectSegments();
        UpdateCharacter();
        InitColonAnimation();
        if (IsPatternMakerEnabled)
        {
            RegisterPointerEvents();
        }
    }

    protected override void CollectSegments()
    {
        Segments = new()
        {
            ("A1", GetTemplateChild("PART_SegmentA1") as Path),
            ("A2", GetTemplateChild("PART_SegmentA2") as Path),
            ("B",  GetTemplateChild("PART_SegmentB") as Path),
            ("C", GetTemplateChild("PART_SegmentC") as Path),
            ("D1", GetTemplateChild("PART_SegmentD1") as Path),
            ("D2", GetTemplateChild("PART_SegmentD2") as Path),
            ("E", GetTemplateChild("PART_SegmentE") as Path),
            ("F", GetTemplateChild("PART_SegmentF") as Path),
            ("G1", GetTemplateChild("PART_SegmentG1") as Path),
            ("G2", GetTemplateChild("PART_SegmentG2") as Path),
            ("H", GetTemplateChild("PART_SegmentH") as Path),
            ("I", GetTemplateChild("PART_SegmentI") as Path),
            ("J", GetTemplateChild("PART_SegmentJ") as Path),
            ("K", GetTemplateChild("PART_SegmentK") as Path),
            ("L", GetTemplateChild("PART_SegmentL") as Path),
            ("M", GetTemplateChild("PART_SegmentM") as Path),
            ("DP1", GetTemplateChild("PART_SegmentDP1") as Ellipse),
            ("DP2", GetTemplateChild("PART_SegmentDP2") as Ellipse),
            (DigitalSegmentHelper.COLON_TOP_KEY, GetTemplateChild("PART_SegmentColon1") as Ellipse),
            (DigitalSegmentHelper.COLON_BOTTOM_KEY, GetTemplateChild("PART_SegmentColon2") as Ellipse),
        };
    }

    public override SegmentChar Clone()
    {
        return new SixteenSegmentChar();
    }
}
