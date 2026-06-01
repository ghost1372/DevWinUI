// https://github.com/jayfunc/BetterLyrics

namespace DevWinUI;

internal partial class RenderLyricsSyllable : BaseRenderLyrics
{
    public List<RenderLyricsChar> ChildrenRenderLyricsChars { get; set; } = [];

    public RenderLyricsSyllable(BaseLyric lyricsSyllable) : base(lyricsSyllable) { }
}
