namespace DevWinUI;
public partial class MessageBox
{
    public static async Task<MessageBoxResult> ShowInfoAsync(object content)
        => await ShowAsync(false, null, false, content, null, MessageBoxButtons.OK, MessageBoxImage.Information, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowInfoAsync(object content, string? title)
        => await ShowAsync(false, null, false, content, title, MessageBoxButtons.OK, MessageBoxImage.Information, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowInfoAsync(object content, string? title, MessageBoxButtons buttons)
        => await ShowAsync(false, null, false, content, title, buttons, MessageBoxImage.Information, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowInfoAsync(object content, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton? defaultButton)
        => await ShowAsync(false, null, false, content, title, buttons, MessageBoxImage.Information, defaultButton);

    public static async Task<MessageBoxResult> ShowInfoAsync(object content, bool isResizable)
    => await ShowAsync(false, null, isResizable, content, null, MessageBoxButtons.OK, MessageBoxImage.Information, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowInfoAsync(object content, string? title, bool isResizable)
        => await ShowAsync(false, null, isResizable, content, title, MessageBoxButtons.OK, MessageBoxImage.Information, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowInfoAsync(object content, string? title, MessageBoxButtons buttons, bool isResizable)
        => await ShowAsync(false, null, isResizable, content, title, buttons, MessageBoxImage.Information, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowInfoAsync(object content, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton? defaultButton, bool isResizable)
        => await ShowAsync(false, null, isResizable, content, title, buttons, MessageBoxImage.Information, defaultButton);

    public static async Task<MessageBoxResult> ShowInfoAsync(bool isModal, Window? owner, object? content)
        => await ShowAsync(isModal, owner, false, content, null, MessageBoxButtons.OK, MessageBoxImage.Information, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowInfoAsync(bool isModal, Window? owner, object? content, string? title)
        => await ShowAsync(isModal, owner, false, content, title, MessageBoxButtons.OK, MessageBoxImage.Information, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowInfoAsync(bool isModal, Window? owner, object? content, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, false, content, title, buttons, MessageBoxImage.Information, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowInfoAsync(bool isModal, Window? owner, object? content, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton? defaultButton)
        => await ShowAsync(isModal, owner, false, content, title, buttons, MessageBoxImage.Information, defaultButton);

    public static async Task<MessageBoxResult> ShowInfoAsync(bool isModal, Window? owner, bool isResizable, object? content)
    => await ShowAsync(isModal, owner, isResizable, content, null, MessageBoxButtons.OK, MessageBoxImage.Information, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowInfoAsync(bool isModal, Window? owner, bool isResizable, object? content, string? title)
        => await ShowAsync(isModal, owner, isResizable, content, title, MessageBoxButtons.OK, MessageBoxImage.Information, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowInfoAsync(bool isModal, Window? owner, bool isResizable, object? content, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, isResizable, content, title, buttons, MessageBoxImage.Information, MessageBoxDefaultButton.Button1);
}
