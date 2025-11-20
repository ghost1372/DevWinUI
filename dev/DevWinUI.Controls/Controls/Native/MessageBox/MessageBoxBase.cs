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
                {
                    MessageBoxButtonString okString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.OK);
                    _dialog.CloseButtonText = okString.Text;
                    _dialog.CloseButtonAccessKey = okString.Key;
                    _dialog.DefaultButton = ContentDialogButton.Close;
                    break;
                }

            case MessageBoxButtons.OKCancel:
                {
                    MessageBoxButtonString okString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.OK);
                    MessageBoxButtonString cancelString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Cancel);
                    _dialog.PrimaryButtonText = okString.Text;
                    _dialog.PrimaryButtonAccessKey = okString.Key;
                    _dialog.CloseButtonText = cancelString.Text;
                    _dialog.CloseButtonAccessKey = cancelString.Key;
                    _dialog.DefaultButton = ContentDialogButton.Close;
                    break;
                }

            case MessageBoxButtons.YesNo:
                {
                    MessageBoxButtonString yesString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Yes);
                    MessageBoxButtonString noString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.No);
                    _dialog.PrimaryButtonText = yesString.Text;
                    _dialog.PrimaryButtonAccessKey = yesString.Key;
                    _dialog.SecondaryButtonText = noString.Text;
                    _dialog.SecondaryButtonAccessKey = noString.Key;
                    break;
                }

            case MessageBoxButtons.YesNoCancel:
                {
                    MessageBoxButtonString yesString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Yes);
                    MessageBoxButtonString noString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.No);
                    MessageBoxButtonString cancelString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Cancel);
                    _dialog.PrimaryButtonText = yesString.Text;
                    _dialog.PrimaryButtonAccessKey = yesString.Key;
                    _dialog.SecondaryButtonText = noString.Text;
                    _dialog.SecondaryButtonAccessKey = noString.Key;
                    _dialog.CloseButtonText = cancelString.Text;
                    _dialog.CloseButtonAccessKey = cancelString.Key;
                    break;
                }

            case MessageBoxButtons.AbortRetryIgnore:
                {
                    MessageBoxButtonString abortString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Abort);
                    MessageBoxButtonString retryString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Retry);
                    MessageBoxButtonString ignoreString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Ignore);
                    _dialog.PrimaryButtonText = abortString.Text;
                    _dialog.PrimaryButtonAccessKey = abortString.Key;
                    _dialog.SecondaryButtonText = retryString.Text;
                    _dialog.SecondaryButtonAccessKey = retryString.Key;
                    _dialog.CloseButtonText = ignoreString.Text;
                    _dialog.CloseButtonAccessKey = ignoreString.Key;
                    break;
                }

            case MessageBoxButtons.RetryCancel:
                {
                    MessageBoxButtonString retryString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Retry);
                    MessageBoxButtonString cancelString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Cancel);
                    _dialog.PrimaryButtonText = retryString.Text;
                    _dialog.PrimaryButtonAccessKey = retryString.Key;
                    _dialog.SecondaryButtonText = cancelString.Text;
                    _dialog.SecondaryButtonAccessKey = cancelString.Key;
                    break;
                }

            case MessageBoxButtons.CancelTryContinue:
                {
                    MessageBoxButtonString continueString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Continue);
                    MessageBoxButtonString tryString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.TryAgain);
                    MessageBoxButtonString cancelString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Cancel);
                    _dialog.PrimaryButtonText = continueString.Text;
                    _dialog.PrimaryButtonAccessKey = continueString.Key;
                    _dialog.SecondaryButtonText = tryString.Text;
                    _dialog.SecondaryButtonAccessKey = tryString.Key;
                    _dialog.CloseButtonText = cancelString.Text;
                    _dialog.CloseButtonAccessKey = cancelString.Key;
                    _dialog.DefaultButton = ContentDialogButton.Close;
                    break;
                }
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

    private struct MessageBoxButtonString
    {
        public MessageBoxButtonString(string text, string key)
        {
            Text = text;
            Key = key;
        }

        public static MessageBoxButtonString FromUser32(string loadedString)
        {
            string text;
            string key;
            int i = loadedString.IndexOf('&');
            if (i == -1)
            {
                text = loadedString;
                key = string.Empty;
            }
            else
            {
                text = loadedString.Remove(i, 1);
                key = loadedString[i + 1].ToString();  // For letters, VirtualKey enum value is the same as unicode.
            }
            return new MessageBoxButtonString(text, key);
        }

        public string Text { get; }

        public string Key { get; }
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
