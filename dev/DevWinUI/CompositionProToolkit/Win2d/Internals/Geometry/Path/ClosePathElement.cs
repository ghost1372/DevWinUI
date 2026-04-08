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
/// Class representing the ClosePath command in a Path Geometry
/// </summary>
internal partial class ClosePathElement : AbstractPathElement
{
    #region Fields

    private bool _isFigureClosed;

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// ctor
    /// </summary>
    public ClosePathElement()
    {
        _isFigureClosed = false;
    }

    #endregion

    #region APIs

    /// <summary>
    /// Initializes the Path Element with the given Match
    /// </summary>
    /// <param name="match">Match object</param>
    /// <param name="index">Index of the Path Element in the Path data.</param>
    public override void Initialize(Match match, int index)
    {
        var main = match.Groups["Main"];
        Index = index;
        Data = main.Value;
        var command = match.Groups["Command"];

        // If the Command is captured, it means that 'Z' is provided and hence 
        // the figure must be closed
        if (command.Captures.Count == 1)
        {
            _isFigureClosed = true;
        }

        // Get the number of non-whitespace characters in the data
        ValidationCount = RegexFactory.ValidationRegex.Replace(main.Value, String.Empty).Length;
    }

    /// <summary>
    /// Initializes the Path Element with the given Capture
    /// </summary>
    /// <param name="capture">Capture object</param>
    /// <param name="index">Index of the Path Element in the Path data.</param>
    /// <param name="isRelative">Indicates whether the Path Element coordinates are
    /// absolute or relative</param>
    public override void InitializeAdditional(Capture capture, int index, bool isRelative)
    {
        // Do nothing as this scenario is not valid for this Path Element
    }

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
        // Execute command
        pathBuilder.EndFigure(_isFigureClosed ? CanvasFigureLoop.Closed : CanvasFigureLoop.Open);
        
        // Log command
        var cmd = _isFigureClosed ? "CanvasFigureLoop.Closed" : "CanvasFigureLoop.Open";
        logger?.AppendLine($"{Indent}pathBuilder.EndFigure({cmd});");

        // Set Last Element
        lastElement = this;
        // Return current point
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
        // Attributes are not present
        return null;
    }

    /// <summary>
    /// Gets the Path Element Attributes from the Match
    /// </summary>
    /// <param name="match">Match object</param>
    protected override void GetAttributes(Match match)
    {
        // Do Nothing
    }

    #endregion
}
