using Microsoft.UI.Text;

namespace DevWinUI;

public partial class SpectrumVisualizer
{
    public Windows.UI.Text.FontWeight TitleFontWeight
    {
        get { return (Windows.UI.Text.FontWeight)GetValue(TitleFontWeightProperty); }
        set { SetValue(TitleFontWeightProperty, value); }
    }

    public static readonly DependencyProperty TitleFontWeightProperty =
        DependencyProperty.Register(nameof(TitleFontWeight), typeof(Windows.UI.Text.FontWeight), typeof(SpectrumVisualizer), new PropertyMetadata(FontWeights.Bold, OnTextFormatChanged));

    public CanvasHorizontalAlignment TitleHorizontalAlignment
    {
        get { return (CanvasHorizontalAlignment)GetValue(TitleHorizontalAlignmentProperty); }
        set { SetValue(TitleHorizontalAlignmentProperty, value); }
    }

    public static readonly DependencyProperty TitleHorizontalAlignmentProperty =
        DependencyProperty.Register(nameof(TitleHorizontalAlignment), typeof(CanvasHorizontalAlignment), typeof(SpectrumVisualizer), new PropertyMetadata(CanvasHorizontalAlignment.Center, OnTextFormatChanged));

    public CanvasWordWrapping TitleWordWrapping
    {
        get { return (CanvasWordWrapping)GetValue(TitleWordWrappingProperty); }
        set { SetValue(TitleWordWrappingProperty, value); }
    }

    public static readonly DependencyProperty TitleWordWrappingProperty =
        DependencyProperty.Register(nameof(TitleWordWrapping), typeof(CanvasWordWrapping), typeof(SpectrumVisualizer), new PropertyMetadata(CanvasWordWrapping.NoWrap, OnTextFormatChanged));

    public CanvasTrimmingSign TitleTrimmingSign
    {
        get { return (CanvasTrimmingSign)GetValue(TitleTrimmingSignProperty); }
        set { SetValue(TitleTrimmingSignProperty, value); }
    }

    public static readonly DependencyProperty TitleTrimmingSignProperty =
        DependencyProperty.Register(nameof(TitleTrimmingSign), typeof(CanvasTrimmingSign), typeof(SpectrumVisualizer), new PropertyMetadata(CanvasTrimmingSign.Ellipsis, OnTextFormatChanged));

    public CanvasTextTrimmingGranularity TitleTrimmingGranularity
    {
        get { return (CanvasTextTrimmingGranularity)GetValue(TitleTrimmingGranularityProperty); }
        set { SetValue(TitleTrimmingGranularityProperty, value); }
    }

    public static readonly DependencyProperty TitleTrimmingGranularityProperty =
        DependencyProperty.Register(nameof(TitleTrimmingGranularity), typeof(CanvasTextTrimmingGranularity), typeof(SpectrumVisualizer), new PropertyMetadata(CanvasTextTrimmingGranularity.Character, OnTextFormatChanged));

    public Windows.UI.Text.FontWeight ArtistFontWeight
    {
        get { return (Windows.UI.Text.FontWeight)GetValue(ArtistFontWeightProperty); }
        set { SetValue(ArtistFontWeightProperty, value); }
    }

    public static readonly DependencyProperty ArtistFontWeightProperty =
        DependencyProperty.Register(nameof(ArtistFontWeight), typeof(Windows.UI.Text.FontWeight), typeof(SpectrumVisualizer), new PropertyMetadata(FontWeights.Bold, OnTextFormatChanged));

    public CanvasHorizontalAlignment ArtistHorizontalAlignment
    {
        get { return (CanvasHorizontalAlignment)GetValue(ArtistHorizontalAlignmentProperty); }
        set { SetValue(ArtistHorizontalAlignmentProperty, value); }
    }

    public static readonly DependencyProperty ArtistHorizontalAlignmentProperty =
        DependencyProperty.Register(nameof(ArtistHorizontalAlignment), typeof(CanvasHorizontalAlignment), typeof(SpectrumVisualizer), new PropertyMetadata(CanvasHorizontalAlignment.Center, OnTextFormatChanged));

    public CanvasWordWrapping ArtistWordWrapping
    {
        get { return (CanvasWordWrapping)GetValue(ArtistWordWrappingProperty); }
        set { SetValue(ArtistWordWrappingProperty, value); }
    }

    public static readonly DependencyProperty ArtistWordWrappingProperty =
        DependencyProperty.Register(nameof(ArtistWordWrapping), typeof(CanvasWordWrapping), typeof(SpectrumVisualizer), new PropertyMetadata(CanvasWordWrapping.NoWrap, OnTextFormatChanged));

