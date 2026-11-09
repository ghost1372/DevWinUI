namespace DevWinUI;

public sealed partial class GrowlOptions
{
    public string Title { get; set; } = string.Empty;

    public string SubTitle { get; set; } = string.Empty;

    public object? Message { get; set; }

    public IconElement? Icon { get; set; }

    public Brush? IconBackground { get; set; }

    public Brush? IconForeground { get; set; }

    public Brush? IconBorderBrush { get; set; }

    public GrowlCardIconStyle IconStyle { get; set; } = GrowlCardIconStyle.Filled;

    public bool ShowIconContainer { get; set; } = true;

    public object? ActionContent { get; set; }

    public GrowlCardSeverity Severity { get; set; } = GrowlCardSeverity.Attention;

    public GrowlCardColorStyle? ColorStyle { get; set; }

    public GrowlPosition? Position { get; set; }

    public GrowlScope Scope { get; set; } = GrowlScope.InApp;

    public GrowlMonitor? Monitor { get; set; }

    public GrowlCardDisplayMode DisplayMode { get; set; } = GrowlCardDisplayMode.Card;

    public GrowlShowAnimation? ShowAnimation { get; set; }

    public GrowlCloseAnimation? CloseAnimation { get; set; }

    public GrowlCardAutoClose AutoClose { get; set; } = GrowlCardAutoClose.Default;

    public TimeSpan AutoCloseDuration { get; set; } = TimeSpan.FromSeconds(5);

    public bool PauseAutoCloseOnPointerOver { get; set; } = true;

    public bool ShowCloseButton { get; set; } = true;

    public GrowlCardSentTimeVisibility SentTimeVisibility { get; set; } = GrowlCardSentTimeVisibility.OnHover;

    public DateTimeOffset? SentAt { get; set; }

    public bool ShowIndicator { get; set; } = true;

    public bool ShowGradientBackground { get; set; }

    public GrowlCardGradientIntensity GradientIntensity { get; set; } = GrowlCardGradientIntensity.Normal;

    public GrowlCardIndicatorPlacement IndicatorPlacement { get; set; } = GrowlCardIndicatorPlacement.Left;

    public GrowlCardCountdownSeverity CountdownSeverity { get; set; } = GrowlCardCountdownSeverity.Default;

    public GrowlCardCountdownColorStyle CountdownColorStyle { get; set; } = GrowlCardCountdownColorStyle.Default;

    public GrowlCardCountdownPlacement CountdownPlacement { get; set; } = GrowlCardCountdownPlacement.Default;

    public bool ShowCountdown { get; set; }

    public bool ShowCountdownText { get; set; } = true;

    public double IndicatorThickness { get; set; } = 3;

    public bool ShowProgress { get; set; }

    public bool ShowProgressText { get; set; } = true;

    public GrowlCardProgressType ProgressType { get; set; } = GrowlCardProgressType.Bar;

    public double ProgressMaximum { get; set; } = 100;

    public double Progress { get; set; }

    public bool IsProgressIndeterminate { get; set; }

    public bool IsProgressError { get; set; }

    public bool IsProgressPaused { get; set; }

    public double? Width { get; set; }

    public string? Key { get; set; }

    public GrowlSound? Sound { get; set; }

    public Uri? SoundUri { get; set; }

    public bool CloseOnInvoke { get; set; }

    public Action<GrowlHandle>? Invoked { get; set; }

    public Action<GrowlCard>? Configure { get; set; }
}
