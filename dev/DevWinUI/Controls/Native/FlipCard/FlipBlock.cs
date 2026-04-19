namespace DevWinUI;

[TemplatePart(Name = "PART_BlockFlip", Type = typeof(Grid))]
[TemplatePart(Name = "PART_TextTop", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_TextBottom", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_TextFlipTop", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_TextFlipBottom", Type = typeof(TextBlock))]
public partial class FlipBlock : Control
{
    private Grid blockFlip;
    private TextBlock textTop;
    private TextBlock textBottom;
    private TextBlock textFlipTop;
    private TextBlock textFlipBottom;

    private Storyboard FlipAnimation;
    private string _from;

    public string Value
    {
        get { return (string)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(string), typeof(FlipBlock), new PropertyMetadata(null, OnValueChanged));

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (FlipBlock)d;
        if (ctl != null)
        {
            ctl.OnValueChanged((string)e.NewValue);
        }
    }
    public FlipBlock()
    {
        DefaultStyleKey = typeof(FlipBlock);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        blockFlip = GetTemplateChild("PART_BlockFlip") as Grid;
        textTop = GetTemplateChild("PART_TextTop") as TextBlock;
        textBottom = GetTemplateChild("PART_TextBottom") as TextBlock;
        textFlipTop = GetTemplateChild("PART_TextFlipTop") as TextBlock;
        textFlipBottom = GetTemplateChild("PART_TextFlipBottom") as TextBlock;

        InitializeFlipAnimation();

        OnValueChanged(Value);
    }

    private void InitializeFlipAnimation()
    {
        FlipAnimation = new Storyboard();

        var scaleAnim = new DoubleAnimationUsingKeyFrames();
        Storyboard.SetTarget(scaleAnim, blockFlip);
        Storyboard.SetTargetProperty(scaleAnim, "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)");

        var key1 = new EasingDoubleKeyFrame { Value = 1, KeyTime = TimeSpan.Zero };
        key1.EasingFunction = new BounceEase { Bounces = 1, Bounciness = 6, EasingMode = EasingMode.EaseOut };
        scaleAnim.KeyFrames.Add(key1);

        var key2 = new EasingDoubleKeyFrame { Value = -1, KeyTime = TimeSpan.FromMilliseconds(250) };
        key2.EasingFunction = new BounceEase { Bounces = 1, Bounciness = 6, EasingMode = EasingMode.EaseOut };
        scaleAnim.KeyFrames.Add(key2);

        FlipAnimation.Children.Add(scaleAnim);

        var topVisibilityAnim = new ObjectAnimationUsingKeyFrames();
        Storyboard.SetTarget(topVisibilityAnim, textFlipTop);
        Storyboard.SetTargetProperty(topVisibilityAnim, "Visibility");

        topVisibilityAnim.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = TimeSpan.Zero, Value = Visibility.Visible });
        topVisibilityAnim.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = TimeSpan.FromMilliseconds(125), Value = Visibility.Collapsed });
        FlipAnimation.Children.Add(topVisibilityAnim);

        var bottomVisibilityAnim = new ObjectAnimationUsingKeyFrames();
        Storyboard.SetTarget(bottomVisibilityAnim, textFlipBottom);
        Storyboard.SetTargetProperty(bottomVisibilityAnim, "Visibility");

        bottomVisibilityAnim.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = TimeSpan.Zero, Value = Visibility.Collapsed });
        bottomVisibilityAnim.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = TimeSpan.FromMilliseconds(125), Value = Visibility.Visible });
        FlipAnimation.Children.Add(bottomVisibilityAnim);
    }

    private void OnValueChanged(string value)
    {
        if (textFlipTop == null || textBottom == null)
            return;

        if (_from != null && _from != value)
        {
            textTop.Text = textFlipBottom.Text = value;
            textFlipTop.Text = _from;

            FlipAnimation.Completed -= FlipAnimation_Completed;
            FlipAnimation.Completed += FlipAnimation_Completed;
            FlipAnimation.Begin();
        }

        if (_from == null)
        {
            textFlipTop.Text = textBottom.Text = value;
        }

        _from = value;
    }
    private void FlipAnimation_Completed(object sender, object e)
    {
        textBottom.Text = _from;
    }
}
