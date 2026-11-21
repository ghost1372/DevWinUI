using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_ActionButtonTextBlock), Type = typeof(TextBlock))]
[TemplatePart(Name = nameof(PART_DescriptionBlock), Type = typeof(TextBlock))]
[TemplatePart(Name = nameof(PART_TitleBlock), Type = typeof(TextBlock))]
[TemplatePart(Name = nameof(PART_ActionButton), Type = typeof(Button))]
[TemplatePart(Name = nameof(PART_PreviousBtnRectangle), Type = typeof(Rectangle))]
[TemplatePart(Name = nameof(PART_NextBtnRectangle), Type = typeof(Rectangle))]
[TemplatePart(Name = nameof(PART_TertiaryCompositionShadow), Type = typeof(CompositionShadow))]
[TemplatePart(Name = nameof(PART_SecondaryCompositionShadow), Type = typeof(CompositionShadow))]
[TemplatePart(Name = nameof(PART_PrimaryCompositionShadow), Type = typeof(CompositionShadow))]
[TemplatePart(Name = nameof(PART_FadeRectangle), Type = typeof(Rectangle))]
[TemplatePart(Name = nameof(PART_ActionPanel), Type = typeof(StackPanel))]
[TemplatePart(Name = nameof(PART_NextBtn), Type = typeof(Button))]
[TemplatePart(Name = nameof(PART_PreviousBtn), Type = typeof(Button))]
[TemplatePart(Name = nameof(PART_Arc), Type = typeof(ArcProgress))]
[TemplatePart(Name = nameof(PART_PipsPager), Type = typeof(PipsPager))]
[TemplatePart(Name = nameof(PART_SliderGrid), Type = typeof(Grid))]
[TemplatePart(Name = nameof(PART_Image), Type = typeof(ImageFrame))]
public partial class StoreCarousel : Control
{
    private const string PART_Image = "PART_Image";
    private const string PART_SliderGrid = "PART_SliderGrid";
    private const string PART_PipsPager = "PART_PipsPager";
    private const string PART_Arc = "PART_Arc";
    private const string PART_PreviousBtn = "PART_PreviousBtn";
    private const string PART_NextBtn = "PART_NextBtn";
    private const string PART_ActionPanel = "PART_ActionPanel";
    private const string PART_FadeRectangle = "PART_FadeRectangle";

    private const string PART_PrimaryCompositionShadow = "PART_PrimaryCompositionShadow";
    private const string PART_SecondaryCompositionShadow = "PART_SecondaryCompositionShadow";
    private const string PART_TertiaryCompositionShadow = "PART_TertiaryCompositionShadow";

    private const string PART_NextBtnRectangle = "PART_NextBtnRectangle";
    private const string PART_PreviousBtnRectangle = "PART_PreviousBtnRectangle";

    private const string PART_ActionButton = "PART_ActionButton";

    private const string PART_TitleBlock = "PART_TitleBlock";
    private const string PART_DescriptionBlock = "PART_DescriptionBlock";
    private const string PART_ActionButtonTextBlock = "PART_ActionButtonTextBlock";

    private static readonly TimeSpan ArcFadeDuration = TimeSpan.FromMilliseconds(200);
    private static readonly TimeSpan ArcPostImageDelay = TimeSpan.FromMilliseconds(900);

    // Template parts: assigned in OnApplyTemplate (which throws if a part is missing),
    // so they are safe to treat as non-null once the template has been applied.
    private CompositionShadow primaryBox = null!;
    private CompositionShadow secondaryBox = null!;
    private CompositionShadow tertiaryBox = null!;

    private ImageFrame image = null!;
    private Grid sliderGrid = null!;
    private PipsPager pipsPager = null!;
    private ArcProgress arc = null!;
    private Button nextButton = null!;
    private Button previousButton = null!;
    private StackPanel actionPanel = null!;
    private Rectangle fadeRectangle = null!;

    private Rectangle nextBtnRectangle = null!;
    private Rectangle previousBtnRectangle = null!;

    private Button actionButton = null!;

    private TextBlock titleTextBlock = null!;
    private TextBlock descriptionTextBlock = null!;
    private TextBlock actionButtonTextBlock = null!;

