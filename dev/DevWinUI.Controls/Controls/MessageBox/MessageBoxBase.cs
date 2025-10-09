//https://github.com/SuGar0218/WindowedContentDialog

namespace DevWinUI;
public abstract partial class MessageBoxBase
{
    protected MessageBoxBase(
        object? content,
        string? title,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton? defaultButton,
        MessageBoxBaseOptions options)
    {
        _content = content;
        _title = title;
        _buttons = buttons;
        _icon = icon;
        _defaultButton = defaultButton;
        _options = options;
        _dialog = null!;
    }

    protected MessageBoxBase(
        string? message,
        string? title,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton? defaultButton,
        MessageBoxBaseOptions options) : this(CreateSelectableTextBlock(message), title, buttons, icon, defaultButton, options) { }

    protected readonly object? _content;
    protected readonly string? _title;
    protected readonly MessageBoxButtons _buttons;
    protected readonly MessageBoxIcon _icon;
    protected readonly MessageBoxDefaultButton? _defaultButton;
    private readonly MessageBoxBaseOptions _options;
    private IStandaloneContentDialog _dialog;

    protected async Task<MessageBoxResult> ShowAsync()
    {
        _dialog = CreateDialog();
        _dialog.Content = _content;
        _dialog.Title = new MessageBoxHeader
        {
            Icon = _icon,
            Text = _title
        };
        _dialog.FlowDirection = _options.FlowDirection;
        _dialog.RequestedTheme = DetermineTheme();
        DetermineDefaultButton();
        DetermineButtonText();
        return await ShowAndWaitForResultAsync();
    }

    /// <summary>
    /// Create instance and set properties not contained in StandaloneContentDialogBase. 
    /// </summary>
    /// <returns></returns>
    protected abstract IStandaloneContentDialog CreateDialog();

    protected virtual ElementTheme DetermineTheme() => _options.RequestedTheme;

    protected void DetermineDefaultButton()
    {
        _dialog.DefaultButton = _defaultButton switch
        {
            MessageBoxDefaultButton.Button1 => ContentDialogButton.Primary,
            MessageBoxDefaultButton.Button2 => ContentDialogButton.Secondary,
            MessageBoxDefaultButton.Button3 => ContentDialogButton.Close,
            null => ContentDialogButton.None,
            _ => ContentDialogButton.None
        };
    }

    protected void DetermineButtonText()
    {
        switch (_buttons)
        {
            case MessageBoxButtons.OK:
                _dialog.CloseButtonText = NativeMessageBoxButtonTextLoader.OK;
                _dialog.DefaultButton = ContentDialogButton.Close;
                break;

            case MessageBoxButtons.OKCancel:
                _dialog.PrimaryButtonText = NativeMessageBoxButtonTextLoader.OK;
                _dialog.SecondaryButtonText = NativeMessageBoxButtonTextLoader.Cancel;
                break;

            case MessageBoxButtons.YesNo:
                _dialog.PrimaryButtonText = NativeMessageBoxButtonTextLoader.Yes;
                _dialog.SecondaryButtonText = NativeMessageBoxButtonTextLoader.No;
                break;

            case MessageBoxButtons.YesNoCancel:
                _dialog.PrimaryButtonText = NativeMessageBoxButtonTextLoader.Yes;
                _dialog.SecondaryButtonText = NativeMessageBoxButtonTextLoader.No;
                _dialog.CloseButtonText = NativeMessageBoxButtonTextLoader.Cancel;
                break;

            case MessageBoxButtons.AbortRetryIgnore:
                _dialog.PrimaryButtonText = NativeMessageBoxButtonTextLoader.Abort;
                _dialog.SecondaryButtonText = NativeMessageBoxButtonTextLoader.Retry;
                _dialog.CloseButtonText = NativeMessageBoxButtonTextLoader.Ignore;
                break;

            case MessageBoxButtons.RetryCancel:
                _dialog.PrimaryButtonText = NativeMessageBoxButtonTextLoader.Retry;
                _dialog.SecondaryButtonText = NativeMessageBoxButtonTextLoader.Cancel;
                break;

            case MessageBoxButtons.CancelTryContinue:
                _dialog.PrimaryButtonText = NativeMessageBoxButtonTextLoader.Continue;
                _dialog.SecondaryButtonText = NativeMessageBoxButtonTextLoader.TryAgain;
                _dialog.CloseButtonText = NativeMessageBoxButtonTextLoader.Cancel;
                _dialog.DefaultButton = ContentDialogButton.Close;
                break;
        }
    }

    protected async Task<MessageBoxResult> ShowAndWaitForResultAsync()
    {
        ContentDialogResult result = await _dialog.ShowAsync();
        MessageBoxResult[] results = resultGroups[(int)_buttons];
        return result switch
        {
            ContentDialogResult.None => results[^1],
            ContentDialogResult.Primary => results[0],
            ContentDialogResult.Secondary => results[1],
            _ => MessageBoxResult.None,
        };
    }

    private static readonly MessageBoxResult[][] resultGroups = [
        [MessageBoxResult.OK],
        [MessageBoxResult.OK, MessageBoxResult.CANCEL],
        [MessageBoxResult.ABORT, MessageBoxResult.RETRY, MessageBoxResult.IGNORE],
        [MessageBoxResult.YES, MessageBoxResult.NO, MessageBoxResult.CANCEL],
        [MessageBoxResult.YES, MessageBoxResult.NO],
        [MessageBoxResult.RETRY, MessageBoxResult.CANCEL],
        [MessageBoxResult.CONTINUE, MessageBoxResult.TRYAGAIN, MessageBoxResult.CANCEL]
    ];

    private static SelectableTextBlock? CreateSelectableTextBlock(string? text) => string.IsNullOrEmpty(text) ? null : new()
    {
        Text = text,
        TextWrapping = TextWrapping.Wrap,
        HorizontalAlignment = HorizontalAlignment.Stretch
    };
}
