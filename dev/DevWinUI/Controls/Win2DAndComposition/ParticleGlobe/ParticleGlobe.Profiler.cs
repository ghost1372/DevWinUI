using System.Diagnostics;

namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    public bool ShowProfiler
    {
        get { return (bool)GetValue(ShowProfilerProperty); }
        set { SetValue(ShowProfilerProperty, value); }
    }

    public static readonly DependencyProperty ShowProfilerProperty =
        DependencyProperty.Register(nameof(ShowProfiler), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(false, OnSettingsPropertyChanged));

    private const int PfSlots = 8;
    private static readonly string[] PfNames = { "Stars", "Body", "Lines", "Particles", "Border+Flow", "Bars/Fx", "Heads/Mk", "Text/HUD" };

    private bool _showProfiler;
    private readonly float[] _pfSec = new float[PfSlots];
    private float _pfUpdateMs, _pfDrawMs, _pfFrameMs;
    private long _pfLastDraw;
    private int _pfDrawn, _pfFrames;
    private float _pfFpsShown;

    private string[] _pfLines = Array.Empty<string>();
    private long _pfTextAt, _pfAllocPrev, _pfGcPrev;
    private CanvasTextFormat _pfFormat;

    private static float PfMs(long ticks) => ticks * 1000f / Stopwatch.Frequency;

    private static void PfSmooth(ref float v, float sample) => v = v <= 0f ? sample : v + (sample - v) * 0.03f;

    private void PfMark(int slot, ref long last)
    {
        if (last == 0) return;
        long now = Stopwatch.GetTimestamp();
        PfSmooth(ref _pfSec[slot], PfMs(now - last));
        last = now;
    }

    private void DrawProfiler(CanvasDrawingSession ds, float w, float h)
    {
        long now = Environment.TickCount64;
        if (_pfLines.Length == 0 || now - _pfTextAt >= 500)
        {
            double secs = _pfTextAt == 0 ? 0.5 : (now - _pfTextAt) / 1000.0;
            long alloc = GC.GetTotalAllocatedBytes(false);
            int gc = GC.CollectionCount(0);
            double allocKb = _pfTextAt == 0 ? 0 : (alloc - _pfAllocPrev) / 1024.0 / secs;
            double gcPerSec = _pfTextAt == 0 ? 0 : (gc - _pfGcPrev) / secs;
            _pfAllocPrev = alloc; _pfGcPrev = gc; _pfTextAt = now;

            // frames counted over the window: steadier than averaging single frame times
            float fps = (float)(_pfFrames / secs);
            _pfFrames = 0;
            _pfFpsShown = _pfFpsShown <= 0f ? fps : _pfFpsShown + (fps - _pfFpsShown) * 0.5f;
            fps = _pfFpsShown;
            _pfFrameMs = fps > 0f ? 1000f / fps : 0f;
            _pfLines = new[]
            {
                $"FPS {fps,5:0.0}   frame {_pfFrameMs,5:0.0} ms",
                $"update {_pfUpdateMs,5:0.00} ms   draw(CPU) {_pfDrawMs,5:0.00} ms",
                $"{PfNames[0]} {_pfSec[0]:0.00}  {PfNames[1]} {_pfSec[1]:0.00}  {PfNames[2]} {_pfSec[2]:0.00}",
                $"{PfNames[3]} {_pfSec[3]:0.00}  {PfNames[4]} {_pfSec[4]:0.00}",
                $"{PfNames[5]} {_pfSec[5]:0.00}  {PfNames[6]} {_pfSec[6]:0.00}  {PfNames[7]} {_pfSec[7]:0.00}",
                $"particles {_pfDrawn:N0}/{_count:N0}   quality {_qKeep * 100f:0}%",
                $"mem {GC.GetTotalMemory(false) / 1048576.0:0.0} MB   alloc {allocKb:0} KB/s   gen0 {gcPerSec:0.0}/s",
            };
        }

        _pfFormat ??= new CanvasTextFormat { FontFamily = "Consolas", FontSize = 11f, WordWrapping = CanvasWordWrapping.NoWrap };

        const float lineH = 15f, pad = 8f, width = 360f;
        float height = _pfLines.Length * lineH + pad * 2f;
        float x = MathF.Max(4f, w - width - 8f), y = 8f;

        ds.FillRoundedRectangle(x, y, width, height, 6f, 6f, Color.FromArgb(170, 6, 10, 18));
        ds.DrawRoundedRectangle(x, y, width, height, 6f, 6f, Color.FromArgb(120, 90, 200, 255), 1f);

        float ms = _pfFrameMs;
        // green when near the target rate, amber above 30 fps, red below
        float fpsNow = ms > 0f ? 1000f / ms : 0f;
        var head = fpsNow >= _qTargetFps * 0.9f ? Color.FromArgb(255, 120, 255, 160) : fpsNow >= 30f ? Color.FromArgb(255, 255, 200, 90) : Color.FromArgb(255, 255, 110, 110);
        var text = Color.FromArgb(235, 200, 230, 255);
        for (int i = 0; i < _pfLines.Length; i++)
            ds.DrawText(_pfLines[i], x + pad, y + pad + i * lineH, i == 0 ? head : text, _pfFormat);
    }
}
