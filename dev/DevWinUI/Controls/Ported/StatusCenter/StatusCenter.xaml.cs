// Copyright (c) Files Community
// Licensed under the MIT License.

using WinRT;

namespace DevWinUI;

public sealed partial class StatusCenter : UserControl
{
	public StatusCenterViewModel ViewModel;

	public StatusCenter()
	{
		InitializeComponent();

		ViewModel = StatusCenterViewModel.Instance;

		Loaded += (_, _) => ViewModel.WindowHandle = WindowHelper.GetWindowHandle(this);
	}

	private void CloseAllItemsButton_Click(object sender, RoutedEventArgs e)
	{
		ViewModel.RemoveAllCompletedItems();
	}

	[DynamicWindowsRuntimeCast(typeof(Button))]
	private void CloseItemButton_Click(object sender, RoutedEventArgs e)
	{
		if (sender is Button button && button.DataContext is StatusCenterItem item)
			ViewModel.RemoveItem(item);
	}
}
