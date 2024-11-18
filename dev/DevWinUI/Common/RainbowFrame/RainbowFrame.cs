using WinRT.Interop;

namespace DevWinUI;
public partial class RainbowFrame : IRainbowFrame
{
    private uint _defaultColor = 0xFFFFFFFF;
    private DispatcherTimer _frameTimer;
    private DateTimeOffset _started;
    private TimeSpan FrameUpdateInterval = TimeSpan.FromMilliseconds(16);
    private IntPtr _hwnd;
    private int EffectSpeed = 4;

    public RainbowFrame()
    {
        
    }
    public RainbowFrame(Window window)
    {
        Initialize(window);
    }
    public void Initialize(Window window, TimeSpan frameUpdateInterval, int effectSpeed)
    {
        InternalInitialize(window, _hwnd, effectSpeed, frameUpdateInterval);
    }

    public void Initialize(IntPtr hwnd, TimeSpan frameUpdateInterval, int effectSpeed)
    {
        InternalInitialize(null, hwnd, effectSpeed, frameUpdateInterval);
    }

    public void Initialize(Window window, TimeSpan frameUpdateInterval)
    {
        InternalInitialize(window, _hwnd, EffectSpeed, frameUpdateInterval);
    }

    public void Initialize(IntPtr hwnd, TimeSpan frameUpdateInterval)
    {
        InternalInitialize(null, hwnd, EffectSpeed, frameUpdateInterval);
    }

    public void Initialize(Window window, int effectSpeed)
    {
        InternalInitialize(window, _hwnd, effectSpeed, FrameUpdateInterval);
    }

    public void Initialize(IntPtr hwnd, int effectSpeed)
    {
        InternalInitialize(null, hwnd, effectSpeed, FrameUpdateInterval);
    }

    public void Initialize(Window window)
    {
        InternalInitialize(window, _hwnd, EffectSpeed, FrameUpdateInterval);
    }

    public void Initialize(IntPtr hwnd)
    {
        InternalInitialize(null, hwnd, EffectSpeed, FrameUpdateInterval);
    }

    private void InternalInitialize(Window window, IntPtr hwnd, int effectSpeed, TimeSpan frameUpdateInterval)
    {
        if (window != null)
        {
            _hwnd = WindowNative.GetWindowHandle(window);
        }
        else
        {
            _hwnd = hwnd;
        }

        InitializeEffectSpeed(effectSpeed);
        FrameUpdateInterval = frameUpdateInterval;
    }

    private void InitializeEffectSpeed(int effectSpeed)
    {
        if (effectSpeed > 0)
        {
            EffectSpeed = effectSpeed;
        }
    }

    /// <summary>
    /// default value is 4 and effectSpeed should be greater than zero (0)
    /// </summary>
    /// <param name="effectSpeed"></param>
    public void UpdateEffectSpeed(int effectSpeed)
    {
        InitializeEffectSpeed(effectSpeed);
    }

    public void ResetFrameColorToDefault()
    {
        _frameTimer?.Stop();
        _frameTimer = null;
        ChangeFrameColor(_defaultColor);
    }

    public void ChangeFrameColor(Color color)
    {
        _frameTimer?.Stop();
        _frameTimer = null;
        ChangeFrameColor(ColorHelper.ColorToUInt(color));
    }

    public void ChangeFrameColor(uint color)
    {
        try
        {
            if (OSVersionHelper.IsWindows11_22000_OrGreater)
            {
                unsafe
                {
                    PInvoke.DwmSetWindowAttribute(new HWND(_hwnd), Windows.Win32.Graphics.Dwm.DWMWINDOWATTRIBUTE.DWMWA_BORDER_COLOR, &color, sizeof(uint));
                }
            }
        }
        catch (Exception)
        {
        }
    }

    public void StartRainbowFrame()
    {
        _started = DateTimeOffset.Now;

        if (_frameTimer == null)
        {
            _frameTimer = new DispatcherTimer();
            _frameTimer.Interval = FrameUpdateInterval;
            _frameTimer.Tick += (s, e) => UpdateFrameColor(s, e);
        }
        _frameTimer.Start();
    }

    public void StopRainbowFrame()
    {
        _frameTimer?.Stop();
    }

    private void UpdateFrameColor(object sender, object e)
    {
        var saturateAndToColor = new Func<float, float, float, uint>((a, b, c) =>
        {
            return ColorHelper.ColorToUInt(new Color
            {
                R = (byte)(255f * Math.Clamp(a, 0f, 1f)),
                G = (byte)(255f * Math.Clamp(b, 0f, 1f)),
                B = (byte)(255f * Math.Clamp(c, 0f, 1f))
            });
        });

        // Helper for converting a hue [0, 1) to an RGB value.
        // Credit to https://www.chilliant.com/rgb2hsv.html
        var hueToRGB = new Func<float, uint>(H =>
        {
            var R = Math.Abs(H * 6 - 3) - 1;
            var G = 2 - Math.Abs(H * 6 - 2);
            var B = 2 - Math.Abs(H * 6 - 4);
            return saturateAndToColor(R, G, B);
        });

        // Now, the main body of work.
        // - Convert the time delta between when we were started and now, to a hue. This will cycle us through all the colors.
        // - Convert that hue to an RGB value.
        // - Set the frame's color to that RGB color.
        var now = DateTimeOffset.Now;
        var delta = now - _started;
        var seconds = delta.TotalSeconds / EffectSpeed; // divide by EffectSpeed (Default = 4), to make the effect slower. Otherwise it flashes way too fast.

        var integerValue = Math.Floor(seconds);

        var decimalValue = seconds - integerValue;

        var color = hueToRGB((float)decimalValue);

        ChangeFrameColor(color);
    }
}
