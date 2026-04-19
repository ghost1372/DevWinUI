namespace DevWinUI;

public static partial class CompositionExtensions
{
    extension (Compositor compositor)
    {
        public ScalarKeyFrameAnimation CreateScalarKeyFrameAnimation(float? from, float to,
        double duration, double delay, CompositionEasingFunction easing, AnimationIterationBehavior iterationBehavior)
        {
            var animation = compositor.CreateScalarKeyFrameAnimation();

            animation.Duration = TimeSpan.FromMilliseconds(duration);
            if (!delay.Equals(0)) animation.DelayTime = TimeSpan.FromMilliseconds(delay);
            if (from.HasValue) animation.InsertKeyFrame(0.0f, from.Value, easing);
            animation.InsertKeyFrame(1.0f, to, easing);
            animation.IterationBehavior = iterationBehavior;

            return animation;
        }

        public Vector2KeyFrameAnimation CreateVector2KeyFrameAnimation(Vector2? from, Vector2 to,
            double duration, double delay, CompositionEasingFunction easing, AnimationIterationBehavior iterationBehavior)
        {
            var animation = compositor.CreateVector2KeyFrameAnimation();

            animation.Duration = TimeSpan.FromMilliseconds(duration);
            animation.DelayTime = TimeSpan.FromMilliseconds(delay);
            if (from.HasValue) animation.InsertKeyFrame(0.0f, from.Value, easing);
            animation.InsertKeyFrame(1.0f, to, easing);
            animation.IterationBehavior = iterationBehavior;

            return animation;
        }

        public Vector3KeyFrameAnimation CreateVector3KeyFrameAnimation(Vector2? from, Vector2 to,
            double duration, double delay, CompositionEasingFunction easing, AnimationIterationBehavior iterationBehavior)
        {
            var animation = compositor.CreateVector3KeyFrameAnimation();

            animation.Duration = TimeSpan.FromMilliseconds(duration);
            animation.DelayTime = TimeSpan.FromMilliseconds(delay);
            if (from.HasValue) animation.InsertKeyFrame(0.0f, new Vector3(from.Value, 1.0f), easing);
            animation.InsertKeyFrame(1.0f, new Vector3(to, 1.0f), easing);
            animation.IterationBehavior = iterationBehavior;

            return animation;
        }

        public Vector3KeyFrameAnimation CreateVector3KeyFrameAnimation(Vector3? from, Vector3 to,
            double duration, double delay, CompositionEasingFunction easing, AnimationIterationBehavior iterationBehavior)
        {
            var animation = compositor.CreateVector3KeyFrameAnimation();

            animation.Duration = TimeSpan.FromMilliseconds(duration);
            animation.DelayTime = TimeSpan.FromMilliseconds(delay);
            if (from.HasValue) animation.InsertKeyFrame(0.0f, from.Value, easing);
            animation.InsertKeyFrame(1.0f, to, easing);
            animation.IterationBehavior = iterationBehavior;

            return animation;
        }

