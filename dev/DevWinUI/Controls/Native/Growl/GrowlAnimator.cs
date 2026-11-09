using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace DevWinUI;

internal static partial class GrowlAnimator
{
    private const float Distance = 24f;
    private const int SnapLayerCount = 32;
    private const int SnapDurationFactor = 5;
    private const int SnapInDurationFactor = 3;

    public static void Show(FrameworkElement element, GrowlShowAnimation kind, GrowlPosition position, TimeSpan duration)
    {
        if (kind == GrowlShowAnimation.None)
        {
            return;
        }

        if (kind == GrowlShowAnimation.Snap)
        {
            SnapIn(element, position, duration);
            return;
        }

        var visual = ElementCompositionPreview.GetElementVisual(element);
        var compositor = visual.Compositor;
        var easing = compositor.CreateCubicBezierEasingFunction(new Vector2(0.16f, 1f), new Vector2(0.3f, 1f));

        visual.CenterPoint = new Vector3((float)element.ActualWidth / 2, (float)element.ActualHeight / 2, 0);

        var fade = compositor.CreateScalarKeyFrameAnimation();
        fade.Duration = kind == GrowlShowAnimation.Fade ? FadeDuration(duration) : duration;
        fade.InsertKeyFrame(0f, 0f);
        fade.InsertKeyFrame(1f, (float)element.Opacity, FadeEasing(compositor));
        visual.StartAnimation("Opacity", fade);

        switch (kind)
        {
            case GrowlShowAnimation.Slide:
                Translate(element, visual, compositor, EdgeOffset(element, position, false), Vector3.Zero, duration, easing);
                break;
            case GrowlShowAnimation.Push:
                Translate(element, visual, compositor, EdgeOffset(element, position, true), Vector3.Zero, duration, easing);
                break;
            case GrowlShowAnimation.Scale:
                var scale = compositor.CreateVector3KeyFrameAnimation();
                scale.Duration = duration;
                scale.InsertKeyFrame(0f, new Vector3(0.85f, 0.85f, 1f));
                scale.InsertKeyFrame(1f, Vector3.One, easing);
                visual.StartAnimation("Scale", scale);
                break;
        }
    }

    public static void Close(FrameworkElement element, GrowlCloseAnimation kind, GrowlPosition position, TimeSpan duration, Action completed)
    {
        if (kind == GrowlCloseAnimation.None)
        {
            completed();
            return;
        }

        if (kind == GrowlCloseAnimation.Snap)
        {
            Snap(element, position, duration, completed);
            return;
        }

        var visual = ElementCompositionPreview.GetElementVisual(element);
        var compositor = visual.Compositor;
        var easing = compositor.CreateCubicBezierEasingFunction(new Vector2(0.4f, 0f), new Vector2(1f, 1f));
        var batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

        visual.CenterPoint = new Vector3((float)element.ActualWidth / 2, (float)element.ActualHeight / 2, 0);

        var fade = compositor.CreateScalarKeyFrameAnimation();
        fade.Duration = kind == GrowlCloseAnimation.Fade ? FadeDuration(duration) : duration;
        fade.InsertKeyFrame(1f, 0f, FadeEasing(compositor));
        visual.StartAnimation("Opacity", fade);

        switch (kind)
        {
            case GrowlCloseAnimation.Slide:
                Translate(element, visual, compositor, Vector3.Zero, EdgeOffset(element, position, false), duration, easing);
                break;
            case GrowlCloseAnimation.Push:
                Translate(element, visual, compositor, Vector3.Zero, EdgeOffset(element, position, true), duration, easing);
                break;
            case GrowlCloseAnimation.Scale:
                var scale = compositor.CreateVector3KeyFrameAnimation();
                scale.Duration = duration;
                scale.InsertKeyFrame(0f, Vector3.One);
                scale.InsertKeyFrame(1f, new Vector3(0.85f, 0.85f, 1f), easing);
                visual.StartAnimation("Scale", scale);
                break;
        }

        batch.Completed += (_, _) => element.DispatcherQueue.TryEnqueue(() => completed());
        batch.End();
    }

