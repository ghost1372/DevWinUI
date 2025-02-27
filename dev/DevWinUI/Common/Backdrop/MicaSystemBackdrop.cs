using Microsoft.UI.Composition.SystemBackdrops;

namespace DevWinUI;
public sealed partial class MicaSystemBackdrop : SystemBackdrop
{
    public readonly MicaKind Kind;
    internal MicaController micaController;

    public SystemBackdropConfiguration BackdropConfiguration { get; private set; }

    private Color? tintColor;
    public Color? TintColor
    {
        get { return tintColor; }
        set
        {
            tintColor = value;
            if (micaController != null && value != null)
            {
                micaController.TintColor = (Color)value;
            }
        }
    }

    private float tintOpacity;
    public float TintOpacity
    {
        get { return tintOpacity; }
        set
        {
            tintOpacity = value;
            if (micaController != null)
            {
                micaController.TintOpacity = value;
            }
        }
    }

    private float luminosityOpacity;
    public float LuminosityOpacity
    {
        get { return luminosityOpacity; }
        set
        {
            luminosityOpacity = value;
            if (micaController != null)
            {
                micaController.LuminosityOpacity = value;
            }
        }
    }

    private Color? fallbackColor;
    public Color? FallbackColor
    {
        get { return fallbackColor; }
        set
        {
            fallbackColor = value;
            if (micaController != null && value != null)
            {
                micaController.FallbackColor = (Color)value;
            }
        }
    }

    public MicaSystemBackdrop() : this(MicaKind.Base)
    {
    }
    public MicaSystemBackdrop(MicaKind micaKind)
    {
        Kind = micaKind;
    }

    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop connectedTarget, XamlRoot xamlRoot)
    {
        base.OnTargetConnected(connectedTarget, xamlRoot);

        micaController = new MicaController() { Kind = this.Kind };
        micaController.AddSystemBackdropTarget(connectedTarget);
        BackdropConfiguration = GetDefaultSystemBackdropConfiguration(connectedTarget, xamlRoot);
        micaController.SetSystemBackdropConfiguration(BackdropConfiguration);
    }

    protected override void OnDefaultSystemBackdropConfigurationChanged(ICompositionSupportsSystemBackdrop target, XamlRoot xamlRoot)
    {
        if (target != null)
            base.OnDefaultSystemBackdropConfigurationChanged(target, xamlRoot);
    }

    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop disconnectedTarget)
    {
        base.OnTargetDisconnected(disconnectedTarget);

        if (micaController is not null)
        {
            micaController.RemoveSystemBackdropTarget(disconnectedTarget);
            micaController = null;
        }
    }
}
