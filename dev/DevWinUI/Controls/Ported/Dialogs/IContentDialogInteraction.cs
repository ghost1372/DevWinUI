// https://github.com/SuGar0218/SuGarToolkit.WinUI3

namespace DevWinUI;

public interface IContentDialogInteraction
{
    Task<ContentDialogResult> ShowAsync();
}
