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
}
