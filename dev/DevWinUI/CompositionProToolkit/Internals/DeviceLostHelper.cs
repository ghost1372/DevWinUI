using System.Runtime.InteropServices;
using Windows.Graphics.DirectX.Direct3D11;
using WinRT;

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

internal sealed unsafe partial class DeviceLostHelper : IDisposable
{
    private static readonly Guid IID_DxgiInterfaceAccess = new("A9B3D012-3DF2-4EE3-B8D1-8695F457D3C1");
    private static readonly Guid IID_ID3D11Device4 = new("8992AB71-02E6-4B8D-BA48-B056DCDA42C4");

    private const int RegisterDeviceRemovedEventIndex = 65;
    private const int UnregisterDeviceRemovedIndex = 66;

    private readonly object _lock = new();
    private IDirect3DDevice? _device;
    private nint _d3dDevice4;
    private uint _cookie;
    private ManualResetEvent? _removedEvent;
    private RegisteredWaitHandle? _waitHandle;

    public event DeviceLostEventHandler? DeviceLost;

    public IDirect3DDevice? CurrentlyWatchedDevice => _device;

    public void WatchDevice(IDirect3DDevice device)
    {
        lock (_lock)
        {
            StopWatchingCurrentDevice();

            nint abi = 0, access = 0, d3d = 0;
            ManualResetEvent? removedEvent = null;

            try
            {
                abi = MarshalInterface<IDirect3DDevice>.FromManaged(device);
                Marshal.ThrowExceptionForHR(Marshal.QueryInterface(abi, in IID_DxgiInterfaceAccess, out access));

                var iid = IID_ID3D11Device4;
                int hr = ((delegate* unmanaged[Stdcall]<nint, Guid*, nint*, int>)(*(void***)access)[3])(access, &iid, &d3d);
                Marshal.ThrowExceptionForHR(hr);

                removedEvent = new ManualResetEvent(false);
                uint cookie;
                hr = ((delegate* unmanaged[Stdcall]<nint, nint, uint*, int>)(*(void***)d3d)[RegisterDeviceRemovedEventIndex])(
                    d3d, removedEvent.SafeWaitHandle.DangerousGetHandle(), &cookie);
                Marshal.ThrowExceptionForHR(hr);

                _device = device;
                _d3dDevice4 = d3d;
                _cookie = cookie;
                _removedEvent = removedEvent;
                d3d = 0;
                removedEvent = null;

                _waitHandle = ThreadPool.RegisterWaitForSingleObject(
                    _removedEvent,
                    static (state, _) => ((DeviceLostHelper)state!).OnDeviceLost(),
                    this,
                    Timeout.Infinite,
                    true);
            }
            catch
            {
            }
            finally
            {
                if (d3d != 0) Marshal.Release(d3d);
                if (access != 0) Marshal.Release(access);
                if (abi != 0) Marshal.Release(abi);
                removedEvent?.Dispose();
            }
        }
    }

    public void StopWatchingCurrentDevice()
    {
        lock (_lock)
        {
            _waitHandle?.Unregister(null);
            _waitHandle = null;

            if (_d3dDevice4 != 0)
            {
                try
                {
                    ((delegate* unmanaged[Stdcall]<nint, uint, void>)(*(void***)_d3dDevice4)[UnregisterDeviceRemovedIndex])(_d3dDevice4, _cookie);
                }
                catch
                {
                }

                Marshal.Release(_d3dDevice4);
                _d3dDevice4 = 0;
                _cookie = 0;
            }

            _removedEvent?.Dispose();
            _removedEvent = null;
            _device = null;
        }
    }

    private void OnDeviceLost()
    {
        IDirect3DDevice? oldDevice;

        lock (_lock)
        {
            oldDevice = _device;
            if (oldDevice == null) return;
            StopWatchingCurrentDevice();
        }

        DeviceLost?.Invoke(this, DeviceLostEventArgs.Create(oldDevice));
    }

    public void Dispose()
    {
        StopWatchingCurrentDevice();
    }
}
