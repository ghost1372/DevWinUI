using Microsoft.UI.Xaml.Automation.Peers;

namespace DevWinUI;

internal sealed partial class OrbitCardPeer : FrameworkElementAutomationPeer
{
    public OrbitCardPeer(OrbitCard owner) : base(owner) { }

    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.ListItem;

    protected override string GetClassNameCore() => nameof(OrbitCard);
}
