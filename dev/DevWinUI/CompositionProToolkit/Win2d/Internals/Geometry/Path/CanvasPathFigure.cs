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
/// Class which contains a collection of ICanvasPathElements
/// which can be used to create CanvasGeometry.
/// </summary>
internal partial class CanvasPathFigure : AbstractPathElement
{
    #region Fields

    // Collection of Path Elements
    private List<ICanvasPathElement> _elements;

    #endregion

    #region Construction / Initialization

    public CanvasPathFigure()
    {
        _elements = new List<ICanvasPathElement>();
        ValidationCount = 0;
    }

    #endregion

    #region APIs

    /// <summary>
    /// Initializes the Path Element with the given Match
    /// </summary>
    /// <param name="match">Match object</param>
    /// <param name="index"></param>
    public override void Initialize(Match match, int index)
    {
        Index = index;
        var main = match.Groups["Main"];
        Data = main.Value;

        var elements = new List<ICanvasPathElement>();
        var pathElementList = new List<PathElementType>(Enum.GetValues<PathElementType>());
        foreach (PathElementType type in pathElementList)
        {
            foreach (Capture elementCapture in match.Groups[type.ToString()].Captures)
            {
                var elementRootIndex = elementCapture.Index;
                var regex = RegexFactory.GetRegex(type);
                var elementMatch = regex.Match(elementCapture.Value);
                var isRelative = false;
                // Process the 'Main' Group which contains the Path Command and
                // corresponding attributes
                if (elementMatch.Groups["Main"].Captures.Count == 1)
                {
                    var figure = PathElementFactory.CreatePathElement(type, elementMatch, elementRootIndex);
                    elements.Add(figure);
                    isRelative = figure.IsRelative;
                }

                // Process the 'Additional' Group which contains just the attributes
                elements.AddRange(from Capture capture in elementMatch.Groups["Additional"].Captures
                                  select PathElementFactory.CreateAdditionalPathElement(type, capture, elementRootIndex + capture.Index, isRelative));
            }
        }

        // Sort the path elements based on their index value
        _elements.AddRange(elements.OrderBy(e => e.Index));
        if (_elements.Count <= 0)
            return;

        // Check if the last path element in the figure is an ClosePathElement
        // which would indicate that the path needs to be closed. Otherwise,
        // add a default ClosePathElement at the end to indicate that the path
        // is not closed.
        var lastElement = _elements.ElementAt(_elements.Count - 1);
        if ((lastElement as ClosePathElement) == null)
        {
            _elements.Add(PathElementFactory.CreateDefaultPathElement(PathElementType.ClosePath));
        }

        // Validation Count will be the cumulative sum of the validation count
        // of child elements of the PathFigure
        ValidationCount = _elements.Sum(x => x.ValidationCount);
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
        // Do nothing as this scenario is not valid for CanvasPathFigure
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
        // Log command
        logger?.AppendLine();

        foreach (var pathElement in _elements)
        {
            currentPoint = pathElement.CreatePath(pathBuilder, currentPoint, ref lastElement, logger);
        }
        
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
        return null;
    }

    /// <summary>
    /// Gets the Path Element Attributes from the Match
    /// </summary>
    /// <param name="match">Match object</param>
    protected override void GetAttributes(Match match)
    {
        // Do nothing
    }

    #endregion
}
