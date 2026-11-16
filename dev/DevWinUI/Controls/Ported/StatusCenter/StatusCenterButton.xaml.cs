// Copyright (c) Files Community
// Licensed under the MIT License.

using System.ComponentModel;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace DevWinUI;

public sealed partial class StatusCenterButton : Control
{
	public static readonly DependencyProperty FlyoutPlacementProperty =
		DependencyProperty.Register(nameof(FlyoutPlacement), typeof(FlyoutPlacementMode), typeof(StatusCenterButton), new PropertyMetadata(FlyoutPlacementMode.BottomEdgeAlignedRight));

	public static readonly DependencyProperty ShowProgressRingProperty =
		DependencyProperty.Register(nameof(ShowProgressRing), typeof(bool), typeof(StatusCenterButton), new PropertyMetadata(false));

	public static readonly DependencyProperty AverageOperationProgressValueProperty =
		DependencyProperty.Register(nameof(AverageOperationProgressValue), typeof(double), typeof(StatusCenterButton), new PropertyMetadata(0d));

	public static readonly DependencyProperty InfoBadgeValueProperty =
		DependencyProperty.Register(nameof(InfoBadgeValue), typeof(int), typeof(StatusCenterButton), new PropertyMetadata(-1));

	public static readonly DependencyProperty InfoBadgeStateProperty =
		DependencyProperty.Register(nameof(InfoBadgeState), typeof(int), typeof(StatusCenterButton), new PropertyMetadata(0));

	private bool _isOpen;

	private Button? _showStatusCenterButton;
	private Flyout? _statusCenterFlyout;

	private StatusCenterViewModel ViewModel { get; } = new();

	public FlyoutPlacementMode FlyoutPlacement
	{
		get => (FlyoutPlacementMode)GetValue(FlyoutPlacementProperty);
		set => SetValue(FlyoutPlacementProperty, value);
	}

	public bool ShowProgressRing
	{
		get => (bool)GetValue(ShowProgressRingProperty);
		private set => SetValue(ShowProgressRingProperty, value);
	}

	public double AverageOperationProgressValue
	{
		get => (double)GetValue(AverageOperationProgressValueProperty);
		private set => SetValue(AverageOperationProgressValueProperty, value);
	}

	public int InfoBadgeValue
	{
		get => (int)GetValue(InfoBadgeValueProperty);
		private set => SetValue(InfoBadgeValueProperty, value);
	}

	public int InfoBadgeState
	{
		get => (int)GetValue(InfoBadgeStateProperty);
		private set => SetValue(InfoBadgeStateProperty, value);
	}

	public StatusCenterButton()
	{
		DefaultStyleKey = typeof(StatusCenterButton);

		ViewModel.PropertyChanged += OnViewModelPropertyChanged;
		Loaded += (_, _) => ViewModel.WindowHandle = WindowHelper.GetWindowHandle(this);

        ShowProgressRing = ViewModel.ShowProgressRing;
        AverageOperationProgressValue = ViewModel.AverageOperationProgressValue;
        InfoBadgeValue = ViewModel.InfoBadgeValue;
        InfoBadgeState = ViewModel.InfoBadgeState;
    }

	public SizeUnitTypes SizeUnitFormat
	{
		get => ViewModel.SizeUnitFormat;
		set => ViewModel.SizeUnitFormat = value;
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

	public static void UpdateCardStrings(StatusCenterItem card, StatusCenterItemProgressModel? progressValue = null)
		=> card.UpdateCardStrings(progressValue);

	public void Open()
	{
		if (_statusCenterFlyout is null || _showStatusCenterButton is null)
			return;

		if (_isOpen)
			_statusCenterFlyout.Hide();
		else
			_statusCenterFlyout.ShowAt(_showStatusCenterButton);
	}

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		_showStatusCenterButton = GetTemplateChild("ShowStatusCenterButton") as Button;
		_statusCenterFlyout = _showStatusCenterButton?.Flyout as Flyout;

		if (_statusCenterFlyout is null)
			return;

		_statusCenterFlyout.Opened += StatusCenterFlyout_Opened;
		_statusCenterFlyout.Closed += StatusCenterFlyout_Closed;

		if (_statusCenterFlyout.Content is StatusCenter statusCenter)
			statusCenter.ViewModel = ViewModel;
	}

	private void StatusCenterFlyout_Opened(object sender, object e)
	{
		_isOpen = true;
		ViewModel.OnStatusCenterFlyoutOpened();
	}

	private void StatusCenterFlyout_Closed(object sender, object e)
		=> _isOpen = false;

	private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
			case nameof(StatusCenterViewModel.ShowProgressRing):
				ShowProgressRing = ViewModel.ShowProgressRing;
				break;
			case nameof(StatusCenterViewModel.AverageOperationProgressValue):
				AverageOperationProgressValue = ViewModel.AverageOperationProgressValue;
				break;
			case nameof(StatusCenterViewModel.InfoBadgeValue):
				InfoBadgeValue = ViewModel.InfoBadgeValue;
				break;
			case nameof(StatusCenterViewModel.InfoBadgeState):
				InfoBadgeState = ViewModel.InfoBadgeState;
				break;
		}
	}
}
