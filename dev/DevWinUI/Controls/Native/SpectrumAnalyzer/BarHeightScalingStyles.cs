namespace DevWinUI;
public enum BarHeightScalingStyles
{
    /// <summary>
    /// A decibel scale. Formula: 20 * Log10(FFTValue). Total bar height
    /// is scaled from -90 to 0 dB.
    /// </summary>
    Decibel,

    /// <summary>
    /// A non-linear squareroot scale. Formula: Sqrt(FFTValue) * 2 * BarHeight.
    /// </summary>
    Sqrt,

    /// <summary>
    /// A linear scale. Formula: 9 * FFTValue * BarHeight.
    /// </summary>
    Linear
}
