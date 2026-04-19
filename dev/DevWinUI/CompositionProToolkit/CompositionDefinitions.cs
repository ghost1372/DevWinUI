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
/// Enum to define the location of the
/// Reflection of a Visual.
/// </summary>
public enum ReflectionLocation
{
    /// <summary>
    /// Reflection at bottom of visual
    /// </summary>
    Bottom = 0,
    /// <summary>
    /// Reflection at top of visual
    /// </summary>
    Top = 1,
    /// <summary>
    /// Reflection at left of visual
    /// </summary>
    Left = 2,
    /// <summary>
    /// Reflection at right of visual
    /// </summary>
    Right = 3
}

/// <summary>
/// Class to define the various options that would
/// influence the rendering of the image on the ImageSurface
/// </summary>
public partial class ImageSurfaceOptions
{
    #region Properties

    /// <summary>
    /// <para>Specifies whether the IImageSurface should resize itself automatically to match the loaded image size.</para>
    /// <para>NOTE: This property is not used by IImageMaskSurface.</para> 
    /// </summary>
    public bool AutoResize { get; set; } = false;

    /// <summary>
    /// <para>Describes how image is resized to fill its allocated space.</para>
    /// <para>NOTE: This property is taken into consideration only if AutoResize is False.</para>
    /// </summary>
    public Stretch Stretch { get; set; } = Stretch.None;

    /// <summary>
    /// <para>Describes how image is positioned horizontally in the IImageSurface or IImageMaskSurface.</para>
    /// <para>NOTE: This property is taken into consideration only if AutoResize is False.</para>
    /// </summary>
    public AlignmentX HorizontalAlignment { get; set; } = AlignmentX.Center;

    /// <summary>
    /// <para>Describes how image is positioned vertically in the IImageSurface or IImageMaskSurface.</para>
    /// <para>NOTE: This property is taken into consideration only if AutoResize is False.</para>
    /// </summary>
    public AlignmentY VerticalAlignment { get; set; } = AlignmentY.Center;

    /// <summary>
    /// Specifies the opacity of the rendered the image in an IImageSurface or the mask in an IImageMaskSurface.
    /// </summary>
    public float Opacity { get; set; } = 1f;

    /// <summary>
    /// Specifies the interpolation used to render the image in an IImageSurface or the mask in an IImageMaskSurface.
    /// </summary>
    public CanvasImageInterpolation Interpolation { get; set; } = CanvasImageInterpolation.HighQualityCubic;

    /// <summary>
    /// Color which will be used to fill the IImageSurface in an IImageSurface or the mask in an IImageMaskSurface
    /// in case the image is not rendered.
    /// </summary>
    public Color SurfaceBackgroundColor { get; set; } = Colors.Transparent;

    /// <summary>
    /// <para>Radius of the Gaussian blur to be applied to the IImageMaskSurface.</para>
    /// <para>NOTE: This property is not used by IImageSurface.</para>
    /// </summary>
    public float BlurRadius { get; set; } = 0f;

    #endregion

    #region Static Properties

    /// <summary>
    /// Default ImageSurfaceOptions when AutoResize is True
    /// Uniform Stretch and Center alignment
    /// </summary>
    public static ImageSurfaceOptions Default =>
                new ImageSurfaceOptions()
                {
                    AutoResize = true,
                    Interpolation = CanvasImageInterpolation.HighQualityCubic,
                    Opacity = 1f,
                    Stretch = Stretch.Uniform,
                    HorizontalAlignment = AlignmentX.Center,
                    VerticalAlignment = AlignmentY.Center,
                    SurfaceBackgroundColor = Colors.Transparent,
                    BlurRadius = 0f
                };

    /// <summary>
    /// Default ImageSurfaceOptions when AutoResize is False
    /// Uniform Stretch and Center alignment
    /// </summary>
    public static ImageSurfaceOptions DefaultOptimized =>
                new ImageSurfaceOptions()
                {
                    AutoResize = false,
                    Interpolation = CanvasImageInterpolation.HighQualityCubic,
                    Opacity = 1f,
                    Stretch = Stretch.Uniform,
                    HorizontalAlignment = AlignmentX.Center,
                    VerticalAlignment = AlignmentY.Center,
                    SurfaceBackgroundColor = Colors.Transparent,
                    BlurRadius = 0f
                };

    /// <summary>
    /// Default ImageSurfaceOptions for IImageMaskSurface
    /// Uniform Stretch and Center alignment
    /// </summary>
    public static ImageSurfaceOptions DefaultImageMaskOptions =>
        new ImageSurfaceOptions()
        {
            AutoResize = false,
            Interpolation = CanvasImageInterpolation.HighQualityCubic,
            Opacity = 1f,
            Stretch = Stretch.Uniform,
            HorizontalAlignment = AlignmentX.Center,
            VerticalAlignment = AlignmentY.Center,
            SurfaceBackgroundColor = Colors.Transparent,
            BlurRadius = 0f
        };

    /// <summary>
    /// Creates ImageSurfaceOptions for IImageMaskSurface for the given blurRadius -
    /// Uniform Stretch and Center alignment
    /// </summary>
    /// <param name="blurRadius">Radius of the Gaussian Blur to be applied on the IImageMaskSurface.</param>
    public static ImageSurfaceOptions GetDefaultImageMaskOptionsForBlur(float blurRadius)
    {
        return new ImageSurfaceOptions()
        {
            AutoResize = false,
            Interpolation = CanvasImageInterpolation.HighQualityCubic,
            Opacity = 1f,
            Stretch = Stretch.Uniform,
            HorizontalAlignment = AlignmentX.Center,
            VerticalAlignment = AlignmentY.Center,
            SurfaceBackgroundColor = Colors.Transparent,
            BlurRadius = blurRadius
        };
    }

    #endregion
}
