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

interface ICanvasElement
{
    #region Properties

    /// <summary>
    /// Gets or sets the list of render layers in CanvasElement
    /// </summary>
    List<ICanvasRenderLayer> Layers { get; set; }
    /// <summary>
    /// Gets or sets the flag which indicates whether the stroke
    /// should be scaled when the CanvasElement is scaled.
    /// </summary>
    bool ScaleStroke { get; set; }

    #endregion

    #region APIs

    /// <summary>
    /// Renders the layers of the CanvasElement, based on the specified dimensions,
    /// offset, padding and rotation, on a Canvas.
    /// </summary>
    /// <param name="session">CanvasDrawingSession</param>
    /// <param name="width">Target width of the rendered geometry</param>
    /// <param name="height">Target height of the rendered geometry</param>
    /// <param name="offset">Offset of the rendered geometry</param>
    /// <param name="padding">Padding of the surface on which the geometry
    /// is rendered.</param>
    /// <param name="rotation">Rotation angle (in radians) about the center of the 
    /// geometry</param>
    void Render(CanvasDrawingSession session, float width, float height, Vector2 offset,
        Vector4 padding, float rotation);

    /// <summary>
    /// Renders the layers of the CanvasElement, based on the specified dimensions,
    /// offset, padding and rotation, on a Canvas.
    /// </summary>
    /// <param name="session">CanvasDrawingSession</param>
    /// <param name="width">Target width of the rendered geometry</param>
    /// <param name="height">Target height of the rendered geometry</param>
    /// <param name="offset">Offset of the rendered geometry</param>
    /// <param name="padding">Padding of the surface on which the geometry
    /// is rendered.</param>
    /// <param name="rotation">Rotation angle (in radians) about the center of the 
    /// geometry</param>
    /// <param name="fill">Overrides layers having valid fill with given fill.</param>
    /// <param name="stroke">Ovderrides layers having valid stroke with given stroke.</param>
    void Render(CanvasDrawingSession session, float width, float height, Vector2 offset,
        Vector4 padding, float rotation, ICanvasBrush fill, ICanvasStroke stroke);

    /// <summary>
    /// Creates a SpriteVisual which contains SpriteVisuals representing each of the 
    /// render layers in the CanvasElement
    /// </summary>
    /// <param name="generator">ICompositionGenerator</param>
    /// <param name="width">Target width of the rendered geometry</param>
    /// <param name="height">Target height of the rendered geometry</param>
    /// <param name="offset">Offset of the rendered geometry</param>
    /// <param name="padding">Padding of the surface on which the geometry
    /// is rendered.</param>
    /// <param name="rotation">Rotation angle (in radians) about the center of the 
    /// geometry</param>
    /// <returns>SpriteVisual</returns>
    SpriteVisual CreateVisual(ICompositionGenerator generator, float width, float height,
        Vector2 offset, Vector4 padding, float rotation);

    #endregion
}
