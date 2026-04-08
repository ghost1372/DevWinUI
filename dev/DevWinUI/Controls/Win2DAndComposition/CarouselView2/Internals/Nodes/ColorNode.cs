namespace DevWinUI;

#pragma warning disable CS0660, CS0661
internal sealed partial class ColorNode : ExpressionNode
{
    internal ColorNode()
    {
    }

    protected internal override string GetValue()
    {
        return $"ColorRgb({_value.A},{_value.R},{_value.G},{_value.B})";
    }

    private Color _value;
}
#pragma warning restore CS0660, CS0661
