using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Windows.Win32.System.RestartManager;

namespace DevWinUI;

public sealed partial class RestartManager : IDisposable
{
    private uint SessionHandle { get; }

    public string SessionKey { get; }

    private RestartManager(uint sessionHandle, string sessionKey)
    {
        SessionHandle = sessionHandle;
        SessionKey = sessionKey;
    }

    public unsafe static RestartManager CreateSession()
    {
        var sessionKey = Guid.NewGuid().ToString();

        uint sessionHandle;
        WIN32_ERROR result;
        fixed (char* pKey = sessionKey)
        {
            result = PInvoke.RmStartSession(
                &sessionHandle,
                0,
                (PWSTR)pKey
            );
        }

        if (result != Windows.Win32.Foundation.WIN32_ERROR.ERROR_SUCCESS)
            throw new Win32Exception((int)result, $"RmStartSession failed ({result})");

        return new RestartManager(sessionHandle, sessionKey);
    }

    /// <summary>Joins an existing Restart Manager session using the specified session key.</summary>
    /// <param name="sessionKey">The session key of an existing Restart Manager session.</param>
    /// <returns>A <see cref="RestartManager"/> instance representing the joined session.</returns>
    /// <exception cref="Win32Exception">Thrown when joining the session fails.</exception>
    public static RestartManager JoinSession(string sessionKey)
    {
        var result = PInvoke.RmJoinSession(out var handle, sessionKey);
        if (result != WIN32_ERROR.ERROR_SUCCESS)
            throw new Win32Exception((int)result, $"RmStartSession failed ({result})");

        return new RestartManager(handle, sessionKey);
    }

    /// <summary>Registers a file to be monitored by the Restart Manager session.</summary>
    /// <param name="path">The full path of the file to register.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="Win32Exception">Thrown when the registration fails.</exception>
    public unsafe void RegisterFile(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        string[] resources = [path];
        WIN32_ERROR result;
        fixed (char* p0 = resources[0])
        {
            char** pcwstr = stackalloc char*[1];
            pcwstr[0] = p0;

            PCWSTR* pResources = (PCWSTR*)pcwstr;
            result = PInvoke.RmRegisterResources(SessionHandle, (uint)resources.Length, pResources, 0, rgApplications: null, 0, rgsServiceNames: null);
        }

        if (result != WIN32_ERROR.ERROR_SUCCESS)
            throw new Win32Exception((int)result, $"RmRegisterResources failed ({result})");
    }

    /// <summary>Registers multiple files to be monitored by the Restart Manager session.</summary>
    /// <param name="paths">An array of full file paths to register.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="paths"/> is <see langword="null"/>.</exception>
    /// <exception cref="Win32Exception">Thrown when the registration fails.</exception>
    public unsafe void RegisterFiles(string[] paths)
    {
        ArgumentNullException.ThrowIfNull(paths);


        WIN32_ERROR result;
        fixed (char* p0 = paths[0])
        {
            char** pcwstr = stackalloc char*[1];
            pcwstr[0] = p0;

            PCWSTR* pPaths = (PCWSTR*)pcwstr;
            result = PInvoke.RmRegisterResources(SessionHandle, (uint)paths.LongLength, pPaths, 0, rgApplications: null, 0, rgsServiceNames: null);
        }


        if (result != WIN32_ERROR.ERROR_SUCCESS)
            throw new Win32Exception((int)result, $"RmRegisterResources failed ({result})");
    }

    /// <summary>Determines whether any of the registered resources are currently locked by running processes.</summary>
    /// <returns><see langword="true"/> if at least one registered resource is locked; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="Win32Exception">Thrown when the operation fails.</exception>
    public bool IsResourcesLocked()
    {
        uint arraySize = 1;
        while (true)
        {
            var array = new RM_PROCESS_INFO[arraySize];
            var result = PInvoke.RmGetList(SessionHandle, out var arrayCount, ref arraySize, array, out _);
            if (result is WIN32_ERROR.ERROR_SUCCESS or WIN32_ERROR.ERROR_MORE_DATA)
            {
                return arrayCount > 0;
            }

            throw new Win32Exception((int)result, $"RmGetList failed ({result})");
        }
    }

