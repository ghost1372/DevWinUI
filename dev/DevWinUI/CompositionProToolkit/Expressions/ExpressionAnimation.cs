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
using System.Text;

namespace DevWinUI;

/// <summary>
/// Generic class representing an ExpressionAnimation which animates a 
/// property (of type T). The animation is governed by the specified
/// mathematical expression.
/// </summary>
/// <typeparam name="T">Type of the property being animated</typeparam>
public partial class ExpressionAnimation<T>
{
    #region Fields

    private readonly Compositor _compositor;
    private bool _anyReferenceUpdated;

    private Expression<CompositionExpression<T>> _expression;
    private ExpressionAnimation _animation;

    private string _expressionString;
    private Dictionary<string, ExpressionParameter> _references;

    #endregion

    #region Properties

    /// <summary>
    /// The CompositionExpression associated with this ExpressionAnimation
    /// </summary>
    public Expression<CompositionExpression<T>> Expression
    {
        get => _expression;
        set => UpdateExpression(value);
    }

    /// <summary>
    /// The ExpressionAnimation obtained from the CompositionExpression
    /// </summary>
    public ExpressionAnimation Animation => CreateAnimation();

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="compositor">Compositor</param>
    internal ExpressionAnimation(Compositor compositor)
    {
        _compositor = compositor;
        _references = new Dictionary<string, ExpressionParameter>();
    }

    #endregion

    #region APIs

    /// <summary>
    /// Sets the object reference in the expression for the given
    /// reference name.
    /// </summary>
    /// <param name="referenceName">Name of the Reference in the expression.</param>
    /// <param name="referenceValue">Reference object</param>
    public void SetReference(string referenceName, object referenceValue)
    {
        // Check if any reference is defined in the expression with the 
        // given name
        if (_references.ContainsKey(referenceName))
        {
            var baseReferenceType = _references[referenceName].Type;
            var referenceType = referenceValue.GetType();

            if (baseReferenceType.IsAssignableFrom(referenceType))
            {
                _references[referenceName] = new ExpressionParameter(referenceValue, baseReferenceType);
                // Since the reference has been updated, new Animation should be created when the 
                // Animation property is referenced.
                _anyReferenceUpdated = true;
            }
            else
            {
                // Type mismatch
                throw new ArgumentException($"Cannot set a reference of type '{referenceType}' to " +
                                            $"'{referenceName}', which is of type '{baseReferenceType}'.");
            }
        }
        else
        {
            // No reference defined with the given name
            throw new ArgumentException($"The reference name '{referenceName}' is not defined in this Expression.");
        }
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Parses the given CompositionExpression and generates the reference dictionary
    /// and the expression string
    /// </summary>
    /// <param name="expression">Expression&lt;CompositionExpression&lt;T&gt;&gt;</param>
    private void UpdateExpression(Expression<CompositionExpression<T>> expression)
    {
        // Clear previous references if any
        _references.Clear();
        _expression = expression;
        var result = CompositionExpressionEngine.CreateCompositionExpression(_expression);
        _expressionString = result.Expression;
        _references = new Dictionary<string, ExpressionParameter>(result.Parameters);
        _anyReferenceUpdated = true;
    }

    /// <summary>
    /// Creates an ExpressionAnimation from the data obtained by 
    /// parsing the CompositionExpression.
    /// </summary>
    /// <returns>ExpressionAnimation</returns>
    private ExpressionAnimation CreateAnimation()
    {
        // If animation already exists and no references have
        // been updated then return the existing animation.
        if ((_animation != null) && (!_anyReferenceUpdated))
            return _animation;

        _anyReferenceUpdated = false;
        // Validate all the references used in the animation expression
        ValidateReferences();
        // Create a new expression animation and set its parameters
        _animation = _compositor.CreateExpressionAnimation(_expressionString);
        _animation.SetParameters(_references);

        return _animation;
    }

    /// <summary>
    /// Validates all the references used in the animation expression.
    /// </summary>
    private void ValidateReferences()
    {
        if ((_references == null) || (!_references.Any()))
            return;

        // Find if any of the references are not set yet!
        var nullReferences = _references.Where(r => r.Value.Reference == null).ToList();
        // All references are valid?
        if (!nullReferences.Any())
            return;

        // Some references are not properly set. Exception must be raised.
        // Create the message for the exception.
        var sb = new StringBuilder();
        sb.AppendLine("Reference to the following reference name(s) have not been set in the animation:");
        foreach (var item in nullReferences)
        {
            sb.AppendLine($"\t'{item.Key}'");
        }
        sb.AppendLine();
        sb.AppendLine("Please call the following methods on the ExpressionAnimation to ensure your references are set:");
        sb.AppendLine();
        foreach (var item in nullReferences)
        {
            sb.AppendLine($"// Set reference for '{item.Key}' with a value of type '{item.Value.Type.Name}'");
            sb.AppendLine($"SetReference(\"{item.Key}\", ...);");
            sb.AppendLine();
        }

        throw new ArgumentException(sb.ToString());
    }

    #endregion
}
