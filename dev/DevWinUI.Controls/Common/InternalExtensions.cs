using System.Globalization;
using Windows.Foundation;

namespace DevWinUI;

internal static partial class InternalExtensions
{
    private static readonly Random rnd = new Random(new Guid().GetHashCode());
    internal static void BindCenterPoint(this Visual target)
    {
        var exp = target.Compositor.CreateExpressionAnimation(
            "Vector3(this.Target.Size.X / 2, this.Target.Size.Y / 2, 0f)");
        target.StartAnimation("CenterPoint", exp);
    }

    internal static void BindSize(this Visual target, Visual source)
    {
        var exp = target.Compositor.CreateExpressionAnimation("host.Size");
        exp.SetReferenceParameter("host", source);
        target.StartAnimation("Size", exp);
    }

    internal static Vector2 ToVector2(this Vector3 value)
    {
        return new Vector2(value.X, value.Y);
    }
    internal static Vector3 NextVector3(this Random rnd, Vector3 value)
    {
        return new Vector3(rnd.Next(Convert.ToInt32(value.X)), rnd.Next(Convert.ToInt32(value.Y)), rnd.Next(Convert.ToInt32(value.Z)));
    }

    internal static double RandomNegative(this double value)
    {
        if (rnd.Next(0, 2) == 0) return -value;
        else return value;
    }

    internal static void CreateColorStopsWithEasingFunction(this CompositionGradientBrush compositionGradientBrush, EasingMode easingMode, float colorStopBegin, float colorStopEnd, float gap = 0.05f)
    {
        var compositor = compositionGradientBrush.Compositor;
        var easingFunc = new SineEase { EasingMode = easingMode };
        if (easingFunc != null)
        {
            for (float i = colorStopBegin; i < colorStopEnd; i += gap)
            {
                var progress = (i - colorStopBegin) / (colorStopEnd - colorStopBegin);

                var colorStop = compositor.CreateColorGradientStop(i, Color.FromArgb((byte)(0xff * easingFunc.Ease(1 - progress)), 0, 0, 0));
                compositionGradientBrush.ColorStops.Add(colorStop);
            }
        }
        else
        {
            compositionGradientBrush.ColorStops.Add(compositor.CreateColorGradientStop(colorStopBegin, Colors.Black));
        }

        compositionGradientBrush.ColorStops.Add(compositor.CreateColorGradientStop(colorStopEnd, Colors.Transparent));
    }
    private static string Unbracket(string text)
    {
        if (text.Length >= 2 &&
            text[0] == '<' &&
            text[text.Length - 1] == '>')
        {
            text = text.Substring(1, text.Length - 2);
        }

        return text;
    }
    internal static Vector2 ToVector2(this string text)
    {
        if (text.Length == 0)
        {
            return Vector2.Zero;
        }
        else
        {
            // The format <x> or <x, y> is supported
            text = Unbracket(text);

            // Skip allocations when only a component is used
            if (text.IndexOf(',') == -1)
            {
                if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out float x))
                {
                    return new(x);
                }
            }
            else
            {
                string[] values = text.Split(',');

                if (values.Length == 2)
                {
                    if (float.TryParse(values[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                        float.TryParse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y))
                    {
                        return new(x, y);
                    }
                }
            }
        }

        return Throw(text);

        static Vector2 Throw(string text) => throw new FormatException($"Cannot convert \"{text}\" to {nameof(Vector2)}. Use the format \"float, float\"");
    }
    internal static bool IntersectsWith(this Rect rect1, Rect rect2)
    {
        if (rect1.IsEmpty || rect2.IsEmpty)
        {
            return false;
        }

        return (rect1.Left <= rect2.Right) &&
               (rect1.Right >= rect2.Left) &&
               (rect1.Top <= rect2.Bottom) &&
               (rect1.Bottom >= rect2.Top);
    }
    internal static string GetNormalizedCenterPoint(DependencyObject obj)
    {
        return (string)obj.GetValue(NormalizedCenterPointProperty);
    }

    internal static void SetNormalizedCenterPoint(DependencyObject obj, string value)
    {
        obj.SetValue(NormalizedCenterPointProperty, value);
    }

    internal static readonly DependencyProperty NormalizedCenterPointProperty =
        DependencyProperty.RegisterAttached("NormalizedCenterPoint", typeof(string), typeof(InternalExtensions), new PropertyMetadata(false, OnNormalizedCenterPointChanged));
    private static void OnNormalizedCenterPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element &&
            e.NewValue is string newValue)
        {
            Vector2 center = newValue.ToVector2();
            Visual visual = ElementCompositionPreview.GetElementVisual(element);
            const string expression = "Vector2(this.Target.Size.X * X, this.Target.Size.Y * Y)";
            ExpressionAnimation animation = visual.Compositor.CreateExpressionAnimation(expression);

            animation.SetScalarParameter("X", center.X);
            animation.SetScalarParameter("Y", center.Y);

            visual.StopAnimation("CenterPoint.XY");
            visual.StartAnimation("CenterPoint.XY", animation);
        }
    }
}
