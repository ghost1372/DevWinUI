namespace DevWinUI;

internal sealed partial class VisualReferenceNode : ReferenceNode
{
    internal VisualReferenceNode(string paramName, Visual? v = null)
        : base(paramName, v)
    {
    }

    public ScalarNode RotationAngleInDegrees
    {
        get { return ReferenceProperty<ScalarNode>("RotationAngleInDegrees"); }
    }

    public Vector3Node Offset
    {
        get { return ReferenceProperty<Vector3Node>("Offset"); }
    }
}
