using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace DevWinUI_Template;

[ContentProperty(nameof(Content))]
public class Card : Control
{
    static Card()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Card), new FrameworkPropertyMetadata(typeof(Card)));
    }

    public object Content
    {
        get { return (object)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(Card), new PropertyMetadata(null));

    public bool IsActive
    {
        get { return (bool)GetValue(IsActiveProperty); }
        set { SetValue(IsActiveProperty, value); }
    }

    public static readonly DependencyProperty IsActiveProperty =
        DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(Card), new PropertyMetadata(false));

    public CornerRadius CornerRadius
    {
        get { return (CornerRadius)GetValue(CornerRadiusProperty); }
        set { SetValue(CornerRadiusProperty, value); }
    }

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(Card), new PropertyMetadata(default(CornerRadius)));

    public double BlurRadius
    {
        get { return (double)GetValue(BlurRadiusProperty); }
        set { SetValue(BlurRadiusProperty, value); }
    }

    public static readonly DependencyProperty BlurRadiusProperty =
        DependencyProperty.Register(nameof(BlurRadius), typeof(double), typeof(Card), new PropertyMetadata(5.0));


    public double GlowOpacity
    {
        get { return (double)GetValue(GlowOpacityProperty); }
        set { SetValue(GlowOpacityProperty, value); }
    }

    public static readonly DependencyProperty GlowOpacityProperty =
        DependencyProperty.Register(nameof(GlowOpacity), typeof(double), typeof(Card), new PropertyMetadata(1.0));

    public Color GlowColor
    {
        get { return (Color)GetValue(GlowColorProperty); }
        set { SetValue(GlowColorProperty, value); }
    }

    public static readonly DependencyProperty GlowColorProperty =
        DependencyProperty.Register(nameof(GlowColor), typeof(Color), typeof(Card), new PropertyMetadata(default(Color)));
}
