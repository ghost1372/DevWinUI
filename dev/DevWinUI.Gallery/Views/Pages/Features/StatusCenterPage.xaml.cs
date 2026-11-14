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
        var fileCount = Random.Shared.Next(3, 9);
        string[] source = FilePaths(@"C:\Users\Public\Documents", fileCount);
        string[] destination = FilePaths(@"C:\Users\Public\Downloads", fileCount);
        var totalSize = fileCount * Random.Shared.NextInt64(24, 97) * 1024 * 1024;
        var banner = AddCard_Copy(ReturnResult.InProgress, source, destination, 0, totalSize);

        var result = await SimulateFileOperationAsync(banner.ProgressEventSource, source, totalSize, banner.CancellationToken, banner);

        StatusCenterViewModel.Instance.RemoveItem(banner);
        AddCard_Copy(result, source, destination, banner.TotalItemsCount, totalSize);
    }

    private async void Move_Click(object sender, RoutedEventArgs e)
    {
        var fileCount = Random.Shared.Next(3, 9);
        string[] source = FilePaths(@"C:\Users\Public\Documents", fileCount);
        string[] destination = FilePaths(@"C:\Users\Public\Downloads", fileCount);
        var totalSize = fileCount * Random.Shared.NextInt64(24, 97) * 1024 * 1024;
        var banner = AddCard_Move(ReturnResult.InProgress, source, destination, 0, totalSize);

        var result = await SimulateFileOperationAsync(banner.ProgressEventSource, source, totalSize, banner.CancellationToken, banner);

        StatusCenterViewModel.Instance.RemoveItem(banner);
        AddCard_Move(result, source, destination, banner.TotalItemsCount, totalSize);
    }

    private async void Delete_Click(object sender, RoutedEventArgs e)
    {
        var fileCount = Random.Shared.Next(4, 11);
        string[] source = FilePaths(@"C:\Users\Public\Documents", fileCount);
        var banner = AddCard_Delete(ReturnResult.InProgress, source, 0);

        var result = await SimulateItemOperationAsync(banner.ProgressEventSource, source, banner.CancellationToken, banner);

        StatusCenterViewModel.Instance.RemoveItem(banner);
        AddCard_Delete(result, source, banner.TotalItemsCount);
    }

    private async void Recycle_Click(object sender, RoutedEventArgs e)
    {
        var fileCount = Random.Shared.Next(6, 15);
        string[] source = FilePaths(@"C:\Users\Public\Documents", fileCount);
        var banner = AddCard_Recycle(ReturnResult.InProgress, source, 0);

        var result = await SimulateItemOperationAsync(banner.ProgressEventSource, source, banner.CancellationToken, banner);

        StatusCenterViewModel.Instance.RemoveItem(banner);
        AddCard_Recycle(result, source, banner.TotalItemsCount);
    }

    private async void Compress_Click(object sender, RoutedEventArgs e)
    {
        var fileCount = Random.Shared.Next(3, 9);
        string[] source = FilePaths(@"C:\Users\Public\Documents", fileCount);
        string[] destination = [@"C:\Users\Public\Downloads\Archive.zip"];
        var totalSize = fileCount * Random.Shared.NextInt64(24, 81) * 1024 * 1024;
        var banner = AddCard_Compress(source, destination, ReturnResult.InProgress, 0, totalSize);

        var result = await SimulateFileOperationAsync(banner.ProgressEventSource, source, totalSize, banner.CancellationToken, banner);

        StatusCenterViewModel.Instance.RemoveItem(banner);
        AddCard_Compress(source, destination, result, banner.TotalItemsCount, totalSize);
    }

    private async void Extract_Click(object sender, RoutedEventArgs e)
    {
        string[] source = [@"C:\Users\Public\Documents\Archive.zip"];
        string[] destination = [@"C:\Users\Public\Downloads\Archive"];
        var totalSize = Random.Shared.NextInt64(128, 513) * 1024 * 1024;
        var banner = AddCard_Decompress(source, destination, ReturnResult.InProgress, 0, totalSize);

        string[] entries = FilePaths(@"C:\Users\Public\Downloads\Archive", Random.Shared.Next(12, 37));
        var result = await SimulateFileOperationAsync(banner.ProgressEventSource, entries, totalSize, banner.CancellationToken, banner, simulateDiscovery: false);

        StatusCenterViewModel.Instance.RemoveItem(banner);
        AddCard_Decompress(source, destination, result, 1, totalSize);
    }

    private async void InstallFont_Click(object sender, RoutedEventArgs e)
    {
        const int fontCount = 2;

        var banner = AddCard_InstallFont([@"C:\Users\Public\Documents"], ReturnResult.InProgress, fontCount);

        var result = await SimulateIndeterminateOperationAsync(banner.CancellationToken, banner);

        StatusCenterViewModel.Instance.RemoveItem(banner);
        AddCard_InstallFont([@"C:\Users\Public\Documents"], result, fontCount);
    }

    private async void EmptyRecycleBin_Click(object sender, RoutedEventArgs e)
    {
        var banner = AddCard_EmptyRecycleBin(ReturnResult.InProgress);

        var result = await SimulateIndeterminateOperationAsync(banner.CancellationToken, banner);

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
            => progress.Percentage is null
                ? progress.EnumerationCompleted ? "Processing items..." : "Discovering items..."
                : $"{StatusCenterHelper.ToSizeString(progress.ProcessedSize)} of {StatusCenterHelper.ToSizeString(progress.TotalSize)} processed";

        var card = StatusCenterHelper.AddCard(
            returnStatus,
            Header(itemsCount),
            SubHeader(itemsCount),
            "Discovering items...",
            StatusCenterItemIconKind.Copy,
            itemsCount: itemsCount,
            totalSize: totalSize,
            update: (item, progress) => progress.EnumerationCompleted
                ? (Header(item.TotalItemsCount), SubHeader(item.TotalItemsCount), Message(progress))
                : ($"Discovered {ItemsText(item.TotalItemsCount)}", SubHeader(item.TotalItemsCount), Message(progress)));

        if (returnStatus is ReturnResult.InProgress)
            card.IsPausable = true;

        return card;
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
            => progress.Percentage is null
                ? progress.EnumerationCompleted ? "Processing items..." : "Discovering items..."
                : $"{StatusCenterHelper.ToSizeString(progress.ProcessedSize)} of {StatusCenterHelper.ToSizeString(progress.TotalSize)} processed";

        var card = StatusCenterHelper.AddCard(
            returnStatus,
            Header(itemsCount),
            SubHeader(itemsCount),
            "Discovering items...",
            StatusCenterItemIconKind.Move,
            itemsCount: itemsCount,
            totalSize: totalSize,
            update: (item, progress) => progress.EnumerationCompleted
                ? (Header(item.TotalItemsCount), SubHeader(item.TotalItemsCount), Message(progress))
                : ($"Discovered {ItemsText(item.TotalItemsCount)}", SubHeader(item.TotalItemsCount), Message(progress)));

        if (returnStatus is ReturnResult.InProgress)
            card.IsPausable = true;

        return card;
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
            => progress.Percentage is null
                ? progress.EnumerationCompleted ? "Processing items..." : "Discovering items..."
                : $"{progress.ProcessedItemsCount}/{ItemsText(progress.ItemsCount)} processed";

        var card = StatusCenterHelper.AddCard(
            returnStatus,
            Header(itemsCount),
            SubHeader(itemsCount),
            "Discovering items...",
            StatusCenterItemIconKind.Delete,
            itemsCount: itemsCount,
            totalSize: totalSize,
            update: (item, progress) => progress.EnumerationCompleted
                ? (Header(item.TotalItemsCount), SubHeader(item.TotalItemsCount), Message(progress))
                : ($"Discovered {ItemsText(item.TotalItemsCount)}", SubHeader(item.TotalItemsCount), Message(progress)));

        if (returnStatus is ReturnResult.InProgress)
            card.IsPausable = true;

        return card;
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
            => progress.Percentage is null
                ? progress.EnumerationCompleted ? "Processing items..." : "Discovering items..."
                : $"{progress.ProcessedItemsCount}/{ItemsText(progress.ItemsCount)} processed";

        var card = StatusCenterHelper.AddCard(
            returnStatus,
            Header(itemsCount),
            string.Empty,
            "Discovering items...",
            StatusCenterItemIconKind.Recycle,
            itemsCount: itemsCount,
            totalSize: totalSize,
            update: (item, progress) => progress.EnumerationCompleted
                ? (Header(item.TotalItemsCount), string.Empty, Message(progress))
                : ($"Discovered {ItemsText(item.TotalItemsCount)}", string.Empty, Message(progress)));

        if (returnStatus is ReturnResult.InProgress)
            card.IsPausable = true;

        return card;
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
            => progress.Percentage is null
                ? progress.EnumerationCompleted ? "Processing items..." : "Discovering items..."
                : $"{progress.ProcessedItemsCount}/{ItemsText(progress.ItemsCount)} processed";

        var card = StatusCenterHelper.AddCard(
            returnStatus,
            Header(itemsCount),
            SubHeader(itemsCount),
            "Discovering items...",
            StatusCenterItemIconKind.Compress,
            itemsCount: itemsCount,
            totalSize: totalSize,
            update: (item, progress) => progress.EnumerationCompleted
                ? (Header(item.TotalItemsCount), SubHeader(item.TotalItemsCount), Message(progress))
                : ($"Discovered {ItemsText(item.TotalItemsCount)}", SubHeader(item.TotalItemsCount), Message(progress)));

        if (returnStatus is ReturnResult.InProgress)
            card.IsPausable = true;

        return card;
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
            => progress.Percentage is null
                ? progress.EnumerationCompleted ? "Processing items..." : "Discovering items..."
                : $"{StatusCenterHelper.ToSizeString(progress.ProcessedSize)} of {StatusCenterHelper.ToSizeString(progress.TotalSize)} processed";

        var card = StatusCenterHelper.AddCard(
            returnStatus,
            Header(),
            SubHeader(),
            "Discovering items...",
            StatusCenterItemIconKind.Extract,
            itemsCount: itemsCount,
            totalSize: totalSize,
            update: (item, progress) => progress.EnumerationCompleted
                ? (Header(), SubHeader(), Message(progress))
                : ($"Discovered {item.TotalItemsCount} item{(item.TotalItemsCount == 1 ? string.Empty : "s")}", SubHeader(), Message(progress)));

        if (returnStatus is ReturnResult.InProgress)
            card.IsPausable = true;

        return card;
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
            => progress.Percentage is null
                ? progress.EnumerationCompleted ? "Processing items..." : "Discovering items..."
                : $"{StatusCenterHelper.ToSizeString(progress.ProcessedSize)} of {StatusCenterHelper.ToSizeString(progress.TotalSize)} processed";

        var card = StatusCenterHelper.AddCard(
            returnStatus,
            Header(itemsCount),
            SubHeader(itemsCount),
            "Discovering items...",
            StatusCenterItemIconKind.InstallFont,
            false,
            itemsCount,
            totalSize,
            (item, progress) => (Header(item.TotalItemsCount), SubHeader(item.TotalItemsCount), Message(progress)));

        if (returnStatus is ReturnResult.InProgress)
            card.IsCancelable = false;

        return card;
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
            => progress.Percentage is null
                ? progress.EnumerationCompleted ? "Processing items..." : "Discovering items..."
                : $"{progress.ProcessedItemsCount}/{ItemsText(progress.ItemsCount)} processed";

        var card = StatusCenterHelper.AddCard(
            returnStatus,
            Header(),
            SubHeader(),
            "Discovering items...",
            StatusCenterItemIconKind.Delete,
            canProvideProgress: false,
            itemsCount: itemsCount,
            totalSize: totalSize,
            update: (_, progress) => (Header(), SubHeader(), Message(progress)));

        if (returnStatus is ReturnResult.InProgress)
            card.IsPausable = true;

        return card;
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

        var card = StatusCenterHelper.AddCard(
            returnStatus,
            Header(),
            SubHeader(itemsCount),
            "0% complete",
            StatusCenterItemIconKind.Git,
            returnStatus is ReturnResult.InProgress && canProvideProgress,
            itemsCount,
            update: (item, progress) => (Header(), SubHeader(item.TotalItemsCount), Message(progress)));

        if (returnStatus is ReturnResult.InProgress)
        {
            card.IsSpeedAndProgressAvailable = false;
            card.IsCancelable = operationKind is GitStatusCenterOperationKind.Clone;
        }

        return card;
    }

    private static async Task<ReturnResult> SimulateFileOperationAsync(
        IProgress<StatusCenterItemProgressModel> progress,
        string[] source,
        long totalSize,
        CancellationToken cancellationToken,
        StatusCenterItem item,
        bool simulateDiscovery = true)
    {
        var itemTotal = simulateDiscovery ? Random.Shared.Next(1, 26) : source.Length;
        var progressModel = new StatusCenterItemProgressModel(
            progress,
            enumerationCompleted: !simulateDiscovery,
            status: ReturnResult.InProgress,
            itemsCount: simulateDiscovery ? 0 : itemTotal);

        if (!simulateDiscovery)
        {
            progressModel.TotalSize = totalSize;
            progressModel.Report();
        }

        var pauseDelay = PauseAwareDelay(item);

        try
        {
            if (simulateDiscovery)
            {
                for (var discovered = 1; discovered <= itemTotal; discovered++)
                {
                    await pauseDelay(Random.Shared.Next(120, 220), cancellationToken);
                    progressModel.ItemsCount = discovered;
                    progressModel.Report();
                }
            }

            progressModel.TotalSize = totalSize;
            progressModel.ItemsCount = itemTotal;
            progressModel.EnumerationCompleted = true;
            progressModel.Report();

            await pauseDelay(Random.Shared.Next(50, 500), cancellationToken);
            progressModel.Report(0);

            var failAtPercentage = Random.Shared.Next(0, 100) < 20 ? Random.Shared.Next(40, 85) : double.MaxValue;
            var percentage = 0.0;

            while (percentage < 100)
            {
                var delay = Random.Shared.Next(100, 140);
                if (Random.Shared.Next(0, 15) == 0)
                    delay += Random.Shared.Next(150, 300);

                await pauseDelay(delay, cancellationToken);

                percentage = Math.Min(100, percentage + Random.Shared.Next(2, 5));

                var processedItems = Math.Min(itemTotal, (int)Math.Ceiling(itemTotal * percentage / 100));
                var pendingItems = processedItems - progressModel.ProcessedItemsCount;

                if (pendingItems > 0)
                    progressModel.AddProcessedItemsCount(pendingItems);

                if (processedItems > 0)
                    progressModel.FileName = Path.GetFileName(source[(processedItems - 1) % source.Length]);

                progressModel.Report(percentage);

                if (percentage >= failAtPercentage && percentage < 100)
                    return ReturnResult.Failed;
            }

            return ReturnResult.Success;
        }
        catch (OperationCanceledException)
        {
            await Task.Delay(Random.Shared.Next(50, 500), CancellationToken.None);
            return ReturnResult.Cancelled;
        }
    }

    private static async Task<ReturnResult> SimulateItemOperationAsync(
        IProgress<StatusCenterItemProgressModel> progress,
        string[] source,
        CancellationToken cancellationToken,
        StatusCenterItem item)
    {
        var itemTotal = Random.Shared.Next(1, 26);
        var progressModel = new StatusCenterItemProgressModel(
            progress,
            enumerationCompleted: false,
            status: ReturnResult.InProgress,
            itemsCount: 0);

        var pauseDelay = PauseAwareDelay(item);

        try
        {
            for (var discovered = 1; discovered <= itemTotal; discovered++)
            {
                await pauseDelay(Random.Shared.Next(120, 220), cancellationToken);
                progressModel.ItemsCount = discovered;
                progressModel.Report();
            }

            progressModel.ItemsCount = itemTotal;
            progressModel.EnumerationCompleted = true;
            progressModel.Report();

            await pauseDelay(Random.Shared.Next(50, 500), cancellationToken);
            progressModel.Report(0);

            var failAtPercentage = Random.Shared.Next(0, 100) < 20 ? Random.Shared.Next(40, 85) : double.MaxValue;
            var processed = 0;

            while (processed < itemTotal)
            {
                await pauseDelay(Random.Shared.Next(55, 85), cancellationToken);

                processed++;
                progressModel.AddProcessedItemsCount(1);
                progressModel.FileName = Path.GetFileName(source[(processed - 1) % source.Length]);
                var percentage = processed * 100.0 / itemTotal;
                progressModel.Report(percentage);

                if (percentage >= failAtPercentage && percentage < 100)
                    return ReturnResult.Failed;
            }

            return ReturnResult.Success;
        }
        catch (OperationCanceledException)
        {
            await Task.Delay(Random.Shared.Next(50, 500), CancellationToken.None);
            return ReturnResult.Cancelled;
        }
    }

    private static async Task<ReturnResult> SimulateIndeterminateOperationAsync(CancellationToken cancellationToken, StatusCenterItem item)
    {
        var pauseDelay = PauseAwareDelay(item);

        try
        {
            var remaining = Random.Shared.Next(1600, 2800);
            while (remaining > 0)
            {
                var step = Math.Min(remaining, 100);
                await pauseDelay(step, cancellationToken);
                remaining -= step;
            }

            return Random.Shared.Next(0, 100) < 20 ? ReturnResult.Failed : ReturnResult.Success;
        }
        catch (OperationCanceledException)
        {
            await Task.Delay(Random.Shared.Next(50, 500), CancellationToken.None);
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
            status: ReturnResult.InProgress,
            itemsCount: itemsCount);

        try
        {
            var failAtPercentage = Random.Shared.Next(0, 100) < 20 ? Random.Shared.Next(40, 90) : double.MaxValue;

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

                            if (percentage >= failAtPercentage && percentage < 100)
                                return ReturnResult.Failed;
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

                            if (percentage >= failAtPercentage && percentage < 100)
                                return ReturnResult.Failed;
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

                            if (percentage >= failAtPercentage && percentage < 100)
                                return ReturnResult.Failed;
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

                            if (percentage >= failAtPercentage && percentage < 100)
                                return ReturnResult.Failed;
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

                            if (percentage >= failAtPercentage && percentage < 100)
                                return ReturnResult.Failed;
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

                            if (percentage >= failAtPercentage && percentage < 100)
                                return ReturnResult.Failed;
                        }
                        break;
                    }
            }

            return ReturnResult.Success;
        }
        catch (OperationCanceledException)
        {
            await Task.Delay(Random.Shared.Next(50, 500), CancellationToken.None);
            return ReturnResult.Cancelled;
        }
    }

    private static Func<int, CancellationToken, Task> PauseAwareDelay(StatusCenterItem item)
    {
        var resumeSignal = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        item.OnPauseAsync = () =>
        {
            resumeSignal = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            return Task.CompletedTask;
        };
        item.OnResumeAsync = () =>
        {
            resumeSignal.TrySetResult();
            return Task.CompletedTask;
        };

        return async (millisecondsDelay, cancellationToken) =>
        {
            while (item.IsPaused)
                await resumeSignal.Task.WaitAsync(cancellationToken);

            await Task.Delay(millisecondsDelay, cancellationToken);
        };
    }

    private static string[] FilePaths(string directory, int count)
        => [.. Enumerable.Range(1, count).Select(i => $@"{directory}\File {i}.txt")];

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
