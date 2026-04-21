// https://github.com/SuGar0218/SuGarToolkit.WinUI3

namespace DevWinUI;

public partial class MessageBoxDefaultButtonToContentDialogButtonConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) => Convert((MessageBoxDefaultButton) value);

    public object ConvertBack(object value, Type targetType, object parameter, string language) => ConvertBack((ContentDialogButton) value);

    public static ContentDialogButton Convert(MessageBoxDefaultButton value) => value switch
    {
        MessageBoxDefaultButton.Button1 => ContentDialogButton.Primary,
        MessageBoxDefaultButton.Button2 => ContentDialogButton.Secondary,
        MessageBoxDefaultButton.Button3 => ContentDialogButton.Close,
        _ => ContentDialogButton.None,
    };

    public static MessageBoxDefaultButton ConvertBack(ContentDialogButton value) => value switch
    {
        ContentDialogButton.None => MessageBoxDefaultButton.Button1,
        ContentDialogButton.Primary => MessageBoxDefaultButton.Button1,
        ContentDialogButton.Secondary => MessageBoxDefaultButton.Button2,
        ContentDialogButton.Close => MessageBoxDefaultButton.Button3,
        _ => MessageBoxDefaultButton.Button1,
    };
}
