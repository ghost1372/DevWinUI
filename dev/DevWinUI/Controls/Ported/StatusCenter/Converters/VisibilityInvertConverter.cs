// Copyright (c) Files Community
// Licensed under the MIT License.

using WinRT;

namespace DevWinUI;

internal sealed partial class VisibilityInvertConverter : IValueConverter
{
	[DynamicWindowsRuntimeCast(typeof(Visibility))]
	public object Convert(object value, Type targetType, object parameter, string language)
	{
		if (value is bool isVisible)
		{
			return isVisible ? Visibility.Collapsed : Visibility.Visible;
		}

		return (Visibility)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		throw new NotImplementedException();
	}
}
