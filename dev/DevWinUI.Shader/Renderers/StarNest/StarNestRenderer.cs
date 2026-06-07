using ComputeSharp.D2D1.WinUI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;

namespace DevWinUI;

public partial class StarNestRenderer : RendererBase
{
    private PixelShaderEffect<StarNestShader>? _effect;
    private float _timeAccumulator = 0f;

    public override void CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        Dispose();
        _effect = new PixelShaderEffect<StarNestShader>();
    }

    public override void Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        if (_effect == null || !isEnabled) return;

        UpdateBreathing(currentBassEnergy, breathingIntensity);
        _timeAccumulator += (float)args.Timing.ElapsedTime.TotalSeconds;
    }

    public override void Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        if (_effect == null || !isEnabled) return;

        var ds = args.DrawingSession;

        float width = sender.ConvertDipsToPixels((float)sender.Size.Width, CanvasDpiRounding.Round);
        float height = sender.ConvertDipsToPixels((float)sender.Size.Height, CanvasDpiRounding.Round);

        var center = new Vector2((float)sender.Size.Width / 2, (float)sender.Size.Height / 2);

        _effect.ConstantBuffer = new StarNestShader(
            _timeAccumulator,
            new float2(width, height),
            speed,
            zoom,
            tile,
            rotationSpeed1,
            rotationSpeed2,
            stepScale,
            fadePower,
            colorIntensity,
            color.ToVector3RGB()
        );

        ApplyBreathingTransform(ds, center, isBreathingEffectEnabled);
        ds.DrawImage(_effect);
        ResetTransform(ds, isBreathingEffectEnabled);
    }

    public override void Dispose()
    {
        _effect?.Dispose();
        _effect = null;
    }
}
