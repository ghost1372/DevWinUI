using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[TemplatePart(Name = PART_Canvas, Type = typeof(CanvasControl))]
[ContentProperty(Name = nameof(Mark))]
public partial class Watermark : Control
{
    private const string PART_Canvas = "PART_Canvas";
    private CanvasControl _Canvas;
    private CanvasBitmap _canvasBitmap;

    private long _foregroundPropertyToken;
    private long _fontFamilyPropertyToken;
    private long _fontSizePropertyToken;
    private long _fontStretchPropertyToken;
    private long _fontWeightPropertyToken;
    private long _fontStylePropertyToken;

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (Watermark)d;
        if (ctl != null && ctl._Canvas != null)
        {
            ctl._Canvas.Invalidate();
        }
    }

    private static void OnMarkImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (Watermark)d;
        if (ctl != null && ctl._Canvas != null)
        {
            if (ctl.IsImage)
            {
                _ = ctl.LoadMarkImage();
            }
            else
            {
                ctl._Canvas.Invalidate();
            }
        }
    }

    public Watermark()
    {
        this.DefaultStyleKey = typeof(Watermark);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_Canvas != null)
        {
            _Canvas.CreateResources -= OnCreateResourcesAsync;
            _Canvas.Draw -= OnDraw;
            _Canvas.SizeChanged -= OnCanvasSizeChanged;
        }

        _Canvas = GetTemplateChild(PART_Canvas) as CanvasControl;

        if (_Canvas != null)
        {
            _Canvas.CreateResources += OnCreateResourcesAsync;
            _Canvas.Draw += OnDraw;
            _Canvas.SizeChanged += OnCanvasSizeChanged;
            _Canvas.Invalidate();
        }

        UnRegisterChangedCallback();
        RegisterChangedCallback();
    }
    private void UnRegisterChangedCallback()
    {
        UnregisterPropertyChangedCallback(ForegroundProperty, _foregroundPropertyToken);
        UnregisterPropertyChangedCallback(FontFamilyProperty, _fontFamilyPropertyToken);
        UnregisterPropertyChangedCallback(FontSizeProperty, _fontSizePropertyToken);
        UnregisterPropertyChangedCallback(FontStretchProperty, _fontStretchPropertyToken);
        UnregisterPropertyChangedCallback(FontWeightProperty, _fontWeightPropertyToken);
        UnregisterPropertyChangedCallback(FontStyleProperty, _fontStylePropertyToken);
    }
    private void RegisterChangedCallback()
    {
        _foregroundPropertyToken = RegisterPropertyChangedCallback(ForegroundProperty, OnSystemPropertyChanged);
        _fontFamilyPropertyToken = RegisterPropertyChangedCallback(FontFamilyProperty, OnSystemPropertyChanged);
        _fontSizePropertyToken = RegisterPropertyChangedCallback(FontSizeProperty, OnSystemPropertyChanged);
        _fontStretchPropertyToken = RegisterPropertyChangedCallback(FontStretchProperty, OnSystemPropertyChanged);
        _fontWeightPropertyToken = RegisterPropertyChangedCallback(FontWeightProperty, OnSystemPropertyChanged);
        _fontStylePropertyToken = RegisterPropertyChangedCallback(FontStyleProperty, OnSystemPropertyChanged);
    }

    private async void OnCreateResourcesAsync(CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
    {
        await LoadMarkImage();
    }

    public async Task LoadMarkImage(Uri uri)
    {
        if (IsImage && _Canvas != null)
        {
            await LoadBitmap(uri);
        }
    }
    private async Task LoadMarkImage()
    {
        if (IsImage && _Canvas != null)
        {
            try
            {
                if (MarkImage == null)
                {
                    _canvasBitmap?.Dispose();
                    _canvasBitmap = null;
                    _Canvas.Invalidate();
                    return;
                }

                await LoadBitmap(MarkImage);
            }
            catch (Exception)
            {
            }
        }
    }

    private async Task LoadBitmap(Uri uri)
    {
        CanvasBitmap bitmap;

        if ((RuntimeHelper.IsPackaged() && !uri.IsFile) || uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
        {
            bitmap = await CanvasBitmap.LoadAsync(_Canvas, uri);
        }
        else
        {
            using var fs = File.OpenRead(uri.LocalPath);
            bitmap = await CanvasBitmap.LoadAsync(_Canvas, fs.AsRandomAccessStream());
        }

        _canvasBitmap?.Dispose();
        _canvasBitmap = bitmap;
        _Canvas.Invalidate();
    }

    private void OnSystemPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        ReDraw();
    }

    private void OnCanvasSizeChanged(object sender, SizeChangedEventArgs e)
    {
        ReDraw();
    }

    private void ReDraw()
    {
        if (_Canvas != null)
        {
            _Canvas.Invalidate();
        }
    }

    private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        if (IsImage ? _canvasBitmap == null : string.IsNullOrEmpty(Mark))
        {
            return;
        }

        var centerPoint = new Vector2((float)CenterPointX, (float)CenterPointY);
        var markScale = (float)MarkScale;

        if (markScale < 0.1)
        {
            markScale = 1f;
        }

        using var list = new CanvasCommandList(sender);
        using var session = list.CreateDrawingSession();
        using var tileEffect = new TileEffect();
        tileEffect.Source = list;

        using var scale = UseScale ? new ScaleEffect
        {
            CacheOutput = true,
            CenterPoint = centerPoint,
            Scale = new Vector2(markScale, markScale)
        } : null;

        if (!IsImage)
        {
            using var textFormat = new CanvasTextFormat
            {
                FontFamily = FontFamily.Source,
                FontSize = (float)FontSize,
                WordWrapping = WordWrapping,
                FontStretch = FontStretch,
                FontStyle = FontStyle,
                FontWeight = FontWeight
            };
            using var textLayout = new CanvasTextLayout(args.DrawingSession, Mark, textFormat, 0.0f, 0.0f);

            var textBounds = textLayout.LayoutBounds;
            var color = Foreground is SolidColorBrush brush ? brush.Color : Colors.Black;

            session.DrawTextLayout(textLayout, Vector2.Zero, color);

            tileEffect.SourceRectangle = new Rect(0, 0, textBounds.Width + HorizontalSpacing, textBounds.Height + VerticalSpacing);

            ICanvasImage graphicsEffect = tileEffect;

            if (scale != null)
            {
                scale.Source = tileEffect;
                graphicsEffect = scale;
            }

            using var markEffect = CreateMarkEffect(graphicsEffect);

            args.DrawingSession.Transform = Matrix3x2.CreateRotation(GetRadians(Angle), centerPoint);
            args.DrawingSession.DrawImage(markEffect ?? graphicsEffect, Vector2.Zero);
        }
        else
        {
            ICanvasImage graphicsEffect = _canvasBitmap;
            tileEffect.SourceRectangle = _canvasBitmap.Bounds;

            if (scale != null)
            {
                scale.Source = _canvasBitmap;
                graphicsEffect = scale;
            }

            using var markEffect = CreateMarkEffect(graphicsEffect);

            session.DrawImage(markEffect ?? graphicsEffect);

            if (scale != null)
            {
                tileEffect.SourceRectangle = scale.GetBounds(sender);
            }

            args.DrawingSession.Transform = Matrix3x2.CreateRotation(GetRadians(Angle), centerPoint);
            args.DrawingSession.DrawImage(tileEffect);
        }
    }

    private ICanvasImage CreateMarkEffect(ICanvasImage source)
    {
        switch (MarkEffect)
        {
            case WatermarkEffect.BlurEffect:
                return new GaussianBlurEffect
                {
                    Source = source,
                    BlurAmount = (float)BlurAmount
                };
            case WatermarkEffect.ShadowEffect:
                return new ShadowEffect
                {
                    Source = source,
                    BlurAmount = (float)BlurAmount,
                    ShadowColor = ShadowColor
                };
            default:
                return null;
        }
    }
    private float GetRadians(double angle)
    {
        return (float)(Math.PI * angle / 180.0);
    }
    public CanvasControl GetCanvas()
    {
        return _Canvas;
    }
}
