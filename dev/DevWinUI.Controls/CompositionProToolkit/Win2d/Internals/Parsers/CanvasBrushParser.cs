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
/// Parser for ICanvasBrush
/// </summary>
internal static partial class CanvasBrushParser
{
    /// <summary>
    /// Parses the Brush data string and converts it into ICanvasBrushElement
    /// </summary>
    /// <param name="brushData">Brush data</param>
    /// <returns>ICanvasBrushElement</returns>
    internal static ICanvasBrushElement Parse(string brushData)
    {
        var matches = RegexFactory.CanvasBrushRegex.Matches(brushData);
        // If no match is found or no captures in the match, then it means
        // that the brush data is invalid.
        if ((matches == null) || (matches.Count == 0))
        {
            throw new ArgumentException($"Invalid Brush data!\nBrush Data: {brushData}", nameof(brushData));
        }

        // If the match contains more than one captures, it means that there
        // are multiple brushes present in the brush data. There should
        // be only one brush defined in the brush data.
        if (matches.Count > 1)
        {
            throw new ArgumentException("Multiple Brushes defined in Brush Data! " +
                                        "There should be only one Brush definition within the Brush Data. " +
                                        "You can either remove Brush definitions or split the Brush Data " +
                                        "into multiple Brush Data and call the CanvasObject.CreateBrush() method on each of them." +
                                        $"\nBrush Data: {brushData}");
        }

        // There should be only one match
        var match = matches[0];
        AbstractCanvasBrushElement brushElement = null;
        if (match.Groups["SolidColorBrush"].Success)
        {
            brushElement = new SolidColorBrushElement(match.Groups["SolidColorBrush"].Captures[0]);
        }
        else if (match.Groups["LinearGradient"].Success)
        {
            brushElement = new LinearGradientBrushElement(match.Groups["LinearGradient"].Captures[0]);
        }
        else if (match.Groups["LinearGradientHdr"].Success)
        {
            brushElement = new LinearGradientHdrBrushElement(match.Groups["LinearGradientHdr"].Captures[0]);
        }
        else if (match.Groups["RadialGradient"].Success)
        {
            brushElement = new RadialGradientBrushElement(match.Groups["RadialGradient"].Captures[0]);
        }
        else if (match.Groups["RadialGradientHdr"].Success)
        {
            brushElement = new RadialGradientHdrBrushElement(match.Groups["RadialGradientHdr"].Captures[0]);
        }

        if (brushElement == null)
        {
            return null;
        }

        // Perform validation to check if there are any invalid characters in the brush data that were not captured
        var preValidationCount = RegexFactory.ValidationRegex.Replace(brushData, string.Empty).Length;

        var postValidationCount = brushElement.ValidationCount;

        if (preValidationCount != postValidationCount)
        {
            throw new ArgumentException($"Brush data contains invalid characters!\nBrush Data: {brushData}", nameof(brushData));
        }

        return brushElement;
    }

    /// <summary>
    /// Parses the Brush data string and converts it into ICanvasBrush
    /// </summary>
    /// <param name="resourceCreator">ICanvasResourceCreator</param>
    /// <param name="brushData">Brush data string</param>
    /// <returns>ICanvasBrush</returns>
    internal static ICanvasBrush Parse(ICanvasResourceCreator resourceCreator, string brushData)
    {
        // Parse the brush data to get the ICanvasBrushElement
        var brushElement = Parse(brushData);
        // Create ICanvasBrush from the brushElement
        return brushElement.CreateBrush(resourceCreator);
    }
}
