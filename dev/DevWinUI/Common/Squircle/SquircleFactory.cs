//https://github.com/cnbluefire/Squircle.Windows

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DevWinUI;

public record struct SquircleCornerRadius(double TopLeft, double TopRight, double BottomRight, double BottomLeft);
public record struct SquircleProperties(double Width, double Height, SquircleCornerRadius CornerRadius, double CornerSmoothing, bool PreserveSmoothing);

/// <summary>
/// <see href="https://github.com/phamfoo/figma-squircle"/>
/// </summary>
internal static partial class SquircleFactory
{
    public static T? CreateGeometry<T>(in SquircleProperties props, Func<T?> pathBuilderFactory) where T : SquirclePathBuilder
    {
        if (!IsValidProperties(in props, true)) return null;
        var pathBuilder = pathBuilderFactory.Invoke();
        if (pathBuilder == null) return null;

        var cornerSmoothing = props.CornerSmoothing;
        if (cornerSmoothing < 0) cornerSmoothing = 0;
        else if (cornerSmoothing > 1) cornerSmoothing = 1;

        if (AreCornersEqual(props.CornerRadius))
        {
            var roundingAndSmoothingBudget = Math.Min(props.Width, props.Height) / 2;
            var cornerRadius = Math.Min(props.CornerRadius.TopLeft, roundingAndSmoothingBudget);

            var pathParams = GetPathParamsForCorner(
                cornerRadius,
                cornerSmoothing,
                props.PreserveSmoothing,
                roundingAndSmoothingBudget);


            CreateGeometryFromParams(
                pathBuilder,
                props.Width,
                props.Height,
                in pathParams,
                in pathParams,
                in pathParams,
                in pathParams);

            return pathBuilder;
        }

        var map = DistributeAndNormalize(props.Width, props.Height, props.CornerRadius);
        CreateGeometryFromParams(
            pathBuilder,
            props.Width,
            props.Height,
            topLeftPathParams: GetPathParamsForCorner(
                map[Adjacent.CornerType.TopLeft].Radius,
                props.CornerSmoothing,
                props.PreserveSmoothing,
                map[Adjacent.CornerType.TopLeft].RoundingAndSmoothingBudget),
            topRightPathParams: GetPathParamsForCorner(
                map[Adjacent.CornerType.TopRight].Radius,
                props.CornerSmoothing,
                props.PreserveSmoothing,
                map[Adjacent.CornerType.TopRight].RoundingAndSmoothingBudget),
            bottomLeftPathParams: GetPathParamsForCorner(
                map[Adjacent.CornerType.BottomLeft].Radius,
                props.CornerSmoothing,
                props.PreserveSmoothing,
                map[Adjacent.CornerType.BottomLeft].RoundingAndSmoothingBudget),
            bottomRightPathParams: GetPathParamsForCorner(
                map[Adjacent.CornerType.BottomRight].Radius,
                props.CornerSmoothing,
                props.PreserveSmoothing,
                map[Adjacent.CornerType.BottomRight].RoundingAndSmoothingBudget));

        return pathBuilder;
    }

    private static void CreateGeometryFromParams(SquirclePathBuilder pathBuilder, double width, double height, in CornerPathParams topLeftPathParams, in CornerPathParams topRightPathParams, in CornerPathParams bottomLeftPathParams, in CornerPathParams bottomRightPathParams)
    {
        pathBuilder.MoveTo(false, new Vector2((float)(width - topRightPathParams.p), 0));

        DrawTopRightPath(pathBuilder, in topRightPathParams);

        pathBuilder.LineTo(false, new Vector2((float)width, (float)(height - bottomRightPathParams.p)));

        DrawBottomRightPath(pathBuilder, in bottomRightPathParams);

        pathBuilder.LineTo(false, new Vector2((float)bottomLeftPathParams.p, (float)height));

        DrawBottomLeftPath(pathBuilder, in bottomLeftPathParams);

        pathBuilder.LineTo(false, new Vector2(0, (float)topLeftPathParams.p));

        DrawTopLeftPath(pathBuilder, in topLeftPathParams);
    }

