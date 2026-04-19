using System.Diagnostics.Contracts;

namespace DevWinUI;

internal static partial class CarouselView2InternalExtensions
{
    [Pure]
    public static string ToCompositionString(this float number)
    {
        var defaultString = number.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var eIndex = defaultString.IndexOf('E');

        // If the default string representation is not in scientific notation, we can use it
        if (eIndex == -1)
        {
            return defaultString;
        }

        // If the number uses scientific notation because it is too large, we can print it without the decimal places
        var exponent = int.Parse(defaultString.Substring(eIndex + 1));
        if (exponent >= 0)
        {
            return number.ToString($"F0", System.Globalization.CultureInfo.InvariantCulture);
        }

        // Otherwise, we need to print it with the right number of decimals
        var decimalPlaces = -exponent // The number of decimal places is the exponent of 10
            + eIndex // Plus each character in the mantissa
            + (number < 0 ?
                -3 : // Minus the sign, dot and first number of the mantissa if negative
                -2); // Minus the dot and first number of the mantissa otherwise

        return number.ToString($"F{decimalPlaces}", System.Globalization.CultureInfo.InvariantCulture);
    }

    public static VisualReferenceNode GetReference(this Visual compObj)
    {
        return new VisualReferenceNode(null, compObj);
    }

    public static void StartAnimation(this CompositionObject compObject, string propertyName, ExpressionNode expressionNode)
    {
        compObject.StartAnimation(propertyName, CreateExpressionAnimationFromNode(compObject.Compositor, expressionNode));
    }

    private static ExpressionAnimation CreateExpressionAnimationFromNode(Compositor compositor, ExpressionNode expressionNode)
    {
        // Only create a new animation if this node hasn't already generated one before, so we don't have to re-parse the expression string.
        if (expressionNode.ExpressionAnimation == null)
        {
            expressionNode.ClearReferenceInfo();
            expressionNode.ExpressionAnimation = compositor.CreateExpressionAnimation(expressionNode.ToExpressionString());
        }

        // We need to make sure all parameters are up to date, even if the animation already existed.
        expressionNode.SetAllParameters(expressionNode.ExpressionAnimation);

        return expressionNode.ExpressionAnimation;
    }
}
