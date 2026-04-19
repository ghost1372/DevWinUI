using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[ContentProperty(Name = nameof(Content))]
public partial class PerspectiveHost : Control
{
    private Compositor m_compositor;
    private Visual m_rootVisual;
    private ExpressionAnimation m_matrixExpression;
    private bool m_setUpExpressions;
    public object Content
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
        control.m_rootVisual?.Properties.InsertScalar(nameof(PerspectiveDepth), (float)(double)e.NewValue);
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
        control.m_rootVisual?.Properties.InsertVector2(nameof(PerspectiveOriginPercent), (Vector2)e.NewValue);
        control.UpdatePerspectiveMatrix();
    }
    public PerspectiveHost()
    {
        DefaultStyleKey = typeof(PerspectiveHost);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        m_rootVisual = ElementCompositionPreview.GetElementVisual(this);
        m_compositor = m_rootVisual.Compositor;

        this.SizeChanged -= OnSizeChanged;
        this.SizeChanged += OnSizeChanged;
    }

    public CompositionPropertySet VisualProperties
    {
        get
        {
            if (!m_setUpExpressions)
            {
                m_setUpExpressions = true;
                UpdatePerspectiveMatrix();
            }
            return m_rootVisual.Properties;
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
        if (!m_setUpExpressions)
        {
            Vector3 perspectiveOrigin = new Vector3(PerspectiveOriginPercent * m_rootVisual.Size, 0);

            Matrix4x4 transform =
                Matrix4x4.CreateTranslation(-perspectiveOrigin) *
                new Matrix4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, -1 / (float)PerspectiveDepth, 0, 0, 0, 1) *
                Matrix4x4.CreateTranslation(perspectiveOrigin);

            m_rootVisual.TransformMatrix = transform;
        }
        else if (m_matrixExpression == null)
        {
            m_matrixExpression = m_compositor.CreateExpressionAnimation();
            m_matrixExpression.Properties.InsertVector3("LayoutSize", new Vector3(m_rootVisual.Size, 0));

            // Expressions don't have an easy way to convert vector2 to vector3. But having this intermediate expression makes the below expression cleaner anyway.
            var perspectiveOriginExpression = m_compositor.CreateExpressionAnimation(
                "Vector3(publicProps.PerspectiveOriginPercent.x, publicProps.PerspectiveOriginPercent.y, 0) * props.LayoutSize");
            perspectiveOriginExpression.SetReferenceParameter("publicProps", m_rootVisual.Properties);
            perspectiveOriginExpression.SetReferenceParameter("props", m_matrixExpression.Properties);
            m_matrixExpression.Properties.InsertVector3("PerspectiveOrigin", Vector3.Zero);
            m_matrixExpression.Properties.StartAnimation("PerspectiveOrigin", perspectiveOriginExpression);

            m_matrixExpression.Expression =
                "Matrix4x4.CreateFromTranslation(-props.PerspectiveOrigin) * " +
                "Matrix4x4(1,0,0,0,  0,1,0,0,  0,0,1,-1/publicProps.PerspectiveDepth,  0,0,0,1) * " +
                "Matrix4x4.CreateFromTranslation( props.PerspectiveOrigin)";
            m_matrixExpression.SetReferenceParameter("publicProps", m_rootVisual.Properties);
            m_matrixExpression.SetReferenceParameter("props", m_matrixExpression.Properties);

            m_rootVisual.StartAnimation("TransformMatrix", m_matrixExpression);
        }
    }
}
