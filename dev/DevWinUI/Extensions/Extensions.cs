using Microsoft.UI.Xaml.Media.Imaging;

namespace DevWinUI;
public static partial class Extensions
{
    public static Color ChangeAlpha(this Color color, double alpha)
    {
        return ColorHelper.ChangeAlpha(color, alpha);
    }

    /// <summary>
    /// Finds the best contrasting color (black or white)
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color ContrastColorBlackWhite(this Color color)
    {
        return ColorHelper.ContrastColorBlackWhite(color);
    }

    /// <summary>
    /// Converts a hexadecimal color string into a SolidColorBrush object.
    /// </summary>
    /// <param name="hex">A string representing a color in hexadecimal format.</param>
    /// <returns>A SolidColorBrush that corresponds to the specified color.</returns>
    public static SolidColorBrush GetSolidColorBrush(this string hex)
    {
        return ColorHelper.GetSolidColorBrush(hex);
    }

    /// <summary>
    /// Formats a rich text block using a string that contains HTML formatting.
    /// </summary>
    /// <param name="textWithHTMLFormatting">The string containing HTML formatting to be applied to the rich text block.</param>
    /// <param name="richTextBlock">The rich text block that will be formatted with the provided HTML string.</param>
    public static void FormatRichTextBlock(this string textWithHTMLFormatting, RichTextBlock richTextBlock)
    {
        RichTextFormatterHelper.FormatRichTextBlock(textWithHTMLFormatting, richTextBlock);
    }

    /// <summary>
    /// Formats a text string with HTML for display in a TextBlock control.
    /// </summary>
    /// <param name="textWithHTMLFormatting">The string containing HTML formatting to be applied.</param>
    /// <param name="textBlock">The TextBlock control where the formatted text will be displayed.</param>
    public static void FormatTextBlock(this string textWithHTMLFormatting, TextBlock textBlock)
    {
        RichTextFormatterHelper.FormatTextBlock(textWithHTMLFormatting, textBlock);
    }
    public static BitmapImage ToBitmapImage(this string uri) => new BitmapImage(new Uri(uri));

    public static float ToFloat(this double value) => (float)value;

    public static int ToInt(this float value) => (int)value;
    public static Point RelativePosition(this UIElement element, UIElement other) =>
        element.TransformToVisual(other).TransformPoint(new Point(0, 0));

    public static int Create(this Random random, int min, int max,
        Func<int, bool> regenerateIfMet = null, int regenrationMaxCount = 5)
    {
        var value = random.Next(min, max);

        if (regenerateIfMet != null)
        {
            var i = 0;
            while (i < regenrationMaxCount && regenerateIfMet(value))
            {
                value = random.Next(min, max);
                i++;
            }

            return value;
        }

        return value;
    }
    public static bool IsFullyVisibile(this FrameworkElement element, FrameworkElement parent)
    {
        if (element == null || parent == null)
            return false;

        if (element.Visibility != Visibility.Visible)
            return false;

        var elementBounds = element.TransformToVisual(parent).TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight));
        var containerBounds = new Rect(0, 0, parent.ActualWidth, parent.ActualHeight);

        var originalElementWidth = elementBounds.Width;
        var originalElementHeight = elementBounds.Height;

        elementBounds.Intersect(containerBounds);

        var newElementWidth = elementBounds.Width;
        var newElementHeight = elementBounds.Height;

        return originalElementWidth.Equals(newElementWidth) && originalElementHeight.Equals(newElementHeight);
    }
    public static void ScrollToElement(this ScrollViewer scrollViewer, UIElement element,
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
    public static void ScrollToElement(this ScrollViewer scrollViewer, FrameworkElement element,
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
    public static async Task ScrollToElementAsync(this ScrollViewer scrollViewer, FrameworkElement element,
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
    public static Task ChangeViewAsync(this ScrollViewer scrollViewer, double? horizontalOffset, double? verticalOffset, float? zoomFactor, bool disableAniamtion)
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
}
