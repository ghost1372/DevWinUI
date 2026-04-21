// https://github.com/SuGar0218/SuGarToolkit.WinUI3

using System.Windows.Input;

namespace DevWinUI;

public interface IContentDialog
{
    object? Header { get; set; }
    DataTemplate? HeaderTemplate { get; set; }

    object? Content { get; set; }
    DataTemplate? ContentTemplate { get; set; }

    ContentDialogResult Result { get; }
    ContentDialogButton DefaultButton { get; set; }

    object? PrimaryButtonContent { get; set; }
    DataTemplate? PrimaryButtonTemplate { get; set; }

    object? SecondaryButtonContent { get; set; }
    DataTemplate? SecondaryButtonTemplate { get; set; }

    object? CloseButtonContent { get; set; }
    DataTemplate? CloseButtonTemplate { get; set; }

    bool IsPrimaryButtonEnabled { get; set; }
    bool IsSecondaryButtonEnabled { get; set; }

    event EventHandler? PrimaryButtonClick;
    event EventHandler? SecondaryButtonClick;
    event EventHandler? CloseButtonClick;

    ICommand? PrimaryButtonCommand { get; set; }
    ICommand? SecondaryButtonCommand { get; set; }
    ICommand? CloseButtonCommand { get; set; }

    Style? PrimaryButtonStyle { get; set; }
    Style? SecondaryButtonStyle { get; set; }
    Style? CloseButtonStyle { get; set; }

    string? PrimaryButtonAccessKey { get; set; }
    string? SecondaryButtonAccessKey { get; set; }
    string? CloseButtonAccessKey { get; set; }
}
