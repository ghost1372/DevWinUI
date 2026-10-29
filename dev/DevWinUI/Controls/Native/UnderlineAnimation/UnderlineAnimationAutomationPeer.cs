using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;

namespace DevWinUI;

internal sealed partial class UnderlineAnimationAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
{
    public UnderlineAnimationAutomationPeer(UnderlineAnimation owner) : base(owner) { }

    private UnderlineAnimation Control => (UnderlineAnimation)Owner;

    public void Invoke()
    {
        
    }

    protected override string GetClassNameCore() => nameof(UnderlineAnimation);

    protected override string GetNameCore()
    {
        string name = base.GetNameCore();
        return string.IsNullOrEmpty(name) ? Control.Text : name;
    }
}
