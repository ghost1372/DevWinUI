namespace DevWinUI;

/// <summary>
/// Delegate which takes an input of type CompositionExpressionContext&lt;T&gt;
/// and gives an object of type T as result. This delegate is mainly used to 
/// create Expressions in Expression Animations.
/// </summary>
/// <typeparam name="T">Type of the property being animated</typeparam>
/// <param name="ctx">CompositinExpressionContext&lt;T&gt;</param>
/// <returns>An object of type T</returns>
public delegate T CompositionExpression<T>(CompositionExpressionContext<T> ctx);