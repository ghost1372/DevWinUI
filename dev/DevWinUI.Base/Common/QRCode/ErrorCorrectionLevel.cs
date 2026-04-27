namespace DevWinUI;

/// <summary>
/// Specifies the error correction level for a QR code.
/// </summary>
public enum ErrorCorrectionLevel
{
    /// <summary>Low error correction (~7% recovery).</summary>
    Low = 0,

    /// <summary>Medium error correction (~15% recovery).</summary>
    Medium = 1,

    /// <summary>Quartile error correction (~25% recovery).</summary>
    Quartile = 2,

    /// <summary>High error correction (~30% recovery).</summary>
    High = 3,
}
