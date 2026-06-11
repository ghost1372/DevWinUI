using ComputeSharp.D2D1.WinUI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;

namespace DevWinUI;

public partial class AbstractMovementBackgroundRenderer : RendererBase
{
    private PixelShaderEffect<AbstractMovementBackgroundShader>? _abstractEffect;
    private float _timeAccumulator = 0f;

    public override void CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        Dispose();
        _abstractEffect = new PixelShaderEffect<AbstractMovementBackgroundShader>();
    }

    public override void Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        if (_abstractEffect == null || !isEnabled) return;
        TimeSpan elapsedTime = args.Timing.ElapsedTime;

        UpdateBreathing(currentBassEnergy, breathingIntensity);
        _timeAccumulator += (float)elapsedTime.TotalSeconds;
    }

    public override void Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        if (_abstractEffect == null || !isEnabled) return;

        var ds = args.DrawingSession;

        float width = sender.ConvertDipsToPixels((float)sender.Size.Width, CanvasDpiRounding.Round);
        float height = sender.ConvertDipsToPixels((float)sender.Size.Height, CanvasDpiRounding.Round);

        var center = new Vector2((float)sender.Size.Width / 2, (float)sender.Size.Height / 2);

        _abstractEffect.ConstantBuffer = new AbstractMovementBackgroundShader(
            _timeAccumulator,
            new float2(width, height),
            accentColor1.ToVector3RGB(),
            accentColor2.ToVector3RGB(),
            accentColor3.ToVector3RGB(),
            accentColor4.ToVector3RGB(),
            direction.ToFloat2()
        );

        ApplyBreathingTransform(ds, center, isBreathingEffectEnabled);
        ds.DrawImage(_abstractEffect);
        ResetTransform(ds, isBreathingEffectEnabled);
    }

    public override void Dispose()
    {
        _abstractEffect?.Dispose();
        _abstractEffect = null;
    }
}
internal static partial class AbstractMovementDirectionExtension
{
    public static float2 ToFloat2(this AbstractMovementDirection direction)
    {
        return direction switch
        {
            AbstractMovementDirection.TopLeft => new float2(-1, -1),
            AbstractMovementDirection.TopRight => new float2(1, -1),
            AbstractMovementDirection.BottomLeft => new float2(-1, 1),
            AbstractMovementDirection.BottomRight => new float2(1, 1),
            _ => new float2(0, 0)
        };
    }
}
