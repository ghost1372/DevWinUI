using System.Runtime.CompilerServices;

namespace DevWinUI;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CA1063 // Implement IDisposable Correctly

internal abstract partial class ExpressionNode : IDisposable
{

    private List<ReferenceInfo> _objRefList = null;
    private Dictionary<CompositionObject, string>? _compObjToNodeNameMap = null;
    private Dictionary<string, object> _constParamMap = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);

    internal ExpressionNode()
    {
    }

    public void SetBooleanParameter(string parameterName, bool value)
    {
        _constParamMap[parameterName] = value;
    }

    public void Dispose()
    {
        _objRefList = null;
        this._compObjToNodeNameMap = null;
        _constParamMap = null;
        Subchannels = null;
        PropertyName = null;
        NodeType = ExpressionNodeType.Count;

        Children = null;

        if (ExpressionAnimation != null)
        {
            ExpressionAnimation.Dispose();
            ExpressionAnimation = null;
        }
    }

    internal static T CreateExpressionNode<T>()
        where T : ExpressionNode
    {
        T? newNode;

        if (typeof(T) == typeof(BooleanNode))
        {
            newNode = new BooleanNode() as T;
        }
        else if (typeof(T) == typeof(ScalarNode))
        {
            newNode = new ScalarNode() as T;
        }
        else if (typeof(T) == typeof(Vector2Node))
        {
            newNode = new Vector2Node() as T;
        }
        else if (typeof(T) == typeof(Vector3Node))
        {
            newNode = new Vector3Node() as T;
        }
        else if (typeof(T) == typeof(ColorNode))
        {
            newNode = new ColorNode() as T;
        }
        else
        {
            throw new Exception("unexpected type");
        }

        return newNode!;
    }

    internal string ToExpressionString()
    {
        if (_objRefList == null)
        {
            EnsureReferenceInfo();
        }

        return ToExpressionStringInternal();
    }

    internal void ClearReferenceInfo()
    {
        _objRefList = null!;
        this.NodeName = null!;
        foreach (var child in Children)
        {
            child.ClearReferenceInfo();
        }
    }

    internal void EnsureReferenceInfo()
    {
        if (_objRefList == null)
        {
            // Get all ReferenceNodes in this expression
            HashSet<ReferenceNode> referenceNodes = new HashSet<ReferenceNode>();
            PopulateParameterNodes(ref _constParamMap, ref referenceNodes);

            // Find all CompositionObjects across all referenceNodes that need a paramName to be created
            HashSet<CompositionObject> compObjects = new HashSet<CompositionObject>();
            foreach (var refNode in referenceNodes)
            {
                if ((refNode.Reference != null) && (refNode.GetReferenceNodeString() == null))
                {
                    compObjects.Add(refNode.Reference);
                }
            }

            // Create a map to store the generated paramNames for each CompObj
            this._compObjToNodeNameMap = new Dictionary<CompositionObject, string>();
            var paramCount = 0u;
            foreach (var compObj in compObjects)
            {
                string nodeName = !string.IsNullOrWhiteSpace(ParamName) ? ParamName : CreateUniqueNodeNameFromIndex(paramCount++);

                this._compObjToNodeNameMap.Add(compObj, nodeName);
            }

            // Go through all reference nodes again to create our full list of referenceInfo. This time, if
            // the param name is null, look it up from our new map and store it.
            _objRefList = new List<ReferenceInfo>();
            foreach (var refNode in referenceNodes)
            {
                string nodeName = refNode.GetReferenceNodeString();

                if ((refNode.Reference == null) && (nodeName == null))
                {
                    // This can't happen - if the ref is null it must be because it's a named param
                    throw new Exception($"{nameof(refNode.Reference)} and {nameof(nodeName)} can't both be null");
                }

                if (nodeName == null)
                {
                    nodeName = this._compObjToNodeNameMap[refNode.Reference!];
                }

                _objRefList.Add(new ReferenceInfo(nodeName, refNode.Reference!));
                refNode.NodeName = nodeName;
            }
        }

        // Generates Excel-column-like identifiers, e.g. A, B, ..., Z, AA, BA...
        // This implementation aggregates characters in reverse order to avoid having to
        // precompute the exact number of characters in the resulting string. This is not
        // important in this context as the only critical property to maintain is to have
        // a unique mapping to each input value to the resulting sequence of letters.
        [SkipLocalsInit]
        static unsafe string CreateUniqueNodeNameFromIndex(uint i)
        {
            const int alphabetLength = 'Z' - 'A' + 1;

            // The total length of the resulting sequence is guaranteed to always
            // be less than 8, given that log26(4294967295) ≈ 6.8. In this case we
            // are just allocating the immediate next power of two following that.
            // Note: this is using a char* buffer instead of Span<char> as the latter
            // is not referenced here, and we don't want to pull in an extra package.
            char* characters = stackalloc char[8];

            characters[0] = (char)('A' + (i % alphabetLength));

            int totalCharacters = 1;

            while ((i /= alphabetLength) > 0)
            {
                i--;

                characters[totalCharacters++] = (char)('A' + (i % alphabetLength));
            }

            return new string(characters, 0, totalCharacters);
        }
    }

    internal void SetAllParameters(CompositionAnimation animation)
    {
        // Make sure the list is populated
        EnsureReferenceInfo();

        foreach (var refInfo in _objRefList)
        {
            animation.SetReferenceParameter(refInfo.ParameterName, refInfo.CompObject);
        }

        foreach (var constParam in _constParamMap)
        {
            if (constParam.Value.GetType() == typeof(bool))
            {
                animation.SetBooleanParameter(constParam.Key, (bool)constParam.Value);
            }
            else if (constParam.Value.GetType() == typeof(float))
            {
                animation.SetScalarParameter(constParam.Key, (float)constParam.Value);
            }
            else if (constParam.Value.GetType() == typeof(Vector2))
            {
                animation.SetVector2Parameter(constParam.Key, (Vector2)constParam.Value);
            }
            else if (constParam.Value.GetType() == typeof(Vector3))
            {
                animation.SetVector3Parameter(constParam.Key, (Vector3)constParam.Value);
            }
            else if (constParam.Value.GetType() == typeof(Color))
            {
                animation.SetColorParameter(constParam.Key, (Color)constParam.Value);
            }
            else
            {
                throw new Exception($"Unexpected constant parameter datatype ({constParam.Value.GetType()})");
            }
        }
    }

    protected internal abstract string GetValue();

    protected internal T SubchannelsInternal<T>(params string[] subchannels)
        where T : ExpressionNode
    {
        ExpressionNodeType swizzleNodeType = ExpressionNodeType.Swizzle;
        T newNode;

        switch (subchannels.GetLength(0))
        {
            case 1:
                newNode = ExpressionFunctions.Function<ScalarNode>(swizzleNodeType, this) as T;
                break;

            case 2:
                newNode = ExpressionFunctions.Function<Vector2Node>(swizzleNodeType, this) as T;
                break;

            case 3:
                newNode = ExpressionFunctions.Function<Vector3Node>(swizzleNodeType, this) as T;
                break;

            default:
                throw new Exception($"Invalid subchannel count ({subchannels.GetLength(0)})");
        }

        (newNode as ExpressionNode)!.Subchannels = subchannels;

        return newNode;
    }

    protected internal void PopulateParameterNodes(ref Dictionary<string, object> constParamMap, ref HashSet<ReferenceNode> referenceNodes)
    {
        var refNode = this as ReferenceNode;
        if ((refNode != null) && (refNode.NodeType != ExpressionNodeType.TargetReference))
        {
            referenceNodes.Add(refNode);
        }

        if ((_constParamMap != null) && (_constParamMap != constParamMap))
        {
            foreach (var entry in _constParamMap)
            {
                // If this parameter hasn't already been set on the root, use this node's parameter info
                if (!constParamMap.ContainsKey(entry.Key))
                {
                    constParamMap[entry.Key] = entry.Value;
                }
            }
        }

        foreach (var child in Children)
        {
            child.PopulateParameterNodes(ref constParamMap, ref referenceNodes);
        }
    }
    private OperationType GetOperationKind()
    {
        return ExpressionFunctions.GetNodeInfoFromType(NodeType).NodeOperationKind;
    }
    private string GetOperationString()
    {
        return ExpressionFunctions.GetNodeInfoFromType(NodeType).OperationString;
    }

    private string ToExpressionStringInternal()
    {
        string ret;

        // Do a recursive depth-first traversal of the node tree to print out the full expression string
        switch (GetOperationKind())
        {
            case OperationType.Function:
                if (Children.Count == 0)
                {
                    throw new Exception("Can't have an expression function with no params");
                }

                ret = $"{GetOperationString()}({Children[0].ToExpressionStringInternal()}";
                for (int i = 1; i < Children.Count; i++)
                {
                    ret += "," + Children[i].ToExpressionStringInternal();
                }

                ret += ")";
                break;

            case OperationType.Operator:
                if (Children.Count != 2)
                {
                    throw new Exception("Can't have an operator that doesn't have 2 exactly params");
                }

                ret = $"({Children[0].ToExpressionStringInternal()} {GetOperationString()} {Children[1].ToExpressionStringInternal()})";
                break;

            case OperationType.UnaryOperator:
                if (Children.Count != 1)
                {
                    throw new Exception("Can't have an unary operator that doesn't have exactly one params");
                }

                ret = $"( {GetOperationString()} {Children[0].ToExpressionStringInternal()} )";
                break;

            case OperationType.Constant:
                if (Children.Count == 0)
                {
                    // If a parameterName was specified, use it. Otherwise write the value.
                    ret = ParamName ?? GetValue();
                }
                else
                {
                    throw new Exception("Constants must have 0 children");
                }

                break;

            case OperationType.Swizzle:
                if (Children.Count != 1)
                {
                    throw new Exception("Swizzles should have exactly 1 child");
                }

                string swizzleString = string.Empty;
                foreach (var sub in Subchannels!)
                {
                    swizzleString += sub;
                }

                ret = $"{Children[0].ToExpressionStringInternal()}.{swizzleString}";
                break;

            case OperationType.Reference:
                if ((NodeType == ExpressionNodeType.Reference) ||
                    (NodeType == ExpressionNodeType.TargetReference))
                {
                    // This is the reference node itself
                    if (Children.Count != 0)
                    {
                        throw new Exception("References cannot have children");
                    }

                    ret = (this as ReferenceNode)!.GetReferenceNodeString();
                }
                else if (NodeType == ExpressionNodeType.ReferenceProperty)
                {
                    // This is the property node of the reference
                    if (Children.Count != 1)
                    {
                        throw new Exception("Reference properties must have exactly one child");
                    }

                    if (PropertyName == null)
                    {
                        throw new Exception("Reference properties must have a property name");
                    }

                    ret = $"{Children[0].ToExpressionStringInternal()}.{PropertyName}";
                }
                else if (NodeType == ExpressionNodeType.StartingValueProperty)
                {
                    // This is a "this.StartingValue" node
                    if (Children.Count != 0)
                    {
                        throw new Exception("StartingValue references Cannot have children");
                    }

                    ret = "this.StartingValue";
                }
                else if (NodeType == ExpressionNodeType.CurrentValueProperty)
                {
                    // This is a "this.CurrentValue" node
                    if (Children.Count != 0)
                    {
                        throw new Exception("CurrentValue references Cannot have children");
                    }

                    ret = "this.CurrentValue";
                }
                else
                {
                    throw new Exception("Unexpected NodeType for OperationType.Reference");
                }

                break;

            case OperationType.Conditional:
                if (Children.Count != 3)
                {
                    throw new Exception("Conditionals must have exactly 3 children");
                }

                ret = $"(({Children[0].ToExpressionStringInternal()}) ? ({Children[1].ToExpressionStringInternal()}) : ({Children[2].ToExpressionStringInternal()}))";
                break;

            default:
                throw new Exception($"Unexpected operation type ({GetOperationKind()}), nodeType = {NodeType}");
        }

        return ret;
    }


    internal struct ReferenceInfo
    {
        public ReferenceInfo(string paramName, CompositionObject compObj)
        {
            ParameterName = paramName;
            CompObject = compObj;
        }

        public string ParameterName { get; set; }

        public CompositionObject CompObject { get; set; }
    }

    internal string PropertyName { get; set; }

    internal ExpressionNodeType NodeType { get; set; }

    internal List<ExpressionNode> Children { get; set; } = new List<ExpressionNode>();

    internal string ParamName { get; set; }

    internal string NodeName { get; set; }

    internal ExpressionAnimation ExpressionAnimation { get; set; }

    protected internal string[]? Subchannels { get; set; }
}

#pragma warning restore CS8625 
#pragma warning restore CS8600
#pragma warning restore CA1063 
