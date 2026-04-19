namespace DevWinUI;
public partial class TextBlockStrokeView : Control
{
    public bool Animate
    {
        get { return (bool)GetValue(AnimateProperty); }
        set { SetValue(AnimateProperty, value); }
    }

    public static readonly DependencyProperty AnimateProperty =
        DependencyProperty.Register(nameof(Animate), typeof(bool), typeof(TextBlockStrokeView), new PropertyMetadata(false, OnAnimateChanged));

    private static void OnAnimateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (TextBlockStrokeView)d;
        if (ctl != null)
        {
            var storyBoard = Application.Current.Resources["EffectBrushAnimation"] as Storyboard;

            if ((bool)e.NewValue)
            {
                ctl.StrokeBrush = Application.Current.Resources["TextBlockStrokeViewGradientBrushSample"] as LinearGradientBrush;
                storyBoard?.Begin();
            }
            else
            {
                storyBoard?.Stop();
            }
        }
    }
}
