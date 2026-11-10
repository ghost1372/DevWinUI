namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    private volatile bool _tourActive;
    private int _tourIndex = -1;
    private float _tourT, _tourFlyLen;
    private float _tourAmt;

    private float _tourDwell = 3f, _tourZoom = 1.8f, _tourDim = 0.65f, _tourSweep = 0.6f;
    private bool _tourOrbit = true, _tourDof = true, _tourLoop = true;

    public event EventHandler<ParticleGlobeMarkerEventArgs> TourMarkerReached;

    public event EventHandler TourEnded;

    public bool IsTourActive => _tourActive;

    public double TourDwellSeconds
    {
        get { return (double)GetValue(TourDwellSecondsProperty); }
        set { SetValue(TourDwellSecondsProperty, value); }
    }

    public static readonly DependencyProperty TourDwellSecondsProperty =
        DependencyProperty.Register(nameof(TourDwellSeconds), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(3.0, OnSettingsPropertyChanged));

    public double TourZoom
    {
        get { return (double)GetValue(TourZoomProperty); }
        set { SetValue(TourZoomProperty, value); }
    }

    public static readonly DependencyProperty TourZoomProperty =
        DependencyProperty.Register(nameof(TourZoom), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(1.8, OnSettingsPropertyChanged));

    public bool IsTourOrbitEnabled
    {
        get { return (bool)GetValue(IsTourOrbitEnabledProperty); }
        set { SetValue(IsTourOrbitEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsTourOrbitEnabledProperty =
        DependencyProperty.Register(nameof(IsTourOrbitEnabled), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public bool IsTourDepthOfFieldEnabled
    {
        get { return (bool)GetValue(IsTourDepthOfFieldEnabledProperty); }
        set { SetValue(IsTourDepthOfFieldEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsTourDepthOfFieldEnabledProperty =
        DependencyProperty.Register(nameof(IsTourDepthOfFieldEnabled), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public double TourDimAmount
    {
        get { return (double)GetValue(TourDimAmountProperty); }
        set { SetValue(TourDimAmountProperty, value); }
    }

    public static readonly DependencyProperty TourDimAmountProperty =
        DependencyProperty.Register(nameof(TourDimAmount), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(0.65, OnSettingsPropertyChanged));

    public bool IsTourLooping
    {
        get { return (bool)GetValue(IsTourLoopingProperty); }
        set { SetValue(IsTourLoopingProperty, value); }
    }

    public static readonly DependencyProperty IsTourLoopingProperty =
        DependencyProperty.Register(nameof(IsTourLooping), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    private void SyncTourSettings()
    {
        _tourDwell = (float)Math.Clamp(double.IsFinite(TourDwellSeconds) ? TourDwellSeconds : 3.0, 0.5, 60.0);
        _tourZoom = (float)Math.Clamp(double.IsFinite(TourZoom) ? TourZoom : 1.8, MinZoom, MaxZoom);
        _tourDim = (float)Math.Clamp(double.IsFinite(TourDimAmount) ? TourDimAmount : 0.65, 0.0, 1.0);
        _tourOrbit = IsTourOrbitEnabled;
        _tourDof = IsTourDepthOfFieldEnabled;
        _tourLoop = IsTourLooping;
    }

    public void StartTour()
    {
        _tourIndex = -1;
        _tourT = 0f;
        _tourActive = true;
    }

    public void StopTour() => _tourActive = false;

    private void EndTour(bool finished)
    {
        _tourActive = false;
        if (!finished) return;

        ResetView();
        var handler = TourEnded;
        if (handler != null) DispatcherQueue?.TryEnqueue(() => handler(this, EventArgs.Empty));
    }

    private void AdvanceTour(float dt, bool userMoved, ref float zoomT)
    {
        float target = _tourActive && _tourDof ? 1f : 0f;
        _tourAmt += Math.Clamp(target - _tourAmt, -dt * 1.5f, dt * 1.5f);

        if (!_tourActive) return;
        if (userMoved)
        {
            EndTour(false);
            return;
        }

        var list = _markers;
        if (list.Length == 0)
        {
            EndTour(true);
            return;
        }

        _tourT += dt;
        if (_tourIndex < 0 || _tourT >= _tourFlyLen + _tourDwell)
        {
            int next = _tourIndex + 1;
            if (next >= list.Length)
            {
                if (!_tourLoop)
                {
                    EndTour(true);
                    return;
                }
                next = 0;
            }

            _tourIndex = next;
            _tourT = 0f;
            _tourFlyLen = _flyDuration + 0.1f;

            var m = list[next];
            RequestFly((float)(m.Latitude * Math.PI / 180.0), (float)(m.Longitude * Math.PI / 180.0), _tourZoom);

            var handler = TourMarkerReached;
            if (handler != null) DispatcherQueue?.TryEnqueue(() => handler(this, new ParticleGlobeMarkerEventArgs(m)));
        }
        else if (_tourOrbit && _tourT > _tourFlyLen)
        {
            float d = _tourT - _tourFlyLen;
            _yaw += _tourSweep / _tourDwell * dt;
            _pitch += 0.05f * 1.3f * MathF.Cos(d * 1.3f) * dt;

            zoomT = Math.Clamp(zoomT * (1f + 0.04f * dt), MinZoom, MaxZoom);
            lock (_inputLock) { _zoomTarget = zoomT; }
        }
    }
}
