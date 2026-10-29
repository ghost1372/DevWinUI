using Microsoft.UI.Xaml.Automation.Peers;

namespace DevWinUI;

public sealed partial class ScrambledTextAutomationPeer : FrameworkElementAutomationPeer
{
    private readonly ScrambledText _owner;

    public ScrambledTextAutomationPeer(ScrambledText owner)
        : base(owner)
    {
        _owner = owner;
    }

    protected override string GetClassNameCore() => nameof(ScrambledText);

    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Text;

    protected override string GetNameCore()
    {
        string name = base.GetNameCore();
        return string.IsNullOrEmpty(name) ? _owner.Text : name;
    }
}
