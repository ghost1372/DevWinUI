using Microsoft.UI.Xaml;

namespace DevWinUI;

public partial class ColourfulBubblesRenderer
{
    private ColourfulBubblesDirection direction = ColourfulBubblesDirection.TopToBottom;
    private float mouseX;
    private float mouseY;
    public ColourfulBubblesDirection Direction
    {
        get { return (ColourfulBubblesDirection)GetValue(DirectionProperty); }
        set { SetValue(DirectionProperty, value); }
    }

    public static readonly DependencyProperty DirectionProperty =
        DependencyProperty.Register(nameof(Direction), typeof(ColourfulBubblesDirection), typeof(ColourfulBubblesRenderer), new PropertyMetadata(ColourfulBubblesDirection.TopToBottom, OnDirectionChanged));

    private static void OnDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColourfulBubblesRenderer)d;
        ctl.direction = (ColourfulBubblesDirection)e.NewValue;
    }

    public double MouseX
    {
        get { return (double)GetValue(MouseXProperty); }
        set { SetValue(MouseXProperty, value); }
    }

    public static readonly DependencyProperty MouseXProperty =
        DependencyProperty.Register(nameof(MouseX), typeof(double), typeof(ColourfulBubblesRenderer), new PropertyMetadata(0.0d, OnMouseXChanged));

    private static void OnMouseXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColourfulBubblesRenderer)d;
        ctl.mouseX = (float)(double)e.NewValue;
    }
    public double MouseY
    {
        get { return (double)GetValue(MouseYProperty); }
        set { SetValue(MouseYProperty, value); }
    }

    public static readonly DependencyProperty MouseYProperty =
        DependencyProperty.Register(nameof(MouseY), typeof(double), typeof(ColourfulBubblesRenderer), new PropertyMetadata(0.0d, OnMouseYChanged));

    private static void OnMouseYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColourfulBubblesRenderer)d;
        ctl.mouseY = (float)(double)e.NewValue;
    }
}
