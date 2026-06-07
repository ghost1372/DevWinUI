using ComputeSharp.D2D1.WinUI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using System.Numerics;
using Windows.UI;

namespace DevWinUI;


public partial class FluidBackgroundRenderer : RendererBase
{
    private readonly ValueTransition<Color> _accentColor1Transition = new(
            initialValue: Colors.Black,
            defaultTotalDuration: 0.3f,
            interpolator: (from, to, progress) => ColorHelper.GetInterpolatedColor(progress, from, to)
        );
    private readonly ValueTransition<Color> _accentColor2Transition = new(
        initialValue: Colors.Black,
        defaultTotalDuration: 0.3f,
        interpolator: (from, to, progress) => ColorHelper.GetInterpolatedColor(progress, from, to)
    );
    private readonly ValueTransition<Color> _accentColor3Transition = new(
        initialValue: Colors.Black,
        defaultTotalDuration: 0.3f,
        interpolator: (from, to, progress) => ColorHelper.GetInterpolatedColor(progress, from, to)
    );
    private readonly ValueTransition<Color> _accentColor4Transition = new(
        initialValue: Colors.Black,
        defaultTotalDuration: 0.3f,
        interpolator: (from, to, progress) => ColorHelper.GetInterpolatedColor(progress, from, to)
    );

    private PixelShaderEffect<FluidBackgroundShader>? _fluidEffect;
    private float _timeAccumulator = 0f;
    private float3 _c1 = float3.Zero, _c2 = float3.Zero, _c3 = float3.Zero, _c4 = float3.Zero;
    private float _rnd1 = 0, _rnd2 = 0, _rnd3 = 0;
    private bool useHSVBlending = false;
    public override void OnApplyTemplate()
    {
        UpdatePalette();
    }
    public override void CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        Dispose();
        _fluidEffect = new();
    }

    public override void Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        if (_fluidEffect == null || !isEnabled) return;

        TimeSpan elapsedTime = args.Timing.ElapsedTime;

        _accentColor1Transition.Update(elapsedTime);
        _accentColor2Transition.Update(elapsedTime);
        _accentColor3Transition.Update(elapsedTime);
        _accentColor4Transition.Update(elapsedTime);

        Vector3 v1 = _accentColor1Transition.Value.ToVector3RGB();
        Vector3 v2 = _accentColor2Transition.Value.ToVector3RGB();
        Vector3 v3 = _accentColor3Transition.Value.ToVector3RGB();
        Vector3 v4 = _accentColor4Transition.Value.ToVector3RGB();

        _c1 = new float3(v1.X, v1.Y, v1.Z);
        _c2 = new float3(v2.X, v2.Y, v2.Z);
        _c3 = new float3(v3.X, v3.Y, v3.Z);
        _c4 = new float3(v4.X, v4.Y, v4.Z);

        UpdateBreathing(currentBassEnergy, breathingIntensity);
        

        _timeAccumulator += (float)elapsedTime.TotalSeconds;
    }

    public override void Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        var currentOpacity = fluidOverlayOpacity / 100.0;

        if (_fluidEffect == null || !isEnabled || currentOpacity <= 0) return;

        var ds = args.DrawingSession;

        float width = sender.ConvertDipsToPixels((float)sender.Size.Width, CanvasDpiRounding.Round);
        float height = sender.ConvertDipsToPixels((float)sender.Size.Height, CanvasDpiRounding.Round);

        _fluidEffect.ConstantBuffer = new FluidBackgroundShader(
            new float2(width, height),
            _timeAccumulator,
            _c1, _c2, _c3, _c4,
            _rnd1, _rnd2, _rnd3,
            useHSVBlending,
            isFluidOverlayLightWaveEnabled,
            isColorDitheringEnabled
        );

        var center = new Vector2((float)sender.Size.Width / 2, (float)sender.Size.Height / 2);

        ApplyBreathingTransform(ds, center, isBreathingEffectEnabled);

        if (currentOpacity >= 1.0)
        {
            ds.DrawImage(_fluidEffect);
        }
        else
        {
            using var opacityEffect = new OpacityEffect
            {
                Source = _fluidEffect,
                Opacity = (float)currentOpacity
            };
            ds.DrawImage(opacityEffect);
        }

        ResetTransform(ds, isBreathingEffectEnabled);
    }

    public override void Dispose()
    {
        _fluidEffect?.Dispose();
        _fluidEffect = null;
    }

    private void UpdatePalette()
    {
        _accentColor1Transition.Start(fluidAccentColor1);
        _accentColor2Transition.Start(fluidAccentColor2);
        _accentColor3Transition.Start(fluidAccentColor3);
        _accentColor4Transition.Start(fluidAccentColor4);
    }

}
