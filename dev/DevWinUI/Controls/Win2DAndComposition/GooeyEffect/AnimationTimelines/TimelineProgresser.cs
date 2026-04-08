namespace DevWinUI;

public partial class TimelineProgresser
{
    public TimelineProgresser(double seconds, bool autoReverse)
    {
        Duration = new Duration(TimeSpan.FromSeconds(seconds));
        AutoReverse = autoReverse;
    }

    public bool AutoReverse { get; set; }
    public TimeSpan? BeginTime { get; set; }
    public Duration Duration { get; set; } = new(TimeSpan.FromSeconds(1));
    public EasingFunctionBase EasingFunction { get; set; }
    public bool Forever { get; set; }

    public double GetCurrentProgress(TimeSpan timeSpan)
    {
        var beginTimeTicks = 0l;

        if (BeginTime != null)
            beginTimeTicks = BeginTime.Value.Ticks;
        if (timeSpan.Ticks <= beginTimeTicks)
            return 0;

        var durationTicks = Duration.TimeSpan.Ticks;
        var scalingFactor = AutoReverse ? 2d : 1d;
        if (timeSpan.Ticks - beginTimeTicks > durationTicks * scalingFactor && Forever == false)
            return 0;

        var offsetFromBegin = (timeSpan.Ticks - beginTimeTicks) % (durationTicks * scalingFactor);

        if (offsetFromBegin > durationTicks)
            offsetFromBegin = durationTicks * 2 - offsetFromBegin;

        var progress = offsetFromBegin / durationTicks;

        if (EasingFunction != null)
            progress = EasingFunction.Ease(progress);

        return progress;
    }
}
