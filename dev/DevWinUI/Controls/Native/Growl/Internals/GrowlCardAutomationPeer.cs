using Microsoft.UI.Xaml.Automation.Peers;

namespace DevWinUI;

internal sealed partial class GrowlCardAutomationPeer : FrameworkElementAutomationPeer
{
    public GrowlCardAutomationPeer(GrowlCard owner) : base(owner)
    {
    }

    private GrowlCard Card => (GrowlCard)Owner;

    protected override string GetClassNameCore()
    {
        return nameof(GrowlCard);
    }

    protected override string GetNameCore()
    {
        var name = Card.GetAutomationText();

        return string.IsNullOrEmpty(name) ? base.GetNameCore() : name;
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.Group;
    }
}
