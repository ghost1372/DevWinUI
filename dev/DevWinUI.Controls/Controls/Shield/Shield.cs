using Microsoft.UI.Xaml.Controls.Primitives;

namespace DevWinUI;
public partial class Shield : ButtonBase
{
    public static readonly DependencyProperty SubjectProperty = DependencyProperty.Register(
        nameof(Subject), typeof(object), typeof(Shield), new PropertyMetadata(default(object)));

    public object Subject
    {
        get => (object)GetValue(SubjectProperty);
        set => SetValue(SubjectProperty, value);
    }

    public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(
        nameof(Status), typeof(object), typeof(Shield), new PropertyMetadata(default(object)));

    public object Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
        nameof(Color), typeof(Brush), typeof(Shield), new PropertyMetadata(default(Brush)));

    public Brush Color
    {
        get => (Brush)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
}
