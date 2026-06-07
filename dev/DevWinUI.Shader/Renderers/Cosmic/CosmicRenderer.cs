using ComputeSharp.D2D1.WinUI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;

namespace DevWinUI;

public partial class CosmicRenderer : RendererBase
{
    private PixelShaderEffect<CosmicShader>? _cosmicEffect;
    private float _timeAccumulator;

    public override void CreateResources(
        CanvasAnimatedControl sender,
        CanvasCreateResourcesEventArgs args)
    {
        Dispose();

        _cosmicEffect =
            new PixelShaderEffect<CosmicShader>();
    }

    public override void Update(
        ICanvasAnimatedControl sender,
        CanvasAnimatedUpdateEventArgs args)
    {
        if (_cosmicEffect == null || !isEnabled)
            return;

        UpdateBreathing(
            currentBassEnergy,
            breathingIntensity);

        _timeAccumulator +=
            (float)args.Timing.ElapsedTime.TotalSeconds;
    }

    public override void Draw(
        ICanvasAnimatedControl sender,
        CanvasAnimatedDrawEventArgs args)
    {
        if (_cosmicEffect == null || !isEnabled)
            return;

        float width =
            sender.ConvertDipsToPixels(
                (float)sender.Size.Width,
                CanvasDpiRounding.Round);

        float height =
            sender.ConvertDipsToPixels(
                (float)sender.Size.Height,
                CanvasDpiRounding.Round);

        var center =
            new Vector2(
                (float)sender.Size.Width / 2,
                (float)sender.Size.Height / 2);

        _cosmicEffect.ConstantBuffer =
            new CosmicShader(
                _timeAccumulator,
                new float2(width, height),
                speed,
                rotationSpeed,
                frequency,
                hotspotContrast,
                hotspotStrength,
                edgeFalloff,
                exposure,
                accentColor1.ToVector4RGBA(),
                accentColor2.ToVector4RGBA(),
                accentColor3.ToVector4RGBA(),
                accentColor4.ToVector4RGBA());

        ApplyBreathingTransform(
            args.DrawingSession,
            center,
            isBreathingEffectEnabled);

        args.DrawingSession.DrawImage(_cosmicEffect);

        ResetTransform(
            args.DrawingSession,
            isBreathingEffectEnabled);
    }

    public override void Dispose()
    {
        _cosmicEffect?.Dispose();
        _cosmicEffect = null;
    }
}
