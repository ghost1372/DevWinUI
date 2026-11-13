using Microsoft.UI.Xaml.Media;

namespace DevWinUIGallery;

public sealed partial class ControlExample
{
    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(ControlExample), new PropertyMetadata(null, OnChromeChanged));

    public string Description
    {
        get { return (string)GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(ControlExample), new PropertyMetadata(null, OnChromeChanged));

    public DocType DocType
    {
        get { return (DocType)GetValue(DocTypeProperty); }
        set { SetValue(DocTypeProperty, value); }
    }

    public static readonly DependencyProperty DocTypeProperty =
        DependencyProperty.Register(nameof(DocType), typeof(DocType), typeof(ControlExample), new PropertyMetadata(DocType.Controls));

    public string Doc
    {
        get { return (string)GetValue(DocProperty); }
        set { SetValue(DocProperty, value); }
    }

    public static readonly DependencyProperty DocProperty =
        DependencyProperty.Register(nameof(Doc), typeof(string), typeof(ControlExample), new PropertyMetadata(null, OnDocChanged));

    public string Xaml
    {
        get { return (string)GetValue(XamlProperty); }
        set { SetValue(XamlProperty, value); }
    }

    public static readonly DependencyProperty XamlProperty =
        DependencyProperty.Register(nameof(Xaml), typeof(string), typeof(ControlExample), new PropertyMetadata(null, OnCodeChanged));

    public string XamlCode
    {
        get { return (string)GetValue(XamlCodeProperty); }
        set { SetValue(XamlCodeProperty, value); }
    }

    public static readonly DependencyProperty XamlCodeProperty =
        DependencyProperty.Register(nameof(XamlCode), typeof(string), typeof(ControlExample), new PropertyMetadata(null, OnCodeChanged));

    public string CSharp
    {
        get { return (string)GetValue(CSharpProperty); }
        set { SetValue(CSharpProperty, value); }
    }

    public static readonly DependencyProperty CSharpProperty =
        DependencyProperty.Register(nameof(CSharp), typeof(string), typeof(ControlExample), new PropertyMetadata(null, OnCodeChanged));

    public string CSharpCode
    {
        get { return (string)GetValue(CSharpCodeProperty); }
        set { SetValue(CSharpCodeProperty, value); }
    }

    public static readonly DependencyProperty CSharpCodeProperty =
        DependencyProperty.Register(nameof(CSharpCode), typeof(string), typeof(ControlExample), new PropertyMetadata(null, OnCodeChanged));

    public object Example
    {
        get { return GetValue(ExampleProperty); }
        set { SetValue(ExampleProperty, value); }
    }

    public static readonly DependencyProperty ExampleProperty =
        DependencyProperty.Register(nameof(Example), typeof(object), typeof(ControlExample), new PropertyMetadata(null, OnSamplesSourceChanged));

    public DependencyObjectCollection Samples
    {
        get { return (DependencyObjectCollection)GetValue(SamplesProperty); }
        set { SetValue(SamplesProperty, value); }
    }

    public static readonly DependencyProperty SamplesProperty =
        DependencyProperty.Register(nameof(Samples), typeof(DependencyObjectCollection), typeof(ControlExample), new PropertyMetadata(null));

    public object Options
    {
        get { return GetValue(OptionsProperty); }
        set { SetValue(OptionsProperty, value); }
    }

    public static readonly DependencyProperty OptionsProperty =
        DependencyProperty.Register(nameof(Options), typeof(object), typeof(ControlExample), new PropertyMetadata(null, OnOptionsChanged));

    public DependencyObjectCollection OptionGroups
    {
        get { return (DependencyObjectCollection)GetValue(OptionGroupsProperty); }
        set { SetValue(OptionGroupsProperty, value); }
    }

    public static readonly DependencyProperty OptionGroupsProperty =
        DependencyProperty.Register(nameof(OptionGroups), typeof(DependencyObjectCollection), typeof(ControlExample), new PropertyMetadata(null));

    public double PaneMaxHeight
    {
        get { return (double)GetValue(PaneMaxHeightProperty); }
        set { SetValue(PaneMaxHeightProperty, value); }
    }

    public static readonly DependencyProperty PaneMaxHeightProperty =
        DependencyProperty.Register(nameof(PaneMaxHeight), typeof(double), typeof(ControlExample), new PropertyMetadata(480d, OnChromeChanged));

    public double PaneWidth
    {
        get { return (double)GetValue(PaneWidthProperty); }
        set { SetValue(PaneWidthProperty, value); }
    }

    public static readonly DependencyProperty PaneWidthProperty =
        DependencyProperty.Register(nameof(PaneWidth), typeof(double), typeof(ControlExample), new PropertyMetadata(300d, OnChromeChanged));

    public bool IsResetInHeader
    {
        get { return (bool)GetValue(IsResetInHeaderProperty); }
        set { SetValue(IsResetInHeaderProperty, value); }
    }

    public static readonly DependencyProperty IsResetInHeaderProperty =
        DependencyProperty.Register(nameof(IsResetInHeader), typeof(bool), typeof(ControlExample), new PropertyMetadata(false, OnChromeChanged));

    public bool IsCodeExpanded
    {
        get { return (bool)GetValue(IsCodeExpandedProperty); }
        set { SetValue(IsCodeExpandedProperty, value); }
    }

    public static readonly DependencyProperty IsCodeExpandedProperty =
        DependencyProperty.Register(nameof(IsCodeExpanded), typeof(bool), typeof(ControlExample), new PropertyMetadata(false));

    public bool IsPaneOpen
    {
        get { return (bool)GetValue(IsPaneOpenProperty); }
        set { SetValue(IsPaneOpenProperty, value); }
    }

    public static readonly DependencyProperty IsPaneOpenProperty =
        DependencyProperty.Register(nameof(IsPaneOpen), typeof(bool), typeof(ControlExample), new PropertyMetadata(true, OnChromeChanged));

    public ExampleToolbarPlacement ToolbarPlacement
    {
        get { return (ExampleToolbarPlacement)GetValue(ToolbarPlacementProperty); }
        set { SetValue(ToolbarPlacementProperty, value); }
    }

    public static readonly DependencyProperty ToolbarPlacementProperty =
        DependencyProperty.Register(nameof(ToolbarPlacement), typeof(ExampleToolbarPlacement), typeof(ControlExample), new PropertyMetadata(ExampleToolbarPlacement.Header, OnChromeChanged));

    public PreviewBackgroundPattern BackgroundPattern
    {
        get { return (PreviewBackgroundPattern)GetValue(BackgroundPatternProperty); }
        set { SetValue(BackgroundPatternProperty, value); }
    }

    public static readonly DependencyProperty BackgroundPatternProperty =
        DependencyProperty.Register(nameof(BackgroundPattern), typeof(PreviewBackgroundPattern), typeof(ControlExample), new PropertyMetadata(PreviewBackgroundPattern.Dotted, OnBackgroundPatternChanged));

    public Brush GradientBrush
    {
        get { return (Brush)GetValue(GradientBrushProperty); }
        set { SetValue(GradientBrushProperty, value); }
    }

    public static readonly DependencyProperty GradientBrushProperty =
        DependencyProperty.Register(nameof(GradientBrush), typeof(Brush), typeof(ControlExample), new PropertyMetadata(null, OnVisualChanged));

    public double GradientOpacity
    {
        get { return (double)GetValue(GradientOpacityProperty); }
        set { SetValue(GradientOpacityProperty, value); }
    }

    public static readonly DependencyProperty GradientOpacityProperty =
        DependencyProperty.Register(nameof(GradientOpacity), typeof(double), typeof(ControlExample), new PropertyMetadata(0.25, OnVisualChanged));

    public ExampleTintSource TintSource
    {
        get { return (ExampleTintSource)GetValue(TintSourceProperty); }
        set { SetValue(TintSourceProperty, value); }
    }

    public static readonly DependencyProperty TintSourceProperty =
        DependencyProperty.Register(nameof(TintSource), typeof(ExampleTintSource), typeof(ControlExample), new PropertyMetadata(ExampleTintSource.Default, OnVisualChanged));

    public double HeaderTintOpacity
    {
        get { return (double)GetValue(HeaderTintOpacityProperty); }
        set { SetValue(HeaderTintOpacityProperty, value); }
    }

    public static readonly DependencyProperty HeaderTintOpacityProperty =
        DependencyProperty.Register(nameof(HeaderTintOpacity), typeof(double), typeof(ControlExample), new PropertyMetadata(0.35, OnVisualChanged));

    public bool IsGlowEnabled
    {
        get { return (bool)GetValue(IsGlowEnabledProperty); }
        set { SetValue(IsGlowEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsGlowEnabledProperty =
        DependencyProperty.Register(nameof(IsGlowEnabled), typeof(bool), typeof(ControlExample), new PropertyMetadata(true, OnIsGlowEnabledChanged));
}
