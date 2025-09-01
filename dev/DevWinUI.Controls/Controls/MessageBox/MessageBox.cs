namespace DevWinUI;
public partial class MessageBox : MessageBoxBase
{
    protected MessageBox(bool isModal, Window? owner, object? message, string? title, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton? defaultButton, MessageBoxOptions options) : base(message, title, buttons, icon, defaultButton, options)
    {
        _isModal = isModal;
        _owner = owner;
        _options = options;
    }

    protected MessageBox(bool isModal, Window? owner, string? message, string? title, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton? defaultButton, MessageBoxOptions options) : base(message, title, buttons, icon, defaultButton, options)
    {
        _isModal = isModal;
        _owner = owner;
        _options = options;
    }

    private readonly bool _isModal;
    private readonly Window? _owner;
    private readonly MessageBoxOptions _options;

    public static async Task<MessageBoxResult> ShowAsync(object? message)
        => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(object? message, string? title)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, string? title)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, string? title)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(object? message, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(object? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(object? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(object? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(object? message, MessageBoxIcon icon)
        => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, MessageBoxIcon icon)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, MessageBoxIcon icon)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(object? message, string? title, MessageBoxIcon icon)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, string? title, MessageBoxIcon icon)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, string? title, MessageBoxIcon icon)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(object? message, MessageBoxIcon icon, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, MessageBoxIcon icon, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, MessageBoxIcon icon, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(object? message, string? title, MessageBoxIcon icon, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, string? title, MessageBoxIcon icon, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, string? title, MessageBoxIcon icon, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(object? message, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(object? message, string? title, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, string? title, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, string? title, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.None, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.None, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.None, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.None, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.None, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.None, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.None, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.None, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.None, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.None, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.None, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.None, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(object? message, MessageBoxIcon icon, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, null, buttons, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, MessageBoxIcon icon, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, null, buttons, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, MessageBoxIcon icon, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, null, buttons, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(object? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, title, buttons, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, title, buttons, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, title, buttons, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(object? message, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(object? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(object? message, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, null, buttons, icon, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, null, buttons, icon, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, null, buttons, icon, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(object? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, title, buttons, icon, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, title, buttons, icon, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, title, buttons, icon, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(object? message, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, icon, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, icon, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, icon, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(object? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, icon, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, object? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, icon, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, object? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, icon, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(string? message)
    => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(string? message, string? title)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, string? title)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, string? title)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(string? message, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(string? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(string? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(string? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, string? title, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(string? message, MessageBoxIcon icon)
        => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, MessageBoxIcon icon)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, MessageBoxIcon icon)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(string? message, string? title, MessageBoxIcon icon)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, string? title, MessageBoxIcon icon)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, string? title, MessageBoxIcon icon)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(string? message, MessageBoxIcon icon, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, MessageBoxIcon icon, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, MessageBoxIcon icon, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(string? message, string? title, MessageBoxIcon icon, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, string? title, MessageBoxIcon icon, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, string? title, MessageBoxIcon icon, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(string? message, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(string? message, string? title, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, string? title, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, string? title, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.None, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.None, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.None, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.None, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.None, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.None, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, MessageBoxIcon.None, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, MessageBoxIcon.None, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, MessageBoxIcon.None, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, MessageBoxIcon.None, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, MessageBoxIcon.None, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, MessageBoxIcon.None, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(string? message, MessageBoxIcon icon, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, null, buttons, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, MessageBoxIcon icon, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, null, buttons, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, MessageBoxIcon icon, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, null, buttons, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(string? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons)
        => await ShowAsync(false, null, message, title, buttons, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons)
        => await ShowAsync(isModal, null, message, title, buttons, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, message, title, buttons, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(string? message, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(string? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, icon, MessageBoxDefaultButton.Button1, options);
    public static async Task<MessageBoxResult> ShowAsync(string? message, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, null, buttons, icon, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, null, buttons, icon, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, null, buttons, icon, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(string? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(false, null, message, title, buttons, icon, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, null, message, title, buttons, icon, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        => await ShowAsync(isModal, owner, message, title, buttons, icon, defaultButton, MessageBoxOptions.Default);
    public static async Task<MessageBoxResult> ShowAsync(string? message, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, null, buttons, icon, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, null, buttons, icon, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, null, buttons, icon, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(string? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(false, null, message, title, buttons, icon, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, string? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, null, message, title, buttons, icon, defaultButton, options);
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window owner, string? message, string? title, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => await ShowAsync(isModal, owner, message, title, buttons, icon, defaultButton, options);

    /// <summary>
    /// Show text messages in a MessageBox with a similar appearance to WinUI 3 ContentDialog.
    /// </summary>
    /// <param name="isModal">Whether the MessageBox is a modal window.</param>
    /// <param name="owner">Owner/Parent window of the MessageBox</param>
    /// <param name="message">Text message displayed in body area</param>
    /// <param name="title">Text text displayed in header area</param>
    /// <param name="buttons">The button combination displayed at the bottom of MessageBox</param>
    /// <param name="icon">MessageBox icon</param>
    /// <param name="defaultButton">Which button should be focused initially</param>
    /// <param name="options">Other style settings like SystemBackdrop.</param>
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window? owner, string? message, string? title, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton? defaultButton, MessageBoxOptions options)
        => await new MessageBox(isModal, owner, message, title, buttons, icon, defaultButton, options).ShowAsync();
    
    /// <summary>
    /// Show text messages in a MessageBox with a similar appearance to WinUI 3 ContentDialog.
    /// </summary>
    /// <param name="isModal">Whether the MessageBox is a modal window.</param>
    /// <param name="owner">Owner/Parent window of the MessageBox</param>
    /// <param name="message">Text object</param>
    /// <param name="title">Text text displayed in header area</param>
    /// <param name="buttons">The button combination displayed at the bottom of MessageBox</param>
    /// <param name="icon">MessageBox icon</param>
    /// <param name="defaultButton">Which button should be focused initially</param>
    /// <param name="options">Other style settings like SystemBackdrop.</param>
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window? owner, object? message, string? title, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton? defaultButton, MessageBoxOptions options)
        => await new MessageBox(isModal, owner, message, title, buttons, icon, defaultButton, options).ShowAsync();
    

    protected override IStandaloneContentDialog CreateDialog() => new WindowedContentDialog
    {
        WindowTitle = _title,
        IsModal = _isModal,
        OwnerWindow = _owner,
        SystemBackdrop = _options.SystemBackdrop,
        CenterInParent = _options.CenterInParent,
        HasTitleBar = _options.HasTitleBar,
        IsResizable = _options.IsResizable,
        Underlay = _options.Underlay,
        UnderlaySystemBackdrop = _options.UnderlaySystemBackdrop,
    };
}
