using System.Runtime.InteropServices;
using Windows.Graphics.DirectX.Direct3D11;

namespace DevWinUI;

internal sealed partial class DeviceLostEventArgs : EventArgs
{
    public IDirect3DDevice Device { get; }

    internal DeviceLostEventArgs(IDirect3DDevice device)
    {
        Device = device;
    }

    internal static DeviceLostEventArgs Create(IDirect3DDevice device)
    {
        return new DeviceLostEventArgs(device);
    }
}

internal delegate void DeviceLostEventHandler(DeviceLostHelper sender, DeviceLostEventArgs args);

internal sealed partial class DeviceLostHelper : IDisposable
{
    private IDirect3DDevice _device;
    private Task _monitorTask;
    private CancellationTokenSource _cts;
    private IntPtr _cookie;
    private ManualResetEvent _deviceRemovedEvent;

    public event DeviceLostEventHandler DeviceLost;

    public IDirect3DDevice CurrentlyWatchedDevice => _device;

    public DeviceLostHelper()
    {
    }

    public void WatchDevice(IDirect3DDevice device)
    {
        StopWatchingCurrentDevice();

        _device = device;

        _deviceRemovedEvent = new ManualResetEvent(false);
        _cts = new CancellationTokenSource();

        _monitorTask = Task.Run(() => MonitorDeviceRemovedAsync(device, _cts.Token));
    }

    public void StopWatchingCurrentDevice()
    {
        try
        {
            _cts?.Cancel();
            _monitorTask?.Wait(50);

            if (_device != null && _cookie != IntPtr.Zero)
            {
                var nativeDevice = Marshal.GetIUnknownForObject(_device);
                var d3d11Device4 = Marshal.GetObjectForIUnknown(nativeDevice) as ID3D11Device4Native;
                Marshal.Release(nativeDevice);

                d3d11Device4?.UnregisterDeviceRemoved(_cookie);
                _cookie = IntPtr.Zero;
            }
        }
        catch { }
        finally
        {
            _deviceRemovedEvent?.Dispose();
            _deviceRemovedEvent = null;

            _cts?.Dispose();
            _cts = null;

            _device = null;
        }
    }

    private async Task MonitorDeviceRemovedAsync(IDirect3DDevice device, CancellationToken token)
    {
        try
        {
            var nativeDevice = Marshal.GetIUnknownForObject(device);
            var d3d11Device4 = Marshal.GetObjectForIUnknown(nativeDevice) as ID3D11Device4Native;
            Marshal.Release(nativeDevice);

            if (d3d11Device4 != null)
            {
                _cookie = d3d11Device4.RegisterDeviceRemoved(_deviceRemovedEvent.SafeWaitHandle.DangerousGetHandle());

                while (!token.IsCancellationRequested)
                {
                    if (_deviceRemovedEvent.WaitOne(10))
                    {
                        OnDeviceLost(device);
                        break;
                    }
                    await Task.Delay(10, token);
                }

                d3d11Device4.UnregisterDeviceRemoved(_cookie);
                _cookie = IntPtr.Zero;
            }
        }
        catch { }
    }

    private void OnDeviceLost(IDirect3DDevice oldDevice)
    {
        StopWatchingCurrentDevice();
        DeviceLost?.Invoke(this, DeviceLostEventArgs.Create(oldDevice));
    }

    public void Dispose()
    {
        StopWatchingCurrentDevice();
    }

    // WinRT Native interface for ID3D11Device4
    [ComImport, Guid("3B1A6840-16F2-4F46-8E0D-2BEE8CA5F978"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface ID3D11Device4Native
    {
        IntPtr RegisterDeviceRemoved(IntPtr eventHandle);
        void UnregisterDeviceRemoved(IntPtr cookie);
    }
}