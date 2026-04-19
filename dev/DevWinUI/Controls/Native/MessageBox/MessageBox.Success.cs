namespace DevWinUI;
public partial class MessageBox
{
    public static async Task<MessageBoxResult> ShowSuccessAsync(object? message)
            => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, object? message)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, object? message)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(object? message, string? title)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, object? message, string? title)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, object? message, string? title)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(object? message, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, object? message, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, object? message, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(object? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, object? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, object? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(object? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, object? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, object? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(object? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, object? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, object? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(object? message, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, object? message, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, object? message, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(object? message, string? title, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, object? message, string? title, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, object? message, string? title, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Success, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Success, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Success, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Success, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Success, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Success, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Success, defaultButton, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Success, defaultButton, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Success, defaultButton, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Success, defaultButton, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Success, defaultButton, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Success, defaultButton, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(string? message)
    => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, string? message)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, string? message)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(string? message, string? title)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, string? message, string? title)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, string? message, string? title)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(string? message, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, string? message, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, string? message, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(string? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, string? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, string? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(string? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, string? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, string? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(string? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, string? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, string? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(string? message, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, string? message, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, string? message, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(string? message, string? title, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, string? message, string? title, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, string? message, string? title, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Success, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Success, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Success, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Success, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Success, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Success, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Success, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowSuccessAsync(string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Success, defaultButton, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Success, defaultButton, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Success, defaultButton, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Success, defaultButton, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Success, defaultButton, options);
    public static async Task<MessageBoxResult> ShowSuccessAsync(bool isModal, Window owner, string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Success, defaultButton, options);
}
