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

using System.Linq.Expressions;
using Windows.Graphics.Effects;

namespace DevWinUI;

/// <summary>
/// Extension methods for Windows.UI.Composition.Compositor
/// The easing function values have been obtained from http://easings.net/
/// </summary>
public static partial class CompositorExtensions
{
    #region CompositionGenerator

    /// <summary>
    /// Creates a CompositionGenerator object
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>ICompositionGenerator</returns>
    public static ICompositionGenerator CreateCompositionGenerator(this Compositor compositor)
    {
        return new CompositionGenerator(compositor);
    }

    /// <summary>
    /// Creates a CompositionGenerator object
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <param name="graphicsDevice">Composition Graphics Device</param>
    /// <returns>ICompositionGenerator</returns>
    public static ICompositionGenerator CreateCompositionGenerator(this Compositor compositor, CompositionGraphicsDevice graphicsDevice)
    {
        return new CompositionGenerator(graphicsDevice);
    }

    /// <summary>
    /// Creates a CompositionGenerator object
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <param name="useSharedCanvasDevice">Whether to use a shared CanvasDevice or to create a new one.</param>
    /// <param name="useSoftwareRenderer">Whether to use Software Renderer when creating a new CanvasDevice.</param>
    /// <returns>ICompositionGenerator</returns>
    public static ICompositionGenerator CreateCompositionGenerator(this Compositor compositor,
        bool useSharedCanvasDevice, bool useSoftwareRenderer)
    {
        return new CompositionGenerator(compositor, useSharedCanvasDevice, useSoftwareRenderer);
    }

    #endregion

    #region StartingValue Expression

    /// <summary>
    /// Creates a CompositionExpression expression for 'c =&gt; c.StartingValue' for the given type
    /// </summary>
    /// <typeparam name="T">Type of the CompositionExpression Expression</typeparam>
    /// <param name="compositor">Compositor</param>
    /// <returns>Expression&lt;CompositionExpression&lt;T&gt;&gt;</returns>
    public static Expression<CompositionExpression<T>> CreateStartingValueExpression<T>(this Compositor compositor)
    {
        Expression<CompositionExpression<T>> expression = c => c.StartingValue;
        return expression;
    }

    /// <summary>
    /// Creates a CompositionExpression expression for 'c =&gt; c.StartingValue' for the type Color
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>Expression&lt;CompositionExpression&lt;Color&gt;&gt;</returns>
    public static Expression<CompositionExpression<Color>> CreateColorStartingValueExpression(this Compositor compositor)
    {
        return compositor.CreateStartingValueExpression<Color>();
    }

    /// <summary>
    /// Creates a CompositionExpression expression for 'c =&gt; c.StartingValue' for the type Quaternion
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>Expression&lt;CompositionExpression&lt;Quaternion&gt;&gt;</returns>
    public static Expression<CompositionExpression<Quaternion>> CreateQuaternionStartingValueExpression(this Compositor compositor)
    {
        return compositor.CreateStartingValueExpression<Quaternion>();
    }

    /// <summary>
    /// Creates a CompositionExpression expression for 'c =&gt; c.StartingValue' for the type Scalar
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>Expression&lt;CompositionExpression&lt;Scalar&gt;&gt;</returns>
    public static Expression<CompositionExpression<float>> CreateScalarStartingValueExpression(this Compositor compositor)
    {
        return compositor.CreateStartingValueExpression<float>();
    }

    /// <summary>
    /// Creates a CompositionExpression expression for 'c =&gt; c.StartingValue' for the type Vector2
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>Expression&lt;CompositionExpression&lt;Vector2&gt;&gt;</returns>
    public static Expression<CompositionExpression<Vector2>> CreateVector2StartingValueExpression(this Compositor compositor)
    {
        return compositor.CreateStartingValueExpression<Vector2>();
    }

