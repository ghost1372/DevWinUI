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

using Microsoft.UI.Composition.Interactions;

namespace DevWinUI;

/// <summary>
/// Base Template class for InteractionTracker
/// </summary>
public abstract partial class InteractionTrackerTemplate : ExpressionTemplate
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="name">Template name</param>
    internal InteractionTrackerTemplate(string name) : base(name)
    {
        Type = typeof(InteractionTracker);
    }

    /// <summary>
    /// Boolean value indicating whether Inertia is from Impulse.
    /// </summary>
    public bool IsInertiaFromImpulse { get; }

    /// <summary>
    /// Boolean value indicating whether position rounding is currently suggested.
    /// </summary>
    public bool IsPositionRoundingSuggested { get; }
    /// <summary>
    /// The minimum scale for the Windows.UI.Composition.Interactions.InteractionTracker.
    /// </summary>
    public float MinScale { get; }
    /// <summary>
    /// The maximum scale for the Windows.UI.Composition.Interactions.InteractionTracker.
    /// </summary>
    public float MaxScale { get; }
    /// <summary>
    /// Natural resting position for the Windows.UI.Composition.Interactions.InteractionTracker.
    /// </summary>
    public float NaturalRestingScale { get; }
    /// <summary>
    /// The output scale calculated by the Windows.UI.Composition.Interactions.InteractionTracker.
    /// The current scale is a relative value that depends on the values specified in
    /// the Windows.UI.Composition.Interactions.InteractionTracker.MinScale and
    /// Windows.UI.Composition.Interactions.InteractionTracker.MaxScale
    /// properties.
    /// </summary>
    public float Scale { get; }
    /// <summary>
    /// Inertia decay rate for scale. Range is from 0 to 1.
    /// </summary>
    public float? ScaleInertiaDecayRate { get; }
    /// <summary>
    /// The rate of change for scale.
    /// </summary>
    public float ScaleVelocityInPercentPerSecond { get; }
    /// <summary>
    /// The minimum position allowed for the Windows.UI.Composition.Interactions.InteractionTracker.
    /// </summary>
    public Vector3 MinPosition { get; }
    /// <summary>
    /// The maximum position allowed for the Windows.UI.Composition.Interactions.InteractionTracker.
    /// </summary>
    public Vector3 MaxPosition { get; }
    /// <summary>
    /// Natural resting position for the Windows.UI.Composition.Interactions.InteractionTracker.
    /// </summary>
    public Vector3 NaturalRestingPosition { get; }
    /// <summary>
    /// The output position calculated by the Windows.UI.Composition.Interactions.InteractionTracker.
    /// The current position is a relative value. During the Idle and CustomAnimation
    /// states, it will always be between the values specified in the MinPosition and
    /// MaxPosition properties. The InteractionTracker’s position property can go outside
    /// this range during the Interacting and Inertia states in order to show a bounce
    /// or resistance at the boundary.
    /// </summary>
    public Vector3 Position { get; }
    /// <summary>
    /// Inertia decay rate for position. Range is from 0 to 1.
    /// </summary>
    public Vector3? PositionInertiaDecayRate { get; }
    /// <summary>
    /// The velocity currently applied to position.
    /// </summary>
    public Vector3 PositionVelocityInPixelsPerSecond { get; }


}

/// <summary>
/// Target Template for InteractionTracker. This provides a reference to
/// the InteractionTracker object to which an Expression is connected.
/// </summary>
public sealed partial class InteractionTrackerTarget : InteractionTrackerTemplate
{
    /// <summary>
    /// ctor
    /// </summary>
    public InteractionTrackerTarget() : base(string.Empty)
    {

    }
}

/// <summary>
/// Reference Template for InteractionTracker. When you use this object to define
/// the reference in the Expression of any animation, you must call the 
/// SetReference() method of the ExpressionAnimation to set the reference
/// to actual InteractionTracker object.
/// </summary>
public sealed partial class InteractionTrackerReference : InteractionTrackerTemplate
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="referenceName">Reference name</param>
    public InteractionTrackerReference(string referenceName) : base(referenceName)
    {

    }
}
