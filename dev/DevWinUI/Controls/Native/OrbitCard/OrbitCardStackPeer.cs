using Microsoft.UI.Xaml.Automation.Peers;

namespace DevWinUI;

internal sealed partial class OrbitCardStackPeer : FrameworkElementAutomationPeer
{
    public OrbitCardStackPeer(OrbitCardStack owner) : base(owner) { }

    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.List;

    protected override string GetClassNameCore() => nameof(OrbitCardStack);
}
