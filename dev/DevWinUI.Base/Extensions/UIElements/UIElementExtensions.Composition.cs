namespace DevWinUI;

public static partial class UIElementExtensions
{
    extension(UIElement element)
    {
        public void EnableFluidVisibilityAnimation(CompositionAnimationAxis? axis = null,
        float showFromOffset = 0.0f, float hideToOffset = 0.0f, Vector3? centerPoint = null,
        float showFromScale = 1.0f, float hideToScale = 1.0f, float showDuration = 800.0f, float hideDuration = 800.0f,
        int showDelay = 0, int hideDelay = 0, bool animateOpacity = true)
        {
            var elementVisual = element.Visual();
            var compositor = elementVisual.Compositor;
            ElementCompositionPreview.SetIsTranslationEnabled(element, true);

            ScalarKeyFrameAnimation hideOpacityAnimation = null;
            ScalarKeyFrameAnimation showOpacityAnimation = null;
            ScalarKeyFrameAnimation hideOffsetAnimation = null;
            ScalarKeyFrameAnimation showOffsetAnimation = null;
            Vector2KeyFrameAnimation hideScaleAnimation = null;
            Vector2KeyFrameAnimation showeScaleAnimation = null;

            if (animateOpacity)
            {
                hideOpacityAnimation = compositor.CreateScalarKeyFrameAnimation();
                hideOpacityAnimation.InsertKeyFrame(1.0f, 0.0f);
                hideOpacityAnimation.Duration = TimeSpan.FromMilliseconds(hideDuration);
                hideOpacityAnimation.DelayTime = TimeSpan.FromMilliseconds(hideDelay);
                hideOpacityAnimation.Target = "Opacity";
            }

            if (!hideToOffset.Equals(0.0f))
            {
                if (axis == null)
                {
                    throw new NullReferenceException("Please specify AnimationAxis!");
                }

                hideOffsetAnimation = compositor.CreateScalarKeyFrameAnimation();
                hideOffsetAnimation.InsertKeyFrame(1.0f, hideToOffset);
                hideOffsetAnimation.Duration = TimeSpan.FromMilliseconds(hideDuration);
                hideOffsetAnimation.DelayTime = TimeSpan.FromMilliseconds(hideDelay);
                hideOffsetAnimation.Target = $"Translation.{axis}";
            }

            if (centerPoint.HasValue)
            {
                elementVisual.CenterPoint = centerPoint.Value;
            }

            if (!hideToScale.Equals(1.0f))
            {
                hideScaleAnimation = compositor.CreateVector2KeyFrameAnimation();
                hideScaleAnimation.InsertKeyFrame(1.0f, new Vector2(hideToScale));
                hideScaleAnimation.Duration = TimeSpan.FromMilliseconds(hideDuration);
                hideScaleAnimation.DelayTime = TimeSpan.FromMilliseconds(hideDelay);
                hideScaleAnimation.Target = "Scale.XY";
            }

            var hideAnimationGroup = compositor.CreateAnimationGroup();
            if (hideOpacityAnimation != null)
            {
                hideAnimationGroup.Add(hideOpacityAnimation);
            }
            if (hideOffsetAnimation != null)
            {
                hideAnimationGroup.Add(hideOffsetAnimation);
            }
            if (hideScaleAnimation != null)
            {
                hideAnimationGroup.Add(hideScaleAnimation);
            }

            ElementCompositionPreview.SetImplicitHideAnimation(element, hideAnimationGroup);

            if (animateOpacity)
            {
                elementVisual.Opacity = 0.0f;

                showOpacityAnimation = compositor.CreateScalarKeyFrameAnimation();
                showOpacityAnimation.InsertKeyFrame(1.0f, 1.0f);
                showOpacityAnimation.Duration = TimeSpan.FromMilliseconds(showDuration);
                showOpacityAnimation.DelayTime = TimeSpan.FromMilliseconds(showDelay);
                showOpacityAnimation.Target = "Opacity";
            }

            if (!showFromOffset.Equals(0.0f))
            {
                if (axis == null)
                {
                    throw new NullReferenceException("Please specify AnimationAxis!");
                }

                showOffsetAnimation = compositor.CreateScalarKeyFrameAnimation();
                showOffsetAnimation.InsertKeyFrame(0.0f, showFromOffset);
                showOffsetAnimation.InsertKeyFrame(1.0f, 0.0f);
                showOffsetAnimation.Duration = TimeSpan.FromMilliseconds(showDuration);
                showOffsetAnimation.DelayTime = TimeSpan.FromMilliseconds(showDelay);
                showOffsetAnimation.Target = $"Translation.{axis}";
            }

            if (!showFromScale.Equals(1.0f))
            {
                showeScaleAnimation = compositor.CreateVector2KeyFrameAnimation();
                showeScaleAnimation.InsertKeyFrame(0.0f, new Vector2(showFromScale));
                showeScaleAnimation.InsertKeyFrame(1.0f, Vector2.One);
                showeScaleAnimation.Duration = TimeSpan.FromMilliseconds(showDuration);
                showeScaleAnimation.DelayTime = TimeSpan.FromMilliseconds(showDelay);
                showeScaleAnimation.Target = "Scale.XY";
            }

            var showAnimationGroup = compositor.CreateAnimationGroup();
            if (showOpacityAnimation != null)
            {
                showAnimationGroup.Add(showOpacityAnimation);
            }
            if (showOffsetAnimation != null)
            {
                showAnimationGroup.Add(showOffsetAnimation);
            }
            if (showeScaleAnimation != null)
            {
                showAnimationGroup.Add(showeScaleAnimation);
            }

            ElementCompositionPreview.SetImplicitShowAnimation(element, showAnimationGroup);
        }

