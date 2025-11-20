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
/// Represents a CanvasLinearGradientBrush with GradientStops
/// </summary>
internal sealed partial class LinearGradientBrushElement : AbstractCanvasBrushElement
{
    #region Fields

    private Vector2 _startPoint;
    private Vector2 _endPoint;
    private CanvasAlphaMode _alphaMode;
    private CanvasBufferPrecision _bufferPrecision;
    private CanvasEdgeBehavior _edgeBehavior;
    private CanvasColorSpace _preInterpolationColorSpace;
    private CanvasColorSpace _postInterpolationColorSpace;
    private List<CanvasGradientStop> _gradientStops;

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="capture">Capture object</param>
    public LinearGradientBrushElement(Capture capture)
    {
        // Set the default values
        _startPoint = Vector2.Zero;
        _endPoint = Vector2.Zero;
        _opacity = 1f;
        _alphaMode = (CanvasAlphaMode)0;
        _bufferPrecision = (CanvasBufferPrecision)0;
        _edgeBehavior = (CanvasEdgeBehavior)0;
        // Default ColorSpace is sRGB
        _preInterpolationColorSpace = CanvasColorSpace.Srgb;
        _postInterpolationColorSpace = CanvasColorSpace.Srgb;
        _gradientStops = new List<CanvasGradientStop>();
        // Initialize
        Initialize(capture);
    }

    #endregion

    #region APIs

    /// <summary>
    /// Creates the CanvasLinearGradientBrush from the parsed data
    /// </summary>
    /// <param name="resourceCreator">ICanvasResourceCreator object</param>
    /// <returns></returns>
    public override ICanvasBrush CreateBrush(ICanvasResourceCreator resourceCreator)
    {
        var brush = new CanvasLinearGradientBrush(resourceCreator, _gradientStops.ToArray(), _edgeBehavior,
            _alphaMode, _preInterpolationColorSpace, _postInterpolationColorSpace, _bufferPrecision)
        {
            StartPoint = _startPoint,
            EndPoint = _endPoint,
            Opacity = _opacity
        };

        return brush;
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Gets the Regex for extracting Brush Element Attributes
    /// </summary>
    /// <returns>Regex</returns>
    protected override Regex GetAttributesRegex()
    {
        return RegexFactory.GetAttributesRegex(BrushType.LinearGradient); 
    }

    /// <summary>
    /// Gets the Brush Element Attributes from the Match
    /// </summary>
    /// <param name="match">Match object</param>
    protected override void GetAttributes(Match match)
    {
        float startX, startY, endX, endY;
        // Start Point
        Single.TryParse(match.Groups["StartX"].Value, out startX);
        Single.TryParse(match.Groups["StartY"].Value, out startY);
        _startPoint = new Vector2(startX, startY);
        // End Point
        Single.TryParse(match.Groups["EndX"].Value, out endX);
        Single.TryParse(match.Groups["EndY"].Value, out endY);
        _endPoint = new Vector2(endX, endY);

        // Opacity (optional)
        var group = match.Groups["Opacity"];
        if (group.Success)
        {
            Single.TryParse(group.Value, out _opacity);
        }
        // Alpha Mode (optional)
        group = match.Groups["AlphaMode"];
        if (group.Success)
        {
            Enum.TryParse(group.Value, out _alphaMode);
        }
        // Buffer Precision (optional)
        group = match.Groups["BufferPrecision"];
        if (group.Success)
        {
            Enum.TryParse(group.Value, out _bufferPrecision);
        }
        // Edge Behavior (optional)
        group = match.Groups["EdgeBehavior"];
        if (group.Success)
        {
            Enum.TryParse(group.Value, out _edgeBehavior);
        }
        // Pre Interpolation ColorSpace (optional)
        group = match.Groups["PreColorSpace"];
        if (group.Success)
        {
            Enum.TryParse(group.Value, out _preInterpolationColorSpace);
        }
        // Post Interpolation ColorSpace (optional)
        group = match.Groups["PostColorSpace"];
        if (group.Success)
        {
            Enum.TryParse(group.Value, out _postInterpolationColorSpace);
        }

        // GradientStops
        group = match.Groups["GradientStops"];
        if (group.Success)
        {
            _gradientStops.Clear();
            foreach (Capture capture in group.Captures)
            {
                var gradientMatch = RegexFactory.GradientStopRegex.Match(capture.Value);
                if (!gradientMatch.Success)
                    continue;

                float position;
                Color color;
                // Main Attributes
                var main = gradientMatch.Groups["Main"];
                if (main.Success)
                {
                    var mainMatch = RegexFactory.GetAttributesRegex(GradientStopAttributeType.Main).Match(main.Value);
                    Single.TryParse(mainMatch.Groups["Position"].Value, out position);
                    color = ColorParser.Parse(mainMatch);

                    _gradientStops.Add(new CanvasGradientStop()
                    {
                        Color = color,
                        Position = position
                    });
                }
                // Additional Attributes
                var additional = gradientMatch.Groups["Additional"];
                if (!additional.Success)
                    continue;

                foreach (Capture addCapture in additional.Captures)
                {
                    var addMatch = RegexFactory.GetAttributesRegex(GradientStopAttributeType.Additional).Match(addCapture.Value);
                    Single.TryParse(addMatch.Groups["Position"].Value, out position);
                    color = ColorParser.Parse(addMatch);

                    _gradientStops.Add(new CanvasGradientStop()
                    {
                        Color = color,
                        Position = position
                    });
                }
            }

            // Sort the stops based on their position
            if (_gradientStops.Any())
            {
                _gradientStops = _gradientStops.OrderBy(g => g.Position).ToList();
            }
        }
    }

    #endregion
}
