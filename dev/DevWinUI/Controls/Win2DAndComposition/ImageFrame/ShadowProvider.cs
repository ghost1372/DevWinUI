namespace DevWinUI;

/// <summary>
/// Provides the shared DropShadow resource
/// </summary>
internal static partial class ShadowProvider
{
    #region Fields

    private static DropShadow _sharedShadow;
    private static readonly object ShadowLock = new object();

    #endregion

    #region Internal APIs

    /// <summary>
    /// Gets the instance of the shared DropShadow
    /// </summary>
    /// <param name="compositor">Compositor</param>
    /// <returns>DropShadow</returns>
    internal static DropShadow GetSharedShadow(Compositor compositor)
    {
        if (_sharedShadow == null)
        {
            lock (ShadowLock)
            {
                if (_sharedShadow == null)
                {
                    _sharedShadow = compositor.CreateDropShadow();
                }
            }
        }

        return _sharedShadow;
    }

    #endregion
}
