namespace DevWinUI;

public enum SizeUnitTypes
{
    BinaryUnits,
    DecimalUnits
}

public static class StatusCenterHelper
{
    public static SizeUnitTypes SizeUnitFormat { get; set; } = SizeUnitTypes.BinaryUnits;

    public static string ToSizeString(long size) => StatusCenterStringExtensions.ToSizeString(size);

    public static StatusCenterItem AddCard(
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
        var item = StatusCenterViewModel.Instance.AddItem(
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

    public static void UpdateCardStrings(StatusCenterItem card, StatusCenterItemProgressModel? progressValue = null)
    {
        if (card.UpdateStrings is not { } update || progressValue is null)
            return;

        var (header, subHeader, message) = update(card, progressValue);

        card.Header = header;
        card.SubHeader = subHeader;
        card.Message = message;
    }
}
