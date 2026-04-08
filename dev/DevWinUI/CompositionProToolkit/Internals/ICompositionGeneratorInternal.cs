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
/// Internal interface for the CompositionMaskGenerator
/// </summary>
internal interface ICompositionGeneratorInternal : ICompositionGenerator
{
    /// <summary>
    /// Creates a CompositionDrawingSurface for the given size.
    /// </summary>
    /// <param name="surfaceLock">The object to lock to prevent multiple threads
    /// from accessing the surface at the same time.</param>
    /// <param name="size">Size of the Mask Surface</param>
    /// <returns>CompositionDrawingSurface</returns>
    CompositionDrawingSurface CreateDrawingSurface(object surfaceLock, Size size);

    /// <summary>
    /// Resizes the MaskSurface to the given size.
    /// </summary>
    /// <param name="surfaceLock">The object to lock to prevent multiple threads
    /// from accessing the surface at the same time.</param>
    /// <param name="surface">CompositionDrawingSurface</param>
    /// <param name="size">New size of the Mask Surface</param>
    void ResizeDrawingSurface(object surfaceLock, CompositionDrawingSurface surface, Size size);

    /// <summary>
    /// Redraws the MaskSurface with the given size and geometry.
    /// </summary>
    /// <param name="surfaceLock">The object to lock to prevent multiple threads
    /// from accessing the surface at the same time.</param>
    /// <param name="surface">CompositionDrawingSurface</param>
    /// <param name="size">Size of the MaskSurface</param>
    /// <param name="geometry">Geometry of the MaskSurface</param>
    /// <param name="offset">The offset from the top left corner of the ICompositionSurface where
    /// the Geometry is rendered.</param>
    void RedrawMaskSurface(object surfaceLock, CompositionDrawingSurface surface, Size size, CanvasGeometry geometry, Vector2 offset);

    /// <summary>
    /// Redraws the GaussianMaskSurface with the given size and geometry.
    /// </summary>
    /// <param name="surfaceLock">The object to lock to prevent multiple threads
    /// from accessing the surface at the same time.</param>
    /// <param name="surface">CompositionDrawingSurface</param>
    /// <param name="size">Size of the MaskSurface</param>
    /// <param name="geometry">Geometry of the MaskSurface</param>
    /// <param name="offset">The offset from the top left corner of the ICompositionSurface where the Geometry is rendered.</param>
    /// <param name="blurRadius">Radius of Gaussian Blur to be applied on the GaussianMaskSurface</param>
    void RedrawGaussianMaskSurface(object surfaceLock, CompositionDrawingSurface surface, Size size, CanvasGeometry geometry, 
        Vector2 offset, float blurRadius);

    /// <summary>
    /// Redraws the GeometrySurface with the given size, geometry, foreground brush and background brush.
    /// </summary>
    /// <param name="surfaceLock">The object to lock to prevent multiple threads
    /// from accessing the surface at the same time.</param>
    /// <param name="surface">CompositionDrawingSurface</param>
    /// <param name="size">Size of the GeometrySurface</param>
    /// <param name="geometry">Geometry of the GeometrySurface</param>
    /// <param name="stroke">ICanvasStroke defining the outline for the geometry</param>
    /// <param name="fillBrush">The brush with which the geometry has to be filled</param>
    /// <param name="backgroundBrush">The brush with which the GeometrySurface background has to be filled</param>
    void RedrawGeometrySurface(object surfaceLock, CompositionDrawingSurface surface, Size size,
        CanvasGeometry geometry, ICanvasStroke stroke, ICanvasBrush fillBrush, ICanvasBrush backgroundBrush);

    /// <summary>
    /// Resizes the ImageSurface to the given size and redraws the ImageSurface by rendering the canvasBitmap onto the surface.
    /// </summary>
    /// <param name="surfaceLock">The object to lock to prevent multiple threads from accessing the surface at the same time.</param>
    /// <param name="surface">CompositionDrawingSurface</param>
    /// <param name="options">Describes the image's resize and alignment options in the allocated space.</param>
    /// <param name="canvasBitmap">The CanvasBitmap on which the image is loaded.</param>
    void RedrawImageSurface(object surfaceLock, CompositionDrawingSurface surface, ImageSurfaceOptions options,
        CanvasBitmap canvasBitmap);

    /// <summary>
    /// Resizes the ImageSurface with the given size and redraws the ImageSurface by loading image from the new Uri.
    /// </summary>
    /// <param name="surfaceLock">The object to lock to prevent multiple threads
    /// from accessing the surface at the same time.</param>
    /// <param name="surface">CompositionDrawingSurface</param>
    /// <param name="uri">Uri of the image to be loaded onto the IImageSurface.</param>
    /// <param name="options">Describes the image's resize and alignment options in the allocated space.</param>
    /// <param name="canvasBitmap">The CanvasBitmap on which the image is loaded.</param>
    /// <returns>CanvasBitmap</returns>
    Task<CanvasBitmap> RedrawImageSurfaceAsync(object surfaceLock, CompositionDrawingSurface surface, Uri uri,
        ImageSurfaceOptions options, CanvasBitmap canvasBitmap);

    /// <summary>
    /// Resizes the ImageMaskSurface to the given size and redraws the ImageMaskSurface by rendering the mask
    /// using the image's alpha values onto the surface. 
    /// </summary>
    /// <param name="surfaceLock">The object to lock to prevent multiple threads from accessing the surface at the same time.</param>
    /// <param name="surface">CompositionDrawingSurface</param>
    /// <param name="options">Describes the image's resize and alignment options and blur radius in the allocated space.</param>
    /// <param name="padding">The padding between the IImageMaskSurface outer bounds and the bounds of the area where
    /// the mask, created from the loaded image's alpha values, should be rendered.</param>
    /// <param name="surfaceBitmap">The image whose alpha values is used to create the IImageMaskSurface.</param>
    void RedrawImageMaskSurface(object surfaceLock, CompositionDrawingSurface surface, Thickness padding, ImageSurfaceOptions options,
        CanvasBitmap surfaceBitmap);

    /// <summary>
    /// Resizes the ImageMaskSurface to the given size and redraws the ImageMaskSurface by loading the image from the new Uri and
    /// rendering the mask using the image's alpha values onto the surface.
    /// </summary>
    /// <param name="surfaceLock">The object to lock to prevent multiple threads
    /// from accessing the surface at the same time.</param>
    /// <param name="surface">CompositionDrawingSurface</param>
    /// <param name="uri">Uri of the image to be loaded onto the IImageMaskSurface.</param>
    /// <param name="padding">The padding between the IImageMaskSurface outer bounds and the bounds of the area where
    /// the mask, created from the loaded image's alpha values, should be rendered.</param>
    /// <param name="options">Describes the image's resize and alignment options and blur radius in the allocated space.</param>
    /// <param name="surfaceBitmap">The CanvasBitmap on which the image is loaded.</param>
    /// <returns>The CanvasBitmap whose alpha values is used to create the IImageMaskSurface.</returns>
    Task<CanvasBitmap> RedrawImageMaskSurfaceAsync(object surfaceLock, CompositionDrawingSurface surface, Uri uri, Thickness padding,
        ImageSurfaceOptions options, CanvasBitmap surfaceBitmap);
}
