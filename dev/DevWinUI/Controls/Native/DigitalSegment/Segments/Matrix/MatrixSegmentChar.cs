using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

public abstract partial class MatrixSegmentChar : SegmentChar
{
    private const string PART_Canvas = "PART_Canvas";
    private Canvas canvas;

    protected abstract int MatrixColumns { get; }
    protected abstract int MatrixRows { get; }
    protected abstract IReadOnlyDictionary<string, string> MatrixPatternTable { get; }
    protected abstract string MatrixDefaultPattern { get; }

    protected override int Columns { get; set; }
    protected override int Rows { get; set; }
    protected override IReadOnlyDictionary<string, string> PatternTable => MatrixPatternTable;
    protected override string DefaultPattern => MatrixDefaultPattern;

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        canvas = GetTemplateChild(PART_Canvas) as Canvas;

        Columns = MatrixColumns;
        Rows = MatrixRows;

        UpdateDraw();
        CollectSegments();
        UpdateCharacter();

        if (IsPatternMakerEnabled)
            RegisterPointerEvents();
    }

    public void UpdateDraw() => ReDrawMatrix();

    protected override void ReDrawMatrix()
    {
        if (canvas == null) return;

        double cellW = MatrixDotSize;
        double cellH = MatrixDotSize;
        double gap = MatrixDotGap;

        // Same lookup as UpdateCharacter (including the upper case fallback).
        var pattern = DigitalSegmentHelper.GetCharPattern(GetPatternForCharacter(Character), Columns, Rows);
        if (pattern == null)
        {
            canvas.Children.Clear();
            return;
        }

        // Reuse the existing shapes when only their appearance changed; only rebuild
        // when the cell count or the shape type is different (up to 112 shapes per character).
        int total = Columns * Rows;
        bool reuse = total > 0
            && canvas.Children.Count == total
            && (IsMatrixSquare ? canvas.Children[0] is Rectangle : canvas.Children[0] is Ellipse);

        if (!reuse)
            canvas.Children.Clear();

        int index = 0;
        for (int row = 0; row < Rows; row++)
        {
            byte line = pattern[row];
            for (int col = 0; col < Columns; col++)
            {
                bool isOn = ((line >> (Columns - 1 - col)) & 1) != 0;

                Shape shape;
                if (reuse)
                {
                    shape = (Shape)canvas.Children[index];
                }
                else
                {
                    shape = IsMatrixSquare ? new Rectangle() : new Ellipse();
                    canvas.Children.Add(shape);
                }

                shape.Width = cellW;
                shape.Height = cellH;
                shape.Stroke = Stroke;
                shape.StrokeThickness = StrokeThickness;
                shape.Fill = isOn ? SegmentForeground : SegmentBackground;

                Canvas.SetLeft(shape, col * (cellW + gap));
                Canvas.SetTop(shape, row * (cellH + gap));
                index++;
            }
        }

        canvas.Width = Columns * (cellW + gap);
        canvas.Height = Rows * (cellH + gap) - gap;
    }

    protected override void CollectSegments()
    {
        if (canvas == null)
            return;

        Segments = new List<(string Name, Shape Segment)>();

        int index = 0;
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                string name = $"R{row + 1}C{col + 1}";
                if (index < canvas.Children.Count && canvas.Children[index] is Shape shape)
                {
                    Segments.Add((name, shape));
                }
                index++;
            }
        }
    }
}
