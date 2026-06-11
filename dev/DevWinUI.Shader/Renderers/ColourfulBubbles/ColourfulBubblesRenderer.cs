using ComputeSharp.D2D1.WinUI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;

namespace DevWinUI;

public partial class ColourfulBubblesRenderer : RendererBase
{
    private PixelShaderEffect<ColourfulBubblesShader>? _colourfulEffect;
    private float _timeAccumulator = 0f;

    public override void CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        Dispose();
        _colourfulEffect = new PixelShaderEffect<ColourfulBubblesShader>();
    }

    public override void Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        if (_colourfulEffect == null || !isEnabled) return;
        TimeSpan elapsedTime = args.Timing.ElapsedTime;

        UpdateBreathing(currentBassEnergy, breathingIntensity);
        _timeAccumulator += (float)elapsedTime.TotalSeconds;
    }

    public override void Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        if (_colourfulEffect == null || !isEnabled) return;

        var ds = args.DrawingSession;

        float width = sender.ConvertDipsToPixels((float)sender.Size.Width, CanvasDpiRounding.Round);
        float height = sender.ConvertDipsToPixels((float)sender.Size.Height, CanvasDpiRounding.Round);

        var center = new Vector2((float)sender.Size.Width / 2, (float)sender.Size.Height / 2);

        _colourfulEffect.ConstantBuffer = new ColourfulBubblesShader(
            _timeAccumulator,
            new float2(width, height),
            new float2(mouseX, mouseY),
            (int)direction
        );

        ApplyBreathingTransform(ds, center, isBreathingEffectEnabled);
        ds.DrawImage(_colourfulEffect);
        ResetTransform(ds, isBreathingEffectEnabled);
    }

    public override void Dispose()
    {
        _colourfulEffect?.Dispose();
        _colourfulEffect = null;
    }
}
