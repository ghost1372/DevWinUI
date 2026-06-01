// https://github.com/jayfunc/BetterLyrics

namespace DevWinUI;

internal struct BetterLyricKeyframe<T>
{
    public T Value { get; }
    public double Duration { get; }

    public BetterLyricKeyframe(T value, double durationSeconds)
    {
        Value = value;
        Duration = durationSeconds;
    }
}
