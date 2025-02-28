using Microsoft.Win32;
using Windows.ApplicationModel;

namespace DevWinUI;

public static partial class StartupHelper
{
    public static string UnPackagedAppStartupTag { get; } = "/onBoot";

    private static readonly string UnPackagedAppRegistryKey = ProcessInfoHelper.ProductName;

    private const string RegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private const string ApprovalPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";

    private static readonly byte[] EnabledBinaryValue = [0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
    private static readonly byte[] DisabledBinaryValue = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];

    /// <summary>
    /// This property is intended solely for XAML binding to control app startup. 
    /// Do not use this property directly in your C# code.
    /// 
    /// The property retrieves the value from the <see cref="IsAppStartupWithWindowsEnabledAsync"/> method.
    /// When setting a value, it calls <see cref="EnableAppStartupWithWindowsAsync"/> or 
    /// <see cref="DisableAppStartupWithWindowsAsync"/> to enable or disable app startup accordingly.
    /// </summary>
    public static bool IsAppStartupWithWindowsForXamlBindingEnabled
    {
        get => Task.Run(async () => await IsAppStartupWithWindowsEnabledAsync()).Result;
        set
        {
            if (value)
            {
                Task.Run(async () => await EnableAppStartupWithWindowsAsync()).Wait();
            }
            else
            {
                Task.Run(async () => await DisableAppStartupWithWindowsAsync()).Wait();
            }
        }
    }

    /// <summary>
    /// Disables the application startup with Windows.
    /// For Packaged applications, the taskId is automatically used from the first StartupTask defined in Package.appxmanifest file.
    /// For UnPackaged applications, Registry.CurrentUser Will be used
    /// </summary>
    /// <returns></returns>
    public static async Task<bool> DisableAppStartupWithWindowsAsync()
    {
        return await SetStartupAsync(false, null, Registry.CurrentUser);
    }

    /// <summary>
    /// Disables the application startup with Windows.
    /// For Packaged applications, the taskId is automatically used from the first StartupTask defined in Package.appxmanifest file.
    /// </summary>
    /// <param name="registryKey">Define registry scope (CurrentUser or LocalMachine) for UnPackaged App</param>
    /// <returns></returns>
    public static async Task<bool> DisableAppStartupWithWindowsAsync(RegistryKey registryKey)
    {
        return await SetStartupAsync(false, null, registryKey);
    }

    /// <summary>
    /// Disables the application startup with Windows.
    /// For UnPackaged applications, Registry.CurrentUser Will be used
    /// </summary>
    /// <param name="taskId">Set a TaskId for a packaged application.</param>
    /// <returns></returns>
    public static async Task<bool> DisableAppStartupWithWindowsAsync(string taskId)
    {
        return await SetStartupAsync(false, taskId, Registry.CurrentUser);
    }

    /// <summary>
    /// Disables the application startup with Windows.
    /// </summary>
    /// <param name="taskId">Set a TaskId for a packaged application.</param>
    /// <param name="registryKey">Define registry scope (CurrentUser or LocalMachine) for UnPackaged App</param>
    /// <returns></returns>
    public static async Task<bool> DisableAppStartupWithWindowsAsync(string taskId, RegistryKey registryKey)
    {
        return await SetStartupAsync(false, taskId, registryKey);
    }

    /// <summary>
    /// Enables the application startup with Windows.
    /// For Packaged applications, the taskId is automatically used from the first StartupTask defined in Package.appxmanifest file.
    /// For UnPackaged applications, Registry.CurrentUser Will be used
    /// </summary>
    /// <returns></returns>
    public static async Task<bool> EnableAppStartupWithWindowsAsync()
    {
        return await SetStartupAsync(true, null, Registry.CurrentUser);
    }

    /// <summary>
    /// Enables the application startup with Windows.
    /// For Packaged applications, the taskId is automatically used from the first StartupTask defined in Package.appxmanifest file.
    /// </summary>
    /// <param name="registryKey">Define registry scope (CurrentUser or LocalMachine) for UnPackaged App</param>
    /// <returns></returns>
    public static async Task<bool> EnableAppStartupWithWindowsAsync(RegistryKey registryKey)
    {
        return await SetStartupAsync(true, null, registryKey);
    }

