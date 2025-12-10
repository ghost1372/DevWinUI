using System.Collections.ObjectModel;
using DevWinUIGallery.Models;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI.Xaml.Media;

namespace DevWinUIGallery.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    public partial DigitalSegmentOption DigitalSegmentSelectedModel { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<SpectrumColorType> SpectrumColorTypeList { get; set; } = new ObservableCollection<SpectrumColorType>(Enum.GetValues<SpectrumColorType>());

    [ObservableProperty]
    public partial ObservableCollection<SpectrumType> SpectrumTypeList { get; set; } = new ObservableCollection<SpectrumType>(Enum.GetValues<SpectrumType>());

    [ObservableProperty]
    public partial ObservableCollection<CountdownState> CountdownState { get; set; } = new ObservableCollection<CountdownState>(Enum.GetValues<CountdownState>());

    [ObservableProperty]
    public partial ObservableCollection<ImageFrameTransitionMode> ImageFrameTransition { get; set; } = new ObservableCollection<ImageFrameTransitionMode>(Enum.GetValues<ImageFrameTransitionMode>());

    [ObservableProperty]
    public partial ObservableCollection<AlignmentX> ImageFrameAlignmentX { get; set; } = new ObservableCollection<AlignmentX>(Enum.GetValues<AlignmentX>());

    [ObservableProperty]
    public partial ObservableCollection<AlignmentY> ImageFrameAlignmentY { get; set; } = new ObservableCollection<AlignmentY>(Enum.GetValues<AlignmentY>());

    [ObservableProperty]
    public partial ObservableCollection<MenuFlyoutSecondaryMenuPlacement> SecondaryMenuPlacement { get; set; } = new ObservableCollection<MenuFlyoutSecondaryMenuPlacement>(Enum.GetValues<MenuFlyoutSecondaryMenuPlacement>());

    [ObservableProperty]
    public partial ObservableCollection<HorizontalAlignment> TitleHorizontalAlignment { get; set; } = new ObservableCollection<HorizontalAlignment>(Enum.GetValues<HorizontalAlignment>());

    [ObservableProperty]
    public partial ObservableCollection<SegmentedSliderTitleVisibility> SegmentedSliderTitleVisibility { get; set; } = new ObservableCollection<SegmentedSliderTitleVisibility>(Enum.GetValues<SegmentedSliderTitleVisibility>());

    [ObservableProperty]
    public partial ObservableCollection<FlipCardsSourceType> FlipCardsSourceItems { get; set; } = new ObservableCollection<FlipCardsSourceType>(Enum.GetValues<FlipCardsSourceType>());

    [ObservableProperty]
    public partial ObservableCollection<Orientation> OrientationItems { get; set; } = new ObservableCollection<Orientation>(Enum.GetValues<Orientation>());

    [ObservableProperty]
    public partial ObservableCollection<BackdropType> BackdropItems { get; set; } = new ObservableCollection<BackdropType>(Enum.GetValues<BackdropType>());

    [ObservableProperty]
    public partial ObservableCollection<ElementTheme> ThemeItems { get; set; } = new ObservableCollection<ElementTheme>(Enum.GetValues<ElementTheme>());

    #region WindowedContentDialog
    [ObservableProperty]
    public partial ObservableCollection<MessageBoxIcon> MessageBoxIconItems { get; set; } = new ObservableCollection<MessageBoxIcon>(Enum.GetValues<MessageBoxIcon>());

    [ObservableProperty]
    public partial ObservableCollection<UnderlayCoverMode> UnderlayCoverModeItems { get; set; } = new ObservableCollection<UnderlayCoverMode>(Enum.GetValues<UnderlayCoverMode>());

    [ObservableProperty]
    public partial ObservableCollection<UnderlayMode> UnderlayModeItems { get; set; } = new ObservableCollection<UnderlayMode>(Enum.GetValues<UnderlayMode>());
    #endregion

