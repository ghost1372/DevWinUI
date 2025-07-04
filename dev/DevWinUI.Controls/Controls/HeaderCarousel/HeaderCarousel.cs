namespace DevWinUI;
public partial class HeaderCarousel : ItemsControl
{
    private readonly Random random = new();
    private readonly DispatcherTimer selectionTimer = new() { Interval = TimeSpan.FromMilliseconds(4000) };
    private readonly DispatcherTimer deselectionTimer = new() { Interval = TimeSpan.FromMilliseconds(3000) };
    private readonly List<int> numbers = [];
    private CarouselItem? selectedTile;
    private int currentIndex;

    private const string PART_ScrollViewer = "PART_ScrollViewer";
    private const string PART_BackDropImage = "PART_BackDropImage";
    private ScrollViewer scrollViewer;
    private AnimatedImage BackDropImage;
    private BlurEffectManager _blurManager;

    public event EventHandler<HeaderCarouselEventArgs> ItemClick;

    public object Header
    {
        get { return (object)GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(object), typeof(HeaderCarousel), new PropertyMetadata(null));

    public object SecondaryHeader
    {
        get { return (object)GetValue(SecondaryHeaderProperty); }
        set { SetValue(SecondaryHeaderProperty, value); }
    }

    public static readonly DependencyProperty SecondaryHeaderProperty =
        DependencyProperty.Register(nameof(SecondaryHeader), typeof(object), typeof(HeaderCarousel), new PropertyMetadata(null));

    public Visibility SecondaryHeaderVisibility
    {
        get { return (Visibility)GetValue(SecondaryHeaderVisibilityProperty); }
        set { SetValue(SecondaryHeaderVisibilityProperty, value); }
    }

    public static readonly DependencyProperty SecondaryHeaderVisibilityProperty =
        DependencyProperty.Register(nameof(SecondaryHeaderVisibility), typeof(Visibility), typeof(HeaderCarousel), new PropertyMetadata(Visibility.Visible));

    public Visibility HeaderVisibility
    {
        get { return (Visibility)GetValue(HeaderVisibilityProperty); }
        set { SetValue(HeaderVisibilityProperty, value); }
    }

    public static readonly DependencyProperty HeaderVisibilityProperty =
        DependencyProperty.Register(nameof(HeaderVisibility), typeof(Visibility), typeof(HeaderCarousel), new PropertyMetadata(Visibility.Visible));

    public bool IsBlurBackground
    {
        get { return (bool)GetValue(IsBlurBackgroundProperty); }
        set { SetValue(IsBlurBackgroundProperty, value); }
    }

    public static readonly DependencyProperty IsBlurBackgroundProperty =
        DependencyProperty.Register(nameof(IsBlurBackground), typeof(bool), typeof(HeaderCarousel), new PropertyMetadata(true, OnBlurChanged));

    public double BlurAmount
    {
        get { return (double)GetValue(BlurAmountProperty); }
        set { SetValue(BlurAmountProperty, value); }
    }

    public static readonly DependencyProperty BlurAmountProperty =
        DependencyProperty.Register(nameof(BlurAmount), typeof(double), typeof(HeaderCarousel), new PropertyMetadata(100.0, OnBlurChanged));

    private static void OnBlurChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (HeaderCarousel)d;
        if (ctl != null)
        {
            if (ctl._blurManager != null && ctl.IsBlurBackground)
            {
                ctl._blurManager.UpdateBlurAmount((float)ctl.BlurAmount);
            }
            else
            {
                ctl.ApplyBackdropBlur();
            }
        }
    }

    public HeaderCarousel()
    {
        DefaultStyleKey = typeof(HeaderCarousel);    
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        scrollViewer = GetTemplateChild(PART_ScrollViewer) as ScrollViewer;
        BackDropImage = GetTemplateChild(PART_BackDropImage) as AnimatedImage;

        Loaded -= HeaderCarousel_Loaded;
        Loaded += HeaderCarousel_Loaded;
        Unloaded -= HeaderCarousel_Unloaded;
        Unloaded += HeaderCarousel_Unloaded;

        if (BackDropImage != null)
        {
            _blurManager = new BlurEffectManager(BackDropImage);

            ApplyBackdropBlur();
        }
    }
    
    private void ApplyBackdropBlur()
    {
        if (_blurManager != null)
        {
            if (IsBlurBackground)
                _blurManager.EnableBlur((float)BlurAmount);
            else
                _blurManager.DisableBlur();
        }
    }
    private void HeaderCarousel_Unloaded(object sender, RoutedEventArgs e)
    {
        UnsubscribeToEvents();
    }

