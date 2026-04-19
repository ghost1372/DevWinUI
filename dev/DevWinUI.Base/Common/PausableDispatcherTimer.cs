using System.Diagnostics;

namespace DevWinUI;

public partial class PausableDispatcherTimer
{
    private readonly DispatcherTimer _timer;
    public TimeSpan Interval { get; set; }
    private Stopwatch _stopwatch;
    private bool _resumed;
    public TimeSpan RemainingAfterPause { get; private set; }

    public event EventHandler Tick;
    public PausableDispatcherTimer(TimeSpan interval)
    {
        Interval = interval;
        _timer = new DispatcherTimer { Interval = interval };
        _timer.Tick += Timer_Tick;
        _stopwatch = new Stopwatch();
    }

    public void Start()
    {
        _stopwatch.Restart();
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
        _stopwatch.Stop();
    }

    private void Timer_Tick(object? sender, object e)
    {
        if (_resumed)
        {
            _resumed = false;
            _timer.Interval = Interval;
        }

        _stopwatch.Restart();
        Tick?.Invoke(this, EventArgs.Empty);
    }

    public void Pause()
    {
        _timer.Stop();
        _stopwatch.Stop();
        RemainingAfterPause = _timer.Interval - _stopwatch.Elapsed;
        if (RemainingAfterPause < TimeSpan.Zero)
            RemainingAfterPause = TimeSpan.Zero;
    }

    public void Resume()
    {
        _resumed = true;
        _timer.Interval = RemainingAfterPause;
        RemainingAfterPause = TimeSpan.Zero;
        _timer.Start();
    }
}
