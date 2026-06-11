using ComputeSharp.D2D1.WinUI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;

namespace DevWinUI;

public partial class WavyBackgroundRenderer : RendererBase
{
    private PixelShaderEffect<WavyBackgroundShader>? _wavyEffect;
    private float _timeAccumulator = 0f;

    public override void CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        Dispose();
        _wavyEffect = new PixelShaderEffect<WavyBackgroundShader>();
    }

    public override void Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        if (_wavyEffect == null || !isEnabled) return;
        TimeSpan elapsedTime = args.Timing.ElapsedTime;

        UpdateBreathing(currentBassEnergy, breathingIntensity);
        _timeAccumulator += (float)elapsedTime.TotalSeconds;
    }

    public override void Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        if (_wavyEffect == null || !isEnabled) return;

        var ds = args.DrawingSession;

        float width = sender.ConvertDipsToPixels((float)sender.Size.Width, CanvasDpiRounding.Round);
        float height = sender.ConvertDipsToPixels((float)sender.Size.Height, CanvasDpiRounding.Round);

        var center = new Vector2((float)sender.Size.Width / 2, (float)sender.Size.Height / 2);

        _wavyEffect.ConstantBuffer = new WavyBackgroundShader(
            _timeAccumulator,
            new float2(width, height)
        );

        ApplyBreathingTransform(ds, center, isBreathingEffectEnabled);
        ds.DrawImage(_wavyEffect);
        ResetTransform(ds, isBreathingEffectEnabled);
    }

    public override void Dispose()
    {
        _wavyEffect?.Dispose();
        _wavyEffect = null;
    }
}
