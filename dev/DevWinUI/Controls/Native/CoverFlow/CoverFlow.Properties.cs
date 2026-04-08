namespace DevWinUI;

public partial class CoverFlow
{
    public EasingFunctionBase EasingFunction
    {
        get { return (EasingFunctionBase)GetValue(EasingFunctionProperty); }
        set { SetValue(EasingFunctionProperty, value); }
    }
    public static readonly DependencyProperty EasingFunctionProperty =
       DependencyProperty.Register(nameof(EasingFunction), typeof(EasingFunctionBase), typeof(CoverFlow), null);

    public Double ManipulationThreshold
    {
        get { return (Double)GetValue(ManipulationThresholdProperty); }
        set { SetValue(ManipulationThresholdProperty, value); }
    }
    public static readonly DependencyProperty ManipulationThresholdProperty =
       DependencyProperty.Register(nameof(ManipulationThreshold), typeof(Double), typeof(CoverFlow), new PropertyMetadata(80d));

    public Duration PageDuration
    {
        get { return (Duration)GetValue(PageDurationProperty); }
        set { SetValue(PageDurationProperty, value); }
    }
    public static readonly DependencyProperty PageDurationProperty =
        DependencyProperty.Register(nameof(PageDuration), typeof(Duration), typeof(CoverFlow), null);

    public double RotationAngle
    {
        get { return (double)GetValue(RotationAngleProperty); }
        set { SetValue(RotationAngleProperty, value); }
    }
    public static readonly DependencyProperty RotationAngleProperty =
        DependencyProperty.Register(nameof(RotationAngle), typeof(double), typeof(CoverFlow), new PropertyMetadata(45d, new PropertyChangedCallback(CoverFlow.OnValuesChanged)));

    public new double Scale
    {
        get { return (double)GetValue(ScaleProperty); }
        set { SetValue(ScaleProperty, value); }
    }
    public static readonly DependencyProperty ScaleProperty =
        DependencyProperty.Register(nameof(Scale), typeof(double), typeof(CoverFlow), new PropertyMetadata(.7d, new PropertyChangedCallback(CoverFlow.OnValuesChanged)));

    public Duration SingleItemDuration
    {
        get { return (Duration)GetValue(SingleItemDurationProperty); }
        set { SetValue(SingleItemDurationProperty, value); }
    }
    public static readonly DependencyProperty SingleItemDurationProperty =
        DependencyProperty.Register(nameof(SingleItemDuration), typeof(Duration), typeof(CoverFlow), null);

    public double SpaceBetweenItems
    {
        get { return (double)GetValue(SpaceBetweenItemsProperty); }
        set { SetValue(SpaceBetweenItemsProperty, value); }
    }
    public static readonly DependencyProperty SpaceBetweenItemsProperty =
        DependencyProperty.Register(nameof(SpaceBetweenItems), typeof(double), typeof(CoverFlow), new PropertyMetadata(60d, new PropertyChangedCallback(CoverFlow.OnValuesChanged)));

    public double SpaceBetweenSelectedItemAndItems
    {
        get { return (double)GetValue(SpaceBetweenSelectedItemAndItemsProperty); }
        set { SetValue(SpaceBetweenSelectedItemAndItemsProperty, value); }
    }
    public static readonly DependencyProperty SpaceBetweenSelectedItemAndItemsProperty =
        DependencyProperty.Register(nameof(SpaceBetweenSelectedItemAndItems), typeof(double), typeof(CoverFlow), new PropertyMetadata(140d, new PropertyChangedCallback(CoverFlow.OnValuesChanged)));

    public double ZDistance
    {
        get { return (double)GetValue(ZDistanceProperty); }
        set { SetValue(ZDistanceProperty, value); }
    }
    public static readonly DependencyProperty ZDistanceProperty =
        DependencyProperty.Register(nameof(ZDistance), typeof(double), typeof(CoverFlow), new PropertyMetadata(0.0d, new PropertyChangedCallback(CoverFlow.OnValuesChanged)));
}
