using Microsoft.UI.Xaml.Controls.Primitives;

namespace DevWinUI;

[TemplatePart(Name = PART_ProgressBar, Type = typeof(ProgressBar))]
public partial class ProgressButton : ToggleButton
{
    private const string PART_ProgressBar = "PART_ProgressBar";
    private ProgressBar progressBar;

    public SolidColorBrush ProgressBackground
    {
        get { return (SolidColorBrush)GetValue(ProgressBackgroundProperty); }
        set { SetValue(ProgressBackgroundProperty, value); }
    }

    public static readonly DependencyProperty ProgressBackgroundProperty =
        DependencyProperty.Register(nameof(ProgressBackground), typeof(SolidColorBrush), typeof(ProgressButton), new PropertyMetadata(null));

    public SolidColorBrush ProgressForeground
    {
        get { return (SolidColorBrush)GetValue(ProgressForegroundProperty); }
        set { SetValue(ProgressForegroundProperty, value); }
    }

    public static readonly DependencyProperty ProgressForegroundProperty =
        DependencyProperty.Register(nameof(ProgressForeground), typeof(SolidColorBrush), typeof(ProgressButton), new PropertyMetadata(null));

    public HorizontalAlignment ProgressRingHorizontalAlignment
    {
        get { return (HorizontalAlignment)GetValue(ProgressRingHorizontalAlignmentProperty); }
        set { SetValue(ProgressRingHorizontalAlignmentProperty, value); }
    }

    public static readonly DependencyProperty ProgressRingHorizontalAlignmentProperty =
        DependencyProperty.Register(nameof(ProgressRingHorizontalAlignment), typeof(HorizontalAlignment), typeof(ProgressButton), new PropertyMetadata(HorizontalAlignment.Right));

    public double ProgressRingSize
    {
        get { return (double)GetValue(ProgressRingSizeProperty); }
        set { SetValue(ProgressRingSizeProperty, value); }
    }

    public static readonly DependencyProperty ProgressRingSizeProperty =
        DependencyProperty.Register(nameof(ProgressRingSize), typeof(double), typeof(ProgressButton), new PropertyMetadata(24.0));

    public Visibility ProgressRingVisibility
    {
        get { return (Visibility)GetValue(ProgressRingVisibilityProperty); }
        set { SetValue(ProgressRingVisibilityProperty, value); }
    }

    public static readonly DependencyProperty ProgressRingVisibilityProperty =
        DependencyProperty.Register(nameof(ProgressRingVisibility), typeof(Visibility), typeof(ProgressButton), new PropertyMetadata(Visibility.Collapsed));

    public bool IsIndeterminateProgressRing
    {
        get { return (bool)GetValue(IsIndeterminateProgressRingProperty); }
        set { SetValue(IsIndeterminateProgressRingProperty, value); }
    }

    public static readonly DependencyProperty IsIndeterminateProgressRingProperty =
        DependencyProperty.Register(nameof(IsIndeterminateProgressRing), typeof(bool), typeof(ProgressButton), new PropertyMetadata(true));

    public Style ProgressRingStyle
    {
        get => (Style)GetValue(ProgressRingStyleProperty);
        set => SetValue(ProgressRingStyleProperty, value);
    }

    public static readonly DependencyProperty ProgressRingStyleProperty = DependencyProperty.Register(
        nameof(ProgressRingStyle), typeof(Style), typeof(ProgressButton), new PropertyMetadata(default(Style)));
    
    public double Progress
    {
        get { return (double)GetValue(ProgressProperty); }
        set { SetValue(ProgressProperty, value); }
    }

    public static readonly DependencyProperty ProgressProperty =
        DependencyProperty.Register(nameof(Progress), typeof(double), typeof(ProgressButton), new PropertyMetadata(0.0));

    public object CheckedContent
    {
        get { return (object)GetValue(CheckedContentProperty); }
        set { SetValue(CheckedContentProperty, value); }
    }

    public static readonly DependencyProperty CheckedContentProperty =
        DependencyProperty.Register(nameof(CheckedContent), typeof(object), typeof(ProgressButton), new PropertyMetadata(default(object)));

    public bool ShowError
    {
        get { return (bool)GetValue(ShowErrorProperty); }
        set { SetValue(ShowErrorProperty, value); }
    }

    public static readonly DependencyProperty ShowErrorProperty =
        DependencyProperty.Register(nameof(ShowError), typeof(bool), typeof(ProgressButton), new PropertyMetadata(false));

    public bool ShowPaused
    {
        get { return (bool)GetValue(ShowPausedProperty); }
        set { SetValue(ShowPausedProperty, value); }
    }

    public static readonly DependencyProperty ShowPausedProperty =
        DependencyProperty.Register(nameof(ShowPaused), typeof(bool), typeof(ProgressButton), new PropertyMetadata(false));

    public bool IsIndeterminate
    {
        get { return (bool)GetValue(IsIndeterminateProperty); }
        set { SetValue(IsIndeterminateProperty, value); }
    }

    public static readonly DependencyProperty IsIndeterminateProperty =
        DependencyProperty.Register(nameof(IsIndeterminate), typeof(bool), typeof(ProgressButton), new PropertyMetadata(false));

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        SizeChanged += ProgressButton_SizeChanged;
        progressBar = GetTemplateChild(PART_ProgressBar) as ProgressBar;
        if (progressBar != null)
        {
            progressBar.MinHeight = ActualHeight;
            progressBar.Loaded += ProgressBar_Loaded;
        }
    }

    private void ProgressBar_Loaded(object sender, RoutedEventArgs e)
    {
        if (progressBar != null)
        {
            progressBar.ApplyTemplate();
            var track = progressBar.FindDescendant("ProgressBarTrack");
            if (track != null)
            {
                track.VerticalAlignment = VerticalAlignment.Stretch;
                track.Height = double.NaN;
            }
        }
    }

    private void ProgressButton_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (progressBar != null)
        {
            progressBar.MinHeight = ActualHeight;
        }
    }
}