    private readonly List<StoreCarouselItem> imageList = new();
    private int _index = 0;
    private PausableDispatcherTimer? timer;
    private bool isUpdatingPager;
    private int arcAnimationVersion;
    private Storyboard? textStoryboard;
    private Duration textStoryboardDuration;
    private Storyboard? arcFadeOutStoryboard;
    private Storyboard? arcFadeInStoryboard;

    public event EventHandler<int>? SelectedIndexChanged;
    public event EventHandler<StoreCarouselEventArgs>? ItemClick;
    public StoreCarousel()
    {
        DefaultStyleKey = typeof(StoreCarousel);
    }

    private T GetRequiredTemplateChild<T>(string name) where T : DependencyObject
        => GetTemplateChild(name) as T
           ?? throw new InvalidOperationException($"Template part '{name}' of type {typeof(T).Name} was not found in the StoreCarousel template.");

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        image = GetRequiredTemplateChild<ImageFrame>(PART_Image);
        sliderGrid = GetRequiredTemplateChild<Grid>(PART_SliderGrid);
        pipsPager = GetRequiredTemplateChild<PipsPager>(PART_PipsPager);
        arc = GetRequiredTemplateChild<ArcProgress>(PART_Arc);
        nextButton = GetRequiredTemplateChild<Button>(PART_NextBtn);
        previousButton = GetRequiredTemplateChild<Button>(PART_PreviousBtn);
        actionPanel = GetRequiredTemplateChild<StackPanel>(PART_ActionPanel);
        fadeRectangle = GetRequiredTemplateChild<Rectangle>(PART_FadeRectangle);
        actionButton = GetRequiredTemplateChild<Button>(PART_ActionButton);
        nextBtnRectangle = GetRequiredTemplateChild<Rectangle>(PART_NextBtnRectangle);
        previousBtnRectangle = GetRequiredTemplateChild<Rectangle>(PART_PreviousBtnRectangle);

        primaryBox = GetRequiredTemplateChild<CompositionShadow>(PART_PrimaryCompositionShadow);
        secondaryBox = GetRequiredTemplateChild<CompositionShadow>(PART_SecondaryCompositionShadow);
        tertiaryBox = GetRequiredTemplateChild<CompositionShadow>(PART_TertiaryCompositionShadow);

        titleTextBlock = GetRequiredTemplateChild<TextBlock>(PART_TitleBlock);
        descriptionTextBlock = GetRequiredTemplateChild<TextBlock>(PART_DescriptionBlock);
        actionButtonTextBlock = GetRequiredTemplateChild<TextBlock>(PART_ActionButtonTextBlock);

        textStoryboard = null;
        arcFadeOutStoryboard = null;
        arcFadeInStoryboard = null;

        actionButton.Resources["ButtonForegroundPointerOver"] = new SolidColorBrush(Colors.White);

        actionButton.Click -= OnActionButtonClick;
        actionButton.Click += OnActionButtonClick;

        sliderGrid.PointerReleased -= OnSliderPointerReleased;
        sliderGrid.PointerReleased += OnSliderPointerReleased;

        sliderGrid.PointerWheelChanged -= OnSliderPointerWheelChanged;
        sliderGrid.PointerWheelChanged += OnSliderPointerWheelChanged;

        sliderGrid.PointerEntered -= OnSliderPointerEntered;
        sliderGrid.PointerEntered += OnSliderPointerEntered;

        sliderGrid.PointerExited -= OnSliderPointerExited;
        sliderGrid.PointerExited += OnSliderPointerExited;

        KeyDown -= OnKeyDown;
        KeyDown += OnKeyDown;

        primaryBox.PointerReleased -= OnPrimaryBoxPointerReleased;
        primaryBox.PointerReleased += OnPrimaryBoxPointerReleased;

        secondaryBox.PointerReleased -= OnSecondaryBoxPointerReleased;
        secondaryBox.PointerReleased += OnSecondaryBoxPointerReleased;

        tertiaryBox.PointerReleased -= OnTertiaryBoxPointerReleased;
        tertiaryBox.PointerReleased += OnTertiaryBoxPointerReleased;

        nextButton.Click -= OnNextClick;
        nextButton.Click += OnNextClick;

