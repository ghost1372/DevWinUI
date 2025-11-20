namespace DevWinUI;
public partial class Particle
{
    public Color LineColor
    {
        get { return (Color)GetValue(LineColorProperty); }
        set { SetValue(LineColorProperty, value); }
    }
    public static readonly DependencyProperty LineColorProperty =
        DependencyProperty.Register("LineColor", typeof(Color), typeof(Particle), new PropertyMetadata(Colors.DarkGray, LineColorChanged));

    public Color ParticleColor
    {
        get { return (Color)GetValue(ParticleColorProperty); }
        set { SetValue(ParticleColorProperty, value); }
    }
    public static readonly DependencyProperty ParticleColorProperty =
        DependencyProperty.Register("ParticleColor", typeof(Color), typeof(Particle), new PropertyMetadata(Colors.Gray, ParticleColorChanged));

    public bool Paused
    {
        get { return (bool)GetValue(PausedProperty); }
        set { SetValue(PausedProperty, value); }
    }
    public static readonly DependencyProperty PausedProperty =
        DependencyProperty.Register("Paused", typeof(bool), typeof(Particle), new PropertyMetadata(false, IsPausedChanged));

    public bool IsPointerEnable
    {
        get { return (bool)GetValue(IsPointerEnableProperty); }
        set { SetValue(IsPointerEnableProperty, value); }
    }

    public static readonly DependencyProperty IsPointerEnableProperty =
        DependencyProperty.Register("IsPointerEnable", typeof(bool), typeof(Particle), new PropertyMetadata(true, IsPointerEnableChanged));

    /// <summary>
    /// Min 0
    /// Max 9
    /// </summary>
    public int Density
    {
        get { return (int)GetValue(DensityProperty); }
        set { SetValue(DensityProperty, value); }
    }

    public static readonly DependencyProperty DensityProperty =
        DependencyProperty.Register("Density", typeof(int), typeof(Particle), new PropertyMetadata(5, DensityChanged));

}
