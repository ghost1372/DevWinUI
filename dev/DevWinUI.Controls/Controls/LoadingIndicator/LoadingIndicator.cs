
using System.ComponentModel;
using System.Reflection;

namespace DevWinUI;

[TemplatePart(Name = TemplateBorderName, Type = typeof(Border))]
public partial class LoadingIndicator : Control
{
    internal const string TemplateBorderName = "PART_Border";

    public static readonly DependencyProperty SpeedRatioProperty =
        DependencyProperty.Register("SpeedRatio", typeof(double), typeof(LoadingIndicator), new PropertyMetadata(1d,
            OnSpeedRatioChanged));

    public static readonly DependencyProperty IsActiveProperty =
        DependencyProperty.Register("IsActive", typeof(bool), typeof(LoadingIndicator), new PropertyMetadata(true,
            OnIsActiveChanged));

    public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
        "Mode", typeof(LoadingIndicatorMode), typeof(LoadingIndicator),
        new PropertyMetadata(default(LoadingIndicatorMode), OnModeChanged));

    private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (LoadingIndicator)d;
        if (ctl != null)
        {
            ctl.SetLoadingIndicatorMode((LoadingIndicatorMode)e.NewValue);
        }
    }

    private void SetLoadingIndicatorMode(LoadingIndicatorMode loadingIndicatorMode)
    {
        var styleName = GetLoadingIndicatorModeDescription(loadingIndicatorMode);
        Style = Application.Current.Resources[styleName] as Style;
    }

    protected Border PART_Border;

    public LoadingIndicatorMode Mode
    {
        get => (LoadingIndicatorMode)GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    public double SpeedRatio
    {
        get => (double)GetValue(SpeedRatioProperty);
        set => SetValue(SpeedRatioProperty, value);
    }

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    private static void OnSpeedRatioChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var li = (LoadingIndicator)o;

        if (li.PART_Border == null || li.IsActive == false)
        {
            return;
        }

        SetStoryBoardSpeedRatio(li.PART_Border, (double)e.NewValue);
    }

    private static void OnIsActiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var li = (LoadingIndicator)o;

        if (li.PART_Border == null)
        {
            return;
        }

        if ((bool)e.NewValue == false)
        {
            VisualStateManager.GoToState(li, IndicatorVisualStateNames.InactiveState.Name,
                false);
            li.PART_Border.SetValue(VisibilityProperty, Visibility.Collapsed);
        }
        else
        {
            VisualStateManager.GoToState(li, IndicatorVisualStateNames.ActiveState.Name, false);

            li.PART_Border.SetValue(VisibilityProperty, Visibility.Visible);

            SetStoryBoardSpeedRatio(li.PART_Border, li.SpeedRatio);
        }
    }

    private static void SetStoryBoardSpeedRatio(FrameworkElement element, double speedRatio)
    {
        var activeStates = element.GetActiveVisualStates();
        foreach (var activeState in activeStates)
        {
            activeState.Storyboard.SpeedRatio = speedRatio;
        }
    }

    public LoadingIndicator()
    {
        SetLoadingIndicatorMode(Mode);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        PART_Border = (Border)GetTemplateChild(TemplateBorderName);

        if (PART_Border == null)
        {
            return;
        }

        VisualStateManager.GoToState(this,
            IsActive
                ? IndicatorVisualStateNames.ActiveState.Name
                : IndicatorVisualStateNames.InactiveState.Name, false);

        SetStoryBoardSpeedRatio(PART_Border, SpeedRatio);

        PART_Border.SetValue(VisibilityProperty, IsActive ? Visibility.Visible : Visibility.Collapsed);
    }

    public string GetLoadingIndicatorModeDescription(LoadingIndicatorMode value)
    {
        return
            value
                .GetType()
                .GetMember(value.ToString())
                .FirstOrDefault()
                ?.GetCustomAttribute<DescriptionAttribute>()
                ?.Description;
    }
}