        public void EnableImplicitAnimation(CompositionVisualPropertyType typeToAnimate,
            double duration = 800, double delay = 0, CompositionEasingFunction easing = null)
        {
            var visual = element.Visual();
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

            var visual = element.Visual();
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

            var visual = element.Visual();
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

            var visual = element.Visual();
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

            var visual = element.Visual();
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

            var visual = element.Visual();
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

            var visual = element.Visual();
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

        public void StartRotationAnimation(Vector3? rotationAxis = null, Vector3? centerPoint = null,
        float? from = null, float to = 0, double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
        AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;

            var visual = element.Visual();
            var compositor = visual.Compositor;

            if (centerPoint == null)
            {
                var size = element.RenderSize.ToVector2();
                centerPoint = new Vector3(size / 2, 0.0f);
            }
            visual.CenterPoint = centerPoint.Value;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            if (rotationAxis.HasValue)
            {
                visual.RotationAxis = rotationAxis.Value;
            }

            visual.StartAnimation("RotationAngleInDegrees",
                compositor.CreateScalarKeyFrameAnimation(from, to, duration, delay, easing, iterationBehavior));

            batch?.End();
        }
        public Task StartRotationAnimationAsync(Vector3? rotationAxis = null, Vector3? centerPoint = null,
        float? from = null, float to = 0, double duration = 800, int delay = 0, CompositionEasingFunction easing = null,
        AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch;

            var visual = element.Visual();
            var compositor = visual.Compositor;

            if (centerPoint == null)
            {
                var size = element.RenderSize.ToVector2();
                centerPoint = new Vector3(size / 2, 0.0f);
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


            if (rotationAxis.HasValue)
            {
                visual.RotationAxis = rotationAxis.Value;
            }

            visual.StartAnimation("RotationAngleInDegrees",
                compositor.CreateScalarKeyFrameAnimation(from, to, duration, delay, easing, iterationBehavior));

            batch.End();

            return taskSource.Task;
        }
        public void StartScaleAnimation(Vector2? from = null, Vector2? to = null,
                double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
                AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;

            var visual = element.Visual();
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

        public void StartScaleAnimation(Vector3? from = null, Vector3? to = null,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;

            var visual = element.Visual();
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

        public void StartScaleAnimation(CompositionAnimationAxis axis, float? from = null, float to = 0,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;

            var visual = element.Visual();
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
        public Task StartScaleAnimationAsync(Vector2? from = null, Vector2? to = null,
                double duration = 800, int delay = 0, CompositionEasingFunction easing = null,
                AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch;

            var visual = element.Visual();
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

        public Task StartScaleAnimationAsync(Vector3? from = null, Vector3? to = null,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch;

            var visual = element.Visual();
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

        public Task StartScaleAnimationAsync(CompositionAnimationAxis axis, float? from = null, float to = 0,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch;

            var visual = element.Visual();
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

            var visual = element.Visual();
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

            var visual = element.Visual();
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
        public void StartTranslationAnimation(CompositionAnimationAxis axis, float? from = null, float to = 0,
                double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
                AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;

            ElementCompositionPreview.SetIsTranslationEnabled(element, true);
            var visual = element.Visual();
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            visual.StartAnimation($"Translation.{axis}", compositor.CreateScalarKeyFrameAnimation(from, to, duration, delay, easing, iterationBehavior));

            batch?.End();
        }

        public void StartTranslationAnimation(Vector2? from = null, Vector2? to = null,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;

            ElementCompositionPreview.SetIsTranslationEnabled(element, true);
            var visual = element.Visual();
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            if (to == null)
            {
                to = Vector2.Zero;
            }

            visual.StartAnimation("Translation.XY", compositor.CreateVector2KeyFrameAnimation(from, to.Value, duration, delay, easing, iterationBehavior));

            batch?.End();
        }

        public void StartTranslationAnimation(Vector3? from = null, Vector3? to = null,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;

            ElementCompositionPreview.SetIsTranslationEnabled(element, true);
            var visual = element.Visual();
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

            visual.StartAnimation("Translation", compositor.CreateVector3KeyFrameAnimation(from, to.Value, duration, delay, easing, iterationBehavior));

            batch?.End();
        }

        public Task StartTranslationAnimationAsync(CompositionAnimationAxis axis, float? from = null, float to = 0,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch;

            ElementCompositionPreview.SetIsTranslationEnabled(element, true);
            var visual = element.Visual();
            var compositor = visual.Compositor;

            var taskSource = new TaskCompletionSource<bool>();

            void Completed(object o, CompositionBatchCompletedEventArgs e)
            {
                batch.Completed -= Completed;
                taskSource.SetResult(true);
            }

            batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            batch.Completed += Completed;

            visual.StartAnimation($"Translation.{axis}", compositor.CreateScalarKeyFrameAnimation(from, to, duration, delay, easing, iterationBehavior));

            batch.End();

            return taskSource.Task;
        }

        public Task StartTranslationAnimationAsync(Vector2? from = null, Vector2? to = null,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch;

            ElementCompositionPreview.SetIsTranslationEnabled(element, true);
            var visual = element.Visual();
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
                to = Vector2.Zero;
            }

            visual.StartAnimation("Translation.XY", compositor.CreateVector2KeyFrameAnimation(from, to.Value, duration, delay, easing, iterationBehavior));

            batch.End();

            return taskSource.Task;
        }

        public Task StartTranslationAnimationAsync(Vector3? from = null, Vector3? to = null,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch;

            ElementCompositionPreview.SetIsTranslationEnabled(element, true);
            var visual = element.Visual();
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

            visual.StartAnimation("Translation", compositor.CreateVector3KeyFrameAnimation(from, to.Value, duration, delay, easing, iterationBehavior));

            batch.End();

            return taskSource.Task;
        }

        public Visual Visual() => ElementCompositionPreview.GetElementVisual(element);

        public CompositionPropertySet GetPointerPositionProperties() => ElementCompositionPreview.GetPointerPositionPropertySet(element);

        public void SetChildVisual(Visual childVisual) => ElementCompositionPreview.SetElementChildVisual(element, childVisual);

        public ContainerVisual ContainerVisual()
        {
            var hostVisual = ElementCompositionPreview.GetElementVisual(element);
            ContainerVisual root = hostVisual.Compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(element, root);
            return root;
        }
    }
}
