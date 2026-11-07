using Microsoft.UI.Dispatching;

namespace DevWinUI;

public sealed partial class GrowlCard
{
    private string? _countdownTextValue;
    private bool _autoCloseActive;

    internal bool IsAutoCloseEnabled
    {
        get { return (AutoClose & SeverityFlag) != GrowlCardAutoClose.None; }
    }

    private GrowlCardAutoClose SeverityFlag
    {
        get
        {
            return Severity switch
            {
                GrowlCardSeverity.Attention => GrowlCardAutoClose.Attention,
                GrowlCardSeverity.Success => GrowlCardAutoClose.Success,
                GrowlCardSeverity.Warning => GrowlCardAutoClose.Warning,
                GrowlCardSeverity.Error => GrowlCardAutoClose.Error,
                GrowlCardSeverity.Info => GrowlCardAutoClose.Info,
                _ => GrowlCardAutoClose.None
            };
        }
    }

    internal bool IsCountdownSuspended
    {
        get { return _countdownSuspended; }
        set
        {
            if (_countdownSuspended == value)
            {
                return;
            }

            _countdownSuspended = value;

            if (value)
            {
                _stopwatch.Stop();
            }
            else if (_timer?.IsRunning == true && !(_isPointerOver && PauseAutoCloseOnPointerOver))
            {
                _stopwatch.Start();
            }
        }
    }

    private static string FormatDuration(TimeSpan value)
    {
        int seconds = (int)Math.Max(0, Math.Ceiling(value.TotalSeconds));

        return seconds >= 60
            ? GrowlStrings.DurationMinutes(seconds / 60, seconds % 60)
            : GrowlStrings.DurationSeconds(seconds);
    }

    private void UpdateCountdownBrush()
    {
        if (_countdownFill is null)
        {
            return;
        }

        var mode = CountdownSeverity;

        if (mode == GrowlCardCountdownSeverity.FollowCardSeverity)
        {
            _countdownFill.Background = _indicatorBrush;
            return;
        }

        _countdownBrush.Color = mode switch
        {
            GrowlCardCountdownSeverity.Attention => ResolveIndicatorColor(GrowlCardSeverity.Attention),
            GrowlCardCountdownSeverity.Success => ResolveIndicatorColor(GrowlCardSeverity.Success),
            GrowlCardCountdownSeverity.Warning => ResolveIndicatorColor(GrowlCardSeverity.Warning),
            GrowlCardCountdownSeverity.Error => ResolveIndicatorColor(GrowlCardSeverity.Error),
            GrowlCardCountdownSeverity.Info => ResolveIndicatorColor(GrowlCardSeverity.Info),
            _ => (_countdownProbe?.Background as SolidColorBrush)?.Color ?? ResolveIndicatorColor(GrowlCardSeverity.Warning)
        };

        _countdownFill.Background = _countdownBrush;
    }

    private void UpdateCountdown(double remainingFraction, TimeSpan remaining)
    {
        if (_countdownScale is not null)
        {
            _countdownScale.ScaleX = Math.Clamp(remainingFraction, 0, 1);
        }

        if (_countdownText is not null)
        {
            var text = FormatDuration(remaining);

            if (text != _countdownTextValue)
            {
                _countdownTextValue = text;
                _countdownText.Text = text;
            }
        }
    }

    private void ResetCountdown()
    {
        UpdateCountdown(1, AutoCloseDuration);
    }

    internal void RestartAutoClose()
    {
        StartAutoClose();
    }

    private void StartAutoClose()
    {
        StopAutoClose();
        ResetCountdown();

        _autoCloseActive = IsAutoCloseEnabled;

        if (!_autoCloseActive || !IsLoaded || Visibility != Visibility.Visible)
        {
            return;
        }

        _stopwatch.Restart();
        if (_countdownSuspended || (_isPointerOver && PauseAutoCloseOnPointerOver))
        {
            _stopwatch.Stop();
        }

        _timer ??= DispatcherQueue.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(33);
        _timer.Tick -= OnTimerTick;
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    private void StopAutoClose()
    {
        _timer?.Stop();
        _stopwatch.Reset();
    }

    private void OnTimerTick(DispatcherQueueTimer sender, object args)
    {
        if (!_stopwatch.IsRunning)
        {
            return;
        }

        double total = AutoCloseDuration.TotalMilliseconds;
        double elapsed = _stopwatch.Elapsed.TotalMilliseconds;
        double fraction = total <= 0 ? 1 : Math.Min(1, elapsed / total);

        UpdateCountdown(1 - fraction, TimeSpan.FromMilliseconds(Math.Max(0, total - elapsed)));

        if (fraction >= 1)
        {
            Close();
        }
    }

}
