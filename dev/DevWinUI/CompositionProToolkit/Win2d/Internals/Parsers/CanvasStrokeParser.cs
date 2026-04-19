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

namespace DevWinUI;

/// <summary>
/// Parser for CanvasStroke
/// </summary>
internal static partial class CanvasStrokeParser
{
    /// <summary>
    /// Parses the Stroke Data string and converts it into ICanvasStrokeElement
    /// </summary>
    /// <param name="strokeData">Stroke Data string</param>
    /// <returns>ICanvasStrokeElement</returns>
    internal static ICanvasStrokeElement Parse(string strokeData)
    {
        var matches = RegexFactory.CanvasStrokeRegex.Matches(strokeData);
        // If no match is found or no captures in the match, then it means
        // that the stroke data is invalid.
        if ((matches == null) || (matches.Count == 0))
        {
            throw new ArgumentException($"Invalid Stroke data!\nStroke Data: {strokeData}", nameof(strokeData));
        }

        // If the match contains more than one captures, it means that there
        // are multiple CanvasStrokes present in the stroke data. There should
        // be only one CanvasStroke defined in the stroke data.
        if (matches.Count > 1)
        {
            throw new ArgumentException("Multiple CanvasStrokes defined in Stroke Data! " +
                                        "There should be only one CanvasStroke definition within the Stroke Data. " +
                                        "You can either remove CanvasStroke definitions or split the Stroke Data " +
                                        "into multiple Stroke Data and call the CanvasObject.CreateStroke() method on each of them." +
                                        $"\nStroke Data: {strokeData}");
        }

        // There should be only one match
        var match = matches[0];
        var strokeElement = new CanvasStrokeElement(match);
        
        // Perform validation to check if there are any invalid characters in the stroke data that were not captured
        var preValidationCount = RegexFactory.ValidationRegex.Replace(strokeData, string.Empty).Length;

        var postValidationCount = strokeElement.ValidationCount;

        if (preValidationCount != postValidationCount)
        {
            throw new ArgumentException($"Stroke data contains invalid characters!\nStroke Data: {strokeData}", nameof(strokeData));
        }

        return strokeElement;
    }

    /// <summary>
    /// Parses the Stroke Data string and converts it into CanvasStroke
    /// </summary>
    /// <param name="resourceCreator">ICanvasResourceCreator</param>
    /// <param name="strokeData">Stroke Data string</param>
    /// <returns>ICanvasStroke</returns>
    internal static ICanvasStroke Parse(ICanvasResourceCreator resourceCreator, string strokeData)
    {
        // Parse the stroke data to create the ICanvasStrokeElement
        var strokeElement = Parse(strokeData);
        // Create the CanvasStroke from the strokeElement
        return strokeElement.CreateStroke(resourceCreator);
    }
}
