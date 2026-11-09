using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;

namespace DevWinUI;

internal sealed partial class Timeline2ItemAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
{
    private readonly Timeline2Item _item;

    public Timeline2ItemAutomationPeer(Timeline2Item owner) : base(owner)
    {
        _item = owner;
    }

    protected override string GetClassNameCore() => nameof(Timeline2Item);

    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.ListItem;

    protected override string GetNameCore()
    {
        string name = AutomationProperties.GetName(_item);
        if (!string.IsNullOrEmpty(name))
        {
            return name;
        }

        return _item.EntryTitle ?? base.GetNameCore();
    }

    protected override object GetPatternCore(PatternInterface patternInterface)
    {
        return patternInterface == PatternInterface.Invoke && _item.IsClickable
            ? this
            : base.GetPatternCore(patternInterface);
    }

    public void Invoke() => _item.Invoke();
}
