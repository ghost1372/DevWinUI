using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

public partial class OffsetBox : ContentControl
{
    private Visual _visual;
    private Compositor _compositor;

    public bool ShowShadowOnHover
    {
        get { return (bool)GetValue(ShowShadowOnHoverProperty); }
        set { SetValue(ShowShadowOnHoverProperty, value); }
    }

    public static readonly DependencyProperty ShowShadowOnHoverProperty =
        DependencyProperty.Register(nameof(ShowShadowOnHover), typeof(bool), typeof(OffsetBox), new PropertyMetadata(true, OnShowShadowOnHoverChanged));

    private static void OnShowShadowOnHoverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (OffsetBox)d;
        if (ctl != null)
        {
            ctl.UpdateShowShadowOnHover();
        }
    }

    public Vector3 HoverValue
    {
        get { return (Vector3)GetValue(HoverValueProperty); }
        set { SetValue(HoverValueProperty, value); }
    }

    public static readonly DependencyProperty HoverValueProperty =
        DependencyProperty.Register(nameof(HoverValue), typeof(Vector3), typeof(OffsetBox), new PropertyMetadata(new Vector3(0, -5f, 0)));

    public Vector3 NormalValue
    {
        get { return (Vector3)GetValue(NormalValueProperty); }
        set { SetValue(NormalValueProperty, value); }
    }

    public static readonly DependencyProperty NormalValueProperty =
        DependencyProperty.Register(nameof(NormalValue), typeof(Vector3), typeof(OffsetBox), new PropertyMetadata(new Vector3(0, 0f, 0)));

    public TimeSpan HoverDuration
    {
        get { return (TimeSpan)GetValue(HoverDurationProperty); }
        set { SetValue(HoverDurationProperty, value); }
    }

    public static readonly DependencyProperty HoverDurationProperty =
        DependencyProperty.Register(nameof(HoverDuration), typeof(TimeSpan), typeof(OffsetBox), new PropertyMetadata(TimeSpan.FromMilliseconds(150)));

    public TimeSpan NormalDuration
    {
        get { return (TimeSpan)GetValue(NormalDurationProperty); }
        set { SetValue(NormalDurationProperty, value); }
    }

    public static readonly DependencyProperty NormalDurationProperty =
        DependencyProperty.Register(nameof(NormalDuration), typeof(TimeSpan), typeof(OffsetBox), new PropertyMetadata(TimeSpan.FromMilliseconds(150)));

    public Vector3 HoverShadowTranslation
    {
        get { return (Vector3)GetValue(HoverShadowTranslationProperty); }
        set { SetValue(HoverShadowTranslationProperty, value); }
    }

    public static readonly DependencyProperty HoverShadowTranslationProperty =
        DependencyProperty.Register(nameof(HoverShadowTranslation), typeof(Vector3), typeof(OffsetBox), new PropertyMetadata(new Vector3(0, 0, 32)));

    public Vector3 NormalShadowTranslation
    {
        get { return (Vector3)GetValue(NormalShadowTranslationProperty); }
        set { SetValue(NormalShadowTranslationProperty, value); }
    }

    public static readonly DependencyProperty NormalShadowTranslationProperty =
        DependencyProperty.Register(nameof(NormalShadowTranslation), typeof(Vector3), typeof(OffsetBox), new PropertyMetadata(new Vector3(0, 0, 0)));

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _visual = ElementCompositionPreview.GetElementVisual(this);

        _compositor = _visual.Compositor;

        if (Content is UIElement element && ShowShadowOnHover)
        {
            element.Shadow = new ThemeShadow();
        }
    }

    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);
        if (Content is UIElement element && ShowShadowOnHover)
        {
            element.Shadow = new ThemeShadow();
        }
    }

    private void UpdateShowShadowOnHover()
    {
        if (Content is UIElement element && element.Shadow == null)
        {
            element.Shadow = new ThemeShadow();
        }
    }
    private void AnimateHover(bool hover)
    {
        if (_visual == null) return;

        if (Content is UIElement element && ShowShadowOnHover)
        {
            element.Translation = hover ? HoverShadowTranslation : NormalShadowTranslation;
        }

        var offsetAnim = _compositor.CreateVector3KeyFrameAnimation();
        offsetAnim.Target = "Offset";
        offsetAnim.InsertKeyFrame(1f, hover ? HoverValue : NormalValue, _compositor.CreateLinearEasingFunction());

        offsetAnim.Duration = hover ? HoverDuration : NormalDuration;
        _visual.StartAnimation("Offset", offsetAnim);
    }

    protected override void OnPointerEntered(PointerRoutedEventArgs e)
    {
        base.OnPointerEntered(e);
        AnimateHover(true);
    }

    protected override void OnPointerExited(PointerRoutedEventArgs e)
    {
        base.OnPointerExited(e);
        AnimateHover(false);
    }
}
