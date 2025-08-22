using Microsoft.UI.Windowing;

namespace DevWinUI;
public abstract class StandaloneContentDialogBase
{
    public abstract Task<ContentDialogResult> ShowAsync();

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
    public bool IsPrimaryButtonEnabled { get; set; } = true;
    public bool IsSecondaryButtonEnabled { get; set; } = true;
    public Style PrimaryButtonStyle { get; set; } = DefaultButtonStyle;
    public Style SecondaryButtonStyle { get; set; } = DefaultButtonStyle;
    public Style CloseButtonStyle { get; set; } = DefaultButtonStyle;
    public ElementTheme RequestedTheme { get; set; }
    public FlowDirection FlowDirection { get; set; }
    public UnderlayMode Underlay { get; set; } = UnderlayMode.SmokeLayer;
    public UnderlaySystemBackdropOptions UnderlaySystemBackdrop { get; set; } = new UnderlaySystemBackdropOptions();
    public UnderlaySmokeLayerOptions UnderlaySmokeLayer { get; set; } = new UnderlaySmokeLayerOptions();

    protected static Style DefaultButtonStyle => (Style)Application.Current.Resources["DefaultButtonStyle"];
    protected static Color SmokeFillColor => (Color)Application.Current.Resources["SmokeFillColorDefault"];

    protected void SizeToXamlRoot(FrameworkElement element, Window window)
    {
        element.Width = window.Content.XamlRoot.Size.Width;

        switch (Underlay)
        {
            case UnderlayMode.SmokeLayer:
                element.Height = window.Content.XamlRoot.Size.Height;
                break;

            case UnderlayMode.SystemBackdrop:
                element.Height = UnderlaySystemBackdrop.CoverMode == UnderlayCoverMode.Full ? window.Content.XamlRoot.Size.Height : window.Content.XamlRoot.Size.Height - GetTitleBarOffset(window);
                break;
        }
    }

    public int GetTitleBarOffset(Window window)
    {
        return window.AppWindow.TitleBar.PreferredHeightOption switch
        {
            TitleBarHeightOption.Standard => 32,
            TitleBarHeightOption.Tall => 48,
            _ => 0
        };
    }
}
