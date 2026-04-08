using Windows.Devices.Geolocation;

namespace DevWinUI;

public partial class LocationHelper
{
    public static async Task<Geoposition> GetGeoLocationAsync()
    {
        return await GetGeoLocationAsync(PositionAccuracy.Default, null);
    }
    public static async Task<Geoposition> GetGeoLocationAsync(PositionAccuracy accuracy)
    {
        return await GetGeoLocationAsync(accuracy, null);
    }
    public static async Task<Geoposition> GetGeoLocationAsync(PositionAccuracy accuracy, uint? desiredAccuracyInMeters)
    {
        try
        {
            // Request access
            var accessStatus = await Geolocator.RequestAccessAsync();
            if (accessStatus != GeolocationAccessStatus.Allowed)
            {
                throw new InvalidOperationException($"Location access failed: {accessStatus}");
            }

            var geolocator = new Geolocator { DesiredAccuracy = accuracy };

            if (desiredAccuracyInMeters.HasValue)
                geolocator.DesiredAccuracyInMeters = desiredAccuracyInMeters;

            return await geolocator.GetGeopositionAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
