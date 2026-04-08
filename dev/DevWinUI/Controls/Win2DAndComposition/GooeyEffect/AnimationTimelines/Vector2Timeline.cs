namespace DevWinUI;

public partial class Vector2Timeline
{
    private readonly TimelineProgresser _progresser;

    public Vector2Timeline(Vector2 from, Vector2 to, double seconds = 1, TimeSpan? beginTime = null,
        bool autoReverse = true, bool forever = true, EasingFunctionBase easingFunction = null)
    {
        _progresser = new TimelineProgresser(seconds, autoReverse)
        { EasingFunction = easingFunction, BeginTime = beginTime, Forever = forever };
        From = from;
        To = to;
        Duration = new Duration(TimeSpan.FromSeconds(seconds));
        AutoReverse = autoReverse;
        Forever = forever;
    }

    public bool AutoReverse { get; }
    public Duration Duration { get; }
    public bool Forever { get; }
    public Vector2 From { get; }
    public Vector2 To { get; }

    public Vector2 GetCurrentValue(TimeSpan timeSpan)
    {
        var progress = (float)_progresser.GetCurrentProgress(timeSpan);
        return From + (To - From) * progress;
    }
}
