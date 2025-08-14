namespace DevWinUI;

public partial class MessageBox
{
    public static async Task<MessageBoxResult> ShowAsync(object content)
        => await ShowAsync(false, null, false, content, null, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowAsync(object content, string? title)
        => await ShowAsync(false, null, false, content, title, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowAsync(object content, string? title, MessageBoxButtons buttons)
        => await ShowAsync(false, null, false, content, title, buttons, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowAsync(object content, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton? defaultButton)
        => await ShowAsync(false, null, false, content, title, buttons, defaultButton);

    public static async Task<MessageBoxResult> ShowAsync(object content, bool isResizable)
    => await ShowAsync(false, null, isResizable, content, null, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowAsync(object content, string? title, bool isResizable)
        => await ShowAsync(false, null, isResizable, content, title, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowAsync(object content, string? title, MessageBoxButtons buttons, bool isResizable)
        => await ShowAsync(false, null, isResizable, content, title, buttons, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowAsync(object content, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton? defaultButton, bool isResizable)
        => await ShowAsync(false, null, isResizable, content, title, buttons, defaultButton);

    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window? owner, object? content)
        => await ShowAsync(isModal, owner, false, content, null, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window? owner, object? content, string? title)
        => await ShowAsync(isModal, owner, false, content, title, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window? owner, object? content, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, false, content, title, buttons, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window? owner, object? content, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton? defaultButton)
        => await ShowAsync(isModal, owner, false, content, title, buttons, defaultButton);

    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window? owner, bool isResizable, object? content)
    => await ShowAsync(isModal, owner, isResizable, content, null, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window? owner, bool isResizable, object? content, string? title)
        => await ShowAsync(isModal, owner, isResizable, content, title, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window? owner, bool isResizable, object? content, string? title, MessageBoxButtons buttons)
        => await ShowAsync(isModal, owner, isResizable, content, title, buttons, MessageBoxDefaultButton.Button1);

    public static async Task<MessageBoxResult> ShowAsync(MessageBoxOptions options)
    {
        ElementTheme theme;
        if (options.RequestedTheme is not ElementTheme.Default)
        {
            theme = options.RequestedTheme;
        }
        else if (options.OwnerWindow is not null)
        {
            if (options.OwnerWindow.Content is FrameworkElement root)
            {
                theme = root.ActualTheme;
            }
            else
            {
                theme = options.OwnerWindow.AppWindow.TitleBar.PreferredTheme switch
                {
                    Microsoft.UI.Windowing.TitleBarTheme.UseDefaultAppMode => ElementTheme.Default,
                    Microsoft.UI.Windowing.TitleBarTheme.Light => ElementTheme.Light,
                    Microsoft.UI.Windowing.TitleBarTheme.Dark => ElementTheme.Dark,
                    _ => ElementTheme.Default
                };
            }
        }
        else
        {
            theme = ElementTheme.Default;
        }

        WindowedContentDialog dialog = new()
        {
            Title = options.Title ?? string.Empty,
            Content = options.Content,
            OwnerWindow = options.OwnerWindow,
            SystemBackdrop = options.SystemBackdrop,
            RequestedTheme = theme,
            Underlay = options.Underlay,
            UnderlaySmokeLayer = options.UnderlaySmokeLayer,
            UnderlaySystemBackdrop = options.UnderlaySystemBackdrop,
            HasTitleBar = options.HasTitleBar,
            FlowDirection = options.FlowDirection,
            CenterInParent = options.CenterInParent,
        };

        ContentDialogButton contentDialogDefaultButton = options.DefaultButton switch
        {
            MessageBoxDefaultButton.Button1 => ContentDialogButton.Primary,
            MessageBoxDefaultButton.Button2 => ContentDialogButton.Secondary,
            MessageBoxDefaultButton.Button3 => ContentDialogButton.Close,
            null => ContentDialogButton.None,
            _ => throw new ArgumentException("MessageBoxDefaultButton defaultButton should be in {Button1=0, Button2=256, Button3=512}")
        };
        dialog.DefaultButton = contentDialogDefaultButton;

        switch (options.Buttons)
        {
            case MessageBoxButtons.OK:
                dialog.CloseButtonText = "OK";
                dialog.DefaultButton = ContentDialogButton.Close;
                break;

            case MessageBoxButtons.OKCancel:
                dialog.PrimaryButtonText = "OK";
                dialog.SecondaryButtonText = "Cancel";
                break;

            case MessageBoxButtons.YesNo:
                dialog.PrimaryButtonText = "Yes";
                dialog.SecondaryButtonText = "No";
                break;

            case MessageBoxButtons.YesNoCancel:
                dialog.PrimaryButtonText = "Yes";
                dialog.SecondaryButtonText = "No";
                dialog.CloseButtonText = "Cancel";
                break;

            case MessageBoxButtons.AbortRetryIgnore:
                dialog.PrimaryButtonText = "Abort";
                dialog.SecondaryButtonText = "Retry";
                dialog.CloseButtonText = "Ignore";
                break;

            case MessageBoxButtons.RetryCancel:
                dialog.PrimaryButtonText = "Retry";
                dialog.SecondaryButtonText = "Cancel";
                break;

            case MessageBoxButtons.CancelTryContinue:
                dialog.PrimaryButtonText = "Continue";
                dialog.SecondaryButtonText = "Try again";
                dialog.CloseButtonText = "Cancel";
                dialog.DefaultButton = ContentDialogButton.Close;
                break;
        }

        ContentDialogResult result = await dialog.ShowAsync(options.IsModal);
        var results = MessageBoxResultsOf(options.Buttons);
        return results[result switch
        {
            ContentDialogResult.Primary => 0,
            ContentDialogResult.Secondary => 1,
            ContentDialogResult.None => results.Length - 1,
            _ => throw new ArgumentOutOfRangeException(nameof(result)),
        }];
    }
    public static async Task<MessageBoxResult> ShowAsync(bool isModal, Window? owner, bool isResizable, object? content, string? title, MessageBoxButtons buttons, MessageBoxDefaultButton? defaultButton)
    {
        return await ShowAsync(new MessageBoxOptions
        {
            IsModal = isModal,
            OwnerWindow = owner,
            IsResizable = isResizable,
            Content = content,
            Title = title,
            Buttons = buttons,
            DefaultButton = defaultButton
        });
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

    private static MessageBoxResult[] MessageBoxResultsOf(MessageBoxButtons buttons) => resultGroups[(int) buttons];
}
