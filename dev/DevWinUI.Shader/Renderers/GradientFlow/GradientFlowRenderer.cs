using ComputeSharp.D2D1.WinUI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;

namespace DevWinUI;

public partial class GradientFlowRenderer : RendererBase
{
    private PixelShaderEffect<GradientFlowShader>? _gradientFlowEffect;
    private float _timeAccumulator = 0f;

    public override void CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        Dispose();
        _gradientFlowEffect = new PixelShaderEffect<GradientFlowShader>();
    }

    public override void Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        if (_gradientFlowEffect == null || !isEnabled) return;
        TimeSpan elapsedTime = args.Timing.ElapsedTime;

        UpdateBreathing(currentBassEnergy, breathingIntensity);
        _timeAccumulator += (float)elapsedTime.TotalSeconds;
    }

    public override void Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        if (_gradientFlowEffect == null || !isEnabled) return;

        var ds = args.DrawingSession;

        float width = sender.ConvertDipsToPixels((float)sender.Size.Width, CanvasDpiRounding.Round);
        float height = sender.ConvertDipsToPixels((float)sender.Size.Height, CanvasDpiRounding.Round);

        var center = new Vector2((float)sender.Size.Width / 2, (float)sender.Size.Height / 2);

        _gradientFlowEffect.ConstantBuffer = new GradientFlowShader(
            _timeAccumulator,
            new float2(width, height),
            accentColor1.ToVector3RGB(),
            accentColor2.ToVector3RGB(),
            accentColor3.ToVector3RGB(),
            accentColor4.ToVector3RGB()
        );

        ApplyBreathingTransform(ds, center, isBreathingEffectEnabled);
        ds.DrawImage(_gradientFlowEffect);
        ResetTransform(ds, isBreathingEffectEnabled);
    }

    public override void Dispose()
    {
        _gradientFlowEffect?.Dispose();
        _gradientFlowEffect = null;
    }
}
