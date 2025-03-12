namespace DevWinUI;
public partial class NetworkHelper
{
    /// <summary>
    /// Checks if there is an active internet connection by verifying the presence of a network adapter.
    /// </summary>
    /// <returns>Returns true if a network adapter is available, otherwise false.</returns>
    public static bool IsNetworkAvailable()
    {
        return NetworkInformation.GetInternetConnectionProfile()?.NetworkAdapter != null;
    }
}
