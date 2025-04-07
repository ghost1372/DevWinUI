using System.Diagnostics;
using Microsoft.UI.Dispatching;
using Microsoft.Win32.SafeHandles;
using Microsoft.Windows.AppLifecycle;

namespace DevWinUI;
public abstract class SingleInstanceApp
{
    public static int Run(string[] args, string key, Func<SingleInstanceApp> program)
    {
        return Run(args, key, program, null);
    }

    public static int Run(string[] args, string key, Func<SingleInstanceApp> program, Action app)
    {
        bool isRedirect = HandleRedirection(key, program);

        if (!isRedirect)
        {
            if (app is null)
                Current.OnLaunched();
            else
                Current.OnLaunched(app);
        }

        return 0;
    }

    private static SingleInstanceApp Current { get; set; }

    private static bool HandleRedirection(string key, Func<SingleInstanceApp> program)
    {
        bool isRedirect = false;
        AppActivationArguments args = AppInstance.GetCurrent().GetActivatedEventArgs();
        AppInstance keyInstance = AppInstance.FindOrRegisterForKey(key);

        if (keyInstance.IsCurrent)
        {
            Current = program();
            keyInstance.Activated += Current.OnActivated;
        }
        else
        {
            isRedirect = true;
            RedirectActivationTo(args, keyInstance);
        }

        return isRedirect;
    }
    protected virtual void OnLaunched()
    {

    }
    protected virtual void OnLaunched(Action app)
    {
        Application.Start(p =>
        {
            var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
            SynchronizationContext.SetSynchronizationContext(context);
            app();
        });
    }

    protected abstract void OnActivated(object sender, AppActivationArguments args);

    private static SafeFileHandle redirectEventHandle = null;

    private unsafe static void RedirectActivationTo(AppActivationArguments args, AppInstance keyInstance)
    {
        redirectEventHandle = PInvoke.CreateEvent(null, true, false, null);
        Task.Run(() =>
        {
            keyInstance.RedirectActivationToAsync(args).AsTask().Wait();
            PInvoke.SetEvent(redirectEventHandle);
        });

        HANDLE handle = new HANDLE(redirectEventHandle.DangerousGetHandle());
        var pHandles = stackalloc HANDLE[] { handle };

        uint lpdwindex;
        PInvoke.CoWaitForMultipleObjects(0, 0xFFFFFFFF, 1, pHandles, &lpdwindex);

        Process process = Process.GetProcessById((int)keyInstance.ProcessId);
        WindowHelper.SetForegroundWindow(process.MainWindowHandle);
    }
}
