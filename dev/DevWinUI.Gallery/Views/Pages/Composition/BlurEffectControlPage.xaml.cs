using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Hosting;

namespace DevWinUIGallery.Views;
public sealed partial class BlurEffectControlPage : Page
{
    public ObservableCollection<BlurSourceType> BlurSourceTypeItems { get; set; } = new ObservableCollection<BlurSourceType>(Enum.GetValues<BlurSourceType>());
    public ObservableCollection<EffectBorderMode> EffectBorderModeItems { get; set; } = new ObservableCollection<EffectBorderMode>(Enum.GetValues<EffectBorderMode>());
    public ObservableCollection<EffectOptimization> EffectOptimizationItems { get; set; } = new ObservableCollection<EffectOptimization>(Enum.GetValues<EffectOptimization>());
    public ObservableCollection<BlendEffectMode> BlendEffectModeItems { get; set; } = new ObservableCollection<BlendEffectMode>(Enum.GetValues<BlendEffectMode>());

    public BlurEffectControlPage()
    {
        this.InitializeComponent();
    }

    private void CmbBlurSourceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CmbBlurSourceType != null && TGIsBlurEnabled.IsOn)
        {
            switch ((BlurSourceType)CmbBlurSourceType.SelectedIndex)
            {
                case BlurSourceType.Surface:
                    BlurEffectControlSample.SurfaceBrushSource = BackdropImage.SurfaceBrush;
                    //var surface = LoadedImageSurface.StartLoadFromUri(new Uri(BackdropImage.Source.AbsoluteUri));
                    //BlurEffectControlSample.SurfaceSource = surface;
                    break;
                case BlurSourceType.Visual:
                    Visual visual = ElementCompositionPreview.GetElementVisual(BackdropImage);
                    BlurEffectControlSample.VisualSource = visual;
                    break;
                case BlurSourceType.Custom:
                    BlurEffectControlSample.CustomSourceBrush = BlurEffectControlSample.GetCompositor().CreateColorBrush(Colors.Green);
                    break;
            }
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
