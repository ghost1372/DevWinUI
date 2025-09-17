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

        canvas.Children.Clear();

        double cellW = MatrixDotSize;
        double cellH = MatrixDotSize;
        double gap = MatrixDotGap;

        double cursorX = 0;

        if (!PatternTable.TryGetValue(Character, out string characterPattern))
            characterPattern = DefaultPattern;

        var pattern = DigitalSegmentHelper.GetCharPattern(characterPattern, Columns, Rows);
        if (pattern == null)
        {
            cursorX += Columns * (cellW + gap);
            return;
        }

        for (int row = 0; row < Rows; row++)
        {
            byte line = pattern[row];
            for (int col = 0; col < Columns; col++)
            {
                bool isOn = ((line >> (Columns - 1 - col)) & 1) != 0;
                double x = cursorX + col * (cellW + gap);
                double y = row * (cellH + gap);

                Shape shape;
                if (IsMatrixSquare)
                {
                    shape = new Rectangle
                    {
                        Width = cellW,
                        Height = cellH,
                        Stroke = Stroke,
                        StrokeThickness = StrokeThickness,
                        Fill = isOn ? SegmentForeground : SegmentBackground,
                    };
                }
                else
                {
                    shape = new Ellipse
                    {
                        Width = cellW,
                        Height = cellH,
                        Stroke = Stroke,
                        StrokeThickness = StrokeThickness,
                        Fill = isOn ? SegmentForeground : SegmentBackground,
                    };
                }

                Canvas.SetLeft(shape, x);
                Canvas.SetTop(shape, y);
                canvas.Children.Add(shape);
            }
        }

        cursorX += Columns * (cellW + gap);

        canvas.Width = cursorX;
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

