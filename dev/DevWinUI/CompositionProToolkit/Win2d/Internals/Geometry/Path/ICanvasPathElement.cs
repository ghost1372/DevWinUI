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
/// Interface for a Path Element which serves
/// as a building block for CanvasPathGeometry
/// </summary>
internal interface ICanvasPathElement
{
    #region Properties

    /// <summary>
    /// Index of the Path Element in the Path Data
    /// </summary>
    int Index { get; }
    /// <summary>
    /// Path data defining the Path Element
    /// </summary>
    string Data { get; }
    /// <summary>
    /// Number of non-whitespace characters in 
    /// the Path Element Data
    /// </summary>
    int ValidationCount { get; }
    /// <summary>
    /// Indicates whether the path element contains
    /// absolute or relative coordinates.
    /// </summary>
    bool IsRelative { get; }

    #endregion

    #region APIs

    /// <summary>
    /// Initializes the Path Element with the given Match
    /// </summary>
    /// <param name="match">Match object</param>
    /// <param name="index">Index of the Path Element in the Path data.</param>
    void Initialize(Match match, int index);

    /// <summary>
    /// Initializes the Path Element with the given Capture
    /// </summary>
    /// <param name="capture">Capture object</param>
    /// <param name="index">Index of the Path Element in the Path data.</param>
    /// <param name="isRelative">Indicates whether the Path Element coordinates are
    /// absolute or relative</param>
    void InitializeAdditional(Capture capture, int index, bool isRelative);

    /// <summary>
    /// Adds the Path Element to the PathBuilder.
    /// </summary>
    /// <param name="pathBuilder">CanvasPathBuilder object</param>
    /// <param name="currentPoint">The current point on the path before the path element is added</param>
    /// <param name="lastElement">The previous PathElement in the Path.</param>
    /// <param name="logger">For logging purpose. To log the set of CanvasPathBuilder 
    /// commands, used for creating the CanvasGeometry, in string format.</param>
    /// <returns>The current point on the path after the path element is added</returns>
    Vector2 CreatePath(CanvasPathBuilder pathBuilder, Vector2 currentPoint, 
        ref ICanvasPathElement lastElement, StringBuilder logger);

    #endregion
}
