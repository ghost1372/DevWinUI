namespace DevWinUI;

/// <summary>
/// Event Arguments for the ImageOpened and ImageFailed events
/// </summary>
public partial class ImageFrameEventArgs : RoutedEventArgs
{
    #region Properties

    /// <summary>
    /// The Uri of the image
    /// </summary>
    public object Source { get; private set; }

    /// <summary>
    /// Optional message
    /// </summary>
    public string Message { get; private set; }

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="source">object (Uri or StorageFile or IRandomAccessStream) 
    /// representing the image</param>
    /// <param name="message">Message</param>
    public ImageFrameEventArgs(object source, string message)
    {
        Source = source;
        Message = message;
    }

    #endregion
}
