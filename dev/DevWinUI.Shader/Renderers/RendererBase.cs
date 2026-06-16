using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;

namespace DevWinUI;

public partial class RendererBase : DependencyObject, IDisposable
{
    protected Matrix4x4 _threeDimMatrix = Matrix4x4.Identity;
    public ParallaxTiltEffect? ParallaxContext { get; set; }

    protected float _currentScale = 1.0f;
    private float _targetScale = 1.0f;
    internal bool isEnabled = true;
    internal bool is3DEnabled = false;
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

    public bool Is3DEnabled
    {
        get { return (bool)GetValue(Is3DEnabledProperty); }
        set { SetValue(Is3DEnabledProperty, value); }
    }

    public static readonly DependencyProperty Is3DEnabledProperty =
        DependencyProperty.Register(nameof(Is3DEnabled), typeof(bool), typeof(RendererBase), new PropertyMetadata(false, OnIs3DEnabledChanged));

    private static void OnIs3DEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RendererBase)d;
        ctl?.is3DEnabled = (bool)e.NewValue;
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
    protected void UpdateParallaxMatrix(Vector3 center, bool isAutoParallax, float manualAngleX = 0, float manualAngleY = 0, float manualAngleZ = 0, float depth = 800f)
    {
        float angleX = 0f, angleY = 0f, angleZ = 0f;
        Matrix4x4 parallaxTranslation = Matrix4x4.Identity;

        if (isAutoParallax && ParallaxContext != null)
        {
            angleX = ParallaxContext.CurrentRotationX;
            angleY = ParallaxContext.CurrentRotationY;
            parallaxTranslation = Matrix4x4.CreateTranslation(
                ParallaxContext.CurrentTranslateX,
                ParallaxContext.CurrentTranslateY,
                0);
        }
        else
        {
            angleX = manualAngleX;
            angleY = manualAngleY;
            angleZ = manualAngleZ;
        }

        float rotationX = (float)(Math.PI * angleX / 180.0);
        float rotationY = (float)(Math.PI * angleY / 180.0);
        float rotationZ = (float)(Math.PI * angleZ / 180.0);

        Matrix4x4 rotation = Matrix4x4.CreateRotationX(rotationX) *
            Matrix4x4.CreateRotationY(rotationY) *
            Matrix4x4.CreateRotationZ(rotationZ);

        Matrix4x4 perspective = Matrix4x4.Identity;
        if (depth > 0) perspective.M34 = 1.0f / depth;

        _threeDimMatrix = Matrix4x4.CreateTranslation(-center) * rotation * perspective * Matrix4x4.CreateTranslation(center) * parallaxTranslation;
    }

    protected void DrawWithParallax(CanvasDrawingSession ds, ICanvasImage? source)
    {
        if (source == null) return;

        if (!_threeDimMatrix.IsIdentity)
        {
            ds.DrawImage(new Transform3DEffect
            {
                Source = source,
                TransformMatrix = _threeDimMatrix
            });
        }
        else
        {
            ds.DrawImage(source);
        }
    }

    protected void ResetParallaxMatrix()
    {
        _threeDimMatrix = Matrix4x4.Identity;
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
