// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;

namespace DevWinUI;

internal partial class TimelineAutomationPeer : FrameworkElementAutomationPeer
{
    public TimelineAutomationPeer(Timeline owner)
        : base(owner)
    {
    }

    protected override string GetClassNameCore() => nameof(Timeline);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    protected override string GetAutomationIdCore()
    {
        var owner = (Timeline)Owner;
        var id = AutomationProperties.GetAutomationId(owner);
        return string.IsNullOrEmpty(id) ? base.GetAutomationIdCore() : id;
    }

    protected override string GetNameCore()
    {
        var owner = (Timeline)Owner;
        var name = AutomationProperties.GetName(owner);
        return !string.IsNullOrEmpty(name)
            ? name
            : $"Timeline from {owner.StartTime} to {owner.EndTime}";
    }
}
