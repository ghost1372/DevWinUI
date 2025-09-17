namespace DevWinUI;

public partial class Matrix8x14SegmentChar : MatrixSegmentChar
{
    protected override int MatrixColumns => 8;
    protected override int MatrixRows => 14;
    protected override IReadOnlyDictionary<string, string> MatrixPatternTable => StandardMatrix8x14Pattern.Patterns;
    protected override string MatrixDefaultPattern => StandardMatrix8x14Pattern.DefaultPattern;

    public override SegmentChar Clone() => new Matrix8x14SegmentChar();
}
