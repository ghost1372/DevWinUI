namespace DevWinUIGallery.Views;

public sealed partial class SunTimesHelperPage : Page
{
    public SunTimesHelperPage()
    {
        InitializeComponent();
    }

    private void BtnCalc_Click(object sender, RoutedEventArgs e)
    {
        var sunTimes = SunTimesHelper.CalculateSunriseSunset(NBLatitude.Value, NBLongitude.Value, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        TxtSunRise.Text = $"Sunrise: {sunTimes.SunriseHour}:{sunTimes.SunriseMinute}";
        TxtSunSet.Text = $"Sunset: {sunTimes.SunsetHour}:{sunTimes.SunsetMinute}";
    }

    private async void BtnGetLocation_Click(object sender, RoutedEventArgs e)
    {
        var pos = await LocationHelper.GetGeoLocationAsync();
        if (pos == null)
            return;

        NBLatitude.Value = pos.Coordinate.Point.Position.Latitude;
        NBLongitude.Value = pos.Coordinate.Point.Position.Longitude;

        InfoBarSource.Message = pos.Coordinate.PositionSource.ToString();
    }
}
