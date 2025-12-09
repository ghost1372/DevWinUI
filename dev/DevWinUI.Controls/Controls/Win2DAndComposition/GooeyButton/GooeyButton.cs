// https://github.com/cnbluefire

using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[ContentProperty(Name = "Content")]
public sealed partial class GooeyButton : ItemsControl
{
    private Brush background;
    private Shape BackgroundShape;
    private TranslateTransform BackgroundShapeTranslate;
    private long brushColorToken = -1;
    private long brushOpacityToken = -1;

    private GaussianBlurEffect effect;
    private IReadOnlyList<GooeyButtonItem.GooeyButtonItemProperty> gooeyButtonItemsProperty;
    private ICanvasImage image;
    private Button InnerButton;
    private bool isAnimating;

    private Grid LayoutRoot;
    private readonly double mainButtonAnimationDuration = 0.6d;
    private Storyboard mainButtonCloseStoryboard;

    private Storyboard mainButtonOpenStoryboard;
    private GooeyButtonItemsPanel panel;
    private readonly GooeyButtonProperty property = new();
    private bool unloaded;
    private CanvasControl Win2DCanvas;
    private Canvas Win2DHost;

    public GooeyButton()
    {
        DefaultStyleKey = typeof(GooeyButton);
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        SizeChanged += OnSizeChanged;
        RegisterPropertyChangedCallback(BackgroundProperty, OnBackgroundChanged);
        RegisterPropertyChangedCallback(OpacityProperty, OnOpacityChanged);
    }

    #region Create Or Update Resources

    #region Storyboards

    private void UpdateStoryboards()
    {
        UpdateOpenStoryboard();
        UpdateCloseStoryboard();
    }

