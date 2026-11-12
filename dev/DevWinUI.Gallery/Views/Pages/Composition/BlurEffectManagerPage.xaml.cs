using Microsoft.Graphics.Canvas.Effects;
using Windows.UI;
namespace DevWinUIGallery.Views;

public sealed partial class BlurEffectManagerPage : Page
{
    private BlurEffectManager _blurEffectManager;
    private BlurEffectManager _blurEffectManager2;
    public BlurEffectManagerPage()
    {
        InitializeComponent();
        _blurEffectManager = new BlurEffectManager(BackdropImage) { IsTintEnabled = true };
        _blurEffectManager2 = new BlurEffectManager(BackdropImage2);
        CmbEffectBorderMode.Value = Enum.GetValues<EffectBorderMode>()[1];
        CmbEffectOptimization.Value = Enum.GetValues<EffectOptimization>()[1];
        CmbBlendEffectMode.Value = Enum.GetValues<BlendEffectMode>()[0];
        CmbNoiseBlendMode.Value = Enum.GetValues<BlendEffectMode>()[1];
        Loaded += BlurEffectManagerPage_Loaded;
    }

    private void BlurEffectManagerPage_Loaded(object sender, RoutedEventArgs e)
    {
        TGIsBlurEnabled_Toggled(null, null);
    }

    private void TGIsBlurEnabled_Toggled(object sender, RoutedEventArgs e)
    {
        if (_blurEffectManager == null)
        {
            return;
        }

        if (TGIsBlurEnabled.IsOn)
        {
            _blurEffectManager.EnableBlur();
        }
        else
        {
            _blurEffectManager.DisableBlur();
        }
    }

    private void TGSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (_blurEffectManager != null && TGIsBlurEnabled.IsOn)
        {
            _blurEffectManager.BlurAmount = e.NewValue;
        }
    }

    private void CpTintColor_ValueChanged(ColorOption sender, Color value)
    {
        if (_blurEffectManager != null && TGIsBlurEnabled.IsOn)
        {
            _blurEffectManager.TintColor = value;
        }
    }

    private void TGUseNoise_Toggled(object sender, RoutedEventArgs e)
    {
        if (_blurEffectManager != null && TGIsBlurEnabled.IsOn)
        {
            _blurEffectManager.UseNoise = TGUseNoise.IsOn;
            if (TGUseNoise.IsOn)
            {
                _blurEffectManager.NoiseUri = TxtNoiseUri.Text;
            }
        }
    }

    private void CmbEffectBorderMode_ValueChanged(ChoiceOption sender, object value)
    {
        if (_blurEffectManager != null && TGIsBlurEnabled.IsOn)
        {
            _blurEffectManager.BorderMode = (EffectBorderMode)value;
        }
    }

    private void CmbEffectOptimization_ValueChanged(ChoiceOption sender, object value)
    {
        if (_blurEffectManager != null && TGIsBlurEnabled.IsOn)
        {
            _blurEffectManager.Optimization = (EffectOptimization)value;
        }
    }

    private void CmbBlendEffectMode_ValueChanged(ChoiceOption sender, object value)
    {
        if (_blurEffectManager != null && TGIsBlurEnabled.IsOn)
        {
            _blurEffectManager.BlendMode = (BlendEffectMode)value;
        }
    }
    private void CmbNoiseBlendMode_ValueChanged(ChoiceOption sender, object value)
    {
        if (_blurEffectManager != null && TGIsBlurEnabled.IsOn)
        {
            _blurEffectManager.NoiseBlendMode = (BlendEffectMode)value;
        }
    }

    private void BackdropImage_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (_blurEffectManager2 != null)
        {
            _blurEffectManager2.StartBlurAnimation(TimeSpan.FromMilliseconds(3000));
        }
    }

    private void BackdropImage_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (_blurEffectManager2 != null)
        {
            _blurEffectManager2.StartBlurReverseAnimation(TimeSpan.FromMilliseconds(3000));
        }
    }
}
