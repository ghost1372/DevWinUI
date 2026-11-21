// Copyright (c) Files Community
// Licensed under the MIT License.

namespace DevWinUI;

public static partial class StatusCenterStringExtensions
{
	public static string ToSizeString(this double size, SizeUnitTypes format) => ToSizeString((long)size, format);
	public static string ToSizeString(this long size, SizeUnitTypes format) => FileHelper.GetFileSize(size, format is SizeUnitTypes.BinaryUnits);
}
