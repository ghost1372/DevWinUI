namespace DevWinUI;

public partial class MessageBox
{
    public static Task<MessageBoxResult> ShowWarningAsync(string message)
    {
        return ShowAsync(null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
    }

    public static Task<MessageBoxResult> ShowWarningAsync(string? message, string? title)
    {
        return ShowAsync(null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
    }

    public static Task<MessageBoxResult> ShowWarningAsync(Window? owner, string message)
    {
        return ShowAsync(owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
    }

    public static Task<MessageBoxResult> ShowWarningAsync(Window? owner, string? message, string? title)
    {
        return ShowAsync(owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
    }
    public static Task<MessageBoxResult> ShowWarningAsync(string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton = default)
    {
        return ShowAsync(null, message, title, buttons, MessageBoxIcon.Warning, defaultButton);
    }
    public static Task<MessageBoxResult> ShowWarningAsync(Window? owner, string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton = default)
    {
        return ShowAsync(owner, message, title, buttons, MessageBoxIcon.Warning, defaultButton);
    }
}