    #region SwitchPresenter
    [ObservableProperty]
    public partial RadioButton RadioSelectedItem { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<Animal> SwitchPresenterItems { get; set; } = new ObservableCollection<Animal>(Enum.GetValues<Animal>());

    #endregion

    #region Picker

    [ObservableProperty]
    public partial ObservableCollection<Windows.Storage.Pickers.PickerLocationId> PickerLocationItems { get; set; } = new ObservableCollection<Windows.Storage.Pickers.PickerLocationId>(Enum.GetValues<Windows.Storage.Pickers.PickerLocationId>());

    #endregion

    #region KeyVisual
    [ObservableProperty]
    public partial ObservableCollection<VisualType> VisualTypeItems { get; set; } = new ObservableCollection<VisualType>(Enum.GetValues<VisualType>());
    #endregion

    #region AnimatedImage
    private DispatcherTimer _timer;
    private readonly Random _random = new();

    private List<string> imagePaths { get; set; } = new List<string>
    {
        "ms-appx:///Assets/Others/Girl.jpg",
        "ms-appx:///Assets/Others/GirlAfter.png",
        "ms-appx:///Assets/Others/GirlBefore.png",
        "ms-appx:///Assets/Others/GirlBlur.jpg",
        "ms-appx:///Assets/Others/Profile.png"
    };

    [ObservableProperty]
    public partial string ImageUrl { get; set; } = "ms-appx:///Assets/Others/Girl.jpg";

    public void InitAnimatedImageTimer()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2)
        };
        _timer.Tick -= Timer_Tick;
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }
    private void Timer_Tick(object sender, object e)
    {
        if (imagePaths.Count > 0)
        {
            var index = _random.Next(imagePaths.Count);
            ImageUrl = imagePaths[index];
        }
    }
    public void StopTimer()
    {
        _timer?.Stop();
        _timer = null;
    }
    #endregion

    #region ForegroundFocusEffect
    [ObservableProperty]
    public partial ObservableCollection<ForegroundFocusEffectTypes> ForegroundFocusEffectItems { get; set; } = new ObservableCollection<ForegroundFocusEffectTypes>(Enum.GetValues<ForegroundFocusEffectTypes>());

    #endregion

    #region BlurEffectManager
    [ObservableProperty]
    public partial ObservableCollection<BlurSourceType> BlurSourceTypeItems { get; set; } = new ObservableCollection<BlurSourceType>(Enum.GetValues<BlurSourceType>());
    [ObservableProperty]
    public partial ObservableCollection<EffectBorderMode> EffectBorderModeItems { get; set; } = new ObservableCollection<EffectBorderMode>(Enum.GetValues<EffectBorderMode>());
    [ObservableProperty]
    public partial ObservableCollection<EffectOptimization> EffectOptimizationItems { get; set; } = new ObservableCollection<EffectOptimization>(Enum.GetValues<EffectOptimization>());
    [ObservableProperty]
    public partial ObservableCollection<BlendEffectMode> BlendEffectModeItems { get; set; } = new ObservableCollection<BlendEffectMode>(Enum.GetValues<BlendEffectMode>());

    #endregion

    #region ColorPalette

    [ObservableProperty]
    public partial ObservableCollection<ColorSetType> ColorSetItems { get; set; } = new ObservableCollection<ColorSetType>(Enum.GetValues<ColorSetType>());

    [ObservableProperty]
    public partial ObservableCollection<ColorItemShape> ColorItemShapeItems { get; set; } = new ObservableCollection<ColorItemShape>(Enum.GetValues<ColorItemShape>());

    #endregion

    #region ArcProgress

    [ObservableProperty]
    public partial ObservableCollection<ArcProgressFillAnimationState> FillAnimationStateItems { get; set; } = new ObservableCollection<ArcProgressFillAnimationState>(Enum.GetValues<ArcProgressFillAnimationState>());

    [ObservableProperty]
    public partial ObservableCollection<SweepDirection> SweepDirectionItems { get; set; } = new ObservableCollection<SweepDirection>(Enum.GetValues<SweepDirection>());

    #endregion

    #region MessageBox

    [ObservableProperty]
    public partial ObservableCollection<MessageBoxButtons> MessageBoxButtonItems { get; set; } = new ObservableCollection<MessageBoxButtons>(Enum.GetValues<MessageBoxButtons>());

    [ObservableProperty]
    public partial ObservableCollection<MessageBoxDefaultButton> MessageBoxDefaultButtonItems { get; set; } = new ObservableCollection<MessageBoxDefaultButton>(Enum.GetValues<MessageBoxDefaultButton>());

    #endregion

    #region StepBar
    [ObservableProperty]
    public partial ObservableCollection<StepBarDisplayMode> StepBarDisplayModeItems { get; set; } = new ObservableCollection<StepBarDisplayMode>(Enum.GetValues<StepBarDisplayMode>());

    [ObservableProperty]
    public partial ObservableCollection<StepBarHeaderDisplayMode> StepBarHeaderDisplayModeItems { get; set; } = new ObservableCollection<StepBarHeaderDisplayMode>(Enum.GetValues<StepBarHeaderDisplayMode>());

    [ObservableProperty]
    public partial ObservableCollection<StepStatus> StepStatusItems { get; set; } = new ObservableCollection<StepStatus>(Enum.GetValues<StepStatus>());

    #endregion

    [ObservableProperty]
    public partial ObservableCollection<SampleData> SampleImageAndTextData { get; set; } = new ObservableCollection<SampleData>()
    {
        new SampleData("Sample 1", "ms-appx:///Assets/Landscapes/Landscape-1.jpg", "Sample Desc 1"),
        new SampleData("Sample 2", "ms-appx:///Assets/Landscapes/Landscape-2.jpg", "Sample Desc 2"),
        new SampleData("Sample 3", "ms-appx:///Assets/Landscapes/Landscape-3.jpg", "Sample Desc 3"),
        new SampleData("Sample 4", "ms-appx:///Assets/Landscapes/Landscape-4.jpg", "Sample Desc 4"),
        new SampleData("Sample 5", "ms-appx:///Assets/Landscapes/Landscape-5.jpg", "Sample Desc 5"),
        new SampleData("Sample 6", "ms-appx:///Assets/Landscapes/Landscape-6.jpg", "Sample Desc 6"),
        new SampleData("Sample 7", "ms-appx:///Assets/Landscapes/Landscape-7.jpg", "Sample Desc 7"),
        new SampleData("Sample 8", "ms-appx:///Assets/Landscapes/Landscape-8.jpg", "Sample Desc 8"),
        new SampleData("Sample 9", "ms-appx:///Assets/Landscapes/Landscape-9.jpg", "Sample Desc 9"),
        new SampleData("Sample 10", "ms-appx:///Assets/Landscapes/Landscape-10.jpg", "Sample Desc 10"),
        new SampleData("Sample 11", "ms-appx:///Assets/Landscapes/Landscape-11.jpg", "Sample Desc 11"),
        new SampleData("Sample 12", "ms-appx:///Assets/Landscapes/Landscape-12.jpg", "Sample Desc 12"),
        new SampleData("Sample 13", "ms-appx:///Assets/Landscapes/Landscape-13.jpg", "Sample Desc 13")
    };


    [ObservableProperty]
    public partial ObservableCollection<DepthLayerSampleData> DepthLayerSampleItems { get; set; } = new ObservableCollection<DepthLayerSampleData>()
    {
        new DepthLayerSampleData(2021,"ms-appx:///Assets/Landscapes/Landscape-1.jpg"),
        new DepthLayerSampleData(2022,"ms-appx:///Assets/Landscapes/Landscape-2.jpg"),
        new DepthLayerSampleData(2023,"ms-appx:///Assets/Landscapes/Landscape-3.jpg"),
        new DepthLayerSampleData(2024,"ms-appx:///Assets/Landscapes/Landscape-4.jpg"),
        new DepthLayerSampleData(2025,"ms-appx:///Assets/Landscapes/Landscape-5.jpg"),
    };
}
