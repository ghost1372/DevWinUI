//https://github.com/SuGar0218/WindowedContentDialog

using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

public interface IStandaloneContentDialog
{
    public Task<ContentDialogResult> ShowAsync();

    public object? Title { get; set; }
    public object? Content { get; set; }
    public string? PrimaryButtonText { get; set; }
    public string? SecondaryButtonText { get; set; }
    public string? CloseButtonText { get; set; }
    public Brush? Foreground { get; set; }
    public Brush? Background { get; set; }
    public Brush? BorderBrush { get; set; }
    public Thickness BorderThickness { get; set; }
    public DataTemplate? TitleTemplate { get; set; }
    public DataTemplate? ContentTemplate { get; set; }
    public ContentDialogButton DefaultButton { get; set; }
    public bool IsPrimaryButtonEnabled { get; set; }
    public bool IsSecondaryButtonEnabled { get; set; }
    public Style? PrimaryButtonStyle { get; set; }
    public Style? SecondaryButtonStyle { get; set; }
    public Style? CloseButtonStyle { get; set; }
    public UnderlayMode Underlay { get; set; }
    public UnderlaySystemBackdropOptions UnderlaySystemBackdrop { get; set; }
    public FlowDirection FlowDirection { get; set; }

    /// <summary>
    /// ElementTheme.Default is treated as following owner window
    /// </summary>
    public ElementTheme RequestedTheme { get; set; }

    public ElementTheme DetermineTheme() => RequestedTheme;

    public IList<KeyboardAccelerator> PrimaryButtonKeyboardAccelerators { get; }
    public IList<KeyboardAccelerator> SecondaryButtonKeyboardAccelerators { get; }
    public IList<KeyboardAccelerator> CloseButtonKeyboardAccelerators { get; }
}
