// https://github.com/jayfunc/BetterLyrics

namespace DevWinUI;

internal struct Keyframe<T>
{
    public T Value { get; }
    public double Duration { get; }

    public Keyframe(T value, double durationSeconds)
    {
        Value = value;
        Duration = durationSeconds;
    }
}
