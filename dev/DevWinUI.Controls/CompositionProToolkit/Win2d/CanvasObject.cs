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

using System.Text;

namespace DevWinUI;

/// <summary>
/// Win2d Helper Class
/// </summary>
public static partial class CanvasObject
{
    #region APIs

    /// <summary>
    /// Parses the Path data string and converts it to CanvasGeometry.
    /// </summary>
    /// <param name="resourceCreator">ICanvasResourceCreator</param>
    /// <param name="pathData">Path data</param>
    /// <param name="logger">(Optional) For logging purpose. To log the set of  
    /// CanvasPathBuilder commands, used for creating the CanvasGeometry, in 
    /// string format.</param>
    /// <returns>CanvasGeometry</returns>
    public static CanvasGeometry CreateGeometry(ICanvasResourceCreator resourceCreator, string pathData,
        StringBuilder logger = null)
    {
        using (new CultureShield("en-US"))
        {
            // Log command
            logger?.AppendLine("using (var pathBuilder = new CanvasPathBuilder(resourceCreator))");
            logger?.AppendLine("{");

            // Get the CanvasGeometry from the path data
            var geometry = CanvasGeometryParser.Parse(resourceCreator, pathData, logger);

            // Log command
            logger?.AppendLine("}");

            return geometry; 
        }
    }

    /// <summary>
    /// Creates a Squircle geometry with the specified extents.
    /// </summary>
    /// <param name="resourceCreator"></param>
    /// <param name="x">X offset of the TopLeft corner of the Squircle</param>
    /// <param name="y">Y offset of the TopLeft corner of the Squircle</param>
    /// <param name="width">Width of the Squircle</param>
    /// <param name="height">Height of the Squircle</param>
    /// <param name="radiusX">Corner Radius on the x-axis</param>
    /// <param name="radiusY">Corner Radius on the y-axis</param>
    /// <returns>CanvasGeometry</returns>
    public static CanvasGeometry CreateSquircle(ICanvasResourceCreator resourceCreator, float x, float y,
        float width, float height, float radiusX, float radiusY)
    {
        using (var pathBuilder = new CanvasPathBuilder(resourceCreator))
        {
            pathBuilder.AddSquircleFigure(x, y, width, height, radiusX, radiusY);
            return CanvasGeometry.CreatePath(pathBuilder);
        }
    }

    /// <summary>
    /// Parses the Path data string and converts it to CanvasGeometry.
    /// </summary>
    /// <param name="pathData">Path data</param>
    /// <param name="logger">(Optional) For logging purpose. To log the set of  
    /// CanvasPathBuilder commands, used for creating the CanvasGeometry, in 
    /// string format.</param>
    /// <returns>CanvasGeometry</returns>
    public static CanvasGeometry CreateGeometry(string pathData, StringBuilder logger = null)
    {
        using (new CultureShield("en-US"))
        {
            // Log command
            logger?.AppendLine("using (var pathBuilder = new CanvasPathBuilder(null))");
            logger?.AppendLine("{");

            // Get the CanvasGeometry from the path data
            var geometry = CanvasGeometryParser.Parse(null, pathData, logger);

            // Log command
            logger?.AppendLine("}");

            return geometry;
        }
    }

    /// <summary>
    /// Parses the given Brush data string and converts it to ICanvasBrush
    /// </summary>
    /// <param name="resourceCreator">ICanvasResourceCreator</param>
    /// <param name="brushData">Brush data in string format</param>
    /// <returns>ICanvasBrush</returns>
    public static ICanvasBrush CreateBrush(ICanvasResourceCreator resourceCreator, string brushData)
    {
        using (new CultureShield("en-US"))
        {
            return CanvasBrushParser.Parse(resourceCreator, brushData); 
        }
    }

    /// <summary>
    /// Parses the given Stroke data string and converts it to ICanvasStroke
    /// </summary>
    /// <param name="resourceCreator">ICanvasResourceCreator</param>
    /// <param name="strokeData">Stroke data in string format</param> 
    /// <returns>ICanvasStroke</returns>
    public static ICanvasStroke CreateStroke(ICanvasResourceCreator resourceCreator, string strokeData)
    {
        using (new CultureShield("en-US"))
        {
            return CanvasStrokeParser.Parse(resourceCreator, strokeData); 
        }
    }

    /// <summary>
    /// Parses the give CanvasStrokeStyle data string and converts it to CanvasStrokeStyle
    /// </summary>
    /// <param name="styleData">CanvasStrokeStyle data in string format</param>
    /// <returns></returns>
    public static CanvasStrokeStyle CreateStrokeStyle(string styleData)
    {
        using (new CultureShield("en-US"))      
        {
            return CanvasStrokeStyleParser.Parse(styleData); 
        }
    }

    /// <summary>
    /// Converts the color string in Hexadecimal or HDR color format to the 
    /// corresponding Color object.
    /// The hexadecimal color string should be in #RRGGBB or #AARRGGBB format.
    /// The '#' character is optional.
    /// The HDR color string should be in R G B A format. 
    /// (R, G, B &amp; A should have value in the range between 0 and 1, inclusive)
    /// </summary>
    /// <param name="colorString">Color string in Hexadecimal or HDR format</param>
    /// <returns>Color</returns>
    public static Color CreateColor(string colorString)
    {
        using (new CultureShield("en-US"))
        {
            return ColorParser.Parse(colorString); 
        }
    }

    /// <summary>
    /// Attempts to convert color string in Hexadecimal or HDR color format to the 
    /// corresponding Color object.
    /// The hexadecimal color string should be in #RRGGBB or #AARRGGBB format.
    /// The '#' character is optional.
    /// The HDR color string should be in R G B A format. 
    /// (R, G, B &amp; A should have value in the range between 0 and 1, inclusive)
    /// </summary>
    /// <param name="colorString">Color string in Hexadecimal or HDR format</param>
    /// <param name="color">Output Color object</param>
    /// <returns>True if successful, otherwise False</returns>
    public static bool TryCreateColor(string colorString, out Color color)
    {
        using (new CultureShield("en-US"))
        {
            return ColorParser.TryParse(colorString, out color); 
        }
    }

    /// <summary>
    /// Converts a Vector4 High Dynamic Range Color to Color object. 
    /// Negative components of the Vector4 will be sanitized by taking the absolute 
    /// value of the component. The HDR Color components should have value in 
    /// the range between 0 and 1, inclusive. If they are more than 1, they
    /// will be clamped at 1.
    /// Vector4's X, Y, Z, W components match to Color's R, G, B, A components respectively.
    /// </summary>
    /// <param name="hdrColor">High Dynamic Range Color</param>
    /// <returns>Color</returns>
    public static Color CreateColor(Vector4 hdrColor)
    {
        using (new CultureShield("en-US"))
        {
            return ColorParser.Parse(hdrColor); 
        }
    }

    #endregion
}
