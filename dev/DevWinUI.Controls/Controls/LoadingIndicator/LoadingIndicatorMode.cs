using System.ComponentModel;

namespace DevWinUI;
public enum LoadingIndicatorMode
{
    [Description("LoadingIndicatorWaveStyle")]
    Wave,

    [Description("LoadingIndicatorArcsStyle")]
    Arcs,

    [Description("LoadingIndicatorArcsRingStyle")]
    ArcsRing,

    [Description("LoadingIndicatorDoubleBounceStyle")]
    DoubleBounce,

    [Description("LoadingIndicatorFlipPlaneStyle")]
    FlipPlane,

    [Description("LoadingIndicatorPulseStyle")]
    Pulse,

    [Description("LoadingIndicatorRingStyle")]
    Ring,

    [Description("LoadingIndicatorThreeDotsStyle")]
    ThreeDots
}
