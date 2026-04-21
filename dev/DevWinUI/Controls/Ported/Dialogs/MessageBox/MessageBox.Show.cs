// https://github.com/SuGar0218/SuGarToolkit.WinUI3

namespace DevWinUI;

public partial class MessageBox
{
    public static Task<MessageBoxResult> ShowAsync(string message)
    {
        return ShowAsync(null, message, null, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
    }

    public static Task<MessageBoxResult> ShowAsync(string? message, string? title)
    {
        return ShowAsync(null, message, title, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
    }

    public static Task<MessageBoxResult> ShowAsync(Window? owner, string message)
    {
        return ShowAsync(owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
    }

    public static Task<MessageBoxResult> ShowAsync(Window? owner, string? message, string? title)
    {
        return ShowAsync(owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
    }

    public static Task<MessageBoxResult> ShowAsync(
        string? message,
        string? title,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton = default)
    {
        return ShowAsync(null, message, title, buttons, icon, defaultButton);
    }

    public static Task<MessageBoxResult> ShowAsync(
        Window? owner,
        string? message,
        string? title,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton = default)
    {
        return new MessageBox
        {
            Owner = owner,
            Title = title,
            Message = message,
            Buttons = buttons,
            Icon = icon,
            DefaultButton = defaultButton
        }
        .ShowAsync();
    }

    public static SystemBackdrop? SystemBackdrop { get; set; } = new MicaBackdrop();
}
