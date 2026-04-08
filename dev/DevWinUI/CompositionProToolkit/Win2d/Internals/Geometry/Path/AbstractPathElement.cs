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
/// Abstract base class for all Path Elements
/// </summary>
internal abstract partial class AbstractPathElement : ICanvasPathElement
{
    #region Constants

    protected readonly string Indent = new string(' ', 4);

    #endregion

    #region Properties

    /// <summary>
    /// Index of the Path Element in the Path Data
    /// </summary>
    public int Index { get; protected set; } = -1;

    /// <summary>
    /// Path data defining the Path Element
    /// </summary>
    public string Data { get; protected set; } = string.Empty;

    /// <summary>
    /// Number of non-whitespace characters in 
    /// the Path Element Data
    /// </summary>
    public int ValidationCount { get; protected set; } = 0;

    /// <summary>
    /// Indicates whether the path element contains
    /// absolute or relative coordinates.
    /// </summary>
    public bool IsRelative { get; protected set; } = false;

    #endregion

    #region APIs

    /// <summary>
    /// Initializes the Path Element with the given Match
    /// </summary>
    /// <param name="match">Match object</param>
    /// <param name="index"></param>
    public virtual void Initialize(Match match, int index)
    {
        var main = match.Groups["Main"];
        Index = index;
        Data = main.Value;
        var command = match.Groups["Command"].Value[0];
        IsRelative = char.IsLower(command);

        // Get the Path Element attributes
        GetAttributes(match);

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
    public virtual void InitializeAdditional(Capture capture, int index, bool isRelative)
    {
        Index = index;
        Data = capture.Value;
        IsRelative = isRelative;

        var match = GetAttributesRegex().Match(Data);
        if (match.Captures.Count != 1)
        {
            return;
        }

        // Get the Path Element attributes
        GetAttributes(match);

        // Get the number of non-whitespace characters in the data
        ValidationCount = RegexFactory.ValidationRegex.Replace(Data, String.Empty).Length;
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
    public abstract Vector2 CreatePath(CanvasPathBuilder pathBuilder, Vector2 currentPoint, 
        ref ICanvasPathElement lastElement, StringBuilder logger);

    #endregion

    #region Helpers

    /// <summary>
    /// Get the Regex for extracting Path Element Attributes
    /// </summary>
    /// <returns></returns>
    protected abstract Regex GetAttributesRegex();

    /// <summary>
    /// Gets the Path Element Attributes from the Match
    /// </summary>
    /// <param name="match">Match object</param>
    protected abstract void GetAttributes(Match match);

    #endregion
}
