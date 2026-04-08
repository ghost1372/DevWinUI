namespace DevWinUI;

internal abstract partial class ReferenceNode : ExpressionNode
{
    internal ReferenceNode(string paramName, CompositionObject? compObj = null)
    {
        Reference = compObj;
        NodeType = ExpressionNodeType.Reference;
        ParamName = paramName;
    }

    public CompositionObject? Reference { get; private set; }

    internal string GetReferenceNodeString()
    {
        if (NodeType == ExpressionNodeType.TargetReference)
        {
            return "this.target";
        }
        else
        {
            return NodeName;
        }
    }

    protected internal T ReferenceProperty<T>(string propertyName)
        where T : ExpressionNode
    {
        T newNode = ExpressionNode.CreateExpressionNode<T>();

        (newNode as ExpressionNode).NodeType = ExpressionNodeType.ReferenceProperty;
        (newNode as ExpressionNode).Children.Add(this);
        (newNode as ExpressionNode).PropertyName = propertyName;

        return newNode;
    }

    protected internal override string GetValue()
    {
        throw new NotImplementedException("GetValue is not implemented for ReferenceNode and shouldn't be called");
    }
}
