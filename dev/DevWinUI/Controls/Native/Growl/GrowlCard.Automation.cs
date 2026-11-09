using Microsoft.UI.Xaml.Automation.Peers;

namespace DevWinUI;

public sealed partial class GrowlCard
{
    private bool _announced;

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new GrowlCardAutomationPeer(this);
    }

    internal string GetAutomationText()
    {
        bool hasTitle = !string.IsNullOrEmpty(Title);
        bool hasSubTitle = !string.IsNullOrEmpty(SubTitle);

        return hasTitle && hasSubTitle ? $"{Title}. {SubTitle}" : hasTitle ? Title : SubTitle;
    }

    private void Announce()
    {
        if (_announced)
        {
            return;
        }

        _announced = true;

        if (FrameworkElementAutomationPeer.FromElement(this) is { } peer)
        {
            peer.RaiseNotificationEvent(AutomationNotificationKind.ActionCompleted, AutomationNotificationProcessing.ImportantMostRecent, GetAutomationText(), nameof(GrowlCard));
        }
    }
}
