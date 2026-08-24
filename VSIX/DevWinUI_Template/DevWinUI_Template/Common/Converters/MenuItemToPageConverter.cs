using DevWinUI_Template.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DevWinUI_Template;

public class MenuItemToPageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not NavigationMenuModel item || item.PageType is null)
        {
            return null;
        }

        try
        {
            return Activator.CreateInstance(item.PageType);
        }
        catch
        {
            return null;
        }
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
