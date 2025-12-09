using Windows.Graphics.Effects;

namespace DevWinUI;

internal abstract class LinearGradientBlurHelperBase<TVisual, TCompositionPropertySet> : IDisposable
{
    private readonly ICompositor compositor;
    private bool disposedValue;

    private float minBlurAmount = 0.5f;
    private float maxBlurAmount = 64f;
    private Vector2 startPoint = new Vector2(0, 0);
    private Vector2 endPoint = new Vector2(0, 1);
    private Color tintColor;

    private ISpriteVisual[] visuals;
    private ICompositionLinearGradientBrush[] maskBrushes;
    private ICompositionLinearGradientBrush tintColorBrush;
    private ICompositionColorGradientStop[] tintColorBrushStops;
    private ColorStop[][] colorStops;
    private ISpriteVisual tintColorVisual;
    private ISpriteVisual rootVisual;
    private ICompositionPropertySet propSet;

    private IExpressionAnimation blurAmountBind;

    private IExpressionAnimation startPointBind;
    private IExpressionAnimation endPointBind;

    private IExpressionAnimation tintColorBrushStop1Bind;
    private IExpressionAnimation tintColorBrushStop2Bind;

    public LinearGradientBlurHelperBase(ICompositor compositor)
    {
        this.compositor = compositor;

        tintColor = Color.FromArgb(0, 0, 0, 0);

        var dColor = Color.FromArgb(255, 0, 0, 0);
        var hColor = Color.FromArgb(0, 0, 0, 0);

        colorStops = new[]
        {
#pragma warning disable IDE0055
                new ColorStop[]{ (0f,     dColor), (0.125f, hColor) },
                new ColorStop[]{ (0f,      dColor), (0.125f, dColor), (0.25f,  hColor) },
                new ColorStop[]{ (0f,      hColor), (0.125f,  dColor), (0.25f,  dColor), (0.375f, hColor) },
                new ColorStop[]{ (0.125f,  hColor), (0.25f,   dColor), (0.375f, dColor), (0.5f,   hColor) },
                new ColorStop[]{ (0.25f,   hColor), (0.375f,  dColor), (0.5f,   dColor), (0.625f, hColor) },
                new ColorStop[]{ (0.375f,  hColor), (0.5f,    dColor), (0.625f, dColor), (0.75f,  hColor) },
                new ColorStop[]{ (0.5f,    hColor), (0.625f,  dColor), (0.75f,  dColor), (0.875f, hColor) },
                new ColorStop[]{ (0.625f,  hColor), (0.75f,   dColor), (0.875f, dColor), (1,      hColor) },
                new ColorStop[]{ (0.75f,   hColor), (0.875f,  dColor), (1,      dColor), (1.125f, hColor) },
                new ColorStop[]{ (0.875f,  hColor), (1,       dColor), (1.125f, dColor), (1.25f,  hColor) },
#pragma warning restore IDE0055
            };
        visuals = new ISpriteVisual[colorStops.Length];
        maskBrushes = new ICompositionLinearGradientBrush[colorStops.Length];

        propSet = compositor.CreatePropertySet();
        propSet.InsertScalar("MinBlurAmount", minBlurAmount);
        propSet.InsertScalar("MaxBlurAmount", maxBlurAmount);
        propSet.InsertVector2("StartPoint", startPoint);
        propSet.InsertVector2("EndPoint", endPoint);
        propSet.InsertVector4("TintColor", new Vector4(tintColor.A, tintColor.R, tintColor.G, tintColor.B));

        rootVisual = compositor.CreateSpriteVisual();
        rootVisual.RelativeSizeAdjustment = Vector2.One;

        blurAmountBind = CreateBindExpression("Clamp((propSet.MaxBlurAmount - propSet.MinBlurAmount) / Pow(2, this.Target._index) + propSet.MinBlurAmount, 0, 250)");

        startPointBind = CreateBindExpression("propSet.StartPoint");
        endPointBind = CreateBindExpression("propSet.EndPoint");

        tintColorBrushStop1Bind = CreateBindExpression("ColorRgb(propSet.TintColor.X, propSet.TintColor.Y, propSet.TintColor.Z, propSet.tintColor.W)");
        tintColorBrushStop2Bind = CreateBindExpression("ColorRgb(0f, propSet.TintColor.Y, propSet.TintColor.Z, propSet.TintColor.W)");

        for (int i = 0; i < visuals.Length; i++)
        {
            maskBrushes[i] = CreateMaskBrush(colorStops[i]);
            visuals[i] = CreateBlurVisual(maskBrushes[i]);
            visuals[i].Brush!.Properties.InsertScalar("_index", i);
            visuals[i].Brush!.Properties.StartAnimation("BlurEffect.BlurAmount", blurAmountBind);
            rootVisual.Children.InsertAtTop(visuals[i]);
        }

        tintColorBrushStops = new ICompositionColorGradientStop[2];
        tintColorBrush = compositor.CreateLinearGradientBrush();
        tintColorBrush.ColorStops.Add(tintColorBrushStops[0] = compositor.CreateColorGradientStop(0, tintColor));
        tintColorBrush.ColorStops.Add(tintColorBrushStops[1] = compositor.CreateColorGradientStop(1, MakeTransparent(tintColor)));

        tintColorBrush.StartAnimation("StartPoint", startPointBind);
        tintColorBrush.StartAnimation("EndPoint", endPointBind);
        tintColorBrushStops[0].StartAnimation("Color", tintColorBrushStop1Bind);
        tintColorBrushStops[1].StartAnimation("Color", tintColorBrushStop2Bind);

        tintColorVisual = compositor.CreateSpriteVisual();
        tintColorVisual.RelativeSizeAdjustment = Vector2.One;
        tintColorVisual.Brush = tintColorBrush;

        rootVisual.Children.InsertAtTop(tintColorVisual);
    }

