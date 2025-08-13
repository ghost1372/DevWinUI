using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls.Primitives;
using WinRT;

namespace DevWinUI;
internal partial class PopupBackdropManager
{
    private MicaController _micaController;
    private DesktopAcrylicController _acrylicController;
    private SystemBackdropConfiguration _backdropConfig;

    public bool TrySetSystemBackdrop(Popup targetPopup, BackdropType backdropType)
    {
        DispatcherQueue.GetForCurrentThread().EnsureSystemDispatcherQueue();

        _backdropConfig = new SystemBackdropConfiguration { IsInputActive = true };

        targetPopup.Closed -= OnPopupClosed;
        targetPopup.Closed += OnPopupClosed;

        targetPopup.ActualThemeChanged += (_, __) => UpdateBackdropTheme(targetPopup);
        UpdateBackdropTheme(targetPopup);

        // Determine controller type and optional kind
        object? controller = null;

        switch (backdropType)
        {
            case BackdropType.Transparent:
            case BackdropType.Mica:
                if (!MicaController.IsSupported()) return false;
                controller = new MicaController();
                break;
            case BackdropType.MicaAlt:
                if (!MicaController.IsSupported()) return false;
                controller = new MicaController { Kind = MicaKind.BaseAlt };
                break;
            case BackdropType.DesktopAcrylic:
            case BackdropType.AcrylicBase:
                if (!DesktopAcrylicController.IsSupported()) return false;
                controller = new DesktopAcrylicController();
                break;
            case BackdropType.AcrylicThin:
                if (!DesktopAcrylicController.IsSupported()) return false;
                controller = new DesktopAcrylicController { Kind = DesktopAcrylicKind.Thin };
                break;
        }

        if (controller is MicaController mica)
        {
            _micaController = mica;
            _micaController.AddSystemBackdropTarget(targetPopup.As<ICompositionSupportsSystemBackdrop>());
            _micaController.SetSystemBackdropConfiguration(_backdropConfig);
        }
        else if (controller is DesktopAcrylicController acrylic)
        {
            _acrylicController = acrylic;
            _acrylicController.AddSystemBackdropTarget(targetPopup.As<ICompositionSupportsSystemBackdrop>());
            _acrylicController.SetSystemBackdropConfiguration(_backdropConfig);
        }

        return controller != null;
    }
    private void OnPopupClosed(object? sender, object e)
    {
        _micaController?.Dispose();
        _micaController = null;

        _acrylicController?.Dispose();
        _acrylicController = null;

        _backdropConfig = null;
    }

    private void UpdateBackdropTheme(Popup popup)
    {
        if (_backdropConfig == null) return;

        _backdropConfig.Theme = popup.ActualTheme switch
        {
            ElementTheme.Dark => SystemBackdropTheme.Dark,
            ElementTheme.Light => SystemBackdropTheme.Light,
            _ => SystemBackdropTheme.Default
        };
    }
}
