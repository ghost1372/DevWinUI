using Microsoft.UI.Xaml.Automation.Peers;

namespace DevWinUI;

public sealed partial class AnimatedTocAutomationPeer : FrameworkElementAutomationPeer
{
    private readonly AnimatedToc _owner;

    public AnimatedTocAutomationPeer(AnimatedToc owner)
        : base(owner)
    {
        _owner = owner;
    }

    protected override string GetClassNameCore() => nameof(AnimatedToc);

    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Group;

    protected override string GetNameCore()
    {
        string name = base.GetNameCore();
        return string.IsNullOrEmpty(name) ? _owner.Title : name;
    }
}
