using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Windows.ApplicationModel;
using Windows.Win32.System.Memory;

namespace DevWinUI;

[Experimental(diagnosticId:"DEVWINUI000", Message = "This API is experimental and may change or be removed in future versions.")]
public static unsafe partial class SystemCompositionHelper
{
    private const int ExpectedState = 2;
    private const int SystemState = 1;
    private const int PageReadWrite = 0x04;
    private const long SwitcherStateRva = 0x2D5F80;
    private const long ExpectedImageSize = 3_204_920;
    private const string ExpectedFileVersion = "10.0.27200.1038 (WinBuild.160101.0800)";

    public static bool TryUnlockLAFAndSetSystemEngine()
    {
        var featureId = "com.microsoft.windows.composition.engine";
        var token = LimitedAccessFeatureTokenGenerator.GenerateTokenFromFeatureId(featureId);
        var attestation = LimitedAccessFeatureTokenGenerator.GenerateAttestation(featureId);

        var accessResult = LimitedAccessFeatures.TryUnlockFeature(featureId, token, attestation);
        return CompositionEngine.TrySetProcessEngine(CompositionEngineType.System);
    }

    public static bool TryBypassLAFAndSetSystemEngine()
    {
        bool initialResult = CompositionEngine.TrySetProcessEngine(CompositionEngineType.System);
        Log($"Initial TrySetProcessEngine(System): {initialResult}");

        if (initialResult)
        {
            return true;
        }

        if (!OperatingSystem.IsWindows() || IntPtr.Size != 8)
        {
            Log($"Unsupported process: Windows={OperatingSystem.IsWindows()}, PointerSize={IntPtr.Size}");
            return false;
        }

        HMODULE module;

        fixed (char* name = "dcompi.dll")
        {
            module = PInvoke.GetModuleHandle(new PCWSTR(name));
        }

        nint moduleAddress = (nint)module.Value;

        Log($"dcompi.dll module handle: 0x{moduleAddress:X}");

        if (moduleAddress == 0)
        {
            Log("dcompi.dll was not loaded");
            return false;
        }

        if (!IsExpectedModule(module))
        {
            Log("dcompi.dll validation failed");
            return false;
        }

        nint stateAddress = moduleAddress + (int)SwitcherStateRva;
        int currentState = *(int*)stateAddress;

        Log($"g_switcherState address: 0x{stateAddress:X}, value: {currentState}");

        if (currentState != ExpectedState)
        {
            Log($"Expected g_switcherState to be {ExpectedState}, found {currentState}");
            return false;
        }

        if (!PInvoke.VirtualProtect((void*)stateAddress, (nuint)sizeof(int), PAGE_PROTECTION_FLAGS.PAGE_READWRITE, out PAGE_PROTECTION_FLAGS oldProtection))
        {
            Log($"VirtualProtect(write) failed: {Marshal.GetLastWin32Error()}");
            return false;
        }

        try
        {
            *(int*)stateAddress = SystemState;
            Log($"g_switcherState patched to {SystemState}");
        }
        finally
        {
            bool restored = PInvoke.VirtualProtect((void*)stateAddress, (nuint)sizeof(int), oldProtection, out _);

            Log(restored
                ? "Original memory protection restored"
                : $"VirtualProtect(restore) failed: {Marshal.GetLastWin32Error()}");
        }

        bool retryResult = CompositionEngine.TrySetProcessEngine(CompositionEngineType.System);
        Log($"Retry TrySetProcessEngine(System): {retryResult}");

        return retryResult;
    }

    private static bool IsExpectedModule(HMODULE module)
    {
        Span<char> pathBuffer = stackalloc char[32768];

        fixed (char* path = pathBuffer)
        {
            uint pathLength = PInvoke.GetModuleFileName(module, new PWSTR(path), (uint)pathBuffer.Length);

            if (pathLength == 0 || pathLength >= pathBuffer.Length)
            {
                Log($"GetModuleFileName failed: {Marshal.GetLastWin32Error()}");
                return false;
            }

            try
            {
                string modulePath = new(path, 0, (int)pathLength);

                FileInfo file = new(modulePath);
                FileVersionInfo version = FileVersionInfo.GetVersionInfo(file.FullName);

                bool matches = file.Length == ExpectedImageSize && string.Equals(version.FileVersion, ExpectedFileVersion, StringComparison.Ordinal);

                Log($"dcompi.dll path: {modulePath}");
                Log($"dcompi.dll version: {version.FileVersion}");
                Log($"dcompi.dll size: {file.Length}; expected: {ExpectedImageSize}; matches: {matches}");

                return matches;
            }
            catch (IOException)
            {
                Log("dcompi.dll validation failed with an I/O error");
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                Log("dcompi.dll validation failed with an access error");
                return false;
            }
            catch (ArgumentException)
            {
                Log("dcompi.dll validation failed with an argument error");
                return false;
            }
        }
    }
    private static void Log(string message)
    {
        string output = $"[SystemCompositionHack] {message}";
        Debug.WriteLine(output);
    }
}
