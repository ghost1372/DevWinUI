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
/// Represents a CanvasStrokeStyle Element
/// </summary>
internal sealed partial class CanvasStrokeStyleElement : ICanvasStrokeStyleElement
{
    #region Properties

    /// <summary>
    /// Gets the Stroke data defining the Brush Element
    /// </summary>
    public string Data { get; private set; }
    /// <summary>
    /// Gets the number of non-whitespace characters in 
    /// the Stroke Data
    /// </summary>
    public int ValidationCount { get; private set; }
    /// <summary>
    /// Gets the CanvasStrokeStyle obtained by parsing
    /// the style data.
    /// </summary>
    public CanvasStrokeStyle Style { get; private set; }

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="match"></param>
    public CanvasStrokeStyleElement(Match match)
    {
        Data = match.Groups["CanvasStrokeStyle"].Value;

        Style = new CanvasStrokeStyle();

        Initialize(match);

        // Get the number of non-whitespace characters in the data
        ValidationCount = RegexFactory.ValidationRegex.Replace(Data, String.Empty).Length;
    }

    #endregion

    #region APIs

    /// <summary>
    /// Initializes the Stroke Element with the given Capture
    /// </summary>
    /// <param name="match">Match object</param>
    public void Initialize(Match match)
    {
        var group = match.Groups["CanvasStrokeStyle"];
        if (group.Success)
        {
            // DashStyle
            group = match.Groups["DashStyle"];
            if (group.Success)
            {
                CanvasDashStyle dashStyle;
                Enum.TryParse(group.Value, out dashStyle);
                Style.DashStyle = dashStyle;
            }
            // LineJoin
            group = match.Groups["LineJoin"];
            if (group.Success)
            {
                CanvasLineJoin lineJoin;
                Enum.TryParse(group.Value, out lineJoin);
                Style.LineJoin = lineJoin;
            }
            // MiterLimit
            group = match.Groups["MiterLimit"];
            if (group.Success)
            {
                float miterLimit;
                Single.TryParse(group.Value, out miterLimit);
                // Sanitize by taking the absolute value
                Style.MiterLimit = Math.Abs(miterLimit);
            }
            // DashOffset
            group = match.Groups["DashOffset"];
            if (group.Success)
            {
                float dashOffset;
                Single.TryParse(group.Value, out dashOffset);
                Style.DashOffset = dashOffset;
            }
            // StartCap
            group = match.Groups["StartCap"];
            if (group.Success)
            {
                CanvasCapStyle capStyle;
                Enum.TryParse(group.Value, out capStyle);
                Style.StartCap = capStyle;
            }
            // EndCap
            group = match.Groups["EndCap"];
            if (group.Success)
            {
                CanvasCapStyle capStyle;
                Enum.TryParse(group.Value, out capStyle);
                Style.EndCap = capStyle;
            }
            // DashCap
            group = match.Groups["DashCap"];
            if (group.Success)
            {
                CanvasCapStyle capStyle;
                Enum.TryParse(group.Value, out capStyle);
                Style.DashCap = capStyle;
            }
            // TransformBehavior
            group = match.Groups["TransformBehavior"];
            if (group.Success)
            {
                CanvasStrokeTransformBehavior transformBehavior;
                Enum.TryParse(group.Value, out transformBehavior);
                Style.TransformBehavior = transformBehavior;
            }
            // CustomDashStyle
            group = match.Groups["CustomDashStyle"];
            if (group.Success)
            {
                List<float> dashes = new List<float>();
                group = match.Groups["Main"];
                if (group.Success)
                {
                    float dashSize;
                    if (Single.TryParse(match.Groups["DashSize"].Value, out dashSize))
                    {
                        // Sanitize by taking the absolute value
                        dashes.Add(Math.Abs(dashSize));
                    }
                    float spaceSize;
                    if (Single.TryParse(match.Groups["SpaceSize"].Value, out spaceSize))
                    {
                        // Sanitize by taking the absolute value
                        dashes.Add(Math.Abs(spaceSize));
                    }
                }

                group = match.Groups["Additional"];
                if (group.Success)
                {
                    foreach (Capture capture in group.Captures)
                    {
                        var dashMatch = RegexFactory.CustomDashAttributeRegex.Match(capture.Value);
                        if (!dashMatch.Success)
                            continue;

                        float dashSize;
                        if (Single.TryParse(dashMatch.Groups["DashSize"].Value, out dashSize))
                        {
                            // Sanitize by taking the absolute value
                            dashes.Add(Math.Abs(dashSize));
                        }
                        float spaceSize;
                        if (Single.TryParse(dashMatch.Groups["SpaceSize"].Value, out spaceSize))
                        {
                            // Sanitize by taking the absolute value
                            dashes.Add(Math.Abs(spaceSize));
                        }
                    }
                }

                // Any valid dashes?
                if (dashes.Any())
                {
                    Style.CustomDashStyle = dashes.ToArray();
                }
            }
        }
    }
    
    #endregion
}
