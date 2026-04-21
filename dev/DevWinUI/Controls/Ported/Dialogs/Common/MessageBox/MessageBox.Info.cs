namespace DevWinUI;
public partial class MessageBox
{
    public static Task<MessageBoxResult> ShowInfoAsync(string message)
    {
        return ShowAsync(null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
    }

    public static Task<MessageBoxResult> ShowInfoAsync(string? message, string? title)
    {
        return ShowAsync(null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
    }

    public static Task<MessageBoxResult> ShowInfoAsync(Window? owner, string message)
    {
        return ShowAsync(owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
    }

    public static Task<MessageBoxResult> ShowInfoAsync(Window? owner, string? message, string? title)
    {
        return ShowAsync(owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
    }
    public static Task<MessageBoxResult> ShowInfoAsync(string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton = default)
    {
        return ShowAsync(null, message, title, buttons, MessageBoxIcon.Information, defaultButton);
    }

    public static Task<MessageBoxResult> ShowInfoAsync(Window? owner, string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton = default)
    {
        return ShowAsync(owner, message, title, buttons, MessageBoxIcon.Information, defaultButton);
    }
}
