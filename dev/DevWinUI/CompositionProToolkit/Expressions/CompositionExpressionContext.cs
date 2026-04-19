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

namespace DevWinUI;

/// <summary>
/// This generic class defines a set of helper functions 
/// (which represent all the helper methods supported by ExpressionAnimation). 
/// These methods are primarily used to create the lambda expressions.
/// https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.composition.expressionanimation.aspx
/// </summary>
public sealed partial class CompositionExpressionContext<T>
{
    #region Expression Keywords

    /// <summary>
    /// Represents the 'StartingValue' expression
    /// </summary>
    public T StartingValue { get; }

    /// <summary>
    /// Represents the 'CurrentValue' expression
    /// </summary>
    public T CurrentValue { get; }

    /// <summary>
    /// Represents the 'FinalValue' expression
    /// </summary>
    public T FinalValue { get; }

    /// <summary>
    /// ctor
    /// </summary>
    internal CompositionExpressionContext()
    {
        StartingValue = default(T);
        CurrentValue = default(T);
        FinalValue = default(T);
    }

    #endregion

    #region Scalar Functions

    /// <summary>
    /// Returns the absolute value of a single-precision floating-point number.
    /// </summary>
    /// <param name="value">A number that is greater than or equal to System.Single.MinValue, but less than
    /// or equal to System.Single.MaxValue.</param>
    /// <returns>A single-precision floating-point number, x, such that 0 ≤ x ≤System.Single.MaxValue.</returns>
    public float Abs(float value) => 0;
    /// <summary>
    /// Returns the angle whose cosine is the specified number.
    /// </summary>
    /// <param name="value">A number representing a cosine, which must be greater than or equal to -1,
    /// but less than or equal to 1.</param>
    /// <returns>An angle, θ, measured in radians, such that 0 ≤ θ ≤ π -or- System.Single.NaN if 
    /// the specified value &lt; -1 or specified value &gt; 1 or the specified value equals System.Single.NaN.</returns>
    public float Acos(float value) => 0;
    /// <summary>
    /// Returns the angle whose sine is the specified number.
    /// </summary>
    /// <param name="value">A number representing a sine, which must be greater than or equal to -1,
    /// but less than or equal to 1.</param>
    /// <returns>An angle, θ, measured in radians, such that -π/2 ≤ θ≤ π/2 -or- System.Single.NaN if 
    /// the specified value &lt; -1 or specified value &gt; 1 or the specified value equals System.Single.NaN.</returns>
    public float Asin(float value) => 0;
    /// <summary>
    /// Returns the angle whose tangent is the specified number.
    /// </summary>
    /// <param name="value">A number representing a tangent.</param>
    /// <returns>An angle, θ, measured in radians, such that -π/2 ≤ θ ≤ π/2 -or- System.Single.NaN
    /// if the specified value equals System.Single.NaN, -π/2 rounded to single precision (-1.5707963)
    /// if the specified value equals System.Single.NegativeInfinity, or π/2 rounded to single precision
    /// (1.5707963) if the specified value equals System.Single.PositiveInfinity.</returns>
    public float Atan(float value) => 0;
    /// <summary>
    /// Returns the smallest integral value that is greater than or equal 
    /// to the specified single-precision floating-point number.
    /// </summary>
    /// <param name="value">A single-precision floating-point number.</param>
    /// <returns>The smallest integral value that is greater than or equal to 
    /// the specified value. If the specified value is equal to System.Single.NaN, 
    /// System.Single.NegativeInfinity, or System.Single.PositiveInfinity, that 
    /// value is returned. Note that this method returns a System.Single instead 
    /// of an integral type.</returns>
    public float Ceiling(float value) => 0;
    /// <summary>
    /// Restricts the specified value between the specified min and max values. 
    /// </summary>
    /// <param name="value">A single-precision floating-point number.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>If the specified value is less than the specified min, then
    /// the specified min is returned. If the specified value is greater than
    /// the specified max, then the specified max is returned. Otherwise
    /// the specified value is returned.</returns>
    public float Clamp(float value, float min, float max) => 0;
    /// <summary>
    /// Returns the cosine of the specified angle.
    /// </summary>
    /// <param name="value">An angle, measured in radians.</param>
    /// <returns>The cosine of the specified value. If the specified value 
    /// is equal to System.Single.NaN, System.Single.NegativeInfinity, or
    /// System.Single.PositiveInfinity, this method returns System.Single.NaN.</returns>
    public float Cos(float value) => 0;
    /// <summary>
    /// Returns the Euclidean distance between the two given points.
    /// </summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance between the points.</returns>
    public float Distance(Vector2 value1, Vector2 value2) => 0;
    /// <summary>
    /// Returns the Euclidean distance between the two given points.
    /// </summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance between the points.</returns>
    public float Distance(Vector3 value1, Vector3 value2) => 0;
    /// <summary>
    /// Returns the Euclidean distance between the two given points.
    /// </summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance between the points.</returns>
    public float Distance(Vector4 value1, Vector4 value2) => 0;
    /// <summary>
    /// Returns the Euclidean distance squared between the two given points.
    /// </summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance squared.</returns>
    public float DistanceSquared(Vector2 value1, Vector2 value2) => 0;
    /// <summary>
    /// Returns the Euclidean distance squared between the two given points.
    /// </summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance squared.</returns>
    public float DistanceSquared(Vector3 value1, Vector3 value2) => 0;
    /// <summary>
    /// Returns the Euclidean distance squared between the two given points.
    /// </summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance squared.</returns>
    public float DistanceSquared(Vector4 value1, Vector4 value2) => 0;
    /// <summary>
    /// Returns the largest integer less than or equal to the specified single-precision
    /// floating-point number.
    /// </summary>
    /// <param name="value">A single-precision floating-point number.</param>
    /// <returns>The largest integer less than or equal to the specified value. 
    /// If the specified value is equal to System.Single.NaN, System.Single.NegativeInfinity,
    /// or System.Single.PositiveInfinity, that value  is returned.</returns>
    public float Floor(float value) => 0;
    /// <summary>
    /// Returns the Euclidean distance between the specified point and
    /// origin Vector2(0,0).
    /// </summary>
    /// <param name="value">The specified point.</param>
    /// <returns>The distance.</returns>
    public float Length(Vector2 value) => 0;
    /// <summary>
    /// Returns the Euclidean distance between the specified point and
    /// origin Vector3(0,0,0).
    /// </summary>
    /// <param name="value">The specified point.</param>
    /// <returns>The distance.</returns>
    public float Length(Vector3 value) => 0;
    /// <summary>
    /// Returns the Euclidean distance between the specified point and
    /// origin Vector4(0,0,0,0).
    /// </summary>
    /// <param name="value">The specified point.</param>
    /// <returns>The distance.</returns>
    public float Length(Vector4 value) => 0;
    /// <summary>
    /// Returns the Euclidean distance between the specified Quaternion and
    /// Quaternion(0,0,0,0).
    /// </summary>
    /// <param name="value">The specified Quaternion.</param>
    /// <returns>The distance.</returns>
    public float Length(Quaternion value) => 0;
    /// <summary>
    /// Returns the Euclidean distance squared between the specified point and
    /// origin Vector2(0,0).
    /// </summary>
    /// <param name="value">The specified point.</param>
    /// <returns>The distance.</returns>
    public float LengthSquared(Vector2 value) => 0;
    /// <summary>
    /// Returns the Euclidean distance squared between the specified point and
    /// origin Vector3(0,0,0).
    /// </summary>
    /// <param name="value">The specified point.</param>
    /// <returns>The distance.</returns>
    public float LengthSquared(Vector3 value) => 0;
    /// <summary>
    /// Returns the Euclidean distance squared between the specified Quaternion and
    /// Quaternion(0,0,0,0).
    /// </summary>
    /// <param name="value">The specified Quaternion.</param>
    /// <returns>The distance.</returns>
    public float LengthSquared(Vector4 value) => 0;
    /// <summary>
    /// Returns the Euclidean distance squared between the specified Quaternion and
    /// Quaternion(0,0,0,0).
    /// </summary>
    /// <param name="value">The specified Quaternion.</param>
    /// <returns>The distance.</returns>
    public float LengthSquared(Quaternion value) => 0;
    /// <summary>
    /// Returns the base 'e' logarithm of the specified number.
    /// </summary>
    /// <param name="value">A number whose logarithm is to be found.</param>
    /// <returns>The base 'e' logarithm of the specified number.</returns>
    public float Ln(float value) => 0;
    /// <summary>
    /// Returns the base 10 logarithm of the specified number.
    /// </summary>
    /// <param name="value">A number whose logarithm is to be found.</param>
    /// <returns>The base 10 logarithm of the specified number.</returns>
    public float Log10(float value) => 0;
    /// <summary>
    /// Returns the larger of two single-precision floating-point numbers.
    /// </summary>
    /// <param name="value1">The first of two single-precision floating-point numbers to compare.</param>
    /// <param name="value2">The second of two single-precision floating-point numbers to compare.</param>
    /// <returns>Parameter val1 or val2, whichever is larger. If val1, or val2, or both val1 and
    /// val2 are equal to System.Single.NaN, System.Single.NaN is returned.</returns>
    public float Max(float value1, float value2) => 0;
    /// <summary>
    /// Returns the smaller of two single-precision floating-point numbers.
    /// </summary>
    /// <param name="value1">The first of two single-precision floating-point numbers to compare.</param>
    /// <param name="value2">The second of two single-precision floating-point numbers to compare.</param>
    /// <returns>Parameter val1 or val2, whichever is smaller. If val1, or val2, or both val1 and
    /// val2 are equal to System.Single.NaN, System.Single.NaN is returned.</returns>
    public float Min(float value1, float value2) => 0;
    /// <summary>
    /// Returns the remainder after Euclidean division of the dividend
    /// by the divisor.
    /// </summary>
    /// <param name="dividend">A single-precision floating-point number which is to be divided.</param>
    /// <param name="divisor">A single-precision floating-point number by which the dividend is divided.</param>
    /// <returns>Remainded after division.</returns>
    public float Mod(float dividend, float divisor) => 0;
    /// <summary>
    /// Returns a specified number raised to the specified power.
    /// </summary>
    /// <param name="value">A single-precision floating-point number to be raised to a power.</param>
    /// <param name="power">A single-precision floating-point number that specifies a power.</param>
    /// <returns>The specified value raised to the specified power.</returns>
    public float Pow(float value, int power) => 0;
    /// <summary>
    /// Rounds a single-precision floating-point value to the nearest integral value.
    /// </summary>
    /// <param name="value">A single-precision floating-point number to be rounded.</param>
    /// <returns>The integer nearest to the specified value. If the fractional component of
    /// the specified value is halfway between two integers, one of which is even and the 
    /// other odd, then the even number is returned.
    /// Note that this method returns a System.Single instead of an integral type.</returns>
    public float Round(float value) => 0;
    /// <summary>
    /// Returns the sine of the specified angle.
    /// </summary>
    /// <param name="value">An angle, measured in radians.</param>
    /// <returns>The sine of the specified value. If the specified value is equal to 
    /// System.Single.NaN, System.Single.NegativeInfinity, or System.Single.PositiveInfinity, 
    /// this method returns System.Single.NaN.</returns>
    public float Sin(float value) => 0;
    /// <summary>
    /// Returns the square root of a specified value.
    /// </summary>
    /// <param name="value">The number whose square root is to be found.</param>
    /// <returns>The positive square root of the specified value.</returns>
    public float Sqrt(float value) => 0;
    /// <summary>
    /// Returns the square of a specified value.
    /// </summary>
    /// <param name="value">The number whose square is to be found.</param>
    /// <returns>The square of the specified value.</returns>
    public float Square(float value) => 0;
    /// <summary>
    /// Returns the tangent of the specified angle.
    /// </summary>
    /// <param name="value">An angle, measured in radians.</param>
    /// <returns>The tangent of the specified value. If the specified value is equal to
    /// System.Single.NaN, System.Single.NegativeInfinity, or System.Single.PositiveInfinity, 
    /// this method returns System.Single.NaN.</returns>
    public float Tan(float value) => 0;
    /// <summary>
    /// Converts the specified radians to degrees.
    /// </summary>
    /// <param name="radians">An angle, measured in radians.</param>
    /// <returns>An angle, measured in degrees.</returns>
    public float ToDegrees(float radians) => 0;
    /// <summary>
    /// Converts the specified degrees to radians.
    /// </summary>
    /// <param name="degrees">An angle, measured in degrees.</param>
    /// <returns>An angle, measured in radians.</returns>
    public float ToRadians(float degrees) => 0;

