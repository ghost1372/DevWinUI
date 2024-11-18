// https://github.com/DinoChan

using Windows.Foundation;
using Windows.Graphics;

namespace DevWinUI;

public partial class TextToBrushWrapper : Control
{
    public static readonly DependencyProperty DashStyleProperty =
        DependencyProperty.Register(nameof(DashStyle), typeof(CanvasDashStyle), typeof(TextToBrushWrapper),
            new PropertyMetadata(default(CanvasDashStyle), OnDashStyleChanged));

    public static readonly DependencyProperty FontColorProperty =
        DependencyProperty.Register(nameof(FontColor), typeof(Color), typeof(TextToBrushWrapper),
            new PropertyMetadata(Colors.Black, OnFontColorChanged));

    public static readonly DependencyProperty OutlineColorProperty =
        DependencyProperty.Register(nameof(OutlineColor), typeof(Color), typeof(TextToBrushWrapper),
            new PropertyMetadata(Colors.Black, OnOutlineColorChanged));

    public static readonly DependencyProperty ShadowBlurAmountProperty =
        DependencyProperty.Register(nameof(ShadowBlurAmount), typeof(double), typeof(TextToBrushWrapper),
            new PropertyMetadata(10d, OnShadowBlurAmountChanged));

    public static readonly DependencyProperty ShadowColorProperty =
        DependencyProperty.Register(nameof(ShadowColor), typeof(Color), typeof(TextToBrushWrapper),
            new PropertyMetadata(Colors.Black, OnShadowColorChanged));

    public static readonly DependencyProperty ShadowOffsetXProperty =
        DependencyProperty.Register(nameof(ShadowOffsetX), typeof(double), typeof(TextToBrushWrapper),
            new PropertyMetadata(default(double), OnShadowOffsetXChanged));

    public static readonly DependencyProperty ShadowOffsetYProperty =
        DependencyProperty.Register(nameof(ShadowOffsetY), typeof(double), typeof(TextToBrushWrapper),
            new PropertyMetadata(default(double), OnShadowOffsetYChanged));

    public static readonly DependencyProperty ShowNonOutlineTextProperty =
        DependencyProperty.Register(nameof(ShowNonOutlineText), typeof(bool), typeof(TextToBrushWrapper),
            new PropertyMetadata(true, OnShowNonOutlineTextChanged));

