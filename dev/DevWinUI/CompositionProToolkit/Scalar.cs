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
/// Class containing some constants
/// represented as Floating point numbers.
/// </summary>
public static partial class Scalar
{
    // Pi related floating point constants

    /// <summary>
    /// (float)Math.PI radians ( or 180 degrees).
    /// </summary>
    public const float Pi = (float)Math.PI;
    /// <summary>
    /// Two times (float)Math.PI radians ( or 360 degrees).
    /// </summary>
    public const float TwoPi = 2f * Pi;
    /// <summary>
    /// Half of (float)Math.PI radians ( or 90 degrees).
    /// </summary>
    public const float PiByTwo = Pi / 2f;
    /// <summary>
    /// One third of (float)Math.PI radians ( or 60 degrees).
    /// </summary>
    public const float PiByThree = Pi / 3f;
    /// <summary>
    /// One fourth of (float)Math.PI radians ( or 45 degrees).
    /// </summary>
    public const float PiByFour = Pi / 4f;
    /// <summary>
    /// One sixth of (float)Math.PI radians ( or 30 degrees).
    /// </summary>
    public const float PiBySix = Pi / 6f;
    /// <summary>
    /// Three times half of (float)Math.PI radians ( or 270 degrees).
    /// </summary>
    public const float ThreePiByTwo = 3f * Pi / 2f;

    // Conversion constants        

    /// <summary>
    /// 1 degree in radians.
    /// </summary>
    public const float DegreesToRadians = Pi / 180f;
    /// <summary>
    /// 1 radian in degrees.
    /// </summary>
    public const float RadiansToDegrees = 180f / Pi;
}
