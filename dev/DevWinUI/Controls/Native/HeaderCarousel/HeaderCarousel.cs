namespace DevWinUI;

[TemplatePart(Name = nameof(PART_BackDropImage), Type = typeof(AnimatedImage))]
[TemplatePart(Name = nameof(PART_ScrollViewer), Type = typeof(ScrollViewer))]
public partial class HeaderCarousel : ItemsControl
{
    private const string PART_ScrollViewer = "PART_ScrollViewer";
    private const string PART_BackDropImage = "PART_BackDropImage";

    private readonly Random random = new();
    private readonly DispatcherTimer selectionTimer = new();
    private readonly DispatcherTimer deselectionTimer = new();
    private readonly List<int> numbers = [];

    private ScrollViewer scrollViewer;
    private AnimatedImage backDropImage;
    private BlurEffectManager _blurManager;
    private HeaderCarouselItem? selectedTile;
    private int currentIndex;
    private bool _isSelecting;

    public event EventHandler<HeaderCarouselEventArgs> ItemClick;

    public HeaderCarousel()
    {
        DefaultStyleKey = typeof(HeaderCarousel);

        selectionTimer.Tick += SelectionTimer_Tick;
        deselectionTimer.Tick += DeselectionTimer_Tick;
        Loaded += HeaderCarousel_Loaded;
        Unloaded += HeaderCarousel_Unloaded;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        selectionTimer.Interval = SelectionDuration;
        deselectionTimer.Interval = DeSelectionDuration;

        scrollViewer = GetTemplateChild(PART_ScrollViewer) as ScrollViewer;
        backDropImage = GetTemplateChild(PART_BackDropImage) as AnimatedImage;

        _blurManager = backDropImage != null ? new BlurEffectManager(backDropImage) : null;
        ApplyBackdropBlur();
    }

    private void ApplyBackdropBlur()
    {
        if (_blurManager == null)
            return;

        if (IsBlurEnabled)
        {
            _blurManager.BlurAmount = BlurAmount;
            _blurManager.EnableBlur();
        }
        else
        {
            _blurManager.DisableBlur();
        }
    }

    private void HeaderCarousel_Loaded(object sender, RoutedEventArgs e)
    {
        SubscribeToEvents();
        ApplyAutoScroll();
    }

    private void HeaderCarousel_Unloaded(object sender, RoutedEventArgs e)
    {
        selectionTimer.Stop();
        deselectionTimer.Stop();
        UnsubscribeFromEvents();
    }

    protected override void OnItemsChanged(object e)
    {
        base.OnItemsChanged(e);

        numbers.Clear();
        currentIndex = 0;

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        foreach (var tile in Items.OfType<HeaderCarouselItem>())
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

    private void UnsubscribeFromEvents()
    {
        foreach (var tile in Items.OfType<HeaderCarouselItem>())
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
        if (sender is HeaderCarouselItem tile)
        {
            tile.PointerExited -= Tile_PointerExited;
            ItemClick?.Invoke(sender, new HeaderCarouselEventArgs { HeaderCarouselItem = tile });
        }
    }

    private void SelectionTimer_Tick(object? sender, object e)
    {
        SelectNextTile();
    }

    private async void SelectNextTile()
    {
        var panel = ItemsPanelRoot;
        if (Items.Count == 0 || panel == null || scrollViewer == null)
            return;

        if (Items[GetNextUniqueRandom()] is not HeaderCarouselItem tile)
            return;

        selectedTile = tile;

        Point point = tile.TransformToVisual(panel).TransformPoint(new Point(0, 0));
        scrollViewer.ChangeView(point.X - (scrollViewer.ActualWidth / 2) + (tile.ActualSize.X / 2), null, null);

        await Task.Delay(500);

        SetTileVisuals();
        deselectionTimer.Start();
    }

    private void DeselectionTimer_Tick(object? sender, object e)
    {
        if (selectedTile != null)
        {
            selectedTile.IsSelected = false;
            selectedTile = null;
        }

        deselectionTimer.Stop();

        if (IsAutoScrollEnabled)
            selectionTimer.Start();
    }

    private void ResetAndShuffle()
    {
        if (Items.Count == 0)
            return;

        numbers.Clear();
        for (int i = 0; i < Items.Count; i++)
        {
            numbers.Add(i);
        }

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
        if (selectedTile == null)
            return;

        selectedTile.IsSelected = true;

        if (backDropImage != null && Uri.TryCreate(selectedTile.ImageUrl, UriKind.Absolute, out var imageUri))
        {
            backDropImage.ImageUrl = imageUri;
        }

        if (selectedTile.Foreground is LinearGradientBrush brush)
        {
            AnimateTitleGradient(brush);
        }
    }

    private void AnimateTitleGradient(LinearGradientBrush brush)
    {
        if (Header is not TextBlock { Foreground: LinearGradientBrush target })
            return;

        Storyboard storyboard = new();
        int count = Math.Min(brush.GradientStops.Count, target.GradientStops.Count);

        for (int i = 0; i < count; i++)
        {
            ColorAnimation colorAnimation = new()
            {
                To = brush.GradientStops[i].Color,
                Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                EnableDependentAnimation = true
            };

            Storyboard.SetTarget(colorAnimation, target.GradientStops[i]);
            Storyboard.SetTargetProperty(colorAnimation, "Color");
            storyboard.Children.Add(colorAnimation);
        }

        storyboard.Begin();
    }

    private void Tile_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        ((HeaderCarouselItem)sender).IsSelected = false;
        if (IsAutoScrollEnabled)
            selectionTimer.Start();
    }

    private void Tile_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        selectedTile = (HeaderCarouselItem)sender;
        SelectTile();
    }

    private async void SelectTile()
    {
        if (_isSelecting)
            return;

        _isSelecting = true;

        try
        {
            await Task.Delay(100);
            selectionTimer.Stop();
            deselectionTimer.Stop();

            foreach (var tile in Items.OfType<HeaderCarouselItem>())
                tile.IsSelected = false;

            await Task.Delay(360);
            SetTileVisuals();
        }
        finally
        {
            _isSelecting = false;
        }
    }

    private void Tile_GotFocus(object sender, RoutedEventArgs e)
    {
        selectedTile = (HeaderCarouselItem)sender;
        SelectTile();
    }

    private void Tile_LostFocus(object sender, RoutedEventArgs e)
    {
        ((HeaderCarouselItem)sender).IsSelected = false;
        if (IsAutoScrollEnabled)
            selectionTimer.Start();
    }

    private void ApplyAutoScroll()
    {
        if (IsAutoScrollEnabled)
        {
            ResetAndShuffle();
            SelectNextTile();
        }
        else
        {
            selectionTimer.Stop();
            deselectionTimer.Stop();
        }
    }
}
