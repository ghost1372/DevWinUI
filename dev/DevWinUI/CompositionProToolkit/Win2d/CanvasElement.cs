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
/// Class representing a vector render object containing
/// one or more CanvasRenderLayers.
/// </summary>
public sealed partial class CanvasElement : ICanvasElement
{
    #region Fields

    private readonly Vector2 _center;
    private readonly Vector2 _baseSize;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the list of render layers in CanvasElement
    /// </summary>
    public List<ICanvasRenderLayer> Layers { get; set; }

    /// <summary>
    /// Gets or sets the flag which indicates whether the stroke
    /// should be scaled when the CanvasElement is scaled.
    /// </summary>
    public bool ScaleStroke { get; set; }

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// Creates a CanvasElement with the specified dimensions and layers.
    /// </summary>
    /// <param name="baseWidth">Base width of the CanvasElement.</param>
    /// <param name="baseHeight">Base height of the CanvasElement.</param>
    /// <param name="layers">Collection of render layers.</param>
    /// <param name="scaleStroke">Indicates whether the stroke width should
    /// be scaled when the CanvasElement is scaled.</param>
    public CanvasElement(float baseWidth, float baseHeight, IEnumerable<ICanvasRenderLayer> layers,
        bool scaleStroke = true)
    {
        // Base size
        _baseSize = new Vector2(baseWidth, baseHeight);
        // Center
        _center = _baseSize * 0.5f;
        // Layers
        Layers = new List<ICanvasRenderLayer>();
        if (layers != null)
        {
            Layers.AddRange(layers);
        }

        ScaleStroke = scaleStroke;
    }

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
    public void Render(CanvasDrawingSession session, float width, float height, Vector2 offset,
        Vector4 padding, float rotation)
    {
        for (var i = 0; i < Layers.Count(); i++)
        {
            var geometry = GetGeometry(i, width, height, offset, padding, rotation);
            if (geometry == null)
                continue;

            var fill = GetFill(i, width, height, offset, padding, rotation);
            if (fill != null)
            {
                session.FillGeometry(geometry, fill);
            }

            var stroke = GetStroke(i, width, height, offset, padding, rotation);
            if (stroke != null)
            {
                session.DrawGeometry(geometry, stroke.Brush, stroke.Width, stroke.Style);
            }
        }
    }

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
    /// <param name="fill">Overrides layers having valid fill with the given fill.</param>
    /// <param name="stroke">Ovderrides layers having valid stroke with the given stroke.</param>
    public void Render(CanvasDrawingSession session, float width, float height, Vector2 offset,
        Vector4 padding, float rotation, ICanvasBrush fill, ICanvasStroke stroke)
    {
        for (var i = 0; i < Layers.Count(); i++)
        {
            var geometry = GetGeometry(i, width, height, offset, padding, rotation);
            if (geometry == null)
                continue;

            var oldFill = GetFill(i, width, height, offset, padding, rotation);
            if (oldFill != null)
            {
                session.FillGeometry(geometry, fill);
            }

            var oldStroke = GetStroke(i, width, height, offset, padding, rotation);
            if (oldStroke != null)
            {
                session.DrawGeometry(geometry, stroke.Brush, stroke.Width, stroke.Style);
            }
        }
    }   
    
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
    public SpriteVisual CreateVisual(ICompositionGenerator generator, float width, float height,
        Vector2 offset, Vector4 padding, float rotation)
    {
        var rootVisual = generator.Compositor.CreateSpriteVisual();
        rootVisual.Size = new Vector2(width, height);

        for (var i = 0; i < Layers.Count(); i++)
        {
            var geometry = GetGeometry(i, width, height, offset, padding, rotation);
            if (geometry == null)
                continue;

            var fill = GetFill(i, width, height, offset, padding, rotation);
            var stroke = GetStroke(i, width, height, offset, padding, rotation);

            var geometrySurface = generator.CreateGeometrySurface(rootVisual.Size.ToSize(), geometry, stroke, fill);
            var surfaceBrush = generator.Compositor.CreateSurfaceBrush(geometrySurface.Surface);
            var visual = generator.Compositor.CreateSpriteVisual();
            visual.Size = rootVisual.Size;
            visual.Brush = surfaceBrush;

            rootVisual.Children.InsertAtTop(visual);
        }

        return rootVisual;
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Calculates the transform to be applied to the base size based on the
    /// specified dimension, offset, padding and rotation.
    /// </summary>
    /// <param name="width">Target width</param>
    /// <param name="height">Target height</param>
    /// <param name="offset">Target offset</param>
    /// <param name="padding">Padding of the surface on which rendering is done.</param>
    /// <param name="rotation">Rotation angle (in radians) about the center </param>
    /// <param name="scale">The scale factor</param>
    /// <returns>Matrix3x2</returns>
    private Matrix3x2 CalculateTransformMatrix(float width, float height, Vector2 offset,
        Vector4 padding, float rotation, out float scale)
    {
        // Content size
        var contentSize = new Vector2(width - padding.X - padding.Z, height - padding.Y - padding.W);
        // Scale
        if (contentSize.X < contentSize.Y)
        {
            scale = contentSize.X / _baseSize.X;
        }
        else
        {
            scale = contentSize.Y / _baseSize.Y;
        }

        // Offset
        offset = offset - _center + (new Vector2(width, height) * 0.5f);
        // Calculate the transform matrix
        var transformMatrix = Matrix3x2.CreateScale(scale, _center) *
                              Matrix3x2.CreateRotation(rotation, _center) *
                              Matrix3x2.CreateTranslation(offset);

        return transformMatrix;
    }

    /// <summary>
    /// Calculates the transform to be applied to the base size based on the
    /// specified dimension, offset, padding and rotation.
    /// </summary>
    /// <param name="width">Target width</param>
    /// <param name="height">Target height</param>
    /// <param name="offset">Target offset</param>
    /// <param name="padding">Padding of the surface on which rendering is done.</param>
    /// <param name="rotation">Rotation angle (in radians) about the center </param>
    /// <returns>Matrix3x2</returns>
    private Matrix3x2 CalculateTransformMatrix(float width, float height, Vector2 offset,
        Vector4 padding, float rotation)
    {
        float scale;
        return CalculateTransformMatrix(width, height, offset, padding, rotation, out scale);
    }

    /// <summary>
    /// Gets the CanvasGeometry associated with the specified CanvasRenderLayer 
    /// having the specified dimension, offset, padding and rotation.
    /// </summary>
    /// <param name="layer">Layer index</param>
    /// <param name="width">Target width of the rendered geometry</param>
    /// <param name="height">Target height of the rendered geometry</param>
    /// <param name="offset">Offset of the rendered geometry</param>
    /// <param name="padding">Padding of the surface on which the geometry
    /// is rendered.</param>
    /// <param name="rotation">Rotation angle (in radians) about the center of the 
    /// geometry</param>
    /// <returns>CanvasGeometry</returns>
    private CanvasGeometry GetGeometry(int layer, float width, float height,
        Vector2 offset, Vector4 padding, float rotation)
    {
        if ((layer < 0) || (layer >= Layers.Count) || (Layers[layer] == null))
        {
            return null;
        }

        // Calculate the final transform matrix of the geometry
        var transformMatrix = CalculateTransformMatrix(width, height, offset, padding, rotation);
        return Layers[layer].Geometry?.Transform(transformMatrix);
    }

    /// <summary>
    /// Gets the ICanvasBrush associated with the specified CanvasRenderLayer 
    /// having the specified dimension, offset, padding and rotation.
    /// </summary>
    /// <param name="layer">Layer index</param>
    /// <param name="width">Target width of the rendered geometry</param>
    /// <param name="height">Target height of the rendered geometry</param>
    /// <param name="offset">Offset of the rendered geometry</param>
    /// <param name="padding">Padding of the surface on which the geometry
    /// is rendered.</param>
    /// <param name="rotation">Rotation angle (in radians) about the center of the 
    /// geometry</param>
    /// <returns>ICanvasBrush</returns>
    private ICanvasBrush GetFill(int layer, float width, float height,
        Vector2 offset, Vector4 padding, float rotation)
    {
        if ((layer < 0) || (layer >= Layers.Count) || (Layers[layer] == null))
        {
            return null;
        }

        // Get the brush
        var fillBrush = Layers[layer].Brush;
        if (fillBrush == null)
            return null;

        // Calculate the final transform matrix of the brush
        var transformMatrix = CalculateTransformMatrix(width, height, offset, padding, rotation);
        fillBrush.Transform = transformMatrix;

        return fillBrush;
    }

    /// <summary>
    /// Gets the ICanvasStroke associated with the specified CanvasRenderLayer 
    /// having the specified dimension, offset, padding and rotation.
    /// </summary>
    /// <param name="layer">Layer index</param>
    /// <param name="width">Target width of the rendered geometry</param>
    /// <param name="height">Target height of the rendered geometry</param>
    /// <param name="offset">Offset of the rendered geometry</param>
    /// <param name="padding">Padding of the surface on which the geometry
    /// is rendered.</param>
    /// <param name="rotation">Rotation angle (in radians) about the center of the 
    /// geometry</param>
    /// <returns>ICanvasStroke</returns>
    private ICanvasStroke GetStroke(int layer, float width, float height,
        Vector2 offset, Vector4 padding, float rotation)
    {
        if ((layer < 0) || (layer >= Layers.Count) || (Layers[layer] == null))
        {
            return null;
        }

        // Get the stroke
        var stroke = Layers[layer].Stroke;
        if (stroke == null)
            return null;

        // Calculate the final transform matrix of the stroke
        float scale;
        var transformMatrix = CalculateTransformMatrix(width, height, offset, padding, rotation, out scale);
        // Does the stroke width need to be scaled?
        var strokeWidth = ScaleStroke ? stroke.Width * scale : stroke.Width;

        var result = new CanvasStroke(stroke.Brush, strokeWidth, stroke.Style) { Transform = transformMatrix };

        return result;
    }

    #endregion
}
