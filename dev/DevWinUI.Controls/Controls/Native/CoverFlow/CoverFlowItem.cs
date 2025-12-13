using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

public partial class CoverFlowItem : ContentControl
{
    public event EventHandler ItemSelected;

    private FrameworkElement LayoutRoot;
    private PlaneProjection planeProjection;
    private Storyboard Animation;
    private ScaleTransform scaleTransform;
    private EasingDoubleKeyFrame rotationKeyFrame, offestZKeyFrame, scaleXKeyFrame, scaleYKeyFrame;

    private double yRotation;
    private double zOffset;
    private double scale;
    private double x;
    private bool isAnimating;
    private ContentControl ContentPresenter;
    private Duration duration;
    private EasingFunctionBase easingFunction;
    private DoubleAnimation xAnimation;

    public CoverFlowItem()
    {
        DefaultStyleKey = typeof(CoverFlowItem);
    }

    public double YRotation
    {
        get
        {
            return yRotation;
        }
        set
        {
            yRotation = value;
            if (planeProjection != null)
            {
                planeProjection.RotationY = value;
            }
        }
    }

    public double ZOffset
    {
        get
        {
            return zOffset;
        }
        set
        {
            zOffset = value;
            if (planeProjection != null)
            {
                planeProjection.LocalOffsetZ = value;
            }
        }
    }

    public double Scale
    {
        get
        {
            return scale;
        }
        set
        {
            scale = value;
            if (scaleTransform != null)
            {
                scaleTransform.ScaleX = scale;
                scaleTransform.ScaleY = scale;
            }
        }
    }

    public double X
    {
        get
        {
            return x;
        }
        set
        {
            x = value;
            Canvas.SetLeft(this, value);
        }
    }

    public void SetValues(double x, int zIndex, double r, double z, double s, Duration d, EasingFunctionBase ease, bool useAnimation)
    {
        if (useAnimation)
        {
            if (!isAnimating && Canvas.GetLeft(this) != x)
                Canvas.SetLeft(this, this.x);

            rotationKeyFrame.Value = r;
            offestZKeyFrame.Value = z;
            scaleYKeyFrame.Value = s;
            scaleXKeyFrame.Value = s;
            xAnimation.To = x;

            if (duration != d)
            {
                duration = d;
                rotationKeyFrame.KeyTime = KeyTime.FromTimeSpan(d.TimeSpan);
                offestZKeyFrame.KeyTime = KeyTime.FromTimeSpan(d.TimeSpan);
                scaleYKeyFrame.KeyTime = KeyTime.FromTimeSpan(d.TimeSpan);
                scaleXKeyFrame.KeyTime = KeyTime.FromTimeSpan(d.TimeSpan);
                xAnimation.Duration = d;
            }
            if (easingFunction != ease)
            {
                easingFunction = ease;
                rotationKeyFrame.EasingFunction = ease;
                offestZKeyFrame.EasingFunction = ease;
                scaleYKeyFrame.EasingFunction = ease;
                scaleXKeyFrame.EasingFunction = ease;
                xAnimation.EasingFunction = ease;
            }

            isAnimating = true;
            Animation.Begin();
            Canvas.SetZIndex(this, zIndex);
        }

        this.x = x;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        ContentPresenter = (ContentControl)GetTemplateChild("ContentPresenter");
        planeProjection = (PlaneProjection)GetTemplateChild("Rotator");
        LayoutRoot = (FrameworkElement)GetTemplateChild("LayoutRoot");

        Animation = (Storyboard)GetTemplateChild("Animation");
        Animation.Completed += Animation_Completed;
        rotationKeyFrame = (EasingDoubleKeyFrame)GetTemplateChild("rotationKeyFrame");
        offestZKeyFrame = (EasingDoubleKeyFrame)GetTemplateChild("offestZKeyFrame");
        scaleXKeyFrame = (EasingDoubleKeyFrame)GetTemplateChild("scaleXKeyFrame");
        scaleYKeyFrame = (EasingDoubleKeyFrame)GetTemplateChild("scaleYKeyFrame");
        scaleTransform = (ScaleTransform)GetTemplateChild("scaleTransform");

        planeProjection.RotationY = yRotation;
        planeProjection.LocalOffsetZ = zOffset;
        if (ContentPresenter != null)
        {
            ContentPresenter.Tapped += ContentPresenter_Tapped;
        }

        if (Animation != null)
        {
            xAnimation = new DoubleAnimation();
            Animation.Children.Add(xAnimation);

            Storyboard.SetTarget(xAnimation, this);
            Storyboard.SetTargetProperty(xAnimation, "(Canvas.Left)");
        }
    }

    private void Animation_Completed(object sender, object e)
    {
        isAnimating = false;
    }

    private void ContentPresenter_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (ItemSelected != null)
            ItemSelected(this, null);
    }
}