    public static void Shift(FrameworkElement element, Vector2 delta, TimeSpan duration)
    {
        var visual = ElementCompositionPreview.GetElementVisual(element);
        var compositor = visual.Compositor;
        var easing = compositor.CreateCubicBezierEasingFunction(new Vector2(0.16f, 1f), new Vector2(0.3f, 1f));

        Translate(element, visual, compositor, new Vector3(delta, 0), Vector3.Zero, duration, easing);
    }

    private sealed record SnapCapture(byte[][] Layers, int Width, int Height, FrameworkElement Source);

    private static async void Snap(FrameworkElement element, GrowlPosition position, TimeSpan duration, Action completed)
    {
        SnapLayers? layers = null;

        try
        {
            layers = await PrepareSnap(element, false);
        }
        catch
        {
        }

        if (layers is null)
        {
            Close(element, GrowlCloseAnimation.Fade, position, duration, completed);
            return;
        }

        var popup = layers.Popup;
        var images = layers.Images;
        var source = layers.Source;

        var cardVisual = ElementCompositionPreview.GetElementVisual(element);
        var compositor = cardVisual.Compositor;
        var random = new Random();
        var total = TimeSpan.FromTicks(duration.Ticks * SnapDurationFactor);
        var layerDuration = TimeSpan.FromTicks((long)(total.Ticks * 0.6));
        var maxDelay = total - layerDuration;
        var easing = compositor.CreateCubicBezierEasingFunction(new Vector2(0.25f, 0.1f), new Vector2(0.25f, 1f));
        var linear = compositor.CreateLinearEasingFunction();
        var center = new Vector3((float)source.ActualWidth / 2, (float)source.ActualHeight / 2, 0);
        var batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

        for (int i = 0; i < images.Count; i++)
        {
            var image = images[i];
            var visual = ElementCompositionPreview.GetElementVisual(image);
            var delay = TimeSpan.FromTicks(maxDelay.Ticks * i / Math.Max(1, images.Count - 1));

            ElementCompositionPreview.SetIsTranslationEnabled(image, true);
            visual.Properties.InsertVector3("Translation", Vector3.Zero);
            visual.CenterPoint = center;

            var translation = compositor.CreateVector3KeyFrameAnimation();
            ConfigureSnap(translation, layerDuration, delay);
            translation.InsertKeyFrame(0f, Vector3.Zero);
            translation.InsertKeyFrame(1f, new Vector3(30f + (float)random.NextDouble() * 80f, -60f + (float)random.NextDouble() * 80f, 0), easing);
            visual.StartAnimation("Translation", translation);

            var rotation = compositor.CreateScalarKeyFrameAnimation();
            ConfigureSnap(rotation, layerDuration, delay);
            rotation.InsertKeyFrame(0f, 0f);
            rotation.InsertKeyFrame(1f, (float)(random.NextDouble() - 0.5) * 30f, easing);
            visual.StartAnimation("RotationAngleInDegrees", rotation);

            var fade = compositor.CreateScalarKeyFrameAnimation();
            ConfigureSnap(fade, layerDuration, delay);
            fade.InsertKeyFrame(0f, 1f);
            fade.InsertKeyFrame(1f, 0f, linear);
            visual.StartAnimation("Opacity", fade);
        }

        batch.Completed += (_, _) => popup.DispatcherQueue.TryEnqueue(() =>
        {
            popup.IsOpen = false;
            popup.Child = null;
        });
        batch.End();

        cardVisual.Opacity = 0;
        completed();
    }

    private sealed record SnapLayers(Popup Popup, List<Image> Images, FrameworkElement Source);

    private static async Task<SnapLayers?> PrepareSnap(FrameworkElement element, bool hidden)
    {
        var captured = await Capture(element);

        if (captured is null || element.XamlRoot is null)
        {
            return null;
        }

        var source = captured.Source;
        var origin = source.TransformToVisual(element.XamlRoot.Content).TransformPoint(new Point(0, 0));
        var canvas = new Canvas { Width = source.ActualWidth, Height = source.ActualHeight, IsHitTestVisible = false };
        var popup = new Popup
        {
            XamlRoot = element.XamlRoot,
            Child = canvas,
            HorizontalOffset = origin.X,
            VerticalOffset = origin.Y,
            IsHitTestVisible = false
        };

        var images = new List<Image>(captured.Layers.Length);

        foreach (var layer in captured.Layers)
        {
            var bitmap = new WriteableBitmap(captured.Width, captured.Height);
            layer.CopyTo(bitmap.PixelBuffer);
            bitmap.Invalidate();

            var image = new Image
            {
                Source = bitmap,
                Width = source.ActualWidth,
                Height = source.ActualHeight,
                Stretch = Stretch.Fill,
                IsHitTestVisible = false
            };

            if (hidden)
            {
                ElementCompositionPreview.GetElementVisual(image).Opacity = 0;
            }

            canvas.Children.Add(image);
            images.Add(image);
        }

        popup.IsOpen = true;

        await Task.WhenAny(WaitLoaded(canvas), Task.Delay(500));

        return new SnapLayers(popup, images, source);
    }

