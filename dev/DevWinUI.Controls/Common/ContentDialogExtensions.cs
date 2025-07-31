using System.Reflection;

namespace DevWinUI;
public static partial class ContentDialogExtensions
{
    public static async Task<ContentDialogResult> ShowAsWindowAsync(this ContentDialog contentDialog, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? primaryClick = null, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? secondaryClick = null, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? closeClick = null)
    {
        return await ShowAsWindowAsync(contentDialog, null, true, true);
    }
    public static async Task<ContentDialogResult> ShowAsWindowAsync(this ContentDialog contentDialog, bool isModal, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? primaryClick = null, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? secondaryClick = null, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? closeClick = null)
    {
        return await ShowAsWindowAsync(contentDialog, null, isModal, true);
    }
    public static async Task<ContentDialogResult> ShowAsWindowAsync(this ContentDialog contentDialog, Window? ownerWindow, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? primaryClick = null, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? secondaryClick = null, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? closeClick = null)
    {
        return await ShowAsWindowAsync(contentDialog, ownerWindow, true, true);
    }
    public static async Task<ContentDialogResult> ShowAsWindowAsync(this ContentDialog contentDialog, Window? ownerWindow, bool isModal, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? primaryClick = null, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? secondaryClick = null, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? closeClick = null)
    {
        return await ShowAsWindowAsync(contentDialog, ownerWindow, isModal, true);
    }
    public static async Task<ContentDialogResult> ShowAsWindowWithoutTitleBarAsync(this ContentDialog contentDialog, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? primaryClick = null, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? secondaryClick = null, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? closeClick = null)
    {
        return await ShowAsWindowAsync(contentDialog, null, true, false);
    }
    public static async Task<ContentDialogResult> ShowAsWindowWithoutTitleBarAsync(this ContentDialog contentDialog, Window? ownerWindow, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? primaryClick = null, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? secondaryClick = null, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? closeClick = null)
    {
        return await ShowAsWindowAsync(contentDialog, ownerWindow, true, false);
    }
    public static async Task<ContentDialogResult> ShowAsWindowWithoutTitleBarAsync(this ContentDialog contentDialog, bool isModal, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? primaryClick = null, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? secondaryClick = null, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? closeClick = null)
    {
        return await ShowAsWindowAsync(contentDialog, null, isModal, false);
    }
    public static async Task<ContentDialogResult> ShowAsWindowWithoutTitleBarAsync(this ContentDialog contentDialog, Window? ownerWindow, bool isModal, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? primaryClick = null, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? secondaryClick = null, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? closeClick = null)
    {
        return await ShowAsWindowAsync(contentDialog, ownerWindow, isModal, false);
    }
    public static async Task<ContentDialogResult> ShowAsWindowAsync(this ContentDialog contentDialog, Window? ownerWindow, bool isModal, bool hasTitleBar, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? primaryClick = null, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? secondaryClick = null, TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? closeClick = null)
    {
        WindowedContentDialog window = new()
        {
            Title = contentDialog.Title?.ToString() ?? string.Empty,
            HasTitleBar = hasTitleBar,
            Content = contentDialog.Content,
            PrimaryButtonText = contentDialog.PrimaryButtonText,
            SecondaryButtonText = contentDialog.SecondaryButtonText,
            CloseButtonText = contentDialog.CloseButtonText,
            IsPrimaryButtonEnabled = contentDialog.IsPrimaryButtonEnabled,
            IsSecondaryButtonEnabled = contentDialog.IsSecondaryButtonEnabled,

            PrimaryButtonStyle = contentDialog.PrimaryButtonStyle,
            SecondaryButtonStyle = contentDialog.SecondaryButtonStyle,
            CloseButtonStyle = contentDialog.CloseButtonStyle,

            OwnerWindow = ownerWindow,
            RequestedTheme = contentDialog.RequestedTheme
        };

        if (primaryClick is not null)
            window.PrimaryButtonClick += primaryClick;
        if (secondaryClick is not null)
            window.SecondaryButtonClick += secondaryClick;
        if (closeClick is not null)
            window.CloseButtonClick += closeClick;

        return await window.ShowAsync(isModal);
    }
}
