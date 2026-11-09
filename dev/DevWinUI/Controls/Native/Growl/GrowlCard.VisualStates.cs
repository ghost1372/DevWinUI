using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

public sealed partial class GrowlCard
{
    private void GoTo(string state)
    {
        VisualStateManager.GoToState(this, state, false);
    }

    private void GoTo(bool condition, string whenTrue, string whenFalse)
    {
        GoTo(condition ? whenTrue : whenFalse);
    }

    private Color ResolveIndicatorColor(GrowlCardSeverity severity)
    {
        return _surface?.Resources[$"{severity}IndicatorBrush"] is SolidColorBrush brush
            ? brush.Color
            : _indicatorBrush.Color;
    }

    private void UpdateVisualStates()
    {
        GoTo($"{DisplayMode}Mode");
        UpdateVisibility();
        UpdateIndicator();
        UpdateIndicatorFill();
        UpdateCountdownFill();
        UpdateIconStates();
        UpdateRingStatus();
        UpdateGradient();
    }

    private void UpdateGradient()
    {
        GoTo(ShowGradientBackground ? $"Gradient{Severity}" : "GradientHidden");
        GoTo($"GradientIntensity{GradientIntensity}");
    }

    private void UpdateRingStatus()
    {
        GoTo(IsProgressError ? "RingError" : IsProgressPaused ? "RingPaused" : "RingNormal");
    }

    private void UpdateVisibility()
    {
        bool hasIcon = ShowIcon;
        bool hasTitle = ShowTitle && !string.IsNullOrEmpty(Title);
        bool hasSubTitle = ShowSubTitle && !string.IsNullOrEmpty(SubTitle);
        bool hasText = hasTitle || hasSubTitle;
        bool hasContent = ShowContent && Content is not null;
        bool hasAction = ShowActionContent && ActionContent is not null;
        bool hasCountdownText = IsAutoCloseEnabled && ShowCountdown && ShowCountdownText;
        bool hasSentTime = SentTimeVisibility != GrowlCardSentTimeVisibility.Hidden;
        bool showBar = ShowProgress && ProgressType == GrowlCardProgressType.Bar;
        bool showRing = ShowProgress && ProgressType == GrowlCardProgressType.Ring;
        bool hasHeader = hasIcon || hasText || ShowCloseButton || hasCountdownText || hasSentTime || showRing;

        GoTo(hasIcon, "IconVisible", "IconCollapsed");
        GoTo(hasTitle, "TitleVisible", "TitleCollapsed");
        GoTo(hasSubTitle, "SubTitleVisible", "SubTitleCollapsed");
        GoTo(hasText, "TextVisible", "TextCollapsed");
        GoTo(ShowCloseButton, "CloseButtonVisible", "CloseButtonCollapsed");
        GoTo(hasCountdownText, "CountdownTextVisible", "CountdownTextCollapsed");
        UpdateSentTimeState();
        GoTo(hasContent, "ContentVisible", "ContentCollapsed");
        GoTo(showBar, "ProgressBarVisible", "ProgressBarCollapsed");
        GoTo(showRing, "RingVisible", "RingCollapsed");
        GoTo(ShowProgressText && !IsProgressIndeterminate, "ProgressTextVisible", "ProgressTextCollapsed");
        GoTo(ShowProgressText && !IsProgressIndeterminate, "RingTextVisible", "RingTextCollapsed");
        GoTo(hasAction, "ActionVisible", "ActionCollapsed");

        GoTo(hasContent && (IsBanner ? hasText : hasHeader), "ContentGapVisible", "ContentGapCollapsed");
        GoTo(showBar && (hasHeader || hasContent || (IsBanner && hasAction)), "ProgressGapVisible", "ProgressGapCollapsed");
        GoTo(hasAction && (hasHeader || hasContent || showBar), "ActionGapVisible", "ActionGapCollapsed");
    }

    private void UpdateDisplayMode()
    {
        if (IsBanner)
        {
            if (ReadLocalValue(WidthProperty) == DependencyProperty.UnsetValue)
            {
                Width = double.NaN;
                _widthManaged = true;
            }
        }
        else if (_widthManaged)
        {
            if (double.IsNaN(Width))
            {
                ClearValue(WidthProperty);
            }

            _widthManaged = false;
        }

        UpdateVisualStates();
    }

    private void UpdateIndicator()
    {
        bool hasCountdown = IsAutoCloseEnabled && ShowCountdown;
        bool countdownAtTop = CountdownPlacement switch
        {
            GrowlCardCountdownPlacement.Top => true,
            GrowlCardCountdownPlacement.Bottom => false,
            _ => IndicatorPlacement == GrowlCardIndicatorPlacement.Bottom
        };

        GoTo(ShowIndicator ? $"Indicator{IndicatorPlacement}" : "IndicatorHidden");
        GoTo(!hasCountdown ? "CountdownHidden" : countdownAtTop ? "CountdownTop" : "CountdownBottom");
    }

