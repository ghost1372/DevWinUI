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
/// Base Template class for PointLight
/// </summary>
public abstract partial class PointLightTemplate : ExpressionTemplate
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="name">Template name</param>
    internal PointLightTemplate(string name) : base(name)
    {
        Type = typeof(PointLight);
    }

    /// <summary>
    /// The constant coefficient in the light's attenuation equation. Controls light
    /// intensity. Range is from 0 to infinity.
    /// </summary>
    public float ConstantAttenuation { get; }
    /// <summary>
    /// The linear coefficient in the light's attenuation equation that determines how
    /// the light falls-off with distance. Range is from 0 to infinity.
    /// </summary>
    public float LinearAttenuation { get; }
    /// <summary>
    /// The quadratic portion of the attenuation equation that determines how the light
    /// falls off with distance. Controls light intensity falloff based on distance squared.
    /// Range is from 0 to infinity.
    /// </summary>
    public float QuadraticAttentuation { get; }
    /// <summary>
    /// Color of the light.
    /// </summary>
    public Color Color { get; }
    /// <summary>
    /// The intensity of the light.
    /// </summary>
    public float Intensity { get; }
    /// <summary>
    /// Offset of the light source relative to its coordinate space Visual.
    /// </summary>
    public Vector3 Offset { get; }
    /// <summary>
    /// Gets the Minimun Attenuation Cutoff
    /// </summary>
    public float MinAttenuationCutoff { get; }
    /// <summary>
    /// Gets the Maximun Attenuation Cutoff
    /// </summary>
    public float MaxAttenuationCutoff { get; }
}

/// <summary>
/// Target Template for PointLight. This provides a reference to
/// the PointLight object to which an Expression is connected.
/// </summary>
public sealed partial class PointLightTarget : PointLightTemplate
{
    /// <summary>
    /// ctor
    /// </summary>
    public PointLightTarget() : base(string.Empty)
    {

    }
}

/// <summary>
/// Reference Template for PointLight. When you use this object to define
/// the reference in the Expression of any animation, you must call the 
/// SetReference() method of the ExpressionAnimation to set the reference
/// to actual PointLight object.
/// </summary>
public sealed partial class PointLightReference : PointLightTemplate
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="referenceName">Reference name</param>
    public PointLightReference(string referenceName) : base(referenceName)
    {

    }
}
