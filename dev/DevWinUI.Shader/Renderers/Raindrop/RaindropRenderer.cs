using System.Numerics;
using ComputeSharp.D2D1.WinUI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace DevWinUI;

public partial class RaindropRenderer : RendererBase
{
    private PixelShaderEffect<RaindropShader>? _raindropEffect;
    private float _timeAccumulator = 0f;

    public override void CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        Dispose();
        _raindropEffect = new PixelShaderEffect<RaindropShader>();
    }

    public override void Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        if (_raindropEffect == null || !isEnabled) return;

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
        if (_raindropEffect == null || !isEnabled) return;

        var ds = args.DrawingSession;

        float width = sender.ConvertDipsToPixels((float)sender.Size.Width, CanvasDpiRounding.Round);
        float height = sender.ConvertDipsToPixels((float)sender.Size.Height, CanvasDpiRounding.Round);

        var center = new Vector2((float)sender.Size.Width / 2, (float)sender.Size.Height / 2);

        _raindropEffect.ConstantBuffer = new RaindropShader(
            _timeAccumulator,
            new float2(width, height),
            (float)raindropSpeed / 100f,
            (float)raindropSize / 100f,
            (float)raindropDensity / 100f,
            MathF.PI * (float)raindropLightAngle / 180f,
            (float)raindropShadowIntensity / 100f
        );

        ApplyBreathingTransform(ds, center, isBreathingEffectEnabled);
        base.DrawWithParallax(ds, _raindropEffect);
        ResetTransform(ds, isBreathingEffectEnabled);
    }

    public override void Dispose()
    {
        _raindropEffect?.Dispose();
        _raindropEffect = null;
    }
}