    private static async void SnapIn(FrameworkElement element, GrowlPosition position, TimeSpan duration)
    {
        if (element.Opacity <= 0)
        {
            return;
        }

        var visual = ElementCompositionPreview.GetElementVisual(element);
        float opacity = (float)element.Opacity;

        visual.Opacity = 0;

        SnapLayers? layers = null;

        try
        {
            layers = await PrepareSnap(element, true);
        }
        catch
        {
        }

        if (layers is null)
        {
            Show(element, GrowlShowAnimation.Fade, position, duration);
            return;
        }

        var popup = layers.Popup;
        var images = layers.Images;
        var compositor = visual.Compositor;
        var random = new Random();
        var total = TimeSpan.FromTicks(duration.Ticks * SnapInDurationFactor);
        var layerDuration = TimeSpan.FromTicks((long)(total.Ticks * 0.6));
        var maxDelay = total - layerDuration;
        var easing = compositor.CreateCubicBezierEasingFunction(new Vector2(0.22f, 1f), new Vector2(0.36f, 1f));
        var linear = compositor.CreateLinearEasingFunction();
        var center = new Vector3((float)layers.Source.ActualWidth / 2, (float)layers.Source.ActualHeight / 2, 0);
        var batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

        for (int i = 0; i < images.Count; i++)
        {
            var image = images[i];
            var imageVisual = ElementCompositionPreview.GetElementVisual(image);
            var delay = TimeSpan.FromTicks(maxDelay.Ticks * (images.Count - 1 - i) / Math.Max(1, images.Count - 1));
            var origin = new Vector3(30f + (float)random.NextDouble() * 80f, -60f + (float)random.NextDouble() * 80f, 0);

            ElementCompositionPreview.SetIsTranslationEnabled(image, true);
            imageVisual.Properties.InsertVector3("Translation", origin);
            imageVisual.CenterPoint = center;

            var translation = compositor.CreateVector3KeyFrameAnimation();
            ConfigureSnap(translation, layerDuration, delay);
            translation.InsertKeyFrame(0f, origin);
            translation.InsertKeyFrame(1f, Vector3.Zero, easing);
            imageVisual.StartAnimation("Translation", translation);

            var rotation = compositor.CreateScalarKeyFrameAnimation();
            ConfigureSnap(rotation, layerDuration, delay);
            rotation.InsertKeyFrame(0f, (float)(random.NextDouble() - 0.5) * 30f);
            rotation.InsertKeyFrame(1f, 0f, easing);
            imageVisual.StartAnimation("RotationAngleInDegrees", rotation);

            var fade = compositor.CreateScalarKeyFrameAnimation();
            ConfigureSnap(fade, layerDuration, delay);
            fade.InsertKeyFrame(0f, 0f);
            fade.InsertKeyFrame(1f, 1f, linear);
            imageVisual.StartAnimation("Opacity", fade);
        }

        batch.Completed += (_, _) => popup.DispatcherQueue.TryEnqueue(() =>
        {
            if (element.IsLoaded)
            {
                visual.Opacity = opacity;
            }

            popup.IsOpen = false;
            popup.Child = null;
        });
        batch.End();
    }

    private static Task WaitLoaded(FrameworkElement element)
    {
        var source = new TaskCompletionSource();

        if (element.IsLoaded)
        {
            source.SetResult();
            return source.Task;
        }

        RoutedEventHandler? handler = null;
        handler = (_, _) =>
        {
            element.Loaded -= handler;
            source.TrySetResult();
        };
        element.Loaded += handler;

        return source.Task;
    }