    public TVisual RootVisual
    {
        get
        {
            ThrowIfDisposed();
            return (TVisual)rootVisual.RawObject;
        }
    }

    public TCompositionPropertySet CompositionProperties
    {
        get
        {
            ThrowIfDisposed();
            return (TCompositionPropertySet)propSet.RawObject;
        }
    }

    public float MinBlurAmount
    {
        get => minBlurAmount;
        set
        {
            ThrowIfDisposed();

            if (minBlurAmount != value)
            {
                minBlurAmount = value;

                propSet.InsertScalar("MinBlurAmount", minBlurAmount);
            }
        }
    }

    public float MaxBlurAmount
    {
        get => maxBlurAmount;
        set
        {
            ThrowIfDisposed();

            if (maxBlurAmount != value)
            {
                maxBlurAmount = value;

                propSet.InsertScalar("MaxBlurAmount", maxBlurAmount);
            }
        }
    }

    public Color TintColor
    {
        get => tintColor;
        set
        {
            ThrowIfDisposed();

            if (tintColor != value)
            {
                tintColor = value;

                propSet.InsertVector4("TintColor", new Vector4(tintColor.A, tintColor.R, tintColor.G, tintColor.B));
            }
        }
    }

    public Vector2 StartPoint
    {
        get => startPoint;
        set
        {
            ThrowIfDisposed();

            if (startPoint != value)
            {
                startPoint = value;

                propSet.InsertVector2("StartPoint", startPoint);
            }
        }
    }

    public Vector2 EndPoint
    {
        get => endPoint;
        set
        {
            ThrowIfDisposed();

            if (endPoint != value)
            {
                endPoint = value;

                propSet.InsertVector2("EndPoint", endPoint);
            }
        }
    }

    private ICompositionLinearGradientBrush CreateMaskBrush(ColorStop[] stops)
    {
        var maskBrush = compositor.CreateLinearGradientBrush();

        maskBrush.MappingMode = CompositionMappingModeEx.Relative;

        maskBrush.StartAnimation("StartPoint", startPointBind);
        maskBrush.StartAnimation("EndPoint", endPointBind);

        for (int i = 0; i < stops.Length; i++)
        {
            maskBrush.ColorStops.Add(compositor.CreateColorGradientStop(stops[i].Offset, stops[i].Color));
        }

        return maskBrush;
    }

    private ISpriteVisual CreateBlurVisual(ICompositionLinearGradientBrush maskBrush)
    {
        var effect = new AlphaMaskEffect()
        {
            AlphaMask = CreateEffectSourceParameter("mask"),
            Source = new GaussianBlurEffect()
            {
                Name = "BlurEffect",
                BlurAmount = 0f,
                BorderMode = EffectBorderMode.Soft,
                Source = CreateEffectSourceParameter("source")
            }
        };

        var brush = compositor.CreateEffectBrush(effect, new string[] { "BlurEffect.BlurAmount" });

        brush.SetSourceParameter("source", compositor.CreateBackdropBrush());
        brush.SetSourceParameter("mask", maskBrush);

        var visual = compositor.CreateSpriteVisual();
        visual.RelativeSizeAdjustment = Vector2.One;
        visual.Brush = brush;
        return visual;
    }

    private IExpressionAnimation CreateBindExpression(string expression)
    {
        var exp = compositor.CreateExpressionAnimation(expression);
        exp.SetReferenceParameter("propSet", propSet);
        return exp;
    }

    protected abstract IGraphicsEffectSource CreateEffectSourceParameter(string name);

    protected void ThrowIfDisposed()
    {
        if (disposedValue) throw new ObjectDisposedException(GetType().Name);
    }

    public void Dispose()
    {
        if (!disposedValue)
        {
            disposedValue = true;

            GC.SuppressFinalize(this);

            rootVisual.Children.RemoveAll();

            for (int i = 0; i < visuals.Length; i++)
            {
                var brush = visuals[i].Brush;
                visuals[i].Brush = null;
                brush?.Dispose();
                visuals[i].Dispose();
                visuals[i] = null!;
            }

            tintColorVisual.Brush = null;
            tintColorVisual.Dispose();
            tintColorVisual = null!;

            tintColorBrush.Dispose();
            tintColorBrush = null!;

            blurAmountBind.Dispose();
            blurAmountBind = null!;
            startPointBind.Dispose();
            startPointBind = null!;
            endPointBind.Dispose();
            endPointBind = null!;
            tintColorBrushStop1Bind.Dispose();
            tintColorBrushStop1Bind = null!;
            tintColorBrushStop2Bind.Dispose();
            tintColorBrushStop2Bind = null!;

            rootVisual.Dispose();
            rootVisual = null!;

            propSet.Dispose();
            propSet = null!;
        }
    }

    private static Color MakeTransparent(Color color) => Color.FromArgb(0, color.R, color.G, color.B);

    protected struct ColorStop
    {
        public readonly float Offset;

        public readonly Color Color;

        public ColorStop(float offset, Color color)
        {
            Offset = offset;
            Color = color;
        }

        public static implicit operator ColorStop((float offset, Color color) tuple)
        {
            return new ColorStop(tuple.offset, tuple.color);
        }
    }
}