        previousButton.Click -= OnPreviousClick;
        previousButton.Click += OnPreviousClick;

        pipsPager.SelectedIndexChanged -= OnPipsPagerSelectedIndexChanged;
        pipsPager.SelectedIndexChanged += OnPipsPagerSelectedIndexChanged;

        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;

        if (timer != null)
        {
            timer.Tick -= OnTimerTick;
            timer.Stop();
        }

        timer = new PausableDispatcherTimer(ShuffleDuration);
        timer.Tick -= OnTimerTick;
        timer.Tick += OnTimerTick;

        if (IsLoaded && AutoShuffle)
        {
            timer.Start();
        }

        ClearLights(sliderGrid);
        ClearLights(primaryBox);
        ClearLights(secondaryBox);
        ClearLights(tertiaryBox);

        AddLights(sliderGrid);
        AddLights(primaryBox);
        AddLights(secondaryBox);
        AddLights(tertiaryBox);

        Loaded -= OnLoaded;
        Loaded += OnLoaded;

        Unloaded -= OnUnloaded;
        Unloaded += OnUnloaded;

        UpdatePager();
        ApplyItem(imageList.Count > 0 ? imageList[_index] : null);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (AutoShuffle)
        {
            timer?.Start();
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        arcAnimationVersion++;
        timer?.Stop();
    }
    private void ClearLights(FrameworkElement element)
    {
        element.Lights.Clear();
    }
    private void OnActionButtonClick(object sender, RoutedEventArgs e)
    {
        if (imageList.Count == 0)
            return;

        var item = imageList[_index];
        // StoreCarouselEventArgs currently declares a non-nullable 'parameter'; consider making it object?.
        item.RaiseAction(this, new StoreCarouselEventArgs(item.Title, item.Description, item.ImageSource, false, item.Parameter!));
    }

    private void AddLights(FrameworkElement element)
    {
        element.Lights.Add(new AmbLight());
        element.Lights.Add(new RippleLight());
        element.Lights.Add(new HoverLight());
    }
    private void OnSecondaryBoxPointerReleased(object sender, PointerRoutedEventArgs e)
        => RaiseItemClick(e, null, null, SecondaryImageSource, true);

    private void OnTertiaryBoxPointerReleased(object sender, PointerRoutedEventArgs e)
        => RaiseItemClick(e, null, null, TertiaryImageSource, true);

    private void OnPrimaryBoxPointerReleased(object sender, PointerRoutedEventArgs e)
        => RaiseItemClick(e, null, null, PrimaryImageSource, true);

    private void OnSliderPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        int delta = e.GetCurrentPoint(this).Properties.MouseWheelDelta;

        if (delta > 0)
            GoToNextPrev(false);
        else if (delta < 0)
            GoToNextPrev(true);

