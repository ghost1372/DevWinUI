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
/// Class for rendering custom shaped geometries onto ICompositionSurface
/// so that they can be used as masks on Composition Visuals.
/// </summary>
internal sealed partial class MaskSurface : IMaskSurface
{
    #region Fields

    private ICompositionGeneratorInternal _generator;
    private CompositionDrawingSurface _surface;
    private readonly object _surfaceLock;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the Composition Generator
    /// </summary>
    public ICompositionGenerator Generator => _generator;

    /// <summary>
    /// Gets the Surface of MaskSurface
    /// </summary>
    public ICompositionSurface Surface => _surface;
    
    /// <summary>
    /// Gets the Geometry of the MaskSurface
    /// </summary>
    public CanvasGeometry Geometry { get; private set; }
    
    /// <summary>
    /// Gets the Size of the MaskSurface
    /// </summary>
    public Size Size { get; private set; }

    /// <summary>
    /// Gets the offset from the top left corner of the ICompositionSurface where
    /// the Geometry is rendered.
    /// </summary>
    public Vector2 Offset { get; private set; }

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="generator">ICompositionMaskGeneratorInternal object</param>
    /// <param name="size">Size of the MaskSurface</param>
    /// <param name="geometry">Geometry of the MaskSurface</param>
    /// <param name="offset">The offset from the top left corner of the ICompositionSurface where
    /// the Geometry is rendered.</param>
    public MaskSurface(ICompositionGeneratorInternal generator, Size size, CanvasGeometry geometry, Vector2 offset)
    {
        _generator = generator ?? throw new ArgumentNullException(nameof(generator), "CompositionGenerator cannot be null!");
        _surfaceLock = new object();
        Geometry = geometry;
        Offset = offset;
        // Create Mask Surface
        _surface = _generator.CreateDrawingSurface(_surfaceLock, size);
        // Set the size
        Size = _surface?.Size ?? new Size(0, 0);
        // Subscribe to DeviceReplaced event
        _generator.DeviceReplaced += OnDeviceReplaced;
    }

    #endregion

    #region APIs

    /// <summary>
    /// Redraws the MaskSurface
    /// </summary>
    public void Redraw()
    {
        // Redraw the mask surface
        RedrawSurface();
    }

    /// <summary>
    /// Redraws the MaskSurface with the new geometry
    /// </summary>
    /// <param name="geometry">New CanvasGeometry to be applied to the mask</param>
    public void Redraw(CanvasGeometry geometry)
    {
        Redraw(Size, geometry, Offset);
    }

    /// <summary>
    /// Redraws the IMaskSurface with the new geometry
    /// </summary>
    /// <param name="geometry">New CanvasGeometry to be applied to the IMaskSurface</param>
    /// <param name="offset">The offset from the top left corner of the ICompositionSurface where
    /// the Geometry is rendered.</param>
    public void Redraw(CanvasGeometry geometry, Vector2 offset)
    {
        Redraw(Size, geometry, offset);
    }

    /// <summary>
    /// Resizes the MaskSurface with the given size and redraws the MaskSurface
    /// with the new geometry and fills it either with White color
    /// (if the MaskMode is True) or with the foreground brush
    /// (if the MaskMode is False).
    /// </summary>
    /// <param name="size">New size of the mask</param>
    /// <param name="geometry">New CanvasGeometry to be applied to the mask</param>
    public void Redraw(Size size, CanvasGeometry geometry)
    {
        Redraw(size, geometry, Offset);
    }

    /// <summary>
    /// Resizes the IMaskSurface with the given size and redraws the IMaskSurface
    /// with the new geometry and fills it with White color
    /// </summary>
    /// <param name="size">New size of the mask</param>
    /// <param name="geometry">New CanvasGeometry to be applied to the IMaskSurface</param>
    /// <param name="offset">The offset from the top left corner of the ICompositionSurface where
    /// the Geometry is rendered.</param>
    public void Redraw(Size size, CanvasGeometry geometry, Vector2 offset)
    {
        if (Size != size)
        {
            // Resize the mask surface
            _generator.ResizeDrawingSurface(_surfaceLock, _surface, size);
            // Set the size
            Size = _surface?.Size ?? new Size(0, 0);
        }

        // Set the new geometry
        Geometry = geometry;
        // Set the offset
        Offset = offset;
        // Redraw the mask surface
        RedrawSurface();
    }

    /// <summary>
    /// Resizes the MaskSurface to the new size.
    /// </summary>
    /// <param name="size">New size of the mask</param>
    public void Resize(Size size)
    {
        // resize the mask surface
        _generator.ResizeDrawingSurface(_surfaceLock, _surface, size);
        // Set the size
        Size = _surface?.Size ?? new Size(0, 0);
        // Redraw the mask surface
        RedrawSurface();
    }

    /// <summary>
    /// Disposes the resources used by the MaskSurface
    /// </summary>
    public void Dispose()
    {
        _surface?.Dispose();
        Geometry?.Dispose();
        Geometry = null;
        if (_generator != null)
            _generator.DeviceReplaced -= OnDeviceReplaced;
        _surface = null;
        _generator = null;
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles the DeviceReplaced event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">object</param>
    private void OnDeviceReplaced(object sender, object e)
    {
        // Recreate the MaskSurface
        _surface = _generator.CreateDrawingSurface(_surfaceLock, Size);
        // Redraw the mask surface
        RedrawSurface();
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Helper class to redraw the MaskSurface
    /// </summary>
    private void RedrawSurface()
    {
        _generator.RedrawMaskSurface(_surfaceLock, _surface, Size, Geometry, Offset);
    }

    #endregion
}