    public static readonly DependencyProperty StrokeWidthProperty =
        DependencyProperty.Register(nameof(StrokeWidth), typeof(double), typeof(TextToBrushWrapper),
            new PropertyMetadata(default(double), OnStrokeWidthChanged));

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextToBrushWrapper),
            new PropertyMetadata(default(string), OnTextChanged));

    private readonly CompositionGraphicsDevice _graphicsDevice;
    private SpriteVisual _spriteTextVisual;

    public TextToBrushWrapper()
    {
        _graphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(Compositor, CanvasDevice.GetSharedDevice());
        _spriteTextVisual = Compositor.CreateSpriteVisual();
        DrawingSurface = _graphicsDevice.CreateDrawingSurface(new Size(10, 10),
            DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);
        Brush = Compositor.CreateSurfaceBrush(DrawingSurface);
        RegisterPropertyChangedCallback(FontSizeProperty, (s, e) => { DrawSurface(); });
        RegisterPropertyChangedCallback(BackgroundProperty, (s, e) => { DrawSurface(); });
        RegisterPropertyChangedCallback(HeightProperty, (s, e) =>
        {
            if (double.IsNaN(Height) || double.IsNaN(Width) || Width == 0 || Height == 0)
                return;

            DrawingSurface.Resize(new SizeInt32 { Width = (int)Width, Height = (int)Height });
            DrawSurface();
        });

        RegisterPropertyChangedCallback(WidthProperty, (s, e) =>
        {
            if (double.IsNaN(Height) || double.IsNaN(Width) || Width == 0 || Height == 0)
                return;

            DrawingSurface.Resize(new SizeInt32 { Width = (int)Width, Height = (int)Height });
            DrawSurface();
        });
    }

    public CompositionSurfaceBrush Brush { get; }

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

    public double ShadowBlurAmount
    {
        get => (double)GetValue(ShadowBlurAmountProperty);
        set => SetValue(ShadowBlurAmountProperty, value);
    }

    public Color ShadowColor
    {
        get => (Color)GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }

    public double ShadowOffsetX
    {
        get => (double)GetValue(ShadowOffsetXProperty);
        set => SetValue(ShadowOffsetXProperty, value);
    }

    public double ShadowOffsetY
    {
        get => (double)GetValue(ShadowOffsetYProperty);
        set => SetValue(ShadowOffsetYProperty, value);
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

    protected CompositionDrawingSurface DrawingSurface { get; }
    private Compositor Compositor => ElementCompositionPreview.GetElementVisual(this).Compositor;

    protected void DrawSurface()
    {
        if (double.IsNaN(Height) || double.IsNaN(Width) || Width == 0 || Height == 0 ||
            string.IsNullOrWhiteSpace(Text) || DrawingSurface == null)
            return;

        var width = (float)Width;
        var height = (float)Height;

        DrawSurfaceCore(DrawingSurface, width, height);
    }

    protected virtual void DrawSurfaceCore(CompositionDrawingSurface drawingSurface, float width, float height)
    {
        using (var session = CanvasComposition.CreateDrawingSession(drawingSurface))
        {
            session.Clear(Colors.Transparent);
            if (Background is SolidColorBrush solidColorBrush)
                session.FillRectangle(new Rect(0, 0, width, height), solidColorBrush.Color);

            using (var textFormat = new CanvasTextFormat
                   {
                       FontSize = (float)FontSize,
                       Direction = CanvasTextDirection.LeftToRightThenTopToBottom,
                       VerticalAlignment = CanvasVerticalAlignment.Top,
                       HorizontalAlignment = CanvasHorizontalAlignment.Center,
                       FontWeight = FontWeight,
                       FontFamily = FontFamily.Source
                   })
            {
                using (var textLayout = new CanvasTextLayout(session, Text, textFormat, width, height))
                {
                    var fullSizeGeometry = CanvasGeometry.CreateRectangle(session, 0, 0, width, height);
                    var textGeometry = CanvasGeometry.CreateText(textLayout);
                    var finalGeometry = fullSizeGeometry.CombineWith(textGeometry, Matrix3x2.Identity,
                        CanvasGeometryCombine.Exclude);
                    using (var layer = session.CreateLayer(1, fullSizeGeometry))
                    {
                        using (var bitmap = new CanvasRenderTarget(session, width, height))
                        {
                            using (var bitmapSession = bitmap.CreateDrawingSession())
                            {
                                DrawText(bitmapSession, textLayout, ShadowColor);
                            }

                            using (var blur = new GaussianBlurEffect
                                   {
                                       BlurAmount = (float)ShadowBlurAmount,
                                       Source = bitmap,
                                       BorderMode = EffectBorderMode.Hard
                                   })
                            {
                                session.DrawImage(blur, (float)ShadowOffsetX, (float)ShadowOffsetY);
                            }
                        }
                    }

                    DrawText(session, textLayout, FontColor);
                }
            }
        }
    }

    protected void DrawText(CanvasDrawingSession session, CanvasTextLayout textLayout, Color color)
    {
        if (ShowNonOutlineText)
            session.DrawTextLayout(textLayout, 0, 0, color);

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

    protected virtual void OnShadowBlurAmountChanged(double oldValue, double newValue)
    {
        DrawSurface();
    }

    protected virtual void OnShadowColorChanged(Color oldValue, Color newValue)
    {
        DrawSurface();
    }

    protected virtual void OnShadowOffsetXChanged(double oldValue, double newValue)
    {
        DrawSurface();
    }

    protected virtual void OnShadowOffsetYChanged(double oldValue, double newValue)
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

    private static void OnDashStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var oldValue = (CanvasDashStyle)args.OldValue;
        var newValue = (CanvasDashStyle)args.NewValue;
        if (oldValue == newValue)
            return;

        var target = obj as TextToBrushWrapper;
        target?.OnDashStyleChanged(oldValue, newValue);
    }

    private static void OnFontColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var oldValue = (Color)args.OldValue;
        var newValue = (Color)args.NewValue;
        if (oldValue == newValue)
            return;

        var target = obj as TextToBrushWrapper;
        target?.OnFontColorChanged(oldValue, newValue);
    }

    private static void OnOutlineColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var oldValue = (Color)args.OldValue;
        var newValue = (Color)args.NewValue;
        if (oldValue == newValue)
            return;

        var target = obj as TextToBrushWrapper;
        target?.OnOutlineColorChanged(oldValue, newValue);
    }

    private static void OnShadowBlurAmountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var oldValue = (double)args.OldValue;
        var newValue = (double)args.NewValue;
        if (oldValue == newValue)
            return;

        var target = obj as TextToBrushWrapper;
        target?.OnShadowBlurAmountChanged(oldValue, newValue);
    }

    private static void OnShadowColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var oldValue = (Color)args.OldValue;
        var newValue = (Color)args.NewValue;
        if (oldValue == newValue)
            return;

        var target = obj as TextToBrushWrapper;
        target?.OnShadowColorChanged(oldValue, newValue);
    }

    private static void OnShadowOffsetXChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var oldValue = (double)args.OldValue;
        var newValue = (double)args.NewValue;
        if (oldValue == newValue)
            return;

        var target = obj as TextToBrushWrapper;
        target?.OnShadowOffsetXChanged(oldValue, newValue);
    }

    private static void OnShadowOffsetYChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var oldValue = (double)args.OldValue;
        var newValue = (double)args.NewValue;
        if (oldValue == newValue)
            return;

        var target = obj as TextToBrushWrapper;
        target?.OnShadowOffsetYChanged(oldValue, newValue);
    }

    private static void OnShowNonOutlineTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var oldValue = (bool)args.OldValue;
        var newValue = (bool)args.NewValue;
        if (oldValue == newValue)
            return;

        var target = obj as TextToBrushWrapper;
        target?.OnShowNonOutlineTextChanged(oldValue, newValue);
    }

    private static void OnStrokeWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var oldValue = (double)args.OldValue;
        var newValue = (double)args.NewValue;
        if (oldValue == newValue)
            return;

        var target = obj as TextToBrushWrapper;
        target?.OnStrokeWidthChanged(oldValue, newValue);
    }

    private static void OnTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var oldValue = (string)args.OldValue;
        var newValue = (string)args.NewValue;
        if (oldValue == newValue)
            return;

        var target = obj as TextToBrushWrapper;
        target?.OnTextChanged(oldValue, newValue);
    }
}
