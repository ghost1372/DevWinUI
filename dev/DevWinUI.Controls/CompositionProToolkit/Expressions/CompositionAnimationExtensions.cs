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
using System.Reflection;

namespace DevWinUI;

/// <summary>
/// Extension methods for Animations deriving from CompositionAnimation
/// </summary>
public static partial class CompositionAnimationExtensions
{
    #region Fields

    private static readonly Dictionary<Type, MethodInfo> SetMethods;

    #endregion

    #region Static Constructor

    /// <summary>
    /// Static Ctor
    /// </summary>
    static CompositionAnimationExtensions()
    {
        // Get all the SetXXXParameter methods
        SetMethods = typeof(CompositionAnimation)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Set") && m.Name.EndsWith("Parameter"))
            .ToDictionary(m => m.GetParameters()[1].ParameterType,
                m => m);
    }

    #endregion

    #region Extensions

    /// <summary>
    /// Sets the Expression property of ExpressionAnimation by converting
    /// the given Expression to appropriate string
    /// </summary>
    /// <typeparam name="T">Type of the Expression</typeparam>
    /// <param name="animation">ExpressionAnimation</param>
    /// <param name="expression">Expression</param>
    /// <returns>Dictionary of parameter names and the parameters</returns>
    public static void SetExpression<T>(this ExpressionAnimation animation,
        Expression<CompositionExpression<T>> expression)
    {
        var result = CompositionExpressionEngine.CreateCompositionExpression(expression);
        animation.Expression = result.Expression;
        animation.SetParameters(result.Parameters);
    }

    /// <summary>
    /// Inserts a KeyFrame in the KeyFrameAnimation by converting
    /// the given Expression to appropriate string
    /// </summary>
    /// <typeparam name="T">Type of the Expression</typeparam>
    /// <param name="animation">KeyFrameAnimation</param>
    /// <param name="normalizedProgressKey">The time the key frame should occur at, expressed as 
    /// a percentage of the animation Duration. Allowed value is from 0.0 to 1.0.</param>
    /// <param name="expression">The expression used to calculate the value of the key frame.</param>
    /// <param name="easingFunction">Easing Function (optional)</param>
    /// <returns>KeyFrameAnimation</returns>
    public static KeyFrameAnimation InsertExpressionKeyFrame<T>(this KeyFrameAnimation animation, float normalizedProgressKey,
        Expression<CompositionExpression<T>> expression, CompositionEasingFunction easingFunction = null)
    {
        var result = CompositionExpressionEngine.CreateCompositionExpression(expression);
        animation.InsertExpressionKeyFrame(normalizedProgressKey, result.Expression, easingFunction);
        animation.SetParameters(result.Parameters);

        return animation;
    }

    /// <summary>
    /// Sets the parameter value in the animation for the given key
    /// </summary>
    /// <typeparam name="T">Type of Animation</typeparam>
    /// <param name="animation">Animation object into which the parameters must be set</param>
    /// <param name="key">Key for the input value</param>
    /// <param name="expressionParameter">Parameter containing Value to set and its type</param>
    /// <returns>True if successful, otherwise False</returns>
    internal static bool SetParameter<T>(this T animation, string key, ExpressionParameter expressionParameter)
        where T : CompositionAnimation
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;

        if (expressionParameter.Reference == null)
        {
            throw new ArgumentException($"No reference value is set for the reference parameter '{key}'.");
        }

        var parameter = expressionParameter.Reference;
        var type = expressionParameter.Type;

        // Can the type be converted to float?
        if (CompositionExpressionEngine.Floatables.Contains(type))
        {
            type = typeof(float);
            parameter = Convert.ToSingle(parameter);
        }

        // Point should be converted to Vector3
        if (type == typeof(Point))
        {
            var point = (Point)parameter;
            parameter = point.ToVector3();
            type = typeof(Vector3);
        }

        // Find matching SetXXXParameter method for the given type or if there
        // is no SetXXXParameter method directly matching the parameter type, then
        // find if the parameter type derives from any of the types which are the 
        // keys in the SetMethods dictionary
        var methodKey = SetMethods.Keys.FirstOrDefault(t => (t == type) || t.IsAssignableFrom(type));

        if (methodKey == null)
            return false;

        // If a matching key is found, invoke the corresponding SetXXXParameter method
        SetMethods[methodKey].Invoke(animation, new[] { key, parameter });
        return true;
    }

    /// <summary>
    /// Sets the parameters obtained from parsing the Expression to the CompositionAnimation
    /// </summary>
    /// <typeparam name="T">Type of Animation</typeparam>
    /// <param name="animation">Animation object into which the parameters must be set</param>
    /// <param name="parameters">Parameters to set</param>
    /// <returns>Animation</returns>
    internal static T SetParameters<T>(this T animation, Dictionary<string, ExpressionParameter> parameters)
        where T : CompositionAnimation
    {
        var newParameters = new Dictionary<string, ExpressionParameter>();

        // Try to set the parameter for each of the keys
        foreach (var key in from key in parameters.Keys
                            let parameter = parameters[key]
                            where (!animation.SetParameter(key, parameter))
                            select key)
        {
            // If SetParameter fails, then convert the parameter into a CompositionPropertySet
            // Since we cannot modify the parameters dictionary while we are inside the loop, add the key and the
            // CompositionPropertySet to the newParameters dictionary, so that the parameters dictionary can be updated 
            // once the loop completes
            newParameters[key] = new ExpressionParameter(CompositionPropertySetExtensions.ToPropertySet(parameters[key].Reference,
                    animation.Compositor),
                typeof(CompositionPropertySet));
        }

        // If any key value pairs exist in the newParameters dictionary, then update the parameters dictionary
        if (newParameters.Any())
        {
            foreach (var item in newParameters)
            {
                parameters[item.Key] = item.Value;
                // Set item.Value as the Reference Parameter for the animation
                animation.SetReferenceParameter(item.Key, (CompositionObject)item.Value.Reference);
            }
            // Clean up newParameters dictionary
            newParameters.Clear();
        }

        return animation;
    }

    #endregion
}
