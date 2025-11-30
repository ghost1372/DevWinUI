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

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.UI.Composition.Interactions;
using Windows.Graphics.Effects;

namespace DevWinUI;

/// <summary>
/// Converts an Expression to a string that can be used as an input
/// for ExpressionAnimation and KeyFrameAnimation
/// </summary>
internal abstract partial class CompositionExpressionEngine
{
    #region Fields

    private static readonly Dictionary<ExpressionType, string> BinaryExpressionStrings;
    private static readonly IEnumerable<Type> ExpressionTypes;
    private static readonly Dictionary<Type, MethodInfo> VisitMethods;
    private static readonly Dictionary<Type, MethodInfo> ParseMethods;
    private static readonly Type[] Newables;
    private static readonly Dictionary<Type, int> NewablesArgCount;
    private static readonly Type[] ExpressionTargets;
    private static readonly Dictionary<Type, Type> ReferenceTypes;
    private static ExpressionType _previousBinaryOperator;

    private static bool _noQuotesForConstant;
    private static bool _firstBinaryExpression;
    private static bool _firstParseBinaryExpression;
    private static bool _noQuotesForParseConstant;

    private static Dictionary<string, ExpressionParameter> _parameters;

    #endregion

    #region Properties

    public static Type[] Floatables { get; private set; }

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// Static constructor
    /// </summary>
    static CompositionExpressionEngine()
    {
        // ExpressionTypes in BinaryExpressions and their corresponding strings
        BinaryExpressionStrings = new Dictionary<ExpressionType, string>()
        {
            { ExpressionType.Add,                     "+"   },
            { ExpressionType.AddChecked,              "+"   },
            { ExpressionType.And,                     "&"   },
            { ExpressionType.AndAlso,                 "&&"  },
            { ExpressionType.Coalesce,                "??"  },
            { ExpressionType.Divide,                  "/"   },
            { ExpressionType.Equal,                   "=="  },
            { ExpressionType.ExclusiveOr,             "^"   },
            { ExpressionType.GreaterThan,             ">"   },
            { ExpressionType.GreaterThanOrEqual,      ">="  },
            { ExpressionType.LeftShift,               "<<"  },
            { ExpressionType.LessThan,                "<"   },
            { ExpressionType.LessThanOrEqual,         "<="  },
            { ExpressionType.Modulo,                  "%"   },
            { ExpressionType.Multiply,                "*"   },
            { ExpressionType.MultiplyChecked,         "*"   },
            { ExpressionType.NotEqual,                "!="  },
            { ExpressionType.Or,                      "|"   },
            { ExpressionType.OrElse,                  "||"  },
            { ExpressionType.Power,                   "^"   },
            { ExpressionType.RightShift,              ">>"  },
            { ExpressionType.Subtract,                "-"   },
            { ExpressionType.SubtractChecked,         "-"   },
            { ExpressionType.Assign,                  "="   },
            { ExpressionType.AddAssign,               "+="  },
            { ExpressionType.AndAssign,               "&="  },
            { ExpressionType.DivideAssign,            "/="  },
            { ExpressionType.ExclusiveOrAssign,       "^="  },
            { ExpressionType.LeftShiftAssign,         "<<=" },
            { ExpressionType.ModuloAssign,            "%="  },
            { ExpressionType.MultiplyAssign,          "*="  },
            { ExpressionType.OrAssign,                "|="  },
            { ExpressionType.PowerAssign,             "^="  },
            { ExpressionType.RightShiftAssign,        ">>=" },
            { ExpressionType.SubtractAssign,          "-="  },
            { ExpressionType.AddAssignChecked,        "+="  },
            { ExpressionType.MultiplyAssignChecked,   "*="  },
            { ExpressionType.SubtractAssignChecked,   "-="  },
        };

        // List of types that can be converted to float
        Floatables = new[]{
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(char),
            typeof(double),
            typeof(bool),
            typeof(float),
            typeof(decimal)
        };

        // List of types for which the 'new' operator is supported
        Newables = new[]
        {
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Matrix3x2),
            typeof(Matrix4x4),
            typeof(Quaternion)
        };

        // Since some types have more than one constructor, currently
        // only one constructor is supported for each type.
        // NOTE: This may change later on.
        NewablesArgCount = new Dictionary<Type, int>()
        {
            [typeof(Vector2)] = 2,
            [typeof(Vector3)] = 3,
            [typeof(Vector4)] = 4,
            [typeof(Matrix3x2)] = 6,
            [typeof(Matrix4x4)] = 16,
            [typeof(Quaternion)] = 4
        };

        // List of types deriving from ExpressionTemplate
        // and evaluate to 'this.Target'
        ExpressionTargets = new[]
        {
            typeof(AmbientLightTarget),
            typeof(ColorBrushTarget),
            typeof(DistantLightTarget),
            typeof(DropShadowTarget),
            typeof(InsetClipTarget),
            typeof(InteractionTrackerTarget),
            typeof(ManipulationPropertySetTarget),
            typeof(NineGridBrushTarget),
            typeof(PointerPositionPropertySetTarget),
            typeof(PointLightTarget),
            typeof(SpotLightTarget),
            typeof(SurfaceBrushTarget),
            typeof(VisualTarget)
        };

        // Dictionary containing types deriving from ExpressionTemplate,
        // which act as references in the expression, and the corresponding
        // actual types they represent
        ReferenceTypes = new Dictionary<Type, Type>()
        {
            [typeof(AmbientLightReference)] = typeof(AmbientLight),
            [typeof(ColorBrushReference)] = typeof(CompositionColorBrush),
            [typeof(DistantLightReference)] = typeof(DistantLight),
            [typeof(DropShadowReference)] = typeof(DropShadow),
            [typeof(InsetClipReference)] = typeof(InsetClip),
            [typeof(InteractionTrackerReference)] = typeof(InteractionTracker),
            [typeof(ManipulationPropertySetReference)] = typeof(CompositionPropertySet),
            [typeof(NineGridBrushReference)] = typeof(CompositionNineGridBrush),
            [typeof(PointerPositionPropertySetReference)] = typeof(CompositionPropertySet),
            [typeof(PointLightReference)] = typeof(PointLight),
            [typeof(SpotLightReference)] = typeof(SpotLight),
            [typeof(SurfaceBrushReference)] = typeof(CompositionSurfaceBrush),
            [typeof(VisualReference)] = typeof(Visual),
        };

