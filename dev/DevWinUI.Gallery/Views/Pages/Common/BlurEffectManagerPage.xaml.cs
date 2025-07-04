namespace DevWinUIGallery.Views;

public sealed partial class BlurEffectManagerPage : Page
{
    private BlurEffectManager _blurEffectManager;
    public BlurEffectManagerPage()
    {
        InitializeComponent();
        _blurEffectManager = new BlurEffectManager(BackdropImage);
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
            var value = 10f;
            if (TGSlider != null)
            {
                value = (float)TGSlider.Value;
            }
            _blurEffectManager.EnableBlur(value);
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
            _blurEffectManager.UpdateBlurAmount((float)e.NewValue);
        }
    }
}
