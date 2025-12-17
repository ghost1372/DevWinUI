using Microsoft.UI.Xaml.Input;

namespace DevWinUI;
public static partial class ContentDialogExtensions
{
    private static TaskCompletionSource<ContentDialog> _contentDialogShowRequest;

    extension(ContentDialog dialog)
    {
        internal async Task<ContentDialogResult> ShowAsyncQueueBase(ContentDialogPlacement? contentDialogPlacement)
        {
            while (_contentDialogShowRequest != null)
            {
                await _contentDialogShowRequest.Task;
            }

            var request = _contentDialogShowRequest = new TaskCompletionSource<ContentDialog>();

            ContentDialogResult result;

            if (contentDialogPlacement == null)
                result = await dialog.ShowAsync();
            else
                result = await dialog.ShowAsync((ContentDialogPlacement)contentDialogPlacement);

            _contentDialogShowRequest = null;
            request.SetResult(dialog);

            return result;
        }

        public async Task<ContentDialogResult> ShowAsyncQueue(ContentDialogPlacement contentDialogPlacement)
        {
            return await ShowAsyncQueueBase(dialog, contentDialogPlacement);
        }
        public async Task<ContentDialogResult> ShowAsyncQueue()
        {
            return await ShowAsyncQueueBase(dialog, null);
        }

        public async Task<ContentDialogResult> ShowAsyncDraggable()
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

        public async Task<ContentDialogResult> ShowAsyncQueueDraggable()
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
}
