namespace DevWinUI;

internal partial class TextBlockStrokeHelper : IDisposable
{
    internal const float padding = 10;

    private bool disposedValue;

    private DisposableCollector disposableCollector;
    private WeakReference<TextBlock> weakTextBlock;
    private float strokeThickness = 0f;
    private TextBlockStrokeOptimization optimization = TextBlockStrokeOptimization.Balanced;

    private Compositor compositor;
    private ContainerVisual strokeVisualForSurface;
    private CompositionVisualSurface strokeVisualSurface;
    private CompositionSurfaceBrush strokeVisualSurfaceBrush;
    private CompositionEffectBrush strokeEffectBrush;

    private CompositionSurfaceBrush textBlockAlphaMask;
    private SpriteVisual alphaMaskSurfaceVisual;
    private CompositionVisualSurface alphaMaskSurface;
    private CompositionSurfaceBrush alphaMaskSurfaceBrush;

    private ContainerVisual rootVisual;
    private SpriteVisual strokeBrushVisual;
    private ExpressionAnimation sizeBind;
    private ExpressionAnimation surfaceSizeBind;

    private CompositionBrush? strokeFillBrush;

    public TextBlockStrokeHelper(TextBlock textBlock)
    {
        disposableCollector = new DisposableCollector();

        weakTextBlock = new WeakReference<TextBlock>(textBlock);

        textBlock.Loaded += new WeakEventListener<TextBlock, object, RoutedEventArgs>(textBlock)
        {
            OnEventAction = (i, s, a) => UpdateStrokeState(),
            OnDetachAction = i => textBlock.Loaded -= i.OnEvent
        }.OnEvent;

        textBlock.Unloaded += new WeakEventListener<TextBlock, object, RoutedEventArgs>(textBlock)
        {
            OnEventAction = (i, s, a) => UpdateStrokeState(),
            OnDetachAction = i => textBlock.Unloaded -= i.OnEvent
        }.OnEvent;

        textBlock.SizeChanged += new WeakEventListener<TextBlock, object, SizeChangedEventArgs>(textBlock)
        {
            OnEventAction = (i, s, a) => UpdateStrokeState(),
            OnDetachAction = i => textBlock.SizeChanged -= i.OnEvent
        }.OnEvent;

        var textBlockVisual = textBlock.GetVisualInternal();
        compositor = textBlockVisual.Compositor;

        sizeBind = compositor.CreateExpressionAnimation("visual.Size")
            .TraceDisposable(disposableCollector);
        sizeBind.SetReferenceParameter("visual", textBlockVisual);

        surfaceSizeBind = compositor.CreateExpressionAnimation($"Vector2(visual.Size.X + {padding * 2}, visual.Size.Y + {padding * 2})")
            .TraceDisposable(disposableCollector);
        surfaceSizeBind.SetReferenceParameter("visual", textBlockVisual);

        strokeBrushVisual = compositor.CreateSpriteVisual()
            .TraceDisposable(disposableCollector);
        strokeBrushVisual.Offset = new Vector3(-padding, -padding, 0);
        strokeBrushVisual.StartAnimation("Size", surfaceSizeBind);

        rootVisual = compositor.CreateContainerVisual()
            .TraceDisposable(disposableCollector);
        rootVisual.StartAnimation("Size", sizeBind);

        textBlockAlphaMask = (CompositionSurfaceBrush)textBlock.GetAlphaMask();

        alphaMaskSurfaceVisual = compositor.CreateSpriteVisual()
            .TraceDisposable(disposableCollector);
        alphaMaskSurfaceVisual.Brush = textBlockAlphaMask;
        alphaMaskSurfaceVisual.StartAnimation("Size", sizeBind);

        alphaMaskSurface = compositor.CreateVisualSurface()
            .TraceDisposable(disposableCollector);
        alphaMaskSurface.SourceVisual = alphaMaskSurfaceVisual;
        alphaMaskSurface.StartAnimation("SourceSize", sizeBind);

        alphaMaskSurfaceBrush = compositor.CreateSurfaceBrush(alphaMaskSurface)
            .TraceDisposable(disposableCollector);
        alphaMaskSurfaceBrush.HorizontalAlignmentRatio = 0;
        alphaMaskSurfaceBrush.VerticalAlignmentRatio = 0;
        alphaMaskSurfaceBrush.Stretch = CompositionStretch.None;

        strokeVisualForSurface = compositor.CreateContainerVisual()
            .TraceDisposable(disposableCollector);
        strokeVisualForSurface.StartAnimation("Size", sizeBind);

        strokeVisualSurface = compositor.CreateVisualSurface()
            .TraceDisposable(disposableCollector);
        strokeVisualSurface.SourceVisual = strokeVisualForSurface;
        strokeVisualSurface.StartAnimation("SourceSize", surfaceSizeBind);
        strokeVisualSurface.SourceOffset = new Vector2(-padding, -padding);

        strokeVisualSurfaceBrush = compositor.CreateSurfaceBrush(strokeVisualSurface)
            .TraceDisposable(disposableCollector);
        strokeVisualSurfaceBrush.HorizontalAlignmentRatio = 0;
        strokeVisualSurfaceBrush.VerticalAlignmentRatio = 0;
        strokeVisualSurfaceBrush.Stretch = CompositionStretch.None;

        using var transform2dEffect = new Transform2DEffect()
        {
            TransformMatrix = Matrix3x2.CreateTranslation(padding, padding),
            Source = new CompositionEffectSourceParameter("alphaMask"),
        };

        using var compositeEffect = new CompositeEffect()
        {
            Mode = CanvasComposite.DestinationOut,
            Sources =
            {
                new CompositionEffectSourceParameter("visualSurface"),
                transform2dEffect
            },
        };

        using var alphaMaskEffect = new AlphaMaskEffect()
        {
            AlphaMask = compositeEffect,
            Source = new CompositionEffectSourceParameter("source"),
        };

        strokeEffectBrush = compositor.CreateEffectFactory(alphaMaskEffect).CreateBrush()
            .TraceDisposable(disposableCollector);
        strokeEffectBrush.SetSourceParameter("visualSurface", strokeVisualSurfaceBrush);
        strokeEffectBrush.SetSourceParameter("alphaMask", alphaMaskSurfaceBrush);
        strokeEffectBrush.SetSourceParameter("source", null);

        strokeBrushVisual.Brush = strokeEffectBrush;

        UpdateStrokeState(true);
    }

