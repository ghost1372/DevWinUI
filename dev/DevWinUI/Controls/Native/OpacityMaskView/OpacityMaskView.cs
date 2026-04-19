// https://github.com/cnbluefire

using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

public partial class OpacityMaskView : RedirectVisualView
{
    public OpacityMaskView()
    {
        opacityMaskHost = new Rectangle();

        compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        opacityMaskVisualSurface = compositor.CreateVisualSurface();
        opacityMaskVisualBrush = compositor.CreateSurfaceBrush(opacityMaskVisualSurface);
        opacityMaskVisualBrush.HorizontalAlignmentRatio = 0;
        opacityMaskVisualBrush.VerticalAlignmentRatio = 0;
        opacityMaskVisualBrush.Stretch = CompositionStretch.None;

        maskBrush = compositor.CreateMaskBrush();
        maskBrush.Mask = opacityMaskVisualBrush;
        maskBrush.Source = ChildVisualBrush;

        RootVisual.Brush = maskBrush;
    }

    private Rectangle opacityMaskHost;

    private Compositor compositor;

    private CompositionVisualSurface opacityMaskVisualSurface;
    private CompositionSurfaceBrush opacityMaskVisualBrush;
    private CompositionMaskBrush maskBrush;

    public Brush? OpacityMask
    {
        get { return (Brush?)GetValue(OpacityMaskProperty); }
        set { SetValue(OpacityMaskProperty, value); }
    }

    public static readonly DependencyProperty OpacityMaskProperty =
        DependencyProperty.Register(nameof(OpacityMask), typeof(Brush), typeof(OpacityMaskView), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)), OnOpacityMaskPropertyChanged));

    private static void OnOpacityMaskPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is OpacityMaskView sender && !Equals(e.NewValue, e.OldValue))
        {
            if (sender.RedirectVisualAttached)
            {
                sender.opacityMaskHost.Fill = e.NewValue as Brush;
            }
            else
            {
                sender.opacityMaskHost.Fill = null;
            }
        }
    }

    protected override void OnDetachVisuals()
    {
        base.OnDetachVisuals();

        if (LayoutRoot != null)
        {
            if (opacityMaskHost != null)
            {
                opacityMaskHost.Fill = null;
            }

            opacityMaskVisualSurface.SourceVisual = null;

            if (OpacityMaskContainer != null)
            {
                OpacityMaskContainer.Visibility = Visibility.Collapsed;
                OpacityMaskContainer.Children.Remove(opacityMaskHost);
            }
        }
    }

    protected override void OnAttachVisuals()
    {
        base.OnAttachVisuals();

        if (LayoutRoot != null)
        {
            if (opacityMaskHost != null)
            {
                opacityMaskHost.Fill = OpacityMask;
                opacityMaskVisualSurface.SourceVisual = ElementCompositionPreview.GetElementVisual(opacityMaskHost);
            }

            if (OpacityMaskContainer != null)
            {
                OpacityMaskContainer.Visibility = Visibility.Visible;
                ElementCompositionPreview.GetElementVisual(OpacityMaskContainer).IsVisible = false;
                OpacityMaskContainer.Children.Add(opacityMaskHost);
            }
        }
    }

    protected override void OnUpdateSize()
    {
        base.OnUpdateSize();

        if (RedirectVisualAttached && LayoutRoot != null)
        {
            if (opacityMaskHost != null)
            {
                opacityMaskHost.Width = LayoutRoot.ActualWidth;
                opacityMaskHost.Height = LayoutRoot.ActualHeight;
            }

            opacityMaskVisualSurface.SourceSize = new Vector2((float)LayoutRoot.ActualWidth, (float)LayoutRoot.ActualHeight);
        }
    }
}
