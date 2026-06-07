using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;

namespace DevWinUI;

public partial class RendererBase : DependencyObject, IDisposable
{
    protected float _currentScale = 1.0f;
    private float _targetScale = 1.0f;

    internal bool isEnabled = true;
    internal float currentBassEnergy = 0f;
    internal int breathingIntensity = 80;
    internal bool isBreathingEffectEnabled = false;

    public bool IsEnabled
    {
        get { return (bool)GetValue(IsEnabledProperty); }
        set { SetValue(IsEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(RendererBase), new PropertyMetadata(true, OnIsEnabledChanged));

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RendererBase)d;
        ctl.isEnabled = (bool)e.NewValue;
    }

    public int BreathingIntensity
    {
        get { return (int)GetValue(BreathingIntensityProperty); }
        set { SetValue(BreathingIntensityProperty, value); }
    }

    public static readonly DependencyProperty BreathingIntensityProperty =
        DependencyProperty.Register(nameof(BreathingIntensity), typeof(int), typeof(RendererBase), new PropertyMetadata(80, OnBreathingIntensityChanged));

    private static void OnBreathingIntensityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RendererBase)d;
        ctl.breathingIntensity = (int)e.NewValue;
    }
    public bool IsBreathingEffectEnabled
    {
        get { return (bool)GetValue(IsBreathingEffectEnabledProperty); }
        set { SetValue(IsBreathingEffectEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsBreathingEffectEnabledProperty =
        DependencyProperty.Register(nameof(IsBreathingEffectEnabled), typeof(bool), typeof(RendererBase), new PropertyMetadata(false, OnIsBreathingEffectEnabledChanged));

    private static void OnIsBreathingEffectEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RendererBase)d;
        ctl.isBreathingEffectEnabled = (bool)e.NewValue;
    }
    public double BassEnergy
    {
        get { return (double)GetValue(BassEnergyProperty); }
        set { SetValue(BassEnergyProperty, value); }
    }

    public static readonly DependencyProperty BassEnergyProperty =
        DependencyProperty.Register(nameof(BassEnergy), typeof(double), typeof(RendererBase), new PropertyMetadata(0.0, OnBassEnergyChanged));

    private static void OnBassEnergyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RendererBase)d;
        ctl.currentBassEnergy = (float)((double)e.NewValue);
    }
    internal void UpdateBreathing(float bassEnergy, int intensity)
    {
        if (intensity <= 0)
        {
            _currentScale = 1.0f;
            return;
        }

        float maxScaleOffset = intensity / 100.0f;
        _targetScale = 1.0f + (bassEnergy * maxScaleOffset);

        if (_targetScale > _currentScale)
        {
            _currentScale += (_targetScale - _currentScale) * 0.2f;
        }
        else
        {
            _currentScale += (_targetScale - _currentScale) * 0.05f;
        }
    }

    internal void ApplyBreathingTransform(CanvasDrawingSession ds, Vector2 center, bool isEnabled)
    {
        if (isEnabled && _currentScale > 1.0f)
        {
            ds.Transform = Matrix3x2.CreateScale(_currentScale, center);
        }
    }

    internal static void ResetTransform(CanvasDrawingSession ds, bool isEnabled)
    {
        if (isEnabled)
        {
            ds.Transform = Matrix3x2.Identity;
        }
    }

    public virtual void Dispose()
    {
    }

    public virtual void OnApplyTemplate()
    {

    }
    public virtual void Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {

    }

    public virtual void Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {

    }

    public virtual void CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {

    }
}
