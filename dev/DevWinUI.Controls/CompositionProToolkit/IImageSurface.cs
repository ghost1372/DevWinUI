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
/// Enumeration to describe the status of the loading of an image
/// on the IImageSurface
/// </summary>
public enum ImageSurfaceLoadStatus
{
    /// <summary>
    /// Indicates that no image has been loaded on the IImageSurface
    /// </summary>
    None,
    /// <summary>
    /// Indicates that the image was successfully loaded on the IImageSurface.
    /// </summary>
    Success,
    /// <summary>
    /// Indicates that the image could not be loaded on the IImageSurface.
    /// </summary>
    Error
}

/// <summary>
/// Interface for rendering an image onto an ICompositionSurface
/// </summary>
public interface IImageSurface : IRenderSurface
{
    #region Events

    /// <summary>
    /// Event that is raised when the image has been downloaded, decoded and loaded
    /// to the underlying IImageSurface. This event fires regardless of success or failure.
    /// </summary>
    event TypedEventHandler<IImageSurface, ImageSurfaceLoadStatus> LoadCompleted;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the Uri of the image to be loaded onto the IImageSurface
    /// </summary>
    Uri Uri { get; }

    /// <summary>
    /// The CanvasBitmap representing the loaded image
    /// </summary>
    CanvasBitmap SurfaceBitmap { get; }

    /// <summary>
    /// Gets the image's resize and alignment options in the allocated space.
    /// </summary>
    ImageSurfaceOptions Options { get; }

    /// <summary>
    /// Gets the size of the decoded image in physical pixels.
    /// </summary>
    Size DecodedPhysicalSize { get; }

    /// <summary>
    /// Gets the size of the decoded image in device independent pixels.
    /// </summary>
    Size DecodedSize { get; }

    /// <summary>
    /// Gets the status whether the image was loaded successfully or not.
    /// </summary>
    ImageSurfaceLoadStatus Status { get; }

    #endregion

    #region APIs

    /// <summary>
    /// Redraws the IImageSurface or IImageMaskSurface with the given image options.
    /// </summary>
    /// <param name="options">The image's resize and alignment options in the allocated space.</param>
    void Redraw(ImageSurfaceOptions options);

    /// <summary>
    /// Redraws the IImageSurface (using the image in the given imageSurface) or the IImageMaskSurface
    /// (using the alpha values of image in the given imageSurface).
    /// </summary>
    /// <param name="imageSurface">IImageSurface whose image is to be loaded on the surface.</param>
    void Redraw(IImageSurface imageSurface);

    /// <summary>
    /// Redraws the IImageSurface (using the given CanvasBitmap) or the IImageMaskSurface
    /// (using the given CanvasBitmap's alpha values) using the given options.
    /// </summary>
    /// <param name="imageSurface">IImageSurface whose image is to be loaded on the surface.</param>
    /// <param name="options">Describes the image's resize, alignment options in the allocated space.</param>
    void Redraw(IImageSurface imageSurface, ImageSurfaceOptions options);

    /// <summary>
    /// Resizes and redraws the IImageSurface (using the given CanvasBitmap) or the IImageMaskSurface
    /// (using the given CanvasBitmap's alpha values) using the given options.
    /// </summary>
    /// <param name="imageSurface">IImageSurface whose image is to be loaded on the surface.</param>
    /// <param name="size">New size of the IImageMaskSurface.</param>
    /// <param name="options">Describes the image's resize, alignment options in the allocated space.</param>
    void Redraw(IImageSurface imageSurface, Size size, ImageSurfaceOptions options);

    /// <summary>
    /// Redraws the IImageSurface (using the given CanvasBitmap) or the IImageMaskSurface
    /// (using the given CanvasBitmap's alpha values).
    /// </summary>
    /// <param name="surfaceBitmap">Image to be loaded on the surface.</param>
    void Redraw(CanvasBitmap surfaceBitmap);

    /// <summary>
    /// Redraws the IImageSurface (using the given CanvasBitmap) or the IImageMaskSurface
    /// (using the given CanvasBitmap's alpha values) using the given options.
    /// </summary>
    /// <param name="surfaceBitmap">Image to be loaded on the surface.</param>
    /// <param name="options">Describes the image's resize, alignment options in the allocated space.</param>
    void Redraw(CanvasBitmap surfaceBitmap, ImageSurfaceOptions options);

    /// <summary>
    /// Resizes and redraws the IImageSurface (using the given CanvasBitmap) or the IImageMaskSurface
    /// (using the given CanvasBitmap's alpha values) using the given options.
    /// </summary>
    /// <param name="surfaceBitmap">Image to be loaded on the surface..</param>
    /// <param name="size">New size of the IImageMaskSurface.</param>
    /// <param name="options">Describes the image's resize, alignment options in the allocated space.</param>
    void Redraw(CanvasBitmap surfaceBitmap, Size size, ImageSurfaceOptions options);

    /// <summary>
    /// Redraws the IImageSurface or IImageMaskSurface by loading image from the new Uri and applying the image options.
    /// </summary>
    /// <param name="uri">Uri of the image to be loaded on to the surface.</param>
    /// <param name="options">The image's resize and alignment options in the allocated space.</param>
    /// <returns>Task</returns>
    Task RedrawAsync(Uri uri, ImageSurfaceOptions options);

    /// <summary>
    /// Resizes the IImageSurface or IImageMaskSurface with the given size and redraws it by loading 
    /// image from the new Uri.
    /// </summary>
    /// <param name="uri">Uri of the image to be loaded onto the IImageSurface.</param>
    /// <param name="size">New size of the IImageSurface</param>
    /// <param name="options">The image's resize and alignment options in the allocated space.</param>
    /// <returns>Task</returns>
    Task RedrawAsync(Uri uri, Size size, ImageSurfaceOptions options);

    /// <summary>
    /// Resizes the IImageSurface or IImageMaskSurface to the new size with the given image options.
    /// </summary>
    /// <param name="size">New size of the IImageSurface</param>
    /// <param name="options">The image's resize and alignment options in the allocated space.</param>
    void Resize(Size size, ImageSurfaceOptions options);

    #endregion
}
