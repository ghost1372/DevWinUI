// https://github.com/jayfunc/BetterLyrics

using System.Runtime.CompilerServices;
using Microsoft.UI.Dispatching;

namespace DevWinUI;

internal static partial class DispatcherQueueTime
{
    private static ConditionalWeakTable<DispatcherQueueTimer, Action> _debounceInstances = new();
    public static void Debounce(this DispatcherQueueTimer timer, Action action, TimeSpan interval, bool immediate = false)
    {
        // Check and stop any existing timer
        var timeout = timer.IsRunning;
        if (timeout)
        {
            timer.Stop();
        }

        // Reset timer parameters
        timer.Tick -= Timer_Tick;
        timer.Interval = interval;

        // Ensure we haven't been misconfigured and won't execute more times than we expect.
        timer.IsRepeating = false;

        if (immediate)
        {
            // If we have a _debounceInstance queued, then we were running in trailing mode,
            // so if we now have the immediate flag, we should ignore this timer, and run immediately.
            if (_debounceInstances.TryGetValue(timer, out var _))
            {
                timeout = false;

                _debounceInstances.Remove(timer);
            }

            // If we're in immediate mode then we only execute if the timer wasn't running beforehand
            if (!timeout)
            {
                action.Invoke();
            }
        }
        else
        {
            // If we're not in immediate mode, then we'll execute when the current timer expires.
            timer.Tick += Timer_Tick;

            // Store/Update function
            _debounceInstances.AddOrUpdate(timer, action);
        }

        // Start the timer to keep track of the last call here.
        timer.Start();
    }

    private static void Timer_Tick(object sender, object e)
    {
        // This event is only registered/run if we weren't in immediate mode above
        if (sender is DispatcherQueueTimer timer)
        {
            timer.Tick -= Timer_Tick;
            timer.Stop();

            if (_debounceInstances.TryGetValue(timer, out Action? action))
            {
                _debounceInstances.Remove(timer);
                action?.Invoke();
            }
        }
    }
}
