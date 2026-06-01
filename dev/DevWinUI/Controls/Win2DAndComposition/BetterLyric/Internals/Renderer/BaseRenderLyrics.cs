// https://github.com/jayfunc/BetterLyrics

namespace DevWinUI;

internal partial class BaseRenderLyrics : BaseLyric
{
    public bool IsPlayingLastFrame { get; set; } = false;

    public BaseRenderLyrics(BaseLyric baseLyrics)
    {
        this.Text = baseLyrics.Text;
        this.StartMs = baseLyrics.StartMs;
        this.EndMs = baseLyrics.EndMs;
        this.StartIndex = baseLyrics.StartIndex;
    }

    public bool GetIsPlaying(double currentMs) => this.StartMs <= currentMs && currentMs < this.EndMs;
    public double GetPlayProgress(double currentMs) => Math.Clamp((currentMs - this.StartMs) / this.DurationMs, 0, 1);
}
