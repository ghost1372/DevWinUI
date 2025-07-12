using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Hosting;

namespace DevWinUIGallery.Views;

public sealed partial class BlurEffectManagerPage : Page
{
    private BlurEffectManager _blurEffectManager;
    private BlurEffectManager _blurEffectManager2;
    public BaseViewModel ViewModel { get; }
    public BlurEffectManagerPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
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

    private void CmbBlurSourceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_blurEffectManager != null && CmbBlurSourceType != null && TGIsBlurEnabled.IsOn)
        {
            switch ((BlurSourceType)CmbBlurSourceType.SelectedIndex)
            {
                case BlurSourceType.Backdrop:
                    break;
                case BlurSourceType.Surface:
                    _blurEffectManager.SurfaceBrushSource = BackdropImage.SurfaceBrush;
                    //var surface = LoadedImageSurface.StartLoadFromUri(new Uri(BackdropImage.Source.AbsoluteUri));
                    //_blurEffectManager.SurfaceSource = surface;
                    break;
                case BlurSourceType.Visual:
                    Visual visual = ElementCompositionPreview.GetElementVisual(BackdropImage);
                    _blurEffectManager.VisualSource = visual;
                    break;
                case BlurSourceType.Custom:
                    _blurEffectManager.CustomSourceBrush = _blurEffectManager.GetCompositor().CreateColorBrush(Colors.Green);
                    break;
            }
            _blurEffectManager.SourceType = (BlurSourceType)CmbBlurSourceType.SelectedIndex;
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
