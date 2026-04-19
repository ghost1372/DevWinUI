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
/// Class to represent the Stroke which
/// can be used to render an outline on a CanvasGeometry
/// </summary>
public sealed partial class CanvasStroke : ICanvasStroke
{
    #region Properties

    /// <summary>
    /// Gets or sets the brush with which the stroke will be rendered
    /// </summary>
    public ICanvasBrush Brush { get; set; }
    /// <summary>
    /// Gets or sets the width of the stroke
    /// </summary>
    public float Width { get; set; }
    /// <summary>
    /// Gets or sets the Style of the stroke
    /// </summary>
    public CanvasStrokeStyle Style { get; set; }
    /// <summary>
    /// Gets or sets the Transform matrix of the Stroke brush.
    /// </summary>
    public Matrix3x2 Transform
    {
        get
        {
            return GetTransform();
        }
        set
        {
            SetTransform(value);
        }
    }

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// ctor with default CanvasStrokeStyle
    /// </summary>
    /// <param name="brush">The brush with the stroke will be rendered</param>
    /// <param name="strokeWidth">Width of the stroke</param>
    public CanvasStroke(ICanvasBrush brush, float strokeWidth = 1f) :
        this(brush, strokeWidth, new CanvasStrokeStyle())
    {

    }

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="brush">The brush with the stroke will be rendered</param>
    /// <param name="strokeWidth">Width of the stroke</param>
    /// <param name="strokeStyle">Style of the stroke</param>
    public CanvasStroke(ICanvasBrush brush, float strokeWidth, CanvasStrokeStyle strokeStyle)
    {
        Brush = brush;
        Width = strokeWidth;
        Style = strokeStyle;
    }

    /// <summary>
    /// ctor with default CanvasStrokeStyle
    /// </summary>
    /// <param name="device">ICanvasResourceCreator</param>
    /// <param name="strokeColor">Color of the stroke</param>
    /// <param name="strokeWidth">Width of the stroke</param>
    public CanvasStroke(ICanvasResourceCreator device, Color strokeColor, float strokeWidth = 1f) :
        this(device, strokeColor, strokeWidth, new CanvasStrokeStyle())
    {

    }

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="device">ICanvasResourceCreator</param>
    /// <param name="strokeColor">Color of the stroke</param>
    /// <param name="strokeWidth">Width of the stroke</param>
    /// <param name="strokeStyle">Style of the stroke</param>
    public CanvasStroke(ICanvasResourceCreator device, Color strokeColor, float strokeWidth,
        CanvasStrokeStyle strokeStyle)
    {
        Brush = new CanvasSolidColorBrush(device, strokeColor);
        Width = strokeWidth;
        Style = strokeStyle;
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Sets the Stroke's Transform.
    /// </summary>
    /// <param name="value">Transform matrix to set</param>
    private void SetTransform(Matrix3x2 value)
    {
        if (Brush != null)
        {
            Brush.Transform = value;
        }
    }

    /// <summary>
    /// Gets the Stroke's Transform. If stroke is null, then returns
    /// Matrix3x2.Identity.
    /// </summary>
    /// <returns>Transform matrix of the Stroke</returns>
    private Matrix3x2 GetTransform()
    {
        return Brush?.Transform ?? Matrix3x2.Identity;
    }

    #endregion
}
