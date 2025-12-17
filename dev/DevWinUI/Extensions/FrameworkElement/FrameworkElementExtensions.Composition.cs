namespace DevWinUI;

public static partial class FrameworkElementExtensions
{
    extension(FrameworkElement element)
    {
        public void StartClipAnimation(CompositionClipAnimationDirection direction, float to,
        double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
        AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;

            var visual = element.Visual();
            // After we get the Visual of the View, we need to SIZE it 'cause by design the
            // Size is (0,0). Without doing this, clipping will not work.
            visual.Size = element.RenderSize.ToVector2();
            var compositor = visual.Compositor;

            if (visual.Clip == null)
            {
                var clip = compositor.CreateInsetClip();
                visual.Clip = clip;
            }

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            visual.Clip.StartAnimation($"{direction}Inset",
                compositor.CreateScalarKeyFrameAnimation(null, to, duration, delay, easing, iterationBehavior));

            batch?.End();
        }


        public Task StartClipAnimationAsync(CompositionClipAnimationDirection direction, float to,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch;

            var visual = element.Visual();
            // After we get the Visual of the View, we need to SIZE it 'cause by design the
            // Size is (0,0). Without doing this, clipping will not work.
            visual.Size = element.RenderSize.ToVector2();
            var compositor = visual.Compositor;

            if (visual.Clip == null)
            {
                var clip = compositor.CreateInsetClip();
                visual.Clip = clip;
            }

            var taskSource = new TaskCompletionSource<bool>();

            void Completed(object o, CompositionBatchCompletedEventArgs e)
            {
                batch.Completed -= Completed;
                taskSource.SetResult(true);
            }

            batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            batch.Completed += Completed;

            visual.Clip.StartAnimation($"{direction}Inset",
                compositor.CreateScalarKeyFrameAnimation(null, to, duration, delay, easing, iterationBehavior));

            batch.End();

            return taskSource.Task;
        }

        public void SetDropShadow(DropShadow shadow, FrameworkElement sizingElement = null)
        {
            var compositor = shadow.Compositor;

            var shadowVisual = compositor.CreateSpriteVisual();
            shadowVisual.Shadow = shadow;

            if (sizingElement == null)
            {
                sizingElement = element;
            }

            sizingElement.SizeChanged += (s, e) =>
            {
                if (e.PreviousSize.Equals(e.NewSize)) return;
                shadowVisual.Size = sizingElement.RenderSize.ToVector2();
            };
            shadowVisual.Size = sizingElement.RenderSize.ToVector2();

            element.SetChildVisual(shadowVisual);
        }
    }
}