    /// <summary>
    /// Creates a CompositionExpression expression for 'c =&gt; c.StartingValue' for the type Vector3
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>Expression&lt;CompositionExpression&lt;Vector3&gt;&gt;</returns>
    public static Expression<CompositionExpression<Vector3>> CreateVector3StartingValueExpression(this Compositor compositor)
    {
        return compositor.CreateStartingValueExpression<Vector3>();
    }

    /// <summary>
    /// Creates a CompositionExpression expression for 'c =&gt; c.StartingValue' for the type Vector4
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>Expression&lt;CompositionExpression&lt;Vector4&gt;&gt;</returns>
    public static Expression<CompositionExpression<Vector4>> CreateVector4StartingValueExpression(this Compositor compositor)
    {
        return compositor.CreateStartingValueExpression<Vector4>();
    }

    #endregion

    #region FinalValue Expression

    /// <summary>
    /// Creates a CompositionExpression expression for 'c =&gt; c.FinalValue' for the given type
    /// </summary>
    /// <typeparam name="T">Type of the CompositionExpression Expression</typeparam>
    /// <param name="compositor">Compositor</param>
    /// <returns>Expression&lt;CompositionExpression&lt;T&gt;&gt;</returns>
    public static Expression<CompositionExpression<T>> CreateFinalValueExpression<T>(this Compositor compositor)
    {
        Expression<CompositionExpression<T>> expression = c => c.FinalValue;
        return expression;
    }

    /// <summary>
    /// Creates a CompositionExpression expression for 'c =&gt; c.FinalValue' for the type Color
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>Expression&lt;CompositionExpression&lt;Color&gt;&gt;</returns>
    public static Expression<CompositionExpression<Color>> CreateColorFinalValueExpression(this Compositor compositor)
    {
        return compositor.CreateFinalValueExpression<Color>();
    }

    /// <summary>
    /// Creates a CompositionExpression expression for 'c =&gt; c.FinalValue' for the type Quaternion
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>Expression&lt;CompositionExpression&lt;Quaternion&gt;&gt;</returns>
    public static Expression<CompositionExpression<Quaternion>> CreateQuaternionFinalValueExpression(this Compositor compositor)
    {
        return compositor.CreateFinalValueExpression<Quaternion>();
    }

    /// <summary>
    /// Creates a CompositionExpression expression for 'c =&gt; c.FinalValue' for the type Scalar
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>Expression&lt;CompositionExpression&lt;Scalar&gt;&gt;</returns>
    public static Expression<CompositionExpression<float>> CreateScalarFinalValueExpression(this Compositor compositor)
    {
        return compositor.CreateFinalValueExpression<float>();
    }

    /// <summary>
    /// Creates a CompositionExpression expression for 'c =&gt; c.FinalValue' for the type Vector2
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>Expression&lt;CompositionExpression&lt;Vector2&gt;&gt;</returns>
    public static Expression<CompositionExpression<Vector2>> CreateVector2FinalValueExpression(this Compositor compositor)
    {
        return compositor.CreateFinalValueExpression<Vector2>();
    }

    /// <summary>
    /// Creates a CompositionExpression expression for 'c =&gt; c.FinalValue' for the type Vector3
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>Expression&lt;CompositionExpression&lt;Vector3&gt;&gt;</returns>
    public static Expression<CompositionExpression<Vector3>> CreateVector3FinalValueExpression(this Compositor compositor)
    {
        return compositor.CreateFinalValueExpression<Vector3>();
    }

    /// <summary>
    /// Creates a CompositionExpression expression for 'c =&gt; c.FinalValue' for the type Vector4
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>Expression&lt;CompositionExpression&lt;Vector4&gt;&gt;</returns>
    public static Expression<CompositionExpression<Vector4>> CreateVector4FinalValueExpression(this Compositor compositor)
    {
        return compositor.CreateFinalValueExpression<Vector4>();
    }

    #endregion

    #region KeyFrame Animation

