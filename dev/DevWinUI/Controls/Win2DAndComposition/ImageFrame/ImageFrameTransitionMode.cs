namespace DevWinUI;

/// <summary>
/// This enum defines the various types of transitions that can 
/// be used to display the loaded image in the ImageFrame
/// </summary>
public enum ImageFrameTransitionMode
{
    /// <summary>
    /// The loaded image fades into view.
    /// </summary>
    FadeIn,
    /// <summary>
    /// The loaded image slides from the right to left of the ImageFrame.
    /// </summary>
    SlideLeft,
    /// <summary>
    /// The loaded image slides from the left to right of the ImageFrame.
    /// </summary>
    SlideRight,
    /// <summary>
    /// The loaded image slides from the bottom to top of the ImageFrame.
    /// </summary>
    SlideUp,
    /// <summary>
    /// The loaded image slides from the top to the bottom of the ImageFrame.
    /// </summary>
    SlideDown,
    /// <summary>
    /// The loaded image zooms into view.
    /// </summary>
    ZoomIn
}
