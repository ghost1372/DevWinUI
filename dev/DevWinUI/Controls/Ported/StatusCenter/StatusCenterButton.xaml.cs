// Copyright (c) Files Community
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls.Primitives;

namespace DevWinUI;

public sealed partial class StatusCenterButton : UserControl
{
	private bool _isOpen;

	public static readonly DependencyProperty FlyoutPlacementProperty =
		DependencyProperty.Register(nameof(FlyoutPlacement), typeof(FlyoutPlacementMode), typeof(StatusCenterButton), new PropertyMetadata(FlyoutPlacementMode.BottomEdgeAlignedRight, OnFlyoutPlacementChanged));

	public StatusCenterViewModel ViewModel { get; } = StatusCenterViewModel.Instance;

	public FlyoutPlacementMode FlyoutPlacement
	{
		get => (FlyoutPlacementMode)GetValue(FlyoutPlacementProperty);
		set => SetValue(FlyoutPlacementProperty, value);
	}

	public void Open()
	{
		if (_isOpen)
			StatusCenterFlyout.Hide();
		else
			StatusCenterFlyout.ShowAt(ShowStatusCenterButton);
	}

	public StatusCenterButton()
	{
		InitializeComponent();

		StatusCenterFlyout.Placement = FlyoutPlacement;
		StatusCenterFlyout.Opened += (_, _) => _isOpen = true;
		StatusCenterFlyout.Closed += (_, _) => _isOpen = false;

		Loaded += (_, _) => ViewModel.WindowHandle = WindowHelper.GetWindowHandle(this);
	}

	private static void OnFlyoutPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is StatusCenterButton { StatusCenterFlyout: not null } statusCenterButton)
			statusCenterButton.StatusCenterFlyout.Placement = (FlyoutPlacementMode)e.NewValue;
	}
}
