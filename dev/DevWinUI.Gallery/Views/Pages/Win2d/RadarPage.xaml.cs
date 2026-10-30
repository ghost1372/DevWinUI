using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DevWinUIGallery.Views;

public sealed partial class RadarPage : Page
{
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(50) };
    private readonly Random _random = new();
    private int _counter = 3;

    public ObservableCollection<RadarMode> ModeItems { get; set; } = new ObservableCollection<RadarMode>(Enum.GetValues<RadarMode>());
    public ObservableCollection<RadarVisibility> VisibilityItems { get; set; } = new ObservableCollection<RadarVisibility>(Enum.GetValues<RadarVisibility>());
    public ObservableCollection<RadarBlipState> StateItems { get; set; } = new ObservableCollection<RadarBlipState>(Enum.GetValues<RadarBlipState>());
    public ObservableCollection<RadarMarker> MarkerItems { get; set; } = new ObservableCollection<RadarMarker>(Enum.GetValues<RadarMarker>());

    public ObservableCollection<RadarAircraft> Aircraft { get; } = new()
    {
        new RadarAircraft("AC-101", 30, 0.8, 6),
        new RadarAircraft("AC-102", 140, 0.55, -4),
        new RadarAircraft("AC-103", 250, 0.9, 8),
    };

    public RadarAircraftMapper Mapper { get; } = new();

    public RadarPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public int ToInt(double value)
    {
        return (int)Math.Round(value);
    }

    public RadarVisibility ZoneVisibility(bool scanOnly)
    {
        return scanOnly ? RadarVisibility.ScanOnly : RadarVisibility.Always;
    }

    public ElementTheme ThemeOf(bool light)
    {
        return light ? ElementTheme.Light : ElementTheme.Dark;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _timer.Tick += OnTick;
        _timer.Start();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _timer.Stop();
        _timer.Tick -= OnTick;
    }

    private void OnTick(object? sender, object e)
    {
        if (!TglLive.IsOn) return;

        foreach (RadarAircraft aircraft in Aircraft)
        {
            aircraft.Bearing = (aircraft.Bearing + aircraft.Speed * 0.05 + 360) % 360;
        }
    }

    private void AddAircraft_Click(object sender, RoutedEventArgs e)
    {
        _counter++;
        Aircraft.Add(new RadarAircraft($"AC-{100 + _counter}", _random.Next(360), 0.3 + _random.NextDouble() * 0.65, _random.Next(-8, 9)));
    }

    private void RemoveAircraft_Click(object sender, RoutedEventArgs e)
    {
        if (Aircraft.Count > 0) Aircraft.RemoveAt(Aircraft.Count - 1);
    }

    private void ToggleAlert_Click(object sender, RoutedEventArgs e)
    {
        if (Aircraft.Count > 0) Aircraft[0].IsAlert = !Aircraft[0].IsAlert;
    }
}

public sealed class RadarAircraft : INotifyPropertyChanged
{
    private string _callsign;
    private double _bearing;
    private double _range;
    private bool _isAlert;

    public RadarAircraft(string callsign, double bearing, double range, double speed)
    {
        _callsign = callsign;
        _bearing = bearing;
        _range = range;
        Speed = speed;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public double Speed { get; }

    public string Callsign
    {
        get { return _callsign; }
        set { SetField(ref _callsign, value); }
    }

    public double Bearing
    {
        get { return _bearing; }
        set { SetField(ref _bearing, value); }
    }

    public double Range
    {
        get { return _range; }
        set { SetField(ref _range, value); }
    }

    public bool IsAlert
    {
        get { return _isAlert; }
        set { SetField(ref _isAlert, value); }
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

public sealed class RadarAircraftMapper : IRadarBlipMapper
{
    public void Map(object item, RadarBlip blip)
    {
        var aircraft = (RadarAircraft)item;

        blip.Angle = aircraft.Bearing;
        blip.Distance = aircraft.Range;
        blip.Heading = aircraft.Bearing + (aircraft.Speed >= 0 ? 90 : -90);
        blip.Label = aircraft.Callsign;
        blip.Marker = RadarMarker.Plane;
        blip.State = aircraft.IsAlert ? RadarBlipState.Alert : RadarBlipState.Normal;
    }
}
