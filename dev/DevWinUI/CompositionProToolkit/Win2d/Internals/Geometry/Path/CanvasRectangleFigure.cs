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
/// Class representing the Rectangle Figure in a Path Geometry
/// </summary>
internal sealed partial class CanvasRectangleFigure : AbstractPathElement
{
    #region Fields

    private float _x;
    private float _y;
    private float _width;
    private float _height;

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// Ctor
    /// </summary>
    public CanvasRectangleFigure()
    {
        _x = _y = _width = _height = 0f;
    }

    #endregion

    #region APIs

    /// <summary>
    /// Adds the Path Element to the Path.
    /// </summary>
    /// <param name="pathBuilder">CanvasPathBuilder object</param>
    /// <param name="currentPoint">The last active location in the Path before adding 
    /// the PolygonFigure</param>
    /// <param name="lastElement">The previous PathElement in the Path.</param>
    /// <param name="logger">For logging purpose. To log the set of CanvasPathBuilder 
    /// commands, used for creating the CanvasGeometry, in string format.</param>
    /// <returns>The latest location in the Path after adding the PolygonFigure</returns>
    public override Vector2 CreatePath(CanvasPathBuilder pathBuilder, Vector2 currentPoint,
        ref ICanvasPathElement lastElement, StringBuilder logger)
    {
        // Calculate coordinates
        var topLeft = new Vector2(_x, _y);
        if (IsRelative)
        {
            topLeft += currentPoint;
        }

        // Execute command
        pathBuilder.AddRectangleFigure(topLeft.X, topLeft.Y, _width, _height);

        // Log command
        logger?.AppendLine();
        logger?.AppendLine($"{Indent}pathBuilder.AddRectangleFigure({topLeft.X}, {topLeft.Y}, {_width}, {_height});");

        // No need to update the lastElement or currentPoint here as we are creating
        // a separate closed figure here.So current point will not change.
        return currentPoint;
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Get the Regex for extracting Path Element Attributes
    /// </summary>
    /// <returns></returns>
    protected override Regex GetAttributesRegex()
    {
        return RegexFactory.GetAttributesRegex(PathFigureType.RectangleFigure);
    }

    /// <summary>
    /// Gets the Path Element Attributes from the Match
    /// </summary>
    /// <param name="match">Match object</param>
    protected override void GetAttributes(Match match)
    {
        // X
        Single.TryParse(match.Groups["X"].Value, out _x);
        // Y
        Single.TryParse(match.Groups["Y"].Value, out _y);
        // Width
        Single.TryParse(match.Groups["Width"].Value, out _width);
        // Sanitize by taking the absolute value
        _width = Math.Abs(_width);
        // Height
        Single.TryParse(match.Groups["Height"].Value, out _height);
        // Sanitize by taking the absolute value
        _height = Math.Abs(_height);
    }

    #endregion
}
