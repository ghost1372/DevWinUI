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
/// Base Template class for DistantLight
/// </summary>
public abstract partial class DistantLightTemplate : ExpressionTemplate
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="name">Template name</param>
    internal DistantLightTemplate(string name) : base(name)
    {
        Type = typeof(DistantLight);
    }

    /// <summary>
    /// Color of the emitted DistantLight.
    /// </summary>
    public Color Color { get; }

    /// <summary>
    /// The direction in which the light is pointing, specified 
    /// relative to its Windows.UI.Composition.DistantLight.CoordinateSpace Visual.
    /// </summary>
    public Vector3 Direction { get; }

    /// <summary>
    /// Gets the intensity of the light.
    /// </summary>
    public float Intensity { get; }
    
    /// <summary>
    /// Gets the CoordinateSpace Visual
    /// </summary>
    public Visual CoordinateSpace { get; }
}

/// <summary>
/// Target Template for DistantLight. This provides a reference to
/// the DistantLight object to which an Expression is connected.
/// </summary>
public sealed partial class DistantLightTarget : DistantLightTemplate
{
    /// <summary>
    /// ctor
    /// </summary>
    public DistantLightTarget() : base(string.Empty)
    {

    }
}

/// <summary>
/// Reference Template for DistantLight. When you use this object to define
/// the reference in the Expression of any animation, you must call the 
/// SetReference() method of the ExpressionAnimation to set the reference 
/// to actual DistantLight object.
/// </summary>
public sealed partial class DistantLightReference : DistantLightTemplate
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="referenceName">Reference name</param>
    public DistantLightReference(string referenceName) : base(referenceName)
    {

    }
}
