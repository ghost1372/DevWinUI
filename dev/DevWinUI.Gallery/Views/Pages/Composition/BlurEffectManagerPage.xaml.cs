using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;

namespace DevWinUIGallery.Views;

public sealed partial class BlurEffectManagerPage : Page
{
    public ObservableCollection<BlurSourceType> BlurSourceTypeItems { get; set; } = new ObservableCollection<BlurSourceType>(Enum.GetValues<BlurSourceType>());
    public ObservableCollection<EffectBorderMode> EffectBorderModeItems { get; set; } = new ObservableCollection<EffectBorderMode>(Enum.GetValues<EffectBorderMode>());
    public ObservableCollection<EffectOptimization> EffectOptimizationItems { get; set; } = new ObservableCollection<EffectOptimization>(Enum.GetValues<EffectOptimization>());
    public ObservableCollection<BlendEffectMode> BlendEffectModeItems { get; set; } = new ObservableCollection<BlendEffectMode>(Enum.GetValues<BlendEffectMode>());

    private BlurEffectManager _blurEffectManager;
    private BlurEffectManager _blurEffectManager2;
    public BlurEffectManagerPage()
    {
        InitializeComponent();
        _blurEffectManager = new BlurEffectManager(BackdropImage) { IsTintEnabled = true };
        _blurEffectManager2 = new BlurEffectManager(BackdropImage2);
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

    private void CpTintColor_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
    {
        if (_blurEffectManager != null && TGIsBlurEnabled.IsOn)
        {
            _blurEffectManager.TintColor = args.NewColor;
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

    private void CmbEffectBorderMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_blurEffectManager != null && CmbEffectBorderMode != null && TGIsBlurEnabled.IsOn)
        {
            _blurEffectManager.BorderMode = (EffectBorderMode)CmbEffectBorderMode.SelectedIndex;
        }
    }

    private void CmbEffectOptimization_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_blurEffectManager != null && CmbEffectOptimization != null && TGIsBlurEnabled.IsOn)
        {
            _blurEffectManager.Optimization = (EffectOptimization)CmbEffectOptimization.SelectedIndex;
        }
    }

    private void CmbBlendEffectMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_blurEffectManager != null && CmbBlendEffectMode != null && TGIsBlurEnabled.IsOn)
        {
            _blurEffectManager.BlendMode = (BlendEffectMode)CmbBlendEffectMode.SelectedIndex;
        }
    }
    private void CmbNoiseBlendMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_blurEffectManager != null && CmbNoiseBlendMode != null && TGIsBlurEnabled.IsOn)
        {
            _blurEffectManager.NoiseBlendMode = (BlendEffectMode)CmbNoiseBlendMode.SelectedIndex;
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
