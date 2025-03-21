using Microsoft.UI.Xaml.Input;

namespace DevWinUI;
public static partial class ContentDialogEx
{
    private static TaskCompletionSource<ContentDialog> _contentDialogShowRequest;

    public static async Task<ContentDialogResult> ShowAsyncQueue(this ContentDialog dialog)
    {
        while (_contentDialogShowRequest != null)
        {
            await _contentDialogShowRequest.Task;
        }

        var request = _contentDialogShowRequest = new TaskCompletionSource<ContentDialog>();
        var result = await dialog.ShowAsync();
        _contentDialogShowRequest = null;
        request.SetResult(dialog);

        return result;
    }

    public static async Task<ContentDialogResult> ShowAsyncDraggable(this ContentDialog dialog)
    {
        dialog.ManipulationDelta += delegate (object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (!e.IsInertial)
                dialog.Margin = new Thickness(
                    dialog.Margin.Left + e.Delta.Translation.X,
                    dialog.Margin.Top + e.Delta.Translation.Y,
                    dialog.Margin.Left - e.Delta.Translation.X,
                    dialog.Margin.Top - e.Delta.Translation.Y);
        };
        var result = await dialog.ShowAsync();
        return result;
    }

    public static async Task<ContentDialogResult> ShowAsyncQueueDraggable(this ContentDialog dialog)
    {
        while (_contentDialogShowRequest != null)
        {
            await _contentDialogShowRequest.Task;
        }
        dialog.ManipulationDelta += delegate (object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (!e.IsInertial)
                dialog.Margin = new Thickness(
                    dialog.Margin.Left + e.Delta.Translation.X,
                    dialog.Margin.Top + e.Delta.Translation.Y,
                    dialog.Margin.Left - e.Delta.Translation.X,
                    dialog.Margin.Top - e.Delta.Translation.Y);
        };
        var request = _contentDialogShowRequest = new TaskCompletionSource<ContentDialog>();
        var result = await dialog.ShowAsync();
        _contentDialogShowRequest = null;
        request.SetResult(dialog);

        return result;
    }
}
