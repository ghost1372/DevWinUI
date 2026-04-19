// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using Microsoft.UI.Xaml.Automation.Peers;

namespace DevWinUI;

public partial class SimpleSettingsGroupAutomationPeer : FrameworkElementAutomationPeer
{
    public SimpleSettingsGroupAutomationPeer(SimpleSettingsGroup owner)
        : base(owner)
    {
    }

    protected override string GetNameCore()
    {
        var selectedSimpleSettingsGroup = (SimpleSettingsGroup)Owner;
        return selectedSimpleSettingsGroup.Header;
    }
}
