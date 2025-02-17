﻿using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;

namespace DevWinUI;
public sealed partial class AcrylicSystemBackdrop : SystemBackdrop
{
    public readonly static Color Default_TintColor_Dark = ColorHelper.GetColorFromHex("#FF202020");
    public readonly static Color Default_TintColor_Light = ColorHelper.GetColorFromHex("#FFF3F3F3");
    public readonly static Color Default_TintColor_MicaAlt_Dark = ColorHelper.GetColorFromHex("#FF0A0A0A");
    public readonly static Color Default_TintColor_MicaAlt_Light = ColorHelper.GetColorFromHex("#FFDADADA");

    public readonly DesktopAcrylicKind Kind;
    internal DesktopAcrylicController acrylicController;

    public SystemBackdropConfiguration BackdropConfiguration { get; private set; }

    private Color _color;
    public Color TintColor
    {
        get { return _color; }
        set
        {
            _color = value;
            if (acrylicController != null)
            {
                acrylicController.TintColor = value;
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
            if (acrylicController != null)
            {
                acrylicController.TintOpacity = value;
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
            if (acrylicController != null)
            {
                acrylicController.LuminosityOpacity = value;
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
            if (acrylicController != null)
            {
                acrylicController.FallbackColor = value;
            }
        }
    }

    public AcrylicSystemBackdrop() : this(DesktopAcrylicKind.Default)
    {
        
    }
    public AcrylicSystemBackdrop(DesktopAcrylicKind desktopAcrylicKind)
    {
        Kind = desktopAcrylicKind;
    }

    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop connectedTarget, XamlRoot xamlRoot)
    {
        base.OnTargetConnected(connectedTarget, xamlRoot);

        acrylicController = new DesktopAcrylicController() { Kind = this.Kind };
        acrylicController.AddSystemBackdropTarget(connectedTarget);
        BackdropConfiguration = GetDefaultSystemBackdropConfiguration(connectedTarget, xamlRoot);
        acrylicController.SetSystemBackdropConfiguration(BackdropConfiguration);
    }

    protected override void OnDefaultSystemBackdropConfigurationChanged(ICompositionSupportsSystemBackdrop target, XamlRoot xamlRoot)
    {
        if (target != null)
            base.OnDefaultSystemBackdropConfigurationChanged(target, xamlRoot);
    }

    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop disconnectedTarget)
    {
        base.OnTargetDisconnected(disconnectedTarget);

        if (acrylicController is not null)
        {
            acrylicController.RemoveSystemBackdropTarget(disconnectedTarget);
            acrylicController = null;
        }
    }
}
