// Copyright (c) Files Community
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.ComponentModel;

namespace DevWinUI;

internal sealed partial class StatusCenterViewModel : ObservableObject
{
	private IntPtr _windowHandle;

	internal IntPtr WindowHandle
	{
		get => _windowHandle;
		set
		{
			if (_windowHandle == value)
				return;

			StatusCenterTaskbarProgress.Unregister(this, _windowHandle);
			_windowHandle = value;

			if (value != IntPtr.Zero)
				StatusCenterTaskbarProgress.Register(this, value);
		}
	}

	public ObservableCollection<StatusCenterItem> StatusCenterItems { get; } = [];

	private int _AverageOperationProgressValue = 0;
	public int AverageOperationProgressValue
	{
		get => _AverageOperationProgressValue;
		private set => SetProperty(ref _AverageOperationProgressValue, value);
	}

	public int InProgressItemCount
	{
		get
		{
			int count = 0;

			foreach (var item in StatusCenterItems)
			{
				if (item.IsInProgress)
					count++;
			}

			return count;
		}
	}

	public bool HasAnyItemInProgress
	{
		get
		{
			if (InProgressItemCount > 0)
				ShowProgressRing = true;

			return InProgressItemCount > 0;
		}
	}

	public bool HasAnyItem
		=> StatusCenterItems.Any();


	private bool _ShowProgressRing = false;
	public bool ShowProgressRing
	{
		get => _ShowProgressRing;
		private set => SetProperty(ref _ShowProgressRing, value);

	}

	public int InfoBadgeState
	{
		get
		{
			var anyFailure = StatusCenterItems.Any(i =>
				i.FileSystemOperationReturnResult != ReturnResult.InProgress &&
				i.FileSystemOperationReturnResult != ReturnResult.Success);

			var anyActiveOperation = StatusCenterItems.Any(i => i.IsInProgress && !i.IsPaused);
			var anyPausedOperation = StatusCenterItems.Any(i => i.IsInProgress && i.IsPaused);

			return (anyFailure, anyActiveOperation, anyPausedOperation) switch
			{
				(false, false, false) => 0,     // All successful
				(false, true, _) => 1,          // In progress
				(true, true, _) => 2,           // In progress with an error
				(true, false, false) => 3,      // Completed with an error
				(_, false, true) => 4           // Paused
			};
		}
	}

	public int InfoBadgeValue
		=> InProgressItemCount > 0 ? InProgressItemCount : -1;

	public event EventHandler<StatusCenterItem>? NewItemAdded;

	public StatusCenterViewModel()
	{
		StatusCenterItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(HasAnyItem));
	}

	public void OnStatusCenterFlyoutOpened()
	{
		ShowProgressRing = HasAnyItemInProgress || InfoBadgeState == 3;
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
	{
		var item = AddItem(
			header,
			subHeader,
			status,
			canProvideProgress,
			itemsCount,
			totalSize,
			status is ReturnResult.InProgress ? new CancellationTokenSource() : null);

		if (status is ReturnResult.InProgress or ReturnResult.Cancelled)
			item.ItemIconKind = iconKind;

		item.Message = message;
		item.Progress.ItemsCount = itemsCount;
		item.Progress.TotalSize = totalSize;
		item.UpdateStrings = update;

		return item;
	}

	public StatusCenterItem AddItem(
		string header,
		string subHeader,
		ReturnResult status,
		bool canProvideProgress = true,
		long itemsCount = 0,
		long totalSize = 0,
		CancellationTokenSource? cancellationTokenSource = null)
	{
		var banner = new StatusCenterItem(
			this,
			header,
			subHeader,
			status,
			canProvideProgress,
			itemsCount,
			totalSize,
			cancellationTokenSource);

		StatusCenterItems.Insert(0, banner);
		NewItemAdded?.Invoke(this, banner);

		NotifyChanges();

		return banner;
	}

	public bool RemoveItem(StatusCenterItem card)
	{
		if (!StatusCenterItems.Contains(card))
			return false;

		StatusCenterItems.Remove(card);

		NotifyChanges();

		return true;
	}

	public void RemoveAllCompletedItems()
	{
		for (var i = StatusCenterItems.Count - 1; i >= 0; i--)
		{
			if (!StatusCenterItems[i].IsInProgress)
				StatusCenterItems.RemoveAt(i);
		}

		NotifyChanges();
	}

	public void NotifyChanges()
	{
		OnPropertyChanged(nameof(InProgressItemCount));
		OnPropertyChanged(nameof(HasAnyItemInProgress));
		OnPropertyChanged(nameof(HasAnyItem));
		OnPropertyChanged(nameof(InfoBadgeState));
		OnPropertyChanged(nameof(InfoBadgeValue));

		UpdateAverageProgressValue();
		UpdateTaskbarProgress();
	}

	public void UpdateAverageProgressValue()
	{
		if (HasAnyItemInProgress)
			AverageOperationProgressValue = (int)StatusCenterItems.Where((item) => item.IsInProgress).Average(x => x.ProgressPercentage);
		else
			AverageOperationProgressValue = 0;
	}

	private void UpdateTaskbarProgress()
		=> StatusCenterTaskbarProgress.Report(this, WindowHandle);
}
