using Microsoft.UI.Xaml.Automation.Peers;

namespace DevWinUI;

internal sealed partial class HoldToConfirmPeer : FrameworkElementAutomationPeer
{
    private readonly HoldToConfirm _owner;

    public HoldToConfirmPeer(HoldToConfirm owner) : base(owner) => _owner = owner;

    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Button;

    protected override string GetClassNameCore() => nameof(HoldToConfirm);

    protected override bool IsEnabledCore() => base.IsEnabledCore() && !_owner.IsDone;
}
