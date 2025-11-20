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
/// Class representing the Arc Element in a Path Geometry
/// </summary>
internal partial class ArcElement : AbstractPathElement
{
    #region Fields

    private float _radiusX;
    private float _radiusY;
    private float _angle;
    private CanvasArcSize _arcSize;
    private CanvasSweepDirection _sweepDirection;
    private float _x;
    private float _y;

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// ctor
    /// </summary>
    public ArcElement()
    {
        _radiusX = _radiusY = _angle = 0;
        _arcSize = CanvasArcSize.Small;
        _sweepDirection = CanvasSweepDirection.Clockwise;
        _sweepDirection = 0;
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
        var point = new Vector2(_x, _y);
        if (IsRelative)
        {
            point += currentPoint;
        }

        // Execute command
        pathBuilder.AddArc(point, _radiusX, _radiusY, _angle, _sweepDirection, _arcSize);

        // Log command
        logger?.Append($"{Indent}pathBuilder.AddArc(new Vector2({point.X}, {point.Y}), ");
        logger?.AppendLine($"{_radiusX}, {_radiusY}, {_angle}, {_sweepDirection}, {_arcSize});");

        // Set Last Element
        lastElement = this;
        // Return current point
        return point;
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Get the Regex for extracting Path Element Attributes
    /// </summary>
    /// <returns></returns>
    protected override Regex GetAttributesRegex()
    {
        return RegexFactory.GetAttributesRegex(PathElementType.Arc);
    }

    /// <summary>
    /// Gets the Path Element Attributes from the Match
    /// </summary>
    /// <param name="match">Match object</param>
    protected override void GetAttributes(Match match)
    {
        Single.TryParse(match.Groups["RadiusX"].Value, out _radiusX);
        // Sanitize by taking only the absolute value
        _radiusX = Math.Abs(_radiusX);
        Single.TryParse(match.Groups["RadiusY"].Value, out _radiusY);
        // Sanitize by taking only the absolute value
        _radiusY = Math.Abs(_radiusY);
        // Angle is provided in degrees
        Single.TryParse(match.Groups["Angle"].Value, out _angle);
        // Convert angle to radians as CanvasPathBuilder.AddArc() method
        // requires the angle to be in radians
        _angle *= Scalar.DegreesToRadians;
        Enum.TryParse(match.Groups["IsLargeArc"].Value, out _arcSize);
        Enum.TryParse(match.Groups["SweepDirection"].Value, out _sweepDirection);
        Single.TryParse(match.Groups["X"].Value, out _x);
        Single.TryParse(match.Groups["Y"].Value, out _y);
    }

    #endregion
}
