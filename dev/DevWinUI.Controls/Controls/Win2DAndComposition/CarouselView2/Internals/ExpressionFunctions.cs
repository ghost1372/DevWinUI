namespace DevWinUI;

internal static partial class ExpressionFunctions
{
    public static ScalarNode Abs(ScalarNode val)
    {
        return Function<ScalarNode>(ExpressionNodeType.Absolute, val);
    }

    public static ColorNode ColorLerp(ColorNode val1, ColorNode val2, ScalarNode progress)
    {
        return Function<ColorNode>(ExpressionNodeType.ColorLerp, val1, val2, progress);
    }

    public static ColorNode ColorRgb(ScalarNode alpha, ScalarNode red, ScalarNode green, ScalarNode blue)
    {
        return Function<ColorNode>(ExpressionNodeType.ColorRgb, alpha, red, green, blue);
    }
    public static ScalarNode Mod(ScalarNode val1, ScalarNode val2)
    {
        return Function<ScalarNode>(ExpressionNodeType.Modulus, val1, val2);
    }

    public static ScalarNode Conditional(BooleanNode condition, ScalarNode trueCase, ScalarNode falseCase)
    {
        return Function<ScalarNode>(ExpressionNodeType.Conditional, condition, trueCase, falseCase);
    }

    public static ColorNode Conditional(BooleanNode condition, ColorNode trueCase, ColorNode falseCase)
    {
        return Function<ColorNode>(ExpressionNodeType.Conditional, condition, trueCase, falseCase);
    }

    internal static T Function<T>(ExpressionNodeType nodeType, params ExpressionNode[] expressionFunctionParams)
        where T : ExpressionNode
    {
        T newNode = ExpressionNode.CreateExpressionNode<T>();
        newNode.NodeType = nodeType;

        foreach (var param in expressionFunctionParams)
        {
            newNode.Children.Add(param);
        }

        return newNode;
    }

    internal static ExpressionNodeInfo GetNodeInfoFromType(ExpressionNodeType type)
    {
        return _expressionNodeInfo[type];
    }

    internal struct ExpressionNodeInfo
    {
        public ExpressionNodeInfo(OperationType nodeOperationKind, string operationString)
        {
            NodeOperationKind = nodeOperationKind;
            OperationString = operationString;
        }

        internal OperationType NodeOperationKind { get; set; }

        internal string OperationString { get; set; }
    }

