namespace DevWinUIGallery.Views;

public sealed partial class ShaderBackgroundPage : Page
{
    private readonly float[] _spectrum = new float[128];
    private readonly Random _random = new();
    private float _time;
    public ShaderBackgroundPage()
    {
        InitializeComponent();
    }
   
    private void GenerateFakeSpectrum()
    {
        _time += 0.03f;

        for (int i = 0; i < _spectrum.Length; i++)
        {
            float x = (float)i / _spectrum.Length;

            // Main movement
            float wave1 = (MathF.Sin(_time * 2f + x * 8f) + 1f) * 0.5f;
            float wave2 = (MathF.Sin(_time * 1.3f + x * 20f) + 1f) * 0.5f;

            // Random flicker
            float noise = (float)_random.NextDouble() * 0.15f;

            // Emphasize lower frequencies
            float falloff = 1f - x * 0.7f;

            float target = (wave1 * 0.7f + wave2 * 0.3f + noise) * falloff;

            // Smooth transition
            _spectrum[i] += (target - _spectrum[i]) * 0.15f;
        }
    }
    private void ShaderBackgroundSample_Update(object sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedUpdateEventArgs e)
    {
        GenerateFakeSpectrum();
    }

    private void ShaderBackgroundSample_Draw(object sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs e)
    {
        SpectrumRendererSample.SpectrumData = _spectrum;
    }
}