        // Get all the types which derive from Expression or MemberBinding
        ExpressionTypes = typeof(Expression)
            .GetTypeInfo()
            .Assembly
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(Expression))
                        || t.IsSubclassOf(typeof(MemberBinding)));


        // Get all the Visit Methods
        VisitMethods = GetMethods("Visit");
        // Get all the Parse Methods
        ParseMethods = GetMethods("Parse");
    }

    /// <summary>
    /// Gets the methods having the given method name and whose first parameter 
    /// matches one of the types in ExpressionTypes.
    /// </summary>
    /// <param name="methodName">Method name</param>
    /// <returns>Dictionary of Expression Type and MethodInfo</returns>
    private static Dictionary<Type, MethodInfo> GetMethods(string methodName)
    {
        // Get all the private static Visit methods defined in the CompositionExpressionEngine
        var methods = typeof(CompositionExpressionEngine)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(m => m.Name == methodName);

        // Get the list of methods whose first parameter matches one of the types in ExpressionTypes
        return ExpressionTypes.Join(methods,
                t => t,                                  // Selector for Expression Types
                m => m.GetParameters()[0].ParameterType, // Selector for CompositionExpressionEngine Methods
                (t, m) => new { Type = t, Method = m })  // Result Selector
            .ToDictionary(t => t.Type, t => t.Method);     // Convert to Dictionary<Type, MethodInfo>
    }

    #endregion

    #region APIs

    /// <summary>
    /// Converts the given Expression to a string that can be used as an input
    /// for ExpressionAnimation and KeyFrameAnimation
    /// </summary>
    /// <typeparam name="T">Type of the Expression</typeparam>
    /// <param name="expression">Expression</param>
    /// <returns>CompositionExpressionResult</returns>
    [RequiresDynamicCode("Calls DevWinUI.CompositionExpressionEngine.Visit(LambdaExpression)")]
    internal static CompositionExpressionResult CreateCompositionExpression<T>(Expression<CompositionExpression<T>> expression)
    {
        // Reset flags
        _noQuotesForConstant = false;
        _firstBinaryExpression = false;
        _parameters = new Dictionary<string, ExpressionParameter>();
        _previousBinaryOperator = ExpressionType.Add;

        var compositionExpr = new CompositionExpressionResult();
        // Visit the Expression Tree and convert it to string
        var expr = Visit(expression).ToString();
        compositionExpr.Expression = expr;
        // Obtain the parameters involved in the expression
        compositionExpr.Parameters = new Dictionary<string, ExpressionParameter>(_parameters);

        return compositionExpr;
    }

    /// <summary>
    /// Parses the given expression and converts it into an appriopriate string.
    /// This is used in the following scenarios
    /// 1. To specify the animatable property on a CompositionObject derived object
    ///    for the StartAnimation and StopAnimation methods.
    /// 2. To specify an animatable property of type IGraphicEffect or IGraphicEffectSource
    ///    for the CreateEffectFactory method of Compositor.
    /// 3. To specify an animatable property of type IGraphicEffect or IGraphicEffectSource
    ///    for the StartAnimation and StopAnimation methods of a CompositionEffectBrush.
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <returns>String</returns>
    internal static string ParseExpression(Expression<Func<object>> expression)
    {
        // Reset flags
        _noQuotesForParseConstant = false;
        _firstParseBinaryExpression = false;

        var exprString = Parse(expression).ToString();
        return exprString;
    }

    /// <summary>
    /// Parses the given expression and converts it into an appriopriate string.
    /// This is used in the following scenarios
    /// 1. To specify the animatable property on a CompositionObject derived object
    ///    for the StartAnimation and StopAnimation methods.
    /// 2. To specify an animatable property of type IGraphicEffect or IGraphicEffectSource
    ///    for the CreateEffectFactory method of Compositor.
    /// 3. To specify an animatable property of type IGraphicEffect or IGraphicEffectSource
    ///    for the StartAnimation and StopAnimation methods of a CompositionEffectBrush.
    /// </summary>
    /// <typeparam name="T">Type of the property being animated</typeparam>
    /// <param name="expression">Expression</param>
    /// <returns>String</returns>
    internal static string ParseExpression<T>(Expression<Func<T>> expression)
    {
        // Reset flags
        _noQuotesForParseConstant = false;
        _firstParseBinaryExpression = false;

        var exprString = Parse(expression).ToString();
        return exprString;
    }

    #endregion

    #region Visit methods

    /// <summary>
    /// Visits an Expression. This method acts as a router and
    /// based on the specific type of Expression, the appropriate
    /// Visit method is called.
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Visit(Expression expression)
    {
        if (expression == null)
        {
            return new SimpleExpressionToken("null");
        }

        var type = expression.GetType();

        // Find matching Visit method whose first parameter best matches the expression type
        // or if there is no Visit method directly matching the expression type, then
        // find if the expression type derives from any of the types which are the 
        // keys in the VisitMethods dictionary
        var methodKey = VisitMethods.Keys.FirstOrDefault(t => (t == type) || t.IsAssignableFrom(type));

        if (methodKey == null)
            return new SimpleExpressionToken(string.Empty);

        // Once a matching Visit method is found, Invoke it!
        return (ExpressionToken)VisitMethods[methodKey].Invoke(null, new object[] { expression });
    }

    /// <summary>
    /// Visits a BinaryExpression
    /// </summary>
    /// <param name="expression">BinaryExpression</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Visit(BinaryExpression expression)
    {
        // This check is done to avoid adding redundant parenthesis to binary expressions
        var prevOpCheck =
            (_previousBinaryOperator == ExpressionType.Add && expression.NodeType == ExpressionType.Add) ||
            (_previousBinaryOperator == ExpressionType.Multiply && expression.NodeType == ExpressionType.Multiply);

        // Is this an Array Index expression?
        if (expression.NodeType == ExpressionType.ArrayIndex)
        {
            return VisitArrayExpression(expression);
        }

        // Check if it is the outermost BinaryExpression
        // If yes, then no need to add round brackets to 
        // the whole visited expression
        var noBrackets = _firstBinaryExpression || prevOpCheck;
        if (_firstBinaryExpression)
        {
            // Set it to false so that the internal BinaryExpression(s)
            // will have round brackets
            _firstBinaryExpression = false;
        }

        _previousBinaryOperator = expression.NodeType;

        var leftToken = Visit(expression.Left);
        var rightToken = Visit(expression.Right);

        if (!BinaryExpressionStrings.TryGetValue(expression.NodeType, out string symbol))
            return new SimpleExpressionToken("");

        // This check is done to avoid wrapping the final ExpressionToken 
        // in Round Brackets if the outermost expression is a BinaryExpression
        var bracketType = noBrackets ? BracketType.None : BracketType.Round;

        var token = new CompositeExpressionToken(bracketType);
        token.AddToken($"{leftToken} {symbol} {rightToken}");

        return token;
    }

    /// <summary>
    /// Visits a ConditionalExpression
    /// </summary>
    /// <param name="expression">ConditionalExpression</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Visit(ConditionalExpression expression)
    {
        var token = new CompositeExpressionToken(BracketType.Round);
        token.AddToken($"{Visit(expression.Test)} ? {Visit(expression.IfTrue)} : {Visit(expression.IfFalse)}");
        return token;
    }

    /// <summary>
    /// Visits a ConstantExpression
    /// </summary>
    /// <param name="expression">ConstantExpression</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Visit(ConstantExpression expression)
    {
        if (expression.Value == null)
            return new SimpleExpressionToken("null");

        if (expression.Value.GetType().IsNested
            && expression.Value.GetType().Name.StartsWith("<", StringComparison.Ordinal))
            return new SimpleExpressionToken(string.Empty);

        var str = expression.Value as string;
        if (str != null)
            return new CompositeExpressionToken(str, _noQuotesForConstant ? BracketType.None : BracketType.Quotes);

        return new SimpleExpressionToken(expression.Value.ToString());
    }

    /// <summary>
    /// Visits a InvocationExpression
    /// </summary>
    /// <param name="expression">InvocationExpression</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Visit(InvocationExpression expression)
    {
        var token = new CompositeExpressionToken();
        token.AddToken("Invoke");
        // Visit each of the arguments
        token.AddToken(new CompositeExpressionToken(expression.Arguments.Select(Visit), BracketType.Round, true));
        return token;
    }

    /// <summary>
    /// Visits a LambdaExpression
    /// </summary>
    /// <param name="expression">LambdaExpression</param>
    /// <returns>ExpressionToken</returns>
    [RequiresDynamicCode("Calls DevWinUI.CompositionExpressionEngine.IsGenericCompositionExpressionContextType(Type)")]
    private static ExpressionToken Visit(LambdaExpression expression)
    {
        var token = new CompositeExpressionToken();

        // No need to print the parameter of type CompositionExpressionContext<T>
        if (!IsGenericCompositionExpressionContextType(expression.Parameters[0].Type))
        {
            // Parameter(s)
            var paramStr = string.Join(", ", expression.Parameters.Select(p => CleanIdentifier(p.Name)).ToArray());
            var bracketType = (expression.Parameters.Count == 1) ? BracketType.None : BracketType.Round;
            token.AddToken(new CompositeExpressionToken(paramStr, bracketType));

            // Arrow
            token.AddToken(" => ");
        }
        // If the parameter is of type CompositionExpressionContext<T> then it means 
        // that this is a CompositionExpression expression (i.e. First specific Visit). 
        // If the outermost Expression in the body of the CompositionExpression expression
        // is a BinaryExpression, then no need to add round brackets
        else if ((expression.Body as BinaryExpression) != null)
        {
            _firstBinaryExpression = true;
        }

        // Expression Body
        var bodyToken = Visit(expression.Body);
        if (bodyToken != null)
        {
            token.AddToken(bodyToken);
        }

        return token;
    }

    /// <summary>
    /// Visits a MemberExpression
    /// </summary>
    /// <param name="expression">MemberExpression</param>
    /// <returns>ExpressionToken</returns>
    [RequiresDynamicCode("Calls DevWinUI.CompositionExpressionEngine.IsGenericCompositionExpressionContextType(Type)")]
    private static ExpressionToken Visit(MemberExpression expression)
    {
        // Check if this expression is accessing the StartingValue or FinalValue
        // Property of CompositionExpressionContext<T>
        if (((expression.Member as PropertyInfo) != null) &&
            (expression.Expression != null) &&
            IsGenericCompositionExpressionContextType(expression.Expression.Type))
        {
            return new SimpleExpressionToken($"this.{expression.Member.Name}");
        }

        // This is a check for types deriving from ExpressionTemplate. Replace the class
        // with their Name property.
        if (typeof(ExpressionTemplate).IsAssignableFrom(expression.Type))
        {
            var parentExpr = expression.Expression;
            var parentValue = parentExpr.GetPropertyValue("Value");
            ExpressionTemplate template = null;

            // Check whether the member is a Field or Property
            if (expression.Member is FieldInfo fieldInfo)
            {
                template = fieldInfo.GetValue(parentValue) as ExpressionTemplate;
            }
            else if (expression.Member is PropertyInfo propInfo)
            {
                template = propInfo.GetValue(parentValue) as ExpressionTemplate;
            }

            // Get the value of the Name property of this Expression Target
            var property = expression.Type.GetProperty("Name", BindingFlags.Instance | BindingFlags.NonPublic);
            var targetName = property.GetValue(template) as string;

            if (string.IsNullOrWhiteSpace(targetName))
            {
                throw new ArgumentException($"Unable to obtain template name for {expression.Member.Name}");
            }

            if (!_parameters.ContainsKey(targetName))
            {
                // Get the template type based on the return type
                var targetType = ReferenceTypes.ContainsKey(expression.Type) ?
                    ReferenceTypes[expression.Type] : null;

                if (targetType == null)
                {
                    throw new ArgumentException($"The Target type '{expression.Type}' is not supported!");
                }

                // Reference property will be 'null' as the actual reference will be set later on
                _parameters[targetName] = new ExpressionParameter(null, targetType);
            }

            // No need to visit further as Expression Target objects should be local objects
            return new SimpleExpressionToken(targetName);
        }

        // This check is for CompositionObject.Properties property which is of type CompositionPropertySet. 
        // So while converting to string, 'Properties' need not be printed 
        if ((expression.Member is PropertyInfo) &&
            (expression.Member.Name == "Properties") &&
            (expression.Type == typeof(CompositionPropertySet)) &&
            (expression.Expression != null) &&
            ((typeof(CompositionObject).IsAssignableFrom(expression.Expression.Type)) ||
             (typeof(VisualTemplate).IsAssignableFrom(expression.Expression.Type))))
        {
            return Visit(expression.Expression);
        }

        // If the expression is of type CompositionPropertySet, then no need to 
        // visit this expression tree further. Just add this CompositionPropertySet
        // to the _parameters dictionary (if it doesn't already exist) and return
        // the name of the expression member
        if (expression.Type == typeof(CompositionPropertySet))
        {
            if (!_parameters.ContainsKey(expression.Member.Name) &&
                expression.Expression is ConstantExpression)
            {
                var parentExpr = expression.Expression;
                var parentValue = parentExpr.GetPropertyValue("Value");

                object paramValue = null;
                if (expression.Member is FieldInfo fieldInfo)
                {
                    paramValue = fieldInfo.GetValue(parentValue);
                }
                else if (expression.Member is PropertyInfo propInfo)
                {
                    paramValue = propInfo.GetValue(parentValue);
                }

                // Add the parameter
                AddParameter(expression.Member.Name, paramValue);
            }

            return new SimpleExpressionToken(expression.Member.Name);
        }

        // Check if the parent of this expression has a name which starts with CS$<
        var parentMemberExpr = expression.Expression as MemberExpression;
        if ((parentMemberExpr != null) &&
            parentMemberExpr.Member.Name.StartsWith("CS$<", StringComparison.Ordinal))
        {
            // Add to the parameters dictionary
            if (!_parameters.ContainsKey(expression.Member.Name)
                && (parentMemberExpr.Expression is ConstantExpression))
            {
                var constantExpr = (ConstantExpression)parentMemberExpr.Expression;

                var grandparentExpr = parentMemberExpr.Expression;
                var grandparentValue = grandparentExpr.GetPropertyValue("Value");

                object paramValue = null;
                if (parentMemberExpr.Member is FieldInfo parentFieldInfo)
                {
                    var localFieldValue = parentFieldInfo.GetValue(grandparentValue);
                    if (expression.Member is FieldInfo fieldInfo)
                    {
                        paramValue = fieldInfo.GetValue(localFieldValue);
                    }
                    else if (expression.Member is PropertyInfo propInfo)
                    {
                        paramValue = propInfo.GetValue(localFieldValue);
                    }
                }
                else if (parentMemberExpr.Member is PropertyInfo parentPropertyInfo)
                {
                    var localFieldValue = parentPropertyInfo.GetValue(grandparentValue);
                    if (expression.Member is FieldInfo fieldInfo)
                    {
                        paramValue = fieldInfo.GetValue(localFieldValue);
                    }
                    else if (expression.Member is PropertyInfo propInfo)
                    {
                        paramValue = propInfo.GetValue(localFieldValue);
                    }
                }

                // Add the parameter
                AddParameter(expression.Member.Name, paramValue);
            }

            return new SimpleExpressionToken(expression.Member.Name);
        }

        var token = new CompositeExpressionToken();
        var constExpr = expression.Expression as ConstantExpression;
        if ((constExpr?.Value != null)
            /*&& constExpr.Value.GetType().IsNested
            && constExpr.Value.GetType().Name.StartsWith("<", StringComparison.Ordinal)*/)
        {
            // Add to the parameters dictionary
            if (!_parameters.ContainsKey(expression.Member.Name))
            {
                object paramValue = null;
                if (expression.Member is FieldInfo fieldInfo)
                {
                    paramValue = fieldInfo.GetValue(constExpr.Value);
                }
                else if (expression.Member is PropertyInfo propInfo)
                {
                    paramValue = propInfo.GetValue(constExpr.Value);
                }

                // Add the parameter
                AddParameter(expression.Member.Name, paramValue);
            }

            return new SimpleExpressionToken(expression.Member.Name);
        }

        if (expression.Expression != null)
        {
            token.AddToken(Visit(expression.Expression));
        }
        else
        {
            token.AddToken(expression.Member.DeclaringType.Name);
        }

        token.AddToken($".{CleanIdentifier(expression.Member.Name)}");

        return token;
    }

    /// <summary>
    /// Visits a MethodCallExpression
    /// </summary>
    /// <param name="expression">MethodCallExpression</param>
    /// <returns>ExpressionToken</returns>
    [RequiresDynamicCode("Calls System.Type.MakeGenericType(params Type[])")]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(CompositionExpressionContext<>))]
    private static ExpressionToken Visit(MethodCallExpression expression)
    {
        var isExtensionMethod = expression.Method.IsDefined(typeof(ExtensionAttribute));
        var methodName = expression.Method.Name;

        var token = new CompositeExpressionToken();
        // If this is an extension method
        if (isExtensionMethod)
        {
            // If the .ToSingle() extension method is being called on a System.Double
            // value, no need to print it.
            if (expression.Method.DeclaringType == typeof(DoubleExtensions))
            {
                token.AddToken(Visit(expression.Arguments[0]));
            }
            // If the extension method being called belongs to CompositionPropertySetExtensions
            // then no need to add the method name
            else if (expression.Method.DeclaringType == typeof(CompositionPropertySetExtensions))
            {
                token.AddToken(Visit(expression.Arguments[0]));
                token.AddToken(".");
                _noQuotesForConstant = true;
                token.AddToken(Visit(expression.Arguments[1]));
                _noQuotesForConstant = false;
            }
            else
            {
                token.AddToken(Visit(expression.Arguments[0]));
                token.AddToken($".{methodName}");
                token.AddToken(new CompositeExpressionToken(expression.Arguments.Skip(1).Select(Visit), BracketType.Round,
                    true));
            }
        }
        else
        {
            var showDot = true;
            // Is this a static method?
            if (expression.Object == null)
            {
                token.AddToken(expression.Method.DeclaringType.FormattedName());
            }
            // No need to print the object name if the object is of type CompositionExpressionContext<T>
            else if (IsGenericCompositionExpressionContextType(expression.Object.Type))
            {
                showDot = false;

                // Is the method called for creating a type deriving from ExpressionTemplate?
                if (typeof(ExpressionTemplate).IsAssignableFrom(expression.Method.ReturnType))
                {
                    return VisitExpressionTemplate(expression, token);
                }
            }
            // Is the parent of this expression a List<> or Dictionary<> of objects
            // deriving from CompositionObject?
            else if (typeof(CompositionObject).IsAssignableFrom(expression.Type) &&
                     ((typeof(List<>).MakeGenericType(expression.Type).IsAssignableFrom(expression.Object.Type)) ||
                      (typeof(Dictionary<,>).MakeGenericType(expression.Arguments[0].Type, expression.Type)
                          .IsAssignableFrom(expression.Object.Type))) &&
                     (expression.Method.Name == "get_Item"))
            {
                return VisitCollection(expression);
            }
            else
            {
                token.AddToken(Visit(expression.Object));
            }

            if (expression.Method.IsSpecialName &&
                (expression.Method.DeclaringType.GetProperties()
                     .FirstOrDefault(p => p.GetAccessors().Contains(expression.Method)) != null))
            {
                token.AddToken(new CompositeExpressionToken(expression.Arguments.Select(Visit), BracketType.Square, true));
            }
            else
            {
                token.AddToken((showDot ? "." : "") + expression.Method.Name);
                token.AddToken(new CompositeExpressionToken(expression.Arguments.Select(Visit), BracketType.Round, true));
            }
        }

        return token;
    }

    /// <summary>
    /// Parses the MethodCallExpression which returns an object of Type
    /// Expression Target or Expression Reference
    /// </summary>
    /// <param name="expression">MethodCallExpression</param>
    /// <param name="token">ExpressionToken</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken VisitExpressionTemplate(MethodCallExpression expression, CompositeExpressionToken token)
    {
        // Is the type an Expression Target?
        // If this is a XXXTarget() method of the CompositionExpressionContext<T>
        // (i.e. the method returns a XXXTarget type deriving from ExpressionTemplate)
        // then it means that this sub expression is defining a reference to whichever 
        // CompositionObject the Expression is connected to
        if (ExpressionTargets.Contains(expression.Type))
        {
            return new SimpleExpressionToken("this.Target");
        }

        // Or is the type an Expression Reference?
        // If this is a XXXReference() method of the CompositionExpressionContext<T>
        // (i.e. the method returns a XXXReference type deriving from ExpressionTemplate)
        // then it means that the whole expression is an Expression Template in
        // which only the template names are defined. The actual object reference to these
        // targets will be added later on.
        // Obtain the method argument (which will be the key to the ExpressionParameter
        // created based on the method)
        if (ReferenceTypes.ContainsKey(expression.Type))
        {
            // There should be only one argument to this method and it should be the
            // Target name
            var key = expression.Arguments.ElementAt(0).GetPropertyValue("Value") as string;
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("targetName must not be empty!");
            }

            if (!_parameters.ContainsKey(key))
            {
                // Reference property will be 'null' as the actual reference will be set later on
                _parameters[key] = new ExpressionParameter(null, ReferenceTypes[expression.Method.ReturnType]);
            }

            token.AddToken(key);
            return token;
        }

        // It is an unsupported type deriving from ExpressionTemplate
        throw new ArgumentException(
            $"The type '{expression.Method.ReturnType}' is not supported as a Target or Reference!");
    }

    /// <summary>
    /// Parses the MethodCallExpression which is of type collection 
    /// (specifically List&lt;T&gt; or Dictionary&lt;T&gt;) 
    /// of objects deriving from CompositionObject.
    /// </summary>
    /// <param name="expression">MethodCallExpression</param>
    /// <returns>ExpressionToken</returns>
    [RequiresDynamicCode("Calls DevWinUI.CompositionExpressionEngine.GetExpressionValue(Expression)")]
    private static ExpressionToken VisitCollection(MethodCallExpression expression)
    {
        // Get the key
        var key = GetExpressionValue(expression.Arguments[0]);
        // Get the collection
        var collection = GetObject(expression.Object);
        object item = null;
        try
        {
            // Get the object deriving from CompositionObject
            item = collection.GetPropertyValue("Item", new[] { key });
        }
        catch (Exception e)
        {
            if (e.InnerException == null) throw;

            if (e.InnerException is KeyNotFoundException)
            {
                var dictionaryName = expression.Object is MemberExpression membExpr
                    ? $"'{membExpr.Member.Name}' dictionary"
                    : "dictionary";

                throw new KeyNotFoundException($"The given key '{key}' " +
                                               $"was not present in the {dictionaryName}.");
            }

            if (e.InnerException is IndexOutOfRangeException)
            {
                var listName = expression.Object is MemberExpression membExpr
                    ? $"'{membExpr.Member.Name}' list"
                    : "list";

                throw new IndexOutOfRangeException($"The given index '{key}'was out of range in " +
                                                   $"the {listName}. Must be non-negative and less " +
                                                   $"than the size of the list.");
            }

            throw;
        }

        string paramName;
        if (expression.Object is MemberExpression memberExpr)
        {
            paramName = $"{memberExpr.Member.Name}_Item_{key}";
        }
        else
        {
            paramName = Guid.NewGuid().ToString("D");
        }

        if (CanAddParameter(paramName, item))
        {
            AddParameter(paramName, item);
        }

        return new SimpleExpressionToken(paramName);
    }

    /// <summary>
    /// Visits a ParameterExpression
    /// </summary>
    /// <param name="expression">ParameterExpression</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Visit(ParameterExpression expression)
    {
        var name = expression.Name ?? "<param>";
        return new SimpleExpressionToken(CleanIdentifier(name));
    }

    /// <summary>
    /// Visits a TypeBinaryExpression
    /// </summary>
    /// <param name="expression">TypeBinaryExpression</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Visit(TypeBinaryExpression expression)
    {
        var token = new CompositeExpressionToken(BracketType.Round);
        token.AddToken($"{Visit(expression.Expression)} is {expression.TypeOperand.Name}");
        return token;
    }

    /// <summary>
    /// Visits a UnaryExpression
    /// </summary>
    /// <param name="expression">UnaryExpression</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Visit(UnaryExpression expression)
    {
        if (expression.NodeType == ExpressionType.Quote)
        {
            return Visit(expression.Operand);
        }

        var token = new CompositeExpressionToken();
        var suffix = string.Empty;
        var bracketsRequired = true;

        switch (expression.NodeType)
        {
            case ExpressionType.Convert:
            case ExpressionType.ConvertChecked:
                if (expression.Operand.Type.IsSubclassOf(expression.Type))
                    return Visit(expression.Operand);

                // Don't add a cast for any of the types in Floatables
                if (Floatables.Contains(expression.Type))
                {
                    bracketsRequired = false;
                }
                else
                {
                    token.AddToken(new CompositeExpressionToken(expression.Type.Name, BracketType.Round));
                }
                break;
            case ExpressionType.Negate:
            case ExpressionType.NegateChecked:
                token.AddToken("-");
                break;
            case ExpressionType.UnaryPlus:
                token.AddToken("+");
                break;
            case ExpressionType.Not:
                token.AddToken("!");
                break;
            case ExpressionType.PreIncrementAssign:
                token.AddToken("++");
                break;
            case ExpressionType.PreDecrementAssign:
                token.AddToken("--");
                break;
            case ExpressionType.TypeAs:
                token.AddToken(Visit(expression.Operand));
                token.AddToken(" as ");
                token.AddToken(expression.Type.Name);
                return token;
            case ExpressionType.OnesComplement:
                token.AddToken("~");
                break;
            case ExpressionType.PostIncrementAssign:
                suffix = "++";
                break;
            case ExpressionType.PostDecrementAssign:
                suffix = "--";
                break;
            default:
                token.AddToken(expression.NodeType.ToString());
                break;
        }

        // Visit the operand
        var operandToken = Visit(expression.Operand);
        var compToken = operandToken as CompositeExpressionToken;
        // If there are more the one tokens in the CompositeExpressionToken
        // then wrap them with Round brackets
        if (bracketsRequired && (compToken?.TokenCount() > 1))
        {
            compToken.SetBrackets(BracketType.Round);
        }

        token.AddToken(operandToken);

        // Is suffix non-empty?
        if (!string.IsNullOrWhiteSpace(suffix))
        {
            token.AddToken(suffix);
        }

        return token;
    }

    /// <summary>
    /// Visits a MemberAssignment
    /// </summary>
    /// <param name="mb">MemberAssignment</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Visit(MemberAssignment mb)
    {
        var token = new CompositeExpressionToken();
        token.AddToken($"{CleanIdentifier(mb.Member.Name)} = {Visit(mb.Expression)}");
        return token;
    }

    /// <summary>
    /// Visits a ListInitExpression
    /// </summary>
    /// <param name="expression">ListInitExpression</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Visit(ListInitExpression expression)
    {
        // Not supported right now
        return null;
    }

    /// <summary>
    /// Visits a MemberInitExpression
    /// </summary>
    /// <param name="expression">MemberInitExpression</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Visit(MemberInitExpression expression)
    {
        // Not supported right now
        return null;
    }

    /// <summary>
    /// Visits a MemberListBinding
    /// </summary>
    /// <param name="mb">MemberListBinding</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Visit(MemberListBinding mb)
    {
        // Not supported right now
        return null;
    }

    /// <summary>
    /// Visits a MemberMemberBinding
    /// </summary>
    /// <param name="mb">MemberMemberBinding</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Visit(MemberMemberBinding mb)
    {
        // Not supported right now
        return null;
    }

    /// <summary>
    /// Visits a NewArrayExpression
    /// </summary>
    /// <param name="expression">NewArrayExpression</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Visit(NewArrayExpression expression)
    {
        // Not supported right now
        return null;
    }

    /// <summary>
    /// Visits a NewExpression
    /// </summary>
    /// <param name="expression">NewExpression</param>
    /// <returns>ExpressionToken</returns>
    [RequiresDynamicCode("Calls DevWinUI.CompositionExpressionEngine.GetExpressionValue(Expression)")]
    private static ExpressionToken Visit(NewExpression expression)
    {
        // Is 'new' called for a type deriving from ExpressionTemplate?
        if (typeof(ExpressionTemplate).IsAssignableFrom(expression.Type))
        {
            // Is the type an Expression Target?
            if (ExpressionTargets.Contains(expression.Type))
            {
                return new SimpleExpressionToken("this.Target");
            }

            // Or is the type an Expression Reference?
            if (ReferenceTypes.ContainsKey(expression.Type))
            {
                // The first argument is the template name
                var key = GetExpressionValue(expression.Arguments.ElementAt(0)) as string;
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentException("Reference Name must not be empty!");
                }

                if (!_parameters.ContainsKey(key))
                {
                    // Reference property will be 'null' as the actual reference will be set later on
                    _parameters[key] = new ExpressionParameter(null, ReferenceTypes[expression.Type]);
                }

                return new SimpleExpressionToken(key);
            }

            // It is an unsupported type deriving from ExpressionTemplate
            throw new ArgumentException($"The type '{expression.Type}' is not supported as a Target or Reference!");
        }

        // Check if the new operator in supported in the expression for the given type
        if (Newables.Contains(expression.Type))
        {
            // Check if the number of arguments provided match
            // the number of arguments for the method (having same name) defined in CompositionExpressionContext<T>
            if (expression.Arguments.Count() != NewablesArgCount[expression.Type])
            {
                throw new ArgumentException(
                    $"Constructor '{expression.Type.Name}' requires {NewablesArgCount[expression.Type]} parameters " +
                    $"but is invoked with {expression.Arguments.Count()} parameters.");
            }

            var token = new CompositeExpressionToken();
            // Get the corresponding method name for creating an instance of this type in the expression
            token.AddToken(expression.Type.Name);
            token.AddToken(new CompositeExpressionToken(expression.Arguments.Select(Visit), BracketType.Round, true));

            return token;
        }

        throw new ArgumentException($"new operator for the type '{expression.Type}' is not supported in the expression.");
    }

    /// <summary>
    /// Visits a BinaryExpression in which an array index is being accessed
    /// </summary>
    /// <param name="expression">BinaryExpression</param>
    /// <returns>ExpressionToken</returns>
    [RequiresDynamicCode("Calls DevWinUI.CompositionExpressionEngine.GetExpressionValue(Expression)")]
    private static ExpressionToken VisitArrayExpression(BinaryExpression expression)
    {
        // In a BinaryExpression for ArrayIndex, the Left expression will be the
        // MemberExpression to access the array object and the Right expression will
        // be the index number or string.
        var arrayExpr = expression.Left as MemberExpression;
        if (arrayExpr == null)
            return null;

        var arrayName = arrayExpr.Member.Name;
        // Get the array index
        var index = (int)GetExpressionValue(expression.Right);
        // Is a valid index obtained?
        if (index == -1)
        {
            throw new ArgumentException($"Invalid value parsed for the index of the '{arrayName}' array!");
        }

        // Generate key name to insert into the _parameters dictionary
        var paramName = $"{arrayName}_Index_{index}";

        if ((!_parameters.ContainsKey(paramName)) && ((arrayExpr?.Expression as ConstantExpression) != null))
        {
            var parentValue = arrayExpr.Expression.GetPropertyValue("Value");

            object arrayValue = null;

            // Is the array a field?
            if (arrayExpr.Member is FieldInfo fieldInfo)
            {
                arrayValue = fieldInfo.GetValue(parentValue);
            }
            // Or is the array a Property?
            else if (arrayExpr.Member is PropertyInfo propInfo)
            {
                arrayValue = propInfo.GetValue(parentValue);
            }

            if ((arrayValue != null) && (arrayValue is Array array))
            {
                // Get the element in the 'index' position in the array
                // and add it to the _parameters dictionary
                AddParameter(paramName, array.GetValue(index));
            }
            else
            {
                return new SimpleExpressionToken(string.Empty);
            }
        }

        return new SimpleExpressionToken(paramName);
    }

    /// <summary>
    /// Obtains the value of the given Expression.
    /// Extracts the array index value as an integer. The expression can be either
    /// a ConstantExpression or a MemberExpression. It is usually the right-side 
    /// expression of a BinaryExpression in which an array index is being accessed.
    /// It can also be the Index of a generic List collection. If the expression
    /// is neither a Constant or MemberExpression, then compile it to get the value.
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <returns>object</returns>
    [RequiresDynamicCode("Calls System.Linq.Expressions.Expression.Lambda(Expression, params ParameterExpression[])")]
    private static object GetExpressionValue(Expression expression)
    {
        object result = null;

        // Is it a ConstantExpression
        if (expression is ConstantExpression constExpr)
        {
            result = constExpr.Value;
        }
        // Or is it a MemberExpression
        else if (expression is MemberExpression membExpr)
        {
            // Get the value of MemberExpression.Expression recursively
            var exprValue = GetExpressionValue(membExpr.Expression);

            // Is the member a field
            if (membExpr.Member is FieldInfo fieldInfo)
            {
                result = (int)fieldInfo.GetValue(exprValue);
            }
            // Or is the member a property
            else if (membExpr.Member is PropertyInfo propInfo)
            {
                result = (int)propInfo.GetValue(exprValue);
            }
        }
        // If it is neither of the above two, then compile the expression 
        // and invoke it dynamically to get the value
        else
        {
            result = Expression.Lambda(expression).Compile().DynamicInvoke();
        }

        return result;
    }

    #endregion

    #region Parse Methods

    /// <summary>
    /// Parses an Expression. This method acts as a router and
    /// based on the specific type of Expression, the appropriate
    /// Parse method is called.
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Parse(Expression expression)
    {
        if (expression == null)
        {
            return new SimpleExpressionToken("null");
        }

        var baseType = expression.GetType();
        while (!baseType.IsPublic())
        {
            baseType = baseType.BaseType();
        }

        // Get the Visit method whose first parameter best matches the type of baseType
        MethodInfo methodInfo;
        if (ParseMethods.TryGetValue(baseType, out methodInfo) ||
            ((baseType.BaseType() != null) && ParseMethods.TryGetValue(baseType.BaseType(), out methodInfo)))
        {
            // Once a matching Visit method is found, Invoke it!
            return (ExpressionToken)methodInfo.Invoke(null, new object[] { expression });
        }

        return null;
    }

    /// <summary>
    /// Parses a BinaryExpression
    /// </summary>
    /// <param name="expression">BinaryExpression</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Parse(BinaryExpression expression)
    {
        // If it the property of an object, (deriving from CompositionObject)
        // which is a part of an array of similar objects, is being 
        // accessed, then no need to further parse this expression
        if ((expression.NodeType == ExpressionType.ArrayIndex) &&
            (expression.Type.IsSubclassOf(typeof(CompositionObject))))
        {
            return null;
        }

        // Check if it is the outermost BinaryExpression
        // If yes, then no need to add round brackets to 
        // the whole visited expression
        var noBrackets = _firstParseBinaryExpression;
        if (_firstParseBinaryExpression)
        {
            // Set it to false so that the internal BinaryExpression(s)
            // will have round brackets
            _firstParseBinaryExpression = false;
        }

        var leftToken = Parse(expression.Left);
        var rightToken = Parse(expression.Right);

        string symbol;
        if (!BinaryExpressionStrings.TryGetValue(expression.NodeType, out symbol))
            return null;

        // This check is done to avoid wrapping the final ExpressionToken in Round Brackets
        var bracketType = noBrackets ? BracketType.None : BracketType.Round;

        var token = new CompositeExpressionToken(bracketType);
        token.AddToken($"{leftToken} {symbol} {rightToken}");

        return token;
    }

    /// <summary>
    /// Parses a ConstantExpression
    /// </summary>
    /// <param name="expression">ConstantExpression</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Parse(ConstantExpression expression)
    {
        if (expression.Value == null)
            return new SimpleExpressionToken("null");

        if (expression.Value.GetType().IsNested
            && expression.Value.GetType().Name.StartsWith("<", StringComparison.Ordinal))
            return new SimpleExpressionToken(string.Empty);

        var str = expression.Value as string;
        if (str != null)
            return new CompositeExpressionToken(str, _noQuotesForParseConstant ? BracketType.None : BracketType.Quotes);

        return new SimpleExpressionToken(expression.Value.ToString());
    }

    /// <summary>
    /// Parses a LambdaExpression
    /// </summary>
    /// <param name="expression">LambdaExpression</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Parse(LambdaExpression expression)
    {
        if ((expression.Body as BinaryExpression) != null)
        {
            _firstParseBinaryExpression = true;
        }

        // Expression Body
        return Parse(expression.Body);
    }

    /// <summary>
    /// Parses a MemberExpression
    /// </summary>
    /// <param name="expression">MemberExpression</param>
    /// <returns>ExpressionToken</returns>
    [RequiresDynamicCode("Calls DevWinUI.CompositionExpressionEngine.IsGenericCompositionExpressionContextType(Type)")]
    private static ExpressionToken Parse(MemberExpression expression)
    {
        // If the the expression type is a class implementing the IGraphicEffect interface or
        // the IGraphicEffectSource interface and the instance object of this expression has
        // a Name property, then return the value of Name property of this class and 
        // proceed no further in the expression. 
        if (typeof(IGraphicsEffect).IsAssignableFrom(expression.Type) ||
            typeof(IGraphicsEffectSource).IsAssignableFrom(expression.Type))
        {
            // Get the instance object for the expression
            var sourceObj = GetObject(expression);
            // Get its Name property value
            var name = sourceObj?.GetPropertyValue("Name") as string;

            return String.IsNullOrWhiteSpace(name) ? null : new SimpleExpressionToken(name);
        }

        // Check if this expression is accessing the StartingValue or FinalValue
        // Property of CompositionExpressionContext<T>
        if (((expression.Member as PropertyInfo) != null) &&
            (expression.Expression != null) &&
            IsGenericCompositionExpressionContextType(expression.Expression.Type))
        {
            return new SimpleExpressionToken($"this.{expression.Member.Name}");
        }

        // This check is for CompositionPropertySet. It has a property called 
        // Properties which is of type CompositionPropertySet. So while converting to string, 'Properties' 
        // need not be printed 
        if (((expression.Member as PropertyInfo) != null) &&
            (expression.Type == typeof(CompositionPropertySet) && (expression.Member.Name == "Properties"))
            && (expression.Expression is MemberExpression) && (expression.Expression.Type == typeof(CompositionPropertySet)))
        {
            return Parse(expression.Expression);
        }

        // If the expression type is a subclass of CompositionObject, no need to proceed further
        // because in one of the scenarios, the ParseExpression API is called on an object which
        // is a subclass of CompositionObject and we just need the property access string.
        if (typeof(CompositionObject).IsAssignableFrom(expression.Type))
            return null;

        var token = new CompositeExpressionToken();
        // Visit the parent
        var parent = expression.Expression != null ?
            Parse(expression.Expression) :
            new SimpleExpressionToken(expression.Member.DeclaringType.Name);

        if (parent != null)
        {
            token.AddToken(parent);
            token.AddToken($".{CleanIdentifier(expression.Member.Name)}");
        }
        else
        {
            token.AddToken(CleanIdentifier(expression.Member.Name));
        }

        return token;
    }

    /// <summary>
    /// Parses a MethodCallExpression
    /// </summary>
    /// <param name="expression">MethodCallExpression</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Parse(MethodCallExpression expression)
    {
        var isExtensionMethod = expression.Method.IsDefined(typeof(ExtensionAttribute));
        var methodName = expression.Method.Name;
        // Special Case: If the extension method name is ScaleXY,
        // then 'Scale.XY' must be the string returned.
        if (methodName == "ScaleXY")
        {
            methodName = "Scale.XY";
        }

        var token = new CompositeExpressionToken();
        // If this is an extension method
        if (isExtensionMethod)
        {
            // If the .ToSingle() extension method is being called on a System.Double
            // value, no need to print it.
            if (expression.Method.DeclaringType == typeof(DoubleExtensions))
            {
                token.AddToken(Parse(expression.Arguments[0]));
            }
            // If the extension method being called belongs to CompositionPropertySetExtensions
            // then no need to add the method name
            else if (expression.Method.DeclaringType == typeof(CompositionPropertySetExtensions))
            {
                var parent = Parse(expression.Arguments[0]);
                if (parent != null)
                {
                    token.AddToken(parent);
                    token.AddToken(".");
                }

                _noQuotesForParseConstant = true;
                token.AddToken(Parse(expression.Arguments[1]));
                _noQuotesForParseConstant = false;
            }
            else
            {
                var parent = Parse(expression.Arguments[0]);
                if (parent != null)
                {
                    token.AddToken(parent);
                    token.AddToken(".");
                }

                token.AddToken(methodName);
            }
        }
        else
        {
            var showDot = true;
            if (expression.Object == null)
            {
                token.AddToken(expression.Method.DeclaringType.FormattedName());
            }
            // No need to print the object name if the object derives from CompositionObject
            else if (typeof(CompositionObject).IsAssignableFrom(expression.Type))
            {
                //showDot = false;
                return null;
            }
            else
            {
                token.AddToken(Parse(expression.Object));
            }

            // Is it an array index based access?
            if (expression.Method.IsSpecialName &&
                (expression.Method.DeclaringType.GetProperties()
                     .FirstOrDefault(p => p.GetAccessors().Contains(expression.Method)) != null))
            {
                token.AddToken(new CompositeExpressionToken(expression.Arguments.Select(Parse), BracketType.Square, true));
            }
            else
            {
                token.AddToken(showDot ? $".{expression.Method.Name}" : expression.Method.Name);
            }
        }

        return token;
    }

    /// <summary>
    /// Parses a ParameterExpression
    /// </summary>
    /// <param name="expression">ParameterExpression</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Parse(ParameterExpression expression)
    {
        var name = expression.Name ?? "<param>";
        return new SimpleExpressionToken(CleanIdentifier(name));
    }

    /// <summary>
    /// Parses a UnaryExpression
    /// </summary>
    /// <param name="expression">UnaryExpression</param>
    /// <returns>ExpressionToken</returns>
    private static ExpressionToken Parse(UnaryExpression expression)
    {
        if (expression.NodeType == ExpressionType.Quote)
        {
            return Parse(expression.Operand);
        }

        var token = new CompositeExpressionToken();

        switch (expression.NodeType)
        {
            case ExpressionType.Convert:
            case ExpressionType.ConvertChecked:
            case ExpressionType.TypeAs:
                token.AddToken(Parse(expression.Operand));
                break;
            default:
                return null;
        }

        return token;
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Converts identifier name to a more readable format 
    /// </summary>
    /// <param name="name">Identifier Name</param>
    /// <returns>Formatted Name</returns>
    private static string CleanIdentifier(string name)
    {
        if (name == null)
            return null;
        if (name.StartsWith("<>h__TransparentIdentifier", StringComparison.Ordinal))
            return "temp_" + name.Substring(26);
        return name;
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(CompositionExpressionContext<>))]
    [RequiresDynamicCode("Calls System.Type.MakeGenericType(params Type[])")]
    /// <summary>
    /// Checks if the given type of of type CompositionExpressionContext&lt;T&gt;
    /// </summary>
    /// <param name="inputType">Type to check</param>
    /// <returns>True of type matches otherwise false</returns>
    private static bool IsGenericCompositionExpressionContextType(Type inputType)
    {
        if ((inputType == null) ||
            (!inputType.IsGenericType()) ||
            (!inputType.GenericTypeArguments.Any()))
            return false;

        var paramType = inputType.GenericTypeArguments[0];
        return (inputType == typeof(CompositionExpressionContext<>).MakeGenericType(paramType));
    }

    /// <summary>
    /// Gets the instance object for the given expression
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <returns>Object</returns>
    public static object GetObject(Expression expression)
    {
        var memberStack = new Stack<MemberInfo>();

        // Move from leaf expression to the root expression
        // as long as it is a MemberExpression or 
        // UnaryExpression(convert or cast operations only)
        do
        {
            if ((expression as MemberExpression) != null)
            {
                var memberExpr = expression as MemberExpression;
                memberStack.Push(memberExpr.Member);
                expression = memberExpr.Expression;
            }
            else if ((expression as UnaryExpression) != null)
            {
                if ((expression.NodeType == ExpressionType.Convert) ||
                    (expression.NodeType == ExpressionType.ConvertChecked) ||
                    (expression.NodeType == ExpressionType.TypeAs))
                {
                    expression = ((UnaryExpression)expression).Operand;
                }
                else
                {
                    // Any other type of Expression is not supported right now!
                    throw new ArgumentException(
                        "This expression is not supported in CompositionExpression!", nameof(expression));
                }
            }
        }
        while ((expression is MemberExpression) || (expression is UnaryExpression));

        var resultObject = (object)null;
        // If the expression is null then it means the root is a static member
        if (expression == null)
        {
            var memberInfo = memberStack.Pop();
            if ((memberInfo as PropertyInfo) != null)
            {
                // Property will be (public or non-public) and static
                resultObject = memberInfo.DeclaringType
                    .GetProperty(memberInfo.Name, BindingFlags.NonPublic |
                                                  BindingFlags.Public |
                                                  BindingFlags.Static)
                    ?.GetValue(null);
            }
            else if ((memberInfo as FieldInfo) != null)
            {
                // Field will be (public or non-public) and static
                var flags = BindingFlags.Static;
                flags |= ((FieldInfo)memberInfo).IsPublic ? BindingFlags.Public : BindingFlags.NonPublic;

                resultObject = memberInfo.DeclaringType
                    .GetField(memberInfo.Name, flags)
                    ?.GetValue(null);
            }
        }
        // If the root is a ConstantExpression, then its Value represents the root object
        else if ((expression as ConstantExpression) != null)
        {
            var constExpr = expression as ConstantExpression;
            resultObject = constExpr.Value;
        }
        else
        {
            // Any other type of Expression is not supported right now!
            throw new ArgumentException(
                "This expression is not supported in CompositionExpression!", nameof(expression));
        }

        // Move from root member info to the leaf member info and get the 
        // corresponding object for each
        while (memberStack.Any() && (resultObject != null))
        {
            var mi = memberStack.Pop();
            if ((mi as PropertyInfo) != null)
            {
                // Property can be (public or non-public) and (static or instance)
                resultObject = resultObject.GetType()
                    .GetProperty(mi.Name, BindingFlags.Static | BindingFlags.Instance |
                                          BindingFlags.NonPublic | BindingFlags.Public)
                    ?.GetValue(resultObject, null);
            }
            else if ((mi as FieldInfo) != null)
            {
                // Field can be (public or non-public) and (static or instance)
                var fieldInfo = mi as FieldInfo;
                var flags = fieldInfo.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
                flags |= fieldInfo.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic;

                resultObject = resultObject.GetType()
                    .GetField(mi.Name, flags)
                    ?.GetValue(resultObject);
            }
        }

        return resultObject;
    }

    /// <summary>
    /// Checks if a parameter having given name and value can be added
    /// to the _parameters dictionary.
    /// </summary>
    /// <param name="paramName">Parameter name</param>
    /// <param name="paramValue">Parameter value</param>
    /// <returns></returns>
    private static bool CanAddParameter(string paramName, object paramValue)
    {
        if (string.IsNullOrWhiteSpace(paramName) || (paramValue == null))
            return false;

        if (!_parameters.ContainsKey(paramName))
            return true;

        // Check if the same object has been already added with the same name
        if (object.ReferenceEquals(paramValue, _parameters[paramName].Reference))
        {
            return false;
        }

        // Check if another object of compatible type has been already added 
        // with the same name
        if (_parameters[paramName].Type.IsAssignableFrom(paramValue.GetType()))
        {
            throw new ArgumentException($"An object with the key '{paramName}' is already added!");
        }

        // Another object of incompatible type has been already added with same name
        throw new ArgumentException($"Cannot add an object of type '{paramValue.GetType()}' to" +
                                    $" _parameters. Expected Type: '{_parameters[paramName].Type}'");
    }

    /// <summary>
    /// Creates an ExpressionParameter from the given paramValue and adds
    /// it to the _parameters dictionary
    /// </summary>
    /// <param name="paramName">Parameter Key</param>
    /// <param name="paramValue">Parameter Value</param>
    private static void AddParameter(string paramName, object paramValue)
    {
        if (paramValue == null)
        {
            throw new ArgumentException("Cannot add parameter: paramValue is null!");
        }

        if (string.IsNullOrWhiteSpace(paramName))
        {
            throw new ArgumentException("Cannot add parameter: paramName is null or empty!");
        }

        var parameter = paramValue;
        var type = parameter.GetType();

        // Can the type be converted to float?
        if (Floatables.Contains(type))
        {
            type = typeof(float);
            parameter = Convert.ToSingle(parameter);
        }

        // Create an ExpressionParameter and add it to parameters
        _parameters[paramName] = new ExpressionParameter(parameter, type);
    }

    #endregion
}
