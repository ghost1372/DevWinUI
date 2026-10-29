using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;

namespace DevWinUI;

internal sealed partial class UnderlineAnimationAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
{
    public UnderlineAnimationAutomationPeer(UnderlineAnimation owner) : base(owner) { }

    private UnderlineAnimation Control => (UnderlineAnimation)Owner;

    protected override string GetClassNameCore() => nameof(UnderlineAnimation);

    protected override AutomationControlType GetAutomationControlTypeCore() =>
        Control.IsInteractive ? AutomationControlType.Hyperlink : AutomationControlType.Text;

    protected override string GetNameCore()
    {
        string name = base.GetNameCore();
        return string.IsNullOrEmpty(name) ? Control.Text : name;
    }

    protected override object? GetPatternCore(PatternInterface patternInterface) =>
        patternInterface == PatternInterface.Invoke && Control.IsInteractive
            ? this
            : base.GetPatternCore(patternInterface);

    public void Invoke()
    {
        UnderlineAnimation control = Control;
        control.DispatcherQueue.TryEnqueue(control.Activate);
    }
}
