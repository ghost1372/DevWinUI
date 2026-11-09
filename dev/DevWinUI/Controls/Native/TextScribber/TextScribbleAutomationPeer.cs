using Microsoft.UI.Xaml.Automation.Peers;

namespace DevWinUI;

internal sealed partial class TextScribbleAutomationPeer : FrameworkElementAutomationPeer
{
    public TextScribbleAutomationPeer(TextScribble owner) : base(owner) { }

    protected override string GetClassNameCore() => nameof(TextScribble);

    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Text;

    protected override string GetNameCore()
    {
        string name = base.GetNameCore();
        return string.IsNullOrEmpty(name) ? ((TextScribble)Owner).Text : name;
    }
}
