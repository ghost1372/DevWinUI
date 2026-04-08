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

using System.Text;
using System.Text.RegularExpressions;

namespace DevWinUI;

/// <summary>
/// Class representing the Smooth Cubic Bezier Element in a Path Geometry
/// </summary>
internal partial class SmoothCubicBezierElement : AbstractPathElement
{
    #region Fields

    private float _x2;
    private float _y2;
    private float _x;
    private float _y;
    private Vector2 _absoluteControlPoint2;

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// ctor
    /// </summary>
    public SmoothCubicBezierElement()
    {
        _x2 = _y2 = 0;
        _x = _y = 0;
    }

    #endregion

    #region APIs

    /// <summary>
    /// Adds the Path Element to the Path.
    /// </summary>
    /// <param name="pathBuilder">CanvasPathBuilder object</param>
    /// <param name="currentPoint">The last active location in the Path before adding 
    /// the Path Element</param>
    /// <param name="lastElement">The previous PathElement in the Path.</param>
    /// <param name="logger">For logging purpose. To log the set of CanvasPathBuilder 
    /// commands, used for creating the CanvasGeometry, in string format.</param>
    /// <returns>The latest location in the Path after adding the Path Element</returns>
    public override Vector2 CreatePath(CanvasPathBuilder pathBuilder, Vector2 currentPoint, 
        ref ICanvasPathElement lastElement, StringBuilder logger)
    {
        // Calculate coordinates
        // Check if the last element was a Cubic Bezier
        Vector2 controlPoint1;
        var cubicBezier = lastElement as CubicBezierElement;
        if (cubicBezier != null)
        {
            // Reflect the second control point of the cubic bezier over the current point. The 
            // resulting point will be the first control point of this Bezier.
            controlPoint1 = Utils.Reflect(cubicBezier.GetControlPoint(), currentPoint);
        }
        // Or if the last element was s Smooth Cubic Bezier
        else
        {
            var smoothCubicBezier = lastElement as SmoothCubicBezierElement;
            // If the last element was a Smooth Cubic Bezier then reflect its second control point 
            // over the current point. The resulting point will be the first control point of this
            // Bezier. Otherwise, if the last element was not a Smooth Cubic Bezier then the 
            // currentPoint will be the first control point of this Bezier
            controlPoint1 = smoothCubicBezier != null
                ? Utils.Reflect(smoothCubicBezier.GetControlPoint(), currentPoint)
                : currentPoint;
        }

        var controlPoint2 = new Vector2(_x2, _y2);
        var point = new Vector2(_x, _y);

        if (IsRelative)
        {
            controlPoint2 += currentPoint;
            point += currentPoint;
        }

        // Save the second absolute control point so that it can be used by the following
        // SmoothCubicBezierElement (if any)
        _absoluteControlPoint2 = controlPoint2;

        // Execute command
        pathBuilder.AddCubicBezier(controlPoint1, controlPoint2, point);

        // Log command
        logger?.Append($"{Indent}pathBuilder.AddCubicBezier(new Vector2({controlPoint1.X}, {controlPoint1.Y})");
        logger?.Append($", new Vector2({controlPoint2.X}, {controlPoint2.Y})");
        logger?.AppendLine($", new Vector2({point.X}, {point.Y}));");

        // Set Last Element
        lastElement = this;
        // Return current point
        return point;
    }

    /// <summary>
    /// Gets the Second Control Point of this Cubic Bezier
    /// </summary>
    /// <returns>Vector2</returns>
    public Vector2 GetControlPoint()
    {
        return _absoluteControlPoint2;
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Get the Regex for extracting Path Element Attributes
    /// </summary>
    /// <returns></returns>
    protected override Regex GetAttributesRegex()
    {
        return RegexFactory.GetAttributesRegex(PathElementType.SmoothCubicBezier);
    }

    /// <summary>
    /// Gets the Path Element Attributes from the Match
    /// </summary>
    /// <param name="match">Match object</param>
    protected override void GetAttributes(Match match)
    {
        Single.TryParse(match.Groups["X2"].Value, out _x2);
        Single.TryParse(match.Groups["Y2"].Value, out _y2);
        Single.TryParse(match.Groups["X"].Value, out _x);
        Single.TryParse(match.Groups["Y"].Value, out _y);
    }

    #endregion
}
