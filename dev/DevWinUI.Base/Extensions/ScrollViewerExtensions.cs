namespace DevWinUI;

public static partial class ScrollViewerExtensions
{
    extension(ScrollViewer scrollViewer)
    {
        public void ScrollToElement(UIElement element,
        bool isVerticalScrolling = true, bool smoothScrolling = true, float? zoomFactor = null)
        {
            var transform = element.TransformToVisual((UIElement)scrollViewer.Content);
            var position = transform.TransformPoint(new Point(0, 0));

            if (isVerticalScrolling)
            {
                scrollViewer.ChangeView(null, position.Y, zoomFactor, !smoothScrolling);
            }
            else
            {
                scrollViewer.ChangeView(position.X, null, zoomFactor, !smoothScrolling);
            }
        }
        public void ScrollToElement(FrameworkElement element,
            bool isVerticalScrolling = true, bool smoothScrolling = true, float? zoomFactor = null, bool bringToTopOrLeft = true)
        {
            if (!bringToTopOrLeft && element.IsFullyVisibile(scrollViewer))
                return;

            var contentArea = (FrameworkElement)scrollViewer.Content;
            var position = element.RelativePosition(contentArea);

            if (isVerticalScrolling)
            {
                scrollViewer.ChangeView(null, position.Y, zoomFactor, !smoothScrolling);
            }
            else
            {
                scrollViewer.ChangeView(position.X, null, zoomFactor, !smoothScrolling);
            }
        }
        public async Task ScrollToElementAsync(FrameworkElement element,
        bool isVerticalScrolling = true, bool smoothScrolling = true, float? zoomFactor = null, bool bringToTopOrLeft = true)
        {
            if (!bringToTopOrLeft && element.IsFullyVisibile(scrollViewer))
                return;

            var contentArea = (FrameworkElement)scrollViewer.Content;
            var position = element.RelativePosition(contentArea);

            if (isVerticalScrolling)
            {
                await scrollViewer.ChangeViewAsync(null, position.Y, zoomFactor, !smoothScrolling);
            }
            else
            {
                await scrollViewer.ChangeViewAsync(position.X, null, zoomFactor, !smoothScrolling);
            }
        }
        public Task ChangeViewAsync(double? horizontalOffset, double? verticalOffset, float? zoomFactor, bool disableAniamtion)
        {
            var taskSource = new TaskCompletionSource<bool>();

            void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
            {
                if (e.IsIntermediate) return;

                scrollViewer.ViewChanged -= OnViewChanged;
                taskSource.SetResult(true);
            }

            scrollViewer.ViewChanged += OnViewChanged;
            scrollViewer.ChangeView(horizontalOffset, verticalOffset, zoomFactor, disableAniamtion);

            return taskSource.Task;
        }

        public CompositionPropertySet ScrollProperties() => ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);
    }
}
