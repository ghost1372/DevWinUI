namespace DevWinUI;

[TemplatePart(Name = nameof(PART_BackDropImage), Type = typeof(AnimatedImage))]
[TemplatePart(Name = nameof(PART_ScrollViewer), Type = typeof(ScrollViewer))]
public partial class HeaderCarousel : ItemsControl
{
    private const string PART_ScrollViewer = "PART_ScrollViewer";
    private const string PART_BackDropImage = "PART_BackDropImage";
    private ScrollViewer scrollViewer;
    private AnimatedImage backDropImage;

    public event EventHandler<HeaderCarouselEventArgs> ItemClick;

    private readonly Random random = new();
    private DispatcherTimer selectionTimer = new();
    private readonly DispatcherTimer deselectionTimer = new();
    private readonly List<int> numbers = [];
    private HeaderCarouselItem? selectedTile;
    private int currentIndex;

    private BlurEffectManager _blurManager;

    public HeaderCarousel()
    {
        DefaultStyleKey = typeof(HeaderCarousel);    
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        selectionTimer.Interval = SelectionDuration;
        deselectionTimer.Interval = DeSelectionDuration;

        scrollViewer = GetTemplateChild(PART_ScrollViewer) as ScrollViewer;
        backDropImage = GetTemplateChild(PART_BackDropImage) as AnimatedImage;

        Loaded -= HeaderCarousel_Loaded;
        Loaded += HeaderCarousel_Loaded;
        Unloaded -= HeaderCarousel_Unloaded;
        Unloaded += HeaderCarousel_Unloaded;

        if (backDropImage != null)
        {
            _blurManager = new BlurEffectManager(backDropImage);

            ApplyBackdropBlur();
        }
    }
    
    private void ApplyBackdropBlur()
    {
        if (_blurManager != null)
        {
            if (IsBlurEnabled)
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
        selectionTimer?.Start();
    }
    protected override void OnItemsChanged(object e)
    {
        base.OnItemsChanged(e);

        SubscribeToEvents();
    }
    private void SubscribeToEvents()
    {
        foreach (HeaderCarouselItem tile in Items)
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
        selectionTimer?.Stop();
        deselectionTimer?.Stop();
        foreach (HeaderCarouselItem tile in Items)
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

        if (Items[GetNextUniqueRandom()] is HeaderCarouselItem tile)
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
                deselectionTimer?.Start();
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
            backDropImage.ImageUrl = new Uri(selectedTile.ImageUrl);
            BlurAnimationHelper.ApplyBlurEffect(backDropImage, 100);
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
        ((HeaderCarouselItem)sender).IsSelected = false;
        selectionTimer.Start();
    }

    private void Tile_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        selectedTile = (HeaderCarouselItem)sender;
        SelectTile();
    }

    private async void SelectTile()
    {
        await Task.Delay(100);
        selectionTimer.Stop();
        deselectionTimer.Stop();

        foreach (HeaderCarouselItem t in Items)
        {
            t.IsSelected = false;
        }

        // Wait for the animation of a potential other tile to finish
        await Task.Delay(360);
        SetTileVisuals();
    }

    private void Tile_GotFocus(object sender, RoutedEventArgs e)
    {
        selectedTile = (HeaderCarouselItem)sender;
        SelectTile();
    }

    private void Tile_LostFocus(object sender, RoutedEventArgs e)
    {
        ((HeaderCarouselItem)sender).IsSelected = false;
        selectionTimer.Start();
    }
}
