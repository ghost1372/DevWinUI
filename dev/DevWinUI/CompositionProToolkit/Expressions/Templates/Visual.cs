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
/// Base Template class for Visual
/// </summary>
public abstract partial class VisualTemplate : ExpressionTemplate
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="name">Template name</param>
    internal VisualTemplate(string name) : base(name)
    {
        Type = typeof(Visual);
    }

    /// <summary>
    /// The opacity of the visual.
    /// </summary>
    public float Opacity { get; }
    /// <summary>
    /// The rotation angle in radians of the visual.
    /// </summary>
    public float RotationAngle { get; }
    /// <summary>
    /// The rotation angle in degrees of the visual.
    /// </summary>
    public float RotationAngleInDegrees { get; }
    /// <summary>
    /// The point on the visual to be positioned at the visual's offset. Value is normalized
    /// with respect to the size of the visual.
    /// </summary>
    public Vector2 AnchorPoint { get; }
    /// <summary>
    /// The size of the visual with respect to the size of its parent visual.
    /// </summary>
    public Vector2 RelativeSizeAdjustment { get; }
    /// <summary>
    /// The width and height of the visual.
    /// </summary>
    public Vector2 Size { get; }
    /// <summary>
    /// The point about which rotation or scaling occurs.
    /// </summary>
    public Vector3 CenterPoint { get; }
    /// <summary>
    /// The offset of the visual relative to its parent or for a root visual the offset
    /// relative to the upper-left corner of the windows that hosts the visual.
    /// </summary>
    public Vector3 Offset { get; }
    /// <summary>
    /// The offset of the visual with respect to the size of its parent visual.
    /// </summary>
    public Vector3 RelativeOffsetAdjustment { get; }
    /// <summary>
    /// The axis to rotate the visual around.
    /// </summary>
    public Vector3 RotationAxis { get; }
    /// <summary>
    /// The scale to apply to the visual. 
    /// </summary>
    public Vector3 Scale { get; }
    /// <summary>
    /// A quaternion describing an orientation and rotation in 3D space that will be
    /// applied to the visual.
    /// </summary>
    public Quaternion Orientation { get; }
    /// <summary>
    /// The transformation matrix to apply to the visual.
    /// </summary>
    public Matrix4x4 TransformMatrix { get; }
    /// <summary>
    /// The translation to apply to the visual.
    /// </summary>
    public Vector3 Translation { get; }
    /// <summary>
    /// The internal PropertySet of the Visual
    /// </summary>
    public CompositionPropertySet Properties { get; }
}

/// <summary>
/// Target Template for Visual. This provides a reference to
/// the Visual object to which an Expression is connected.
/// </summary>
public sealed partial class VisualTarget : VisualTemplate
{
    /// <summary>
    /// ctor
    /// </summary>
    public VisualTarget() : base(string.Empty)
    {

    }
}

/// <summary>
/// Reference Template for Visual. When you use this object to define
/// the reference in the Expression of any animation, you must call the 
/// SetReference() method of the ExpressionAnimation to set the reference
/// to actual Visual object.
/// </summary>
public sealed partial class VisualReference : VisualTemplate
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="referenceName">Reference name</param>
    public VisualReference(string referenceName) : base(referenceName)
    {

    }
}
