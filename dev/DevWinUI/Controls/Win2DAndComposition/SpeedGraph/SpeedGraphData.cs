using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

public partial class SpeedGraphData
{
    private Size _graphSize;
    private ulong _total;
    private ulong _currentMax = 1024 * 1024;
    private float _ratio = 1.0f;
    private double _currentPercent;

    private SpeedGraphMode _mode = SpeedGraphMode.StaticProgress;
    private float _stepX = 4f;
    private Polygon polygon;
    public SpeedGraphData(Polygon polygon)
    {
        this.polygon = polygon;
    }
    public void SetMode(SpeedGraphMode mode)
    {
        _mode = mode;
    }

    private float GetX(ulong progressBytes)
    {
        return (float)(_graphSize.Width * ((float)progressBytes / _total));
    }

    private float GetY(ulong speed)
    {
        return (float)(_graphSize.Height * (1.0f - (float)speed / _currentMax / _ratio));
    }

    private void AddInitialPointIfNeeded(ref uint count)
    {
        if (count == 0)
        {
            polygon.Points.Add(new Point(0, _graphSize.Height));
            count++;
        }
    }

    private void ShiftLeft(float dx)
    {
        for (int i = 0; i < polygon.Points.Count; i++)
        {
            var p = polygon.Points[i];
            polygon.Points[i] = new Point(p.X - dx, p.Y);
        }
    }

    public class SetSpeedResult
    {
        public float NewScaleRatio { get; set; } = 1.0f;
        public bool NeedAnimation { get; set; } = false;
    }

    public SetSpeedResult SetSpeed(double percent, ulong speed)
    {
        var result = new SetSpeedResult();

        if (polygon.Points == null)
            return result;

        if (_currentMax == 0 && speed == 0)
            return result;

        _currentPercent = percent;

        uint count = (uint)polygon.Points.Count;

        if (_currentMax < speed)
        {
            result.NewScaleRatio = (float)_currentMax / speed;
            _currentMax = speed;
        }

        float y = GetY(speed);

        if (_mode == SpeedGraphMode.StaticProgress)
        {
            float x = (float)(percent / 100.0 * _graphSize.Width);

            switch (count)
            {
                case 0:
                    AddInitialPointIfNeeded(ref count);
                    polygon.Points.Add(new Point(x, y));
                    break;

                case 2:
                    polygon.Points.Add(new Point(x, y));
                    polygon.Points.Add(new Point(x, _graphSize.Height));
                    break;

                default:
                    polygon.Points[polygon.Points.Count - 1] = new Point(x, y);
                    polygon.Points.Add(new Point(x, _graphSize.Height));
                    break;
            }

            if (count > 2)
                result.NeedAnimation = true;
        }

        else
        {
            ShiftLeft(_stepX);

            float x = (float)_graphSize.Width;

            if (count == 0)
            {
                polygon.Points.Add(new Point(x, y));
            }
            else
            {
                polygon.Points[polygon.Points.Count - 1] = new Point(x, y);
            }

            polygon.Points.Add(new Point(x, _graphSize.Height));

            while (polygon.Points.Count > 0 && polygon.Points[0].X < 0)
                polygon.Points.RemoveAt(0);

            result.NeedAnimation = true;
        }

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

    public float Height => (float)_graphSize.Height;
    public float Width => (float)_graphSize.Width;

    public void NewSize(Size size)
    {
        _graphSize = size;
    }

    public Point GetLastPoint()
    {
        return polygon.Points[polygon.Points.Count - 2];
    }
}