    private void UpdateIndicatorFill()
    {
        GoTo(ColorStyle switch
        {
            GrowlCardColorStyle.Solid => "IndicatorFillSolid",
            GrowlCardColorStyle.Rainbow => "IndicatorFillRainbow",
            var style => $"IndicatorFill{Severity}{style}"
        });
    }

    private void UpdateCountdownFill()
    {
        var mode = CountdownSeverity;

        var kind = mode switch
        {
            GrowlCardCountdownSeverity.FollowCardSeverity => Severity,
            GrowlCardCountdownSeverity.Attention => GrowlCardSeverity.Attention,
            GrowlCardCountdownSeverity.Success => GrowlCardSeverity.Success,
            GrowlCardCountdownSeverity.Warning => GrowlCardSeverity.Warning,
            GrowlCardCountdownSeverity.Error => GrowlCardSeverity.Error,
            GrowlCardCountdownSeverity.Info => GrowlCardSeverity.Info,
            _ => GrowlCardSeverity.Warning
        };

        var style = CountdownColorStyle switch
        {
            GrowlCardCountdownColorStyle.Solid => GrowlCardColorStyle.Solid,
            GrowlCardCountdownColorStyle.Gradient => GrowlCardColorStyle.Gradient,
            GrowlCardCountdownColorStyle.Vivid => GrowlCardColorStyle.Vivid,
            GrowlCardCountdownColorStyle.Rainbow => GrowlCardColorStyle.Rainbow,
            _ => ColorStyle
        };

        bool mirror = mode == GrowlCardCountdownSeverity.FollowCardSeverity
            && CountdownColorStyle == GrowlCardCountdownColorStyle.Default;

        if (style == GrowlCardColorStyle.Rainbow && !mirror && mode != GrowlCardCountdownSeverity.Default)
        {
            style = GrowlCardColorStyle.Vivid;
        }

        GoTo(style switch
        {
            GrowlCardColorStyle.Solid => "CountdownFillSolid",
            GrowlCardColorStyle.Rainbow => "CountdownFillRainbow",
            _ => $"CountdownFill{kind}{style}"
        });
    }

    private void UpdateIconStates()
    {
        var type = Severity;

        GoTo(Icon is null ? "DefaultIconVisible" : "DefaultIconHidden");
        GoTo(ColorStyle switch
        {
            GrowlCardColorStyle.Solid => $"IconColor{type}",
            GrowlCardColorStyle.Rainbow => "IconColorRainbow",
            var style => $"IconColor{type}{style}"
        });
        GoTo(IconBackground is null ? "IconBackgroundDefault" : "IconBackgroundCustom");
        GoTo(IconForeground is null ? $"IconForeground{type}" : "IconForegroundCustom");
        GoTo(IconBorderBrush is null ? "IconBorderDefault" : "IconBorderCustom");
        GoTo($"IconStyle{IconStyle}");
        GoTo(ShowIconContainer ? "IconContainerVisible" : "IconContainerHidden");

        if (_iconMark is not null)
        {
            _iconMark.Data = CreateMarkGeometry(type);
        }

        if (_iconMarkOutline is not null)
        {
            _iconMarkOutline.Data = CreateMarkGeometry(type);
        }
    }

    private static readonly Dictionary<GrowlCardSeverity, string> MarkPaths = new()
    {
        [GrowlCardSeverity.Attention] = "M12 6.5C9.8 6.5 8.5 8.2 8.5 10.2V12.5L7.5 14.5H16.5L15.5 12.5V10.2C15.5 8.2 14.2 6.5 12 6.5ZM10.5 16.5C10.8 17.3 11.3 17.6 12 17.6C12.7 17.6 13.2 17.3 13.5 16.5",
        [GrowlCardSeverity.Success] = "M7.8 12.4L10.6 15.2L16.3 9.2",
        [GrowlCardSeverity.Warning] = "M12 7V13.2M12 16.6H12.01",
        [GrowlCardSeverity.Error] = "M8.6 8.6L15.4 15.4M15.4 8.6L8.6 15.4",
        [GrowlCardSeverity.Info] = "M12 11V16.8M12 7.4H12.01"
    };

    private static Geometry CreateMarkGeometry(GrowlCardSeverity severity)
    {
        return (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), MarkPaths[severity]);
    }

    private void UpdateProgressText()
    {
        double percent = ProgressMaximum > 0 ? Math.Clamp(Progress / ProgressMaximum, 0, 1) * 100 : 0;
        string text = $"{percent:0}%";

        if (_progressText is not null)
        {
            _progressText.Text = text;
        }

        if (_ringText is not null)
        {
            _ringText.Text = text;
        }
    }

}
