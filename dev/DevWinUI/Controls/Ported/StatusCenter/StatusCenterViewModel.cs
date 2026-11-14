// Copyright (c) Files Community
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.ComponentModel;

namespace DevWinUI;

public sealed partial class StatusCenterViewModel : ObservableObject
{
	public static StatusCenterViewModel Instance { get; } = new();

	internal IntPtr WindowHandle { get; set; }

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
	{
		try
		{
			if (HasAnyItemInProgress)
			{
				TaskbarHelper.SetProgressState(WindowHandle, TaskbarStates.Normal);
				TaskbarHelper.SetProgressValue(WindowHandle, Math.Clamp(AverageOperationProgressValue, 0, 100), 100);
			}
			else
			{
				TaskbarHelper.SetProgressState(WindowHandle, TaskbarStates.NoProgress);
			}
		}
		catch
		{
			// Ignore taskbar update failures to avoid interrupting status updates.
		}
	}
}
