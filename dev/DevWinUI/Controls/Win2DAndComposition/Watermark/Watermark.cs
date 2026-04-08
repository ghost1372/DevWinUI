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
    private long _fontSizePropertyToken;
    private long _fontStretchPropertyToken;
    private long _fontWeightPropertyToken;
    private long _fontStylePropertyToken;

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (Watermark)d;
        if (ctl != null && ctl._Canvas != null)
        {
            if (ctl.IsImage)
            {
                ctl._Canvas.CreateResources -= ctl.OnCreateResourcesAsync;
                ctl._Canvas.CreateResources += ctl.OnCreateResourcesAsync;
            }
            ctl._Canvas.Invalidate();
        }
    }

    public Watermark()
    {
        this.DefaultStyleKey = typeof(Watermark);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _Canvas = GetTemplateChild(PART_Canvas) as CanvasControl;

        if (_Canvas != null)
        {
            _Canvas.CreateResources -= OnCreateResourcesAsync;
            _Canvas.CreateResources += OnCreateResourcesAsync;

            _Canvas.Draw -= OnDraw;
            _Canvas.Draw += OnDraw;

            _Canvas.SizeChanged -= OnCanvasSizeChanged;
            _Canvas.SizeChanged += OnCanvasSizeChanged;
            _Canvas.Invalidate();
        }

        UnRegisterChangedCallback();
        RegisterChangedCallback();
    }
    private void UnRegisterChangedCallback()
    {
        UnregisterPropertyChangedCallback(ForegroundProperty, _foregroundPropertyToken);
        UnregisterPropertyChangedCallback(FontSizeProperty, _fontSizePropertyToken);
        UnregisterPropertyChangedCallback(FontStretchProperty, _fontStretchPropertyToken);
        UnregisterPropertyChangedCallback(FontWeightProperty, _fontWeightPropertyToken);
        UnregisterPropertyChangedCallback(FontStyleProperty, _fontStylePropertyToken);
    }
    private void RegisterChangedCallback()
    {
        _foregroundPropertyToken = RegisterPropertyChangedCallback(ForegroundProperty, OnSystemPropertyChanged);
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
        if (IsImage)
        {
            if ((RuntimeHelper.IsPackaged() && !uri.IsFile) || uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            {
                _canvasBitmap = await CanvasBitmap.LoadAsync(_Canvas, uri);
            }
            else
            {
                using var fs = File.OpenRead(uri.LocalPath);
                _canvasBitmap = await CanvasBitmap.LoadAsync(_Canvas, fs.AsRandomAccessStream());
            }
            _Canvas.Invalidate();
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
                    return;
                }

                if ((RuntimeHelper.IsPackaged() && !MarkImage.IsFile) || MarkImage.Scheme == Uri.UriSchemeHttp || MarkImage.Scheme == Uri.UriSchemeHttps)
                {
                    _canvasBitmap = await CanvasBitmap.LoadAsync(_Canvas, MarkImage);
                }
                else
                {
                    using var fs = File.OpenRead(MarkImage.OriginalString);
                    _canvasBitmap = await CanvasBitmap.LoadAsync(_Canvas, fs.AsRandomAccessStream());
                }
            }
            catch (Exception)
            {
            }
        }
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
        using var list = new CanvasCommandList(sender);
        using var session = list.CreateDrawingSession();
        using var tileEffect = new TileEffect();
        tileEffect.Source = list;

        using var blurEffect = new GaussianBlurEffect();
        blurEffect.BlurAmount = Convert.ToSingle(BlurAmount);

        using var shadowEffect = new ShadowEffect();
        shadowEffect.BlurAmount = Convert.ToSingle(BlurAmount);
        shadowEffect.ShadowColor = ShadowColor;

        using var scale = new ScaleEffect();
        scale.CacheOutput = true;
        scale.CenterPoint = new Vector2(Convert.ToSingle(CenterPointX), Convert.ToSingle(CenterPointY));

        var markScale = Convert.ToSingle(MarkScale);

        if (markScale < 0.1)
        {
            markScale = 1f;
        }

        ICanvasImage graphicsEffect = null;

        if (!IsImage)
        {
            if (string.IsNullOrEmpty(Mark))
            {
                return;
            }

            using (var textFormat = new CanvasTextFormat
            {
                FontFamily = FontFamily.Source,
                FontSize = (float)FontSize,
                WordWrapping = WordWrapping,
                FontStretch = FontStretch,
                FontStyle = FontStyle,
                FontWeight = FontWeight
            })
            using (var textLayout = new CanvasTextLayout(args.DrawingSession, Mark, textFormat, 0.0f, 0.0f))
            {
                var textBounds = textLayout.LayoutBounds;
                var color = Foreground is SolidColorBrush ? (Foreground as SolidColorBrush).Color : Colors.Black;

                session.DrawTextLayout(textLayout, new Vector2(0, 0), color);

                tileEffect.SourceRectangle = new Rect(0, 0, textBounds.Width + HorizontalSpacing, textBounds.Height + VerticalSpacing);

                graphicsEffect = tileEffect;

                if (UseScale)
                {
                    scale.Source = tileEffect;
                    scale.Scale = new Vector2(markScale, markScale);
                    graphicsEffect = scale;
                }

                switch (MarkEffect)
                {
                    case WatermarkEffect.None:
                        break;
                    case WatermarkEffect.BlurEffect:
                        blurEffect.Source = graphicsEffect;
                        graphicsEffect = blurEffect;
                        break;
                    case WatermarkEffect.ShadowEffect:
                        shadowEffect.Source = graphicsEffect;
                        graphicsEffect = shadowEffect;
                        break;
                }

                args.DrawingSession.Transform = Matrix3x2.CreateRotation(GetRadians(Angle), new Vector2(Convert.ToSingle(CenterPointX), Convert.ToSingle(CenterPointY)));
                args.DrawingSession.DrawImage(graphicsEffect, new Vector2(0, 0));
            }
        }
        else
        {
            if (_canvasBitmap != null)
            {
                graphicsEffect = _canvasBitmap;
                tileEffect.SourceRectangle = _canvasBitmap.Bounds;

                if (UseScale)
                {
                    scale.Source = _canvasBitmap;
                    scale.Scale = new Vector2(markScale, markScale);
                    graphicsEffect = scale;
                }

                switch (MarkEffect)
                {
                    case WatermarkEffect.None:
                        break;
                    case WatermarkEffect.BlurEffect:
                        blurEffect.Source = graphicsEffect;
                        graphicsEffect = blurEffect;
                        break;
                    case WatermarkEffect.ShadowEffect:
                        shadowEffect.Source = graphicsEffect;
                        graphicsEffect = shadowEffect;
                        break;
                }

                session.DrawImage(graphicsEffect);

                if (UseScale)
                {
                    tileEffect.SourceRectangle = scale.GetBounds(sender);
                }

                args.DrawingSession.Transform = Matrix3x2.CreateRotation(GetRadians(Angle), new Vector2(Convert.ToSingle(CenterPointX), Convert.ToSingle(CenterPointY)));
                args.DrawingSession.DrawImage(tileEffect);
            }
        }

        _Canvas.Invalidate();
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
