namespace DevWinUI;

public partial class LinearGradientBlurPanel : Grid
{
    public LinearGradientBlurPanel()
    {
        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment = VerticalAlignment.Stretch;

        this.Loaded += OnLoaded;
    }

    private object locker = new object();
    private LinearGradientBlurHelper? helper;

    private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        lock (locker)
        {
            if (IsLoaded)
            {
                EnsureHelper();
            }
        }
    }

    private static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LinearGradientBlurPanel sender && !Equals(e.NewValue, e.OldValue))
        {
            if (sender.helper != null)
            {
                if (e.Property == MaxBlurAmountProperty) sender.helper.MaxBlurAmount = Convert.ToSingle(e.NewValue);
                if (e.Property == MinBlurAmountProperty) sender.helper.MinBlurAmount = Convert.ToSingle(e.NewValue);
                if (e.Property == TintColorProperty) sender.helper.TintColor = (Color)e.NewValue;
                if (e.Property == StartPointProperty) sender.helper.StartPoint = ((Point)e.NewValue).ToVector2();
                if (e.Property == EndPointProperty) sender.helper.EndPoint = ((Point)e.NewValue).ToVector2();
            }
        }
    }

    private LinearGradientBlurHelper EnsureHelper()
    {
        if (helper == null)
        {
            lock (locker)
            {
                if (helper == null)
                {
                    var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
                    helper = new LinearGradientBlurHelper(compositor)
                    {
                        MaxBlurAmount = (float)MaxBlurAmount,
                        MinBlurAmount = (float)MinBlurAmount,
                        TintColor = (Color)TintColor,
                        StartPoint = StartPoint.ToVector2(),
                        EndPoint = EndPoint.ToVector2(),
                    };
                    ElementCompositionPreview.SetElementChildVisual(this, helper.RootVisual);
                }
            }
        }

        return helper;
    }

    public double MaxBlurAmount
    {
        get { return (double)GetValue(MaxBlurAmountProperty); }
        set { SetValue(MaxBlurAmountProperty, value); }
    }

    public static readonly DependencyProperty MaxBlurAmountProperty =
        DependencyProperty.Register(nameof(MaxBlurAmount), typeof(double), typeof(LinearGradientBlurPanel), new PropertyMetadata(64d, OnDependencyPropertyChanged));

    public double MinBlurAmount
    {
        get { return (double)GetValue(MinBlurAmountProperty); }
        set { SetValue(MinBlurAmountProperty, value); }
    }

    public static readonly DependencyProperty MinBlurAmountProperty =
        DependencyProperty.Register(nameof(MinBlurAmount), typeof(double), typeof(LinearGradientBlurPanel), new PropertyMetadata(0.5d, OnDependencyPropertyChanged));

    public Color TintColor
    {
        get { return (Color)GetValue(TintColorProperty); }
        set { SetValue(TintColorProperty, value); }
    }

    public static readonly DependencyProperty TintColorProperty =
        DependencyProperty.Register(nameof(TintColor), typeof(Color), typeof(LinearGradientBlurPanel), new PropertyMetadata(Color.FromArgb(0, 0, 0, 0), OnDependencyPropertyChanged));

    public Point StartPoint
    {
        get { return (Point)GetValue(StartPointProperty); }
        set { SetValue(StartPointProperty, value); }
    }

    public static readonly DependencyProperty StartPointProperty =
        DependencyProperty.Register(nameof(StartPoint), typeof(Point), typeof(LinearGradientBlurPanel), new PropertyMetadata(new Point(0, 0), OnDependencyPropertyChanged));

    public Point EndPoint
    {
        get { return (Point)GetValue(EndPointProperty); }
        set { SetValue(EndPointProperty, value); }
    }

    public static readonly DependencyProperty EndPointProperty =
        DependencyProperty.Register(nameof(EndPoint), typeof(Point), typeof(LinearGradientBlurPanel), new PropertyMetadata(new Point(0, 1), OnDependencyPropertyChanged));

    public void StartMaxBlurAmountAnimation(CompositionAnimation animation) => StartHelperCompositionAnimation("MaxBlurAmount", animation);
    public void StopMaxBlurAmountAnimation() => StopHelperCompositionAnimation("MaxBlurAmount");

    public void StartMinBlurAmountAnimation(CompositionAnimation animation) => StartHelperCompositionAnimation("MinBlurAmount", animation);
    public void StopMinBlurAmountAnimation() => StopHelperCompositionAnimation("MinBlurAmount");

    public void StartStartPointAnimation(CompositionAnimation animation) => StartHelperCompositionAnimation("StartPoint", animation);
    public void StopStartPointAnimation() => StopHelperCompositionAnimation("StartPoint");

    public void StartEndPointAnimation(CompositionAnimation animation) => StartHelperCompositionAnimation("EndPoint", animation);
    public void StopEndPointAnimation() => StopHelperCompositionAnimation("EndPoint");


    private void StartHelperCompositionAnimation(string propertyName, CompositionAnimation animation)
    {
        EnsureHelper().CompositionProperties.StartAnimation(propertyName, animation);
    }

    private void StopHelperCompositionAnimation(string propertyName)
    {
        var helper = this.helper;
        if (helper == null) return;

        helper.CompositionProperties.StopAnimation(propertyName);
    }
}
