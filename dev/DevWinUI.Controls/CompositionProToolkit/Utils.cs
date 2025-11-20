// Copyright (c) Ratish Philip 
//
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions: 
// 
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software. 
// 
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE. 
//
// This file is part of the CompositionProToolkit project: 
// https://github.com/ratishphilip/CompositionProToolkit
//
// CompositionProToolkit v1.0.1
// 

using System.Globalization;
using System.Runtime.InteropServices;

namespace DevWinUI;

/// <summary>
/// Class containing collection of useful methods
/// for various types
/// </summary>
public static partial class Utils
{
    #region Double and Float

    // Constant values 

    // Smallest double value such that 1.0 + DoubleEpsilon != 1.0
    internal const double DoubleEpsilon = 2.2204460492503131e-016;
    // Number close to zero, where float.MinValue is -float.MaxValue
    internal const float FloatMin = 1.175494351e-38F;

    /// <summary>
    /// Returns whether or not two doubles are "close". 
    /// </summary>
    /// <param name="value1"> The first double to compare. </param>
    /// <param name="value2"> The second double to compare. </param>
    /// <returns>
    /// bool - the result of the AreClose comparision.
    /// </returns>
    public static bool IsCloseTo(this double value1, double value2)
    {
        //in case they are Infinities or NaN (then epsilon check does not work)
        if ((Double.IsInfinity(value1) &&
             Double.IsInfinity(value2)) ||
            (IsNaN(value1) && (IsNaN(value2))))
            return true;
        // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DoubleEpsilon
        var eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DoubleEpsilon;
        var delta = value1 - value2;
        return (-eps < delta) && (eps > delta);
    }

    /// <summary>
    /// Returns whether or not the first double is less than the second double.
    /// </summary>
    /// <param name="value1"> The first double to compare. </param>
    /// <param name="value2"> The second double to compare. </param>
    /// <returns>
    /// bool - the result of the LessThan comparision.
    /// </returns>
    public static bool IsLessThan(double value1, double value2)
    {
        return (value1 < value2) && !value1.IsCloseTo(value2);
    }

    /// <summary>
    /// Returns whether or not the first double is greater than the second double.
    /// </summary>
    /// <param name="value1"> The first double to compare. </param>
    /// <param name="value2"> The second double to compare. </param>
    /// <returns>
    /// bool - the result of the GreaterThan comparision.
    /// </returns>
    public static bool IsGreaterThan(this double value1, double value2)
    {
        return (value1 > value2) && !value1.IsCloseTo(value2);
    }

    /// <summary>
    /// Returns whether or not the double is "close" to 1.  Same as AreClose(double, 1),
    /// but this is faster.
    /// </summary>
    /// <param name="value"> The double to compare to 1. </param>
    /// <returns>
    /// bool - the result of the AreClose comparision.
    /// </returns>
    public static bool IsOne(this double value)
    {
        return Math.Abs(value - 1.0) < 10.0 * DoubleEpsilon;
    }

    /// <summary>
    /// IsZero - Returns whether or not the double is "close" to 0.  Same as AreClose(double, 0),
    /// but this is faster.
    /// </summary>
    /// <param name="value"> The double to compare to 0. </param>
    /// <returns>
    /// bool - the result of the AreClose comparision.
    /// </returns>
    public static bool IsZero(this double value)
    {
        return Math.Abs(value) < 10.0 * DoubleEpsilon;
    }

    /// <summary>
    /// Returns whether or not two floats are "close". 
    /// </summary>
    /// <param name="value1"> The first float to compare. </param>
    /// <param name="value2"> The second float to compare. </param>
    /// <returns>
    /// bool - the result of the AreClose comparision.
    /// </returns>
    public static bool IsCloseTo(this float value1, float value2)
    {
        // In case they are Infinities or NaN (then epsilon check does not work)
        if ((Single.IsInfinity(value1) &&
             Single.IsInfinity(value2)) ||
            (Single.IsNaN(value1) && (Single.IsNaN(value2))))
            return true;

        // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < FloatMin
        var eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * FloatMin;
        var delta = value1 - value2;
        return (-eps < delta) && (eps > delta);
    }

    /// <summary>
    /// Returns whether or not the first float is less than the second float.
    /// </summary>
    /// <param name="value1"> The first float to compare. </param>
    /// <param name="value2"> The second float to compare. </param>
    /// <returns>
    /// bool - the result of the LessThan comparision.
    /// </returns>
    public static bool IsLessThan(float value1, float value2)
    {
        return (value1 < value2) && !value1.IsCloseTo(value2);
    }

