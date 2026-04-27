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
    ThreeDots,

    [Description("LoadingIndicatorBarStyle")]
    Bar,

    [Description("LoadingIndicatorCogStyle")]
    Cog,

    [Description("LoadingIndicatorCupertinoStyle")]
    Cupertino,

    [Description("LoadingIndicatorDotCircleStyle")]
    DotCircle,

    [Description("LoadingIndicatorGridStyle")]
    Grid,

    [Description("LoadingIndicatorPistonStyle")]
    Piston,

    [Description("LoadingIndicatorRing2Style")]
    Ring2,

    [Description("LoadingIndicatorSwirlStyle")]
    Swirl,

    [Description("LoadingIndicatorTwistStyle")]
    Twist,
}
