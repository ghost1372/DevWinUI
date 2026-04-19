// Copyright (c) Files Community
// Licensed under the MIT License.

namespace DevWinUI;

public partial class ToolbarRadioButton : RadioButton, IToolbarItemSet
{
	public ToolbarRadioButton()
	{
		DefaultStyleKey = typeof(ToolbarRadioButton);
	}

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
	}
}
