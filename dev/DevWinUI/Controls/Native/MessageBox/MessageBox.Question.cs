namespace DevWinUI;
public partial class MessageBox
{
    public static async Task<MessageBoxResult> ShowQuestionAsync(object? message)
            => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, object? message)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, object? message)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(object? message, string? title)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, object? message, string? title)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, object? message, string? title)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(object? message, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, object? message, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, object? message, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(object? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, object? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, object? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(object? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, object? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, object? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(object? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, object? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, object? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(object? message, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, object? message, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, object? message, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(object? message, string? title, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, object? message, string? title, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, object? message, string? title, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Question, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Question, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Question, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Question, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Question, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Question, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Question, defaultButton, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Question, defaultButton, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Question, defaultButton, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Question, defaultButton, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Question, defaultButton, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Question, defaultButton, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(string? message)
    => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, string? message)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, string? message)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(string? message, string? title)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, string? message, string? title)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, string? message, string? title)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(string? message, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, string? message, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, string? message, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(string? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, string? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, string? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(string? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, string? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, string? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(string? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, string? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, string? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(string? message, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, string? message, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, string? message, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(string? message, string? title, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, string? message, string? title, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, string? message, string? title, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Question, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Question, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Question, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Question, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Question, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Question, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowQuestionAsync(string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.Question, defaultButton, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.Question, defaultButton, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.Question, defaultButton, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.Question, defaultButton, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.Question, defaultButton, options);
    public static async Task<MessageBoxResult> ShowQuestionAsync(bool isModal, Window owner, string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.Question, defaultButton, options);
}