    /// <summary>
    /// Creates an instance of KeyFrameAnimation&lt;T&gt; which animates a property of type T.
    /// </summary>
    /// <typeparam name="T">Type of property to be animated</typeparam>
    /// <param name="compositor">Compositor</param>
    /// <returns>KeyFrameAnimation&lt;T&gt;</returns>
    internal static KeyFrameAnimation<T> CreateKeyFrameAnimation<T>(this Compositor compositor)
    {
        return new KeyFrameAnimation<T>(KeyFrameAnimationHelper.CreateAnimation<T>(compositor));
    }

    /// <summary>
    /// Creates an instance of KeyFrameAnimation&lt;Color&gt; which animates a property of type
    /// Color.
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>KeyFrameAnimation&lt;Color&gt;</returns>
    public static KeyFrameAnimation<Color> GenerateColorKeyFrameAnimation(this Compositor compositor)
    {
        return compositor.CreateKeyFrameAnimation<Color>();
    }

    /// <summary>
    /// Creates an instance of KeyFrameAnimation&lt;Quaternion&gt; which animates a property of type
    /// Quaternion.
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>KeyFrameAnimation&lt;Quaternion&gt;</returns>
    public static KeyFrameAnimation<Quaternion> GenerateQuaternionKeyFrameAnimation(this Compositor compositor)
    {
        return compositor.CreateKeyFrameAnimation<Quaternion>();
    }

    /// <summary>
    /// Creates an instance of KeyFrameAnimation&lt;float&gt; which animates a property of type
    /// float.
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>KeyFrameAnimation&lt;float&gt;</returns>
    public static KeyFrameAnimation<float> GenerateScalarKeyFrameAnimation(this Compositor compositor)
    {
        return compositor.CreateKeyFrameAnimation<float>();
    }

    /// <summary>
    /// Creates an instance of KeyFrameAnimation&lt;Vector2&gt; which animates a property of type
    /// Vector2.
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>KeyFrameAnimation&lt;Vector2&gt;</returns>
    public static KeyFrameAnimation<Vector2> GenerateVector2KeyFrameAnimation(this Compositor compositor)
    {
        return compositor.CreateKeyFrameAnimation<Vector2>();
    }

    /// <summary>
    /// Creates an instance of KeyFrameAnimation&lt;Vector3&gt; which animates a property of type
    /// Vector3.
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>KeyFrameAnimation&lt;Vector3&gt;</returns>
    public static KeyFrameAnimation<Vector3> GenerateVector3KeyFrameAnimation(this Compositor compositor)
    {
        return compositor.CreateKeyFrameAnimation<Vector3>();
    }

    /// <summary>
    /// Creates an instance of KeyFrameAnimation&lt;Vector4&gt; which animates a property of type
    /// Vector4.
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>KeyFrameAnimation&lt;Vector4&gt;</returns>
    public static KeyFrameAnimation<Vector4> GenerateVector4KeyFrameAnimation(this Compositor compositor)
    {
        return compositor.CreateKeyFrameAnimation<Vector4>();
    }

    /// <summary>
    /// Creates an instance of KeyFrameAnimation&lt;CompositionPath&gt; which animates a CompositionPath.
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>KeyFrameAnimation&lt;CompositionPath&gt;</returns>
    public static KeyFrameAnimation<CompositionPath> GeneratePathKeyFrameAnimation(this Compositor compositor)
    {
        return compositor.CreateKeyFrameAnimation<CompositionPath>();
    }

    #endregion

    #region Expression Animation

    /// <summary>
    /// Creates an instance of ExpressionAnimation&lt;T&gt; which animates a property of type T.
    /// </summary>
    /// <typeparam name="T">Type of property to be animated</typeparam>
    /// <param name="compositor">Compositor</param>
    /// <returns>ExpressionAnimation&lt;T&gt;</returns>
    internal static ExpressionAnimation<T> CreateExpressionAnimation<T>(this Compositor compositor)
    {
        return new ExpressionAnimation<T>(compositor);
    }

