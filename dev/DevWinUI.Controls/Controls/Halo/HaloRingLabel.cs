using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

public partial class HaloRingLabel : HaloChain
{
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(HaloRingLabel), new PropertyMetadata("", RefreshText));

    public bool Flip
    {
        get { return (bool)GetValue(FlipProperty); }
        set { SetValue(FlipProperty, value); }
    }
    public static readonly DependencyProperty FlipProperty =
        DependencyProperty.Register(nameof(Flip), typeof(bool), typeof(HaloRingLabel), new PropertyMetadata(false, RefreshFlip));

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }
    public static readonly DependencyProperty FontSizeProperty =
        DependencyProperty.Register(nameof(FontSize), typeof(double), typeof(HaloRingLabel), new PropertyMetadata(16.0));

    public HaloRingLabel()
    {
        BindingOperations.SetBinding(this, HaloPanel.ThicknessProperty, new Binding
        {
            Source = this, Path = new PropertyPath("FontSize"), Mode = BindingMode.TwoWay
        });
    }

    private static void RefreshFlip(object o, DependencyPropertyChangedEventArgs e)
    {
        var label = (HaloRingLabel)o;

        if (label.Flip)
        {
            label.Offset = 180;
            label.FlowDirection = FlowDirection.RightToLeft;
        }
        else
        {
            label.Offset = 0;
            label.FlowDirection = FlowDirection.LeftToRight;
        }
    }

    private static void RefreshText(object o, DependencyPropertyChangedEventArgs e)
    {
        var label = (HaloRingLabel)o;

        label.Children.Clear();

        foreach (var letter in label.Text)
        {
            if (letter == ' ')
            {
                label.Children.Add(MakeSpace(label));
            }
            else
            {
                label.Children.Add(new TextBlock
                {
                    Text = letter.ToString()
                });
            }
        }
    }

    private static UIElement MakeSpace(HaloRingLabel label)
    {
        var space = new Rectangle();

        BindingOperations.SetBinding(space, FrameworkElement.WidthProperty, new Binding
        {
            Source = label, Path = new PropertyPath("FontSize"), Mode = BindingMode.TwoWay
        });

        BindingOperations.SetBinding(space, FrameworkElement.HeightProperty, new Binding
        {
            Source = label, Path = new PropertyPath("FontSize"), Mode = BindingMode.TwoWay
        });

        return space;
    }
}
