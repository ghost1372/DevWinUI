namespace DevWinUI;

internal record class LyricsRenderConfig(
    LyricsWordByWordEffectMode WordByWordEffectMode,
    LyricsContentOrientation LyricsLineContentOrientation,
    bool Is3DLyricsEnabled,
    bool AutoWrap,
    bool IsLyricsBrethingEffectEnabled,
    bool IsLyricsFloatAnimationEnabled,
    bool IsLyricsGlowEffectEnabled,
    bool IsLyricsScaleEffectEnabled,
    bool IsRightToLeftLyric,
    bool IsFanLyricsEnabled,
    float FanLyricsAngle,
    int LyricsFontStrokeWidth,
    double PlayingLineTopOffset,
    float Lyrics3DXAngle,
    float Lyrics3DYAngle,
    float Lyrics3DZAngle,
    float Lyrics3DDepth
);
