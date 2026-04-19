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
/// Class representing a layer in a CanvasElement
/// </summary>
public partial class CanvasRenderLayer : ICanvasRenderLayer
{
    #region Properties

    /// <summary>
    /// Gets the CanvasGeometry to be rendered on
    /// this layer.
    /// </summary>
    public CanvasGeometry Geometry { get; }
    /// <summary>
    /// Gets the ICanvasBrush used to fill the 
    /// rendered geometry.
    /// </summary>
    public ICanvasBrush Brush { get; }
    /// <summary>
    /// Gets the ICanvasStroke used to outline the
    /// rendered geometry.
    /// </summary>
    public ICanvasStroke Stroke { get; }

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// Creates a CanvasRenderLayer with the specified geometry, fill brush and stroke.
    /// </summary>
    /// <param name="geometry">CanvasGeometry to be rendered in this layer.</param>
    /// <param name="brush">ICanvasBrush used to fill the rendered geometry.</param>
    /// <param name="stroke">ICanvasStroke used to outline the rendered geometry.</param>
    public CanvasRenderLayer(CanvasGeometry geometry, ICanvasBrush brush, ICanvasStroke stroke)
    {
        Geometry = geometry;
        Brush = brush;
        Stroke = stroke;
    }

    /// <summary>
    /// Creates a CanvasRenderLayer with the geometry, fill brush and stroke specified in 
    /// string formats.
    /// </summary>
    /// <param name="creator">ICanvasResourceCreator.</param>
    /// <param name="geometryData">CanvasGeometry string definition.</param>
    /// <param name="brushData">ICanvasBrush string definition.</param>
    /// <param name="strokeData">ICanvasStroke string definition.</param>
    public CanvasRenderLayer(ICanvasResourceCreator creator, string geometryData, string brushData, string strokeData)
    {
        Geometry = String.IsNullOrWhiteSpace(geometryData) ? null : CanvasObject.CreateGeometry(creator, geometryData);
        Brush = String.IsNullOrWhiteSpace(brushData) ? null : CanvasObject.CreateBrush(creator, brushData);
        Stroke = String.IsNullOrWhiteSpace(strokeData) ? null : CanvasObject.CreateStroke(creator, strokeData);
    }

    /// <summary>
    /// Creates a CanvasRenderLayer with the specified geometry string, fill brush, outline color and
    /// strokeWidth (optional, defaults to 1f).
    /// </summary>
    /// <param name="creator">ICanvasResourceCreator</param>
    /// <param name="geometryData">CanvasGeometry string definition.</param>
    /// <param name="brush">ICanvasBrush used to fill the rendered geometry.</param>
    /// <param name="strokeColor">Color of the rendered geometry outline.</param>
    /// <param name="strokeWidth">Width of the rendered geometry outline.</param>
    public CanvasRenderLayer(ICanvasResourceCreator creator, string geometryData, ICanvasBrush brush, Color strokeColor,
        float strokeWidth = 1f)
    {
        Geometry = String.IsNullOrWhiteSpace(geometryData) ? null : CanvasObject.CreateGeometry(creator, geometryData);
        Brush = brush;
        Stroke = new CanvasStroke(creator, strokeColor, strokeWidth);
    }

    /// <summary>
    /// Creates a CanvasRenderLayer with the specified geometry string, fill brush, outline color,
    /// strokeWidth and stroke style.
    /// </summary>
    /// <param name="creator">ICanvasResourceCreator</param>
    /// <param name="geometryData">CanvasGeometry string definition.</param>
    /// <param name="brush">ICanvasBrush used to fill the rendered geometry.</param>
    /// <param name="strokeColor">Color of the rendered geometry outline.</param>
    /// <param name="strokeWidth">Width of the rendered geometry outline.</param>
    /// <param name="strokeStyle">CanvasStrokeStyle</param>
    public CanvasRenderLayer(ICanvasResourceCreator creator, string geometryData, ICanvasBrush brush, Color strokeColor,
        float strokeWidth, CanvasStrokeStyle strokeStyle)
    {
        Geometry = String.IsNullOrWhiteSpace(geometryData) ? null : CanvasObject.CreateGeometry(creator, geometryData);
        Brush = brush;
        Stroke = new CanvasStroke(creator, strokeColor, strokeWidth, strokeStyle);
    }

    /// <summary>
    /// Creates a CanvasRenderLayer with the specified geometry string, fill brush, outline brush,
    /// strokeWidth (optional, defaults to 1f).
    /// </summary>
    /// <param name="creator">ICanvasResourceCreator</param>
    /// <param name="geometryData">CanvasGeometry string definition.</param>
    /// <param name="brush">ICanvasBrush used to fill the rendered geometry.</param>
    /// <param name="strokeBrush">ICanvasBrush for the rendered geometry outline.</param>
    /// <param name="strokeWidth">Width of the rendered geometry outline.</param>
    public CanvasRenderLayer(ICanvasResourceCreator creator, string geometryData, ICanvasBrush brush, ICanvasBrush strokeBrush,
        float strokeWidth = 1)
    {
        Geometry = String.IsNullOrWhiteSpace(geometryData) ? null : CanvasObject.CreateGeometry(creator, geometryData);
        Brush = brush;
        Stroke = new CanvasStroke(strokeBrush, strokeWidth);
    }

    /// <summary>
    /// Creates a CanvasRenderLayer with the specified geometry string, fill brush, outline brush,
    /// strokeWidth and stroke style.
    /// </summary>
    /// <param name="creator">ICanvasResourceCreator</param>
    /// <param name="geometryData">CanvasGeometry string definition.</param>
    /// <param name="brush">ICanvasBrush used to fill the rendered geometry.</param>
    /// <param name="strokeBrush">ICanvasBrush for the rendered geometry outline.</param>
    /// <param name="strokeWidth">Width of the rendered geometry outline.</param>
    /// <param name="strokeStyle">CanvasStrokeStyle</param>
    public CanvasRenderLayer(ICanvasResourceCreator creator, string geometryData, ICanvasBrush brush, ICanvasBrush strokeBrush,
        float strokeWidth, CanvasStrokeStyle strokeStyle)
    {
        Geometry = String.IsNullOrWhiteSpace(geometryData) ? null : CanvasObject.CreateGeometry(creator, geometryData);
        Brush = brush;
        Stroke = new CanvasStroke(strokeBrush, strokeWidth, strokeStyle);
    }

    #endregion
}