    /// <summary>
    /// Creates an ExpressionAnimation&lt;T&gt; which animates a property of type T
    /// based on the given CompositionExpression Expression
    /// </summary>
    /// <typeparam name="T">Type of the property being animated</typeparam>
    /// <param name="compositor">Compositor</param>
    /// <param name="expression">CompositionExpression Expression</param>
    /// <returns>ExpressionAnimation&lt;T&gt;</returns>
    internal static ExpressionAnimation<T> CreateExpressionAnimation<T>(this Compositor compositor,
        Expression<CompositionExpression<T>> expression)
    {
        var animation = compositor.CreateExpressionAnimation<T>();
        animation.Expression = expression;

        return animation;
    }

    /// <summary>
    /// Creates an instance of ExpressionAnimation&lt;Color&gt; which animates a property of type Color.
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>ExpressionAnimation&lt;Color&gt;</returns>
    public static ExpressionAnimation<Color> CreateColorExpressionAnimation(this Compositor compositor)
    {
        return compositor.CreateExpressionAnimation<Color>();
    }

    /// <summary>
    /// Creates an instance of ExpressionAnimation&lt;Color&gt; which animates a property of type Color
    /// based on the given CompositionExpression.
    /// </summary>
    /// <param name="compositor"></param>
    /// <param name="expression"></param>
    /// <returns>ExpressionAnimation&lt;Color&gt;</returns>
    public static ExpressionAnimation<Color> CreateColorExpressionAnimation(this Compositor compositor,
        Expression<CompositionExpression<Color>> expression)
    {
        return compositor.CreateExpressionAnimation(expression);
    }

    /// <summary>
    /// Creates an instance of ExpressionAnimation&lt;Quaternion&gt; which animates a property of type Quaternion.
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>ExpressionAnimation&lt;Quaternion&gt;</returns>
    public static ExpressionAnimation<Quaternion> CreateQuaternionExpressionAnimation(this Compositor compositor)
    {
        return compositor.CreateExpressionAnimation<Quaternion>();
    }

    /// <summary>
    /// Creates an instance of ExpressionAnimation&lt;Quaternion&gt; which animates a property of type Quaternion
    /// based on the given CompositionExpression.
    /// </summary>
    /// <param name="compositor"></param>
    /// <param name="expression"></param>
    /// <returns>ExpressionAnimation&lt;Quaternion&gt;</returns>
    public static ExpressionAnimation<Quaternion> CreateQuaternionExpressionAnimation(this Compositor compositor,
        Expression<CompositionExpression<Quaternion>> expression)
    {
        return compositor.CreateExpressionAnimation(expression);
    }

    /// <summary>
    /// Creates an instance of ExpressionAnimation&lt;float&gt; which animates a property of type float.
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>ExpressionAnimation&lt;float&gt;</returns>
    public static ExpressionAnimation<float> CreateScalarExpressionAnimation(this Compositor compositor)
    {
        return compositor.CreateExpressionAnimation<float>();
    }

    /// <summary>
    /// Creates an instance of ExpressionAnimation&lt;float&gt; which animates a scalar property
    /// based on the given CompositionExpression.
    /// </summary>
    /// <param name="compositor"></param>
    /// <param name="expression"></param>
    /// <returns>ExpressionAnimation&lt;float&gt;</returns>
    public static ExpressionAnimation<float> CreateScalarExpressionAnimation(this Compositor compositor,
        Expression<CompositionExpression<float>> expression)
    {
        return compositor.CreateExpressionAnimation(expression);
    }

    /// <summary>
    /// Creates an instance of ExpressionAnimation&lt;Vector2&gt; which animates a property of type Vector2.
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>ExpressionAnimation&lt;Vector2&gt;</returns>
    public static ExpressionAnimation<Vector2> CreateVector2ExpressionAnimation(this Compositor compositor)
    {
        return compositor.CreateExpressionAnimation<Vector2>();
    }

