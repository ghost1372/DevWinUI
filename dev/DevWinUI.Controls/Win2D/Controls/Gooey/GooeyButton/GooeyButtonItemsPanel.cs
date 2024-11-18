// https://github.com/cnbluefire

using Windows.Foundation;

namespace DevWinUI;

public partial class GooeyButtonItemsPanel : Canvas
{
    private readonly List<Storyboard> closeStoryboards = new();

    private readonly List<Storyboard> openStoryboards = new();
    public double Duration => 1.8d;
    public double CloseDuration => Duration - 0.6d;


    #region Override Methods

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        ResetAnimation();
        foreach (var item in Children)
        {
            var x = (size.Width - item.DesiredSize.Width) / 2;
            var y = (size.Height - item.DesiredSize.Height) / 2;

            SetLeft(item, x);
            SetTop(item, y);
        }

        if (Expanded)
            StartAnimation();
        else
            foreach (var item in Children)
                if (item is GooeyButtonItem gooeyButtonItem)
                    gooeyButtonItem.Visibility = Visibility.Collapsed;

        return size;
    }

    #endregion Override Methods

    #region Event Methods

    private void OnStoryboardCompleted(object sender, object e)
    {
        foreach (var item in Children)
            if (item is GooeyButtonItem gooeyButtonItem)
                if (!Expanded)
                    gooeyButtonItem.Visibility = Visibility.Collapsed;
        ItemsAnimationCompleted?.Invoke(this, EventArgs.Empty);
    }

    #endregion Event Methods

    #region Methods

    private void StartAnimation()
    {
        ItemsAnimationStarted?.Invoke(this, EventArgs.Empty);
        if (openStoryboards.Count != Children.Count) ResetAnimation();

        foreach (var item in Children)
            if (item is GooeyButtonItem gooeyButtonItem)
                gooeyButtonItem.Visibility = Visibility.Visible;

        if (Expanded)
            foreach (var sb in openStoryboards)
                sb.Begin();
        else
            foreach (var sb in closeStoryboards)
                sb.Begin();
    }

    private void ResetAnimation()
    {
        if (openStoryboards.Count > 0)
        {
            openStoryboards[0].Completed -= OnStoryboardCompleted;
            openStoryboards.Clear();
        }

        if (closeStoryboards.Count > 0)
        {
            closeStoryboards[0].Completed -= OnStoryboardCompleted;
            closeStoryboards.Clear();
        }

        if (Children.Count == 0) return;

        // k = tan(alpha)

        var unitRad = 0d;
        if (Children.Count > 1) unitRad = Math.PI / 2 / (Children.Count - 1);

        // 联立：
        // x^2 + y^2 = R^2
        // y = kx
        var easing1 = new ElasticEase
        {
            Oscillations = 3,
            Springiness = 10
        };
        var easing2 = new ElasticEase
        {
            Oscillations = 1,
            Springiness = 8
        };
        var sign = false;
        for (var i = 0; i < Children.Count; i++)
        {
            var ik = Math.Tan(unitRad * i);
            var x = Distance / Math.Sqrt(ik * ik + 1);
            var y = -ik * x;

            TranslateTransform trans;
            if (Children[i].RenderTransform is TranslateTransform _trans)
            {
                trans = _trans;
            }
            else
            {
                trans = new TranslateTransform();
                Children[i].RenderTransform = trans;
            }

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

            var sb1 = CreateTranslateStoryboard(x, y, Children[i], trans, easing1, Duration);
            var sb2 = CreateTranslateStoryboard(0, 0, Children[i], trans, easing2, CloseDuration);

            if (!sign)
            {
                sign = true;
                sb1.Completed += OnStoryboardCompleted;
                sb2.Completed += OnStoryboardCompleted;
            }


            openStoryboards.Add(sb1);
            closeStoryboards.Add(sb2);
        }
    }

    private Storyboard CreateTranslateStoryboard(double x, double y, DependencyObject element,
        TranslateTransform translate, EasingFunctionBase easing, double duration = 0.8)
    {
        var sb = new Storyboard();

        var dax = new DoubleAnimation();
        Storyboard.SetTarget(dax, translate);
        Storyboard.SetTargetProperty(dax, "X");
        dax.To = x;
        dax.Duration = TimeSpan.FromSeconds(duration);
        dax.EasingFunction = easing;
        sb.Children.Add(dax);


        var day = new DoubleAnimation();
        Storyboard.SetTarget(day, translate);
        Storyboard.SetTargetProperty(day, "Y");
        day.To = y;
        day.Duration = TimeSpan.FromSeconds(duration);
        day.EasingFunction = easing;
        sb.Children.Add(day);

        var wdax = new DoubleAnimation();
        Storyboard.SetTarget(wdax, element);
        Storyboard.SetTargetProperty(wdax, "Win2DTranslateX");
        wdax.To = x;
        wdax.Duration = TimeSpan.FromSeconds(duration);
        wdax.EasingFunction = easing;
        wdax.EnableDependentAnimation = true;
        sb.Children.Add(wdax);


        var wday = new DoubleAnimation();
        Storyboard.SetTarget(wday, element);
        Storyboard.SetTargetProperty(wday, "Win2DTranslateY");
        wday.To = y;
        wday.Duration = TimeSpan.FromSeconds(duration);
        wday.EasingFunction = easing;
        wday.EnableDependentAnimation = true;
        sb.Children.Add(wday);

        return sb;
    }

    #endregion Methods

    #region Events

    public event EventHandler ItemsAnimationStarted;
    public event EventHandler ItemsAnimationCompleted;

    #endregion Events

    #region Dependency Properties

    public bool Expanded
    {
        get => (bool)GetValue(ExpandedProperty);
        set => SetValue(ExpandedProperty, value);
    }

    // Using a DependencyProperty as the backing store for Expanded.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ExpandedProperty =
        DependencyProperty.Register("Expanded", typeof(bool), typeof(GooeyButtonItemsPanel),
            new PropertyMetadata(false, OnExpandedChanged));

    public double Distance
    {
        get => (double)GetValue(DistanceProperty);
        set => SetValue(DistanceProperty, value);
    }

    // Using a DependencyProperty as the backing store for Distance.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DistanceProperty =
        DependencyProperty.Register("Distance", typeof(double), typeof(GooeyButtonItemsPanel),
            new PropertyMetadata(0d, OnDistanceChanged));


    public GooeyButtonItemsPosition ItemsPosition
    {
        get => (GooeyButtonItemsPosition)GetValue(ItemsPositionProperty);
        set => SetValue(ItemsPositionProperty, value);
    }

    // Using a DependencyProperty as the backing store for ItemsPosition.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ItemsPositionProperty =
        DependencyProperty.Register("ItemsPosition", typeof(GooeyButtonItemsPosition), typeof(GooeyButton),
            new PropertyMetadata(GooeyButtonItemsPosition.LeftTop, OnItemsPositionChanged));


    private static void OnDistanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GooeyButtonItemsPanel sender)
            if (e.NewValue != e.OldValue)
            {
                sender.ResetAnimation();
                if (sender.Expanded) sender.StartAnimation();
            }
    }

    private static void OnExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GooeyButtonItemsPanel sender)
            if (e.NewValue != e.OldValue)
                sender.StartAnimation();
    }


    private static void OnItemsPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GooeyButtonItemsPanel sender)
            if (e.NewValue != e.OldValue)
            {
                sender.ResetAnimation();
                if (sender.Expanded) sender.StartAnimation();
            }
    }

    #endregion Dependency Properties
}
