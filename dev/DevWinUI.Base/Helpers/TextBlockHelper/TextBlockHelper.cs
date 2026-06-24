namespace DevWinUI;

public static partial class TextBlockHelper
{
    private static readonly CubicEase cubicEase = new CubicEase { EasingMode = EasingMode.EaseOut };
    public static async Task AnimateTextChangeAsync(TextBlock textBlock, string newText, TextBlockSlideDirection direction = TextBlockSlideDirection.BottomToTop, double offset = 20, int duration = 250)
    {
        if (textBlock.RenderTransform is not TranslateTransform transform)
        {
            transform = new TranslateTransform();
            textBlock.RenderTransform = transform;
        }

        GetOffsets(direction, offset, out double hideX, out double hideY, out double showX, out double showY);

        await RunAnimationAsync(textBlock, fromOpacity: 1, toOpacity: 0, fromX: 0, toX: hideX, fromY: 0, toY: hideY, duration);

        textBlock.Text = newText;

        transform.X = showX;
        transform.Y = showY;
        textBlock.Opacity = 0;

        await RunAnimationAsync(textBlock, fromOpacity: 0, toOpacity: 1, fromX: showX, toX: 0, fromY: showY, toY: 0, duration);
    }

    private static void GetOffsets(TextBlockSlideDirection direction, double offset, out double hideX, out double hideY, out double showX, out double showY)
    {
        hideX = hideY = showX = showY = 0;

        switch (direction)
        {
            case TextBlockSlideDirection.BottomToTop:
                hideY = -offset;
                showY = offset;
                break;

            case TextBlockSlideDirection.TopToBottom:
                hideY = offset;
                showY = -offset;
                break;

            case TextBlockSlideDirection.LeftToRight:
                hideX = offset;
                showX = -offset;
                break;

            case TextBlockSlideDirection.RightToLeft:
                hideX = -offset;
                showX = offset;
                break;
        }
    }

    private static Task RunAnimationAsync(TextBlock textBlock, double fromOpacity, double toOpacity, double fromX, double toX, double fromY, double toY, int duration)
    {
        var tcs = new TaskCompletionSource();

        var storyboard = new Storyboard();

        var opacityAnim = new DoubleAnimation
        {
            From = fromOpacity,
            To = toOpacity,
            Duration = TimeSpan.FromMilliseconds(duration),
            EasingFunction = cubicEase
        };

        Storyboard.SetTarget(opacityAnim, textBlock);
        Storyboard.SetTargetProperty(opacityAnim, "Opacity");

        var xAnim = new DoubleAnimation
        {
            From = fromX,
            To = toX,
            Duration = TimeSpan.FromMilliseconds(duration),
            EasingFunction = cubicEase
        };

        Storyboard.SetTarget(xAnim, textBlock);
        Storyboard.SetTargetProperty(xAnim, "(UIElement.RenderTransform).(TranslateTransform.X)");

        var yAnim = new DoubleAnimation
        {
            From = fromY,
            To = toY,
            Duration = TimeSpan.FromMilliseconds(duration),
            EasingFunction = cubicEase
        };

        Storyboard.SetTarget(yAnim, textBlock);
        Storyboard.SetTargetProperty(yAnim, "(UIElement.RenderTransform).(TranslateTransform.Y)");

        storyboard.Children.Add(opacityAnim);
        storyboard.Children.Add(xAnim);
        storyboard.Children.Add(yAnim);

        EventHandler<object> completedHandler = null;
        completedHandler = (_, _) =>
        {
            storyboard.Completed -= completedHandler;
            tcs.SetResult();
        };
        storyboard.Completed += completedHandler;

        storyboard.Begin();

        return tcs.Task;
    }
}