    /// <summary>
    /// Enables the application startup with Windows.
    /// For UnPackaged applications, Registry.CurrentUser Will be used
    /// </summary>
    /// <param name="taskId">Set a TaskId for a packaged application.</param>
    /// <returns></returns>
    public static async Task<bool> EnableAppStartupWithWindowsAsync(string taskId)
    {
        return await SetStartupAsync(true, taskId, Registry.CurrentUser);
    }

    /// <summary>
    /// Enables the application startup with Windows.
    /// </summary>
    /// <param name="taskId">Set a TaskId for a packaged application.</param>
    /// <param name="registryKey">Define registry scope (CurrentUser or LocalMachine) for UnPackaged App</param>
    /// <returns></returns>
    public static async Task<bool> EnableAppStartupWithWindowsAsync(string taskId, RegistryKey registryKey)
    {
        return await SetStartupAsync(true, taskId, registryKey);
    }

    /// <summary>
    /// Checks if the application is configured to run at startup.
    /// For Packaged applications, the taskId is automatically used from the first StartupTask defined in Package.appxmanifest file.
    /// For UnPackaged applications, Registry.CurrentUser Will be used
    /// </summary>
    /// <returns></returns>
    public static async Task<bool> IsAppStartupWithWindowsEnabledAsync()
    {
        return await IsAppStartupWithWindowsEnabledAsync(null, Registry.CurrentUser);
    }

    /// <summary>
    /// Checks if the application is configured to run at startup.
    /// For Packaged applications, the taskId is automatically used from the first StartupTask defined in Package.appxmanifest file.
    /// </summary>
    /// <param name="registryKey">Define registry scope (CurrentUser or LocalMachine) for UnPackaged App</param>
    /// <returns></returns>
    public static async Task<bool> IsAppStartupWithWindowsEnabledAsync(RegistryKey registryKey)
    {
        return await IsAppStartupWithWindowsEnabledAsync(null, registryKey);
    }

    /// <summary>
    /// Checks if the application is configured to run at startup.
    /// For UnPackaged applications, Registry.CurrentUser Will be used
    /// </summary>
    /// <param name="taskId">Set a TaskId for a packaged application.</param>
    /// <returns></returns>
    public static async Task<bool> IsAppStartupWithWindowsEnabledAsync(string taskId)
    {
        return await IsAppStartupWithWindowsEnabledAsync(taskId, Registry.CurrentUser);
    }

    /// <summary>
    /// Checks if the application is configured to run at startup.
    /// </summary>
    /// <param name="taskId">Set a TaskId for a packaged application.</param>
    /// <param name="registryKey">Define registry scope (CurrentUser or LocalMachine) for UnPackaged App</param>
    /// <returns></returns>
    public static async Task<bool> IsAppStartupWithWindowsEnabledAsync(string taskId, RegistryKey registryKey)
    {
        if (PackageHelper.IsPackaged)
        {
            StartupTask startupTask = null;
            if (string.IsNullOrEmpty(taskId))
            {
                var taskList = await StartupTask.GetForCurrentPackageAsync();
                if (taskList.Count > 0)
                {
                    startupTask = taskList.FirstOrDefault();
                }
            }
            else
            {
                startupTask = await StartupTask.GetAsync(taskId);
            }

            if (startupTask == null)
            {
                throw new Exception("Couldn't find a StartupTask in the appx manifest with the input taskId");
            }

            return startupTask.State == StartupTaskState.Enabled || startupTask.State == StartupTaskState.EnabledByPolicy;
        }
        else
        {
            return CheckAndGetStartupRegistryKey(registryKey);
        }
    }

