using System.Collections;
using Microsoft.UI.Xaml.Input;
using Windows.ApplicationModel;

namespace DevWinUI;

public sealed partial class EasyCarouselPanel : Panel
{
    private DataTemplate _itemTemplate;

    private RectangleGeometry _viewportRect;

    private Compositor _carouselCompositor;

    private DispatcherTimer _timer;

    public event EventHandler<FrameworkElement> ItemTapped;

    public int SelectedIndex
    {
        get { return (int)GetValue(SelectedIndexProperty); }
        set { SetValue(SelectedIndexProperty, value); }
    }

    public static readonly DependencyProperty SelectedIndexProperty =
        DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(EasyCarouselPanel), new PropertyMetadata(0, OnSelectedIndexChanged));

    public object SelectedItem
    {
        get { return (int)GetValue(SelectedItemProperty); }
        private set { SetValue(SelectedItemProperty, value); }
    }

    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(EasyCarouselPanel), new PropertyMetadata(null, null));

    public double Duration
    {
        get { return (double)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(double), typeof(EasyCarouselPanel), new PropertyMetadata(300d, null));

    public double ItemWidth
    {
        get { return (double)GetValue(ItemWidthProperty); }
        set { SetValue(ItemWidthProperty, value); }
    }

    public static readonly DependencyProperty ItemWidthProperty =
        DependencyProperty.Register(nameof(ItemWidth), typeof(double), typeof(EasyCarouselPanel), new PropertyMetadata(310d, null));

    public double ItemHeight
    {
        get { return (double)GetValue(ItemHeightProperty); }
        set { SetValue(ItemHeightProperty, value); }
    }

    public static readonly DependencyProperty ItemHeightProperty =
        DependencyProperty.Register(nameof(ItemHeight), typeof(double), typeof(EasyCarouselPanel), new PropertyMetadata(150d, null));

    public object ItemsSource
    {
        get { return GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(EasyCarouselPanel), new PropertyMetadata(null, OnItemsSourceChanged));

    public double ItemSpacing
    {
        get { return Convert.ToDouble(GetValue(ItemSpacingProperty)); }
        set { SetValue(ItemSpacingProperty, value); }
    }

    public static readonly DependencyProperty ItemSpacingProperty =
        DependencyProperty.Register(nameof(ItemSpacing), typeof(double), typeof(EasyCarouselPanel), new PropertyMetadata(0, OnItemSpacingChanged));

    public DataTemplate ItemTemplate
    {
        get { return _itemTemplate; }
        set { _itemTemplate = value; }
    }

    public bool DetectPointerWheelChange
    {
        get { return (bool)(GetValue(DetectPointerWheelChangeProperty)); }
        set { SetValue(DetectPointerWheelChangeProperty, value); }
    }

    public static readonly DependencyProperty DetectPointerWheelChangeProperty =
        DependencyProperty.Register(nameof(DetectPointerWheelChange), typeof(bool), typeof(EasyCarouselPanel), new PropertyMetadata(true, null));

    public bool AutoShift
    {
        get { return (bool)(GetValue(AutoShiftProperty)); }
        set { SetValue(AutoShiftProperty, value); }
    }

    public static readonly DependencyProperty AutoShiftProperty =
        DependencyProperty.Register(nameof(AutoShift), typeof(bool), typeof(EasyCarouselPanel), new PropertyMetadata(false, OnAutoShiftChanged));

    public TimeSpan Interval
    {
        get { return (TimeSpan)(GetValue(IntervalProperty)); }
        set { SetValue(IntervalProperty, value); }
    }

    public static readonly DependencyProperty IntervalProperty =
        DependencyProperty.Register(nameof(Interval), typeof(TimeSpan), typeof(EasyCarouselPanel), new PropertyMetadata(TimeSpan.FromSeconds(3), OnIntervalChanged));

    public CarouselShiftingDirection ShiftingDirection
    {
        get { return (CarouselShiftingDirection)GetValue(ShiftingDirectionProperty); }
        set { SetValue(ShiftingDirectionProperty, value); }
    }

    public static readonly DependencyProperty ShiftingDirectionProperty =
        DependencyProperty.Register(nameof(ShiftingDirection), typeof(CarouselShiftingDirection), typeof(EasyCarouselPanel), new PropertyMetadata(CarouselShiftingDirection.Forward, null));

    public EasyCarouselPanel()
    {
        Visual carouselVisual = ElementCompositionPreview.GetElementVisual(this);
        _carouselCompositor = carouselVisual.Compositor;
        _timer = new DispatcherTimer { Interval = this.Interval };

        this.ManipulationMode = ManipulationModes.TranslateX;

        this.Tapped -= OnTapped;
        this.Tapped += OnTapped;
        this.ManipulationCompleted -= OnManipulationCompleted;
        this.ManipulationCompleted += OnManipulationCompleted;
        this.PointerWheelChanged -= OnPointerWheelChanged;
        this.PointerWheelChanged += OnPointerWheelChanged;
        _timer.Tick += (sender, o) =>
        {
            switch (this.ShiftingDirection)
            {
                case CarouselShiftingDirection.Forward:
                    MoveForward();
                    break;
                case CarouselShiftingDirection.Backward:
                    MoveBackward();
                    break;
            }
        };
    }

    private void OnTapped(object sender, TappedRoutedEventArgs args)
    {
        FrameworkElement fxElement = args.OriginalSource as FrameworkElement;
        ItemTapped?.Invoke(sender, fxElement);
    }

    private void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        if (e.Cumulative.Translation.X < -100)
        {
            MoveForward();
        }

        if (e.Cumulative.Translation.X > 100)
        {
            MoveBackward();
        }
    }

    private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        e.Handled = true;
        if (DetectPointerWheelChange)
        {
            if (e.GetCurrentPoint(this).Properties.MouseWheelDelta > 0)
            {
                MoveForward();
            }
            else
            {
                MoveBackward();
            }
        }
    }

    private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == null)
            return;

        if (e.NewValue == e.OldValue)
            return;

        EasyCarouselPanel instance = d as EasyCarouselPanel;

        if (instance == null)
            return;

        if (DesignMode.DesignModeEnabled)
            return;

        instance.ShiftElementsAnimatedly((int)e.NewValue);

        instance.SelectedItem = (instance.Children[(int)e.NewValue] as FrameworkElement)?.DataContext;
    }

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == null)
            return;

        if (e.NewValue == e.OldValue)
            return;

        EasyCarouselPanel instance = d as EasyCarouselPanel;

        if (instance == null)
            return;

        instance.BindItems();

        instance.SelectedItem = (instance.Children[instance.SelectedIndex] as FrameworkElement)?.DataContext;
    }

    private static void OnItemSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == null)
            return;

        if (e.NewValue == e.OldValue)
            return;

        EasyCarouselPanel instance = d as EasyCarouselPanel;

        if (instance == null)
            return;

        instance.InvalidateArrange();
    }

    private static void OnAutoShiftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == null)
            return;

        if (e.NewValue == e.OldValue)
            return;

        EasyCarouselPanel instance = d as EasyCarouselPanel;

        if (instance == null)
            return;

        if ((bool)e.NewValue)
        {
            instance._timer.Interval = instance.Interval;
            instance._timer.Start();
        }
        else
        {
            instance._timer.Stop();
        }
    }

    private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == null)
            return;

        if (e.NewValue == e.OldValue)
            return;

        EasyCarouselPanel instance = d as EasyCarouselPanel;

        if (instance == null)
            return;

        instance._timer.Interval = (TimeSpan)e.NewValue;
    }

    private void BindItems()
    {
        if (ItemsSource == null || (ItemsSource as IEnumerable) == null)
            return;

        this.Children.Clear();

        foreach (object item in (IEnumerable)ItemsSource)
            this.CreateElement(item);
    }

    private void CreateElement(object item)
    {
        FrameworkElement element;

        if (this.ItemTemplate != null)
        {
            element = this.ItemTemplate.LoadContent() as FrameworkElement;
        }
        else
        {
            element = new TextBlock();
        }

        if (element == null)
            return;

        element.DataContext = item;
        element.RenderTransformOrigin = new Point(0.5, 0.5);

        this.Children.Add(element);
    }

    private void ShiftElementsAnimatedly(int targetIndex)
    {
        if (targetIndex < 0 || this.Children.Count <= 0)
            return;

        int sliceLength = (int)Math.Ceiling((double)this.Children.Count / 2);
        bool equallyDivided = (this.Children.Count % 2 == 0);

        int pointer = targetIndex;

        for (int i = 0; i < (!equallyDivided ? sliceLength : sliceLength + 1); i++)
        {
            pointer = (targetIndex + i) % this.Children.Count;

            UIElement element = this.Children[pointer];

            double offsetX = i * (this.ItemWidth + this.ItemSpacing);

            //Do not animate elements outside of the viewport.
            ShiftElement(element, offsetX, Math.Abs(offsetX) < _viewportRect?.Rect.Width);
        }

        for (int i = 1; i < sliceLength; i++)
        {
            pointer = (pointer + 1) % this.Children.Count;

            UIElement element = this.Children[pointer];

            double offsetX = (i - sliceLength) * (this.ItemWidth + this.ItemSpacing);

            //Do not animate elements outside of the viewport.
            ShiftElement(element, offsetX, Math.Abs(offsetX) < _viewportRect?.Rect.Width);
        }
    }

    /// <summary>
    /// Shift items with classic TranslateTransforms.
    /// </summary>
    /// <param name="targetIndex"></param>
    private void ShiftElements(int targetIndex)
    {
        if (targetIndex < 0 || this.Children.Count <= 0)
            return;

        int sliceLength = (int)Math.Ceiling((double)this.Children.Count / 2);

        int pointer = targetIndex;

        for (int i = 0; i < sliceLength; i++)
        {
            pointer = (targetIndex + i) % this.Children.Count;

            UIElement element = this.Children[pointer];

            double offsetX = i * this.ItemWidth;

            element.RenderTransform = new TranslateTransform { X = offsetX };
        }

        for (int i = 1; i < sliceLength; i++)
        {
            pointer = (pointer + 1) % this.Children.Count;

            UIElement element = this.Children[pointer];

            double offsetX = (i - sliceLength) * this.ItemWidth;

            element.RenderTransform = new TranslateTransform { X = offsetX };
        }
    }

    private void ShiftElement(UIElement element, double offsetX, bool useAnimation)
    {
        Visual elementVisual = ElementCompositionPreview.GetElementVisual(element);

        if (useAnimation)
        {
            var scalarAnimation = _carouselCompositor.CreateScalarKeyFrameAnimation();
            scalarAnimation.Duration = TimeSpan.FromMilliseconds(Duration);
            scalarAnimation.InsertKeyFrame(1f, (float)offsetX - (float)this.ItemSpacing);
            elementVisual.StartAnimation("Offset.X", scalarAnimation);
        }
        else
        {
            elementVisual.Offset = new Vector3((float)offsetX - (float)this.ItemSpacing, elementVisual.Offset.Y, elementVisual.Offset.Z);
        }
    }

    public void MoveForward()
    {
        if (!this.Children.Any())
            return;

        if (this.SelectedIndex == this.Children.Count - 1)
        {
            this.SelectedIndex = 0;
        }
        else
        {
            this.SelectedIndex += 1;
        }
    }

    public void MoveBackward()
    {
        if (!this.Children.Any())
            return;

        if (this.SelectedIndex == 0)
        {
            this.SelectedIndex = this.Children.Count - 1;
        }
        else
        {
            this.SelectedIndex -= 1;
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        _viewportRect = this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, availableSize.Width, availableSize.Height) };

        if (!this.Children.Any())
            return (availableSize);

        foreach (var uiElement in this.Children)
        {
            var container = (FrameworkElement)uiElement;

            container.Measure(new Size(this.ItemWidth, this.ItemHeight));
        }

        return (availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _viewportRect = this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, finalSize.Width, finalSize.Height) };

        if (!this.Children.Any())
            return finalSize;

        var selectedElement = this.Children[SelectedIndex];

        Double centerX = (finalSize.Width / 2) - (ItemWidth / 2);
        Double centerY = (finalSize.Height - selectedElement.DesiredSize.Height) / 2;

        for (int i = 0; i < this.Children.Count; i++)
        {
            UIElement element = this.Children[i];

            if (double.IsNaN(element.DesiredSize.Width) || double.IsNaN(element.DesiredSize.Height))
                continue;

            var rect = new Rect(centerX + this.ItemSpacing, centerY, element.DesiredSize.Width, element.DesiredSize.Height);

            element.Arrange(rect);
        }

        if (!DesignMode.DesignModeEnabled)
            ShiftElementsAnimatedly(SelectedIndex);

        return finalSize;
    }
}
