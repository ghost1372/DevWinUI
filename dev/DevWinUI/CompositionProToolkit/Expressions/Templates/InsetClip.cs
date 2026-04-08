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
/// Base Template class for InsetClip
/// </summary>
public abstract partial class InsetClipTemplate : ExpressionTemplate
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="name">Template name</param>
    internal InsetClipTemplate(string name) : base(name)
    {
        Type = typeof(InsetClip);
    }

    /// <summary>
    /// The offset from the bottom of the visual.
    /// </summary>
    public float BottomInset { get; }
    /// <summary>
    /// The offset from the left of the visual.
    /// </summary>
    public float LeftInset { get; }
    /// <summary>
    /// The offset from the right of the visual.
    /// </summary>
    public float RightInset { get; }
    /// <summary>
    /// The offset from the top of the visual.
    /// </summary>
    public float TopInset { get; }
    /// <summary>
    /// The angle of rotation applied to the clip, in radians.For a Windows.UI.Composition.CompositionClip,
    /// the axis of rotation is always about the z-axis, with positive RotationAngle
    /// values resulting in a clockwise rotation and negative values resulting in a counter-clockwise
    /// rotation. For specifying RotationAngle in degrees, use Windows.UI.Composition.CompositionClip.RotationAngleInDegrees.
    /// </summary>
    public float RotationAngle { get; }
    /// <summary>
    /// The angle of rotation applied to the clip, in degrees.For a Windows.UI.Composition.CompositionClip,
    /// the axis of rotation is always about the z-axis, with positive Windows.UI.Composition.CompositionClip.RotationAngleInDegrees
    /// values resulting in a clockwise rotation and negative values resulting in a counter-clockwise
    /// rotation. For specifying RotationAngle in radians, use Windows.UI.Composition.CompositionClip.RotationAngle. 
    /// </summary>
    public float RotationAngleInDegrees { get; }
    /// <summary>
    /// The point on the clip to be positioned at the clip's offset. Value is normalized
    /// with respect to the size of the clip. An AnchorPoint value of (0, 0) refers to
    /// the top-left corner of the untransformed clip and a value of (1, 1) refers to
    /// the bottom-right corner. Negative values and values greater than one are accepted
    /// but will result in an AnchorPoint that is outside the boundaries of the original,
    /// untransformed clip.
    /// </summary>
    public Vector2 AnchorPoint { get; }
    /// <summary>
    /// The point about which rotation or scaling occurs. Value is in pixels within the
    /// local coordinate space of the visual on which the clip is applied.
    /// </summary>
    public Vector2 CenterPoint { get; }
    /// <summary>
    /// The offset of the clip relative to the visual on which the clip is applied.
    /// </summary>
    public Vector2 Offset { get; }
    /// <summary>
    /// The scale to apply to the clip. The scale value is a multiplier of the clip's
    /// size. By default, the Scale value on a clip is (1, 1).
    /// </summary>
    public Vector2 Scale { get; }
    /// <summary>
    /// The transformation matrix to apply to the clip.
    /// </summary>
    public Matrix3x2 TransformMatrix { get; }
}

/// <summary>
/// Target Template for InsetClip. This provides a reference to
/// the InsetClip object to which an Expression is connected.
/// </summary>
public sealed partial class InsetClipTarget : InsetClipTemplate
{
    /// <summary>
    /// ctor
    /// </summary>
    public InsetClipTarget() : base(string.Empty)
    {

    }
}

/// <summary>
/// Reference Template for InsetClip. When you use this object to define
/// the reference in the Expression of any animation, you must call the 
/// SetReference() method of the ExpressionAnimation to set the reference
/// to actual InsetClip object.
/// </summary>
public sealed partial class InsetClipReference : InsetClipTemplate
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="referenceName">Reference name</param>
    public InsetClipReference(string referenceName) : base(referenceName)
    {

    }
}
