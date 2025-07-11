using DevWinUI;
using Microsoft.Graphics.Canvas.Effects;

namespace DevWinUIGallery.Views;
public sealed partial class BlurEffectControlPage : Page
{
    public BaseViewModel ViewModel { get; }
    public BlurEffectControlPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        this.InitializeComponent();
    }

    private void CmbBlurSourceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CmbBlurSourceType != null && TGIsBlurEnabled.IsOn)
        {
            BlurEffectControlSample.SourceType = (BlurSourceType)CmbBlurSourceType.SelectedIndex;
        }
    }

    private void CmbEffectBorderMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CmbEffectBorderMode != null && TGIsBlurEnabled.IsOn)
        {
            BlurEffectControlSample.BorderMode = (EffectBorderMode)CmbEffectBorderMode.SelectedIndex;
        }
    }

    private void CmbEffectOptimization_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CmbEffectOptimization != null && TGIsBlurEnabled.IsOn)
        {
            BlurEffectControlSample.Optimization = (EffectOptimization)CmbEffectOptimization.SelectedIndex;
        }
    }

    private void CmbBlendEffectMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CmbBlendEffectMode != null && TGIsBlurEnabled.IsOn)
        {
            BlurEffectControlSample.BlendMode = (BlendEffectMode)CmbBlendEffectMode.SelectedIndex;
        }
    }
    private void CmbNoiseBlendMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CmbNoiseBlendMode != null && TGIsBlurEnabled.IsOn)
        {
            BlurEffectControlSample.NoiseBlendMode = (BlendEffectMode)CmbNoiseBlendMode.SelectedIndex;
        }
    }
}
