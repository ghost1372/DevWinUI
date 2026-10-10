using Windows.Win32.System.Threading;

namespace DevWinUI;

/// <summary>
/// Provides helper methods for enabling EcoQoS and process efficiency mode.
/// </summary>
public static unsafe partial class EcoQoSHelper
{
    private static void EnableEfficiencyMode()
    {
        EnableEfficiencyMode(EcoQosProcessPriority.IDLE_PRIORITY_CLASS);
    }
    public static void EnableEfficiencyMode(EcoQosProcessPriority ecoQosProcessPriority)
    {
        PROCESS_POWER_THROTTLING_STATE powerThrottling = new()
        {
            ControlMask = PInvoke.PROCESS_POWER_THROTTLING_EXECUTION_SPEED,
            StateMask = PInvoke.PROCESS_POWER_THROTTLING_EXECUTION_SPEED,
            Version = PInvoke.PROCESS_POWER_THROTTLING_CURRENT_VERSION
        };

        _ = PInvoke.SetProcessInformation(PInvoke.GetCurrentProcess(), PROCESS_INFORMATION_CLASS.ProcessPowerThrottling, &powerThrottling, (uint)sizeof(PROCESS_POWER_THROTTLING_STATE));

        _ = PInvoke.SetPriorityClass(PInvoke.GetCurrentProcess(), (PROCESS_CREATION_FLAGS)ecoQosProcessPriority);
    }

    public static void DisableEfficiencyMode()
    {
        DisableEfficiencyMode(EcoQosProcessPriority.NORMAL_PRIORITY_CLASS);
    }
    public static void DisableEfficiencyMode(EcoQosProcessPriority ecoQosProcessPriority)
    {
        PROCESS_POWER_THROTTLING_STATE powerThrottling = new()
        {
            ControlMask = PInvoke.PROCESS_POWER_THROTTLING_EXECUTION_SPEED,
            StateMask = 0,
            Version = PInvoke.PROCESS_POWER_THROTTLING_CURRENT_VERSION
        };

        _ = PInvoke.SetProcessInformation(PInvoke.GetCurrentProcess(), PROCESS_INFORMATION_CLASS.ProcessPowerThrottling, &powerThrottling, (uint)sizeof(PROCESS_POWER_THROTTLING_STATE));

        _ = PInvoke.SetPriorityClass(PInvoke.GetCurrentProcess(), (PROCESS_CREATION_FLAGS)ecoQosProcessPriority);
    }
}
