using System.Numerics;
using ComputeSharp.D2D1.WinUI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace DevWinUI;

public partial class FogRenderer : RendererBase
{
    private PixelShaderEffect<FogShader>? _fogEffect;
    private float _timeAccumulator = 0f;


    public override void CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        Dispose();
        _fogEffect = new PixelShaderEffect<FogShader>();
    }

    public override void Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        if (_fogEffect == null || !isEnabled) return;
        UpdateBreathing(currentBassEnergy, breathingIntensity);

        TimeSpan elapsedTime = args.Timing.ElapsedTime;

        _timeAccumulator += (float)elapsedTime.TotalSeconds;

        if (is3DEnabled)
        {
            Vector3 center = new Vector3((float)sender.Size.Width / 2, (float)sender.Size.Height / 2, 0);
            base.UpdateParallaxMatrix(center, isAutoParallax: true);
        }
        else
        {
            base.ResetParallaxMatrix();
        }
    }

    public override void Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        if (_fogEffect == null || !isEnabled) return;

        var ds = args.DrawingSession;

        float width = sender.ConvertDipsToPixels((float)sender.Size.Width, CanvasDpiRounding.Round);
        float height = sender.ConvertDipsToPixels((float)sender.Size.Height, CanvasDpiRounding.Round);

        var center = new Vector2((float)sender.Size.Width / 2, (float)sender.Size.Height / 2);

        _fogEffect.ConstantBuffer = new FogShader(
             _timeAccumulator,
             new float2(width, height)
         );

        ApplyBreathingTransform(ds, center, isBreathingEffectEnabled);
        base.DrawWithParallax(ds, _fogEffect);
        ResetTransform(ds, isBreathingEffectEnabled);
    }

    public override void Dispose()
    {
        _fogEffect?.Dispose();
        _fogEffect = null;
    }
}
