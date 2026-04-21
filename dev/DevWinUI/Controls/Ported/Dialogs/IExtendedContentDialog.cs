// https://github.com/SuGar0218/SuGarToolkit.WinUI3

namespace DevWinUI;

public interface IExtendedContentDialog : IContentDialog
{
    object? ExtendedHeader { get; set; }
    DataTemplate? ExtendedHeaderTemplate { get; set; }
}
