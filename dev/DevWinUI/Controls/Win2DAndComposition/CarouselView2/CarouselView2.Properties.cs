namespace DevWinUI;

public partial class CarouselView2
{
    public bool UseGestures
    {
        get
        {
            return (bool)base.GetValue(UseGesturesProperty);
        }
        set
        {
            base.SetValue(UseGesturesProperty, value);
        }
    }

    public static readonly DependencyProperty UseGesturesProperty = DependencyProperty.Register(nameof(UseGestures), typeof(bool), typeof(CarouselView2),
    new PropertyMetadata(true, new PropertyChangedCallback((s, e) =>
    {
        var control = s as CarouselView2;
        control.Refresh();
    })));

    public object ItemsSource
    {
        get
        {
            return (object)base.GetValue(ItemsSourceProperty);
        }
        set
        {
            base.SetValue(ItemsSourceProperty, value);
        }
    }

    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(CarouselView2),
        new PropertyMetadata(null, new PropertyChangedCallback((s, e) =>
        {
            var control = s as CarouselView2;
            if (e.NewValue is IEnumerable<object> newValue)
            {
                control.Items = newValue.ToArray();
            }
            else
            {
                control.Items = null;
            }
            control.Refresh();
        })));

    public DataTemplate ItemTemplate
    {
        get { return (DataTemplate)GetValue(ItemTemplateProperty); }
        set { SetValue(ItemTemplateProperty, value); }
    }

    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(CarouselView2),
        new PropertyMetadata(null, OnCaptionPropertyChanged));

    public Style ItemContentStyle
    {
        get { return (Style)GetValue(ItemContentStyleProperty); }
        set { SetValue(ItemContentStyleProperty, value); }
    }

    public static readonly DependencyProperty ItemContentStyleProperty = DependencyProperty.Register(nameof(ItemContentStyle), typeof(Style), typeof(CarouselView2),
        new PropertyMetadata(null, OnCaptionPropertyChanged));

    public int SelectedIndex
    {
        get
        {
            return (int)base.GetValue(SelectedIndexProperty);
        }
        set
        {
            base.SetValue(SelectedIndexProperty, value);
        }
    }

    public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(CarouselView2),
