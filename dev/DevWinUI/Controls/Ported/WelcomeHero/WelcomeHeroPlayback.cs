//http://github.com/microsoft/powerToys

namespace DevWinUI;

/// <summary>
/// How the Welcome hero enters the screen.
/// </summary>
public enum WelcomeHeroPlayback
{
    /// <summary>
    /// The full "Warp" intro, played on the first visit of the Welcome page.
    /// </summary>
    Intro,

    /// <summary>
    /// A short fade and scale, used when the Welcome page is visited again.
    /// </summary>
    Settle,

    /// <summary>
    /// The final state without motion, used when Windows animations are turned off.
    /// </summary>
    Static,
}
