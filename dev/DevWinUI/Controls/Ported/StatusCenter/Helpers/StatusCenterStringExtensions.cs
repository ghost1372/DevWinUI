// Copyright (c) Files Community
// Licensed under the MIT License.

using ByteSize = ByteSizeLib.ByteSize;

namespace DevWinUI
{
	internal static class StatusCenterStringExtensions
	{
		public static string ToSizeString(this double size) => ByteSize.FromBytes(size).ToSizeString();
		public static string ToSizeString(this long size) => ByteSize.FromBytes(size).ToSizeString();
		public static string ToSizeString(this ulong size) => ByteSize.FromBytes(size).ToSizeString();
		public static string ToSizeString(this decimal size) => ByteSize.FromBytes((double)size).ToSizeString();
		public static string ToSizeString(this ByteSize size) => StatusCenterHelper.SizeUnitFormat is SizeUnitTypes.BinaryUnits ? size.ToBinaryString() : size.ToString();
	}
}