new PropertyMetadata(0, OnSelectedIndexChanged));

    public Brush SelectedItemForegroundBrush
    {
        get
        {
            return (Brush)base.GetValue(SelectedItemForegroundBrushProperty);
        }
        set
        {
            base.SetValue(SelectedItemForegroundBrushProperty, value);
        }
    }

    public static readonly DependencyProperty SelectedItemForegroundBrushProperty = DependencyProperty.Register(nameof(SelectedItemForegroundBrush), typeof(Brush), typeof(CarouselView2),
        new PropertyMetadata(null, OnCaptionPropertyChanged));

    public object TriggerSelectionAnimation
    {
        get
        {
            return (object)base.GetValue(TriggerSelectionAnimationProperty);
        }
        set
        {
            base.SetValue(TriggerSelectionAnimationProperty, value);
        }
    }

    public static readonly DependencyProperty TriggerSelectionAnimationProperty = DependencyProperty.Register(nameof(TriggerSelectionAnimation), typeof(object), typeof(CarouselView2),
        new PropertyMetadata(null, new PropertyChangedCallback((s, e) =>
        {
            if (e.NewValue != null)
            {
                var control = s as CarouselView2;
                control.AnimateSelection();
            }
        })));

    public int NavigationSpeed
    {
        get
        {
            return (int)base.GetValue(NavigationSpeedProperty);
        }
        set
        {
            base.SetValue(NavigationSpeedProperty, value);
        }
    }

    public static readonly DependencyProperty NavigationSpeedProperty = DependencyProperty.Register(nameof(NavigationSpeed), typeof(int), typeof(CarouselView2),
    new PropertyMetadata(500, new PropertyChangedCallback((s, e) =>
    {
        var control = s as CarouselView2;
        if (e.NewValue is int newValue && newValue > 1)
        {
            control._delayedZIndexUpdateTimer.Interval = TimeSpan.FromMilliseconds(newValue / 2);
        }
        control.Refresh();
    })));

    public bool ZIndexUpdateWaitsForAnimation
    {
        get
        {
            return (bool)base.GetValue(ZIndexUpdateWaitsForAnimationProperty);
        }
        set
        {
            base.SetValue(ZIndexUpdateWaitsForAnimationProperty, value);
        }
    }

    public static readonly DependencyProperty ZIndexUpdateWaitsForAnimationProperty = DependencyProperty.Register(nameof(ZIndexUpdateWaitsForAnimation), typeof(bool), typeof(CarouselView2),
    new PropertyMetadata(false));

    public double SelectedItemScale
    {
        get
        {
            return (double)base.GetValue(SelectedItemScaleProperty);
        }
        set
        {
            base.SetValue(SelectedItemScaleProperty, value);
        }
    }

    public static readonly DependencyProperty SelectedItemScaleProperty = DependencyProperty.Register(nameof(SelectedItemScale), typeof(double), typeof(CarouselView2),
    new PropertyMetadata(1.0, OnCaptionPropertyChanged));

    public int AdditionalItemsToScale
    {
        get
        {
            return (int)base.GetValue(AdditionalItemsToScaleProperty);
        }
        set
        {
            base.SetValue(AdditionalItemsToScaleProperty, value);
        }
    }

    public static readonly DependencyProperty AdditionalItemsToScaleProperty = DependencyProperty.Register(nameof(AdditionalItemsToScale), typeof(int), typeof(CarouselView2),
     new PropertyMetadata(0, OnCaptionPropertyChanged));

    public int AdditionalItemsToWarp
    {
        get
        {
            return (int)base.GetValue(AdditionalItemsToWarpProperty);
        }
        set
        {
            base.SetValue(AdditionalItemsToWarpProperty, value);
        }
    }

    public static readonly DependencyProperty AdditionalItemsToWarpProperty = DependencyProperty.Register(nameof(AdditionalItemsToWarp), typeof(int), typeof(CarouselView2),
        new PropertyMetadata(4, OnCaptionPropertyChanged));

    public CarouselView2CarouselTypes CarouselType
    {
        get
        {
            return (CarouselView2CarouselTypes)base.GetValue(CarouselTypeProperty);
        }
        set
        {
            base.SetValue(CarouselTypeProperty, value);
        }
    }
    public static readonly DependencyProperty CarouselTypeProperty = DependencyProperty.Register(nameof(CarouselType), typeof(CarouselView2CarouselTypes), typeof(CarouselView2),
    new PropertyMetadata(CarouselView2CarouselTypes.Row, OnCaptionPropertyChanged));

    public CarouselView2WheelAlignments WheelAlignment
    {
        get
        {
            return (CarouselView2WheelAlignments)base.GetValue(WheelAlignmentProperty);
        }
        set
        {
            base.SetValue(WheelAlignmentProperty, value);
        }
    }

    public static readonly DependencyProperty WheelAlignmentProperty = DependencyProperty.Register(nameof(WheelAlignment), typeof(CarouselView2WheelAlignments), typeof(CarouselView2),
        new PropertyMetadata(CarouselView2WheelAlignments.Right, OnCaptionPropertyChanged));

    public int Density
    {
        get
        {
            var val = (int)base.GetValue(DensityProperty);
            int newValue = val;
            // min and max values
            if (val < 12)
            {
                newValue = 12;
            }
            else if (val > 72)
            {
                newValue = 72;
            }
            // ensure it's always divisible by 4.
            else if (val % 12 == 0)
            {
                newValue = val;
            }
            else
            {
                newValue = (val % 12) + val;
            }
            return newValue;
        }
        set
        {
            base.SetValue(DensityProperty, value);
        }
    }

    public static readonly DependencyProperty DensityProperty = DependencyProperty.Register(nameof(Density), typeof(int), typeof(CarouselView2),
    new PropertyMetadata(36, OnCaptionPropertyChanged));

    public double FliptychDegrees
    {
        get
        {
            return (double)base.GetValue(FliptychDegreesProperty);
        }
        set
        {
            base.SetValue(FliptychDegreesProperty, value);
        }
    }

    public static readonly DependencyProperty FliptychDegreesProperty = DependencyProperty.Register(nameof(FliptychDegrees), typeof(double), typeof(CarouselView2),
    new PropertyMetadata(0.0, OnCaptionPropertyChanged));

    public int WarpIntensity
    {
        get
        {
            return (int)base.GetValue(WarpIntensityProperty);
        }
        set
        {
            base.SetValue(WarpIntensityProperty, value);
        }
    }
    public static readonly DependencyProperty WarpIntensityProperty = DependencyProperty.Register(nameof(WarpIntensity), typeof(int), typeof(CarouselView2),
    new PropertyMetadata(0, OnCaptionPropertyChanged));

    public double WarpCurve
    {
        get
        {
            return (double)base.GetValue(WarpCurveProperty);
        }
        set
        {
            base.SetValue(WarpCurveProperty, value);
        }
    }
    public static readonly DependencyProperty WarpCurveProperty = DependencyProperty.Register(nameof(WarpCurve), typeof(double), typeof(CarouselView2),
    new PropertyMetadata(.002, OnCaptionPropertyChanged));

    public int ItemGap
    {
        get
        {
            return (int)base.GetValue(ItemGapProperty);
        }
        set
        {
            base.SetValue(ItemGapProperty, value);
        }
    }

    public static readonly DependencyProperty ItemGapProperty = DependencyProperty.Register(nameof(ItemGap), typeof(int), typeof(CarouselView2),
    new PropertyMetadata(0, OnCaptionPropertyChanged));

    public object SelectNextTrigger
    {
        get
        {
            return base.GetValue(SelectNextTriggerProperty);
        }
        set
        {
            base.SetValue(SelectNextTriggerProperty, value);
        }
    }

    public static readonly DependencyProperty SelectNextTriggerProperty = DependencyProperty.Register(nameof(SelectNextTrigger), typeof(object), typeof(CarouselView2),
    new PropertyMetadata(null, new PropertyChangedCallback((s, e) =>
    {
        if (e.NewValue != null)
        {
            var control = s as CarouselView2;
            control.ChangeSelection(false);
        }
    })));

    public object SelectPreviousTrigger
    {
        get
        {
            return base.GetValue(SelectPreviousTriggerProperty);
        }
        set
        {
            base.SetValue(SelectPreviousTriggerProperty, value);
        }
    }

    public static readonly DependencyProperty SelectPreviousTriggerProperty = DependencyProperty.Register(nameof(SelectPreviousTrigger), typeof(object), typeof(CarouselView2),
    new PropertyMetadata(null, new PropertyChangedCallback((s, e) =>
    {
        if (e.NewValue != null)
        {
            var control = s as CarouselView2;
            control.ChangeSelection(true);
        }
    })));

    public object ManipulationStartedTrigger
    {
        get
        {
            return base.GetValue(ManipulationStartedTriggerProperty);
        }
        set
        {
            base.SetValue(ManipulationStartedTriggerProperty, value);
        }
    }

    public static readonly DependencyProperty ManipulationStartedTriggerProperty = DependencyProperty.Register(nameof(ManipulationStartedTrigger), typeof(object), typeof(CarouselView2),
    new PropertyMetadata(null, new PropertyChangedCallback((s, e) =>
    {
        if (e.NewValue != null)
        {
            var control = s as CarouselView2;
            control.StartManipulationMode();
        }
    })));

    public object ManipulationCompletedTrigger
    {
        get
        {
            return base.GetValue(ManipulationCompletedTriggerProperty);
        }
        set
        {
            base.SetValue(ManipulationCompletedTriggerProperty, value);
        }
    }

    public static readonly DependencyProperty ManipulationCompletedTriggerProperty = DependencyProperty.Register(nameof(ManipulationCompletedTrigger), typeof(object), typeof(CarouselView2),
    new PropertyMetadata(null, new PropertyChangedCallback((s, e) =>
    {
        if (e.NewValue != null)
        {
            var control = s as CarouselView2;
            control.StopManipulationMode();
        }
    })));

    public double CarouselRotationAngle
    {
        get { return (double)GetValue(CarouselRotationAngleProperty); }
        set { SetValue(CarouselRotationAngleProperty, value); }
    }

    public static readonly DependencyProperty CarouselRotationAngleProperty = DependencyProperty.Register(nameof(CarouselRotationAngle), typeof(double), typeof(CarouselView2),
        new PropertyMetadata(null, new PropertyChangedCallback((s, e) =>
        {
            var control = s as CarouselView2;
            if (control._inputAllowed && control.IsCarouselMoving && e.NewValue is double v)
            {
                control.UpdateWheelRotation((float)v);
            }
        })));

    public double CarouselPositionY
    {
        get { return (double)GetValue(CarouselPositionYProperty); }
        set { SetValue(CarouselPositionYProperty, value); }
    }

    public static readonly DependencyProperty CarouselPositionYProperty = DependencyProperty.Register(nameof(CarouselPositionY), typeof(double), typeof(CarouselView2),
        new PropertyMetadata(null, new PropertyChangedCallback((s, e) =>
        {
            var control = s as CarouselView2;
            if (control._inputAllowed && control.IsCarouselMoving && e.NewValue is double v)
            {
                control.UpdateCarouselVerticalScrolling(Convert.ToSingle(v));
            }
        })));

    public double CarouselPositionX
    {
        get { return (double)GetValue(CarouselPositionXProperty); }
        set { SetValue(CarouselPositionXProperty, value); }
    }

    public static readonly DependencyProperty CarouselPositionXProperty = DependencyProperty.Register(nameof(CarouselPositionX), typeof(double), typeof(CarouselView2),
        new PropertyMetadata(null, new PropertyChangedCallback((s, e) =>
        {
            var control = s as CarouselView2;
            if (control._inputAllowed && control.IsCarouselMoving && e.NewValue is double v)
            {
                control.UpdateCarouselHorizontalScrolling(Convert.ToSingle(v));
            }
        })));

    public bool IsPointerWheelEnabled
    {
        get { return (bool)GetValue(IsPointerWheelEnabledProperty); }
        set { SetValue(IsPointerWheelEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsPointerWheelEnabledProperty =
        DependencyProperty.Register(nameof(IsPointerWheelEnabled), typeof(bool), typeof(CarouselView2), new PropertyMetadata(true));

    public bool IsKeyboardNavigationEnabled
    {
        get { return (bool)GetValue(IsKeyboardNavigationEnabledProperty); }
        set { SetValue(IsKeyboardNavigationEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsKeyboardNavigationEnabledProperty =
        DependencyProperty.Register(nameof(IsKeyboardNavigationEnabled), typeof(bool), typeof(CarouselView2), new PropertyMetadata(true));
}
