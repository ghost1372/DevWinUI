namespace DevWinUI;


#pragma warning disable CS0660, CS0661

internal sealed partial class Vector3Node : ExpressionNode
{
    internal Vector3Node()
    {
    }

    public enum Subchannel
    {
        X,
        Y,
        Z
    }

    public ScalarNode X
    {
        get { return GetSubchannels(Subchannel.X); }
    }

    public ScalarNode Y
    {
        get { return GetSubchannels(Subchannel.Y); }
    }

    public ScalarNode GetSubchannels(Subchannel s)
    {
        return SubchannelsInternal<ScalarNode>(s.ToString());
    }

#pragma warning restore SA1117 // Parameters must be on same line or separate lines

    protected internal override string GetValue()
    {
        return $"Vector3({_value.X.ToCompositionString()},{_value.Y.ToCompositionString()},{_value.Z.ToCompositionString()})";
    }

    private Vector3 _value;
}
#pragma warning restore CS0660, CS0661
