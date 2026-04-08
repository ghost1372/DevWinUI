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
/// Represents the core interface for interfaces
/// which render onto ICompositionSurface.
/// </summary>
public interface IRenderSurface : IDisposable
{
    #region Properties

    /// <summary>
    /// Gets the Generator
    /// </summary>
    ICompositionGenerator Generator { get; }

    /// <summary>
    /// Gets the Surface
    /// </summary>
    ICompositionSurface Surface { get; }

    /// <summary>
    /// Gets the Surface Size
    /// </summary>
    Size Size { get; }

    #endregion

    #region APIs

    /// <summary>
    /// Redraws the surface
    /// </summary>
    void Redraw();

    /// <summary>
    /// Resizes the surface to the new size.
    /// </summary>
    /// <param name="size">New size of the surface</param>
    void Resize(Size size);

    #endregion
}
