namespace DevWinUI;

#pragma warning disable CS0660, CS0661

internal sealed partial class BooleanNode : ExpressionNode
{
    internal BooleanNode()
    {
    }

    internal BooleanNode(bool value)
    {
        _value = value;
        NodeType = ExpressionNodeType.ConstantValue;
    }

    internal BooleanNode(string paramName)
    {
        ParamName = paramName;
        NodeType = ExpressionNodeType.ConstantParameter;
    }

    internal BooleanNode(string paramName, bool value)
    {
        ParamName = paramName;
        _value = value;
        NodeType = ExpressionNodeType.ConstantParameter;

        SetBooleanParameter(paramName, value);
    }

    public static implicit operator BooleanNode(bool value)
    {
        return new BooleanNode(value);
    }

    public static BooleanNode operator &(BooleanNode left, BooleanNode right)
    {
        return ExpressionFunctions.Function<BooleanNode>(ExpressionNodeType.And, left, right);
    }

    protected internal override string GetValue()
    {
        return _value ? "true" : "false";
    }

    private bool _value;
}
#pragma warning restore CS0660, CS0661