        e.Handled = true;
    }
    private void OnSliderPointerExited(object sender, PointerRoutedEventArgs e)
    {
        arc.ResumeFillAnimation();
        timer?.Resume();

        nextButton.Visibility = Visibility.Collapsed;
        previousButton.Visibility = Visibility.Collapsed;
    }

    private void OnSliderPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        arc.PauseFillAnimation();
        timer?.Pause();

        nextButton.Visibility = Visibility.Visible;
        previousButton.Visibility = Visibility.Visible;
    }
    private void OnSliderPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (imageList.Count == 0)
            return;

        var item = imageList[_index];
        RaiseItemClick(e, item.Title, item.Description, item.ImageSource, false, item.Parameter);
    }
    private void RaiseItemClick(PointerRoutedEventArgs e, string? title, string? description, object? imageSource, bool isThumbnail, object? parameter = null)
    {
        var properties = e.GetCurrentPoint(null).Properties;
        if (e.Pointer.PointerDeviceType != Microsoft.UI.Input.PointerDeviceType.Mouse ||
            properties.PointerUpdateKind == Microsoft.UI.Input.PointerUpdateKind.LeftButtonReleased)
        {
            // StoreCarouselEventArgs currently declares non-nullable title/description/parameter;
            // consider making them nullable there and dropping the '!' below.
            ItemClick?.Invoke(this, new StoreCarouselEventArgs(title!, description!, imageSource?.ToString() ?? "", isThumbnail, parameter!));
        }
    }
    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (image == null || fadeRectangle == null)
            return;

        fadeRectangle.Width = image.ActualWidth * 0.4;
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        switch (e.Key)
        {
            case Windows.System.VirtualKey.Left:
                GoToNextPrev(false);
                e.Handled = true;
                break;

            case Windows.System.VirtualKey.Right:
                GoToNextPrev(true);
                e.Handled = true;
                break;
        }
    }

    private void OnPreviousClick(object sender, RoutedEventArgs e)
    {
        GoToNextPrev(false);
    }

    private void OnNextClick(object sender, RoutedEventArgs e)
    {
        GoToNextPrev(true);
    }

    private void OnTimerTick(object? sender, object? e)
    {
        GoToNextPrev(true);
    }
    private void OnPipsPagerSelectedIndexChanged(PipsPager sender, PipsPagerSelectedIndexChangedEventArgs args)
    {
        if (isUpdatingPager)
            return;

        GoToIndex(sender.SelectedPageIndex);
    }

    private void StartTransition(int index, ImageFrameTransitionMode transitionMode)
    {
        timer?.Stop();

        _index = index;
        UpdatePager();

        image.TransitionMode = transitionMode;

        FadeOutInArc();
        ApplyItem(imageList[index]);

        SelectedIndexChanged?.Invoke(this, index);

        if (AutoShuffle)
        {
            timer?.Start();
        }
    }

    private void UpdatePager()
    {
        isUpdatingPager = true;
        try
        {
            if (pipsPager.NumberOfPages != imageList.Count)
                pipsPager.NumberOfPages = imageList.Count;

            if (imageList.Count > 0)
                pipsPager.SelectedPageIndex = _index;
        }
        finally
        {
            isUpdatingPager = false;
        }
    }

    private void GoToNextPrev(bool isNext)
    {
        int count = imageList.Count;

        if (image == null || count < 2)
            return;

        int newIndex = isNext ? (_index + 1) % count : (_index - 1 + count) % count;

        StartTransition(newIndex, isNext ? ImageFrameTransitionMode.SlideLeft : ImageFrameTransitionMode.SlideRight);
    }

    public void GoToNext()
    {
        GoToNextPrev(true);
    }
    public void GoToPrevious()
    {
        GoToNextPrev(false);
    }

    public void GoToIndex(int index)
    {
        if (index < 0 || index >= imageList.Count || index == _index)
            return;

        if (image == null)
        {
            _index = index;
            return;
        }

        StartTransition(index, index > _index ? ImageFrameTransitionMode.SlideLeft : ImageFrameTransitionMode.SlideRight);
    }

    private void OnItemsSourceChanged(IList<StoreCarouselItem>? newList)
    {
        imageList.Clear();

        if (newList != null)
            imageList.AddRange(newList);

        _index = 0;

        if (image == null)
            return;

        UpdatePager();
        ApplyItem(imageList.Count > 0 ? imageList[0] : null);
    }

    private void ApplyItem(StoreCarouselItem? item)
    {
        if (item == null)
        {
            // ImageFrame.Source is non-nullable; consider declaring it as nullable in ImageFrame.
            image.Source = null!;
            titleTextBlock.Text = string.Empty;
            descriptionTextBlock.Text = string.Empty;
            actionButtonTextBlock.Text = string.Empty;
            actionButton.Visibility = Visibility.Collapsed;
            return;
        }

        image.useImageEdgeOverContentColor = UseImageEdgeOverContentColor;
        image.Source = (Uri.TryCreate(item.ImageSource, UriKind.Absolute, out var uri) ? uri : null)!;
        titleTextBlock.Text = item.Title;
        descriptionTextBlock.Text = item.Description;
        actionButtonTextBlock.Text = item.ActionButtonText;
        actionButton.Visibility = item.ShowActionButton ? Visibility.Visible : Visibility.Collapsed;
        UpdateVisuals();
        RunTextAnimation();
    }

    private void UpdateVisuals()
    {
        Color color = image._shadowColor;
        var darkBrush = new SolidColorBrush(ColorHelper.DarkenColor(color, 0.5f)) { Opacity = 0.9 };

        fadeRectangle.Width = image.ActualWidth * 0.4;
        fadeRectangle.Fill = new SolidColorBrush(color) { Opacity = 0.5 };
        actionButton.Background = darkBrush;
        actionButton.BorderBrush = CreateLinearGradientBrush(color);
        actionButton.Resources["ButtonBackgroundPointerOver"] = new SolidColorBrush(color);

        nextBtnRectangle.Fill = darkBrush;
        previousBtnRectangle.Fill = darkBrush;
    }

    private LinearGradientBrush CreateLinearGradientBrush(Color color)
    {
        var brush = new LinearGradientBrush
        {
            MappingMode = BrushMappingMode.Absolute,
            StartPoint = new Point(0, 0),
            EndPoint = new Point(0, 2),
            RelativeTransform = new ScaleTransform
            {
                ScaleY = -1,
                CenterY = 0.5,
            }
        };

        brush.GradientStops.Add(new GradientStop
        {
            Offset = 1.0,
            Color = color
        });
        return brush;
    }

    private void RunTextAnimation()
    {
        Duration duration = image.TransitionDuration;

        if (textStoryboard == null || textStoryboardDuration != duration)
        {
            textStoryboard?.Stop();
            textStoryboard = CreateTextStoryboard(duration);
            textStoryboardDuration = duration;
        }

        textStoryboard.Begin();
    }

    private Storyboard CreateTextStoryboard(Duration duration)
    {
        if (actionPanel.RenderTransform is not CompositeTransform transform)
        {
            transform = new CompositeTransform();
            actionPanel.RenderTransform = transform;
            actionPanel.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
        }

        var storyboard = new Storyboard();

        var slideAnimation = new DoubleAnimation
        {
            From = 100,
            To = 0,
            Duration = duration,
            EnableDependentAnimation = true
        };
        Storyboard.SetTarget(slideAnimation, transform);
        Storyboard.SetTargetProperty(slideAnimation, "TranslateX");
        storyboard.Children.Add(slideAnimation);

        var fadeAnimation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = duration,
            EnableDependentAnimation = true
        };
        Storyboard.SetTarget(fadeAnimation, actionPanel);
        Storyboard.SetTargetProperty(fadeAnimation, "Opacity");
        storyboard.Children.Add(fadeAnimation);

        var scaleXAnimation = new DoubleAnimation
        {
            From = 0.8,
            To = 1,
            Duration = duration,
            EnableDependentAnimation = true
        };
        Storyboard.SetTarget(scaleXAnimation, transform);
        Storyboard.SetTargetProperty(scaleXAnimation, "ScaleX");
        storyboard.Children.Add(scaleXAnimation);

        var scaleYAnimation = new DoubleAnimation
        {
            From = 0.8,
            To = 1,
            Duration = duration,
            EnableDependentAnimation = true
        };
        Storyboard.SetTarget(scaleYAnimation, transform);
        Storyboard.SetTargetProperty(scaleYAnimation, "ScaleY");
        storyboard.Children.Add(scaleYAnimation);

        return storyboard;
    }

    private Storyboard CreateArcFadeStoryboard(double to)
    {
        var fade = new DoubleAnimation
        {
            To = to,
            Duration = ArcFadeDuration,
            EnableDependentAnimation = true
        };
        var storyboard = new Storyboard();
        storyboard.Children.Add(fade);
        Storyboard.SetTarget(fade, arc);
        Storyboard.SetTargetProperty(fade, "Opacity");
        return storyboard;
    }

    private async void FadeOutInArc()
    {
        if (!AutoShuffle) return;

        int version = ++arcAnimationVersion;

        arcFadeOutStoryboard ??= CreateArcFadeStoryboard(0);
        arcFadeOutStoryboard.Begin();

        await Task.Delay(ArcFadeDuration + ArcPostImageDelay);

        if (version != arcAnimationVersion) return;

        arcFadeInStoryboard ??= CreateArcFadeStoryboard(1);
        arcFadeInStoryboard.Begin();

        var arcFillDuration = ShuffleDuration - ArcFadeDuration - ArcFadeDuration - ArcPostImageDelay;

        if (arcFillDuration > TimeSpan.Zero)
            arc.StartFillAnimation(arcFillDuration);
    }
}
