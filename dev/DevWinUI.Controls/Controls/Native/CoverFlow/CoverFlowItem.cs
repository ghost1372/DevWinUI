using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

public partial class CoverFlowItem : ContentControl
{
    private const string PART_ContentPresenter = "PART_ContentPresenter";
    private const string PART_Rotator = "PART_Rotator";
    private const string PART_LayoutRoot = "PART_LayoutRoot";
    private const string PART_Animation = "PART_Animation";
    private const string PART_RotationKeyFrame = "PART_RotationKeyFrame";
    private const string PART_OffestZKeyFrame = "PART_OffestZKeyFrame";
    private const string PART_ScaleXKeyFrame = "PART_ScaleXKeyFrame";
    private const string PART_ScaleYKeyFrame = "PART_ScaleYKeyFrame";
    private const string PART_ScaleTransform = "PART_ScaleTransform";
    public event EventHandler ItemSelected;

    private Grid layoutRoot;
    private PlaneProjection planeProjection;
    private Storyboard animation;
    private ScaleTransform scaleTransform;
    private EasingDoubleKeyFrame rotationKeyFrame, offestZKeyFrame, scaleXKeyFrame, scaleYKeyFrame;

    private double yRotation;
    private double zOffset;
    private double scale;
    private double x;
    private bool isAnimating;
    private ContentControl contentPresenter;
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

    public new double Scale
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
        if (rotationKeyFrame == null || offestZKeyFrame == null || scaleXKeyFrame == null || scaleYKeyFrame == null)
            return;

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
            animation.Begin();
            Canvas.SetZIndex(this, zIndex);
        }

        this.x = x;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        contentPresenter = GetTemplateChild(PART_ContentPresenter) as ContentControl;
        planeProjection = GetTemplateChild(PART_Rotator) as PlaneProjection;
        layoutRoot = GetTemplateChild(PART_LayoutRoot) as Grid;

        animation = GetTemplateChild(PART_Animation) as Storyboard;
        rotationKeyFrame = GetTemplateChild(PART_RotationKeyFrame) as EasingDoubleKeyFrame;
        offestZKeyFrame = GetTemplateChild(PART_OffestZKeyFrame) as EasingDoubleKeyFrame;
        scaleXKeyFrame = GetTemplateChild(PART_ScaleXKeyFrame) as EasingDoubleKeyFrame;
        scaleYKeyFrame = GetTemplateChild(PART_ScaleYKeyFrame) as EasingDoubleKeyFrame;
        scaleTransform = GetTemplateChild(PART_ScaleTransform) as ScaleTransform;

        if (planeProjection != null)
        {
            planeProjection.RotationY = yRotation;
            planeProjection.LocalOffsetZ = zOffset;
        }

        if (contentPresenter != null)
        {
            contentPresenter.Tapped += ContentPresenter_Tapped;
        }

        if (animation != null)
        {
            animation.Completed -= Animation_Completed;
            animation.Completed += Animation_Completed;

            xAnimation = new DoubleAnimation();
            animation.Children.Add(xAnimation);

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
