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
/// Class representing the MoveTo Command in a Path Geometry
/// </summary>
internal partial class MoveToElement : AbstractPathElement
{
    #region Fields

    private float _x;
    private float _y;

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// ctor
    /// </summary>
    public MoveToElement()
    {
        _x = _y = 0;
    }

    #endregion

    #region APIs

    /// <summary>
    /// Initializes the Path Element with the given Capture
    /// </summary>
    /// <param name="capture">Capture object</param>
    /// <param name="index">Index of the Path Element in the Path data.</param>
    /// <param name="isRelative">Indicates whether the Path Element coordinates are
    /// absolute or relative</param>
    public override void InitializeAdditional(Capture capture, int index, bool isRelative)
    {
        // Do nothing as this scenario is not valid for MoveTo Command
        // Additional coordinates specified with MoveTo will be converted to Line Commands
    }

    /// <summary>
    /// Adds the Path Element to the PathBuilder.
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
        pathBuilder.BeginFigure(point);

        // Log command
        logger?.AppendLine($"{Indent}pathBuilder.BeginFigure(new Vector2({point.X}, {point.Y}));");

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
        return RegexFactory.GetAttributesRegex(PathElementType.MoveTo);
    }

    /// <summary>
    /// Gets the Path Element Attributes from the Match
    /// </summary>
    /// <param name="match">Match object</param>
    protected override void GetAttributes(Match match)
    {
        Single.TryParse(match.Groups["X"].Value, out _x);
        Single.TryParse(match.Groups["Y"].Value, out _y);
    }

    #endregion
}