        public CubicBezierEasingFunction CreateEaseInCubicEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.550f, 0.055f), new Vector2(0.675f, 0.190f));
        }

        public CubicBezierEasingFunction CreateEaseOutCubicEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.215f, 0.610f), new Vector2(0.355f, 1.000f));
        }

        public CubicBezierEasingFunction CreateEaseInOutCubicEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.645f, 0.045f), new Vector2(0.355f, 1.000f));
        }

        public CubicBezierEasingFunction CreateEaseInBackEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.600f, -0.280f), new Vector2(0.735f, 0.045f));
        }

        public CubicBezierEasingFunction CreateEaseOutBackEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.175f, 0.885f), new Vector2(0.320f, 1.275f));
        }

        public CubicBezierEasingFunction CreateEaseOutStrongBackEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.175f, 0.885f), new Vector2(0.52f, 3.275f));
        }

        public CubicBezierEasingFunction CreateEaseInOutBackEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.680f, -0.550f), new Vector2(0.265f, 1.550f));
        }

        public CubicBezierEasingFunction CreateEaseInSineEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.470f, 0.000f), new Vector2(0.745f, 0.715f));
        }

        public CubicBezierEasingFunction CreateEaseOutSineEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.390f, 0.575f), new Vector2(0.565f, 1.000f));
        }

        public CubicBezierEasingFunction CreateEaseInOutSineEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.445f, 0.050f), new Vector2(0.550f, 0.950f));
        }

        public CubicBezierEasingFunction CreateEaseOutCircleEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.075f, 0.820f), new Vector2(0.165f, 1.000f));
        }

        public CubicBezierEasingFunction CreateEaseOutExponentialEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.190f, 1.000f), new Vector2(0.220f, 1.000f));
        }

        public CubicBezierEasingFunction CreateEaseOutQuadraticEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.250f, 0.460f), new Vector2(0.450f, 0.940f));
        }

        public CubicBezierEasingFunction CreateEaseOutQuarticEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.165f, 0.840f), new Vector2(0.440f, 1.000f));
        }

        public CubicBezierEasingFunction CreateEaseOutQuinticEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.230f, 1.000f), new Vector2(0.320f, 1.000f));
        }

        public CubicBezierEasingFunction CreateEaseInOutCircleEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.785f, 0.135f), new Vector2(0.150f, 0.860f));
        }

        public CubicBezierEasingFunction CreateEaseInOutExponentialEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(1.000f, 0.000f), new Vector2(0.000f, 1.000f));
        }

        public CubicBezierEasingFunction CreateEaseInOutQuadraticEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.455f, 0.030f), new Vector2(0.515f, 0.955f));
        }

        public CubicBezierEasingFunction CreateEaseInOutQuarticEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.770f, 0.000f), new Vector2(0.175f, 1.000f));
        }

        public CubicBezierEasingFunction CreateEaseInOutQuinticEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.860f, 0.000f), new Vector2(0.070f, 1.000f));
        }

        public CubicBezierEasingFunction CreateEaseInCircleEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.600f, 0.040f), new Vector2(0.980f, 0.335f));
        }

        public CubicBezierEasingFunction CreateEaseInExponentialEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.950f, 0.050f), new Vector2(0.795f, 0.035f));
        }

        public CubicBezierEasingFunction CreateEaseInQuadraticEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.550f, 0.085f), new Vector2(0.680f, 0.530f));
        }

        public CubicBezierEasingFunction CreateEaseInQuarticEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.895f, 0.030f), new Vector2(0.685f, 0.220f));
        }

        public CubicBezierEasingFunction CreateEaseInQuinticEasingFunction()
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.755f, 0.050f), new Vector2(0.855f, 0.060f));
        }

        internal KeyFrameAnimation CreateAnimationByType(CompositionVisualPropertyType type,
        double duration = 800, double delay = 0, CompositionEasingFunction easing = null)
        {
            KeyFrameAnimation animation;

            switch (type)
            {
                case CompositionVisualPropertyType.Offset:
                case CompositionVisualPropertyType.Scale:
                    animation = compositor.CreateVector3KeyFrameAnimation();
                    break;
                case CompositionVisualPropertyType.Size:
                    animation = compositor.CreateVector2KeyFrameAnimation();
                    break;
                case CompositionVisualPropertyType.Opacity:
                case CompositionVisualPropertyType.RotationAngleInDegrees:
                    animation = compositor.CreateScalarKeyFrameAnimation();
                    break;
                default:
                    return null;
            }

            animation.InsertExpressionKeyFrame(1.0f, "this.FinalValue", easing);
            animation.Duration = TimeSpan.FromMilliseconds(duration);
            animation.DelayTime = TimeSpan.FromMilliseconds(delay);
            animation.Target = type.ToString();

            return animation;
        }
    }
}
