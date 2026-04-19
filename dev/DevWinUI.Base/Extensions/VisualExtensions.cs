namespace DevWinUI;

public static partial class VisualExtensions
{
    extension(Visual visual)
    {
        public Task StartClipAnimationAsync(CompositionClipAnimationDirection direction, float to,
        double duration = 800, int delay = 0, CompositionEasingFunction easing = null,
        AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch;

            if (visual.Size.X.Equals(0) || visual.Size.Y.Equals(0))
            {
                throw new ArgumentException("The visual is not properly sized.");
            }

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

        public void StartClipAnimation(CompositionClipAnimationDirection direction, float to,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;

            if (visual.Size.X.Equals(0) || visual.Size.Y.Equals(0))
            {
                throw new ArgumentException("The visual is not properly sized.");
            }

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

        public void EnableImplicitAnimation(CompositionVisualPropertyType typeToAnimate,
        double duration = 800, double delay = 0, CompositionEasingFunction easing = null)
        {
            var compositor = visual.Compositor;

            var animationCollection = compositor.CreateImplicitAnimationCollection();

            foreach (var type in Extensions.GetValues<CompositionVisualPropertyType>())
            {
                if (!typeToAnimate.HasFlag(type)) continue;

                var animation = compositor.CreateAnimationByType(type, duration, delay, easing);

                if (animation != null)
                {
                    animationCollection[type.ToString()] = animation;
                }
            }

            visual.ImplicitAnimations = animationCollection;
        }

        public void StartOffsetAnimation(CompositionAnimationAxis axis, float? from = null, float to = 0,
        double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
        AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            visual.StartAnimation($"Offset.{axis}",
                compositor.CreateScalarKeyFrameAnimation(from, to, duration, delay, easing, iterationBehavior));

            batch?.End();
        }

        public void StartOffsetAnimation(Vector3? from = null, Vector3? to = null,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            if (to == null)
            {
                to = Vector3.Zero;
            }

            visual.StartAnimation("Offset",
                compositor.CreateVector3KeyFrameAnimation(from, to.Value, duration, delay, easing, iterationBehavior));

            batch?.End();
        }

        public Task StartOffsetAnimationAsync(CompositionAnimationAxis axis, float? from = null, float to = 0,
        double duration = 800, int delay = 0, CompositionEasingFunction easing = null,
        AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch;
            var compositor = visual.Compositor;

            var taskSource = new TaskCompletionSource<bool>();

            void Completed(object o, CompositionBatchCompletedEventArgs e)
            {
                batch.Completed -= Completed;
                taskSource.SetResult(true);
            }

            batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            batch.Completed += Completed;

            visual.StartAnimation($"Offset.{axis}",
                compositor.CreateScalarKeyFrameAnimation(from, to, duration, delay, easing, iterationBehavior));

            batch.End();

            return taskSource.Task;
        }

        public Task StartOffsetAnimationAsync(Vector3? from = null, Vector3? to = null,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch;
            var compositor = visual.Compositor;

            var taskSource = new TaskCompletionSource<bool>();

            void Completed(object o, CompositionBatchCompletedEventArgs e)
            {
                batch.Completed -= Completed;
                taskSource.SetResult(true);
            }

            batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            batch.Completed += Completed;

            if (to == null)
            {
                to = Vector3.Zero;
            }

            visual.StartAnimation("Offset",
                compositor.CreateVector3KeyFrameAnimation(from, to.Value, duration, delay, easing, iterationBehavior));

            batch.End();

            return taskSource.Task;
        }

        public void StartOpacityAnimation(float? from = null, float to = 1.0f,
       double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
       AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            visual.StartAnimation("Opacity",
                compositor.CreateScalarKeyFrameAnimation(from, to, duration, delay, easing, iterationBehavior));

            batch?.End();
        }


        public Task StartOpacityAnimationAsync(float? from = null, float to = 1.0f,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch;
            var compositor = visual.Compositor;

            var taskSource = new TaskCompletionSource<bool>();

            void Completed(object o, CompositionBatchCompletedEventArgs e)
            {
                batch.Completed -= Completed;
                taskSource.SetResult(true);
            }

            batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            batch.Completed += Completed;


            visual.StartAnimation("Opacity",
                compositor.CreateScalarKeyFrameAnimation(from, to, duration, delay, easing, iterationBehavior));

            batch.End();

            return taskSource.Task;
        }

        public void StartRotationAnimation(Vector3 rotationAxis, Vector3? centerPoint = null,
        float? from = null, float to = 0, double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
        AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;
            var compositor = visual.Compositor;

            if (centerPoint == null)
            {
                centerPoint = new Vector3(visual.Size / 2, 0.0f);
            }
            visual.CenterPoint = centerPoint.Value;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            visual.RotationAxis = rotationAxis;

            visual.StartAnimation("RotationAngleInDegrees",
                compositor.CreateScalarKeyFrameAnimation(from, to, duration, delay, easing, iterationBehavior));

            batch?.End();
        }
        public Task StartRotationAnimationAsync(Vector3 rotationAxis, Vector3? centerPoint = null,
                float? from = null, float to = 0, double duration = 800, int delay = 0, CompositionEasingFunction easing = null,
                AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch;
            var compositor = visual.Compositor;

            if (centerPoint == null)
            {
                centerPoint = new Vector3(visual.Size / 2, 0.0f);
            }
            visual.CenterPoint = centerPoint.Value;

            var taskSource = new TaskCompletionSource<bool>();

            void Completed(object o, CompositionBatchCompletedEventArgs e)
            {
                batch.Completed -= Completed;
                taskSource.SetResult(true);
            }

            batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            batch.Completed += Completed;


            visual.RotationAxis = rotationAxis;

            visual.StartAnimation("RotationAngleInDegrees",
                compositor.CreateScalarKeyFrameAnimation(from, to, duration, delay, easing, iterationBehavior));

            batch.End();

            return taskSource.Task;
        }
        public void StartScaleAnimation(Vector3? from = null, Vector3? to = null,
        double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
        AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            if (to == null)
            {
                to = Vector3.One;
            }

            visual.StartAnimation("Scale",
                compositor.CreateVector3KeyFrameAnimation(from, to.Value, duration, delay, easing, iterationBehavior));

            batch?.End();
        }

        public void StartScaleAnimation(Vector2? from = null, Vector2? to = null,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            if (to == null)
            {
                to = Vector2.One;
            }

            visual.StartAnimation("Scale",
                compositor.CreateVector3KeyFrameAnimation(from, to.Value, duration, delay, easing, iterationBehavior));

            batch?.End();
        }

        public void StartScaleAnimation(CompositionAnimationAxis axis, float? from = null, float to = 0,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            visual.StartAnimation($"Scale.{axis}",
                compositor.CreateScalarKeyFrameAnimation(from, to, duration, delay, easing, iterationBehavior));

            batch?.End();
        }
        public Task StartScaleAnimationAsync(Vector3? from = null, Vector3? to = null,
                double duration = 800, int delay = 0, CompositionEasingFunction easing = null,
                AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch;
            var compositor = visual.Compositor;

            var taskSource = new TaskCompletionSource<bool>();

            void Completed(object o, CompositionBatchCompletedEventArgs e)
            {
                batch.Completed -= Completed;
                taskSource.SetResult(true);
            }

            batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            batch.Completed += Completed;

            if (to == null)
            {
                to = Vector3.One;
            }

            visual.StartAnimation("Scale",
                compositor.CreateVector3KeyFrameAnimation(from, to.Value, duration, delay, easing, iterationBehavior));

            batch.End();

            return taskSource.Task;
        }

        public Task StartScaleAnimationAsync(Vector2? from = null, Vector2? to = null,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch;
            var compositor = visual.Compositor;

            var taskSource = new TaskCompletionSource<bool>();

            void Completed(object o, CompositionBatchCompletedEventArgs e)
            {
                batch.Completed -= Completed;
                taskSource.SetResult(true);
            }

            batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            batch.Completed += Completed;

            if (to == null)
            {
                to = Vector2.One;
            }

            visual.StartAnimation("Scale",
                compositor.CreateVector3KeyFrameAnimation(from, to.Value, duration, delay, easing, iterationBehavior));

            batch.End();

            return taskSource.Task;
        }

        public Task StartScaleAnimationAsync(CompositionAnimationAxis axis, float? from = null, float to = 0,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch;
            var compositor = visual.Compositor;

            var taskSource = new TaskCompletionSource<bool>();

            void Completed(object o, CompositionBatchCompletedEventArgs e)
            {
                batch.Completed -= Completed;
                taskSource.SetResult(true);
            }

            batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            batch.Completed += Completed;


            visual.StartAnimation($"Scale.{axis}",
                compositor.CreateScalarKeyFrameAnimation(from, to, duration, delay, easing, iterationBehavior));

            batch.End();

            return taskSource.Task;
        }
        public void StartSizeAnimation(Vector2? from = null, Vector2? to = null,
        double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
        AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            if (to == null)
            {
                to = Vector2.One;
            }

            visual.StartAnimation("Size",
                compositor.CreateVector2KeyFrameAnimation(from, to.Value, duration, delay, easing, iterationBehavior));

            batch?.End();
        }
        public Task StartSizeAnimationAsync(Vector2? from = null, Vector2? to = null,
                double duration = 800, int delay = 0, CompositionEasingFunction easing = null,
                AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch;

            var compositor = visual.Compositor;

            var taskSource = new TaskCompletionSource<bool>();

            void Completed(object o, CompositionBatchCompletedEventArgs e)
            {
                batch.Completed -= Completed;
                taskSource.SetResult(true);
            }

            batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            batch.Completed += Completed;

            if (to == null)
            {
                to = Vector2.One;
            }

            visual.StartAnimation("Size",
                compositor.CreateVector2KeyFrameAnimation(from, to.Value, duration, delay, easing, iterationBehavior));

            batch.End();

            return taskSource.Task;
        }
    }
}