    private static async Task<bool> SetStartupAsync(bool startup, string taskId, RegistryKey registryKey)
    {
        if (PackageHelper.IsPackaged)
        {
            StartupTask startupTask = null;
            if (string.IsNullOrEmpty(taskId))
            {
                var taskList = await StartupTask.GetForCurrentPackageAsync();
                if (taskList.Count > 0)
                {
                    startupTask = taskList.FirstOrDefault();
                }
            }
            else
            {
                startupTask = await StartupTask.GetAsync(taskId);
            }

            if (startupTask == null)
            {
                throw new Exception("Couldn't find a StartupTask in the appx manifest with the input taskId");
            }

            switch (startupTask.State)
            {
                case StartupTaskState.Disabled:
                    if (startup)
                    {
                        return await startupTask.RequestEnableAsync() == StartupTaskState.Enabled;
                    }
                    break;
                case StartupTaskState.DisabledByUser:
                    if (startup)
                    {
                        // TerminalTODO: GH#6254: define UX for other StartupTaskStates
                        // Reference: terminal_main\src\cascadia\TerminalSettingsEditor\AppLogic.cpp
                    }
                    break;
                case StartupTaskState.Enabled:
                    if (!startup)
                    {
                        startupTask.Disable();
                    }
                    break;
                case StartupTaskState.EnabledByPolicy:
                    if (!startup)
                    {
                        return false;
                    }
                    break;
                case StartupTaskState.DisabledByPolicy:
                    if (startup)
                    {
                        return false;
                    }
                    break;
            }
        }
        else
        {
            var state = await IsAppStartupWithWindowsEnabledAsync(registryKey);
            if (!state && startup)
            {
                return SetStartupRegistryKey(startup, registryKey);
            }
            else if (state && !startup)
            {
                return SetStartupRegistryKey(startup, registryKey);
            }
        }
        return true;
    }
    private static bool CheckAndGetStartupRegistryKey(RegistryKey registryKey)
    {
        if (Environment.ProcessPath is not string appPath)
        {
            return false;
        }

        var root = registryKey;
        try
        {
            var startup = false;
            var path = root.OpenSubKey(RegistryPath, true);
            if (path == null)
            {
                path = root.CreateSubKey(RegistryPath);
            }
            var keyNames = path.GetValueNames();

            foreach (var keyName in keyNames)
            {
                if (keyName.Equals(UnPackagedAppRegistryKey, StringComparison.CurrentCultureIgnoreCase))
                {
                    startup = true;
                    if (startup)
                    {
                        var value = path.GetValue(keyName)!.ToString()!;
                        if (!value.Contains(@appPath, StringComparison.CurrentCultureIgnoreCase))
                        {
                            path.SetValue(UnPackagedAppRegistryKey, $@"""{@appPath}"" {UnPackagedAppStartupTag}");
                            path.Close();
                            path = root.OpenSubKey(ApprovalPath, true);
                            if (path != null)
                            {
                                path.SetValue(UnPackagedAppRegistryKey, EnabledBinaryValue);
                                path.Close();
                            }
                        }
                    }
                    break;
                }
            }
            if (startup)
            {
                path?.Close();
                path = root.OpenSubKey(ApprovalPath, false);
                if (path != null)
                {
                    keyNames = path.GetValueNames();
                    foreach (var keyName in keyNames)
                    {
                        if (keyName.Equals(UnPackagedAppRegistryKey, StringComparison.CurrentCultureIgnoreCase))
                        {
                            var value = (byte[])path.GetValue(keyName)!;
                            if (!(value.SequenceEqual(EnabledBinaryValue) || value.SequenceEqual(DisabledBinaryValue)))
                            {
                                startup = false;
                            }
                            break;
                        }
                    }
                }
            }
            path?.Close();
            return startup;
        }
        catch
        {
            return false;
        }
    }
    private static bool SetStartupRegistryKey(bool startup, RegistryKey registryKey)
    {
        if (Environment.ProcessPath is not string appPath)
        {
            return false;
        }

        var root = registryKey;
        var value = $@"""{@appPath}"" {UnPackagedAppStartupTag}";
        try
        {
            var path = root.OpenSubKey(RegistryPath, true);
            if (path == null)
            {
                path = root.CreateSubKey(RegistryPath);
            }

            if (startup)
            {
                path.SetValue(UnPackagedAppRegistryKey, value);
                path.Close();
                path = root.OpenSubKey(ApprovalPath, true);
                if (path != null)
                {
                    path.SetValue(UnPackagedAppRegistryKey, EnabledBinaryValue);
                    path.Close();
                }
            }
            else
            {
                var keyNames = path.GetValueNames();
                foreach (var keyName in keyNames)
                {
                    if (keyName.Equals(UnPackagedAppRegistryKey, StringComparison.CurrentCultureIgnoreCase))
                    {
                        path.DeleteValue(UnPackagedAppRegistryKey);
                        path.Close();
                        break;
                    }
                }
                path = root.OpenSubKey(ApprovalPath, true);
                if (path != null)
                {
                    path.DeleteValue(UnPackagedAppRegistryKey);
                    path.Close();
                }
            }
            path?.Close();
        }
        catch
        {
            return false;
        }
        return true;
    }
}
