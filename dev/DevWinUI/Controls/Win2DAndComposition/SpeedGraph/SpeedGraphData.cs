using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

public partial class SpeedGraphData
{
    private const ulong InitialMax = 1024 * 1024;

    private Size _graphSize;
    private ulong _total;
    private ulong _currentMax = InitialMax;
    private float _ratio = 1.0f;
    private double _currentPercent;

    private Polygon polygon;
    public SpeedGraphData(Polygon polygon)
    {
        this.polygon = polygon;
    }

    private float GetX(ulong progressBytes)
    {
        return (float)(_graphSize.Width * ((float)progressBytes / _total));
    }

    private float GetY(ulong speed, float ratio)
    {
        return (float)(_graphSize.Height * (1.0f - (float)speed / _currentMax / ratio));
    }

    private void AddInitialPointIfNeeded(PointCollection points, ref uint count)
    {
        if (count == 0)
        {
            points.Add(new Point(0, _graphSize.Height));
            count++;
        }
    }
    public struct SetSpeedResult
    {
        public float NewScaleRatio { get; set; }
        public bool NeedAnimation { get; set; }
    }

    public SetSpeedResult SetSpeed(double percent, ulong speed)
    {
        var result = new SetSpeedResult { NewScaleRatio = 1.0f };

        var points = polygon?.Points;
        if (points == null)
            return result;

        if (_currentMax == 0 && speed == 0)
            return result;

        _currentPercent = percent;

        uint count = (uint)points.Count;
        float ratio = _ratio;

        if (_currentMax < speed)
        {
            result.NewScaleRatio = (float)_currentMax / speed;
            _currentMax = speed;
            ratio *= result.NewScaleRatio;
        }

        float y = GetY(speed, ratio);

        float x = (float)(Math.Clamp(percent, 0.0, 100.0) / 100.0 * _graphSize.Width);

        switch (count)
        {
            case 0:
                AddInitialPointIfNeeded(points, ref count);
                points.Add(new Point(x, y));
                break;

            case 2:
                points.Add(new Point(x, y));
                points.Add(new Point(x, _graphSize.Height));
                break;

            default:
                points[(int)count - 1] = new Point(x, y);
                points.Add(new Point(x, _graphSize.Height));
                break;
        }

        if (count > 2)
            result.NeedAnimation = true;

        return result;
    }

    public void SetRatio(float ratio)
    {
        _ratio *= ratio;
    }

    public float GetRatio()
    {
        return _ratio;
    }

    public void Reset()
    {
        polygon?.Points?.Clear();
        _currentMax = InitialMax;
        _ratio = 1.0f;
        _currentPercent = 0;
    }

    public float Height => (float)_graphSize.Height;
    public float Width => (float)_graphSize.Width;

    public void NewSize(Size size)
    {
        _graphSize = size;
    }

    public Point GetLastPoint()
    {
        var points = polygon.Points;
        return points[points.Count - 2];
    }
}
