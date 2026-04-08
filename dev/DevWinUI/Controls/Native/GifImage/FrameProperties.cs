namespace DevWinUI;
public struct FrameProperties
{
    public readonly Rect Rect;
    public readonly double DelayMilliseconds;
    public readonly bool ShouldDispose;

    public FrameProperties(Rect rect, double delayMilliseconds, bool shouldDispose)
    {
        Rect = rect;
        DelayMilliseconds = delayMilliseconds;
        ShouldDispose = shouldDispose;
    }
}
