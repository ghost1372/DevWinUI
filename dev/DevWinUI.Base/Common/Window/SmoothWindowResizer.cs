using Microsoft.UI.Composition.SystemBackdrops;
using Windows.UI.Composition.Desktop;
using WinRT;

namespace DevWinUI;

public partial class SmoothWindowResizer
{
    private DesktopWindowTarget target;
    private ISystemBackdropControllerWithTargets controller;
    private WindowId windowId;
    public SmoothWindowResizer(Window window)
    {
        windowId = window.AppWindow.Id;

        window.SystemBackdrop = new TransparentBackdrop();

        window.DispatcherQueue.EnsureSystemDispatcherQueue();
        var compositor = new Windows.UI.Composition.Compositor();
        var interop = compositor.As<ICompositorDesktopInterop>();
        interop.CreateDesktopWindowTarget((nint)windowId.Value, isTopmost: false, out IntPtr targetPtr);

        target = DesktopWindowTarget.FromAbi(targetPtr);
        target.Root = compositor.CreateContainerVisual();

        controller = new MicaController();
        controller.SetTarget(windowId, target);
    }

    public ISystemBackdropControllerWithTargets GetController()
    {
        return controller;
    }

    public DesktopWindowTarget GetTarget()
    {
        return target; 
    }

    /// <summary>
    /// None and Transparent does not supported
    /// </summary>
    /// <param name="backdropType"></param>
    public void ChangeBackdrop(BackdropType backdropType)
    {
        controller.RemoveAllSystemBackdropTargets();
        controller.Dispose();

        controller = backdropType switch
        {
            BackdropType.MicaAlt => new MicaController { Kind = MicaKind.BaseAlt },
            BackdropType.Acrylic => new DesktopAcrylicController(),
            BackdropType.AcrylicThin => new DesktopAcrylicController() { Kind = DesktopAcrylicKind.Thin },
            BackdropType.Mica or _ => new MicaController()
        };
        controller.SetTarget(windowId, target);
    }
}

[ComImport]
[Guid("29E691FA-4567-4DCA-B319-D0F207EB6807")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICompositorDesktopInterop
{
    void CreateDesktopWindowTarget(IntPtr hwndTarget, bool isTopmost, out IntPtr result);
}
