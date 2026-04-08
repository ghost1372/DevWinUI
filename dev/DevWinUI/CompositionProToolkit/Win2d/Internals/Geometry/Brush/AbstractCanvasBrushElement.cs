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

using System.Text.RegularExpressions;

namespace DevWinUI;

/// <summary>
/// Abstract base class for all Brush Elements
/// </summary>
internal abstract partial class AbstractCanvasBrushElement : ICanvasBrushElement
{
    #region Fields

    protected float _opacity;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the Brush data defining the Brush Element
    /// </summary>
    public string Data { get; protected set; }
    /// <summary>
    /// Gets the number of non-whitespace characters in 
    /// the Brush Data
    /// </summary>
    public int ValidationCount { get; protected set; }

    #endregion

    #region APIs

    /// <summary>
    /// Initializes the Brush Element with the given Capture
    /// </summary>
    /// <param name="capture">Capture object</param>
    public virtual void Initialize(Capture capture)
    {
        Data = capture.Value;

        var regex = GetAttributesRegex();
        var match = regex.Match(Data);
        if (!match.Success)
        {
            return;
        }

        GetAttributes(match);

        // Get the number of non-whitespace characters in the data
        ValidationCount = RegexFactory.ValidationRegex.Replace(Data, String.Empty).Length;
    }

    /// <summary>
    /// Creates the ICanvasBrush from the parsed data
    /// </summary>
    /// <param name="resourceCreator">ICanvasResourceCreator object</param>
    /// <returns>ICanvasBrush</returns>
    public abstract ICanvasBrush CreateBrush(ICanvasResourceCreator resourceCreator);

    #endregion

    #region Helpers

    /// <summary>
    /// Gets the Regex for extracting Brush Element Attributes
    /// </summary>
    /// <returns>Regex</returns>
    protected abstract Regex GetAttributesRegex();

    /// <summary>
    /// Gets the Brush Element Attributes from the Match
    /// </summary>
    /// <param name="match">Match object</param>
    protected abstract void GetAttributes(Match match);

    #endregion
}
