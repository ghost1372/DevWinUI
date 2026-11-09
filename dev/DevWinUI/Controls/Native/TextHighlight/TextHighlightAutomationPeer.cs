using Microsoft.UI.Xaml.Automation.Peers;

namespace DevWinUI;

internal sealed partial class TextHighlightAutomationPeer : FrameworkElementAutomationPeer
{
    public TextHighlightAutomationPeer(TextHighlight owner) : base(owner) { }

    protected override string GetClassNameCore() => nameof(TextHighlight);

    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Text;

    protected override string GetNameCore()
    {
        string name = base.GetNameCore();
        return string.IsNullOrEmpty(name) ? ((TextHighlight)Owner).Text : name;
    }
}
