namespace DevWinUI;


#pragma warning disable CS0660, CS0661
internal sealed partial class Vector2Node : ExpressionNode
{
    internal Vector2Node()
    {
    }
#pragma warning restore SA1117

    protected internal override string GetValue()
    {
        return $"Vector2({_value.X.ToCompositionString()},{_value.Y.ToCompositionString()})";
    }

    private Vector2 _value;
}
#pragma warning restore CS0660, CS0661
