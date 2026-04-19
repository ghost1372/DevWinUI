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
/// Structure which encapsulates the details of each of the core points
/// of the path of the rounded rectangle which is calculated based on
/// the given CornerRadius, BorderThickness and Padding
/// </summary>
internal struct CompositionPathInfo
{
    //   |--LeftTop----------------------RightTop--|
    //   |                                         |
    // TopLeft                                TopRight
    //   |                                         |
    //   |                                         |
    //   |                                         |
    //   |                                         |
    //   |                                         |
    //   |                                         |
    // BottomLeft                          BottomRight
    //   |                                         |
    //   |--LeftBottom----------------RightBottom--|
    #region Fields

    internal readonly double LeftTop;
    internal readonly double TopLeft;
    internal readonly double TopRight;
    internal readonly double RightTop;
    internal readonly double RightBottom;
    internal readonly double BottomRight;
    internal readonly double BottomLeft;
    internal readonly double LeftBottom;

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="corners">CornerRadius</param>
    /// <param name="borders">BorderThickness</param>
    /// <param name="padding">Padding</param>
    /// <param name="isOuterBorder">Flag to indicate whether outer or inner border needs 
    /// to be calculated</param>
    internal CompositionPathInfo(CornerRadius corners, Thickness borders, Thickness padding, bool isOuterBorder)
    {
        var factor = 0.5;
        var left = factor * (borders.Left + padding.Left);
        var top = factor * (borders.Top + padding.Top);
        var right = factor * (borders.Right + padding.Right);
        var bottom = factor * (borders.Bottom + padding.Bottom);

        if (isOuterBorder)
        {
            if (corners.TopLeft.IsZero())
            {
                LeftTop = TopLeft = 0.0;
            }
            else
            {
                LeftTop = corners.TopLeft + left;
                TopLeft = corners.TopLeft + top;
            }

            if (corners.TopRight.IsZero())
            {
                TopRight = RightTop = 0.0;
            }
            else
            {
                TopRight = corners.TopRight + top;
                RightTop = corners.TopRight + right;
            }

            if (corners.BottomRight.IsZero())
            {
                RightBottom = BottomRight = 0.0;
            }
            else
            {
                RightBottom = corners.BottomRight + right;
                BottomRight = corners.BottomRight + bottom;
            }

            if (corners.BottomLeft.IsZero())
            {
                BottomLeft = LeftBottom = 0.0;
            }
            else
            {
                BottomLeft = corners.BottomLeft + bottom;
                LeftBottom = corners.BottomLeft + left;
            }
        }
        else
        {
            LeftTop = Math.Max(0.0, corners.TopLeft - left);
            TopLeft = Math.Max(0.0, corners.TopLeft - top);
            TopRight = Math.Max(0.0, corners.TopRight - top);
            RightTop = Math.Max(0.0, corners.TopRight - right);
            RightBottom = Math.Max(0.0, corners.BottomRight - right);
            BottomRight = Math.Max(0.0, corners.BottomRight - bottom);
            BottomLeft = Math.Max(0.0, corners.BottomLeft - bottom);
            LeftBottom = Math.Max(0.0, corners.BottomLeft - left);
        }
    }

    #endregion
}
