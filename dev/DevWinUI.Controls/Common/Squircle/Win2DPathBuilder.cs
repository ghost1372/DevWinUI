//https://github.com/cnbluefire/Squircle.Windows

namespace DevWinUI;

internal partial class Win2DPathBuilder : SquirclePathBuilder
{
    public CanvasGeometry? CreateGeometry(ICanvasResourceCreator? resourceCreator)
    {
        if (Elements.Count == 0) return null;

        using var canvasPathBuilder = new CanvasPathBuilder(resourceCreator);

        var currentPoint = Vector2.Zero;
        bool started = false;

        for (int i = 0; i < Elements.Count; i++)
        {
            var element = Elements[i];

            if (element is MoveElement moveTo)
            {
                if (moveTo.IsRelative) currentPoint += moveTo.MoveTo;
                else currentPoint = moveTo.MoveTo;

                if (started) canvasPathBuilder.EndFigure(CanvasFigureLoop.Closed);
                canvasPathBuilder.BeginFigure(currentPoint);
                started = true;
            }
            else if (element is LineElement lineTo)
            {
                if (lineTo.IsRelative) currentPoint = lineTo.LineTo;
                else currentPoint = lineTo.LineTo;

                if (!started)
                {
                    canvasPathBuilder.BeginFigure(0, 0);
                    started = true;
                }
                canvasPathBuilder.AddLine(currentPoint);
            }
            else if (element is CubicBezierElement cubicBezierTo)
            {
                var cp1 = cubicBezierTo.ControlPoint1;
                var cp2 = cubicBezierTo.ControlPoint2;
                var ep = cubicBezierTo.EndPoint;
                if (cubicBezierTo.IsRelative)
                {
                    cp1 += currentPoint;
                    cp2 += currentPoint;
                    ep += currentPoint;
                }
                currentPoint = ep;

                if (!started)
                {
                    canvasPathBuilder.BeginFigure(0, 0);
                    started = true;
                }
                canvasPathBuilder.AddCubicBezier(cp1, cp2, ep);
            }
            else if (element is ArcElement arcTo)
            {
                if (arcTo.IsRelative) currentPoint += arcTo.EndPoint;
                else currentPoint = arcTo.EndPoint;

                if (!started)
                {
                    canvasPathBuilder.BeginFigure(0, 0);
                    started = true;
                }
                canvasPathBuilder.AddArc(
                    currentPoint,
                    (float)arcTo.RadiusX,
                    (float)arcTo.RadiusY,
                    (float)arcTo.Angle,
                    arcTo.SweepDirectionClockwise ? CanvasSweepDirection.Clockwise : CanvasSweepDirection.CounterClockwise,
                    arcTo.IsLargeFlag ? CanvasArcSize.Large : CanvasArcSize.Small);
            }
        }

        if (started)
        {
            canvasPathBuilder.EndFigure(CanvasFigureLoop.Closed);
            return CanvasGeometry.CreatePath(canvasPathBuilder);
        }
        return null;
    }
}
