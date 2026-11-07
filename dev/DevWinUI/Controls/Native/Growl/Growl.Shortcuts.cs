namespace DevWinUI;

public static partial class Growl
{
    public static GrowlHandle Show(GrowlCardSeverity severity, string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => Resolve(null).Show(severity, title, subTitle, message, canClose, autoClose);

    public static GrowlHandle ShowSuccess(string title) => Resolve(null).ShowSuccess(title);
    public static GrowlHandle ShowSuccess(string title, bool canClose, GrowlCardAutoClose autoClose) => Resolve(null).ShowSuccess(title, canClose, autoClose);
    public static GrowlHandle ShowSuccess(string title, string subTitle) => Resolve(null).ShowSuccess(title, subTitle);
    public static GrowlHandle ShowSuccess(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => Resolve(null).ShowSuccess(title, subTitle, canClose, autoClose);
    public static GrowlHandle ShowSuccess(string title, string subTitle, object? message) => Resolve(null).ShowSuccess(title, subTitle, message);
    public static GrowlHandle ShowSuccess(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => Resolve(null).ShowSuccess(title, subTitle, message, canClose, autoClose);

    public static GrowlHandle ShowWarning(string title) => Resolve(null).ShowWarning(title);
    public static GrowlHandle ShowWarning(string title, bool canClose, GrowlCardAutoClose autoClose) => Resolve(null).ShowWarning(title, canClose, autoClose);
    public static GrowlHandle ShowWarning(string title, string subTitle) => Resolve(null).ShowWarning(title, subTitle);
    public static GrowlHandle ShowWarning(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => Resolve(null).ShowWarning(title, subTitle, canClose, autoClose);
    public static GrowlHandle ShowWarning(string title, string subTitle, object? message) => Resolve(null).ShowWarning(title, subTitle, message);
    public static GrowlHandle ShowWarning(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => Resolve(null).ShowWarning(title, subTitle, message, canClose, autoClose);

    public static GrowlHandle ShowError(string title) => Resolve(null).ShowError(title);
    public static GrowlHandle ShowError(string title, bool canClose, GrowlCardAutoClose autoClose) => Resolve(null).ShowError(title, canClose, autoClose);
    public static GrowlHandle ShowError(string title, string subTitle) => Resolve(null).ShowError(title, subTitle);
    public static GrowlHandle ShowError(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => Resolve(null).ShowError(title, subTitle, canClose, autoClose);
    public static GrowlHandle ShowError(string title, string subTitle, object? message) => Resolve(null).ShowError(title, subTitle, message);
    public static GrowlHandle ShowError(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => Resolve(null).ShowError(title, subTitle, message, canClose, autoClose);

    public static GrowlHandle ShowInfo(string title) => Resolve(null).ShowInfo(title);
    public static GrowlHandle ShowInfo(string title, bool canClose, GrowlCardAutoClose autoClose) => Resolve(null).ShowInfo(title, canClose, autoClose);
    public static GrowlHandle ShowInfo(string title, string subTitle) => Resolve(null).ShowInfo(title, subTitle);
    public static GrowlHandle ShowInfo(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => Resolve(null).ShowInfo(title, subTitle, canClose, autoClose);
    public static GrowlHandle ShowInfo(string title, string subTitle, object? message) => Resolve(null).ShowInfo(title, subTitle, message);
    public static GrowlHandle ShowInfo(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => Resolve(null).ShowInfo(title, subTitle, message, canClose, autoClose);

    public static GrowlHandle ShowAttention(string title) => Resolve(null).ShowAttention(title);
    public static GrowlHandle ShowAttention(string title, bool canClose, GrowlCardAutoClose autoClose) => Resolve(null).ShowAttention(title, canClose, autoClose);
    public static GrowlHandle ShowAttention(string title, string subTitle) => Resolve(null).ShowAttention(title, subTitle);
    public static GrowlHandle ShowAttention(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => Resolve(null).ShowAttention(title, subTitle, canClose, autoClose);
    public static GrowlHandle ShowAttention(string title, string subTitle, object? message) => Resolve(null).ShowAttention(title, subTitle, message);
    public static GrowlHandle ShowAttention(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => Resolve(null).ShowAttention(title, subTitle, message, canClose, autoClose);

    public static GrowlHandle ShowGlobal(GrowlCardSeverity severity, string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose)
    {
        return GrowlGlobal.Show(new GrowlOptions
        {
            Title = title,
            SubTitle = subTitle,
            Message = message,
            Severity = severity,
            ShowCloseButton = canClose,
            AutoClose = autoClose,
            Scope = GrowlScope.Global
        });
    }

    public static GrowlHandle ShowSuccessGlobal(string title) => ShowGlobal(GrowlCardSeverity.Success, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public static GrowlHandle ShowSuccessGlobal(string title, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobal(GrowlCardSeverity.Success, title, string.Empty, null, canClose, autoClose);
    public static GrowlHandle ShowSuccessGlobal(string title, string subTitle) => ShowGlobal(GrowlCardSeverity.Success, title, subTitle, null, true, GrowlCardAutoClose.None);
    public static GrowlHandle ShowSuccessGlobal(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobal(GrowlCardSeverity.Success, title, subTitle, null, canClose, autoClose);
    public static GrowlHandle ShowSuccessGlobal(string title, string subTitle, object? message) => ShowGlobal(GrowlCardSeverity.Success, title, subTitle, message, true, GrowlCardAutoClose.None);
    public static GrowlHandle ShowSuccessGlobal(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobal(GrowlCardSeverity.Success, title, subTitle, message, canClose, autoClose);

    public static GrowlHandle ShowWarningGlobal(string title) => ShowGlobal(GrowlCardSeverity.Warning, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public static GrowlHandle ShowWarningGlobal(string title, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobal(GrowlCardSeverity.Warning, title, string.Empty, null, canClose, autoClose);
    public static GrowlHandle ShowWarningGlobal(string title, string subTitle) => ShowGlobal(GrowlCardSeverity.Warning, title, subTitle, null, true, GrowlCardAutoClose.None);
    public static GrowlHandle ShowWarningGlobal(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobal(GrowlCardSeverity.Warning, title, subTitle, null, canClose, autoClose);
    public static GrowlHandle ShowWarningGlobal(string title, string subTitle, object? message) => ShowGlobal(GrowlCardSeverity.Warning, title, subTitle, message, true, GrowlCardAutoClose.None);
    public static GrowlHandle ShowWarningGlobal(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobal(GrowlCardSeverity.Warning, title, subTitle, message, canClose, autoClose);

    public static GrowlHandle ShowErrorGlobal(string title) => ShowGlobal(GrowlCardSeverity.Error, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public static GrowlHandle ShowErrorGlobal(string title, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobal(GrowlCardSeverity.Error, title, string.Empty, null, canClose, autoClose);
    public static GrowlHandle ShowErrorGlobal(string title, string subTitle) => ShowGlobal(GrowlCardSeverity.Error, title, subTitle, null, true, GrowlCardAutoClose.None);
    public static GrowlHandle ShowErrorGlobal(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobal(GrowlCardSeverity.Error, title, subTitle, null, canClose, autoClose);
    public static GrowlHandle ShowErrorGlobal(string title, string subTitle, object? message) => ShowGlobal(GrowlCardSeverity.Error, title, subTitle, message, true, GrowlCardAutoClose.None);
    public static GrowlHandle ShowErrorGlobal(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobal(GrowlCardSeverity.Error, title, subTitle, message, canClose, autoClose);

    public static GrowlHandle ShowInfoGlobal(string title) => ShowGlobal(GrowlCardSeverity.Info, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public static GrowlHandle ShowInfoGlobal(string title, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobal(GrowlCardSeverity.Info, title, string.Empty, null, canClose, autoClose);
    public static GrowlHandle ShowInfoGlobal(string title, string subTitle) => ShowGlobal(GrowlCardSeverity.Info, title, subTitle, null, true, GrowlCardAutoClose.None);
    public static GrowlHandle ShowInfoGlobal(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobal(GrowlCardSeverity.Info, title, subTitle, null, canClose, autoClose);
    public static GrowlHandle ShowInfoGlobal(string title, string subTitle, object? message) => ShowGlobal(GrowlCardSeverity.Info, title, subTitle, message, true, GrowlCardAutoClose.None);
    public static GrowlHandle ShowInfoGlobal(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobal(GrowlCardSeverity.Info, title, subTitle, message, canClose, autoClose);

    public static GrowlHandle ShowAttentionGlobal(string title) => ShowGlobal(GrowlCardSeverity.Attention, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public static GrowlHandle ShowAttentionGlobal(string title, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobal(GrowlCardSeverity.Attention, title, string.Empty, null, canClose, autoClose);
    public static GrowlHandle ShowAttentionGlobal(string title, string subTitle) => ShowGlobal(GrowlCardSeverity.Attention, title, subTitle, null, true, GrowlCardAutoClose.None);
    public static GrowlHandle ShowAttentionGlobal(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobal(GrowlCardSeverity.Attention, title, subTitle, null, canClose, autoClose);
    public static GrowlHandle ShowAttentionGlobal(string title, string subTitle, object? message) => ShowGlobal(GrowlCardSeverity.Attention, title, subTitle, message, true, GrowlCardAutoClose.None);
    public static GrowlHandle ShowAttentionGlobal(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobal(GrowlCardSeverity.Attention, title, subTitle, message, canClose, autoClose);

    public static Task<GrowlHandle> ShowAsync(GrowlCardSeverity severity, string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose)
    {
        return ShowAsync(CreateOptions(severity, title, subTitle, message, canClose, autoClose, GrowlScope.InApp));
    }

    public static Task<GrowlHandle> ShowGlobalAsync(GrowlCardSeverity severity, string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose)
    {
        return ShowAsync(CreateOptions(severity, title, subTitle, message, canClose, autoClose, GrowlScope.Global));
    }

    private static GrowlOptions CreateOptions(GrowlCardSeverity severity, string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose, GrowlScope scope)
    {
        return new GrowlOptions
        {
            Title = title,
            SubTitle = subTitle,
            Message = message,
            Severity = severity,
            ShowCloseButton = canClose,
            AutoClose = autoClose,
            Scope = scope
        };
    }

    public static Task<GrowlHandle> ShowSuccessAsync(string title) => ShowAsync(GrowlCardSeverity.Success, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowSuccessAsync(string title, bool canClose, GrowlCardAutoClose autoClose) => ShowAsync(GrowlCardSeverity.Success, title, string.Empty, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowSuccessAsync(string title, string subTitle) => ShowAsync(GrowlCardSeverity.Success, title, subTitle, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowSuccessAsync(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => ShowAsync(GrowlCardSeverity.Success, title, subTitle, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowSuccessAsync(string title, string subTitle, object? message) => ShowAsync(GrowlCardSeverity.Success, title, subTitle, message, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowSuccessAsync(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => ShowAsync(GrowlCardSeverity.Success, title, subTitle, message, canClose, autoClose);

    public static Task<GrowlHandle> ShowWarningAsync(string title) => ShowAsync(GrowlCardSeverity.Warning, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowWarningAsync(string title, bool canClose, GrowlCardAutoClose autoClose) => ShowAsync(GrowlCardSeverity.Warning, title, string.Empty, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowWarningAsync(string title, string subTitle) => ShowAsync(GrowlCardSeverity.Warning, title, subTitle, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowWarningAsync(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => ShowAsync(GrowlCardSeverity.Warning, title, subTitle, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowWarningAsync(string title, string subTitle, object? message) => ShowAsync(GrowlCardSeverity.Warning, title, subTitle, message, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowWarningAsync(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => ShowAsync(GrowlCardSeverity.Warning, title, subTitle, message, canClose, autoClose);

    public static Task<GrowlHandle> ShowErrorAsync(string title) => ShowAsync(GrowlCardSeverity.Error, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowErrorAsync(string title, bool canClose, GrowlCardAutoClose autoClose) => ShowAsync(GrowlCardSeverity.Error, title, string.Empty, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowErrorAsync(string title, string subTitle) => ShowAsync(GrowlCardSeverity.Error, title, subTitle, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowErrorAsync(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => ShowAsync(GrowlCardSeverity.Error, title, subTitle, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowErrorAsync(string title, string subTitle, object? message) => ShowAsync(GrowlCardSeverity.Error, title, subTitle, message, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowErrorAsync(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => ShowAsync(GrowlCardSeverity.Error, title, subTitle, message, canClose, autoClose);

    public static Task<GrowlHandle> ShowInfoAsync(string title) => ShowAsync(GrowlCardSeverity.Info, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowInfoAsync(string title, bool canClose, GrowlCardAutoClose autoClose) => ShowAsync(GrowlCardSeverity.Info, title, string.Empty, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowInfoAsync(string title, string subTitle) => ShowAsync(GrowlCardSeverity.Info, title, subTitle, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowInfoAsync(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => ShowAsync(GrowlCardSeverity.Info, title, subTitle, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowInfoAsync(string title, string subTitle, object? message) => ShowAsync(GrowlCardSeverity.Info, title, subTitle, message, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowInfoAsync(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => ShowAsync(GrowlCardSeverity.Info, title, subTitle, message, canClose, autoClose);

    public static Task<GrowlHandle> ShowAttentionAsync(string title) => ShowAsync(GrowlCardSeverity.Attention, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowAttentionAsync(string title, bool canClose, GrowlCardAutoClose autoClose) => ShowAsync(GrowlCardSeverity.Attention, title, string.Empty, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowAttentionAsync(string title, string subTitle) => ShowAsync(GrowlCardSeverity.Attention, title, subTitle, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowAttentionAsync(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => ShowAsync(GrowlCardSeverity.Attention, title, subTitle, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowAttentionAsync(string title, string subTitle, object? message) => ShowAsync(GrowlCardSeverity.Attention, title, subTitle, message, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowAttentionAsync(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => ShowAsync(GrowlCardSeverity.Attention, title, subTitle, message, canClose, autoClose);

    public static Task<GrowlHandle> ShowSuccessGlobalAsync(string title) => ShowGlobalAsync(GrowlCardSeverity.Success, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowSuccessGlobalAsync(string title, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobalAsync(GrowlCardSeverity.Success, title, string.Empty, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowSuccessGlobalAsync(string title, string subTitle) => ShowGlobalAsync(GrowlCardSeverity.Success, title, subTitle, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowSuccessGlobalAsync(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobalAsync(GrowlCardSeverity.Success, title, subTitle, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowSuccessGlobalAsync(string title, string subTitle, object? message) => ShowGlobalAsync(GrowlCardSeverity.Success, title, subTitle, message, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowSuccessGlobalAsync(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobalAsync(GrowlCardSeverity.Success, title, subTitle, message, canClose, autoClose);

    public static Task<GrowlHandle> ShowWarningGlobalAsync(string title) => ShowGlobalAsync(GrowlCardSeverity.Warning, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowWarningGlobalAsync(string title, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobalAsync(GrowlCardSeverity.Warning, title, string.Empty, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowWarningGlobalAsync(string title, string subTitle) => ShowGlobalAsync(GrowlCardSeverity.Warning, title, subTitle, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowWarningGlobalAsync(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobalAsync(GrowlCardSeverity.Warning, title, subTitle, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowWarningGlobalAsync(string title, string subTitle, object? message) => ShowGlobalAsync(GrowlCardSeverity.Warning, title, subTitle, message, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowWarningGlobalAsync(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobalAsync(GrowlCardSeverity.Warning, title, subTitle, message, canClose, autoClose);

    public static Task<GrowlHandle> ShowErrorGlobalAsync(string title) => ShowGlobalAsync(GrowlCardSeverity.Error, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowErrorGlobalAsync(string title, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobalAsync(GrowlCardSeverity.Error, title, string.Empty, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowErrorGlobalAsync(string title, string subTitle) => ShowGlobalAsync(GrowlCardSeverity.Error, title, subTitle, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowErrorGlobalAsync(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobalAsync(GrowlCardSeverity.Error, title, subTitle, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowErrorGlobalAsync(string title, string subTitle, object? message) => ShowGlobalAsync(GrowlCardSeverity.Error, title, subTitle, message, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowErrorGlobalAsync(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobalAsync(GrowlCardSeverity.Error, title, subTitle, message, canClose, autoClose);

    public static Task<GrowlHandle> ShowInfoGlobalAsync(string title) => ShowGlobalAsync(GrowlCardSeverity.Info, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowInfoGlobalAsync(string title, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobalAsync(GrowlCardSeverity.Info, title, string.Empty, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowInfoGlobalAsync(string title, string subTitle) => ShowGlobalAsync(GrowlCardSeverity.Info, title, subTitle, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowInfoGlobalAsync(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobalAsync(GrowlCardSeverity.Info, title, subTitle, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowInfoGlobalAsync(string title, string subTitle, object? message) => ShowGlobalAsync(GrowlCardSeverity.Info, title, subTitle, message, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowInfoGlobalAsync(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobalAsync(GrowlCardSeverity.Info, title, subTitle, message, canClose, autoClose);

    public static Task<GrowlHandle> ShowAttentionGlobalAsync(string title) => ShowGlobalAsync(GrowlCardSeverity.Attention, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowAttentionGlobalAsync(string title, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobalAsync(GrowlCardSeverity.Attention, title, string.Empty, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowAttentionGlobalAsync(string title, string subTitle) => ShowGlobalAsync(GrowlCardSeverity.Attention, title, subTitle, null, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowAttentionGlobalAsync(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobalAsync(GrowlCardSeverity.Attention, title, subTitle, null, canClose, autoClose);
    public static Task<GrowlHandle> ShowAttentionGlobalAsync(string title, string subTitle, object? message) => ShowGlobalAsync(GrowlCardSeverity.Attention, title, subTitle, message, true, GrowlCardAutoClose.None);
    public static Task<GrowlHandle> ShowAttentionGlobalAsync(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => ShowGlobalAsync(GrowlCardSeverity.Attention, title, subTitle, message, canClose, autoClose);

}