    /// <summary>
    /// Creates an instance of ExpressionAnimation&lt;Vector2&gt; which animates a property of type Vector2
    /// based on the given CompositionExpression.
    /// </summary>
    /// <param name="compositor"></param>
    /// <param name="expression"></param>
    /// <returns>ExpressionAnimation&lt;Vector2&gt;</returns>
    public static ExpressionAnimation<Vector2> CreateVector2ExpressionAnimation(this Compositor compositor,
        Expression<CompositionExpression<Vector2>> expression)
    {
        return compositor.CreateExpressionAnimation(expression);
    }

    /// <summary>
    /// Creates an instance of ExpressionAnimation&lt;Vector3&gt; which animates a property of type Vector3.
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>ExpressionAnimation&lt;Vector3&gt;</returns>
    public static ExpressionAnimation<Vector3> CreateVector3ExpressionAnimation(this Compositor compositor)
    {
        return compositor.CreateExpressionAnimation<Vector3>();
    }

    /// <summary>
    /// Creates an instance of ExpressionAnimation&lt;Vector3&gt; which animates a property of type Vector3
    /// based on the given CompositionExpression.
    /// </summary>
    /// <param name="compositor"></param>
    /// <param name="expression"></param>
    /// <returns>ExpressionAnimation&lt;Vector3&gt;</returns>
    public static ExpressionAnimation<Vector3> CreateVector3ExpressionAnimation(this Compositor compositor,
        Expression<CompositionExpression<Vector3>> expression)
    {
        return compositor.CreateExpressionAnimation(expression);
    }

    /// <summary>
    /// Creates an instance of ExpressionAnimation&lt;Vector4&gt; which animates a property of type Vector4.
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>ExpressionAnimation&lt;Vector4&gt;</returns>
    public static ExpressionAnimation<Vector4> CreateVector4ExpressionAnimation(this Compositor compositor)
    {
        return compositor.CreateExpressionAnimation<Vector4>();
    }

    /// <summary>
    /// Creates an instance of ExpressionAnimation&lt;Vector4&gt; which animates a property of type Vector4
    /// based on the given CompositionExpression.
    /// </summary>
    /// <param name="compositor"></param>
    /// <param name="expression"></param>
    /// <returns>ExpressionAnimation&lt;Vector4&gt;</returns>
    public static ExpressionAnimation<Vector4> CreateVector4ExpressionAnimation(this Compositor compositor,
        Expression<CompositionExpression<Vector4>> expression)
    {
        return compositor.CreateExpressionAnimation(expression);
    }

    /// <summary>
    /// Creates an instance of ExpressionAnimation&lt;Matrix4x4&gt; which animates a property of type Matrix4x4.
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>ExpressionAnimation&lt;Matrix4x4&gt;</returns>
    public static ExpressionAnimation<Matrix4x4> CreateMatrix4x4ExpressionAnimation(this Compositor compositor)
    {
        return compositor.CreateExpressionAnimation<Matrix4x4>();
    }

    /// <summary>
    /// Creates an instance of ExpressionAnimation&lt;Matrix4x4&gt; which animates a property of type Matrix4x4
    /// based on the given CompositionExpression.
    /// </summary>
    /// <param name="compositor"></param>
    /// <param name="expression"></param>
    /// <returns>ExpressionAnimation&lt;Matrix4x4&gt;</returns>
    public static ExpressionAnimation<Matrix4x4> CreateMatrix4x4ExpressionAnimation(this Compositor compositor,
        Expression<CompositionExpression<Matrix4x4>> expression)
    {
        return compositor.CreateExpressionAnimation(expression);
    }

    #endregion

    #region CompositionEffectFactory

