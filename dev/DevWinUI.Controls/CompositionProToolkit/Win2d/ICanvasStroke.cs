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
/// Interface to represent the Stroke which
/// can be used to render an outline on a CanvasGeometry
/// </summary>
public interface ICanvasStroke
{
    /// <summary>
    /// Gets or sets the brush with which the stroke will be rendered
    /// </summary>
    ICanvasBrush Brush { get; set; }

    /// <summary>
    /// Gets or sets the width of the stroke
    /// </summary>
    float Width { get; set; }

    /// <summary>
    /// Gets or sets the Style of the stroke
    /// </summary>
    CanvasStrokeStyle Style { get; set; }

    /// <summary>
    /// Gets or sets transform matrix of the Stroke brush.
    /// </summary>
    Matrix3x2 Transform { get; set; }
}
