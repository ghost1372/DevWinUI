using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[ContentProperty(Name = nameof(Content))]
public partial class PerspectiveHost : Control
{
    private Visual? m_rootVisual;
    private ExpressionAnimation? m_matrixExpression;
    private bool m_setUpExpressions;
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(PerspectiveHost), new PropertyMetadata(null));

    public double PerspectiveDepth
    {
        get => (double)GetValue(PerspectiveDepthProperty);
        set => SetValue(PerspectiveDepthProperty, value);
    }

    public static readonly DependencyProperty PerspectiveDepthProperty =
        DependencyProperty.Register(nameof(PerspectiveDepth), typeof(double), typeof(PerspectiveHost), new PropertyMetadata(0.0, OnPerspectiveDepthChanged));

    private static void OnPerspectiveDepthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (PerspectiveHost)d;
        control.EnsureVisual().Properties.InsertScalar(nameof(PerspectiveDepth), (float)(double)e.NewValue);
        control.UpdatePerspectiveMatrix();
    }

    public Vector2 PerspectiveOriginPercent
    {
        get => (Vector2)GetValue(PerspectiveOriginPercentProperty);
        set => SetValue(PerspectiveOriginPercentProperty, value);
    }

    public static readonly DependencyProperty PerspectiveOriginPercentProperty =
        DependencyProperty.Register(nameof(PerspectiveOriginPercent), typeof(Vector2), typeof(PerspectiveHost), new PropertyMetadata(new Vector2(0.5f, 0.5f), OnPerspectiveOriginPercentChanged));

    private static void OnPerspectiveOriginPercentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (PerspectiveHost)d;
        control.EnsureVisual().Properties.InsertVector2(nameof(PerspectiveOriginPercent), (Vector2)e.NewValue);
        control.UpdatePerspectiveMatrix();
    }
    public PerspectiveHost()
    {
        DefaultStyleKey = typeof(PerspectiveHost);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        EnsureVisual();

        this.SizeChanged -= OnSizeChanged;
        this.SizeChanged += OnSizeChanged;
    }

    private Visual EnsureVisual()
    {
        return m_rootVisual ??= ElementCompositionPreview.GetElementVisual(this);
    }

    public CompositionPropertySet VisualProperties
    {
        get
        {
            var visual = EnsureVisual();
            if (!m_setUpExpressions)
            {
                m_setUpExpressions = true;
                UpdatePerspectiveMatrix();
            }
            return visual.Properties;
        }
    }

    private void OnSizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
    {
        if (m_rootVisual != null)
        {
            m_rootVisual.Size = new System.Numerics.Vector2((float)this.ActualWidth, (float)this.ActualHeight);
            if (m_matrixExpression != null)
            {
                m_matrixExpression.Properties.InsertVector3("LayoutSize", new Vector3(m_rootVisual.Size, 0));
            }
            else
            {
                UpdatePerspectiveMatrix();
            }
        }
    }

    private void UpdatePerspectiveMatrix()
    {
        var rootVisual = EnsureVisual();
        var compositor = rootVisual.Compositor;

        if (!m_setUpExpressions)
        {
            Vector3 perspectiveOrigin = new Vector3(PerspectiveOriginPercent * rootVisual.Size, 0);

            Matrix4x4 transform =
                Matrix4x4.CreateTranslation(-perspectiveOrigin) *
                new Matrix4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, -1 / (float)PerspectiveDepth, 0, 0, 0, 1) *
                Matrix4x4.CreateTranslation(perspectiveOrigin);

            rootVisual.TransformMatrix = transform;
        }
        else if (m_matrixExpression == null)
        {
            var matrixExpression = compositor.CreateExpressionAnimation();
            m_matrixExpression = matrixExpression;
            matrixExpression.Properties.InsertVector3("LayoutSize", new Vector3(rootVisual.Size, 0));

            var perspectiveOriginExpression = compositor.CreateExpressionAnimation(
                "Vector3(publicProps.PerspectiveOriginPercent.x, publicProps.PerspectiveOriginPercent.y, 0) * props.LayoutSize");
            perspectiveOriginExpression.SetReferenceParameter("publicProps", rootVisual.Properties);
            perspectiveOriginExpression.SetReferenceParameter("props", matrixExpression.Properties);
            matrixExpression.Properties.InsertVector3("PerspectiveOrigin", Vector3.Zero);
            matrixExpression.Properties.StartAnimation("PerspectiveOrigin", perspectiveOriginExpression);

            matrixExpression.Expression =
                "Matrix4x4.CreateFromTranslation(-props.PerspectiveOrigin) * " +
                "Matrix4x4(1,0,0,0,  0,1,0,0,  0,0,1,-1/publicProps.PerspectiveDepth,  0,0,0,1) * " +
                "Matrix4x4.CreateFromTranslation( props.PerspectiveOrigin)";
            matrixExpression.SetReferenceParameter("publicProps", rootVisual.Properties);
            matrixExpression.SetReferenceParameter("props", matrixExpression.Properties);

            rootVisual.StartAnimation("TransformMatrix", matrixExpression);
        }
    }
}
