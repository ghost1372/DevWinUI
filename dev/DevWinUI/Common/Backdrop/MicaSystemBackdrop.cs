using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;

namespace DevWinUI;
public sealed partial class MicaSystemBackdrop : SystemBackdrop
{
    public readonly static Color Default_TintColor_Dark = Color.FromArgb(0xff, 0x20, 0x20, 0x20);
    public readonly static Color Default_TintColor_Light = Color.FromArgb(0xff, 0xf3, 0xf3, 0xf3);
    public readonly static Color Default_TintColor_MicaAlt_Dark = Color.FromArgb(0xff, 0x0a, 0x0a, 0x0a);
    public readonly static Color Default_TintColor_MicaAlt_Light = Color.FromArgb(0xff, 0xda, 0xda, 0xda);
    public readonly static Color Default_FallbackColor_MicaAlt_Dark = Color.FromArgb(0xff, 0x20, 0x20, 0x20);
    public readonly static Color Default_FallbackColor_MicaAlt_Light = Color.FromArgb(0xff, 0xe8, 0xe8, 0xe8);

    public readonly MicaKind Kind;
    private MicaController micaController;

    public SystemBackdropConfiguration BackdropConfiguration { get; private set; }

    private Color tintColor;
    public Color TintColor
    {
        get { return tintColor; }
        set
        {
            tintColor = value;
            if (micaController != null)
            {
                micaController.TintColor = value;
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

    private Color fallbackColor;
    public Color FallbackColor
    {
        get { return fallbackColor; }
        set
        {
            fallbackColor = value;
            if (micaController != null)
            {
                micaController.FallbackColor = value;
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
