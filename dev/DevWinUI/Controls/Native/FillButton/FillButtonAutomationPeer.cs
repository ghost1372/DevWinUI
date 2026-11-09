using Microsoft.UI.Xaml.Automation.Peers;

namespace DevWinUI;

public sealed partial class FillButtonAutomationPeer : ButtonAutomationPeer
{
    private readonly FillButton _owner;

    public FillButtonAutomationPeer(FillButton owner)
        : base(owner)
    {
        _owner = owner;
    }

    protected override string GetClassNameCore() => nameof(FillButton);

    protected override AutomationControlType GetAutomationControlTypeCore() =>
        _owner.NavigateUri is null ? base.GetAutomationControlTypeCore() : AutomationControlType.Hyperlink;
}
