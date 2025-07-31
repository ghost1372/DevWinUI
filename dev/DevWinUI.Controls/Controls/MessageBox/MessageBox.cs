namespace DevWinUI;

public partial class MessageBox
{
    public static async Task<MessageBoxResult> ShowAsync(
        object content,
        string? title = null)
        => await ShowAsync(false, null, content, title, MessageBoxButtons.OK);

    public static async Task<MessageBoxResult> ShowAsync(
        object content,
        string? title,
        MessageBoxButtons buttons,
        MessageBoxDefaultButton? defaultButton = MessageBoxDefaultButton.Button1)
        => await ShowAsync(false, null, content, title, buttons, defaultButton);

    public static async Task<MessageBoxResult> ShowAsync(
        bool modal,
        Window owner,
        object content,
        string? title = null) 
        => await ShowAsync(modal, owner, content, title, MessageBoxButtons.OK);

    public static async Task<MessageBoxResult> ShowAsync(
        bool modal,
        Window? owner,
        object? content, string? title,
        MessageBoxButtons buttons,
        MessageBoxDefaultButton? defaultButton = MessageBoxDefaultButton.Button1)
    {
        FrameworkElement? root = owner?.Content as FrameworkElement;
        WindowedContentDialog dialog = new()
        {
            Title = title ?? string.Empty,
            Content = content,
            OwnerWindow = owner,
            RequestedTheme = root is not null ? root.ActualTheme : owner is null ? ElementTheme.Default : owner.AppWindow.TitleBar.PreferredTheme switch
            {
                Microsoft.UI.Windowing.TitleBarTheme.UseDefaultAppMode => ElementTheme.Default,
                Microsoft.UI.Windowing.TitleBarTheme.Light => ElementTheme.Light,
                Microsoft.UI.Windowing.TitleBarTheme.Dark => ElementTheme.Dark,
                _ => ElementTheme.Default
            }
        };

        ContentDialogButton contentDialogDefaultButton = defaultButton switch
        {
            MessageBoxDefaultButton.Button1 => ContentDialogButton.Primary,
            MessageBoxDefaultButton.Button2 => ContentDialogButton.Secondary,
            MessageBoxDefaultButton.Button3 => ContentDialogButton.Close,
            null => ContentDialogButton.None,
            _ => throw new ArgumentException("MessageBoxDefaultButton defaultButton should be in {Button1=0, Button2=256, Button3=512}")
        };
        dialog.DefaultButton = contentDialogDefaultButton;

        switch (buttons)
        {
            case MessageBoxButtons.OK:
                dialog.PrimaryButtonText = "OK";
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

        ContentDialogResult result = await dialog.ShowAsync(modal);
        var results = MessageBoxResultsOf(buttons);
        return results[result switch
        {
            ContentDialogResult.Primary => 0,
            ContentDialogResult.Secondary => 1,
            ContentDialogResult.None => results.Length - 1,
            _ => throw new ArgumentOutOfRangeException(nameof(result)),
        }];
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
