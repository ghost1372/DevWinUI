// Copyright (c) Files Community
// Licensed under the MIT License.

namespace DevWinUI;

public partial class ToolbarFlyoutButton : DropDownButton, IToolbarItemSet
{
	public ToolbarFlyoutButton()
	{
		this.DefaultStyleKey = typeof(ToolbarFlyoutButton);
	}

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
	}
}
