// Copyright (c) Files Community
// Licensed under the MIT License.

namespace DevWinUI;

internal static class StatusCenterStringExtensions
{
	public static string ToSizeString(this double size) => ToSizeString((long)size);
	public static string ToSizeString(this long size) => FileHelper.GetFileSize(size, StatusCenterHelper.SizeUnitFormat is SizeUnitTypes.BinaryUnits);
}