    /// <summary>
    /// Creates an instance of CompositionEffectFactory.
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <param name="graphicsEffect">The type of effect to create.</param>
    /// <param name="animatablePropertyExpressions">List of Expression each specifying 
    /// an animatable property</param>
    /// <returns>The created CompositionEffectFactory object.</returns>
    public static CompositionEffectFactory CreateEffectFactory(this Compositor compositor,
        IGraphicsEffect graphicsEffect, params Expression<Func<object>>[] animatablePropertyExpressions)
    {
        var animatableProperties = animatablePropertyExpressions.Select(CompositionExpressionEngine.ParseExpression).ToArray();

        return compositor.CreateEffectFactory(graphicsEffect, animatableProperties);
    }

    #endregion

    #region Scoped Batch

    /// <summary>
    /// This extension method creates a scoped batch and handles the completed event
    /// the subscribing and unsubscribing process internally.
    /// 
    /// Example usage:
    /// _compositor.CreateScopedBatch(CompositionBatchTypes.Animation,
    ///        () => // Action
    ///        {
    ///            transitionVisual.StartAnimation("Scale.XY", _scaleUpAnimation);
    ///        },
    ///        () => // Post Action
    ///        {
    ///            BackBtn.IsEnabled = true;
    ///        });
    /// 
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <param name="batchType">Composition Batch Type</param>
    /// <param name="action">Action to perform within the scoped batch</param>
    /// <param name="postAction">Action to perform once the batch completes</param>
    public static void CreateScopedBatch(this Compositor compositor, CompositionBatchTypes batchType,
        Action action, Action postAction = null)
    {
        if (action == null)
            throw new ArgumentException("Cannot create a scoped batch on an action with null value!", nameof(action));

        // Create ScopedBatch
        var batch = compositor.CreateScopedBatch(batchType);

        // Handler for the Completed Event
        void BatchCompletedHandler(object s, CompositionBatchCompletedEventArgs ea)
        {
            var scopedBatch = s as CompositionScopedBatch;

            // Unsubscribe the handler from the Completed event
            if (scopedBatch != null)
            {
                scopedBatch.Completed -= BatchCompletedHandler;
            }

            try
            {
                // Invoke the post action
                postAction?.Invoke();
            }
            finally
            {
                scopedBatch?.Dispose();
            }
        }

        // Subscribe to the Completed event
        batch.Completed += BatchCompletedHandler;

        // Invoke the action
        action();

        // End Batch
        batch.End();
    }

    /// <summary>
    /// This extension method creates a scoped batch and handles the completed event
    /// the subscribing and unsubscribing process internally.
    /// 
    /// Example usage:
    /// _compositor.CreateScopedBatch(CompositionBatchTypes.Animation,
    ///        (batch) => // Action
    ///        {
    ///            transitionVisual.StartAnimation("Scale.XY", _scaleUpAnimation);
    ///        },
    ///        (batch) => // Post Action
    ///        {
    ///            BackBtn.IsEnabled = true;
    ///        });
    /// 
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <param name="batchType">Composition Batch Type</param>
    /// <param name="action">Action to perform within the scoped batch</param>
    /// <param name="postAction">Action to perform once the batch completes</param>
    public static void CreateScopedBatch(this Compositor compositor, CompositionBatchTypes batchType,
        Action<CompositionScopedBatch> action, Action<CompositionScopedBatch> postAction = null)
    {
        if (action == null)
            throw new ArgumentException("Cannot create a scoped batch on an action with null value!", nameof(action));

        // Create ScopedBatch
        var batch = compositor.CreateScopedBatch(batchType);

        // Handler for the Completed Event
        void BatchCompletedHandler(object s, CompositionBatchCompletedEventArgs ea)
        {
            var scopedBatch = s as CompositionScopedBatch;

            // Unsubscribe the handler from the Completed event
            if (scopedBatch != null)
            {
                scopedBatch.Completed -= BatchCompletedHandler;
            }

            try
            {
                // Invoke the post action
                postAction?.Invoke(scopedBatch);
            }
            finally
            {
                scopedBatch?.Dispose();
            }
        }

        // Subscribe to the Completed event
        batch.Completed += BatchCompletedHandler;

        // Invoke the action
        action(batch);

        // End Batch
        batch.End();
    }