    private static readonly Dictionary<ExpressionNodeType, ExpressionNodeInfo> _expressionNodeInfo = new Dictionary<ExpressionNodeType, ExpressionNodeInfo>
    {
        { ExpressionNodeType.ConstantValue,            new ExpressionNodeInfo(OperationType.Constant,      null!) },
        { ExpressionNodeType.ConstantParameter,        new ExpressionNodeInfo(OperationType.Constant,      null!) },
        { ExpressionNodeType.CurrentValueProperty,     new ExpressionNodeInfo(OperationType.Reference,     null!) },
        { ExpressionNodeType.Reference,                new ExpressionNodeInfo(OperationType.Reference,     null!) },
        { ExpressionNodeType.ReferenceProperty,        new ExpressionNodeInfo(OperationType.Reference,     null!) },
        { ExpressionNodeType.StartingValueProperty,    new ExpressionNodeInfo(OperationType.Reference,     null!) },
        { ExpressionNodeType.TargetReference,          new ExpressionNodeInfo(OperationType.Reference,     null!) },
        { ExpressionNodeType.Conditional,              new ExpressionNodeInfo(OperationType.Conditional,   null!) },
        { ExpressionNodeType.Swizzle,                  new ExpressionNodeInfo(OperationType.Swizzle,       null!) },
        { ExpressionNodeType.Add,                      new ExpressionNodeInfo(OperationType.Operator,      "+") },
        { ExpressionNodeType.And,                      new ExpressionNodeInfo(OperationType.Operator,      "&&") },
        { ExpressionNodeType.Divide,                   new ExpressionNodeInfo(OperationType.Operator,      "/") },
        { ExpressionNodeType.Equals,                   new ExpressionNodeInfo(OperationType.Operator,      "==") },
        { ExpressionNodeType.GreaterThan,              new ExpressionNodeInfo(OperationType.Operator,      ">") },
        { ExpressionNodeType.GreaterThanEquals,        new ExpressionNodeInfo(OperationType.Operator,      ">=") },
        { ExpressionNodeType.LessThan,                 new ExpressionNodeInfo(OperationType.Operator,      "<") },
        { ExpressionNodeType.LessThanEquals,           new ExpressionNodeInfo(OperationType.Operator,      "<=") },
        { ExpressionNodeType.Multiply,                 new ExpressionNodeInfo(OperationType.Operator,      "*") },
        { ExpressionNodeType.Not,                      new ExpressionNodeInfo(OperationType.UnaryOperator, "!") },
        { ExpressionNodeType.NotEquals,                new ExpressionNodeInfo(OperationType.Operator,      "!=") },
        { ExpressionNodeType.Or,                       new ExpressionNodeInfo(OperationType.Operator,      "||") },
        { ExpressionNodeType.Subtract,                 new ExpressionNodeInfo(OperationType.Operator,      "-") },
        { ExpressionNodeType.Absolute,                 new ExpressionNodeInfo(OperationType.Function,      "abs") },
        { ExpressionNodeType.Acos,                     new ExpressionNodeInfo(OperationType.Function,      "acos") },
        { ExpressionNodeType.Asin,                     new ExpressionNodeInfo(OperationType.Function,      "asin") },
        { ExpressionNodeType.Atan,                     new ExpressionNodeInfo(OperationType.Function,      "atan") },
        { ExpressionNodeType.Cos,                      new ExpressionNodeInfo(OperationType.Function,      "cos") },
        { ExpressionNodeType.Ceil,                     new ExpressionNodeInfo(OperationType.Function,      "ceil") },
        { ExpressionNodeType.Clamp,                    new ExpressionNodeInfo(OperationType.Function,      "clamp") },
        { ExpressionNodeType.ColorHsl,                 new ExpressionNodeInfo(OperationType.Function,      "colorhsl") },
        { ExpressionNodeType.ColorRgb,                 new ExpressionNodeInfo(OperationType.Function,      "colorrgb") },
        { ExpressionNodeType.ColorLerp,                new ExpressionNodeInfo(OperationType.Function,      "colorlerp") },
        { ExpressionNodeType.ColorLerpHsl,             new ExpressionNodeInfo(OperationType.Function,      "colorlerphsl") },
        { ExpressionNodeType.ColorLerpRgb,             new ExpressionNodeInfo(OperationType.Function,      "colorlerprgb") },
        { ExpressionNodeType.Concatenate,              new ExpressionNodeInfo(OperationType.Function,      "concatenate") },
        { ExpressionNodeType.Distance,                 new ExpressionNodeInfo(OperationType.Function,      "distance") },
        { ExpressionNodeType.DistanceSquared,          new ExpressionNodeInfo(OperationType.Function,      "distancesquared") },
        { ExpressionNodeType.Floor,                    new ExpressionNodeInfo(OperationType.Function,      "floor") },
        { ExpressionNodeType.Inverse,                  new ExpressionNodeInfo(OperationType.Function,      "inverse") },
        { ExpressionNodeType.Length,                   new ExpressionNodeInfo(OperationType.Function,      "length") },
        { ExpressionNodeType.LengthSquared,            new ExpressionNodeInfo(OperationType.Function,      "lengthsquared") },
        { ExpressionNodeType.Lerp,                     new ExpressionNodeInfo(OperationType.Function,      "lerp") },
        { ExpressionNodeType.Ln,                       new ExpressionNodeInfo(OperationType.Function,      "ln") },
        { ExpressionNodeType.Log10,                    new ExpressionNodeInfo(OperationType.Function,      "log10") },
        { ExpressionNodeType.Max,                      new ExpressionNodeInfo(OperationType.Function,      "max") },
        { ExpressionNodeType.Min,                      new ExpressionNodeInfo(OperationType.Function,      "min") },
        { ExpressionNodeType.Modulus,                  new ExpressionNodeInfo(OperationType.Function,      "mod") },
        { ExpressionNodeType.Negate,                   new ExpressionNodeInfo(OperationType.Function,      "-") },
        { ExpressionNodeType.Normalize,                new ExpressionNodeInfo(OperationType.Function,      "normalize") },
        { ExpressionNodeType.Pow,                      new ExpressionNodeInfo(OperationType.Function,      "pow") },
        { ExpressionNodeType.QuaternionFromAxisAngle,  new ExpressionNodeInfo(OperationType.Function,      "quaternion.createfromaxisangle") },
        { ExpressionNodeType.Quaternion,               new ExpressionNodeInfo(OperationType.Function,      "quaternion") },
        { ExpressionNodeType.Round,                    new ExpressionNodeInfo(OperationType.Function,      "round") },
        { ExpressionNodeType.Scale,                    new ExpressionNodeInfo(OperationType.Function,      "scale") },
        { ExpressionNodeType.Sin,                      new ExpressionNodeInfo(OperationType.Function,      "sin") },
        { ExpressionNodeType.Slerp,                    new ExpressionNodeInfo(OperationType.Function,      "slerp") },
        { ExpressionNodeType.Sqrt,                     new ExpressionNodeInfo(OperationType.Function,      "sqrt") },
        { ExpressionNodeType.Square,                   new ExpressionNodeInfo(OperationType.Function,      "square") },
        { ExpressionNodeType.Tan,                      new ExpressionNodeInfo(OperationType.Function,      "tan") },
        { ExpressionNodeType.ToDegrees,                new ExpressionNodeInfo(OperationType.Function,      "todegrees") },
        { ExpressionNodeType.ToRadians,                new ExpressionNodeInfo(OperationType.Function,      "toradians") },
        { ExpressionNodeType.Transform,                new ExpressionNodeInfo(OperationType.Function,      "transform") },
        { ExpressionNodeType.Vector2,                  new ExpressionNodeInfo(OperationType.Function,      "vector2") },
        { ExpressionNodeType.Vector3,                  new ExpressionNodeInfo(OperationType.Function,      "vector3") },
    };
}