    /// <summary>
    /// Returns whether or not the first float is greater than the second float.
    /// </summary>
    /// <param name="value1"> The first float to compare. </param>
    /// <param name="value2"> The second float to compare. </param>
    /// <returns>
    /// bool - the result of the GreaterThan comparision.
    /// </returns>
    public static bool IsGreaterThan(this float value1, float value2)
    {
        return (value1 > value2) && !value1.IsCloseTo(value2);
    }

    /// <summary>
    /// Returns whether or not the float is "close" to 1.  Same as AreClose(float, 1),
    /// but this is faster.
    /// </summary>
    /// <param name="value"> The float to compare to 1. </param>
    /// <returns>
    /// bool - the result of the AreClose comparision.
    /// </returns>
    public static bool IsOne(this float value)
    {
        return Math.Abs(value - 1.0) < 10.0 * FloatMin;
    }

    /// <summary>
    /// IsZero - Returns whether or not the float is "close" to 0.  Same as AreClose(float, 0),
    /// but this is faster.
    /// </summary>
    /// <param name="value"> The float to compare to 0. </param>
    /// <returns>
    /// bool - the result of the AreClose comparision.
    /// </returns>
    public static bool IsZero(this float value)
    {
        return Math.Abs(value) < 10.0 * FloatMin;
    }

    /// <summary>
    /// Compares two points for fuzzy equality.  This function
    /// helps compensate for the fact that double values can 
    /// acquire error when operated upon
    /// </summary>
    /// <param name='point1'>The first point to compare</param>
    /// <param name='point2'>The second point to compare</param>
    /// <returns>Whether or not the two points are equal</returns>
    public static bool IsCloseTo(this Point point1, Point point2)
    {
        return point1.X.IsCloseTo(point2.X) && point1.Y.IsCloseTo(point2.Y);
    }

    /// <summary>
    /// Compares two Size instances for fuzzy equality.  This function
    /// helps compensate for the fact that double values can 
    /// acquire error when operated upon
    /// </summary>
    /// <param name='size1'>The first size to compare</param>
    /// <param name='size2'>The second size to compare</param>
    /// <returns>Whether or not the two Size instances are equal</returns>
    public static bool IsCloseTo(this Size size1, Size size2)
    {
        return size1.Width.IsCloseTo(size2.Width) && size1.Height.IsCloseTo(size2.Height);
    }

    /// <summary>
    /// Compares two rectangles for fuzzy equality.  This function
    /// helps compensate for the fact that double values can 
    /// acquire error when operated upon
    /// </summary>
    /// <param name='rect1'>The first rectangle to compare</param>
    /// <param name='rect2'>The second rectangle to compare</param>
    /// <returns>Whether or not the two rectangles are equal</returns>
    public static bool IsCloseTo(this Rect rect1, Rect rect2)
    {
        // If they're both empty, don't bother with the double logic.
        if (rect1.IsEmpty)
        {
            return rect2.IsEmpty;
        }

        // At this point, rect1 isn't empty, so the first thing we can test is
        // rect2.IsEmpty, followed by property-wise compares.

        return (!rect2.IsEmpty)
               && rect1.X.IsCloseTo(rect2.X) && rect1.Y.IsCloseTo(rect2.Y)
               && rect1.Height.IsCloseTo(rect2.Height) && rect1.Width.IsCloseTo(rect2.Width);
    }

    /// <summary>
    /// Restricts the given single precision number to be within the
    /// specified min and max values (inclusive).
    /// </summary>
    /// <param name="num">Given single precision number</param>
    /// <param name="min">Minimum value</param>
    /// <param name="max">Maximum value</param>
    /// <returns>Clamped value</returns>
    public static float Clamp(this float num, float min, float max)
    {
        if (min > max)
        {
            var temp = min;
            min = max;
            max = temp;
        }

        return Math.Max(min, Math.Min(max, num));
    }

