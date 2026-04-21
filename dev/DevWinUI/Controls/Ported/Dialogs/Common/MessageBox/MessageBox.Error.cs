namespace DevWinUI;

public partial class MessageBox
{
    public static Task<MessageBoxResult> ShowErrorAsync(string message)
    {
        return ShowAsync(null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
    }

    public static Task<MessageBoxResult> ShowErrorAsync(string? message, string? title)
    {
        return ShowAsync(null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
    }

    public static Task<MessageBoxResult> ShowErrorAsync(Window? owner, string message)
    {
        return ShowAsync(owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
    }

    public static Task<MessageBoxResult> ShowErrorAsync(Window? owner, string? message, string? title)
    {
        return ShowAsync(owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
    }

    public static Task<MessageBoxResult> ShowErrorAsync(string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton = default)
    {
        return ShowAsync(null, message, title, buttons, MessageBoxIcon.Error, defaultButton);
    }
    public static Task<MessageBoxResult> ShowErrorAsync(Window? owner, string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton = default)
    {
        return ShowAsync(owner, message, title, buttons, MessageBoxIcon.Error, defaultButton);
    }
}