    /// <summary>Gets a list of processes that are currently locking the registered resources.</summary>
    /// <returns>A read-only list of <see cref="Process"/> instances that are locking the registered resources.</returns>
    /// <exception cref="Win32Exception">Thrown when the operation fails.</exception>
    public IReadOnlyList<Process> GetProcessesLockingResources()
    {
        uint arraySize = 10;
        while (true)
        {
            var array = new RM_PROCESS_INFO[arraySize];
            var result = PInvoke.RmGetList(SessionHandle, out var arrayCount, ref arraySize, array, out _);
            if (result == WIN32_ERROR.ERROR_SUCCESS)
            {
                var processes = new List<Process>((int)arrayCount);
                for (var i = 0; i < arrayCount; i++)
                {
                    try
                    {
                        var process = Process.GetProcessById((int)array[i].Process.dwProcessId);
                        if (process is not null)
                            processes.Add(process);
                    }
                    catch
                    {
                    }
                }

                return processes;
            }
            else if (result == WIN32_ERROR.ERROR_MORE_DATA)
            {
                arraySize = arrayCount;
            }
            else
            {
                throw new Win32Exception((int)result, $"RmGetList failed ({result})");
            }
        }
    }

    /// <summary>Shuts down applications and services that are using the registered resources.</summary>
    /// <param name="action">The shutdown options to use.</param>
    /// <exception cref="Win32Exception">Thrown when the shutdown operation fails.</exception>
    public unsafe void Shutdown(RestartManagerShutdownType action)
    {
        Shutdown(action, null);
    }

    /// <summary>Shuts down applications and services that are using the registered resources.</summary>
    /// <param name="action">The shutdown options to use.</param>
    /// <param name="statusCallback">An optional callback to receive progress updates during the shutdown operation.</param>
    /// <exception cref="Win32Exception">Thrown when the shutdown operation fails.</exception>
    public unsafe void Shutdown(RestartManagerShutdownType action, delegate* unmanaged[Stdcall]<uint, void> callback)
    {
        var result = PInvoke.RmShutdown(SessionHandle, (uint)action, callback);
        if (result != WIN32_ERROR.ERROR_SUCCESS)
            throw new Win32Exception((int)result, $"RmShutdown failed ({result})");
    }

    /// <summary>Restarts applications and services that were shut down by the Restart Manager and that were registered for restart.</summary>
    /// <exception cref="Win32Exception">Thrown when the restart operation fails.</exception>
    public unsafe void Restart()
    {
        Restart(null);
    }

    /// <summary>Restarts applications and services that were shut down by the Restart Manager and that were registered for restart.</summary>
    /// <param name="statusCallback">An optional callback to receive progress updates during the restart operation.</param>
    /// <exception cref="Win32Exception">Thrown when the restart operation fails.</exception>
    public unsafe void Restart(delegate* unmanaged[Stdcall]<uint, void> callback)
    {
        var result = PInvoke.RmRestart(SessionHandle, 0, callback);
        if (result != WIN32_ERROR.ERROR_SUCCESS)
            throw new Win32Exception((int)result, $"RmRestart failed ({result})");
    }

    /// <summary>Ends the Restart Manager session and releases all resources.</summary>
    /// <exception cref="Win32Exception">Thrown when ending the session fails.</exception>
    [SuppressMessage("Design", "CA1065:Do not raise exceptions in unexpected locations", Justification = "<Pending>")]
    public void Dispose()
    {
        if (SessionHandle != 0)
        {
            var result = PInvoke.RmEndSession(SessionHandle);
            if (result != WIN32_ERROR.ERROR_SUCCESS)
                throw new Win32Exception((int)result, $"RmEndSession failed ({result})");
        }
    }

    /// <summary>Determines whether the specified file is currently locked by any process.</summary>
    /// <param name="path">The full path of the file to check.</param>
    /// <returns><see langword="true"/> if the file is locked; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="Win32Exception">Thrown when the operation fails.</exception>
    public static bool IsFileLocked(string path)
    {
        using var restartManager = CreateSession();
        restartManager.RegisterFile(path);
        return restartManager.IsResourcesLocked();
    }

    /// <summary>Gets a list of processes that are currently locking the specified file.</summary>
    /// <param name="path">The full path of the file to check.</param>
    /// <returns>A read-only list of <see cref="Process"/> instances that are locking the file.</returns>
    /// <exception cref="Win32Exception">Thrown when the operation fails.</exception>
    public static IReadOnlyList<Process> GetProcessesLockingFile(string path)
    {
        using var restartManager = CreateSession();
        restartManager.RegisterFile(path);
        return restartManager.GetProcessesLockingResources();
    }
}
