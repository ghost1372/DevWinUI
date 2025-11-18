using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

public partial class HaloSlider : Control
{
    public event Action<object, HaloSliderEventArgs> SlideStart, SlideStop;

    public double Offset
    {
        get { return (double)GetValue(OffsetProperty); }
        set { SetValue(OffsetProperty, value); }
    }
    public static readonly DependencyProperty OffsetProperty =
        DependencyProperty.Register(nameof(Offset), typeof(double), typeof(HaloSlider), new PropertyMetadata(0.0));

    public double Angle
    {
        get { return (double)GetValue(AngleProperty); }
        set { SetValue(AngleProperty, value); }
    }
    public static readonly DependencyProperty AngleProperty =
        DependencyProperty.Register(nameof(Angle), typeof(double), typeof(HaloSlider), new PropertyMetadata(0.0));

    public ControlTemplate Thumb
    {
        get { return (ControlTemplate)GetValue(ThumbProperty); }
        set { SetValue(ThumbProperty, value); }
    }
    public static readonly DependencyProperty ThumbProperty =
        DependencyProperty.Register(nameof(Thumb), typeof(ControlTemplate), typeof(HaloSlider), new PropertyMetadata(null));

    public HaloSlider()
    {
        DefaultStyleKey = typeof(HaloSlider);
    }

    protected override void OnApplyTemplate()
    {
        AddHandler(PointerPressedEvent, new PointerEventHandler(StealPointer), true);

        AddHandler(PointerReleasedEvent, new PointerEventHandler(ReleasePointer), true);

        AddHandler(PointerCanceledEvent, new PointerEventHandler(ReleasePointer), true);

        AddHandler(PointerCaptureLostEvent, new PointerEventHandler(ReleasePointer), true);

        AddHandler(PointerMovedEvent, new PointerEventHandler(UpdateValue), true);
    }

    private void StealPointer(object sender, PointerRoutedEventArgs e)
    {
        if (SlideStart != null)
        {
            SlideStart(this, new HaloSliderEventArgs(Angle));
        }

        CapturePointer(e.Pointer);
    }

    private void UpdateValue(object sender, PointerRoutedEventArgs e)
    {
        if (PointerCaptures == null || PointerCaptures.Count != 1)
        {
            return;
        }

        SetValue(AngleProperty, SliderAngle(e) - (double)GetValue(OffsetProperty));
    }

    private void ReleasePointer(object sender, PointerRoutedEventArgs e)
    {
        ReleasePointerCapture(e.Pointer);

        if (SlideStop != null)
        {
            SlideStop(this, new HaloSliderEventArgs(Angle));
        }
    }

    private double SliderAngle(PointerRoutedEventArgs e)
    {
        var centre = new Point(ActualWidth / 2, ActualHeight / 2);

        var thumb = e.GetCurrentPoint(this).Position.RelativeTo(centre);

        var vertical = new HaloVector(0, -1);
        return thumb.AngleTo(vertical);
    }
}