    private void HeaderCarousel_Loaded(object sender, RoutedEventArgs e)
    {
        ResetAndShuffle();
        SelectNextTile();

        selectionTimer.Tick += SelectionTimer_Tick;
        deselectionTimer.Tick += DeselectionTimer_Tick;
        selectionTimer.Start();
    }
    protected override void OnItemsChanged(object e)
    {
        base.OnItemsChanged(e);

        SubscribeToEvents();
    }
    private void SubscribeToEvents()
    {
        foreach (CarouselItem tile in Items)
        {
            tile.PointerEntered -= Tile_PointerEntered;
            tile.PointerEntered += Tile_PointerEntered;

            tile.PointerExited -= Tile_PointerExited;
            tile.PointerExited += Tile_PointerExited;

            tile.GotFocus -= Tile_GotFocus;
            tile.GotFocus += Tile_GotFocus;

            tile.LostFocus -= Tile_LostFocus;
            tile.LostFocus += Tile_LostFocus;

            tile.Click -= Tile_Click;
            tile.Click += Tile_Click;
        }
    }

    private void UnsubscribeToEvents()
    {
        selectionTimer.Tick -= SelectionTimer_Tick;
        deselectionTimer.Tick -= DeselectionTimer_Tick;
        selectionTimer.Stop();
        deselectionTimer.Stop();
        foreach (CarouselItem tile in Items)
        {
            tile.PointerEntered -= Tile_PointerEntered;
            tile.PointerExited -= Tile_PointerExited;
            tile.GotFocus -= Tile_GotFocus;
            tile.LostFocus -= Tile_LostFocus;
            tile.Click -= Tile_Click;
        }
    }

    private void Tile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is CarouselItem tile)
        {
            tile.PointerExited -= Tile_PointerExited;
            ItemClick?.Invoke(sender, new HeaderCarouselEventArgs { CarouselItem = tile });
        }
    }

    private void SelectionTimer_Tick(object? sender, object e)
    {
        SelectNextTile();
    }

    private async void SelectNextTile()
    {
        if (Items.Count == 0)
        {
            return;
        }

        if (Items[GetNextUniqueRandom()] is CarouselItem tile)
        {
            selectedTile = tile;
            var panel = ItemsPanelRoot;
            if (panel != null)
            {
                GeneralTransform transform = selectedTile.TransformToVisual(panel);
                Point point = transform.TransformPoint(new Point(0, 0));
                scrollViewer.ChangeView(point.X - (scrollViewer.ActualWidth / 2) + (selectedTile.ActualSize.X / 2), null, null);
                await Task.Delay(500);
                SetTileVisuals();
                deselectionTimer.Start();
            }
        }
    }

    private void DeselectionTimer_Tick(object? sender, object e)
    {
        if (selectedTile != null)
        {
            selectedTile.IsSelected = false;
            selectedTile = null;
        }

        deselectionTimer.Stop();
    }

    private void ResetAndShuffle()
    {
        if (Items.Count == 0)
        {
            return;
        }

        numbers.Clear();
        for (int i = 0; i <= Items.Count - 1; i++)
        {
            numbers.Add(i);
        }

        // Shuffle the list
        for (int i = numbers.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (numbers[j], numbers[i]) = (numbers[i], numbers[j]);
        }

        currentIndex = 0;
    }

    private int GetNextUniqueRandom()
    {
        if (currentIndex >= numbers.Count)
        {
            ResetAndShuffle();
        }

        return numbers[currentIndex++];
    }

    private void SetTileVisuals()
    {
        if (selectedTile != null)
        {
            selectedTile.IsSelected = true;
            BackDropImage.ImageUrl = new Uri(selectedTile.ImageUrl);
            BlurAnimationHelper.ApplyBlurEffect(BackDropImage, 100);
            if (selectedTile.Foreground is LinearGradientBrush brush)
            {
                AnimateTitleGradient(brush);
            }
        }
    }

    private void AnimateTitleGradient(LinearGradientBrush brush)
    {
        //// Create a storyboard to hold the animations
        Storyboard storyboard = new();

        int i = 0;
        foreach (GradientStop stop in brush.GradientStops)
        {
            ColorAnimation colorAnimation1 = new()
            {
                To = stop.Color,
                Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                EnableDependentAnimation = true
            };

            if (Header is TextBlock block && block.Foreground is LinearGradientBrush animatedGradientBrush)
            {
                Storyboard.SetTarget(colorAnimation1, animatedGradientBrush.GradientStops[i]);
                Storyboard.SetTargetProperty(colorAnimation1, "Color");
                storyboard.Children.Add(colorAnimation1);
                i++;
            }
        }

        storyboard.Begin();
    }

    private void Tile_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        ((CarouselItem)sender).IsSelected = false;
        selectionTimer.Start();
    }

    private void Tile_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        selectedTile = (CarouselItem)sender;
        SelectTile();
    }

    private async void SelectTile()
    {
        await Task.Delay(100);
        selectionTimer.Stop();
        deselectionTimer.Stop();

        foreach (CarouselItem t in Items)
        {
            t.IsSelected = false;
        }

        // Wait for the animation of a potential other tile to finish
        await Task.Delay(360);
        SetTileVisuals();
    }

    private void Tile_GotFocus(object sender, RoutedEventArgs e)
    {
        selectedTile = (CarouselItem)sender;
        SelectTile();
    }

    private void Tile_LostFocus(object sender, RoutedEventArgs e)
    {
        ((CarouselItem)sender).IsSelected = false;
        selectionTimer.Start();
    }
}
