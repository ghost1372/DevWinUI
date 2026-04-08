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
/// Parser for the CanvasStrokeStyle
/// </summary>
internal static partial class CanvasStrokeStyleParser
{
    /// <summary>
    /// Parses the given style data and converts it to CanvasStrokeStyle
    /// </summary>
    /// <param name="styleData">Style data</param>
    /// <returns>CanvasStrokeStyle</returns>
    internal static CanvasStrokeStyle Parse(string styleData)
    {
        var matches = RegexFactory.CanvasStrokeStyleRegex.Matches(styleData);
        // If no match is found or no captures in the match, then it means
        // that the style data is invalid.
        if ((matches == null) || (matches.Count == 0))
        {
            throw new ArgumentException($"Invalid CanvasStrokeStyle data!\nCanvasStrokeStyle Data: {styleData}", nameof(styleData));
        }

        // If the match contains more than one captures, it means that there
        // are multiple CanvasStrokeStyles present in the CanvasStrokeStyle data. There should
        // be only one CanvasStrokeStyle defined in the CanvasStrokeStyle data.
        if (matches.Count > 1)
        {
            throw new ArgumentException("Multiple CanvasStrokeStyles defined in CanvasStrokeStyle Data! " +
                                        "There should be only one CanvasStrokeStyle definition within the CanvasStrokeStyle Data. " +
                                        "You can either remove CanvasStrokeStyle definitions or split the CanvasStrokeStyle Data " +
                                        "into multiple CanvasStrokeStyle Data and call the CanvasObject.CreateStrokeStyle() method on each of them." +
                                        $"\nCanvasStrokeStyle Data: {styleData}");
        }

        // There should be only one match
        var match = matches[0];
        var styleElement = new CanvasStrokeStyleElement(match);
        
        // Perform validation to check if there are any invalid characters in the brush data that were not captured
        var preValidationCount = RegexFactory.ValidationRegex.Replace(styleData, string.Empty).Length;

        var postValidationCount = styleElement.ValidationCount;

        if (preValidationCount != postValidationCount)
        {
            throw new ArgumentException($"CanvasStrokeStyle data contains invalid characters!\nCanvasStrokeStyle Data: {styleData}", nameof(styleData));
        }

        return styleElement.Style;
    }

    /// <summary>
    /// Parses and constructs a ICanvasStrokeStyleElement from the 
    /// specified Match object.
    /// </summary>
    /// <param name="match">Match object</param>
    /// <returns>ICanvasStrokeStyleElement</returns>
    internal static ICanvasStrokeStyleElement Parse(Match match)
    {
        return new CanvasStrokeStyleElement(match);
    }
}
