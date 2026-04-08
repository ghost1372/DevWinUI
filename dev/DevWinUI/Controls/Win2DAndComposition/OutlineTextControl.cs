namespace DevWinUI;

public partial class OutlineTextControl : Control
{
    public static readonly DependencyProperty DashStyleProperty =
        DependencyProperty.Register(nameof(DashStyle), typeof(CanvasDashStyle), typeof(OutlineTextControl), new PropertyMetadata(default(CanvasDashStyle), OnDashStyleChanged));

    public static readonly DependencyProperty FontColorProperty =
        DependencyProperty.Register(nameof(FontColor), typeof(Color), typeof(OutlineTextControl), new PropertyMetadata(Colors.Black, OnFontColorChanged));

    public static readonly DependencyProperty OutlineColorProperty =
        DependencyProperty.Register(nameof(OutlineColor), typeof(Color), typeof(OutlineTextControl), new PropertyMetadata(Colors.Black, OnOutlineColorChanged));

    public static readonly DependencyProperty ShowNonOutlineTextProperty =
        DependencyProperty.Register(nameof(ShowNonOutlineText), typeof(bool), typeof(OutlineTextControl), new PropertyMetadata(default(bool), OnShowNonOutlineTextChanged));

    public static readonly DependencyProperty StrokeWidthProperty =
        DependencyProperty.Register(nameof(StrokeWidth), typeof(double), typeof(OutlineTextControl), new PropertyMetadata(default(double), OnStrokeWidthChanged));

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(OutlineTextControl), new PropertyMetadata(default(string), OnTextChanged));

    public OutlineTextControl()
    {
        var graphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(Compositor, CanvasDevice.GetSharedDevice());
        var spriteTextVisual = Compositor.CreateSpriteVisual();

        ElementCompositionPreview.SetElementChildVisual(this, spriteTextVisual);
        SizeChanged += (s, e) =>
        {
            DrawingSurface = graphicsDevice.CreateDrawingSurface(e.NewSize, DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);
            DrawSurface();
            UpdateSpriteVisual(spriteTextVisual, DrawingSurface);
            spriteTextVisual.Size = e.NewSize.ToVector2();
        };
        RegisterPropertyChangedCallback(FontSizeProperty, new DependencyPropertyChangedCallback((s, e) =>
         {
             DrawSurface();
         }));
    }

    public CanvasDashStyle DashStyle
    {
        get => (CanvasDashStyle)GetValue(DashStyleProperty);
        set => SetValue(DashStyleProperty, value);
    }

    public Color FontColor
    {
        get => (Color)GetValue(FontColorProperty);
        set => SetValue(FontColorProperty, value);
    }

    public Color OutlineColor
    {
        get => (Color)GetValue(OutlineColorProperty);
        set => SetValue(OutlineColorProperty, value);
    }

    public bool ShowNonOutlineText
    {
        get => (bool)GetValue(ShowNonOutlineTextProperty);
        set => SetValue(ShowNonOutlineTextProperty, value);
    }

    public double StrokeWidth
    {
        get => (double)GetValue(StrokeWidthProperty);
        set => SetValue(StrokeWidthProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    protected CompositionDrawingSurface DrawingSurface { get; private set; }
    private Compositor Compositor => ElementCompositionPreview.GetElementVisual(this).Compositor;

    protected void DrawSurface()
    {
        if (ActualHeight == 0 || ActualWidth == 0 || string.IsNullOrWhiteSpace(Text) || DrawingSurface == null)
            return;

        var width = (float)ActualWidth;
        var height = (float)ActualHeight;

        DrawSurfaceCore(DrawingSurface, width, height);
    }

    protected virtual void DrawSurfaceCore(CompositionDrawingSurface drawingSurface, float width, float height)
    {
        using (var session = CanvasComposition.CreateDrawingSession(drawingSurface))
        {
            session.Clear(Colors.Transparent);
            using (var textFormat = new CanvasTextFormat()
            {
                FontSize = (float)FontSize,
                Direction = CanvasTextDirection.LeftToRightThenTopToBottom,
                VerticalAlignment = CanvasVerticalAlignment.Center,
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                FontWeight = FontWeight,
                FontFamily = FontFamily.Source
            })
            {
                using (var textLayout = new CanvasTextLayout(session, Text, textFormat, width, height))
                {
                    DrawText(session, textLayout);
                }
            }
        }
    }

    protected void DrawText(CanvasDrawingSession session, CanvasTextLayout textLayout)
    {
        if (ShowNonOutlineText)
            session.DrawTextLayout(textLayout, 0, 0, FontColor);

        using (var textGeometry = CanvasGeometry.CreateText(textLayout))
        {
            using (var dashedStroke = new CanvasStrokeStyle())
            {
                dashedStroke.DashStyle = DashStyle;
                session.DrawGeometry(textGeometry, OutlineColor, (float)StrokeWidth, dashedStroke);
            }
        }
    }

    protected virtual void OnDashStyleChanged(CanvasDashStyle oldValue, CanvasDashStyle newValue)
    {
        DrawSurface();
    }

    protected virtual void OnFontColorChanged(Color oldValue, Color newValue)
    {
        DrawSurface();
    }

    protected virtual void OnOutlineColorChanged(Color oldValue, Color newValue)
    {
        DrawSurface();
    }

    protected virtual void OnShowNonOutlineTextChanged(bool oldValue, bool newValue)
    {
        DrawSurface();
    }

    protected virtual void OnStrokeWidthChanged(double oldValue, double newValue)
    {
        DrawSurface();
    }

    protected virtual void OnTextChanged(string oldValue, string newValue)
    {
        DrawSurface();
    }

    protected virtual void UpdateSpriteVisual(SpriteVisual spriteVisual, CompositionDrawingSurface drawingSurface)
    {
        var maskSurfaceBrush = Compositor.CreateSurfaceBrush(DrawingSurface);
        spriteVisual.Brush = maskSurfaceBrush;
    }

    private static void OnDashStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var oldValue = (CanvasDashStyle)args.OldValue;
        var newValue = (CanvasDashStyle)args.NewValue;
        if (oldValue == newValue)
            return;

        var target = obj as OutlineTextControl;
        target?.OnDashStyleChanged(oldValue, newValue);
    }

    private static void OnFontColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var oldValue = (Color)args.OldValue;
        var newValue = (Color)args.NewValue;
        if (oldValue == newValue)
            return;

        var target = obj as OutlineTextControl;
        target?.OnFontColorChanged(oldValue, newValue);
    }

    private static void OnOutlineColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var oldValue = (Color)args.OldValue;
        var newValue = (Color)args.NewValue;
        if (oldValue == newValue)
            return;

        var target = obj as OutlineTextControl;
        target?.OnOutlineColorChanged(oldValue, newValue);
    }

    private static void OnShowNonOutlineTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var oldValue = (bool)args.OldValue;
        var newValue = (bool)args.NewValue;
        if (oldValue == newValue)
            return;

        var target = obj as OutlineTextControl;
        target?.OnShowNonOutlineTextChanged(oldValue, newValue);
    }

    private static void OnStrokeWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var oldValue = (double)args.OldValue;
        var newValue = (double)args.NewValue;
        if (oldValue == newValue)
            return;

        var target = obj as OutlineTextControl;
        target?.OnStrokeWidthChanged(oldValue, newValue);
    }

    private static void OnTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var oldValue = (string)args.OldValue;
        var newValue = (string)args.NewValue;
        if (oldValue == newValue)
            return;

        var target = obj as OutlineTextControl;
        target?.OnTextChanged(oldValue, newValue);
    }
}
