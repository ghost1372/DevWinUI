using Microsoft.UI.Xaml.Hosting;

namespace DevWinUIGallery.Views;

public sealed partial class BlurEffectControlPage : Page
{
    public BlurEffectControlPage()
    {
        InitializeComponent();
        CmbBlurSourceType.Value = BlurSourceType.Backdrop;
    }

    private void CmbBlurSourceType_ValueChanged(ChoiceOption sender, object value)
    {
        var sourceType = (BlurSourceType)value;
        switch (sourceType)
        {
            case BlurSourceType.Surface:
                BlurEffectControlSample.SurfaceBrushSource = BackdropImage.SurfaceBrush;
                break;
            case BlurSourceType.Visual:
                BlurEffectControlSample.VisualSource = ElementCompositionPreview.GetElementVisual(BackdropImage);
                break;
            case BlurSourceType.Custom:
                if (BlurEffectControlSample.GetCompositor() is { } compositor)
                {
                    BlurEffectControlSample.CustomSourceBrush = compositor.CreateColorBrush(Colors.Green);
                }
                break;
        }

        BlurEffectControlSample.SourceType = sourceType;
    }
}
