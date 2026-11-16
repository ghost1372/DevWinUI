// Copyright (c) Files Community
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls.Primitives;

namespace DevWinUI;

public sealed partial class StatusCenterButton : UserControl
{
	public static readonly DependencyProperty FlyoutPlacementProperty =
		DependencyProperty.Register(nameof(FlyoutPlacement), typeof(FlyoutPlacementMode), typeof(StatusCenterButton), new PropertyMetadata(FlyoutPlacementMode.BottomEdgeAlignedRight, OnFlyoutPlacementChanged));

	private bool _isOpen;

	private StatusCenterViewModel ViewModel { get; } = new();

	public FlyoutPlacementMode FlyoutPlacement
	{
		get => (FlyoutPlacementMode)GetValue(FlyoutPlacementProperty);
		set => SetValue(FlyoutPlacementProperty, value);
	}

	public StatusCenterButton()
	{
		InitializeComponent();

		StatusCenterFlyout.Placement = FlyoutPlacement;
		StatusCenterFlyout.Opened += (_, _) => _isOpen = true;
		StatusCenterFlyout.Closed += (_, _) => _isOpen = false;
        OngoingTasksControl?.ViewModel = ViewModel;
        Loaded += (_, _) => ViewModel.WindowHandle = WindowHelper.GetWindowHandle(this);
	}

	public StatusCenterItem AddCard(
		ReturnResult status,
		string header,
		string subHeader,
		string message,
		StatusCenterItemIconKind iconKind,
		bool canProvideProgress = true,
		long itemsCount = 0,
		long totalSize = 0,
		Func<StatusCenterItem, StatusCenterItemProgressModel, (string Header, string SubHeader, string Message)>? update = null)
		=> ViewModel.AddCard(status, header, subHeader, message, iconKind, canProvideProgress, itemsCount, totalSize, update);

	public bool RemoveItem(StatusCenterItem item)
		=> ViewModel.RemoveItem(item);

	public void RemoveAllCompletedItems()
		=> ViewModel.RemoveAllCompletedItems();

	public void UpdateCardStrings(StatusCenterItem card, StatusCenterItemProgressModel? progressValue = null)
		=> card.UpdateCardStrings(progressValue);

	public void Open()
	{
		if (_isOpen)
			StatusCenterFlyout.Hide();
		else
			StatusCenterFlyout.ShowAt(ShowStatusCenterButton);
	}

	private static void OnFlyoutPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is StatusCenterButton { StatusCenterFlyout: not null } statusCenterButton)
			statusCenterButton.StatusCenterFlyout.Placement = (FlyoutPlacementMode)e.NewValue;
	}
}
