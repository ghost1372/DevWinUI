using System.Diagnostics;
using Microsoft.UI.Dispatching;
using Microsoft.Win32.SafeHandles;
using Microsoft.Windows.AppLifecycle;
using WinRT;

namespace DevWinUI;

public abstract partial class SingleInstanceApp
{
    private static SingleInstanceApp? Current;

    public static int Run(string[] args, string key, Func<SingleInstanceApp> program, Action app)
    {
        ComWrappersSupport.InitializeComWrappers();

        if (HandleRedirection(key, program))
            return 0;

        Application.Start(_ =>
        {
            SynchronizationContext.SetSynchronizationContext(
                new DispatcherQueueSynchronizationContext(
                    DispatcherQueue.GetForCurrentThread()));

            app();
        });

        return 0;
    }

    private static bool HandleRedirection(string key, Func<SingleInstanceApp> program)
    {
        var current = AppInstance.GetCurrent();
        var args = current.GetActivatedEventArgs();

        var keyInstance = AppInstance.FindOrRegisterForKey(key);

        if (keyInstance.IsCurrent)
        {
            Current = program();

            keyInstance.Activated += (_, e) =>
            {
                Current.OnActivated(e);
            };

            return false;
        }

        keyInstance.RedirectActivationToAsync(args).AsTask().Wait();

        return true;
    }

    protected abstract void OnActivated(AppActivationArguments args);
}
