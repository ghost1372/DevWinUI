// Copyright (c) Files Community
// Licensed under the MIT License.

using WinRT.Interop;

namespace DevWinUI;

internal static partial class StatusCenterTaskbarProgress
{
    private static readonly Dictionary<IntPtr, WindowTaskbarProgress> _windows = [];
    private static readonly object _sync = new();

    public static void Register(StatusCenterViewModel viewModel, IntPtr windowHandle)
    {
        lock (_sync)
        {
            if (!_windows.TryGetValue(windowHandle, out var windowProgress))
            {
                windowProgress = new WindowTaskbarProgress(windowHandle);
                _windows[windowHandle] = windowProgress;
            }

            windowProgress.ViewModels.Add(viewModel);
            Report(windowHandle);
        }
    }

    public static void Unregister(StatusCenterViewModel viewModel, IntPtr windowHandle)
    {
        lock (_sync)
        {
            if (_windows.TryGetValue(windowHandle, out var windowProgress) && windowProgress.ViewModels.Remove(viewModel) && windowProgress.ViewModels.Count == 0)
                _windows.Remove(windowHandle);
        }
    }

    public static void Report(IntPtr windowHandle)
    {
        lock (_sync)
        {
            if (!_windows.TryGetValue(windowHandle, out var windowProgress))
                return;

            var allItems = windowProgress.ViewModels
                .SelectMany(vm => vm.StatusCenterItems)
                .ToList();

            var inProgressItems = allItems
                .Where(item => item.IsInProgress)
                .ToList();

            var activeItems = inProgressItems.Where(item => !item.IsPaused).ToList();
            var determinateActiveValues = activeItems
                .Where(item => !item.IsIndeterminateProgress)
                .Select(item => (double)item.ProgressPercentage)
                .ToList();

            var determinateValues = inProgressItems
                .Where(item => !item.IsIndeterminateProgress)
                .Select(item => (double)item.ProgressPercentage)
                .ToList();

            try
            {
                if (activeItems.Count > 0)
                {
                    windowProgress.HasError = false;
                    if (determinateActiveValues.Count > 0)
                    {
                        TaskbarHelper.SetProgressState(windowHandle, TaskbarStates.Normal);
                        TaskbarHelper.SetProgressValue(windowHandle, (int)determinateActiveValues.Average(), 100);
                    }
                    else
                    {
                        TaskbarHelper.SetProgressState(windowHandle, TaskbarStates.Indeterminate);
                    }
                }
                else if (inProgressItems.Count > 0)
                {
                    windowProgress.HasError = false;
                    TaskbarHelper.SetProgressState(windowHandle, TaskbarStates.Paused);
                    if (determinateValues.Count > 0)
                        TaskbarHelper.SetProgressValue(windowHandle, (int)determinateValues.Average(), 100);
                }
                else if (allItems.Any(item => item.OperationReturnResult is ReturnResult.Failed or ReturnResult.Cancelled))
                {
                    if (!windowProgress.HasError)
                        TaskbarHelper.FlashWindow(windowHandle);

                    windowProgress.HasError = true;
                    TaskbarHelper.SetProgressValue(windowHandle, 100, 100);
                    TaskbarHelper.SetProgressState(windowHandle, TaskbarStates.Error);
                }
                else
                {
                    windowProgress.HasError = false;
                    TaskbarHelper.SetProgressState(windowHandle, TaskbarStates.NoProgress);
                }
            }
            catch
            {
                // Ignore taskbar update failures to avoid interrupting status updates.
            }
        }
    }

    private sealed partial class WindowTaskbarProgress
    {
        public HashSet<StatusCenterViewModel> ViewModels { get; } = [];

        public bool HasError { get; set; }

        public WindowTaskbarProgress(IntPtr windowHandle)
        {
            var window = WindowHelper.ActiveWindows
                .Select(item => item.Window)
                .FirstOrDefault(w => WindowNative.GetWindowHandle(w) == windowHandle);

            if (window is null)
                return;

            window.Closed += (_, _) => RemoveWindow(windowHandle);
        }

        private static void RemoveWindow(IntPtr windowHandle)
        {
            lock (_sync)
                _windows.Remove(windowHandle);
        }
    }
}