    internal CompositionBrush? StrokeBrush
    {
        get => strokeFillBrush;
        set
        {
            if (strokeFillBrush != value)
            {
                strokeFillBrush = value;
                strokeEffectBrush.SetSourceParameter("source", value);

                UpdateStrokeState(true);
            }
        }
    }

    internal float StrokeThickness
    {
        get => strokeThickness;
        set
        {
            if (strokeThickness != value)
            {
                if (value < 0) throw new ArgumentException(nameof(StrokeThickness));
                strokeThickness = value;

                UpdateStrokeState(true);
            }
        }
    }

    internal TextBlockStrokeOptimization Optimization
    {
        get => optimization;
        set
        {
            if (optimization != value)
            {
                optimization = value;

                UpdateStrokeState(true);
            }
        }
    }

    public Visual StrokeVisual => rootVisual;

    private bool IsStrokeEnabled =>
        !disposedValue
        && strokeThickness > 0
        && strokeFillBrush != null
        && weakTextBlock.TryGetTarget(out var target)
        && target.IsLoaded
        && target.ActualWidth > 0
        && target.ActualHeight > 0;


    private void UpdateStrokeState(bool forceUpdate = false)
    {
        if (IsStrokeEnabled)
        {
            if (rootVisual.Children.Count == 0)
            {
                rootVisual.Children.InsertAtTop(strokeBrushVisual);
            }
        }
        else if (!disposedValue)
        {
            rootVisual.Children.RemoveAll();
        }

        UpdateStrokeVisualSurface(forceUpdate);
    }

    private void UpdateStrokeVisualSurface(bool forceUpdate)
    {
        if (disposedValue) return;

        var optimization = this.optimization;

        float unitOffset = optimization == TextBlockStrokeOptimization.Quality ?
            0.5f : 1;
        var unitVisualCount = optimization == TextBlockStrokeOptimization.Speed ?
            4 : 8;

        int visualCount = 0;

        var thickness = Math.Clamp(strokeThickness, 0, 6);

        if (IsStrokeEnabled)
        {
            visualCount = (int)Math.Ceiling(thickness / unitOffset) * unitVisualCount;
        }

        if (!forceUpdate && visualCount == strokeVisualForSurface.Children.Count) return;

        while (strokeVisualForSurface.Children.Count > visualCount)
        {
            var visual = (SpriteVisual)strokeVisualForSurface.Children.First();
            strokeVisualForSurface.Children.Remove(visual);
            visual.StopAnimation("Size");
            visual.Brush = null;
            SpriteVisualPool.Instance.Return(visual);
        }

        while (strokeVisualForSurface.Children.Count < visualCount)
        {
            var visual = SpriteVisualPool.Instance.GetOrCreate();

            visual.StartAnimation("Size", sizeBind);
            visual.Brush = alphaMaskSurfaceBrush;

            strokeVisualForSurface.Children.InsertAtTop(visual);
        }

        int index = 0;
        foreach (var visual in strokeVisualForSurface.Children)
        {
            var progress = index / unitVisualCount + 1;
            var offset = Math.Min(progress * unitOffset, thickness);

            if (unitVisualCount == 4)
            {
                switch (index % 4)
                {
                    case 0: visual.Offset = new Vector3(-offset, -offset, 0); break;
                    case 1: visual.Offset = new Vector3(offset, -offset, 0); break;
                    case 2: visual.Offset = new Vector3(-offset, offset, 0); break;
                    case 3: visual.Offset = new Vector3(offset, offset, 0); break;
                }
            }
            else if (unitVisualCount == 8)
            {
                switch (index % 8)
                {
                    case 0: visual.Offset = new Vector3(-offset, -offset, 0); break;
                    case 1: visual.Offset = new Vector3(0, -offset, 0); break;
                    case 2: visual.Offset = new Vector3(offset, -offset, 0); break;

                    case 3: visual.Offset = new Vector3(-offset, 0, 0); break;
                    case 4: visual.Offset = new Vector3(offset, 0, 0); break;

                    case 5: visual.Offset = new Vector3(-offset, offset, 0); break;
                    case 6: visual.Offset = new Vector3(0, offset, 0); break;
                    case 7: visual.Offset = new Vector3(offset, offset, 0); break;
                }
            }


            index++;
        }
    }

    public void Dispose()
    {
        if (!disposedValue)
        {
            if (strokeVisualForSurface.Children.Count > 0)
            {
                var visuals = strokeVisualForSurface.Children.ToArray();
                strokeVisualForSurface.Children.RemoveAll();

                for (int i = 0; i < visuals.Length; i++)
                {
                    SpriteVisualPool.Instance.Return((SpriteVisual)visuals[i]);
                }
            }

            ((IDisposable)disposableCollector).Dispose();

            disposedValue = true;
        }
    }
}

public enum TextBlockStrokeOptimization
{
    Speed,
    Balanced,
    Quality
}