    private static void ConfigureSnap(KeyFrameAnimation animation, TimeSpan duration, TimeSpan delay)
    {
        animation.Duration = duration;
        animation.DelayTime = delay;
        animation.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
    }

    private static IEnumerable<FrameworkElement> CaptureTargets(FrameworkElement element)
    {
        var current = element;

        for (int depth = 0; depth < 2; depth++)
        {
            if (VisualTreeHelper.GetChildrenCount(current) == 0 || VisualTreeHelper.GetChild(current, 0) is not FrameworkElement child)
            {
                break;
            }

            yield return child;
            current = child;
        }

        yield return element;
    }

    private static async Task<SnapCapture?> Capture(FrameworkElement element)
    {
        if (element.XamlRoot is null || element.ActualWidth <= 0 || element.ActualHeight <= 0)
        {
            return null;
        }

        await Task.Delay(16);

        double scale = element.XamlRoot.RasterizationScale;

        foreach (var target in CaptureTargets(element))
        {
            if (target.ActualWidth <= 0 || target.ActualHeight <= 0)
            {
                continue;
            }

            try
            {
                var bitmap = new RenderTargetBitmap();
                await bitmap.RenderAsync(target);

                var pixels = (await bitmap.GetPixelsAsync()).ToArray();
                int width = bitmap.PixelWidth > 0 ? bitmap.PixelWidth : (int)Math.Round(target.ActualWidth * scale);
                int height = width > 0 ? pixels.Length / 4 / width : 0;

                if (width <= 0 || height <= 0)
                {
                    continue;
                }

                var layers = await Task.Run(() => Split(pixels, width, height, SnapLayerCount));

                if (layers is not null)
                {
                    return new SnapCapture(layers, width, height, target);
                }
            }
            catch
            {
            }
        }

        return null;
    }

    private static byte[][]? Split(byte[] source, int width, int height, int count)
    {
        var layers = new byte[count][];
        var random = new Random();
        bool any = false;

        for (int i = 0; i < count; i++)
        {
            layers[i] = new byte[source.Length];
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int offset = (y * width + x) * 4;

                if (source[offset + 3] == 0)
                {
                    continue;
                }

                any = true;

                int index = Math.Min(count - 1, (int)(count * (random.NextDouble() + 2.0 * x / width) / 3));
                var layer = layers[index];

                layer[offset] = source[offset];
                layer[offset + 1] = source[offset + 1];
                layer[offset + 2] = source[offset + 2];
                layer[offset + 3] = source[offset + 3];
            }
        }

        return any ? layers : null;
    }

    private static TimeSpan FadeDuration(TimeSpan duration)
    {
        return TimeSpan.FromTicks(duration.Ticks * 2);
    }

    private static CompositionEasingFunction FadeEasing(Compositor compositor)
    {
        return compositor.CreateCubicBezierEasingFunction(new Vector2(0.4f, 0f), new Vector2(0.2f, 1f));
    }

    private static void Translate(FrameworkElement element, Visual visual, Compositor compositor, Vector3 from, Vector3 to, TimeSpan duration, CompositionEasingFunction easing)
    {
        ElementCompositionPreview.SetIsTranslationEnabled(element, true);
        visual.Properties.InsertVector3("Translation", from);

        var animation = compositor.CreateVector3KeyFrameAnimation();
        animation.Duration = duration;
        animation.InsertKeyFrame(0f, from);
        animation.InsertKeyFrame(1f, to, easing);
        visual.StartAnimation("Translation", animation);
    }

    private static Vector3 EdgeOffset(FrameworkElement element, GrowlPosition position, bool forceVertical)
    {
        float width = (float)element.ActualWidth + Distance;
        float height = (float)element.ActualHeight + Distance;
        bool top = position is GrowlPosition.TopLeft or GrowlPosition.TopCenter or GrowlPosition.TopRight;
        float vertical = top ? -height : height;

        if (forceVertical || position is GrowlPosition.TopCenter or GrowlPosition.BottomCenter)
        {
            return new Vector3(0, vertical, 0);
        }

        bool left = position is GrowlPosition.TopLeft or GrowlPosition.BottomLeft;

        return new Vector3(left ? -width : width, 0, 0);
    }
}
