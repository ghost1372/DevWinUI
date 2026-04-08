using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[ContentProperty(Name = "TextBlock")]
public partial class TextBlockStrokeView : Control
{
    public TextBlockStrokeView()
    {
        this.DefaultStyleKey = typeof(TextBlockStrokeView);

        brushHost = new Rectangle()
        {
            Fill = StrokeBrush
        };

        brushHostCanvas = new Canvas()
        {
            Width = 0,
            Height = 0,
            IsTabStop = false,
            IsHitTestVisible = false,
            Children =
            {
                brushHost
            }
        };

        brushHostCanvas.GetVisualInternal().IsVisible = false;

        var brushHostVisual = brushHost.GetVisualInternal();

        var compositor = brushHostVisual.Compositor;

        brushHostVisualSurface = compositor.CreateVisualSurface();
        brushHostVisualSurface.SourceVisual = brushHostVisual;

        actualStrokeBrush = compositor.CreateSurfaceBrush(brushHostVisualSurface);

        sizeBind = compositor.CreateExpressionAnimation("visual.Size");
        sizeBind.SetReferenceParameter("visual", brushHostVisual);
        brushHostVisualSurface.StartAnimation("SourceSize", sizeBind);

        this.Loaded += (s, a) => ConnectStrokeVisual();
        this.Unloaded += (s, a) => DisconnectStrokeVisual();
    }

    private Canvas brushHostCanvas;
    private Rectangle brushHost;
    private CompositionVisualSurface brushHostVisualSurface;
    private CompositionSurfaceBrush actualStrokeBrush;
    private ExpressionAnimation sizeBind;

    private Grid? LayoutRoot;
    private Border? TextBlockBorder;
    private TextBlockStrokeHelper? textBlockStroke;

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        DisconnectStrokeVisual();

        if (TextBlockBorder != null)
        {
            TextBlockBorder.SizeChanged -= TextBlockBorder_SizeChanged;
        }
        if (LayoutRoot != null)
        {
            LayoutRoot.Children.Remove(brushHostCanvas);
        }

        LayoutRoot = (Grid)GetTemplateChild(nameof(LayoutRoot));
        TextBlockBorder = (Border)GetTemplateChild(nameof(TextBlockBorder));

        if (TextBlockBorder != null)
        {
            UpdateBrushHostSize(TextBlockBorder.ActualWidth, TextBlockBorder.ActualHeight);
            TextBlockBorder.SizeChanged += TextBlockBorder_SizeChanged;
        }
        if (LayoutRoot != null)
        {
            LayoutRoot.Children.Add(brushHostCanvas);
        }

        ConnectStrokeVisual();
    }

    public Brush StrokeBrush
    {
        get { return (Brush)GetValue(StrokeBrushProperty); }
        set { SetValue(StrokeBrushProperty, value); }
    }

    public static readonly DependencyProperty StrokeBrushProperty =
        DependencyProperty.Register("StrokeBrush", typeof(Brush), typeof(TextBlockStrokeView), new PropertyMetadata(null, (s, a) =>
        {
            if (s is TextBlockStrokeView sender && !Equals(a.NewValue, a.OldValue))
            {
                sender.brushHost.Fill = a.NewValue as Brush;
            }
        }));



    public double StrokeThickness
    {
        get { return (double)GetValue(StrokeThicknessProperty); }
        set { SetValue(StrokeThicknessProperty, value); }
    }

    public static readonly DependencyProperty StrokeThicknessProperty =
        DependencyProperty.Register("StrokeThickness", typeof(double), typeof(TextBlockStrokeView), new PropertyMetadata(0d, (s, a) =>
        {
            if (s is TextBlockStrokeView sender && !Equals(a.NewValue, a.OldValue))
            {
                if (sender.textBlockStroke != null)
                {
                    sender.textBlockStroke.StrokeThickness = Convert.ToSingle(a.NewValue);
                }
            }
        }));

    public TextBlockStrokeOptimization Optimization
    {
        get { return (TextBlockStrokeOptimization)GetValue(OptimizationProperty); }
        set { SetValue(OptimizationProperty, value); }
    }

    public static readonly DependencyProperty OptimizationProperty =
        DependencyProperty.Register("Optimization", typeof(TextBlockStrokeOptimization), typeof(TextBlockStrokeView), new PropertyMetadata(TextBlockStrokeOptimization.Balanced, (s, a) =>
        {
            if (s is TextBlockStrokeView sender && !Equals(a.NewValue, a.OldValue))
            {
                if (sender.textBlockStroke != null)
                {
                    sender.textBlockStroke.Optimization = (TextBlockStrokeOptimization)a.NewValue;
                }
            }
        }));

    public TextBlock TextBlock
    {
        get { return (TextBlock)GetValue(TextBlockProperty); }
        set { SetValue(TextBlockProperty, value); }
    }

    public static readonly DependencyProperty TextBlockProperty =
        DependencyProperty.Register("TextBlock", typeof(TextBlock), typeof(TextBlockStrokeView), new PropertyMetadata(null, (s, a) =>
        {
            if (s is TextBlockStrokeView sender && !Equals(a.NewValue, a.OldValue))
            {
                sender.DisconnectStrokeVisual();
                sender.ConnectStrokeVisual();
            }
        }));

    private void TextBlockBorder_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateBrushHostSize(e.NewSize.Width + TextBlockStrokeHelper.padding * 2, e.NewSize.Height + TextBlockStrokeHelper.padding * 2);
    }

    private void UpdateBrushHostSize(double width, double height)
    {
        brushHost.Width = width;
        brushHost.Height = height;
    }

    private void ConnectStrokeVisual()
    {
        if (TextBlockBorder == null || TextBlock == null) return;

        if (textBlockStroke == null)
        {
            textBlockStroke = new TextBlockStrokeHelper(TextBlock);
        }

        textBlockStroke.Optimization = Optimization;
        textBlockStroke.StrokeBrush = actualStrokeBrush;
        textBlockStroke.StrokeThickness = (float)StrokeThickness;

        ElementCompositionPreview.SetElementChildVisual(TextBlockBorder, textBlockStroke.StrokeVisual);
    }

    private void DisconnectStrokeVisual()
    {
        if (TextBlockBorder == null) return;

        ElementCompositionPreview.SetElementChildVisual(TextBlockBorder, null);

        if (textBlockStroke != null)
        {
            textBlockStroke.StrokeBrush = null;
            textBlockStroke.Dispose();
            textBlockStroke = null;
        }
    }
}
