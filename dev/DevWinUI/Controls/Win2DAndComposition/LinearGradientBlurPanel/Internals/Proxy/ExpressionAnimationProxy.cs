namespace DevWinUI;

internal partial class ExpressionAnimationProxy : CompositionAnimationProxy, IExpressionAnimation
{
    public ExpressionAnimationProxy(CompositionAnimation animation)
        : base(animation) { }

    public string Expression
    {
        get => ((ExpressionAnimation)RawObject).Expression;
        set => ((ExpressionAnimation)RawObject).Expression = value;
    }
}
