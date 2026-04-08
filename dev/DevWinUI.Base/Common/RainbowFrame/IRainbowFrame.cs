namespace DevWinUI;
public interface IRainbowFrame
{
    void Initialize(IntPtr window, TimeSpan frameUpdateInterval, int effectSpeed);
    void Initialize(Microsoft.UI.Xaml.Window window, TimeSpan frameUpdateInterval, int effectSpeed);
    void Initialize(Microsoft.UI.Xaml.Window window, TimeSpan frameUpdateInterval);
    void Initialize(IntPtr window, TimeSpan frameUpdateInterval);
    void Initialize(Microsoft.UI.Xaml.Window window, int effectSpeed);
    void Initialize(IntPtr window);
    void Initialize(Microsoft.UI.Xaml.Window window);
    void UpdateEffectSpeed(int effectSpeed);
    void ResetFrameColorToDefault();
    void ChangeFrameColor(Color color);
    void ChangeFrameColor(uint color);
    void StartRainbowFrame();
    void StopRainbowFrame();
}
