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

        RefreshMatrix();
    }

    public void UpdateDraw() => RefreshMatrix();

    protected override void RefreshMatrix()
    {
        if (canvas == null)
            return;

        bool rebuilt = ReDrawMatrix();

        if (rebuilt || Segments == null)
        {
            CollectSegments();

            // Freshly created shapes have no handlers yet.
            if (IsPatternMakerEnabled)
                RegisterPointerEvents();
        }

        // Fills and the active tags come from the pattern.
        UpdateCharacter();
    }

    /// <summary>
    /// Lays out the dots. Existing shapes are reused when only their appearance changed.
    /// </summary>
    /// <returns>True when the shape instances were replaced.</returns>
    private bool ReDrawMatrix()
    {
        double cellSize = MatrixDotSize;
        double gap = MatrixDotGap;
        double step = cellSize + gap;

        int total = Columns * Rows;
        bool reuse = total > 0
            && canvas.Children.Count == total
            && (IsMatrixSquare ? canvas.Children[0] is Rectangle : canvas.Children[0] is Ellipse);

        if (!reuse)
            canvas.Children.Clear();

        int index = 0;
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
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

                shape.Width = cellSize;
                shape.Height = cellSize;
                shape.Stroke = Stroke;
                shape.StrokeThickness = StrokeThickness;

                Canvas.SetLeft(shape, col * step);
                Canvas.SetTop(shape, row * step);
                index++;
            }
        }

        canvas.Width = Columns * step;
        canvas.Height = Rows * step - gap;

        return !reuse;
    }

    protected override void CollectSegments()
    {
        if (canvas == null)
            return;

        var list = new List<(string Name, Shape Segment)>(canvas.Children.Count);

        int index = 0;
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                if (index < canvas.Children.Count && canvas.Children[index] is Shape shape)
                    list.Add(($"R{row + 1}C{col + 1}", shape));

                index++;
            }
        }

        Segments = list;
    }
}
