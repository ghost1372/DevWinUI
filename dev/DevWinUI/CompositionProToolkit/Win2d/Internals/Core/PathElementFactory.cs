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

using System.Text.RegularExpressions;

namespace DevWinUI;

/// <summary>
/// Factory class to instantiate various PathElements.
/// </summary>
internal static partial class PathElementFactory
{
    #region APIs

    /// <summary>
    /// Creates a default Path Element for the given PathFigureType
    /// </summary>
    /// <param name="figureType">PathFigureType</param>
    /// <returns>ICanvasPathElement</returns>
    internal static ICanvasPathElement CreateDefaultPathElement(PathFigureType figureType)
    {
        ICanvasPathElement result;

        switch (figureType)
        {
            case PathFigureType.FillRule:
                result = new FillRuleElement();
                break;
            default:
                throw new ArgumentException("Creation of Only Default FillRuleElement is supported.", nameof(figureType));
        }

        return result;
    }

    /// <summary>
    /// Creates a default Path Element for the given PathElementType
    /// </summary>
    /// <param name="elementType">PathElementType</param>
    /// <returns>ICanvasPathElement</returns>
    internal static ICanvasPathElement CreateDefaultPathElement(PathElementType elementType)
    {
        ICanvasPathElement result;

        switch (elementType)
        {
            case PathElementType.ClosePath:
                result = new ClosePathElement();
                break;
            default:
                throw new ArgumentException("Creation of Only Default ClosePathElement is supported.", nameof(elementType));
        }

        return result;
    }

    /// <summary>
    /// Instantiates a PathElement based on the PathFigureType
    /// </summary>
    /// <param name="figureType">PathFigureType</param>
    /// <param name="match">Match object</param>
    /// <param name="index">Index of the match</param>
    /// <returns>ICanvasPathElement</returns>
    internal static ICanvasPathElement CreatePathFigure(PathFigureType figureType, Match match, int index)
    {
        var element = CreatePathElement(figureType);
        element?.Initialize(match, index);
        return element;
    }

    /// <summary>
    /// Instantiates a PathElement based on the PathFigureType
    /// </summary>
    /// <param name="figureType">PathFigureType</param>
    /// <param name="capture">Capture object</param>
    /// <param name="index">Index of the capture</param>
    /// <param name="isRelative">Indicates whether the coordinates are absolute or relative</param>
    /// <returns>ICanvasPathElement</returns>
    internal static ICanvasPathElement CreateAdditionalPathFigure(PathFigureType figureType, Capture capture,
        int index, bool isRelative)
    {
        var element = CreatePathElement(figureType);
        element?.InitializeAdditional(capture, index, isRelative);
        return element;
    }

    /// <summary>
    /// Instantiates a PathElement based on the PathElementType
    /// </summary>
    /// <param name="elementType">PathElementType</param>
    /// <param name="match">Match object</param>
    /// <param name="index">Index of the match</param>
    /// <returns>ICanvasPathElement</returns>
    internal static ICanvasPathElement CreatePathElement(PathElementType elementType, Match match, int index)
    {
        var element = CreatePathElement(elementType);
        element?.Initialize(match, index);
        return element;
    }

    /// <summary>
    /// Instantiates a PathElement based on the PathElementType
    /// </summary>
    /// <param name="elementType">PathElementType</param>
    /// <param name="capture">Capture object</param>
    /// <param name="index">Index of the capture</param>
    /// <param name="isRelative">Indicates whether the coordinates are absolute or relative</param>
    /// <returns>ICanvasPathElement</returns>
    internal static ICanvasPathElement CreateAdditionalPathElement(PathElementType elementType, Capture capture,
        int index, bool isRelative)
    {
        // Additional attributes in MoveTo Command must be converted
        // to Line commands
        if (elementType == PathElementType.MoveTo)
        {
            elementType = PathElementType.Line;
        }

        var element = CreatePathElement(elementType);
        element?.InitializeAdditional(capture, index, isRelative);
        return element;
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Instantiates a PathElement based on the PathFigureType
    /// </summary>
    /// <param name="figureType">PathFigureType</param>
    /// <returns>ICanvasPathElement</returns>
    private static ICanvasPathElement CreatePathElement(PathFigureType figureType)
    {
        ICanvasPathElement result = null;

        switch (figureType)
        {
            case PathFigureType.FillRule:
                result = new FillRuleElement();
                break;
            case PathFigureType.PathFigure:
                result = new CanvasPathFigure();
                break;
            case PathFigureType.EllipseFigure:
                result = new CanvasEllipseFigure();
                break;
            case PathFigureType.PolygonFigure:
                result = new CanvasPolygonFigure();
                break;
            case PathFigureType.RectangleFigure:
                result = new CanvasRectangleFigure();
                break;
            case PathFigureType.RoundedRectangleFigure:
                result = new CanvasRoundRectangleFigure();
                break;
        }

        return result;
    }

    /// <summary>
    /// Instantiates a PathElement based on the PathElementType
    /// </summary>
    /// <param name="elementType">PathElementType</param>
    /// <returns>ICanvasPathElement</returns>
    private static ICanvasPathElement CreatePathElement(PathElementType elementType)
    {
        ICanvasPathElement result = null;

        switch (elementType)
        {
            case PathElementType.MoveTo:
                result = new MoveToElement();
                break;
            case PathElementType.Line:
                result = new LineElement();
                break;
            case PathElementType.HorizontalLine:
                result = new HorizontalLineElement();
                break;
            case PathElementType.VerticalLine:
                result = new VerticalLineElement();
                break;
            case PathElementType.QuadraticBezier:
                result = new QuadraticBezierElement();
                break;
            case PathElementType.SmoothQuadraticBezier:
                result = new SmoothQuadraticBezierElement();
                break;
            case PathElementType.CubicBezier:
                result = new CubicBezierElement();
                break;
            case PathElementType.SmoothCubicBezier:
                result = new SmoothCubicBezierElement();
                break;
            case PathElementType.Arc:
                result = new ArcElement();
                break;
            case PathElementType.ClosePath:
                result = new ClosePathElement();
                break;
        }

        return result;
    }

    #endregion
}