    #endregion

    #region Cubic Bezier Easing Functions

    /// <summary>
    /// Back Easing Function - Ease In
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseInBackEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.600f, -0.280f), new Vector2(0.735f, 0.045f));
    }

    /// <summary>
    /// Circle Easing Function - Ease In
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseInCircleEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.600f, 0.040f), new Vector2(0.980f, 0.335f));
    }

    /// <summary>
    /// Cubic Easing Function - Ease In
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseInCubicEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.550f, 0.055f), new Vector2(0.675f, 0.190f));
    }

    /// <summary>
    /// Exponential Easing Function - Ease In
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseInExponentialEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.950f, 0.050f), new Vector2(0.795f, 0.035f));
    }

    /// <summary>
    /// Quadratic Easing Function - Ease In
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseInQuadraticEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.550f, 0.085f), new Vector2(0.680f, 0.530f));
    }

    /// <summary>
    /// Quartic Easing Function - Ease In
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseInQuarticEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.895f, 0.030f), new Vector2(0.685f, 0.220f));
    }

    /// <summary>
    /// Quintic Easing Function - Ease In
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseInQuinticEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.755f, 0.050f), new Vector2(0.855f, 0.060f));
    }

    /// <summary>
    /// Sine Easing Function - Ease In
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseInSineEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.470f, 0.000f), new Vector2(0.745f, 0.715f));
    }

    /// <summary>
    /// Back Easing Function - Ease Out
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseOutBackEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.175f, 0.885f), new Vector2(0.320f, 1.275f));
    }

    /// <summary>
    /// Circle Easing Function - Ease Out
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseOutCircleEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.075f, 0.820f), new Vector2(0.165f, 1.000f));
    }

    /// <summary>
    /// Cubic Easing Function - Ease Out
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseOutCubicEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.215f, 0.610f), new Vector2(0.355f, 1.000f));
    }

    /// <summary>
    /// Exponential Easing Function - Ease Out
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseOutExponentialEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.190f, 1.000f), new Vector2(0.220f, 1.000f));
    }

    /// <summary>
    /// Quadratic Easing Function - Ease Out
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseOutQuadraticEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.250f, 0.460f), new Vector2(0.450f, 0.940f));
    }

    /// <summary>
    /// Quartic Easing Function - Ease Out
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseOutQuarticEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.165f, 0.840f), new Vector2(0.440f, 1.000f));
    }

    /// <summary>
    /// Quintic Easing Function - Ease Out
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseOutQuinticEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.230f, 1.000f), new Vector2(0.320f, 1.000f));
    }

    /// <summary>
    /// Sine Easing Function - Ease Out
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseOutSineEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.390f, 0.575f), new Vector2(0.565f, 1.000f));
    }

    /// <summary>
    /// Back Easing Function - Ease In Out
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseInOutBackEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.680f, -0.550f), new Vector2(0.265f, 1.550f));
    }

    /// <summary>
    /// Circle Easing Function - Ease In Out
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseInOutCircleEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.785f, 0.135f), new Vector2(0.150f, 0.860f));
    }

    /// <summary>
    /// Cubic Easing Function - Ease In Out
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseInOutCubicEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.645f, 0.045f), new Vector2(0.355f, 1.000f));
    }

    /// <summary>
    /// Exponential Easing Function - Ease In Out
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseInOutExponentialEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(1.000f, 0.000f), new Vector2(0.000f, 1.000f));
    }

    /// <summary>
    /// Quadratic Easing Function - Ease In Out
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseInOutQuadraticEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.455f, 0.030f), new Vector2(0.515f, 0.955f));
    }

    /// <summary>
    /// Quartic Easing Function - Ease In Out
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseInOutQuarticEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.770f, 0.000f), new Vector2(0.175f, 1.000f));
    }

    /// <summary>
    /// Quintic Easing Function - Ease In Out
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseInOutQuinticEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.860f, 0.000f), new Vector2(0.070f, 1.000f));
    }

    /// <summary>
    /// Sine Easing Function - Ease In Out
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>CubicBezierEasingFunction</returns>
    public static CubicBezierEasingFunction CreateEaseInOutSineEasingFunction(this Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.445f, 0.050f), new Vector2(0.550f, 0.950f));
    }

    #endregion

    #region CompositionSurfaceBrush

    /// <summary>
    /// Creates the CompositionSurfaceBrush from the specified render surface.
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <param name="renderSurface">An object deriving from IMaskSurface, IGaussianMaskSurface, IGeometrySurface or IImageSurface</param>
    /// <returns>CompositionSurfaceBrush</returns>
    public static CompositionSurfaceBrush CreateSurfaceBrush(this Compositor compositor, IRenderSurface renderSurface)
    {
        return compositor.CreateSurfaceBrush(renderSurface.Surface);
    }

    #endregion

    #region CompositionGeometry

    /// <summary>
    /// Creates a CompositionPath based on the given path data.
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <param name="pathData">Path data in string format.</param>
    /// <returns>CompositionPath</returns>
    public static CompositionPath CreatePath(this Compositor compositor, string pathData)
    {
        // Create CanvasGeometry
        var geometry = CanvasObject.CreateGeometry(pathData);
        // Create CompositionPath
        return new CompositionPath(geometry);
    }

    /// <summary>
    /// Creates a CompositionPathGeometry based on the given path data.
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <param name="pathData">Path data in string format.</param>
    /// <returns>CompositionPathGeometry</returns>
    public static CompositionPathGeometry CreatePathGeometry(this Compositor compositor, string pathData)
    {
        // Create CanvasGeometry
        var geometry = CanvasObject.CreateGeometry(pathData);
        // Create CompositionPathGeometry
        return compositor.CreatePathGeometry(new CompositionPath(geometry));
    }

    /// <summary>
    /// Creates a CompositionSpriteShape based on the given path data.
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <param name="pathData">Path data in string format.</param>
    /// <returns>CompositionSpriteShape</returns>
    public static CompositionSpriteShape CreateSpriteShape(this Compositor compositor, string pathData)
    {
        // Create CanvasGeometry
        var geometry = CanvasObject.CreateGeometry(pathData);
        // Create CompositionPathGeometry
        var pathGeometry = compositor.CreatePathGeometry(new CompositionPath(geometry));
        // Create CompositionSpriteShape
        return compositor.CreateSpriteShape(pathGeometry);
    }

    /// <summary>
    /// Creates a CompositionGeometricClip from the given CanvasGeometry
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <param name="geometry">CanvasGeometry</param>
    /// <returns>CompositionGeometricClip</returns>
    public static CompositionGeometricClip CreateGeometricClip(this Compositor compositor, CanvasGeometry geometry)
    {
        // Create the CompositionPath
        var path = new CompositionPath(geometry);
        // Create the CompositionPathGeometry
        var pathGeometry = compositor.CreatePathGeometry(path);
        // Create the CompositionGeometricClip
        return compositor.CreateGeometricClip(pathGeometry);
    }

    /// <summary>
    /// Parses the given Path data and converts it to CompositionGeometricClip
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <param name="pathData">Path data in string format.</param>
    /// <returns>CompositionGeometricClip</returns>
    public static CompositionGeometricClip CreateGeometricClip(this Compositor compositor, string pathData)
    {
        // Create the CanvasGeometry from the path data
        var geometry = CanvasObject.CreateGeometry(pathData);
        // Create the CompositionGeometricClip
        return compositor.CreateGeometricClip(geometry);
    }

    #endregion
}
