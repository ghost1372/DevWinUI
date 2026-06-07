using Microsoft.UI.Xaml;

namespace DevWinUI;

public partial class ShaderBackground
{
    private int edgeFeatheringLeft = 0;
    private int edgeFeatheringTop = 0;
    private int edgeFeatheringRight = 0;
    private int edgeFeatheringBottom = 0;

    public int EdgeFeatheringLeft
    {
        get { return (int)GetValue(EdgeFeatheringLeftProperty); }
        set { SetValue(EdgeFeatheringLeftProperty, value); }
    }

    public static readonly DependencyProperty EdgeFeatheringLeftProperty =
        DependencyProperty.Register(nameof(EdgeFeatheringLeft), typeof(int), typeof(ShaderBackground), new PropertyMetadata(0, OnEdgeFeatheringLeftChanged));

    private static void OnEdgeFeatheringLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ShaderBackground)d;
        ctl.edgeFeatheringLeft = (int)e.NewValue;
    }

    public int EdgeFeatheringTop
    {
        get { return (int)GetValue(EdgeFeatheringTopProperty); }
        set { SetValue(EdgeFeatheringTopProperty, value); }
    }

    public static readonly DependencyProperty EdgeFeatheringTopProperty =
        DependencyProperty.Register(nameof(EdgeFeatheringTop), typeof(int), typeof(ShaderBackground), new PropertyMetadata(0, OnEdgeFeatheringTopChanged));

    private static void OnEdgeFeatheringTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ShaderBackground)d;
        ctl.edgeFeatheringTop = (int)e.NewValue;
    }

    public int EdgeFeatheringRight
    {
        get { return (int)GetValue(EdgeFeatheringRightProperty); }
        set { SetValue(EdgeFeatheringRightProperty, value); }
    }

    public static readonly DependencyProperty EdgeFeatheringRightProperty =
        DependencyProperty.Register(nameof(EdgeFeatheringRight), typeof(int), typeof(ShaderBackground), new PropertyMetadata(0, OnEdgeFeatheringRightChanged));

    private static void OnEdgeFeatheringRightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ShaderBackground)d;
        ctl.edgeFeatheringRight = (int)e.NewValue;
    }

    public int EdgeFeatheringBottom
    {
        get { return (int)GetValue(EdgeFeatheringBottomProperty); }
        set { SetValue(EdgeFeatheringBottomProperty, value); }
    }

    public static readonly DependencyProperty EdgeFeatheringBottomProperty =
        DependencyProperty.Register(nameof(EdgeFeatheringBottom), typeof(int), typeof(ShaderBackground), new PropertyMetadata(0, OnEdgeFeatheringBottomChanged));

    private static void OnEdgeFeatheringBottomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ShaderBackground)d;
        ctl.edgeFeatheringBottom = (int)e.NewValue;
    }
}
