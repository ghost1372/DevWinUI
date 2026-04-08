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
/// Base Template class for SpotLight
/// </summary>
public abstract partial class SpotLightTemplate: ExpressionTemplate
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="name">Template name</param>
    internal SpotLightTemplate(string name) : base(name)
    {
        Type = typeof(SpotLight);
    }

    /// <summary>
    /// The constant coefficient in the light's attenuation equation. Controls light
    /// intensity. Range is from 0 to infinity.
    /// </summary>
    public float ConstantAttenuation { get; }
    /// <summary>
    /// The linear coefficient in the light's attenuation equation that determines how
    /// the light falls off with distance. Range is from 0 to infinity.
    /// </summary>
    public float LinearAttenuation { get; }
    /// <summary>
    /// The quadratic coefficient in the light's attenuation equation. Controls light
    /// intensity falloff based on distance squared. Range is from 0 to infinity.
    /// </summary>
    public float QuadraticAttentuation { get; }
    /// <summary>
    /// The SpotLight’s inner cone angle, expressed as a semi-vertical angle in radians.
    /// </summary>
    public float InnerConeAngle { get; }
    /// <summary>
    /// The SpotLight’s inner cone angle, expressed as a semi-vertical angle in degrees.
    /// </summary>
    public float InnerConeAngleInDegrees { get; }
    /// <summary>
    /// The SpotLight’s outer cone angle, expressed as a semi-vertical angle in radians.
    /// The value must be between 0 and pi.
    /// </summary>
    public float OuterConeAngle { get; }
    /// <summary>
    /// The SpotLight’s inner cone angle, expressed as a semi-vertical angle in degrees.
    /// </summary>
    public float OuterConeAngleInDegrees { get; }
    /// <summary>
    /// Color of the SpotLight.
    /// </summary>
    public Color Color { get; }
    /// <summary>
    /// The color of the spotlight's inner cone.
    /// </summary>
    public Color InnerConeColor { get; }
    /// <summary>
    /// The color of the spotlight's outer cone.
    /// </summary>
    public Color OuterConeColor { get; }
    /// <summary>
    /// The direction in which the light is pointing, specified relative to its coordinate
    /// space Visual.
    /// </summary>
    public Vector3 Direction { get; }
    /// <summary>
    /// Offset of the light source relative to its coordinate space Visual.
    /// </summary>
    public Vector3 Offset { get; }
    /// <summary>
    /// Gets the intensity of the light in the spotlight's outer cone.
    /// </summary>
    public float OuterConeIntensity { get; }
    /// <summary>
    /// Gets the intensity of the light in the spotlight's inner cone.
    /// </summary>
    public float InnerConeIntensity { get; }
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
/// Target Template for SpotLight. This provides a reference to
/// the SpotLight object to which an Expression is connected.
/// </summary>
public sealed partial class SpotLightTarget : SpotLightTemplate
{
    /// <summary>
    /// ctor
    /// </summary>
    public SpotLightTarget() : base(string.Empty)
    {

    }
}

/// <summary>
/// Reference Template for SpotLight. When you use this object to define
/// the reference in the Expression of any animation, you must call the 
/// SetReference() method of the ExpressionAnimation to set the reference
/// to actual SpotLight object.
/// </summary>
public sealed partial class SpotLightReference : SpotLightTemplate
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="referenceName">Reference name</param>
    public SpotLightReference(string referenceName) : base(referenceName)
    {

    }
}
