using Microsoft.UI.Dispatching;

namespace DevWinUI;

public abstract partial class CompositionBrushBackdrop : SystemBackdrop
{
    static Windows.UI.Composition.Compositor? compositor;
    static object compositorLock = new object();
    internal static Windows.UI.Composition.Compositor Compositor
    {
        get
        {
            if (compositor == null)
            {
                lock (compositorLock)
                {
                    if (compositor == null)
                    {
                        DispatcherQueue.GetForCurrentThread().EnsureSystemDispatcherQueue();
                        compositor = new Windows.UI.Composition.Compositor();
                    }
                }
            }
            return compositor;
        }
    }
    public CompositionBrushBackdrop()
    {
    }
    protected abstract Windows.UI.Composition.CompositionBrush CreateBrush(Windows.UI.Composition.Compositor compositor);

    protected override void OnDefaultSystemBackdropConfigurationChanged(ICompositionSupportsSystemBackdrop target, XamlRoot xamlRoot)
    {
        if (target != null)
            base.OnDefaultSystemBackdropConfigurationChanged(target, xamlRoot);
    }

    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop connectedTarget, XamlRoot xamlRoot)
    {
        connectedTarget.SystemBackdrop = CreateBrush(Compositor);
        base.OnTargetConnected(connectedTarget, xamlRoot);
    }

    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop disconnectedTarget)
    {
        var backdrop = disconnectedTarget.SystemBackdrop;
        disconnectedTarget.SystemBackdrop = null;
        backdrop?.Dispose();
        base.OnTargetDisconnected(disconnectedTarget);
    }
}
