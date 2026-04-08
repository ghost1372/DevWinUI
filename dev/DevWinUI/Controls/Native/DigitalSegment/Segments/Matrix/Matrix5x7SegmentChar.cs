namespace DevWinUI;

public partial class Matrix5x7SegmentChar : MatrixSegmentChar
{
    protected override int MatrixColumns => 5;
    protected override int MatrixRows => 7;
    protected override IReadOnlyDictionary<string, string> MatrixPatternTable => StandardMatrix5x7Pattern.Patterns;
    protected override string MatrixDefaultPattern => StandardMatrix5x7Pattern.DefaultPattern;

    public override SegmentChar Clone() => new Matrix5x7SegmentChar();

    public Matrix5x7SegmentChar()
    {
        DefaultStyleKey = typeof(Matrix5x7SegmentChar);
    }
}
