using Microsoft.UI.Xaml.Controls.Primitives;

namespace DevWinUIGallery.Views;

public sealed partial class StatusCenterPage : Page
{
    public StatusCenterPage()
    {
        InitializeComponent();
        CmbFlyoutPlacement.SelectedIndex = 0;
        CmbSizeUnit.SelectedIndex = 0;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);

        var viewModel = StatusCenterViewModel.Instance;

        for (var index = viewModel.StatusCenterItems.Count - 1; index >= 0; index--)
            viewModel.RemoveItem(viewModel.StatusCenterItems[index]);
    }

    private void CmbFlyoutPlacement_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CmbFlyoutPlacement.SelectedItem is ComboBoxItem { Tag: string placement } && Enum.TryParse(placement, out FlyoutPlacementMode flyoutPlacement))
            StatusCenterButtonSample.FlyoutPlacement = flyoutPlacement;
    }

    private void CmbSizeUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CmbSizeUnit.SelectedItem is ComboBoxItem { Tag: string sizeUnit } && Enum.TryParse(sizeUnit, out SizeUnitTypes sizeUnitType))
            StatusCenterHelper.SizeUnitFormat = sizeUnitType;
    }

    private async void Copy_Click(object sender, RoutedEventArgs e)
    {
        string[] source =
        [
            @"C:\Users\Public\Documents\File 1.txt",
            @"C:\Users\Public\Documents\File 2.txt",
            @"C:\Users\Public\Documents\File 3.txt"
        ];
        string[] destination =
        [
            @"C:\Users\Public\Downloads\File 1.txt",
            @"C:\Users\Public\Downloads\File 2.txt",
            @"C:\Users\Public\Downloads\File 3.txt"
        ];
        var totalSize = source.Length * 64L * 1024 * 1024;
        var banner = AddCard_Copy(ReturnResult.InProgress, source, destination, source.Length, totalSize);

        var result = await SimulateFileOperationAsync(banner.ProgressEventSource, source, totalSize, banner.CancellationToken);

        StatusCenterViewModel.Instance.RemoveItem(banner);
        AddCard_Copy(result, source, destination, banner.TotalItemsCount, totalSize);
    }

    private async void Move_Click(object sender, RoutedEventArgs e)
    {
        string[] source =
        [
            @"C:\Users\Public\Documents\File 1.txt",
            @"C:\Users\Public\Documents\File 2.txt",
            @"C:\Users\Public\Documents\File 3.txt",
            @"C:\Users\Public\Documents\File 4.txt"
        ];
        string[] destination =
        [
            @"C:\Users\Public\Downloads\File 1.txt",
            @"C:\Users\Public\Downloads\File 2.txt",
            @"C:\Users\Public\Downloads\File 3.txt",
            @"C:\Users\Public\Downloads\File 4.txt"
        ];
        var totalSize = source.Length * 64L * 1024 * 1024;
        var banner = AddCard_Move(ReturnResult.InProgress, source, destination, source.Length, totalSize);

        var result = await SimulateFileOperationAsync(banner.ProgressEventSource, source, totalSize, banner.CancellationToken);

        StatusCenterViewModel.Instance.RemoveItem(banner);
        AddCard_Move(result, source, destination, banner.TotalItemsCount, totalSize);
    }

    private async void Delete_Click(object sender, RoutedEventArgs e)
    {
        string[] source =
        [
            @"C:\Users\Public\Documents\File 1.txt",
            @"C:\Users\Public\Documents\File 2.txt"
        ];
        var banner = AddCard_Delete(ReturnResult.InProgress, source, source.Length);

        var result = await SimulateItemOperationAsync(banner.ProgressEventSource, source, banner.CancellationToken);

        StatusCenterViewModel.Instance.RemoveItem(banner);
        AddCard_Delete(result, source, banner.TotalItemsCount);
    }

    private async void Recycle_Click(object sender, RoutedEventArgs e)
    {
        string[] source =
        [
            @"C:\Users\Public\Documents\File 1.txt",
            @"C:\Users\Public\Documents\File 2.txt",
            @"C:\Users\Public\Documents\File 3.txt",
            @"C:\Users\Public\Documents\File 4.txt",
            @"C:\Users\Public\Documents\File 5.txt"
        ];
        var banner = AddCard_Recycle(ReturnResult.InProgress, source, source.Length);

        var result = await SimulateItemOperationAsync(banner.ProgressEventSource, source, banner.CancellationToken);

        StatusCenterViewModel.Instance.RemoveItem(banner);
        AddCard_Recycle(result, source, banner.TotalItemsCount);
    }

    private async void Compress_Click(object sender, RoutedEventArgs e)
    {
        string[] source =
        [
            @"C:\Users\Public\Documents\File 1.txt",
            @"C:\Users\Public\Documents\File 2.txt",
            @"C:\Users\Public\Documents\File 3.txt"
        ];
        string[] destination = [@"C:\Users\Public\Downloads\Archive.zip"];
        var totalSize = source.Length * 48L * 1024 * 1024;
        var banner = AddCard_Compress(source, destination, ReturnResult.InProgress, source.Length, totalSize);

        var result = await SimulateFileOperationAsync(banner.ProgressEventSource, source, totalSize, banner.CancellationToken);

        StatusCenterViewModel.Instance.RemoveItem(banner);
        AddCard_Compress(source, destination, result, source.Length, totalSize);
    }

    private async void Extract_Click(object sender, RoutedEventArgs e)
    {
        string[] source = [@"C:\Users\Public\Documents\Archive.zip"];
        string[] destination = [@"C:\Users\Public\Downloads\Archive"];
        const long totalSize = 256L * 1024 * 1024;
        var banner = AddCard_Decompress(source, destination, ReturnResult.InProgress, 1, totalSize);

        var result = await SimulateFileOperationAsync(banner.ProgressEventSource, source, totalSize, banner.CancellationToken);

        StatusCenterViewModel.Instance.RemoveItem(banner);
        AddCard_Decompress(source, destination, result, 1, totalSize);
    }

    private async void InstallFont_Click(object sender, RoutedEventArgs e)
    {
        const int fontCount = 2;

        var banner = AddCard_InstallFont([@"C:\Users\Public\Documents"], ReturnResult.InProgress, fontCount);

        var result = await SimulateIndeterminateOperationAsync(banner.CancellationToken);

        StatusCenterViewModel.Instance.RemoveItem(banner);
        AddCard_InstallFont([@"C:\Users\Public\Documents"], result, fontCount);
    }

    private async void EmptyRecycleBin_Click(object sender, RoutedEventArgs e)
    {
        var banner = AddCard_EmptyRecycleBin(ReturnResult.InProgress);

        var result = await SimulateIndeterminateOperationAsync(banner.CancellationToken);

        StatusCenterViewModel.Instance.RemoveItem(banner);
        AddCard_EmptyRecycleBin(result);
    }

    private async void GitClone_Click(object sender, RoutedEventArgs e)
        => await RunGitAsync(GitStatusCenterOperationKind.Clone, "https://github.com/ghost1372/DevWinUI");

    private async void GitFetch_Click(object sender, RoutedEventArgs e)
        => await RunGitAsync(GitStatusCenterOperationKind.Fetch, null);

    private async void GitPull_Click(object sender, RoutedEventArgs e)
        => await RunGitAsync(GitStatusCenterOperationKind.Pull, null);

    private async void GitPush_Click(object sender, RoutedEventArgs e)
        => await RunGitAsync(GitStatusCenterOperationKind.Push, null);

    private async void GitSync_Click(object sender, RoutedEventArgs e)
        => await RunGitAsync(GitStatusCenterOperationKind.Sync, null);

    private async void GitCheckout_Click(object sender, RoutedEventArgs e)
        => await RunGitAsync(GitStatusCenterOperationKind.Checkout, "main");

    private static async Task RunGitAsync(GitStatusCenterOperationKind operationKind, string? operationTarget)
    {
        var itemsCount = operationKind is GitStatusCenterOperationKind.Clone ? 1024 : 0;

        var banner = AddCard_GitOperation(operationKind, @"C:\Users\Public\Documents\DevWinUI", ReturnResult.InProgress, true, operationTarget, itemsCount);

        var result = await SimulateGitOperationAsync(operationKind, banner.ProgressEventSource, itemsCount, banner.CancellationToken);

        StatusCenterViewModel.Instance.RemoveItem(banner);
        AddCard_GitOperation(operationKind, @"C:\Users\Public\Documents\DevWinUI", result, false, operationTarget, itemsCount);
    }

    private static StatusCenterItem AddCard_Copy(
        ReturnResult returnStatus,
        string[] source,
        string[] destination,
        long itemsCount = 0,
        long totalSize = 0)
    {
        var sourcePath = GetParentDir(source.FirstOrDefault());
        var destinationPath = GetParentDir(destination.FirstOrDefault());
        var destinationDirName = destinationPath.Split('\\').Last();

        string ItemsText(long count) => count == 1 ? "1 item" : $"{count} items";

        string Header(long count) => returnStatus switch
        {
            ReturnResult.Cancelled => $"Canceled copying {ItemsText(count)} to \"{destinationDirName}\"",
            ReturnResult.Success => $"Copied {ItemsText(count)} to \"{destinationDirName}\"",
            ReturnResult.Failed => $"Error copying {ItemsText(count)} to \"{destinationDirName}\"",
            _ => $"Copying {ItemsText(count)} to \"{destinationDirName}\""
        };

        string SubHeader(long count) => returnStatus switch
        {
            ReturnResult.Cancelled => $"Canceled copying {ItemsText(count)} from \"{sourcePath}\" to \"{destinationPath}\"",
            ReturnResult.Success => $"Copied {ItemsText(count)} from \"{sourcePath}\" to \"{destinationPath}\"",
            ReturnResult.Failed => $"Failed to copy {ItemsText(count)} from \"{sourcePath}\" to \"{destinationPath}\"",
            _ => $"Copying {ItemsText(count)} from \"{sourcePath}\" to \"{destinationPath}\""
        };

        string Message(StatusCenterItemProgressModel progress)
            => $"{StatusCenterHelper.ToSizeString(progress.ProcessedSize)} of {StatusCenterHelper.ToSizeString(progress.TotalSize)} processed";

        return StatusCenterHelper.AddCard(
            returnStatus,
            Header(itemsCount),
            SubHeader(itemsCount),
            "Discovering items...",
            StatusCenterItemIconKind.Copy,
            source,
            destination,
            itemsCount: itemsCount,
            totalSize: totalSize,
            update: (item, progress) => (Header(item.TotalItemsCount), SubHeader(item.TotalItemsCount), Message(progress)));
    }

    private static StatusCenterItem AddCard_Move(
        ReturnResult returnStatus,
        string[] source,
        string[] destination,
        long itemsCount = 0,
        long totalSize = 0)
    {
        var sourcePath = GetParentDir(source.FirstOrDefault());
        var destinationPath = GetParentDir(destination.FirstOrDefault());
        var destinationDirName = destinationPath.Split('\\').Last();

        string ItemsText(long count) => count == 1 ? "1 item" : $"{count} items";

        string Header(long count) => returnStatus switch
        {
            ReturnResult.Cancelled => $"Canceled moving {ItemsText(count)} to \"{destinationDirName}\"",
            ReturnResult.Success => $"Moved {ItemsText(count)} to \"{destinationDirName}\"",
            ReturnResult.Failed => $"Error moving {ItemsText(count)} to \"{destinationDirName}\"",
            _ => $"Moving {ItemsText(count)} to \"{destinationDirName}\""
        };

        string SubHeader(long count) => returnStatus switch
        {
            ReturnResult.Cancelled => $"Canceled moving {ItemsText(count)} from \"{sourcePath}\" to \"{destinationPath}\"",
            ReturnResult.Success => $"Moved {ItemsText(count)} from \"{sourcePath}\" to \"{destinationPath}\"",
            ReturnResult.Failed => $"Failed to move {ItemsText(count)} from \"{sourcePath}\" to \"{destinationPath}\"",
            _ => $"Moving {ItemsText(count)} from \"{sourcePath}\" to \"{destinationPath}\""
        };

        string Message(StatusCenterItemProgressModel progress)
            => $"{StatusCenterHelper.ToSizeString(progress.ProcessedSize)} of {StatusCenterHelper.ToSizeString(progress.TotalSize)} processed";

        return StatusCenterHelper.AddCard(
            returnStatus,
            Header(itemsCount),
            SubHeader(itemsCount),
            "Discovering items...",
            StatusCenterItemIconKind.Move,
            source,
            destination,
            itemsCount: itemsCount,
            totalSize: totalSize,
            update: (item, progress) => (Header(item.TotalItemsCount), SubHeader(item.TotalItemsCount), Message(progress)));
    }

    private static StatusCenterItem AddCard_Delete(
        ReturnResult returnStatus,
        string[]? source,
        long itemsCount = 0,
        long totalSize = 0)
    {
        var sourcePath = GetParentDir(source?.FirstOrDefault());
        var sourceDirName = sourcePath.Split('\\').Last();

        string ItemsText(long count) => count == 1 ? "1 item" : $"{count} items";

        string Header(long count) => returnStatus switch
        {
            ReturnResult.Cancelled => $"Canceled deleting {ItemsText(count)} from \"{sourceDirName}\"",
            ReturnResult.Success => $"Deleted {ItemsText(count)} from \"{sourceDirName}\"",
            ReturnResult.Failed => $"Error deleting {ItemsText(count)} from \"{sourceDirName}\"",
            _ => $"Deleting {ItemsText(count)} from \"{sourceDirName}\""
        };

        string SubHeader(long count) => returnStatus is ReturnResult.Failed
            ? $"Failed to delete {ItemsText(count)} from \"{sourcePath}\""
            : string.Empty;

        string Message(StatusCenterItemProgressModel progress)
            => $"{progress.ProcessedItemsCount}/{ItemsText(progress.ItemsCount)} processed";

        return StatusCenterHelper.AddCard(
            returnStatus,
            Header(itemsCount),
            SubHeader(itemsCount),
            "Discovering items...",
            StatusCenterItemIconKind.Delete,
            source,
            itemsCount: itemsCount,
            totalSize: totalSize,
            update: (item, progress) => (Header(item.TotalItemsCount), SubHeader(item.TotalItemsCount), Message(progress)));
    }

    private static StatusCenterItem AddCard_Recycle(
        ReturnResult returnStatus,
        string[]? source,
        long itemsCount = 0,
        long totalSize = 0)
    {
        var sourcePath = GetParentDir(source?.FirstOrDefault());
        var sourceDirName = sourcePath.Split('\\').Last();

        string ItemsText(long count) => count == 1 ? "1 item" : $"{count} items";

        string Header(long count) => returnStatus switch
        {
            ReturnResult.Cancelled => $"Canceled deleting {ItemsText(count)} from \"{sourceDirName}\"",
            ReturnResult.Success => $"Deleted {ItemsText(count)} from \"{sourceDirName}\"",
            ReturnResult.Failed => $"Error deleting {ItemsText(count)} from \"{sourceDirName}\"",
            _ => $"Deleting {ItemsText(count)} from \"{sourceDirName}\""
        };

        string Message(StatusCenterItemProgressModel progress)
            => $"{progress.ProcessedItemsCount}/{ItemsText(progress.ItemsCount)} processed";

        return StatusCenterHelper.AddCard(
            returnStatus,
            Header(itemsCount),
            string.Empty,
            "Discovering items...",
            StatusCenterItemIconKind.Recycle,
            source,
            itemsCount: itemsCount,
            totalSize: totalSize,
            update: (item, progress) => (Header(item.TotalItemsCount), string.Empty, Message(progress)));
    }

    private static StatusCenterItem AddCard_Compress(
        string[] source,
        string[] destination,
        ReturnResult returnStatus,
        long itemsCount = 0,
        long totalSize = 0)
    {
        var sourcePath = GetParentDir(source.FirstOrDefault());
        var destinationPath = GetParentDir(destination.FirstOrDefault());
        var destinationDirName = destinationPath.Split('\\').Last();

        string ItemsText(long count) => count == 1 ? "1 item" : $"{count} items";

        string Header(long count) => returnStatus switch
        {
            ReturnResult.Cancelled => $"Canceled compressing {ItemsText(count)} to \"{destinationDirName}\"",
            ReturnResult.Success => $"Compressed {ItemsText(count)} to \"{destinationDirName}\"",
            ReturnResult.Failed => $"Error compressing {ItemsText(count)} to \"{destinationDirName}\"",
            _ => $"Compressing {ItemsText(count)} to \"{destinationDirName}\""
        };

        string SubHeader(long count) => returnStatus switch
        {
            ReturnResult.Cancelled => $"Canceled compressing {ItemsText(count)} from \"{sourcePath}\" to \"{destinationPath}\"",
            ReturnResult.Success => $"Compressed {ItemsText(count)} from \"{sourcePath}\" to \"{destinationPath}\"",
            ReturnResult.Failed => $"Failed to compress {ItemsText(count)} from \"{sourcePath}\" to \"{destinationPath}\"",
            _ => $"Compressing {ItemsText(count)} from \"{sourcePath}\" to \"{destinationPath}\""
        };

        string Message(StatusCenterItemProgressModel progress)
            => $"{progress.ProcessedItemsCount}/{ItemsText(progress.ItemsCount)} processed";

        return StatusCenterHelper.AddCard(
            returnStatus,
            Header(itemsCount),
            SubHeader(itemsCount),
            "Discovering items...",
            StatusCenterItemIconKind.Compress,
            source,
            destination,
            itemsCount: itemsCount,
            totalSize: totalSize,
            update: (item, progress) => (Header(item.TotalItemsCount), SubHeader(item.TotalItemsCount), Message(progress)));
    }

    private static StatusCenterItem AddCard_Decompress(
        string[] source,
        string[] destination,
        ReturnResult returnStatus,
        long itemsCount = 0,
        long totalSize = 0)
    {
        var sourceFileName = source.First().Split('\\').Last();
        var sourcePath = GetParentDir(source.FirstOrDefault());
        var destinationPath = GetParentDir(destination.FirstOrDefault());
        var destinationDirName = destinationPath.Split('\\').Last();

        string Header() => returnStatus switch
        {
            ReturnResult.Cancelled => $"Canceled extracting \"{sourceFileName}\" to \"{destinationDirName}\"",
            ReturnResult.Success => $"Extracted \"{sourceFileName}\" to \"{destinationDirName}\"",
            ReturnResult.Failed => $"Error extracting \"{sourceFileName}\" to \"{destinationDirName}\"",
            _ => $"Extracting \"{sourceFileName}\" to \"{destinationDirName}\""
        };

        string SubHeader() => returnStatus switch
        {
            ReturnResult.Cancelled => $"Canceled extracting \"{sourceFileName}\" from \"{sourcePath}\" to \"{destinationPath}\"",
            ReturnResult.Success => $"Extracted \"{sourceFileName}\" from \"{sourcePath}\" to \"{destinationPath}\"",
            ReturnResult.Failed => $"Failed to extract \"{sourceFileName}\" from \"{sourcePath}\" to \"{destinationPath}\"",
            _ => $"Extracting \"{sourceFileName}\" from \"{sourcePath}\" to \"{destinationPath}\""
        };

        string Message(StatusCenterItemProgressModel progress)
            => $"{StatusCenterHelper.ToSizeString(progress.ProcessedSize)} of {StatusCenterHelper.ToSizeString(progress.TotalSize)} processed";

        return StatusCenterHelper.AddCard(
            returnStatus,
            Header(),
            SubHeader(),
            "Discovering items...",
            StatusCenterItemIconKind.Extract,
            source,
            destination,
            itemsCount: itemsCount,
            totalSize: totalSize,
            update: (_, progress) => (Header(), SubHeader(), Message(progress)));
    }

    private static StatusCenterItem AddCard_InstallFont(
        string[] source,
        ReturnResult returnStatus,
        long itemsCount = 0,
        long totalSize = 0)
    {
        var sourcePath = GetParentDir(source.FirstOrDefault());

        string FontsText(long count) => count == 1 ? "1 font" : $"{count} fonts";

        string Header(long count) => returnStatus switch
        {
            ReturnResult.Cancelled => $"Canceled installing {FontsText(count)}",
            ReturnResult.Success => $"Installed {FontsText(count)}",
            ReturnResult.Failed => $"Error installing {FontsText(count)}",
            _ => $"Installing {FontsText(count)}"
        };

        string SubHeader(long count) => returnStatus switch
        {
            ReturnResult.Cancelled => $"Canceled installing {FontsText(count)} from \"{sourcePath}\"",
            ReturnResult.Success => $"Installed {FontsText(count)} from \"{sourcePath}\"",
            ReturnResult.Failed => $"Failed to install {FontsText(count)} from \"{sourcePath}\"",
            _ => $"Installing {FontsText(count)} from \"{sourcePath}\""
        };

        string Message(StatusCenterItemProgressModel progress)
            => $"{StatusCenterHelper.ToSizeString(progress.ProcessedSize)} of {StatusCenterHelper.ToSizeString(progress.TotalSize)} processed";

        var item = StatusCenterHelper.AddCard(
            returnStatus,
            Header(itemsCount),
            SubHeader(itemsCount),
            "Discovering items...",
            StatusCenterItemIconKind.InstallFont,
            source,
            (string[])[string.Empty],
            false,
            itemsCount,
            totalSize,
            (item, progress) => (Header(item.TotalItemsCount), SubHeader(item.TotalItemsCount), Message(progress)));

        if (returnStatus is ReturnResult.InProgress)
            item.IsCancelable = false;

        return item;
    }

    private static StatusCenterItem AddCard_EmptyRecycleBin(
        ReturnResult returnStatus,
        long itemsCount = 0,
        long totalSize = 0)
    {
        string ItemsText(long count) => count == 1 ? "1 item" : $"{count} items";

        string Header() => returnStatus switch
        {
            ReturnResult.Cancelled => "Canceled emptying Recycle Bin",
            ReturnResult.Success => "Emptied Recycle Bin",
            ReturnResult.Failed => "Error emptying Recycle Bin",
            _ => "Emptying Recycle Bin"
        };

        string SubHeader() => returnStatus is ReturnResult.Failed
            ? "Failed to empty Recycle Bin"
            : string.Empty;

        string Message(StatusCenterItemProgressModel progress)
            => $"{progress.ProcessedItemsCount}/{ItemsText(progress.ItemsCount)} processed";

        return StatusCenterHelper.AddCard(
            returnStatus,
            Header(),
            SubHeader(),
            "Discovering items...",
            StatusCenterItemIconKind.Delete,
            canProvideProgress: false,
            itemsCount: itemsCount,
            totalSize: totalSize,
            update: (_, progress) => (Header(), SubHeader(), Message(progress)));
    }

    private static StatusCenterItem AddCard_GitOperation(
        GitStatusCenterOperationKind operationKind,
        string repositoryPath,
        ReturnResult returnStatus,
        bool canProvideProgress = false,
        string? operationTarget = null,
        long itemsCount = 0)
    {
        var isClone = operationKind is GitStatusCenterOperationKind.Clone;
        var displayPath = isClone ? GetParentDir(repositoryPath) : repositoryPath;
        var repositoryName = Path.GetFileName(displayPath.TrimEnd('\\', '/'));

        if (string.IsNullOrEmpty(repositoryName))
            repositoryName = displayPath;

        string Header() => operationKind switch
        {
            GitStatusCenterOperationKind.Clone => returnStatus switch
            {
                ReturnResult.Cancelled => $"Canceled cloning {operationTarget} to \"{repositoryName}\"",
                ReturnResult.Success => $"Cloned \"{operationTarget}\" to \"{repositoryName}\"",
                ReturnResult.Failed => $"Error cloning \"{operationTarget}\" to \"{repositoryName}\"",
                _ => $"Cloning \"{operationTarget}\" to \"{repositoryName}\""
            },
            GitStatusCenterOperationKind.Fetch => returnStatus switch
            {
                ReturnResult.Cancelled => $"Canceled fetching remote changes for \"{repositoryName}\"",
                ReturnResult.Success => $"Fetched remote changes for \"{repositoryName}\"",
                ReturnResult.Failed => $"Failed to fetch remote changes for \"{repositoryName}\"",
                _ => $"Fetching remote changes for \"{repositoryName}\""
            },
            GitStatusCenterOperationKind.Pull => returnStatus switch
            {
                ReturnResult.Cancelled => $"Canceled pulling remote changes into \"{repositoryName}\"",
                ReturnResult.Success => $"Pulled remote changes into \"{repositoryName}\"",
                ReturnResult.Failed => $"Failed to pull remote changes into \"{repositoryName}\"",
                _ => $"Pulling remote changes into \"{repositoryName}\""
            },
            GitStatusCenterOperationKind.Push => returnStatus switch
            {
                ReturnResult.Cancelled => $"Canceled pushing local commits from \"{repositoryName}\"",
                ReturnResult.Success => $"Pushed local commits from \"{repositoryName}\"",
                ReturnResult.Failed => $"Failed to push local commits from \"{repositoryName}\"",
                _ => $"Pushing local commits from \"{repositoryName}\""
            },
            GitStatusCenterOperationKind.Sync => returnStatus switch
            {
                ReturnResult.Cancelled => $"Canceled syncing local and remote commits for \"{repositoryName}\"",
                ReturnResult.Success => $"Synced local and remote commits for \"{repositoryName}\"",
                ReturnResult.Failed => $"Failed to sync local and remote commits for \"{repositoryName}\"",
                _ => $"Syncing local and remote commits for \"{repositoryName}\""
            },
            _ => returnStatus switch
            {
                ReturnResult.Cancelled => $"Canceled switching to \"{operationTarget}\" in \"{repositoryName}\"",
                ReturnResult.Success => $"Switched to \"{operationTarget}\" in \"{repositoryName}\"",
                ReturnResult.Failed => $"Failed to switch to \"{operationTarget}\" in \"{repositoryName}\"",
                _ => $"Switching to \"{operationTarget}\" in \"{repositoryName}\""
            }
        };

        string SubHeader(long count) => isClone
            ? returnStatus switch
            {
                ReturnResult.Cancelled => $"Canceled cloning {count} from \"{operationTarget}\" to \"{displayPath}\"",
                ReturnResult.Success => $"Cloned {count} from \"{operationTarget}\" to \"{displayPath}\"",
                ReturnResult.Failed => $"Failed to clone {count} from \"{operationTarget}\" to \"{displayPath}\"",
                _ => $"Cloning {count} from \"{operationTarget}\" to \"{displayPath}\""
            }
            : string.Empty;

        string Message(StatusCenterItemProgressModel progress)
            => $"{(int)(progress.Percentage ?? 0)}% complete";

        var item = StatusCenterHelper.AddCard(
            returnStatus,
            Header(),
            SubHeader(itemsCount),
            "0% complete",
            StatusCenterItemIconKind.Git,
            operationTarget is null ? null : (string[])[operationTarget],
            (string[])[repositoryPath],
            returnStatus is ReturnResult.InProgress && canProvideProgress,
            itemsCount,
            update: (item, progress) => (Header(), SubHeader(item.TotalItemsCount), Message(progress)));

        if (returnStatus is ReturnResult.InProgress)
        {
            item.IsSpeedAndProgressAvailable = false;
            item.IsCancelable = operationKind is GitStatusCenterOperationKind.Clone;
        }

        return item;
    }

    private static async Task<ReturnResult> SimulateFileOperationAsync(
        IProgress<StatusCenterItemProgressModel> progress,
        string[] source,
        long totalSize,
        CancellationToken cancellationToken)
    {
        var progressModel = new StatusCenterItemProgressModel(
            progress,
            enumerationCompleted: true,
            status: FileSystemStatusCode.InProgress,
            itemsCount: source.Length)
        {
            TotalSize = totalSize
        };
        progressModel.Report(0);

        try
        {
            var percentage = 0.0;

            while (percentage < 100)
            {
                var delay = Random.Shared.Next(100, 140);
                if (Random.Shared.Next(0, 15) == 0)
                    delay += Random.Shared.Next(150, 300);

                await Task.Delay(delay, cancellationToken);

                percentage = Math.Min(100, percentage + Random.Shared.Next(2, 5));

                var processedItems = Math.Min(source.Length, (int)Math.Ceiling(source.Length * percentage / 100));
                var pendingItems = processedItems - progressModel.ProcessedItemsCount;

                if (pendingItems > 0)
                    progressModel.AddProcessedItemsCount(pendingItems);

                if (source.Length > 0)
                    progressModel.FileName = Path.GetFileName(source[Math.Max(0, processedItems - 1)]);

                progressModel.Report(percentage);
            }

            return ReturnResult.Success;
        }
        catch (OperationCanceledException)
        {
            return ReturnResult.Cancelled;
        }
    }

    private static async Task<ReturnResult> SimulateItemOperationAsync(
        IProgress<StatusCenterItemProgressModel> progress,
        string[] source,
        CancellationToken cancellationToken)
    {
        var progressModel = new StatusCenterItemProgressModel(
            progress,
            enumerationCompleted: true,
            status: FileSystemStatusCode.InProgress,
            itemsCount: source.Length);
        progressModel.Report(0);

        try
        {
            for (var i = 0; i < source.Length; i++)
            {
                var delay = Random.Shared.Next(200, 300);
                await Task.Delay(delay, cancellationToken);

                progressModel.AddProcessedItemsCount(1);
                progressModel.FileName = Path.GetFileName(source[i]);
                progressModel.Report((i + 1) * 100.0 / source.Length);
            }

            return ReturnResult.Success;
        }
        catch (OperationCanceledException)
        {
            return ReturnResult.Cancelled;
        }
    }

    private static async Task<ReturnResult> SimulateIndeterminateOperationAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(Random.Shared.Next(1600, 2800), cancellationToken);

            return ReturnResult.Success;
        }
        catch (OperationCanceledException)
        {
            return ReturnResult.Cancelled;
        }
    }

    private static async Task<ReturnResult> SimulateGitOperationAsync(
        GitStatusCenterOperationKind operationKind,
        IProgress<StatusCenterItemProgressModel> progress,
        long itemsCount,
        CancellationToken cancellationToken)
    {
        var progressModel = new StatusCenterItemProgressModel(
            progress,
            enumerationCompleted: true,
            status: FileSystemStatusCode.InProgress,
            itemsCount: itemsCount);

        try
        {
            switch (operationKind)
            {
                case GitStatusCenterOperationKind.Checkout:
                {
                    var percentage = 0.0;
                    while (percentage < 100)
                    {
                        await Task.Delay(Random.Shared.Next(80, 120), cancellationToken);
                        percentage = Math.Min(100, percentage + Random.Shared.Next(25, 40));
                        progressModel.Report(percentage);
                    }
                    break;
                }
                case GitStatusCenterOperationKind.Fetch:
                {
                    var percentage = 0.0;
                    while (percentage < 100)
                    {
                        await Task.Delay(Random.Shared.Next(100, 140), cancellationToken);
                        percentage = Math.Min(100, percentage + Random.Shared.Next(18, 30));
                        progressModel.Report(percentage);
                    }
                    break;
                }
                case GitStatusCenterOperationKind.Push:
                {
                    var percentage = 0.0;
                    while (percentage < 100)
                    {
                        await Task.Delay(Random.Shared.Next(110, 150), cancellationToken);
                        percentage = Math.Min(100, percentage + Random.Shared.Next(14, 24));
                        progressModel.Report(percentage);
                    }
                    break;
                }
                case GitStatusCenterOperationKind.Pull:
                {
                    var percentage = 0.0;
                    while (percentage < 100)
                    {
                        await Task.Delay(Random.Shared.Next(110, 150), cancellationToken);
                        percentage = Math.Min(100, percentage + Random.Shared.Next(10, 18));
                        progressModel.Report(percentage);
                    }
                    break;
                }
                case GitStatusCenterOperationKind.Sync:
                {
                    var percentage = 0.0;
                    while (percentage < 100)
                    {
                        await Task.Delay(Random.Shared.Next(110, 150), cancellationToken);
                        percentage = Math.Min(100, percentage + Random.Shared.Next(7, 14));
                        progressModel.Report(percentage);
                    }
                    break;
                }
                case GitStatusCenterOperationKind.Clone:
                {
                    var percentage = 0.0;
                    while (percentage < 100)
                    {
                        var delay = Random.Shared.Next(100, 150);
                        if (Random.Shared.Next(0, 12) == 0)
                            delay += Random.Shared.Next(200, 450);

                        await Task.Delay(delay, cancellationToken);

                        percentage = Math.Min(100, percentage + Random.Shared.Next(3, 7));

                        var processedItems = Math.Min(itemsCount, (long)(itemsCount * percentage / 100));
                        var pendingItems = processedItems - progressModel.ProcessedItemsCount;
                        if (pendingItems > 0)
                            progressModel.AddProcessedItemsCount(pendingItems);

                        progressModel.Report(percentage);
                    }
                    break;
                }
            }

            return ReturnResult.Success;
        }
        catch (OperationCanceledException)
        {
            return ReturnResult.Cancelled;
        }
    }

    private static string GetParentDir(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;

        var index = path.Contains('/', StringComparison.Ordinal) ? path.LastIndexOf('/') : path.LastIndexOf('\\');
        return path[..(index != -1 ? index : path.Length)];
    }

    private enum GitStatusCenterOperationKind
    {
        Clone,
        Fetch,
        Pull,
        Push,
        Sync,
        Checkout
    }
}
