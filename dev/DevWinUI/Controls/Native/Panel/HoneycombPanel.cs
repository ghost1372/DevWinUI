namespace DevWinUI;
public partial class HoneycombPanel : Panel
{
    private double _unitLength;

    private HoneycombStuffer _stuffer;

    private static int GetXCount(int count)
    {
        if (count == 0) return 0;

        count -= 1;

        var index = (int)Math.Floor(Math.Pow((12.0 * count + 25) / 36, 0.5) - 5.0 / 6);
        var valeIndex = 3 * index * index + 5 * index;
        var centerValue = valeIndex + 2;

        return count >= centerValue
            ? 4 * index + 6
            : count > valeIndex
                ? 4 * index + 4
                : 4 * index + 2;
    }

    private static int GetYCount(int count)
    {
        if (count == 0) return 0;

        count -= 1;

        var index = (int)Math.Floor(Math.Pow(count / 3.0 + 0.25, 0.5) - 0.5);
        var valeIndex = 3 * index * index + 3 * index;

        return count > valeIndex
            ? 2 * index + 2
            : 2 * index;
    }

    /*
     *                layout order
     *
     *                  ●       ●
     *                 (7)     (8)
     *
     *              ●       ●       ●
     *             (6)     (1)     (9)
     *
     *          ●       ●       ●       ...
     *         (5)     (0)     (2)
     *
     *              ●       ●       ...
     *             (4)     (3)
     *
     */
    protected override Size MeasureOverride(Size availableSize)
    {
        var maxSize = new Size();

        foreach (UIElement child in Children)
        {
            if (child != null)
            {
                child.Measure(availableSize);
                maxSize.Width = Math.Max(maxSize.Width, child.DesiredSize.Width);
                maxSize.Height = Math.Max(maxSize.Height, child.DesiredSize.Height);
            }
        }

        _unitLength = Math.Max(maxSize.Width, maxSize.Height) / 2;

        var xCount = GetXCount(Children.Count);
        var yCount = GetYCount(Children.Count);

        var availableWidth = xCount * _unitLength;
        var availableHeight = yCount * Math.Pow(3, 0.5) * _unitLength + _unitLength * 2;

        return new Size(availableWidth, availableHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var childLength = _unitLength * 2;
        _stuffer = new HoneycombStuffer(new Rect(finalSize.Width / 2 - _unitLength,
            finalSize.Height / 2 - _unitLength, childLength, childLength));

        foreach (UIElement child in Children)
        {
            child.Arrange(_stuffer.Move());
        }

        return finalSize;
    }

    private class HoneycombStuffer
    {
        private int _turns;

        private int _maxIndex;

        private int _currentIndex = -1;

        private readonly double _offsetX;

        private readonly double _offsetY;

        private Rect _childBounds;

        private readonly double[] _offsetXArr;

        private readonly double[] _offsetYArr;

        public HoneycombStuffer(Rect childBounds)
        {
            _childBounds = childBounds;
            _offsetX = childBounds.Width / 2;
            _offsetY = Math.Pow(3, 0.5) * _offsetX;

            _offsetXArr = new[]
            {
                2 * _offsetX,
                _offsetX,
                -_offsetX,
                -2 * _offsetX,
                -_offsetX,
                _offsetX
            };

            _offsetYArr = new[]
            {
                0,
                _offsetY,
                _offsetY,
                0,
                -_offsetY,
                -_offsetY
            };
        }

        public Rect Move()
        {
            _currentIndex++;
            if (_currentIndex > _maxIndex)
            {
                _turns++;
                _maxIndex = _turns * 6 - 1;
                _currentIndex = 0;
                _childBounds = new Rect(_childBounds.X + _offsetX, _childBounds.Y - _offsetY, _childBounds.Width, _childBounds.Height);
                return _childBounds;
            }

            if (_turns > 0)
            {
                var index = _currentIndex / _turns;
                _childBounds = new Rect(_childBounds.X + _offsetXArr[index], _childBounds.Y + _offsetYArr[index], _childBounds.Width, _childBounds.Height);
            }

            return _childBounds;
        }
    }
}
