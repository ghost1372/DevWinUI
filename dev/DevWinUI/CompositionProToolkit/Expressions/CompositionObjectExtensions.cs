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

namespace DevWinUI;

/// <summary>
/// Extension methods for the CompositionObject class
/// </summary>
public static partial class CompositionObjectExtensions
{
    /// <summary>
    /// Starts the given animation on the property specified by the given expression.
    /// The expression is converted to the appropriate property string by the
    /// CompositionExpressionEngine 
    /// </summary>
    /// <param name="compositionObject">CompositionObject</param>
    /// <param name="expression">Expression defining the property on which to start the animation</param>
    /// <param name="animation">The animation to execute on the specified property</param>
    public static void StartAnimation(this CompositionObject compositionObject,
        Expression<Func<object>> expression, CompositionAnimation animation)
    {
        compositionObject.StartAnimation(CompositionExpressionEngine.ParseExpression(expression), animation);
    }

    /// <summary>
    /// Starts the given KeyFrameAnimation&lt;&gt; on the property specified by the given expression.
    /// The expression is converted to the appropriate property string by the
    /// CompositionExpressionEngine
    /// </summary>
    /// <typeparam name="T">Type of the property to be animated.</typeparam>
    /// <param name="compositionObject">CompositionObject</param>
    /// <param name="targetProperty">Property on which the animation has to be started.</param>
    /// <param name="keyframeAnimation">The KeyFrameAnimation to run on the specified property.</param>
    public static void StartAnimation<T>(this CompositionObject compositionObject,
        string targetProperty, KeyFrameAnimation<T> keyframeAnimation)
    {
        compositionObject.StartAnimation(targetProperty, keyframeAnimation.Animation);
    }

    /// <summary>
    /// Connects an ExpressionAnimation&lt;T&gt; with the specified property of the object 
    /// and starts the animation.
    /// </summary>
    /// <typeparam name="T">Type of the property to be animated</typeparam>
    /// <param name="compositionObject">The object on whose property the animation has to
    /// be started.</param>
    /// <param name="targetProperty">Property on which the animation has to be started.</param>
    /// <param name="expressionAnimation">The ExpressionAnimation&lt;T&gt; to run on the specified property.</param>
    public static void StartAnimation<T>(this CompositionObject compositionObject,
        string targetProperty, ExpressionAnimation<T> expressionAnimation)
    {
        compositionObject.StartAnimation(targetProperty, expressionAnimation.Animation);
    }

    /// <summary>
    /// Starts the given KeyFrameAnimation&lt;&gt; on the property specified by the given expression.
    /// The expression is converted to the appropriate property string by the
    /// CompositionExpressionEngine
    /// </summary>
    /// <typeparam name="T">Type of the property to be animated.</typeparam>
    /// <param name="compositionObject">CompositionObject</param>
    /// <param name="expression">Lambda expression defining the property on 
    /// which the animation has to be started.</param>
    /// <param name="keyframeAnimation">The KeyFrameAnimation to run on the specified property.</param>
    public static void StartAnimation<T>(this CompositionObject compositionObject,
        Expression<Func<T>> expression, KeyFrameAnimation<T> keyframeAnimation)
    {
        compositionObject.StartAnimation(CompositionExpressionEngine.ParseExpression(expression), keyframeAnimation.Animation);
    }

    /// <summary>
    /// Connects an ExpressionAnimation&lt;T&gt; with the specified property of the object 
    /// and starts the animation.
    /// </summary>
    /// <typeparam name="T">Type of the property to be animated</typeparam>
    /// <param name="compositionObject">The object on whose property the animation has to
    /// be started.</param>
    /// <param name="expression">Lambda expression representing the specified property to be
    /// animated.</param>
    /// <param name="expressionAnimation">The ExpressionAnimation&lt;T&gt; to run on the specified property.</param>
    public static void StartAnimation<T>(this CompositionObject compositionObject,
        Expression<Func<T>> expression, ExpressionAnimation<T> expressionAnimation)
    {
        compositionObject.StartAnimation(CompositionExpressionEngine.ParseExpression(expression), expressionAnimation.Animation);
    }

    /// <summary>
    /// Stops the given animation on the property specified by the given expression.
    /// The expression is converted to the appropriate property string by the
    /// CompositionExpressionEngine 
    /// </summary>
    /// <param name="compositionObject">CompositionObject</param>
    /// <param name="expression">Expression defining the property on which to stop the animation</param>
    public static void StopAnimation(this CompositionObject compositionObject,
        Expression<Func<object>> expression)
    {
        compositionObject.StopAnimation(CompositionExpressionEngine.ParseExpression(expression));
    }

    /// <summary>
    /// This extension method is a dummy method added to specify to start/stop animation on the 
    /// 'Scale.XY' property of the CompositionObject. Though no such property exists in CompositionObject,
    /// it merely indicates that the animation has to be executed (or stopped) on both the 'Scale.X' and 'Scale.Y'
    /// properties of the CompositionObject.
    /// </summary>
    /// <param name="compositionObject">CompositionObject</param>
    /// <returns>Vector2</returns>
    public static Vector2 ScaleXY(this CompositionObject compositionObject)
    {
        return new Vector2();
    }
}
