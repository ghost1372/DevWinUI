namespace DevWinUI;
public partial class MessageBox
{
    public static async Task<MessageBoxResult> ShowErrorAsync(object? message)
        => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, object? message)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, object? message)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(object? message, string? title)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, object? message, string? title)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, object? message, string? title)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(object? message, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, object? message, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, object? message, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(object? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, object? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, object? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(object? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, object? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, object? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(object? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, object? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, object? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(object? message, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, object? message, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, object? message, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(object? message, string? title, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, object? message, string? title, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, object? message, string? title, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Error, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Error, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Error, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Error, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Error, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Error, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Error, defaultButton, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Error, defaultButton, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Error, defaultButton, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Error, defaultButton, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Error, defaultButton, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Error, defaultButton, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(string? message)
    => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, string? message)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, string? message)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(string? message, string? title)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, string? message, string? title)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, string? message, string? title)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(string? message, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, string? message, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, string? message, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(string? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, string? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, string? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(string? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, string? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, string? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(string? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, string? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, string? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(string? message, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, string? message, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, string? message, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(string? message, string? title, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, string? message, string? title, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, string? message, string? title, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Error, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Error, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Error, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Error, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Error, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Error, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowErrorAsync(string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Error, defaultButton, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Error, defaultButton, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Error, defaultButton, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Error, defaultButton, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Error, defaultButton, options);
    public static async Task<MessageBoxResult> ShowErrorAsync(bool isModal, Window owner, string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Error, defaultButton, options);
}