    private static void DrawTopRightPath(SquirclePathBuilder pathBuilder, in CornerPathParams topRightPathParams)
    {
        var (a, b, c, d, p, cornerRadius, arcSectionLength) = topRightPathParams;

        if (cornerRadius > 0)
        {
            pathBuilder.CubicBezierTo(
                true,
                new Vector2((float)a, 0),
                new Vector2((float)(a + b), 0),
                new Vector2((float)(a + b + c), (float)d));

            pathBuilder.ArcTo(true,
                cornerRadius, cornerRadius, 0, false, true, new Vector2((float)arcSectionLength, (float)arcSectionLength));

            pathBuilder.CubicBezierTo(
                true,
                new Vector2((float)d, (float)c),
                new Vector2((float)d, (float)(b + c)),
                new Vector2((float)d, (float)(a + b + c)));
        }
        else
        {
            pathBuilder.LineTo(true, new Vector2((float)p, 0));
        }
    }
    private static void DrawBottomRightPath(SquirclePathBuilder pathBuilder, in CornerPathParams bottomRightPathParams)
    {
        var (a, b, c, d, p, cornerRadius, arcSectionLength) = bottomRightPathParams;

        if (cornerRadius > 0)
        {
            pathBuilder.CubicBezierTo(
                true,
                new Vector2(0, (float)(a)),
                new Vector2(0, (float)(a + b)),
                new Vector2(-(float)d, (float)(a + b + c)));

            pathBuilder.ArcTo(true,
                cornerRadius, cornerRadius, 0, false, true, new Vector2(-(float)arcSectionLength, (float)arcSectionLength));

            pathBuilder.CubicBezierTo(
                true,
                new Vector2(-(float)c, (float)d),
                new Vector2(-(float)(b + c), (float)d),
                new Vector2(-(float)(a + b + c), (float)d));
        }
        else
        {
            pathBuilder.LineTo(true, new Vector2(0, (float)p));
        }
    }
    private static void DrawBottomLeftPath(SquirclePathBuilder pathBuilder, in CornerPathParams bottomLeftPathParams)
    {
        var (a, b, c, d, p, cornerRadius, arcSectionLength) = bottomLeftPathParams;

        if (cornerRadius > 0)
        {
            pathBuilder.CubicBezierTo(
                true,
                new Vector2(-(float)a, 0),
                new Vector2(-(float)(a + b), 0),
                new Vector2(-(float)(a + b + c), -(float)d));

            pathBuilder.ArcTo(true,
                cornerRadius, cornerRadius, 0, false, true, new Vector2(-(float)arcSectionLength, -(float)arcSectionLength));

            pathBuilder.CubicBezierTo(
                true,
                new Vector2(-(float)d, -(float)c),
                new Vector2(-(float)d, -(float)(b + c)),
                new Vector2(-(float)d, -(float)(a + b + c)));
        }
        else
        {
            pathBuilder.LineTo(true, new Vector2(-(float)p, 0));
        }
    }
    private static void DrawTopLeftPath(SquirclePathBuilder pathBuilder, in CornerPathParams topLeftPathParams)
    {
        var (a, b, c, d, p, cornerRadius, arcSectionLength) = topLeftPathParams;

        if (cornerRadius > 0)
        {
            pathBuilder.CubicBezierTo(
                true,
                new Vector2(0, -(float)(a)),
                new Vector2(0, -(float)(a + b)),
                new Vector2((float)d, -(float)(a + b + c)));

            pathBuilder.ArcTo(true,
                cornerRadius, cornerRadius, 0, false, true, new Vector2((float)arcSectionLength, -(float)arcSectionLength));

            pathBuilder.CubicBezierTo(
                true,
                new Vector2((float)c, -(float)d),
                new Vector2((float)(b + c), -(float)d),
                new Vector2((float)(a + b + c), -(float)d));
        }
        else
        {
            pathBuilder.LineTo(true, new Vector2(0, -(float)p));
        }
    }
    private static CornerMap<NormalizedCorner> DistributeAndNormalize(double width, double height, SquircleCornerRadius cornerRadius)
    {
        CornerMap<double> roundingAndSmoothingBudgetMap = new(-1);

        CornerMap<double> cornerRadiusMap = new()
        {
            [Adjacent.CornerType.TopLeft] = cornerRadius.TopLeft,
            [Adjacent.CornerType.TopRight] = cornerRadius.TopRight,
            [Adjacent.CornerType.BottomLeft] = cornerRadius.BottomLeft,
            [Adjacent.CornerType.BottomRight] = cornerRadius.BottomRight,
        };

        var cornerRadiusMapSpan = cornerRadiusMap.AsSpan();
        cornerRadiusMapSpan.Sort((c1, c2) => c2.value.CompareTo(c1.value));

        for (int i = 0; i < 4; i++)
        {
            var (corner, radius) = cornerRadiusMapSpan[i];

            var adjacents = AdjacentsByCorner[corner];
            double budget = double.MaxValue;

            for (int j = 0; j < adjacents.Count; j++)
            {
                var adjacent = adjacents[j];
                var adjacentCornerRadius = cornerRadiusMap[adjacent.Corner];
                if (radius == 0 && adjacentCornerRadius == 0)
                {
                    budget = 0;
                    break;
                }
                var adjacentCornerBudget = roundingAndSmoothingBudgetMap[adjacent.Corner];

                var sideLength = adjacent.Side switch
                {
                    Adjacent.SideType.Top or Adjacent.SideType.Bottom => width,
                    _ => height
                };

                if (adjacentCornerBudget >= 0)
                {
                    budget = Math.Min(sideLength - adjacentCornerBudget, budget);
                }
                else
                {
                    budget = Math.Min((radius / (radius + adjacentCornerRadius)) * sideLength, budget);
                }

            }

            roundingAndSmoothingBudgetMap[corner] = budget;
            cornerRadiusMap[corner] = Math.Min(radius, budget);
        }

        return new CornerMap<NormalizedCorner>()
        {
            [Adjacent.CornerType.TopLeft] = new(cornerRadiusMap[Adjacent.CornerType.TopLeft], roundingAndSmoothingBudgetMap[Adjacent.CornerType.TopLeft]),
            [Adjacent.CornerType.TopRight] = new(cornerRadiusMap[Adjacent.CornerType.TopRight], roundingAndSmoothingBudgetMap[Adjacent.CornerType.TopRight]),
            [Adjacent.CornerType.BottomLeft] = new(cornerRadiusMap[Adjacent.CornerType.BottomLeft], roundingAndSmoothingBudgetMap[Adjacent.CornerType.BottomLeft]),
            [Adjacent.CornerType.BottomRight] = new(cornerRadiusMap[Adjacent.CornerType.BottomRight], roundingAndSmoothingBudgetMap[Adjacent.CornerType.BottomRight]),
        };
    }
    private static CornerPathParams GetPathParamsForCorner(double cornerRadius, double cornerSmoothing, bool preserveSmoothing, double roundingAndSmoothingBudget)
    {
        // From figure 12.2 in the article
        // p = (1 + cornerSmoothing) * q
        // in this case q = R because theta = 90deg
        var p = (1 + cornerSmoothing) * cornerRadius;

        // When there's not enough space left (p > roundingAndSmoothingBudget), there are 2 options:
        //
        // 1. What figma's currently doing: limit the smoothing value to make sure p <= roundingAndSmoothingBudget
        // But what this means is that at some point when cornerRadius is large enough,
        // increasing the smoothing value wouldn't do anything
        //
        // 2. Keep the original smoothing value and use it to calculate the bezier curve normally,
        // then adjust the control points to achieve similar curvature profile
        //
        // preserveSmoothing is a new option I added
        //
        // If preserveSmoothing is on then we'll just keep using the original smoothing value
        // and adjust the bezier curve later
        if (!preserveSmoothing)
        {
            var maxCornerSmoothing = roundingAndSmoothingBudget / cornerRadius - 1;
            cornerSmoothing = Math.Min(cornerSmoothing, maxCornerSmoothing);
            p = Math.Min(p, roundingAndSmoothingBudget);
        }

        // In a normal rounded rectangle (cornerSmoothing = 0), this is 90
        // The larger the smoothing, the smaller the arc
        var arcMeasure = 90 * (1 - cornerSmoothing);
        var arcSectionLength =
          Math.Sin(ToRadians(arcMeasure / 2)) * cornerRadius * Math.Sqrt(2);

        // In the article this is the distance between 2 control points: P3 and P4
        var angleAlpha = (90 - arcMeasure) / 2;
        var p3ToP4Distance = cornerRadius * Math.Tan(ToRadians(angleAlpha / 2));

        // a, b, c and d are from figure 11.1 in the article
        var angleBeta = 45 * cornerSmoothing;
        var c = p3ToP4Distance * Math.Cos(ToRadians(angleBeta));
        var d = c * Math.Tan(ToRadians(angleBeta));

        var b = (p - arcSectionLength - c - d) / 3;
        var a = 2 * b;

        // Adjust the P1 and P2 control points if there's not enough space left
        if (preserveSmoothing && p > roundingAndSmoothingBudget)
        {
            var p1ToP3MaxDistance =
              roundingAndSmoothingBudget - d - arcSectionLength - c;

            // Try to maintain some distance between P1 and P2 so the curve wouldn't look weird
            var minA = p1ToP3MaxDistance / 6;
            var maxB = p1ToP3MaxDistance - minA;

            b = Math.Min(b, maxB);
            a = p1ToP3MaxDistance - b;
            p = Math.Min(p, roundingAndSmoothingBudget);
        }

        return new(a, b, c, d, p, cornerRadius, arcSectionLength);

        static double ToRadians(double degrees)
        {
            return (degrees * Math.PI) / 180;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool AreCornersEqual(in SquircleCornerRadius cornerRadius)
    {
        return cornerRadius.TopLeft == cornerRadius.TopRight
            && cornerRadius.TopLeft == cornerRadius.BottomLeft
            && cornerRadius.TopLeft == cornerRadius.BottomRight;
    }
    public static bool IsValidProperties(in SquircleProperties props, bool checkSize)
    {
        if (checkSize && (props.Width == 0 || props.Height == 0)) return false;
        if (!IsValidCornerRadius(props.CornerRadius)) return false;

        return true;
    }

    private static bool IsValidCornerRadius(in SquircleCornerRadius cornerRadius)
    {
        if (cornerRadius.TopLeft < 0
            || cornerRadius.TopRight < 0
            || cornerRadius.BottomRight < 0
            || cornerRadius.BottomLeft < 0) return false;

        if (cornerRadius.TopLeft == 0
            && cornerRadius.TopRight == 0
            && cornerRadius.BottomRight == 0
            && cornerRadius.BottomLeft == 0) return false;

        return true;
    }

    private record struct CornerPathParams(double a, double b, double c, double d, double p, double cornerRadius, double arcSectionLength);
    private record struct NormalizedCorner(double Radius, double RoundingAndSmoothingBudget);
    private record struct Adjacent(Adjacent.SideType Side, Adjacent.CornerType Corner)
    {
        internal enum SideType
        {
            Top, Left, Right, Bottom
        }

        internal enum CornerType
        {
            TopLeft, TopRight, BottomLeft, BottomRight
        }
    }

    private static IReadOnlyDictionary<Adjacent.CornerType, IReadOnlyList<Adjacent>>? adjacentsByCorner;
    private static IReadOnlyDictionary<Adjacent.CornerType, IReadOnlyList<Adjacent>> AdjacentsByCorner => adjacentsByCorner ??= new Dictionary<Adjacent.CornerType, IReadOnlyList<Adjacent>>
    {
        [Adjacent.CornerType.TopLeft] = [
                new Adjacent(Adjacent.SideType.Top, Adjacent.CornerType.TopRight),
                new Adjacent(Adjacent.SideType.Left, Adjacent.CornerType.BottomLeft)],

        [Adjacent.CornerType.TopRight] = [
                new Adjacent(Adjacent.SideType.Top, Adjacent.CornerType.TopLeft),
                new Adjacent(Adjacent.SideType.Right, Adjacent.CornerType.BottomRight)],

        [Adjacent.CornerType.BottomLeft] = [
                new Adjacent(Adjacent.SideType.Bottom, Adjacent.CornerType.BottomRight),
                new Adjacent(Adjacent.SideType.Left, Adjacent.CornerType.TopLeft)],

        [Adjacent.CornerType.BottomRight] = [
                new Adjacent(Adjacent.SideType.Bottom, Adjacent.CornerType.BottomLeft),
                new Adjacent(Adjacent.SideType.Right, Adjacent.CornerType.TopRight)],
    };
    private struct CornerMap<TValue>
    {
        private __InlineMap _values;

        public CornerMap() : this(default) { }

        public CornerMap(TValue? initializeValue)
        {
            _values[0] = (Adjacent.CornerType.TopLeft, initializeValue);
            _values[1] = (Adjacent.CornerType.TopRight, initializeValue);
            _values[2] = (Adjacent.CornerType.BottomLeft, initializeValue);
            _values[3] = (Adjacent.CornerType.BottomRight, initializeValue);
        }

        public TValue? this[Adjacent.CornerType corner]
        {
            get
            {
                for (int i = 0; i < 4; i++)
                {
                    if (_values[i].corner == corner) return _values[i].value;
                }
                return Throw();
            }
            set
            {
                for (int i = 0; i < 4; i++)
                {
                    if (_values[i].corner == corner)
                    {
                        _values[i].value = value;
                        return;
                    }
                }
                Throw();
            }
        }

        public Span<(Adjacent.CornerType corner, TValue? value)> AsSpan() => MemoryMarshal.CreateSpan(ref _values[0], 4);

        [DoesNotReturn]
        private static TValue Throw() => throw new ArgumentException();

        [InlineArray(4)]
        private struct __InlineMap
        {
            (Adjacent.CornerType corner, TValue? value) First;
        }
    }
}
