using Microsoft.UI.Xaml.Automation.Peers;

namespace DevWinUI;

public sealed class SegmentedMeterAutomationPeer : FrameworkElementAutomationPeer
{
    private readonly SegmentedMeter _owner;

    public SegmentedMeterAutomationPeer(SegmentedMeter owner)
        : base(owner)
    {
        _owner = owner;
    }

    protected override string GetClassNameCore()
    {
        return nameof(SegmentedMeter);
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.Group;
    }

    protected override string GetNameCore()
    {
        string name = base.GetNameCore();
        return string.IsNullOrEmpty(name) ? _owner.GetSummary() : name;
    }
}
