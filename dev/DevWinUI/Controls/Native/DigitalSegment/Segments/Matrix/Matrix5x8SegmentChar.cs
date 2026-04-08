namespace DevWinUI;

public partial class Matrix5x8SegmentChar : MatrixSegmentChar
{
    protected override int MatrixColumns => 5;
    protected override int MatrixRows => 8;
    protected override IReadOnlyDictionary<string, string> MatrixPatternTable => StandardMatrix5x8Pattern.Patterns;
    protected override string MatrixDefaultPattern => StandardMatrix5x8Pattern.DefaultPattern;

    public override SegmentChar Clone() => new Matrix5x8SegmentChar();

    public Matrix5x8SegmentChar()
    {
        DefaultStyleKey = typeof(Matrix5x8SegmentChar);
    }
}
