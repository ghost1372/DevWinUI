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
/// Represents a Stroke Element
/// </summary>
internal sealed partial class CanvasStrokeElement : AbstractCanvasStrokeElement
{
    #region Fields

    private float _width;
    private ICanvasBrushElement _brush;
    private ICanvasStrokeStyleElement _style;
    private int _widthValidationCount;

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="match">Match object</param>
    public CanvasStrokeElement(Match match)
    {
        _width = 1f;
        _brush = null;
        _style = null;
        _widthValidationCount = 0;

        Initialize(match);
    }

    #endregion

    #region APIs

    /// <summary>
    /// Creates the ICanvasStroke from the parsed data
    /// </summary>
    /// <returns>ICanvasStroke</returns>
    public override ICanvasStroke CreateStroke(ICanvasResourceCreator resourceCreator)
    {
        return new CanvasStroke(_brush.CreateBrush(resourceCreator), _width, _style.Style);
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Gets the Stroke Element Attributes from the Match
    /// </summary>
    /// <param name="match">Match object</param>
    protected override void GetAttributes(Match match)
    {
        // Stroke Width
        var group = match.Groups["StrokeWidth"];
        Single.TryParse(group.Value, out _width);
        // Sanitize by taking the absolute value
        _width = Math.Abs(_width);

        _widthValidationCount = RegexFactory.ValidationRegex.Replace(group.Value, String.Empty).Length;

        // Stroke Brush
        group = match.Groups["CanvasBrush"];
        if (group.Success)
        {
            _brush = CanvasBrushParser.Parse(group.Value);
        }

        // If the ICanvasBrushElement was not created, then the ICanvasStroke cannot be created
        if (_brush == null)
        {
            throw new NullReferenceException($"Unable to create a valid ICanvasBrush for the " +
                                             $"ICanvasStroke with the following Brush data - '{group.Value}'");
        }

        // Stroke Style
        _style = CanvasStrokeStyleParser.Parse(match);
    }

    /// <summary>
    /// Gets the number of non-whitespace characters in the data
    /// </summary>
    protected override void Validate()
    {
        // Add 2 to the Validation Count to include the stroke command 'ST'
        ValidationCount += 2;
        // StrokeWidth Validation Count
        ValidationCount += _widthValidationCount;
        // Stroke Brush Validation Count
        if (_brush != null)
        {
            ValidationCount += _brush.ValidationCount;
        }
        // Stroke Style Validation Count
        if (_style != null)
        {
            ValidationCount += _style.ValidationCount;
        }
    }

    #endregion
}