    #endregion

    #region Vector2 Functions

    /// <summary>
    /// Returns a vector whose elements are the absolute values of each 
    /// of the source vector's elements.
    /// </summary>
    /// <param name="value">The source vector.</param>
    /// <returns>The absolute value vector.</returns>
    public Vector2 Abs(Vector2 value) => default(Vector2);
    /// <summary>
    /// Restricts a vector between a min and max value.
    /// </summary>
    /// <param name="value">The source vector.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The clamped value.</returns>
    public Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max) => default(Vector2);
    /// <summary>
    /// Linearly interpolates between two vectors based on the given weighting.
    /// </summary>
    /// <param name="value1">The first source vector.</param>
    /// <param name="value2">The second source vector.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of the second source vector.</param>
    /// <returns>The interpolated vector.</returns>
    public Vector2 Lerp(Vector2 value1, Vector2 value2, float amount) => default(Vector2);
    /// <summary>
    /// Returns a vector whose elements are the maximum of each of the pairs of elements
    /// in the two source vectors.
    /// </summary>
    /// <param name="value1">The first source vector.</param>
    /// <param name="value2">The second source vector.</param>
    /// <returns>The maximized vector.</returns>
    public Vector2 Max(Vector2 value1, Vector2 value2) => default(Vector2);
    /// <summary>
    /// Returns a vector whose elements are the minimum of each of the pairs of elements
    /// in the two source vectors.
    /// </summary>
    /// <param name="value1">The first source vector.</param>
    /// <param name="value2">The second source vector.</param>
    /// <returns>The minimized vector.</returns>
    public Vector2 Min(Vector2 value1, Vector2 value2) => default(Vector2);
    /// <summary>
    /// Returns a vector with the same direction as the given vector, but with a length
    /// of 1.
    /// </summary>
    /// <param name="value">The vector to normalize.</param>
    /// <returns>The normalized vector.</returns>
    public Vector2 Normalize(Vector2 value) => default(Vector2);
    /// <summary>
    /// Scales the components of a vector by the given factor.
    /// </summary>
    /// <param name="value">The source vector.</param>
    /// <param name="factor">Scaling factor.</param>
    /// <returns>The scaled vector.</returns>
    public Vector2 Scale(Vector2 value, float factor) => default(Vector2);
    /// <summary>
    /// Transforms a vector by the given matrix.
    /// </summary>
    /// <param name="value">The source vector.</param>
    /// <param name="matrix">The transformation matrix.</param>
    /// <returns> The transformed vector.</returns>
    public Vector2 Transform(Vector2 value, Matrix3x2 matrix) => default(Vector2);
    /// <summary>
    /// Constructs a vector with the given individual elements.
    /// </summary>
    /// <param name="x">The X component.</param>
    /// <param name="y">The Y component.</param>
    /// <returns>Vector2</returns>
    public Vector2 Vector2(float x, float y) => default(Vector2);

    #endregion

    #region Vector3 Functions

    /// <summary>
    /// Returns a vector whose elements are the absolute values of each 
    /// of the source vector's elements.
    /// </summary>
    /// <param name="value">The source vector.</param>
    /// <returns>The absolute value vector.</returns>
    public Vector3 Abs(Vector3 value) => new Vector3(0);
    /// <summary>
    /// Restricts a vector between a min and max value.
    /// </summary>
    /// <param name="value">The source vector.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The clamped value.</returns>
    public Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max) => default(Vector3);
    /// <summary>
    /// Linearly interpolates between two vectors based on the given weighting.
    /// </summary>
    /// <param name="value1">The first source vector.</param>
    /// <param name="value2">The second source vector.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of the second source vector.</param>
    /// <returns>The interpolated vector.</returns>
    public Vector3 Lerp(Vector3 value1, Vector3 value2, float amount) => default(Vector3);
    /// <summary>
    /// Returns a vector whose elements are the maximum of each of the pairs of elements
    /// in the two source vectors.
    /// </summary>
    /// <param name="value1">The first source vector.</param>
    /// <param name="value2">The second source vector.</param>
    /// <returns>The maximized vector.</returns>
    public Vector3 Max(Vector3 value1, Vector3 value2) => default(Vector3);
    /// <summary>
    /// Returns a vector whose elements are the minimum of each of the pairs of elements
    /// in the two source vectors.
    /// </summary>
    /// <param name="value1">The first source vector.</param>
    /// <param name="value2">The second source vector.</param>
    /// <returns>The minimized vector.</returns>
    public Vector3 Min(Vector3 value1, Vector3 value2) => default(Vector3);
    /// <summary>
    /// Returns a vector with the same direction as the given vector, but with a length
    /// of 1.
    /// </summary>
    /// <param name="value">The vector to normalize.</param>
    /// <returns>The normalized vector.</returns>
    public Vector3 Normalize(Vector3 value) => default(Vector3);
    /// <summary>
    /// Scales the components of a vector by the given factor.
    /// </summary>
    /// <param name="value">The source vector.</param>
    /// <param name="factor">Scaling factor.</param>
    /// <returns>The scaled vector.</returns>
    public Vector3 Scale(Vector3 value, float factor) => default(Vector3);
    /// <summary>
    /// Transforms a vector by the given matrix.
    /// </summary>
    /// <param name="value">The source vector.</param>
    /// <param name="matrix">The transformation matrix.</param>
    /// <returns> The transformed vector.</returns>
    public Vector3 Transform(Vector3 value, Matrix4x4 matrix) => default(Vector3);
    /// <summary>
    /// Constructs a vector with the given individual elements.
    /// </summary>
    /// <param name="x">The X component.</param>
    /// <param name="y">The Y component.</param>
    /// <param name="z">The Z component.</param>
    /// <returns>Vector2</returns>
    public Vector3 Vector3(float x, float y, float z) => default(Vector3);

    #endregion

    #region Vector4 Functions

    /// <summary>
    /// Returns a vector whose elements are the absolute values of each 
    /// of the source vector's elements.
    /// </summary>
    /// <param name="value">The source vector.</param>
    /// <returns>The absolute value vector.</returns>
    public Vector4 Abs(Vector4 value) => new Vector4(0);
    /// <summary>
    /// Restricts a vector between a min and max value.
    /// </summary>
    /// <param name="value">The source vector.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The clamped value.</returns>
    public Vector4 Clamp(Vector4 value, Vector4 min, Vector4 max) => default(Vector4);
    /// <summary>
    /// Linearly interpolates between two vectors based on the given weighting.
    /// </summary>
    /// <param name="value1">The first source vector.</param>
    /// <param name="value2">The second source vector.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of the second source vector.</param>
    /// <returns>The interpolated vector.</returns>
    public Vector4 Lerp(Vector4 value1, Vector4 value2, float amount) => default(Vector4);
    /// <summary>
    /// Returns a vector whose elements are the maximum of each of the pairs of elements
    /// in the two source vectors.
    /// </summary>
    /// <param name="value1">The first source vector.</param>
    /// <param name="value2">The second source vector.</param>
    /// <returns>The maximized vector.</returns>
    public Vector4 Max(Vector4 value1, Vector4 value2) => default(Vector4);
    /// <summary>
    /// Returns a vector whose elements are the minimum of each of the pairs of elements
    /// in the two source vectors.
    /// </summary>
    /// <param name="value1">The first source vector.</param>
    /// <param name="value2">The second source vector.</param>
    /// <returns>The minimized vector.</returns>
    public Vector4 Min(Vector4 value1, Vector4 value2) => default(Vector4);
    /// <summary>
    /// Returns a vector with the same direction as the given vector, but with a length
    /// of 1.
    /// </summary>
    /// <param name="value">The vector to normalize.</param>
    /// <returns>The normalized vector.</returns>
    public Vector4 Normalize(Vector4 value) => default(Vector4);
    /// <summary>
    /// Scales the components of a vector by the given factor.
    /// </summary>
    /// <param name="value">The source vector.</param>
    /// <param name="factor">Scaling factor.</param>
    /// <returns>The scaled vector.</returns>
    public Vector4 Scale(Vector4 value, float factor) => default(Vector4);
    /// <summary>
    /// Transforms a vector by the given matrix.
    /// </summary>
    /// <param name="value">The source vector.</param>
    /// <param name="matrix">The transformation matrix.</param>
    /// <returns> The transformed vector.</returns>
    public Vector4 Transform(Vector4 value, Matrix4x4 matrix) => default(Vector4);
    /// <summary>
    /// Constructs a vector with the given individual elements.
    /// </summary>
    /// <param name="x">The X component.</param>
    /// <param name="y">The Y component.</param>
    /// <param name="z">The Z component.</param>
    /// <param name="w">The W component.</param>
    /// <returns>Vector2</returns>
    public Vector4 Vector4(float x, float y, float z, float w) => default(Vector4);

    #endregion

    #region Matrix3x2 Functions

    /// <summary>
    /// Creates a rotation matrix using the given rotation in radians.
    /// </summary>
    /// <param name="radians">The amount of rotation, in radians.</param>
    /// <returns>A rotation matrix.</returns>
    public Matrix3x2 CreateRotation(float radians) => default(Matrix3x2);
    /// <summary>
    /// Creates a scale matrix from the given vector.
    /// </summary>
    /// <param name="scale">The scale vector.</param>
    /// <returns>A scaling matrix.</returns>
    public Matrix3x2 CreateScale(Vector2 scale) => default(Matrix3x2);
    /// <summary>
    /// Creates a skew matrix from the given angles in radians and a center point.
    /// </summary>
    /// <param name="radiansX">The X angle, in radians.</param>
    /// <param name="radiansY">The Y angle, in radians.</param>
    /// <param name="centerPoint">The center point.</param>
    /// <returns>A skew matrix.</returns>
    public Matrix3x2 CreateSkew(float radiansX, float radiansY, Vector2 centerPoint) => default(Matrix3x2);
    /// <summary>
    /// Creates a translation matrix from the given vector.
    /// </summary>
    /// <param name="translation">The translation vector.</param>
    /// <returns>A translation matrix.</returns>
    public Matrix3x2 CreateTranslation(Vector2 translation) => default(Matrix3x2);
    /// <summary>
    /// Returns a Matrix3x2 object that represents the reciprocal matrix.
    /// </summary>
    /// <param name="value">The source matrix.</param>
    /// <returns>Inverted matrix.</returns>
    public Matrix3x2 Inverse(Matrix3x2 value) => default(Matrix3x2);
    /// <summary>
    /// Linearly interpolates from matrix1 to matrix2, based on the specified weight.
    /// </summary>
    /// <param name="value1">The first source matrix.</param>
    /// <param name="value2">The second source matrix.</param>
    /// <param name="progress">Value between 0 and 1 indicating the relative weight 
    /// of the second source matrix.</param>
    /// <returns>The interpolated matrix.</returns>
    public Matrix3x2 Lerp(Matrix3x2 value1, Matrix3x2 value2, float progress) => default(Matrix3x2);
    /// <summary>
    /// Constructs a Matrix3x2 from the given components.
    /// </summary>
    /// <param name="m11">Row 1 Col 1</param>
    /// <param name="m12">Row 1 Col 2</param>
    /// <param name="m21">Row 2 Col 1</param>
    /// <param name="m22">Row 2 Col 2</param>
    /// <param name="m31">Row 3 Col 1</param>
    /// <param name="m32">Row 3 Col 2</param>
    /// <returns>Matrix3x2</returns>
    public Matrix3x2 Matrix3x2(float m11, float m12, float m21, float m22, float m31, float m32) => default(Matrix3x2);
    /// <summary>
    /// Constructs a Matrix3x2 from a Vector2 representing scale[scale.X, 0.0 0.0, scale.Y 0.0, 0.0 ]
    /// </summary>
    /// <param name="scale">The scale vector.</param>
    /// <returns>The scaled matrix.</returns>
    public Matrix3x2 Matrix3x2CreateFromScale(Vector2 scale) => default(Matrix3x2);
    /// <summary>
    /// Constructs a Matrix3x2 from a Vector2 representing translation[1.0, 0.0, 0.0, 1.0, translation.X, translation.Y]
    /// </summary>
    /// <param name="translation">The translation vector.</param>
    /// <returns>The translated matrix.</returns>
    public Matrix3x2 Matrix3x2CreateFromTranslation(Vector2 translation) => default(Matrix3x2);
    /// <summary>
    /// Returns a Matrix3x2 with each component of the matrix multiplied by the scaling factor.
    /// </summary>
    /// <param name="value">The source matrix.</param>
    /// <param name="factor">The scaling factor.</param>
    /// <returns>The scaled matrix.</returns>
    public Matrix3x2 Scale(Matrix3x2 value, float factor) => default(Matrix3x2);

    #endregion

    #region Matrix4x4 Functions

    /// <summary>
    /// Creates a scale matrix from the given vector.
    /// </summary>
    /// <param name="scale">The scale vector.</param>
    /// <returns>A scaling matrix.</returns>
    public Matrix4x4 CreateScale(Vector3 scale) => default(Matrix4x4);
    /// <summary>
    /// Creates a translation matrix from the given vector.
    /// </summary>
    /// <param name="translation">The translation vector.</param>
    /// <returns>A translation matrix.</returns>
    public Matrix4x4 CreateTranslation(Vector3 translation) => default(Matrix4x4);
    /// <summary>
    /// Linearly interpolates from matrix1 to matrix2, based on the specified weight.
    /// </summary>
    /// <param name="value1">The first source matrix.</param>
    /// <param name="value2">The second source matrix.</param>
    /// <param name="progress">Value between 0 and 1 indicating the relative weight 
    /// of the second source matrix.</param>
    /// <returns>The interpolated matrix.</returns>
    public Matrix4x4 Lerp(Matrix4x4 value1, Matrix4x4 value2, float progress) => default(Matrix4x4);
    /// <summary>
    /// Constructs a Matrix4x4 object from the given components.
    /// </summary>
    /// <param name="m11">Row 1 Col 1</param>
    /// <param name="m12">Row 1 Col 2</param>
    /// <param name="m13">Row 1 Col 3</param>
    /// <param name="m14">Row 1 Col 4</param>
    /// <param name="m21">Row 2 Col 1</param>
    /// <param name="m22">Row 2 Col 2</param>
    /// <param name="m23">Row 2 Col 3</param>
    /// <param name="m24">Row 2 Col 4</param>
    /// <param name="m31">Row 3 Col 1</param>
    /// <param name="m32">Row 3 Col 2</param>
    /// <param name="m33">Row 3 Col 3</param>
    /// <param name="m34">Row 3 Col 4</param>
    /// <param name="m41">Row 4 Col 1</param>
    /// <param name="m42">Row 4 Col 2</param>
    /// <param name="m43">Row 4 Col 3</param>
    /// <param name="m44">Row 4 Col 4</param>
    /// <returns>Matrix4x4</returns>
    public Matrix4x4 Matrix4x4(
        float m11, float m12, float m13, float m14,
        float m21, float m22, float m23, float m24,
        float m31, float m32, float m33, float m34,
        float m41, float m42, float m43, float m44) => default(Matrix4x4);
    /// <summary>
    /// Constructs a Matrix4x4 using a Matrix3x2
    /// [matrix.11, matrix.12, 0, 0, 
    ///  matrix.21, matrix.22, 0, 0, 
    ///  0, 0, 1, 0, 
    ///  matrix.31, matrix.32, 0, 1]
    /// </summary>
    /// <param name="matrix">The source Matrix3x2.</param>
    /// <returns>Matrix4x4</returns>
    public Matrix4x4 Matrix4x4(Matrix3x2 matrix) => default(Matrix4x4);
    /// <summary>
    /// Constructs a Matrix4x4 from a Vector3 representing scale
    /// [scale.X, 0.0, 0.0, 0.0, 
    ///  0.0, scale.Y, 0.0, 0.0, 
    ///  0.0, 0.0, scale.Z, 0.0, 
    ///  0.0, 0.0, 0.0, 1.0]
    /// </summary>
    /// <param name="scale">The scale vector.</param>
    /// <returns>The scaled matrix.</returns>
    public Matrix4x4 Matrix4x4CreateFromScale(Vector3 scale) => default(Matrix4x4);
    /// <summary>
    /// Constructs a Matrix4x4 from a Vector3 representing translation
    /// [1.0, 0.0, 0.0, 0.0, 
    ///  0.0, 1.0, 0.0, 0.0, 
    ///  0.0, 0.0, 1.0, 0.0, 
    /// translation.X, translation.Y, translation.Z, 1.0]
    /// </summary>
    /// <param name="translation">The translation vector.</param>
    /// <returns>The translated matrix.</returns>
    public Matrix4x4 Matrix4x4CreateFromTranslation(Vector3 translation) => default(Matrix4x4);
    /// <summary>
    /// Creates a matrix that rotates around an arbitrary vector.
    /// </summary>
    /// <param name="axis">The axis to rotate around.</param>
    /// <param name="angle">The angle to rotate around the given axis, in radians.</param>
    /// <returns>The rotation matrix.</returns>
    public Matrix4x4 Matrix4x4CreateFromAxisAngle(Vector3 axis, float angle) => default(Matrix4x4);
    /// <summary>
    /// Returns a Matrix 4x4 with each component of the matrix multiplied by the scaling factor.
    /// </summary>
    /// <param name="value">The source matrix.</param>
    /// <param name="factor">The scaling factor.</param>
    /// <returns>The scaled matrix.</returns>
    public Matrix4x4 Scale(Matrix4x4 value, float factor) => default(Matrix4x4);

    #endregion

    #region Quaternion Functions

    /// <summary>
    /// Concatenates two Quaternions; the result represents the value1 rotation followed
    /// by the value2 rotation.
    /// </summary>
    /// <param name="value1">The first Quaternion rotation in the series.</param>
    /// <param name="value2">The second Quaternion rotation in the series.</param>
    /// <returns>A new Quaternion representing the concatenation of the value1 rotation followed
    /// by the value2 rotation.</returns>
    public Quaternion Concatenate(Quaternion value1, Quaternion value2) => default(Quaternion);
    /// <summary>
    /// Creates a Quaternion from a normalized vector axis and an angle to rotate about
    /// the vector.
    /// </summary>
    /// <param name="axis">The unit vector to rotate around. This vector must be normalized before calling
    /// this function or the resulting Quaternion will be incorrect.</param>
    /// <param name="angle">The angle, in radians, to rotate around the vector.</param>
    /// <returns>The created Quaternion.</returns>
    public Quaternion CreateFromAxisAngle(Vector3 axis, float angle) => default(Quaternion);
    /// <summary>
    /// Linearly interpolates between two quaternions.
    /// </summary>
    /// <param name="value1">The first source Quaternion.</param>
    /// <param name="value2">The second source Quaternion.</param>
    /// <param name="amount">Value between 0 and 1 indicating the relative weight of the second 
    /// source Quaternion in the interpolation.</param>
    /// <returns>The interpolated Quaternion.</returns>
    public Quaternion Lerp(Quaternion value1, Quaternion value2, float amount) => default(Quaternion);
    /// <summary>
    /// Divides each component of the Quaternion by the length of the Quaternion.
    /// </summary>
    /// <param name="value">The source Quaternion.</param>
    /// <returns>The normalized Quaternion.</returns>
    public Quaternion Normalize(Quaternion value) => default(Quaternion);
    /// <summary>
    /// Constructs a Quaternion from the given components.
    /// </summary>
    /// <param name="x">The X component of the Quaternion.</param>
    /// <param name="y">The Y component of the Quaternion.</param>
    /// <param name="z">The Z component of the Quaternion.</param>
    /// <param name="w">The W component of the Quaternion.</param>
    /// <returns>Quaternion.</returns>
    public Quaternion Quaternion(float x, float y, float z, float w) => default(Quaternion);
    /// <summary>
    /// Interpolates between two quaternions, using spherical linear interpolation.
    /// </summary>
    /// <param name="value1">The first source Quaternion.</param>
    /// <param name="value2">The second source Quaternion.</param>
    /// <param name="amount">Value between 0 and 1 indicating the relative weight of the second 
    /// source Quaternion in the interpolation.</param>
    /// <returns>The interpolated Quaternion.</returns>
    public Quaternion Slerp(Quaternion value1, Quaternion value2, float amount) => default(Quaternion);

    #endregion

    #region Color

    /// <summary>
    /// Linearly interpolates between two Colors based on the given weighting.
    /// </summary>
    /// <param name="colorTo">Source color.</param>
    /// <param name="colorFrom">Target color.</param>
    /// <param name="progression">Value between 0 and 1 indicating the weight of the target color.</param>
    /// <returns></returns>
    public Color ColorLerp(Color colorTo, Color colorFrom, float progression) => default(Color);
    /// <summary>
    /// Linearly interpolates between two Colors based on the given weighting.
    /// </summary>
    /// <param name="colorTo">Source color.</param>
    /// <param name="colorFrom">Target color.</param>
    /// <param name="progression">Value between 0 and 1 indicating the weight of the target color.</param>
    /// <returns></returns>
    public Color ColorLerpHSL(Color colorTo, Color colorFrom, float progression) => default(Color);
    /// <summary>
    /// Linearly interpolates between two Colors based on the given weighting.
    /// </summary>
    /// <param name="colorTo">Source color.</param>
    /// <param name="colorFrom">Target color.</param>
    /// <param name="progression">Value between 0 and 1 indicating the weight of the target color.</param>
    /// <returns></returns>
    public Color ColorLerpRGB(Color colorTo, Color colorFrom, float progression) => default(Color);
    /// <summary>
    /// Constructs a Color object from the specified Hue, Saturation and Luminance components.
    /// </summary>
    /// <param name="h">Hue</param>
    /// <param name="s">Saturation</param>
    /// <param name="l">Luminance</param>
    /// <returns>Color</returns>
    public Color ColorHsl(float h, float s, float l) => default(Color);
    /// <summary>
    /// Constructs a Color object from the specified Alpha, Red, Green and Blue components.
    /// </summary>
    /// <param name="a">Alpha</param>
    /// <param name="r">Red</param>
    /// <param name="g">Green</param>
    /// <param name="b">Blue</param>
    /// <returns>Color</returns>
    public Color ColorRgb(byte a, byte r, byte g, byte b) => default(Color);

    #endregion

    #region Expression Templates - Targets

    /// <summary>
    /// Returns an AmbientLightTarget object.
    /// </summary>
    /// <returns>AmbientLightTarget object.</returns>
    public AmbientLightTarget AmbientLightTarget()
    {
        return new AmbientLightTarget();
    }

    /// <summary>
    /// Returns a ColorBrushTarget object.
    /// </summary>
    /// <returns>ColorBrushTarget object.</returns>
    public ColorBrushTarget ColorBrushTarget()
    {
        return new ColorBrushTarget();
    }

    /// <summary>
    /// Returns a DistantLightTarget object.
    /// </summary>
    /// <returns>DistantLightTarget object.</returns>
    public DistantLightTarget DistantLightTarget()
    {
        return new DistantLightTarget();
    }

    /// <summary>
    /// Returns a DropShadowTarget object.
    /// </summary>
    /// <returns>DropShadowTarget object.</returns>
    public DropShadowTarget DropShadowTarget()
    {
        return new DropShadowTarget();
    }

    /// <summary>
    /// Returns a InsetClipTarget object.
    /// </summary>
    /// <returns>InsetClipTarget object.</returns>
    public InsetClipTarget InsetClipTarget()
    {
        return new InsetClipTarget();
    }

    /// <summary>
    /// Returns a InteractionTrackerTarget object.
    /// </summary>
    /// <returns>InteractionTrackerTarget object.</returns>
    public InteractionTrackerTarget InteractionTrackerTarget()
    {
        return new InteractionTrackerTarget();
    }

    /// <summary>
    /// Returns a ManipulationPropertySetTarget object.
    /// </summary>
    /// <returns>ManipulationPropertySetTarget object.</returns>
    public ManipulationPropertySetTarget ManipulationPropertySetTarget()
    {
        return new ManipulationPropertySetTarget();
    }

    /// <summary>
    /// Returns a NineGridBrushTarget object.
    /// </summary>
    /// <returns>NineGridBrushTarget object.</returns>
    public NineGridBrushTarget NineGridBrushTarget()
    {
        return new NineGridBrushTarget();
    }

    /// <summary>
    /// Returns a PointerPositionPropertySetTarget object.
    /// </summary>
    /// <returns>PointerPositionPropertySetTarget object.</returns>
    public PointerPositionPropertySetTarget PointerPositionPropertySetTarget()
    {
        return new PointerPositionPropertySetTarget();
    }

    /// <summary>
    /// Returns a PointLightTarget object.
    /// </summary>
    /// <returns>PointLightTarget object.</returns>
    public PointLightTarget PointLightTarget()
    {
        return new PointLightTarget();
    }

    /// <summary>
    /// Returns a SpotLightTarget object.
    /// </summary>
    /// <returns>SpotLightTarget object.</returns>
    public SpotLightTarget SpotLightTarget()
    {
        return new SpotLightTarget();
    }

    /// <summary>
    /// Returns a SurfaceBrushTarget object.
    /// </summary>
    /// <returns>SurfaceBrushTarget object.</returns>
    public SurfaceBrushTarget SurfaceBrushTarget()
    {
        return new SurfaceBrushTarget();
    }

    /// <summary>
    /// Returns a VisualTarget object.
    /// </summary>
    /// <returns>VisualTarget object.</returns>
    public VisualTarget VisualTarget()
    {
        return new VisualTarget();
    }

    #endregion

    #region Expression Templates - References

    /// <summary>
    /// Returns a reference template to an AmbientLight object with the specified reference name.
    /// </summary>
    /// <param name="referenceName">The name of the AmbientLightReference object.</param>
    /// <returns>AmbientLightReference object.</returns>
    public AmbientLightReference AmbientLightReference(string referenceName)
    {
        return new AmbientLightReference(referenceName);
    }

    /// <summary>
    /// Returns a reference template to a CompositionColorBrush object with the specified reference name.
    /// </summary>
    /// <param name="referenceName">The name of the ColorBrushReference object.</param>
    /// <returns>ColorBrushReference object.</returns>
    public ColorBrushReference ColorBrushReference(string referenceName)
    {
        return new ColorBrushReference(referenceName);
    }

    /// <summary>
    /// Returns a reference template to a DistantLight object with the specified reference name.
    /// </summary>
    /// <param name="referenceName">The name of the DistantLightReference object.</param>
    /// <returns>DistantLightReference object.</returns>
    public DistantLightReference DistantLightReference(string referenceName)
    {
        return new DistantLightReference(referenceName);
    }

    /// <summary>
    /// Returns a reference template to a DropShadow object with the specified reference name.
    /// </summary>
    /// <param name="referenceName">The name of the DropShadowReference object.</param>
    /// <returns>DropShadowReference object.</returns>
    public DropShadowReference DropShadowReference(string referenceName)
    {
        return new DropShadowReference(referenceName);
    }

    /// <summary>
    /// Returns a reference template to a InsetClip object with the specified reference name.
    /// </summary>
    /// <param name="referenceName">The name of the InsetClipReference object.</param>
    /// <returns>InsetClipReference object.</returns>
    public InsetClipReference InsetClipReference(string referenceName)
    {
        return new InsetClipReference(referenceName);
    }

    /// <summary>
    /// Returns a reference template to a InteractionTracker object with the specified reference name.
    /// </summary>
    /// <param name="referenceName">The name of the InteractionTrackerReference object.</param>
    /// <returns>InteractionTrackerReference object.</returns>
    public InteractionTrackerReference InteractionTrackerReference(string referenceName)
    {
        return new InteractionTrackerReference(referenceName);
    }

    /// <summary>
    /// Returns a reference template to a ManipulationPropertySet object with the specified reference name.
    /// </summary>
    /// <param name="referenceName">The name of the ManipulationPropertySetReference object.</param>
    /// <returns>ManipulationPropertySetReference object.</returns>
    public ManipulationPropertySetReference ManipulationPropertySetReference(string referenceName)
    {
        return new ManipulationPropertySetReference(referenceName);
    }

    /// <summary>
    /// Returns a reference template to a CompositionNineGridBrush object with the specified reference name.
    /// </summary>
    /// <param name="referenceName">The name of the NineGridBrushReference object.</param>
    /// <returns>NineGridBrushReference object.</returns>
    public NineGridBrushReference NineGridBrushReference(string referenceName)
    {
        return new NineGridBrushReference(referenceName);
    }

    /// <summary>
    /// Returns a reference template to a PointerPositionPropertySet object with the specified reference name.
    /// </summary>
    /// <param name="referenceName">The name of the PointerPositionPropertySetReference object.</param>
    /// <returns>PointerPositionPropertySetReference object.</returns>
    public PointerPositionPropertySetReference PointerPositionPropertySetReference(string referenceName)
    {
        return new PointerPositionPropertySetReference(referenceName);
    }

    /// <summary>
    /// Returns a reference template to a PointLight object with the specified reference name.
    /// </summary>
    /// <param name="referenceName">The name of the PointLightReference object.</param>
    /// <returns>PointLightReference object.</returns>
    public PointLightReference PointLightReference(string referenceName)
    {
        return new PointLightReference(referenceName);
    }

    /// <summary>
    /// Returns a reference template to a SpotLight object with the specified reference name.
    /// </summary>
    /// <param name="referenceName">The name of the SpotLightReference object.</param>
    /// <returns>SpotLightReference object.</returns>
    public SpotLightReference SpotLightReference(string referenceName)
    {
        return new SpotLightReference(referenceName);
    }

    /// <summary>
    /// Returns a reference template to a CompositionSurfaceBrush object with the specified reference name.
    /// </summary>
    /// <param name="referenceName">The name of the SurfaceBrushReference object.</param>
    /// <returns>SurfaceBrushReference object.</returns>
    public SurfaceBrushReference SurfaceBrushReference(string referenceName)
    {
        return new SurfaceBrushReference(referenceName);
    }

    /// <summary>
    /// Returns a reference template to a Visual object with the specified reference name.
    /// </summary>
    /// <param name="referenceName">The name of the VisualReference object.</param>
    /// <returns>VisualReference object.</returns>
    public VisualReference VisualReference(string referenceName)
    {
        return new VisualReference(referenceName);
    }

    #endregion
}
