using Microsoft.UI.Xaml.Automation.Peers;

namespace DevWinUI;

public sealed partial class ReasoningTrigger : Button
{
    internal bool IsExpanded;
    internal bool IsLeaf;
    internal Action<bool>? RequestOpen;

    public ReasoningTrigger()
    {
        DefaultStyleKey = typeof(ReasoningTrigger);
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new ReasoningTriggerPeer(this);

    internal void RaiseState(bool oldValue, bool newValue)
    {
        if (FrameworkElementAutomationPeer.FromElement(this) is ReasoningTriggerPeer peer) peer.Raise(oldValue, newValue);
    }
}