    /// <summary>
    /// Restricts the given double precision number to be within the
    /// specified min and max values (inclusive).
    /// </summary>
    /// <param name="num">Given double precision number</param>
    /// <param name="min">Minimum value</param>
    /// <param name="max">Maximum value</param>
    /// <returns>Clamped value</returns>
    public static double Clamp(this double num, double min, double max)
    {
        if (min > max)
        {
            var temp = min;
            min = max;
            max = temp;
        }

        return Math.Max(min, Math.Min(max, num));
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct NanUnion
    {
        [FieldOffset(0)]
        internal double DoubleValue;
        [FieldOffset(0)]
        internal UInt64 UintValue;
    }

    /// <summary>
    /// Faster check for NaN ( faster than double.IsNaN() )
    /// IEEE 754 : If the argument is any value in the range 0x7ff0000000000001L through 0x7fffffffffffffffL 
    /// or in the range 0xfff0000000000001L through 0xffffffffffffffffL, the result will be NaN.
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <returns></returns>
    public static bool IsNaN(double value)
    {
        var t = new NanUnion
        {
            DoubleValue = value
        };

        var exp = t.UintValue & 0xfff0000000000000;
        var man = t.UintValue & 0x000fffffffffffff;

        return (exp == 0x7ff0000000000000 || exp == 0xfff0000000000000) && (man != 0);
    }

    /// <summary>
    /// Rounds the given value based on the DPI scale
    /// </summary>
    /// <param name="value">Value to round</param>
    /// <param name="dpiScale">DPI Scale</param>
    /// <returns></returns>
    public static double RoundLayoutValue(double value, double dpiScale)
    {
        double newValue;

        // If DPI == 1, don't use DPI-aware rounding. 
        if (!dpiScale.IsCloseTo(1.0))
        {
            newValue = Math.Round(value * dpiScale) / dpiScale;
            // If rounding produces a value unacceptable to layout (NaN, Infinity or MaxValue), use the original value.
            if (IsNaN(newValue) ||
                Double.IsInfinity(newValue) ||
                newValue.IsCloseTo(Double.MaxValue))
            {
                newValue = value;
            }
        }
        else
        {
            newValue = Math.Round(value);
        }

        return newValue;
    }

    /// <summary>
    /// Calculates the linear interpolated value based on the given values.
    /// </summary>
    /// <param name="start">Starting value.</param>
    /// <param name="end">Ending value.</param>
    /// <param name="amount">Weightage given to the ending value.</param>
    /// <returns>Linear interpolated value.</returns>
    public static float Lerp(this float start, float end, float amount)
    {
        return start + (end - start) * amount;
    }

    #endregion

    #region Thickness

    /// <summary>
    /// Verifies if this Thickness contains only valid values
    /// The set of validity checks is passed as parameters.
    /// </summary>
    /// <param name='thick'>Thickness value</param>
    /// <param name='allowNegative'>allows negative values</param>
    /// <param name='allowNaN'>allows Double.NaN</param>
    /// <param name='allowPositiveInfinity'>allows Double.PositiveInfinity</param>
    /// <param name='allowNegativeInfinity'>allows Double.NegativeInfinity</param>
    /// <returns>Whether or not the thickness complies to the range specified</returns>
    public static bool IsValid(this Thickness thick, bool allowNegative, bool allowNaN, bool allowPositiveInfinity, bool allowNegativeInfinity)
    {
        if (!allowNegative)
        {
            if (thick.Left < 0d || thick.Right < 0d || thick.Top < 0d || thick.Bottom < 0d)
                return false;
        }

        if (!allowNaN)
        {
            if (IsNaN(thick.Left) || IsNaN(thick.Right)
                || IsNaN(thick.Top) || IsNaN(thick.Bottom))
                return false;
        }

        if (!allowPositiveInfinity)
        {
            if (Double.IsPositiveInfinity(thick.Left) || Double.IsPositiveInfinity(thick.Right)
                || Double.IsPositiveInfinity(thick.Top) || Double.IsPositiveInfinity(thick.Bottom))
            {
                return false;
            }
        }

        if (!allowNegativeInfinity)
        {
            if (Double.IsNegativeInfinity(thick.Left) || Double.IsNegativeInfinity(thick.Right)
                || Double.IsNegativeInfinity(thick.Top) || Double.IsNegativeInfinity(thick.Bottom))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Method to add up the left and right size as width, as well as the top and bottom size as height
    /// </summary>
    /// <param name="thick">Thickness</param>
    /// <returns>Size</returns>
    public static Size CollapseThickness(this Thickness thick)
    {
        return new Size(thick.Left + thick.Right, thick.Top + thick.Bottom);
    }

    /// <summary>
    /// Verifies if the Thickness contains only zero values
    /// </summary>
    /// <param name="thick">Thickness</param>
    /// <returns>Size</returns>
    public static bool IsZero(this Thickness thick)
    {
        return thick.Left.IsZero()
                && thick.Top.IsZero()
                && thick.Right.IsZero()
                && thick.Bottom.IsZero();
    }

    /// <summary>
    /// Verifies if all the values in Thickness are same
    /// </summary>
    /// <param name="thick">Thickness</param>
    /// <returns>true if yes, otherwise false</returns>
    public static bool IsUniform(this Thickness thick)
    {
        return thick.Left.IsCloseTo(thick.Top)
                && thick.Left.IsCloseTo(thick.Right)
                && thick.Left.IsCloseTo(thick.Bottom);
    }

    /// <summary>
    /// Converts the Thickness object to Vector4. If the Thickness 
    /// object's component have values NaN, PositiveInfinity or 
    /// NegativeInfinity, then Vector4.Zero will be returned.
    /// </summary>
    /// <param name="thickness">Thickness object</param>
    /// <returns>Vector4</returns>
    public static Vector4 ToVector4(this Thickness thickness)
    {
        if (thickness.IsValid(true, false, false, false))
        {
            // Sanitize the component by taking only 
            return new Vector4(thickness.Left.ToSingle(),
                               thickness.Top.ToSingle(),
                               thickness.Right.ToSingle(),
                               thickness.Bottom.ToSingle());
        }

        return Vector4.Zero;
    }

    /// <summary>
    /// Converts the Thickness object to Vector4. If the Thickness
    /// object contains negative components they will be converted
    /// to positive values. If the Thickness object's component
    /// have values NaN, PositiveInfinity or NegativeInfinity,
    /// then Vector4.Zero will be returned.
    /// </summary>
    /// <param name="thickness">Thickness object</param>
    /// <returns>Vector2</returns>
    public static Vector4 ToAbsVector4(this Thickness thickness)
    {
        if (thickness.IsValid(true, false, false, false))
        {
            // Sanitize the component by taking only 
            return new Vector4(Math.Abs(thickness.Left).ToSingle(),
                               Math.Abs(thickness.Top).ToSingle(),
                               Math.Abs(thickness.Right).ToSingle(),
                               Math.Abs(thickness.Bottom).ToSingle());
        }

        return Vector4.Zero;
    }

    /// <summary>
    /// Gets the top left corner of the thickness structure.
    /// </summary>
    /// <param name="thickness">Thickness object</param>
    /// <returns>Vector2</returns>
    public static Vector2 GetOffset(this Thickness thickness)
    {
        return new Vector2(thickness.Left.ToSingle(), thickness.Top.ToSingle());
    }

    #endregion

    #region CornerRadius

    /// <summary>
    /// Verifies if this CornerRadius contains only valid values
    /// The set of validity checks is passed as parameters.
    /// </summary>
    /// <param name='corner'>CornerRadius value</param>
    /// <param name='allowNegative'>allows negative values</param>
    /// <param name='allowNaN'>allows Double.NaN</param>
    /// <param name='allowPositiveInfinity'>allows Double.PositiveInfinity</param>
    /// <param name='allowNegativeInfinity'>allows Double.NegativeInfinity</param>
    /// <returns>Whether or not the CornerRadius complies to the range specified</returns>
    public static bool IsValid(this CornerRadius corner, bool allowNegative, bool allowNaN, bool allowPositiveInfinity, bool allowNegativeInfinity)
    {
        if (!allowNegative)
        {
            if (corner.TopLeft < 0d || corner.TopRight < 0d || corner.BottomLeft < 0d || corner.BottomRight < 0d)
            {
                return (false);
            }
        }

        if (!allowNaN)
        {
            if (IsNaN(corner.TopLeft) || IsNaN(corner.TopRight) ||
                IsNaN(corner.BottomLeft) || IsNaN(corner.BottomRight))
            {
                return (false);
            }
        }

        if (!allowPositiveInfinity)
        {
            if (Double.IsPositiveInfinity(corner.TopLeft) || Double.IsPositiveInfinity(corner.TopRight) ||
                Double.IsPositiveInfinity(corner.BottomLeft) || Double.IsPositiveInfinity(corner.BottomRight))
            {
                return (false);
            }
        }

        if (!allowNegativeInfinity)
        {
            if (Double.IsNegativeInfinity(corner.TopLeft) || Double.IsNegativeInfinity(corner.TopRight) ||
                Double.IsNegativeInfinity(corner.BottomLeft) || Double.IsNegativeInfinity(corner.BottomRight))
            {
                return (false);
            }
        }

        return true;
    }

    /// <summary>
    /// Verifies if the CornerRadius contains only zero values
    /// </summary>
    /// <param name="corner">CornerRadius</param>
    /// <returns>true if yes, otherwise false</returns>
    public static bool IsZero(this CornerRadius corner)
    {
        return corner.TopLeft.IsZero()
               && corner.TopRight.IsZero()
               && corner.BottomRight.IsZero()
               && corner.BottomLeft.IsZero();
    }

    /// <summary>
    /// Verifies if the CornerRadius contains same values
    /// </summary>
    /// <param name="corner">CornerRadius</param>
    /// <returns>true if yes, otherwise false</returns>
    public static bool IsUniform(this CornerRadius corner)
    {
        var topLeft = corner.TopLeft;
        return topLeft.IsCloseTo(corner.TopRight) &&
               topLeft.IsCloseTo(corner.BottomLeft) &&
               topLeft.IsCloseTo(corner.BottomRight);
    }

    /// <summary>
    /// Converts the given corner value to a valid positive value.
    /// Returns zero if the corner value is Infinity or NaN or 0.
    /// </summary>
    /// <param name="corner">Corner value</param>
    /// <returns>Valid Corner value</returns>
    public static double ConvertToValidCornerValue(double corner)
    {
        if (IsNaN(corner) ||
            Double.IsInfinity(corner) ||
            (corner < 0d))
        {
            return 0d;
        }

        return corner;
    }

    /// <summary>
    /// Converts the CornerRadius object to Vector4. If the CornerRadius 
    /// object's component have values NaN, PositiveInfinity or 
    /// NegativeInfinity, then Vector4.Zero will be returned.
    /// </summary>
    /// <param name="corner">CornerRadius object</param>
    /// <returns>Vector4</returns>
    public static Vector4 ToVector4(this CornerRadius corner)
    {
        if (corner.IsValid(true, false, false, false))
        {
            return new Vector4(corner.TopLeft.ToSingle(),
                               corner.TopRight.ToSingle(),
                               corner.BottomRight.ToSingle(),
                               corner.BottomLeft.ToSingle());
        }

        return Vector4.Zero;
    }

    /// <summary>
    /// Converts the CornerRadius object to Vector4. If the CornerRadius
    /// object contains negative components they will be converted
    /// to positive values. If the Thickness object's component
    /// have values NaN, PositiveInfinity or NegativeInfinity,
    /// then Vector4.Zero will be returned.
    /// </summary>
    /// <param name="corner">CornerRadius object</param>
    /// <returns>Vector4</returns>
    public static Vector4 ToAbsVector4(this CornerRadius corner)
    {
        if (corner.IsValid(true, false, false, false))
        {
            // Sanitize the component by taking only 
            return new Vector4(Math.Abs(corner.TopLeft).ToSingle(),
                               Math.Abs(corner.TopRight).ToSingle(),
                               Math.Abs(corner.BottomRight).ToSingle(),
                               Math.Abs(corner.BottomLeft).ToSingle());
        }

        return Vector4.Zero;
    }

    #endregion

    #region Rect

    /// <summary>
    /// Deflates rectangle by given thickness
    /// </summary>
    /// <param name="rect">Rectangle</param>
    /// <param name="thick">Thickness</param>
    /// <returns>Deflated Rectangle</returns>
    public static Rect Deflate(this Rect rect, Thickness thick)
    {
        return new Rect(rect.Left + thick.Left,
            rect.Top + thick.Top,
            Math.Max(0.0, rect.Width - thick.Left - thick.Right),
            Math.Max(0.0, rect.Height - thick.Top - thick.Bottom));
    }

    /// <summary>
    /// Inflates rectangle by given thickness
    /// </summary>
    /// <param name="rect">Rectangle</param>
    /// <param name="thick">Thickness</param>
    /// <returns>Inflated Rectangle</returns>
    public static Rect Inflate(this Rect rect, Thickness thick)
    {
        return new Rect(rect.Left - thick.Left,
            rect.Top - thick.Top,
            Math.Max(0.0, rect.Width + thick.Left + thick.Right),
            Math.Max(0.0, rect.Height + thick.Top + thick.Bottom));
    }

    #endregion

    #region Brush

    /// <summary>
    /// Verifies if the given brush is a SolidColorBrush and
    /// its color does not include transparency.
    /// </summary>
    /// <param name="brush">Brush</param>
    /// <returns>true if yes, otherwise false</returns>
    public static bool IsOpaqueSolidColorBrush(this Brush brush)
    {
        return (brush as SolidColorBrush)?.Color.A == 0xff;
    }

    /// <summary>
    /// Verifies if the given brush is the same as the otherBrush.
    /// </summary>
    /// <param name="brush">Brush</param>
    /// <param name="otherBrush">Brush</param>
    /// <returns>true if yes, otherwise false</returns>
    public static bool IsEqualTo(this Brush brush, Brush otherBrush)
    {
        if (brush.GetType() != otherBrush.GetType())
            return false;

        if (ReferenceEquals(brush, otherBrush))
            return true;

        // Are both instances of SolidColorBrush
        var solidBrushA = brush as SolidColorBrush;
        var solidBrushB = otherBrush as SolidColorBrush;
        if ((solidBrushA != null) && (solidBrushB != null))
        {
            return (solidBrushA.Color == solidBrushB.Color)
                   && solidBrushA.Opacity.IsCloseTo(solidBrushB.Opacity);
        }

        // Are both instances of LinearGradientBrush
        var linGradBrushA = brush as LinearGradientBrush;
        var linGradBrushB = otherBrush as LinearGradientBrush;

        if ((linGradBrushA != null) && (linGradBrushB != null))
        {
            var result = (linGradBrushA.ColorInterpolationMode == linGradBrushB.ColorInterpolationMode)
                           && (linGradBrushA.EndPoint == linGradBrushB.EndPoint)
                           && (linGradBrushA.MappingMode == linGradBrushB.MappingMode)
                           && linGradBrushA.Opacity.IsCloseTo(linGradBrushB.Opacity)
                           && (linGradBrushA.StartPoint == linGradBrushB.StartPoint)
                           && (linGradBrushA.SpreadMethod == linGradBrushB.SpreadMethod)
                           && (linGradBrushA.GradientStops.Count == linGradBrushB.GradientStops.Count);
            if (!result)
                return false;

            for (var i = 0; i < linGradBrushA.GradientStops.Count; i++)
            {
                result = (linGradBrushA.GradientStops[i].Color == linGradBrushB.GradientStops[i].Color)
                         && linGradBrushA.GradientStops[i].Offset.IsCloseTo(linGradBrushB.GradientStops[i].Offset);

                if (!result)
                    break;
            }

            return result;
        }

        // Are both instances of RadialGradientBrush
        //var radGradBrushA = brush as RadialGradientBrush;
        //var radGradBrushB = otherBrush as RadialGradientBrush;

        //if ((radGradBrushA != null) && (radGradBrushB != null))
        //{
        //    var result = (radGradBrushA.ColorInterpolationMode == radGradBrushB.ColorInterpolationMode)
        //                 && (radGradBrushA.GradientOrigin == radGradBrushB.GradientOrigin)
        //                 && (radGradBrushA.MappingMode == radGradBrushB.MappingMode)
        //                 && radGradBrushA.Opacity.IsCloseTo(radGradBrushB.Opacity)
        //                 && radGradBrushA.RadiusX.IsCloseTo(radGradBrushB.RadiusX)
        //                 && radGradBrushA.RadiusY.IsCloseTo(radGradBrushB.RadiusY)
        //                 && (radGradBrushA.SpreadMethod == radGradBrushB.SpreadMethod)
        //                 && (radGradBrushA.GradientStops.Count == radGradBrushB.GradientStops.Count);

        //    if (!result)
        //        return false;

        //    for (var i = 0; i < radGradBrushA.GradientStops.Count; i++)
        //    {
        //        result = (radGradBrushA.GradientStops[i].Color == radGradBrushB.GradientStops[i].Color)
        //                 && radGradBrushA.GradientStops[i].Offset.IsCloseTo(radGradBrushB.GradientStops[i].Offset);

        //        if (!result)
        //            break;
        //    }

        //    return result;
        //}

        // Are both instances of ImageBrush
        var imgBrushA = brush as ImageBrush;
        var imgBrushB = otherBrush as ImageBrush;
        if ((imgBrushA != null) && (imgBrushB != null))
        {
            var result = (imgBrushA.AlignmentX == imgBrushB.AlignmentX)
                          && (imgBrushA.AlignmentY == imgBrushB.AlignmentY)
                          && imgBrushA.Opacity.IsCloseTo(imgBrushB.Opacity)
                          && (imgBrushA.Stretch == imgBrushB.Stretch)
                          //&& (imgBrushA.TileMode == imgBrushB.TileMode)
                          //&& (imgBrushA.Viewbox == imgBrushB.Viewbox)
                          //&& (imgBrushA.ViewboxUnits == imgBrushB.ViewboxUnits)
                          //&& (imgBrushA.Viewport == imgBrushB.Viewport)
                          //&& (imgBrushA.ViewportUnits == imgBrushB.ViewportUnits)
                          && (imgBrushA.ImageSource == imgBrushB.ImageSource);

            return result;
        }

        return false;
    }

    #endregion

    #region Uri

    /// <summary>
    /// Compares one URI with another URI
    /// </summary>
    /// <param name="uri">URI to compare with</param>
    /// <param name="otherUri">URI to compare</param>
    /// <returns></returns>
    public static bool IsEqualTo(this Uri uri, Uri otherUri)
    {
        return
            Uri.Compare(uri, otherUri, UriComponents.AbsoluteUri, UriFormat.SafeUnescaped,
                StringComparison.OrdinalIgnoreCase) == 0;
    }

    #endregion

    #region Vector2

    /// <summary>
    /// Reflects point 'a' over point 'b'
    /// </summary>
    /// <param name="a">Point to be reflected</param>
    /// <param name="b">Point of reflection</param>
    /// <returns>Reflected point</returns>
    public static Vector2 Reflect(Vector2 a, Vector2 b)
    {
        //
        // Let 'c' be the reflected point. Then point 'b' 
        // becomes the middle point between 'a' and 'c'.
        // As per MidPoint formula,
        // b.X = (a.X + c.X) / 2 and 
        // b.Y = (a.Y + c.Y) / 2
        // Therefore, c.X = 2 * b.X - a.X
        //            c.y = 2 * b.Y - a.Y
        //
        return new Vector2(2f * b.X - a.X,
                           2f * b.Y - a.Y);
    }

    /// <summary>
    /// Converts a Vector2 structure (x,y) to Vector3 structure (x, y, 0)
    /// </summary>
    /// <param name="v">Input Vector2</param>
    /// <returns>Vector3</returns>
    public static Vector3 ToVector3(this Vector2 v)
    {
        return new Vector3(v, 0);
    }

    #endregion

    #region Vector4

    /// <summary>
    /// Verifies if the Vector4 contains only zero values
    /// </summary>
    /// <param name="vector">Vector4</param>
    /// <returns>true if yes, otherwise false</returns>
    public static bool IsZero(this Vector4 vector)
    {
        return vector.X.IsZero()
               && vector.Y.IsZero()
               && vector.Z.IsZero()
               && vector.W.IsZero();
    }

    /// <summary>
    /// Useful in converting the four components
    /// of Thickness or Padding to two components
    /// by taking a sum of alternate components
    /// (X &amp; Z and Y &amp; W).
    /// </summary>
    /// <param name="vector">Vector4</param>
    /// <returns>Vector3</returns>
    public static Vector2 Collapse(this Vector4 vector)
    {
        return new Vector2(vector.X + vector.Z, vector.Y + vector.W);
    }

    /// <summary>
    /// Useful in converting the four components
    /// of Thickness or Padding to two components
    /// by adding alternate components - 
    /// (X &amp; Z and Y &amp; W).
    /// </summary>
    /// <param name="vector">Vector4</param>
    /// <returns>Size</returns>
    public static Size ToSize(this Vector4 vector)
    {
        return new Size(vector.X + vector.Z, vector.Y + vector.W);
    }

    /// <summary>
    /// Converts the Vector4 to Thickness - Left(X), Top(Y), Right(Z), Bottom(W)
    /// </summary>
    /// <param name="vector">Vector4</param>
    /// <returns>Thickness</returns>
    public static Thickness ToThickness(this Vector4 vector)
    {
        return new Thickness(vector.X, vector.Y, vector.Z, vector.W);
    }

    /// <summary>
    /// Converts the Vector4 to CornerRadius - TopLeft(X), TopRight(Y), BottomRight(Z), BottomLeft(W)
    /// </summary>
    /// <param name="vector">Vector4</param>
    /// <returns>CornerRadius</returns>
    public static CornerRadius ToCornerRadius(this Vector4 vector)
    {
        return new CornerRadius(vector.X, vector.Y, vector.Z, vector.W);
    }

    #endregion

    #region Color

    /// <summary>
    /// Calculates the linear interpolated Color based on the given Color values.
    /// </summary>
    /// <param name="colorFrom">Source Color.</param>
    /// <param name="colorTo">Target Color.</param>
    /// <param name="amount">Weightage given to the target color.</param>
    /// <returns>Linear Interpolated Color.</returns>
    public static Color Lerp(this Color colorFrom, Color colorTo, float amount)
    {
        // Convert colorFrom components to lerp-able floats
        float sa = colorFrom.A,
            sr = colorFrom.R,
            sg = colorFrom.G,
            sb = colorFrom.B;

        // Convert colorTo components to lerp-able floats
        float ea = colorTo.A,
            er = colorTo.R,
            eg = colorTo.G,
            eb = colorTo.B;

        // lerp the colors to get the difference
        byte a = (byte)Math.Max(0, Math.Min(255, sa.Lerp(ea, amount))),
            r = (byte)Math.Max(0, Math.Min(255, sr.Lerp(er, amount))),
            g = (byte)Math.Max(0, Math.Min(255, sg.Lerp(eg, amount))),
            b = (byte)Math.Max(0, Math.Min(255, sb.Lerp(eb, amount)));

        // return the new color
        return Color.FromArgb(a, r, g, b);
    }

    /// <summary>
    /// Darkens the color by the given percentage.
    /// </summary>
    /// <param name="color">Source color.</param>
    /// <param name="amount">Percentage to darken. Value should be between 0 and 1.</param>
    /// <returns>Color</returns>
    public static Color DarkerBy(this Color color, float amount)
    {
        return color.Lerp(Colors.Black, amount);
    }

    /// <summary>
    /// Lightens the color by the given percentage.
    /// </summary>
    /// <param name="color">Source color.</param>
    /// <param name="amount">Percentage to lighten. Value should be between 0 and 1.</param>
    /// <returns>Color</returns>
    public static Color LighterBy(this Color color, float amount)
    {
        return color.Lerp(Colors.White, amount);
    }

    #endregion

    #region Point

    /// <summary>
    /// Converts the Point structure P (X,Y) to Vector3 structure
    /// V (P.X, P.Y, 0);
    /// </summary>
    /// <param name="p">Point structure</param>
    /// <returns>Vector3</returns>
    public static Vector3 ToVector3(this Point p)
    {
        return new Vector3((float)p.X, (float)p.Y, 0f);
    }

    #endregion

    #region Stretch and Fit

    /// <summary>
    /// Calculates the best size that can fit in the destination area based on the given stretch and
    /// alignment options.
    /// </summary>
    /// <param name="srcWidth">Width of the source.</param>
    /// <param name="srcHeight">Height of the source.</param>
    /// <param name="destWidth">Width of the destination area.</param>
    /// <param name="destHeight">Height of the destination area.</param>
    /// <param name="stretch">Defines how the source should stretch to fit the destination.</param>
    /// <param name="horizontalAlignment">Horizontal Alignment</param>
    /// <param name="verticalAlignment">Vertical Alignment</param>
    /// <returns>The best fitting Rectangle in the destination area.</returns>
    public static Rect GetOptimumSize(double srcWidth, double srcHeight, double destWidth, double destHeight,
        Stretch stretch, AlignmentX horizontalAlignment, AlignmentY verticalAlignment)
    {
        var ratio = srcWidth / srcHeight;
        var targetWidth = 0d;
        var targetHeight = 0d;

        // Stretch Mode
        switch (stretch)
        {
            case Stretch.None:
                targetWidth = srcWidth;
                targetHeight = srcHeight;
                break;
            case Stretch.Fill:
                targetWidth = destWidth;
                targetHeight = destHeight;
                break;
            case Stretch.Uniform:
                // If width is greater than height
                if (ratio > 1.0)
                {
                    targetHeight = Math.Min(destWidth / ratio, destHeight);
                    targetWidth = targetHeight * ratio;
                }
                else
                {
                    targetWidth = Math.Min(destHeight * ratio, destWidth);
                    targetHeight = targetWidth / ratio;
                }
                break;
            case Stretch.UniformToFill:
                // If width is greater than height
                if (ratio > 1.0)
                {
                    targetHeight = Math.Max(destWidth / ratio, destHeight);
                    targetWidth = targetHeight * ratio;
                }
                else
                {
                    targetWidth = Math.Max(destHeight * ratio, destWidth);
                    targetHeight = targetWidth / ratio;
                }
                break;
        }

        var left = 0d;
        switch (horizontalAlignment)
        {
            case AlignmentX.Left:
                left = 0;
                break;
            case AlignmentX.Center:
                left = (destWidth - targetWidth) / 2.0;
                break;
            case AlignmentX.Right:
                left = destWidth - targetWidth;
                break;
        }

        var top = 0d;
        switch (verticalAlignment)
        {
            case AlignmentY.Top:
                top = 0;
                break;
            case AlignmentY.Center:
                top = (destHeight - targetHeight) / 2.0;
                break;
            case AlignmentY.Bottom:
                top = destHeight - targetHeight;
                break;
        }

        return new Rect(left, top, targetWidth, targetHeight);
    }

    #endregion
}

/// <summary>
/// Class which can be used to encapsulate code statement(s)
/// so that they are executed in a specific culture.
/// <para />
/// Usage example:
/// <para />
/// The following code block will be executed using the French culture.
/// <para />
/// using (new CultureShield("fr-FR"))
/// <para />
/// {
/// <para />
///    ...
/// <para />
/// }
/// </summary>
public partial class CultureShield : IDisposable
{
    #region Fields

    private CultureInfo _prevCulture;

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// Instantiates a culture wrapper so that the encapsulated code
    /// statement(s) can be executed using the specified culture.
    /// <para />
    /// Usage example:
    /// <para />
    /// The following code block will be executed using the French culture.
    /// <para />
    /// using (new CultureShield("fr-FR"))
    /// <para />
    /// {
    /// <para />
    ///   ...
    /// <para />
    /// }
    /// </summary>
    /// <param name="culture">The culture in which the encapsulated code statement(s) are to be executed.</param>
    public CultureShield(string culture)
    {
        _prevCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo(culture);
    }

    #endregion

    #region APIs

    /// <summary>
    /// Disposes the CultureShield object.
    /// </summary>
    public void Dispose()
    {
        CultureInfo.CurrentCulture = _prevCulture;
    }

    #endregion
}
