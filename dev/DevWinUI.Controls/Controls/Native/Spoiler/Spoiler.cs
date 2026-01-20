using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

public partial class Spoiler : ContentControl
{
    public event EventHandler<SpoilerRevealChangedEventArgs> RevealChanged;
    public string Header
    {
        get { return (string)GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(string), typeof(Spoiler), new PropertyMetadata(default(string)));

    public IconSource HeaderIcon
    {
        get { return (IconSource)GetValue(HeaderIconProperty); }
        set { SetValue(HeaderIconProperty, value); }
    }

    public static readonly DependencyProperty HeaderIconProperty =
        DependencyProperty.Register(nameof(HeaderIcon), typeof(IconSource), typeof(Spoiler), new PropertyMetadata(default(IconSource)));

    public VerticalAlignment HeaderVerticalAlignment
    {
        get { return (VerticalAlignment)GetValue(HeaderVerticalAlignmentProperty); }
        set { SetValue(HeaderVerticalAlignmentProperty, value); }
    }

    public static readonly DependencyProperty HeaderVerticalAlignmentProperty =
        DependencyProperty.Register(nameof(HeaderVerticalAlignment), typeof(VerticalAlignment), typeof(Spoiler), new PropertyMetadata(default(VerticalAlignment)));

    public HorizontalAlignment HeaderHorizontalAlignment
    {
        get { return (HorizontalAlignment)GetValue(HeaderHorizontalAlignmentProperty); }
        set { SetValue(HeaderHorizontalAlignmentProperty, value); }
    }

    public static readonly DependencyProperty HeaderHorizontalAlignmentProperty =
        DependencyProperty.Register(nameof(HeaderHorizontalAlignment), typeof(HorizontalAlignment), typeof(Spoiler), new PropertyMetadata(default(HorizontalAlignment)));

    public Thickness HeaderMargin
    {
        get { return (Thickness)GetValue(HeaderMarginProperty); }
        set { SetValue(HeaderMarginProperty, value); }
    }

    public static readonly DependencyProperty HeaderMarginProperty =
        DependencyProperty.Register(nameof(HeaderMargin), typeof(Thickness), typeof(Spoiler), new PropertyMetadata(default(Thickness)));

    public double ShimmerWidth
    {
        get { return (double)GetValue(ShimmerWidthProperty); }
        set { SetValue(ShimmerWidthProperty, value); }
    }

    public static readonly DependencyProperty ShimmerWidthProperty =
        DependencyProperty.Register(nameof(ShimmerWidth), typeof(double), typeof(Spoiler), new PropertyMetadata(double.NaN));

    public double ShimmerHeight
    {
        get { return (double)GetValue(ShimmerHeightProperty); }
        set { SetValue(ShimmerHeightProperty, value); }
    }

    public static readonly DependencyProperty ShimmerHeightProperty =
        DependencyProperty.Register(nameof(ShimmerHeight), typeof(double), typeof(Spoiler), new PropertyMetadata(double.NaN));

    public CornerRadius ShimmerCornerRadius
    {
        get { return (CornerRadius)GetValue(ShimmerCornerRadiusProperty); }
        set { SetValue(ShimmerCornerRadiusProperty, value); }
    }

    public static readonly DependencyProperty ShimmerCornerRadiusProperty =
        DependencyProperty.Register(nameof(ShimmerCornerRadius), typeof(CornerRadius), typeof(Spoiler), new PropertyMetadata(default(CornerRadius)));

    public DataTemplate ShimmerTemplate
    {
        get { return (DataTemplate)GetValue(ShimmerTemplateProperty); }
        set { SetValue(ShimmerTemplateProperty, value); }
    }

    public static readonly DependencyProperty ShimmerTemplateProperty =
        DependencyProperty.Register(nameof(ShimmerTemplate), typeof(DataTemplate), typeof(Spoiler), new PropertyMetadata(null));

    public bool IsActiveShimmer
    {
        get { return (bool)GetValue(IsActiveShimmerProperty); }
        set { SetValue(IsActiveShimmerProperty, value); }
    }

    public static readonly DependencyProperty IsActiveShimmerProperty =
        DependencyProperty.Register(nameof(IsActiveShimmer), typeof(bool), typeof(Spoiler), new PropertyMetadata(true, OnIsActiveShimmerChanged));

    private static void OnIsActiveShimmerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (Spoiler)d;
        if (ctl != null)
        {
            ctl.InternalIsActiveShimmer = (bool)e.NewValue;
        }
    }

    internal bool InternalIsActiveShimmer
    {
        get { return (bool)GetValue(InternalIsActiveShimmerProperty); }
        set { SetValue(InternalIsActiveShimmerProperty, value); }
    }

    public static readonly DependencyProperty InternalIsActiveShimmerProperty =
        DependencyProperty.Register(nameof(InternalIsActiveShimmer), typeof(bool), typeof(Spoiler), new PropertyMetadata(true));

    public bool IsRevealed
    {
        get { return (bool)GetValue(IsRevealedProperty); }
        set { SetValue(IsRevealedProperty, value); }
    }

    public static readonly DependencyProperty IsRevealedProperty =
        DependencyProperty.Register(nameof(IsRevealed), typeof(bool), typeof(Spoiler), new PropertyMetadata(false, OnIsRevealedChanged));
    private static void OnIsRevealedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (Spoiler)d;
        if (ctl != null)
        {
            var result = (bool)e.NewValue;
            ctl.UpdateVisualState();

            if (result)
            {
                ctl.InternalIsActiveShimmer = false;
            }
            else
            {
                if (ctl.IsActiveShimmer)
                {
                    ctl.InternalIsActiveShimmer = true;
                }
            }

            if (result && ctl.RevealMode == SpoilerRevealMode.Once)
            {
                GeneralHelper.ChangeCursor(ctl, InputSystemCursor.Create(InputSystemCursorShape.Arrow));
            }
            else
            {
                GeneralHelper.ChangeCursor(ctl, InputSystemCursor.Create(InputSystemCursorShape.Hand));
            }

            ctl.RevealChanged?.Invoke(ctl, new SpoilerRevealChangedEventArgs(result));
        }
    }

    public SpoilerRevealMode RevealMode
    {
        get { return (SpoilerRevealMode)GetValue(RevealModeProperty); }
        set { SetValue(RevealModeProperty, value); }
    }

    public static readonly DependencyProperty RevealModeProperty =
        DependencyProperty.Register(nameof(RevealMode), typeof(SpoilerRevealMode), typeof(Spoiler), new PropertyMetadata(SpoilerRevealMode.Once));

    public Spoiler()
    {
        DefaultStyleKey = typeof(Spoiler);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        Tapped -= OnTapped;
        Tapped += OnTapped;

        UpdateVisualState();

        if (RevealMode == SpoilerRevealMode.Toggle || !IsRevealed)
        {
            GeneralHelper.ChangeCursor(this, InputSystemCursor.Create(InputSystemCursorShape.Hand));
        }
    }

    private void OnTapped(object sender, TappedRoutedEventArgs e)
    {
        if (RevealMode == SpoilerRevealMode.Once)
        {
            IsRevealed = true;
        }
        else
        {
            IsRevealed = !IsRevealed;
        }
    }

    private void UpdateVisualState()
    {
        VisualStateManager.GoToState(this, IsRevealed ? "Revealed" : "Hidden", true);
    }
}
