using System.Collections;

namespace DevWinUI;

public partial class ContentSlider : ItemsControl
{
    private const string PART_Indicator = "PART_Indicator";
    private const string PART_OldPresenter = "PART_OldPresenter";
    private const string PART_NewPresenter = "PART_NewPresenter";
    private const string PART_Next = "PART_Next";
    private const string PART_Prev = "PART_Prev";

    private PipsPager pipsPager;
    private ContentPresenter oldPresenter;
    private ContentPresenter newPresenter;
    private Button nextBtn;
    private Button prevBtn;

    private TranslateTransform _oldTrans;
    private TranslateTransform _newTrans;

    private List<object> itemCache = new();
    private int selectedIndex = 0;
    private bool isAnimating;

    public TimeSpan Duration
    {
        get { return (TimeSpan)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(TimeSpan), typeof(ContentSlider), new PropertyMetadata(TimeSpan.FromMilliseconds(300)));

    public double ViewportWidth
    {
        get { return (double)GetValue(ViewportWidthProperty); }
        set { SetValue(ViewportWidthProperty, value); }
    }

    public static readonly DependencyProperty ViewportWidthProperty =
        DependencyProperty.Register(nameof(ViewportWidth), typeof(double), typeof(ContentSlider), new PropertyMetadata(30.0));

    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(ContentSlider), new PropertyMetadata(default(object)));

    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static new readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(ContentSlider), new PropertyMetadata(null, OnItemsSourceChanged));
    public new IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ContentSlider view && e.NewValue is IEnumerable items)
        {
            var itemList = items.Cast<object>().ToList();
            view.itemCache = itemList;

            if (view.itemCache.Any())
            {
                view.selectedIndex = 0;
                view.SelectedItem = view.itemCache[0];
                view.pipsPager?.NumberOfPages = itemList.Count;
            }
        }
    }

    public ContentSlider()
    {
        DefaultStyleKey = typeof(ContentSlider);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        pipsPager = GetTemplateChild(PART_Indicator) as PipsPager;
        oldPresenter = GetTemplateChild(PART_OldPresenter) as ContentPresenter;
        newPresenter = GetTemplateChild(PART_NewPresenter) as ContentPresenter;
        nextBtn = GetTemplateChild(PART_Next) as Button;
        prevBtn = GetTemplateChild(PART_Prev) as Button;

        pipsPager.SelectedIndexChanged -= OnPipsPagerSelectedIndexChanged;
        pipsPager.SelectedIndexChanged += OnPipsPagerSelectedIndexChanged;

        nextBtn.Click -= OnNextClick;
        nextBtn.Click += OnNextClick;

        prevBtn.Click -= OnPrevClick;
        prevBtn.Click += OnPrevClick;

        if (oldPresenter != null && !(oldPresenter.RenderTransform is TranslateTransform))
        {
            _oldTrans = new TranslateTransform();
            oldPresenter.RenderTransform = _oldTrans;
        }
        else
        {
            _oldTrans = (TranslateTransform)oldPresenter.RenderTransform;
        }

        if (newPresenter != null && !(newPresenter.RenderTransform is TranslateTransform))
        {
            _newTrans = new TranslateTransform();
            newPresenter.RenderTransform = _newTrans;
        }
        else
        {
            _newTrans = (TranslateTransform)newPresenter.RenderTransform;
        }
    }

    private void OnPrevClick(object sender, RoutedEventArgs e)
    {
        SlideTo(-1);
    }

    private void OnNextClick(object sender, RoutedEventArgs e)
    {
        SlideTo(+1);
    }

    private void OnPipsPagerSelectedIndexChanged(PipsPager sender, PipsPagerSelectedIndexChangedEventArgs args)
    {
        int newIndex = sender.SelectedPageIndex;
        int delta = newIndex - selectedIndex;

        if (delta == 0)
            return;

        SlideTo(delta);
    }
    public void SlideTo(int direction)
    {
        if (isAnimating || itemCache.Count < 2)
            return;

        isAnimating = true;

        selectedIndex = (selectedIndex + direction + itemCache.Count) % itemCache.Count;
        pipsPager.SelectedPageIndex = selectedIndex;

        object nextItem = itemCache[selectedIndex];

        newPresenter.Content = nextItem;
        newPresenter.Visibility = Visibility.Visible;

        var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        var oldVisual = ElementCompositionPreview.GetElementVisual(oldPresenter);
        var newVisual = ElementCompositionPreview.GetElementVisual(newPresenter);

        float width = (float)ViewportWidth;
        float from = direction > 0 ? width : -width;

        newVisual.Offset = new Vector3(from, 0, 0);
        oldVisual.Offset = Vector3.Zero;
        newVisual.Opacity = 1f;
        oldVisual.Opacity = 1f;

        var easing = compositor.CreateCubicBezierEasingFunction(
            new Vector2(0.4f, 0.0f),
            new Vector2(0.2f, 1.0f));

        var oldOffsetAnim = compositor.CreateScalarKeyFrameAnimation();
        oldOffsetAnim.InsertKeyFrame(1f, -from, easing);
        oldOffsetAnim.Duration = Duration;

        var newOffsetAnim = compositor.CreateScalarKeyFrameAnimation();
        newOffsetAnim.InsertKeyFrame(1f, 0f, easing);
        newOffsetAnim.Duration = Duration;

        var oldOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
        oldOpacityAnim.InsertKeyFrame(1f, 0f);
        oldOpacityAnim.Duration = Duration;

        var batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

        oldVisual.StartAnimation("Offset.X", oldOffsetAnim);
        oldVisual.StartAnimation("Opacity", oldOpacityAnim);
        newVisual.StartAnimation("Offset.X", newOffsetAnim);

        batch.Completed += (_, __) =>
        {
            SelectedItem = nextItem;

            oldVisual.StopAnimation("Opacity");
            oldVisual.StopAnimation("Offset.X");

            oldVisual.Opacity = 1f;
            oldVisual.Offset = Vector3.Zero;

            newVisual.StopAnimation("Offset.X");
            newVisual.Offset = Vector3.Zero;

            var temp = oldPresenter;
            oldPresenter = newPresenter;
            newPresenter = temp;

            newPresenter.Content = null;
            newPresenter.Visibility = Visibility.Collapsed;

            isAnimating = false;
        };

        batch.End();
    }
}
