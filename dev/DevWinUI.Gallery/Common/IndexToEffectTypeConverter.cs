using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace DevWinUIGallery.Common;
public partial class IndexToForegroundFocusEffectTypesConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int index)
        {
            return (ForegroundFocusEffectTypes)index;
        }
        return ForegroundFocusEffectTypes.Blur;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return (int)value;
    }
}