    public CanvasTrimmingSign ArtistTrimmingSign
    {
        get { return (CanvasTrimmingSign)GetValue(ArtistTrimmingSignProperty); }
        set { SetValue(ArtistTrimmingSignProperty, value); }
    }

    public static readonly DependencyProperty ArtistTrimmingSignProperty =
        DependencyProperty.Register(nameof(ArtistTrimmingSign), typeof(CanvasTrimmingSign), typeof(SpectrumVisualizer), new PropertyMetadata(CanvasTrimmingSign.Ellipsis, OnTextFormatChanged));

    public CanvasTextTrimmingGranularity ArtistTrimmingGranularity
    {
        get { return (CanvasTextTrimmingGranularity)GetValue(ArtistTrimmingGranularityProperty); }
        set { SetValue(ArtistTrimmingGranularityProperty, value); }
    }

    public static readonly DependencyProperty ArtistTrimmingGranularityProperty =
        DependencyProperty.Register(nameof(ArtistTrimmingGranularity), typeof(CanvasTextTrimmingGranularity), typeof(SpectrumVisualizer), new PropertyMetadata(CanvasTextTrimmingGranularity.Character, OnTextFormatChanged));

    private static void OnTextFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumVisualizer)d;
        if (ctl != null)
        {
            ctl.InitializeText();
            ctl.spectrumCanvasControl?.Invalidate();
        }
    }

    public bool ShowAlbumArt
    {
        get { return (bool)GetValue(ShowAlbumArtProperty); }
        set { SetValue(ShowAlbumArtProperty, value); }
    }

    public static readonly DependencyProperty ShowAlbumArtProperty =
        DependencyProperty.Register(nameof(ShowAlbumArt), typeof(bool), typeof(SpectrumVisualizer), new PropertyMetadata(true, OnShowAlbumArtChanged));

    private static void OnShowAlbumArtChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumVisualizer)d;
        if (ctl != null)
        {
            ctl._showAlbumArt = (bool)e.NewValue;
            ctl.spectrumCanvasControl?.Invalidate();
        }
    }

    public bool ShowTitle
    {
        get { return (bool)GetValue(ShowTitleProperty); }
        set { SetValue(ShowTitleProperty, value); }
    }

    public static readonly DependencyProperty ShowTitleProperty =
        DependencyProperty.Register(nameof(ShowTitle), typeof(bool), typeof(SpectrumVisualizer), new PropertyMetadata(true, OnShowTitleChanged));

    private static void OnShowTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumVisualizer)d;
        if (ctl != null)
        {
            ctl._showTitle = (bool)e.NewValue;
            ctl.spectrumCanvasControl?.Invalidate();
        }
    }
    public bool ShowArtist
    {
        get { return (bool)GetValue(ShowArtistProperty); }
        set { SetValue(ShowArtistProperty, value); }
    }

    public static readonly DependencyProperty ShowArtistProperty =
        DependencyProperty.Register(nameof(ShowArtist), typeof(bool), typeof(SpectrumVisualizer), new PropertyMetadata(true, OnShowArtistChanged));

    private static void OnShowArtistChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumVisualizer)d;
        if (ctl != null)
        {
            ctl._showArtist = (bool)e.NewValue;
            ctl.spectrumCanvasControl?.Invalidate();
        }
    }

    public SpectrumType SpectrumType
    {
        get { return (SpectrumType)GetValue(SpectrumTypeProperty); }
        set { SetValue(SpectrumTypeProperty, value); }
    }

    public static readonly DependencyProperty SpectrumTypeProperty =
        DependencyProperty.Register(nameof(SpectrumType), typeof(SpectrumType), typeof(SpectrumVisualizer), new PropertyMetadata(SpectrumType.Round, OnSpectrumModeChanged));

    private static void OnSpectrumModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumVisualizer)d;
        if (ctl != null)
        {
            ctl.spectrumMode = (SpectrumType)e.NewValue;
            ctl.spectrumCanvasControl?.Invalidate();
        }
    }

    public double RotationSpeed
    {
        get { return (double)GetValue(RotationSpeedProperty); }
        set { SetValue(RotationSpeedProperty, value); }
    }

    public static readonly DependencyProperty RotationSpeedProperty =
        DependencyProperty.Register(nameof(RotationSpeed), typeof(double), typeof(SpectrumVisualizer), new PropertyMetadata(10.0d, OnRotationSpeedChanged));

    private static void OnRotationSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumVisualizer)d;
        if (ctl != null)
        {
            ctl.rotationSpeed = (float)((double)e.NewValue);
            ctl.spectrumCanvasControl?.Invalidate();
        }
    }

    public double CoverOpacity
    {
        get { return (double)GetValue(CoverOpacityProperty); }
        set { SetValue(CoverOpacityProperty, value); }
    }

    public static readonly DependencyProperty CoverOpacityProperty =
        DependencyProperty.Register(nameof(CoverOpacity), typeof(double), typeof(SpectrumVisualizer), new PropertyMetadata(1.0d, OnCoverOpacityChanged));

    private static void OnCoverOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumVisualizer)d;
        if (ctl != null)
        {
            ctl.coverOpacity = (float)((double)e.NewValue);
            ctl.spectrumCanvasControl?.Invalidate();
        }
    }

    public double SpectrumOpacity
    {
        get { return (double)GetValue(SpectrumOpacityProperty); }
        set { SetValue(SpectrumOpacityProperty, value); }
    }

    public static readonly DependencyProperty SpectrumOpacityProperty =
        DependencyProperty.Register(nameof(SpectrumOpacity), typeof(double), typeof(SpectrumVisualizer), new PropertyMetadata(1.0d, OnSpectrumOpacityChanged));

    private static void OnSpectrumOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumVisualizer)d;
        if (ctl != null)
        {
            ctl.spectrumOpacity = (float)((double)e.NewValue);
            ctl.spectrumCanvasControl?.Invalidate();
        }
    }

    public double FontOpacity
    {
        get { return (double)GetValue(FontOpacityProperty); }
        set { SetValue(FontOpacityProperty, value); }
    }

    public static readonly DependencyProperty FontOpacityProperty =
        DependencyProperty.Register(nameof(FontOpacity), typeof(double), typeof(SpectrumVisualizer), new PropertyMetadata(1.0d, OnFontOpacityChanged));
    private static void OnFontOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumVisualizer)d;
        if (ctl != null)
        {
            ctl.fontOpacity = (float)((double)e.NewValue);
            ctl.spectrumCanvasControl?.Invalidate();
        }
    }

    public double SmoothingFactor
    {
        get { return (double)GetValue(SmoothingFactorProperty); }
        set { SetValue(SmoothingFactorProperty, value); }
    }

    public static readonly DependencyProperty SmoothingFactorProperty =
        DependencyProperty.Register(nameof(SmoothingFactor), typeof(double), typeof(SpectrumVisualizer), new PropertyMetadata(0.95d, OnSmoothingFactorChanged));

    private static void OnSmoothingFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumVisualizer)d;
        if (ctl != null)
        {
            ctl.smoothingFactor = (float)((double)e.NewValue);
            ctl.spectrumCanvasControl?.Invalidate();
        }
    }

    public double Sensitivity
    {
        get { return (double)GetValue(SensitivityProperty); }
        set { SetValue(SensitivityProperty, value); }
    }

    public static readonly DependencyProperty SensitivityProperty =
        DependencyProperty.Register(nameof(Sensitivity), typeof(double), typeof(SpectrumVisualizer), new PropertyMetadata(20.0d, OnSensitivityChanged));
    private static void OnSensitivityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumVisualizer)d;
        if (ctl != null)
        {
            ctl.sensitivity = (float)((double)e.NewValue);
            ctl.spectrumCanvasControl?.Invalidate();
        }
    }

    public SpectrumColorType ColorType
    {
        get { return (SpectrumColorType)GetValue(ColorTypeProperty); }
        set { SetValue(ColorTypeProperty, value); }
    }

    public static readonly DependencyProperty ColorTypeProperty =
        DependencyProperty.Register(nameof(ColorType), typeof(SpectrumColorType), typeof(SpectrumVisualizer), new PropertyMetadata(SpectrumColorType.GradientLoop, OnSpectrumColorChanged));

    private static void OnSpectrumColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumVisualizer)d;
        if (ctl != null)
        {
            ctl.colorType = (SpectrumColorType)e.NewValue;
            ctl.spectrumCanvasControl?.Invalidate();
        }
    }

    public ISpectrumAnalyzer Analyzer
    {
        get => _analyzer;
        set
        {
            if (_analyzer != null)
            {
                _analyzer.SpectrumDataUpdated -= OnSpectrumDataUpdated;
                _analyzer.Dispose();
            }

            _analyzer = value;

            if (_analyzer != null)
            {
                _analyzer.SpectrumDataUpdated += OnSpectrumDataUpdated;
                _analyzer.Start();
            }
        }
    }
}

