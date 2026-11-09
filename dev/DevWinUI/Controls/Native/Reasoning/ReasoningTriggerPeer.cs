using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;

namespace DevWinUI;

internal sealed partial class ReasoningTriggerPeer : ButtonAutomationPeer, IExpandCollapseProvider
{
    readonly ReasoningTrigger _owner;

    public ReasoningTriggerPeer(ReasoningTrigger owner) : base(owner) => _owner = owner;

    public ExpandCollapseState ExpandCollapseState => State(_owner.IsExpanded);

    protected override object GetPatternCore(PatternInterface patternInterface) =>
        patternInterface == PatternInterface.ExpandCollapse ? this : base.GetPatternCore(patternInterface);

    public void Expand() => _owner.RequestOpen?.Invoke(true);

    public void Collapse() => _owner.RequestOpen?.Invoke(false);

    internal void Raise(bool oldValue, bool newValue) =>
        RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, State(oldValue), State(newValue));

    ExpandCollapseState State(bool expanded) =>
        _owner.IsLeaf ? ExpandCollapseState.LeafNode : expanded ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
}
