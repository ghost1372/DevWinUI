namespace DevWinUI;

public partial class RoadmapSegment : ContentControl
{
    public RoadmapSegment()
    {
        DefaultStyleKey = typeof(RoadmapSegment);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateTone(false);
    }

    public RoadmapSegmentTone Tone
    {
        get { return (RoadmapSegmentTone)GetValue(ToneProperty); }
        set { SetValue(ToneProperty, value); }
    }

    public static readonly DependencyProperty ToneProperty =
        DependencyProperty.Register(nameof(Tone), typeof(RoadmapSegmentTone), typeof(RoadmapSegment), new PropertyMetadata(RoadmapSegmentTone.Neutral, OnToneChanged));

    private static void OnToneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RoadmapSegment segment)
        {
            segment.UpdateTone(true);
        }
    }

    private void UpdateTone(bool useTransitions)
    {
        VisualStateManager.GoToState(this, Tone.ToString(), useTransitions);
    }
}
