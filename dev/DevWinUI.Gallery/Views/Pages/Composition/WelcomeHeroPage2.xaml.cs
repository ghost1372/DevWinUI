namespace DevWinUIGallery.Views;

public sealed partial class WelcomeHeroPage2 : Page
{
    public static Action<string> NavigateToModuleCallback { get; set; }

    private const string WelcomeAssetFolder = "ms-appx:///Assets/Modules/OOBE/Welcome/";
    public WelcomeHeroPage2()
    {
        InitializeComponent();

        NavigateToModuleCallback = NavigateToModule;

        Hero.Playback = WelcomeHeroPage.WelcomeIntroPlayed ? WelcomeHeroPlayback.Settle : WelcomeHeroPlayback.Intro;
        WelcomeHeroPage.WelcomeIntroPlayed = true;

        Hero.LogoAsset = $"ms-appx:///Assets/AppIcon.png";

        Hero.Modules =
        [
            new($"{WelcomeAssetFolder}CommandPalette.png", "CmdPal", "Command Palette"),
            new($"{WelcomeAssetFolder}AdvancedPaste.png", "AdvancedPaste", "Advanced Paste"),
            new($"{WelcomeAssetFolder}FancyZones.png", "FancyZones", "FancyZones"),
            new($"{WelcomeAssetFolder}ColorPicker.png", "ColorPicker", "Color Picker"),
            new($"{WelcomeAssetFolder}PowerRename.png", "PowerRename", "PowerRename"),
            new($"{WelcomeAssetFolder}Peek.png", "Peek", "Peek"),
            new($"{WelcomeAssetFolder}Workspaces.png", "Workspaces", "Workspaces"),
            new($"{WelcomeAssetFolder}LightSwitch.png", "LightSwitch", "Light Switch"),
            new($"{WelcomeAssetFolder}TextExtractor.png", "TextExtractor", "Text Extractor"),
            new($"{WelcomeAssetFolder}ZoomIt.png", "ZoomIt", "ZoomIt"),
            new($"{WelcomeAssetFolder}KeyboardManager.png", "KBM", "Keyboard Manager"),
            new($"{WelcomeAssetFolder}EnvironmentVariables.png", "EnvironmentVariables", "Environment Variables"),
            new($"{WelcomeAssetFolder}ImageResizer.png", "ImageResizer", "Image Resizer"),
            new($"{WelcomeAssetFolder}AlwaysOnTop.png", "AlwaysOnTop", "Always on Top"),
            new($"{WelcomeAssetFolder}FileLocksmith.png", "FileLocksmith", "File Locksmith"),
            new($"{WelcomeAssetFolder}NewPlus.png", "NewPlus", "New+"),
            new($"{WelcomeAssetFolder}PowerDisplay.png", "PowerDisplay", "Power Display"),
            new($"{WelcomeAssetFolder}MouseWithoutBorders.png", "MouseWithoutBorders", "Mouse Without Borders"),
            new($"{WelcomeAssetFolder}Awake.png", "Awake", "Awake"),
            new($"{WelcomeAssetFolder}CropAndLock.png", "CropAndLock", "Crop And Lock"),
            new($"{WelcomeAssetFolder}ScreenRuler.png", "MeasureTool", "Screen Ruler"),
            new($"{WelcomeAssetFolder}ShortcutGuide.png", "ShortcutGuide", "Shortcut Guide"),
            new($"{WelcomeAssetFolder}QuickAccent.png", "QuickAccent", "Quick Accent"),
            new($"{WelcomeAssetFolder}Hosts.png", "Hosts", "Hosts File Editor"),
            new($"{WelcomeAssetFolder}RegistryPreview.png", "RegistryPreview", "Registry Preview"),
            new($"{WelcomeAssetFolder}CommandNotFound.png", "CmdNotFound", "Command Not Found"),
            new($"{WelcomeAssetFolder}FileExplorerPreview.png", "FileExplorer", "File Explorer Add-ons"),
            new($"{WelcomeAssetFolder}PowerToysRun.png", "Run", "PowerToys Run"),
            new($"{WelcomeAssetFolder}GrabAndMove.png", "GrabAndMove", "Grab And Move"),
            new($"{WelcomeAssetFolder}FindMyMouse.png", "MouseUtils", "Find My Mouse"),
            new($"{WelcomeAssetFolder}MouseJump.png", "MouseUtils", "Mouse Jump"),
            new($"{WelcomeAssetFolder}MouseHighlighter.png", "MouseUtils", "Mouse Highlighter"),
            new($"{WelcomeAssetFolder}MouseCrosshairs.png", "MouseUtils", "Mouse Crosshairs"),
            new($"{WelcomeAssetFolder}CursorWrap.png", "MouseUtils", "Cursor Wrap"),
            new($"{WelcomeAssetFolder}WindowingAndLayouts.png", "WindowingAndLayouts", "Windowing & Layouts"),
            new($"{WelcomeAssetFolder}SystemTools.png", "SystemTools", "System Tools"),
            new($"{WelcomeAssetFolder}SemanticKernel.png", "SemanticKernel", "Semantic Kernel"),
            new($"{WelcomeAssetFolder}MouseUtils.png", "MouseUtils", "Mouse Utilities"),
            new($"{WelcomeAssetFolder}InputOutput.png", "InputOutput", "Input / Output"),
            new($"{WelcomeAssetFolder}FileManagement.png", "FileManagement", "File Management"),
            new($"{WelcomeAssetFolder}Advanced.png", "Advanced", "Advanced"),
        ];

        Hero.RevealTargets.Add(WelcomeTitle);
        Hero.RevealTargets.Add(WelcomeDescription);
        Hero.RevealTargets.Add(WelcomeActions);
        Hero.RevealTargets.Add(DataDiagnosticsCard);
    }

    private void Hero_ModuleInvoked(object sender, string navigationTag)
    {
        NavigateToModuleCallback?.Invoke(navigationTag);
    }

    private void NavigateToModule(string tag)
    {
        // Simulates opening a module's settings page. Navigating back to BlankPage1
        // re-constructs it, and since WelcomeIntroPlayed is now true it plays the
        // short "Settle" animation instead of the full "Intro" again.
        WelcomeHeroPage.Instance.NavigateToModule(tag);
    }
}
