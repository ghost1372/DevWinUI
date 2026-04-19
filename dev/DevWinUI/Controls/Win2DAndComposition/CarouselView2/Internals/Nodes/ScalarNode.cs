namespace DevWinUI;


#pragma warning disable CS0660, CS0661
internal sealed partial class ScalarNode : ExpressionNode
{
    internal ScalarNode()
    {
    }

    internal ScalarNode(float value)
    {
        _value = value;
        NodeType = ExpressionNodeType.ConstantValue;
    }

    public static implicit operator ScalarNode(float value)
    {
        return new ScalarNode(value);
    }

    public static ScalarNode operator +(ScalarNode left, ScalarNode right)
    {
        return ExpressionFunctions.Function<ScalarNode>(ExpressionNodeType.Add, left, right);
    }

    public static ScalarNode operator -(ScalarNode left, ScalarNode right)
    {
        return ExpressionFunctions.Function<ScalarNode>(ExpressionNodeType.Subtract, left, right);
    }

    public static ScalarNode operator *(ScalarNode left, ScalarNode right)
    {
        return ExpressionFunctions.Function<ScalarNode>(ExpressionNodeType.Multiply, left, right);
    }

    public static ScalarNode operator /(ScalarNode left, ScalarNode right)
    {
        return ExpressionFunctions.Function<ScalarNode>(ExpressionNodeType.Divide, left, right);
    }

    public static BooleanNode operator <=(ScalarNode left, ScalarNode right)
    {
        return ExpressionFunctions.Function<BooleanNode>(ExpressionNodeType.LessThanEquals, left, right);
    }

    public static BooleanNode operator <(ScalarNode left, ScalarNode right)
    {
        return ExpressionFunctions.Function<BooleanNode>(ExpressionNodeType.LessThan, left, right);
    }

    public static BooleanNode operator >=(ScalarNode left, ScalarNode right)
    {
        return ExpressionFunctions.Function<BooleanNode>(ExpressionNodeType.GreaterThanEquals, left, right);
    }

    public static BooleanNode operator >(ScalarNode left, ScalarNode right)
    {
        return ExpressionFunctions.Function<BooleanNode>(ExpressionNodeType.GreaterThan, left, right);
    }

    protected internal override string GetValue()
    {
        return _value.ToCompositionString();
    }

    private float _value;
}
#pragma warning restore CS0660, CS0661