    private void UpdateOpenStoryboard()
    {
        mainButtonOpenStoryboard = null;
        if (panel == null) return;

        var duration = mainButtonAnimationDuration / 2;

        var ease1 = new CubicEase { EasingMode = EasingMode.EaseOut };
        var ease2 = new ElasticEase { Oscillations = 1, EasingMode = EasingMode.EaseOut };

        var sb = new Storyboard();
        var x = ActualWidth / 15;
        var y = -ActualHeight / 15;
        if (ItemsPosition == GooeyButtonItemsPosition.LeftTop)
        {
            x = -Math.Abs(x);
            y = -Math.Abs(y);
        }
        else if (ItemsPosition == GooeyButtonItemsPosition.RightTop)
        {
            x = Math.Abs(x);
            y = -Math.Abs(y);
        }
        else if (ItemsPosition == GooeyButtonItemsPosition.LeftBottom)
        {
            x = -Math.Abs(x);
            y = Math.Abs(y);
        }
        else if (ItemsPosition == GooeyButtonItemsPosition.RightBottom)
        {
            x = Math.Abs(x);
            y = Math.Abs(y);
        }

        var dax = new DoubleAnimationUsingKeyFrames();
        Storyboard.SetTarget(dax, BackgroundShapeTranslate);
        Storyboard.SetTargetProperty(dax, "X");
        dax.Duration = TimeSpan.FromSeconds(mainButtonAnimationDuration);
        dax.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            KeyTime = TimeSpan.FromSeconds(mainButtonAnimationDuration / 3),
            Value = x,
            EasingFunction = ease1
        });
        dax.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            KeyTime = TimeSpan.FromSeconds(mainButtonAnimationDuration),
            Value = 0,
            EasingFunction = ease2
        });

        var day = new DoubleAnimationUsingKeyFrames();
        Storyboard.SetTarget(day, BackgroundShapeTranslate);
        Storyboard.SetTargetProperty(day, "Y");
        day.Duration = TimeSpan.FromSeconds(mainButtonAnimationDuration);
        day.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            KeyTime = TimeSpan.FromSeconds(mainButtonAnimationDuration / 3),
            Value = y,
            EasingFunction = ease1
        });
        day.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            KeyTime = TimeSpan.FromSeconds(mainButtonAnimationDuration),
            Value = 0,
            EasingFunction = ease2
        });

        var baan = new DoubleAnimation();
        Storyboard.SetTarget(baan, this);
        Storyboard.SetTargetProperty(baan, "BlurAmount");
        baan.EnableDependentAnimation = true;
        baan.To = 0d;
        baan.Duration = TimeSpan.FromSeconds(0.3);
        baan.EasingFunction = new CircleEase
        {
            EasingMode = EasingMode.EaseIn
        };

        sb.Children.Add(dax);
        sb.Children.Add(day);
        //sb.Children.Add(wdax);
        //sb.Children.Add(wday);
        sb.Children.Add(baan);
        mainButtonOpenStoryboard = sb;
    }

    private void UpdateCloseStoryboard()
    {
        mainButtonCloseStoryboard = null;
        if (panel == null) return;

        var begin = 0.16d;

        var ease1 = new CubicEase { EasingMode = EasingMode.EaseOut };
        var ease2 = new ElasticEase { Oscillations = 1, Springiness = 5, EasingMode = EasingMode.EaseOut };

        var sb = new Storyboard();
        var x = ActualWidth / 25;
        var y = ActualHeight / 25;

        if (ItemsPosition == GooeyButtonItemsPosition.LeftTop)
        {
            x = -Math.Abs(x);
            y = -Math.Abs(y);
        }
        else if (ItemsPosition == GooeyButtonItemsPosition.RightTop)
        {
            x = Math.Abs(x);
            y = -Math.Abs(y);
        }
        else if (ItemsPosition == GooeyButtonItemsPosition.LeftBottom)
        {
            x = -Math.Abs(x);
            y = Math.Abs(y);
        }
        else if (ItemsPosition == GooeyButtonItemsPosition.RightBottom)
        {
            x = Math.Abs(x);
            y = Math.Abs(y);
        }

        x = -x;
        y = -y;

        var dax = new DoubleAnimationUsingKeyFrames();
        Storyboard.SetTarget(dax, BackgroundShapeTranslate);
        Storyboard.SetTargetProperty(dax, "X");
        dax.Duration = TimeSpan.FromSeconds(mainButtonAnimationDuration);
        dax.BeginTime = TimeSpan.FromSeconds(begin);
        dax.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            KeyTime = TimeSpan.FromSeconds(mainButtonAnimationDuration / 3),
            Value = x,
            EasingFunction = ease1
        });
        dax.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            KeyTime = TimeSpan.FromSeconds(mainButtonAnimationDuration),
            Value = 0,
            EasingFunction = ease2
        });

        var day = new DoubleAnimationUsingKeyFrames();
        Storyboard.SetTarget(day, BackgroundShapeTranslate);
        Storyboard.SetTargetProperty(day, "Y");
        day.Duration = TimeSpan.FromSeconds(mainButtonAnimationDuration);
        day.BeginTime = TimeSpan.FromSeconds(begin);
        day.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            KeyTime = TimeSpan.FromSeconds(mainButtonAnimationDuration / 3),
            Value = y,
            EasingFunction = ease1
        });
        day.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            KeyTime = TimeSpan.FromSeconds(mainButtonAnimationDuration),
            Value = 0,
            EasingFunction = ease2
        });

        var baan = new DoubleAnimation();
        Storyboard.SetTarget(baan, this);
        Storyboard.SetTargetProperty(baan, "BlurAmount");
        baan.EnableDependentAnimation = true;
        baan.BeginTime = TimeSpan.FromSeconds(begin / 3);
        baan.Duration = TimeSpan.FromSeconds(panel.Duration / 4);
        baan.To = 20d;
        baan.EasingFunction = new CircleEase
        {
            EasingMode = EasingMode.EaseOut
        };

        sb.Children.Add(dax);
        sb.Children.Add(day);
        //sb.Children.Add(wdax);
        //sb.Children.Add(wday);
        sb.Children.Add(baan);

        mainButtonCloseStoryboard = sb;
    }

    #endregion Storyboards

    private void ResetItemsProperty()
    {
        gooeyButtonItemsProperty = null;
        if (panel != null && panel.Children.Count > 0)
        {
            var list = new List<GooeyButtonItem.GooeyButtonItemProperty>();
            foreach (var item in panel.Children)
                if (item is GooeyButtonItem bgItem)
                    list.Add(bgItem.ItemProperty);
                else return;
            gooeyButtonItemsProperty = list;
        }
    }

    private void UpdateWin2DCanvas()
    {
        if (unloaded) return;
        if (ItemsPanelRoot is GooeyButtonItemsPanel panel)
        {
            if (this.panel == null) this.panel = panel;
            if (Win2DCanvas == null)
            {
                Win2DCanvas = new CanvasControl();
                Win2DHost.Children.Add(Win2DCanvas);
                Win2DCanvas.IsHitTestVisible = false;
                Win2DCanvas.CreateResources += OnWin2DCreateResources;
                Win2DCanvas.Draw += OnWin2DDraw;
            }

            var radius = panel.Children.Count == 0
                ? 0
                : panel.Children.Max(c => c.RenderSize.IsEmpty ? 0 : Math.Max(c.RenderSize.Width, c.RenderSize.Height));
            var size = (Distance + radius * 4) * 2;
            Win2DCanvas.Width = size;
            Win2DCanvas.Height = size;
            property.CenterPoint = new Vector2((float)size / 2);
            Canvas.SetLeft(Win2DCanvas, -size / 2);
            Canvas.SetTop(Win2DCanvas, -size / 2);
        }
    }

    private void UpdateProperty()
    {
        Color? color = null;
        var opacity = Opacity;
        if (Background is SolidColorBrush brush)
        {
            color = brush.Color;
            opacity *= brush.Opacity;
        }

        property.BackgroundColor = color;
        property.Opacity = opacity;
        property.Radius = Math.Min(ActualWidth, ActualHeight) / 2;
    }

    #endregion Create Or Update Resources

    #region Event Methods

    #region Win2D

    private void OnWin2DCreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
    {
        var effect1 = new GaussianBlurEffect
        {
            BlurAmount = 20f
        };

        effect = effect1;

        var effect2 = new ColorMatrixEffect
        {
            ColorMatrix = new Matrix5x4
            {
                M11 = 1,
                M12 = 0,
                M13 = 0,
                M14 = 0,
                M21 = 0,
                M22 = 1,
                M23 = 0,
                M24 = 0,
                M31 = 0,
                M32 = 0,
                M33 = 1,
                M34 = 0,
                M41 = 0,
                M42 = 0,
                M43 = 0,
                M44 = 18,
                M51 = 0,
                M52 = 0,
                M53 = 0,
                M54 = -7
            },
            Source = effect1
        };


        image = effect2;
    }

    private void OnWin2DDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        if (panel != null && gooeyButtonItemsProperty != null && property.BackgroundColor.HasValue)
        {
            var source = new CanvasCommandList(sender);
            using (var ds = source.CreateDrawingSession())
            {
                ds.FillCircle(property.CenterPoint, (float)property.Radius - 1f,
                    new CanvasSolidColorBrush(sender, property.BackgroundColor.Value)
                    {
                        Opacity = (float)property.Opacity
                    });

                foreach (var item in gooeyButtonItemsProperty)
                    if (item.BackgroundColor.HasValue)
                        ds.FillCircle(property.CenterPoint.X + (float)item.TranslateX,
                            property.CenterPoint.Y + (float)item.TranslateY, (float)item.Radius,
                            new CanvasSolidColorBrush(sender, item.BackgroundColor.Value)
                            {
                                Opacity = (float)item.Opacity
                            });
            }

            if (property.BlurAmount > 0)
            {
                effect.BlurAmount = (float)property.BlurAmount;
                effect.Source = source;

                args.DrawingSession.DrawImage(image);
            }
            else
            {
                args.DrawingSession.DrawImage(source);
            }
        }

        sender.Invalidate();
    }

    #endregion Win2D

    #region ItemsPanel

    private void OnItemsAnimationCompleted(object sender, EventArgs e)
    {
        isAnimating = false;
    }

    private void OnItemsAnimationStarted(object sender, EventArgs e)
    {
        isAnimating = true;
    }

    private void OnItemsPanelUnloaded(object sender, RoutedEventArgs e)
    {
        if (panel != null)
        {
            panel.Unloaded -= OnItemsPanelUnloaded;
            panel.ItemsAnimationStarted -= OnItemsAnimationStarted;
            panel.ItemsAnimationCompleted -= OnItemsAnimationCompleted;
        }
    }

    #endregion ItemsPanel

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (ItemsPanelRoot is GooeyButtonItemsPanel panel)
        {
            if (this.panel == null) this.panel = panel;
            panel.Distance = Distance;
            panel.Expanded = Expanded;
            panel.ItemsPosition = ItemsPosition;
            panel.ItemsAnimationStarted += OnItemsAnimationStarted;
            panel.ItemsAnimationCompleted += OnItemsAnimationCompleted;
            panel.Unloaded += OnItemsPanelUnloaded;
        }

        UpdateStoryboards();
        UpdateProperty();
        ResetItemsProperty();
        UpdateWin2DCanvas();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        unloaded = true;

        Win2DCanvas.Draw -= OnWin2DDraw;
        panel = null;
        Win2DCanvas.RemoveFromVisualTree();
        Win2DHost.Children.Remove(Win2DCanvas);
        Win2DCanvas = null;
    }

    private void InnerButton_Click(object sender, RoutedEventArgs e)
    {
        if (!OnInvoked()) Expanded = !Expanded;
    }

    private void OnItemInvoked(object sender, RoutedEventArgs e)
    {
        if (sender is GooeyButtonItem gooeyButtonItem)
            ItemInvoked?.Invoke(this, new GooeyButtonItemInvokedEventArgs(gooeyButtonItem.Content));
    }


    #region Update Property

    private void OnOpacityChanged(DependencyObject sender, DependencyProperty dp)
    {
        UpdateProperty();
    }

    private void OnBackgroundChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (background is SolidColorBrush oldSolid)
        {
            if (brushColorToken > -1)
                oldSolid.UnregisterPropertyChangedCallback(SolidColorBrush.ColorProperty, brushColorToken);
            if (brushOpacityToken > -1)
                oldSolid.UnregisterPropertyChangedCallback(Brush.OpacityProperty, brushOpacityToken);
            background = null;
        }

        if (sender.GetValue(dp) is SolidColorBrush newSolid)
        {
            background = newSolid;
            brushColorToken =
                newSolid.RegisterPropertyChangedCallback(SolidColorBrush.ColorProperty, OnBrushColorChanged);
            brushOpacityToken = newSolid.RegisterPropertyChangedCallback(Brush.OpacityProperty, OnBrushOpacityChanged);
        }

        UpdateProperty();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateStoryboards();
    }

    private void OnBrushColorChanged(DependencyObject sender, DependencyProperty dp)
    {
        UpdateProperty();
    }

    private void OnBrushOpacityChanged(DependencyObject sender, DependencyProperty dp)
    {
        UpdateProperty();
    }

    #endregion Update Property

    #endregion Event Methods

    #region Events

    public event GooeyButtonInvokedEventHandler Invoked;
    public event GooeyButtonItemInvokedEventHandler ItemInvoked;

    private bool OnInvoked()
    {
        var args = new GooeyButtonInvokedEventArgs();
        Invoked?.Invoke(this, args);
        return args.Cancel;
    }

    #endregion Events

    #region Override Methods

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        LayoutRoot = GetTemplateChild(nameof(LayoutRoot)) as Grid;
        InnerButton = GetTemplateChild(nameof(InnerButton)) as Button;
        Win2DHost = GetTemplateChild(nameof(Win2DHost)) as Canvas;
        BackgroundShape = GetTemplateChild(nameof(BackgroundShape)) as Shape;
        BackgroundShapeTranslate = GetTemplateChild(nameof(BackgroundShapeTranslate)) as TranslateTransform;
        if (InnerButton != null) InnerButton.Click += InnerButton_Click;
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is GooeyButtonItem;
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        return new GooeyButtonItem();
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);
        ResetItemsProperty();
        if (element is GooeyButtonItem gooeyButtonItem) gooeyButtonItem.Click += OnItemInvoked;
    }

    protected override void ClearContainerForItemOverride(DependencyObject element, object item)
    {
        base.ClearContainerForItemOverride(element, item);
        ResetItemsProperty();
        if (element is GooeyButtonItem gooeyButtonItem) gooeyButtonItem.Click -= OnItemInvoked;
    }

    #endregion Override Methods

    #region Dependency Properties

    public object Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register("Content", typeof(object), typeof(GooeyButton), new PropertyMetadata(null));

    public bool Expanded
    {
        get => (bool)GetValue(ExpandedProperty);
        set => SetValue(ExpandedProperty, value);
    }

    // Using a DependencyProperty as the backing store for Expanded.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ExpandedProperty =
        DependencyProperty.Register("Expanded", typeof(bool), typeof(GooeyButton), new PropertyMetadata(false, (s, a) =>
        {
            if (s is GooeyButton sender)
                if (a.NewValue != a.OldValue)
                {
                    var v = (bool)a.NewValue;
                    if (sender.ItemsPanelRoot is GooeyButtonItemsPanel panel) panel.Expanded = v;
                    if (v && sender.mainButtonOpenStoryboard != null) sender.mainButtonOpenStoryboard.Begin();
                    if (!v && sender.mainButtonCloseStoryboard != null) sender.mainButtonCloseStoryboard.Begin();
                }
        }));


    public double Distance
    {
        get => (double)GetValue(DistanceProperty);
        set => SetValue(DistanceProperty, value);
    }

    // Using a DependencyProperty as the backing store for Distance.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DistanceProperty =
        DependencyProperty.Register("Distance", typeof(double), typeof(GooeyButton), new PropertyMetadata(0d, (s, a) =>
        {
            if (s is GooeyButton sender)
                if (a.NewValue != a.OldValue)
                {
                    if (sender.ItemsPanelRoot is GooeyButtonItemsPanel panel) panel.Distance = (double)a.NewValue;
                    sender.UpdateWin2DCanvas();
                }
        }));

    public double BlurAmount
    {
        get => (double)GetValue(BlurAmountProperty);
        set => SetValue(BlurAmountProperty, value);
    }

    // Using a DependencyProperty as the backing store for BlurAmount.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty BlurAmountProperty =
        DependencyProperty.Register("BlurAmount", typeof(double), typeof(GooeyButton), new PropertyMetadata(20d,
            (s, a) =>
            {
                if (s is GooeyButton sender)
                    if (a.NewValue != a.OldValue)
                        sender.property.BlurAmount = (double)a.NewValue;
            }));

    public GooeyButtonItemsPosition ItemsPosition
    {
        get => (GooeyButtonItemsPosition)GetValue(ItemsPositionProperty);
        set => SetValue(ItemsPositionProperty, value);
    }

    // Using a DependencyProperty as the backing store for ItemsPosition.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ItemsPositionProperty =
        DependencyProperty.Register("ItemsPosition", typeof(GooeyButtonItemsPosition), typeof(GooeyButton),
            new PropertyMetadata(GooeyButtonItemsPosition.LeftTop, (s, a) =>
            {
                if (s is GooeyButton sender)
                    if (a.NewValue != a.OldValue)
                    {
                        if (sender.ItemsPanelRoot is GooeyButtonItemsPanel panel)
                            panel.ItemsPosition = (GooeyButtonItemsPosition)a.NewValue;
                        sender.UpdateStoryboards();
                    }
            }));

    #endregion Dependency Properties

    #region Nested Class

    public partial class GooeyButtonProperty
    {
        public Color? BackgroundColor { get; set; }

        public double Opacity { get; set; }

        public double Radius { get; set; }

        public Vector2 CenterPoint { get; set; }

        public double BlurAmount { get; set; }
    }

    public delegate void GooeyButtonInvokedEventHandler(object sender, GooeyButtonInvokedEventArgs args);

    public delegate void GooeyButtonItemInvokedEventHandler(object sender, GooeyButtonItemInvokedEventArgs args);

    public partial class GooeyButtonInvokedEventArgs : EventArgs
    {
        public bool Cancel { get; set; } = false;
    }

    public partial class GooeyButtonItemInvokedEventArgs : EventArgs
    {
        internal GooeyButtonItemInvokedEventArgs(object item)
        {
            Item = item;
        }

        public object Item { get; }
    }

    #endregion Nested Class
}

public enum GooeyButtonItemsPosition
{
    LeftTop,

    RightTop,

    LeftBottom,

    RightBottom
}
