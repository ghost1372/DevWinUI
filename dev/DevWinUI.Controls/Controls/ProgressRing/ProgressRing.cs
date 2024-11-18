using Windows.Foundation;

namespace DevWinUI;

[TemplateVisualState(GroupName = GroupActive, Name = StateActive)]
[TemplateVisualState(GroupName = GroupActive, Name = StateInactive)]
public partial class ProgressRing : Control
{
    public const string GroupActive = "ActiveStates";
    public const string StateActive = "Active";
    public const string StateInactive = "Inactive";

    public static readonly DependencyProperty IsActiveProperty =
        DependencyProperty.Register("IsActive", typeof(bool), typeof(ProgressRing),
            new PropertyMetadata(false, IsActiveChanged));

    public static readonly DependencyProperty TemplateSettingsProperty =
        DependencyProperty.Register("TemplateSettings", typeof(TemplateSettingValues), typeof(ProgressRing),
            new PropertyMetadata(null));

    private bool _hasAppliedTemplate;

    public ProgressRing()
    {
        DefaultStyleKey = typeof(ProgressRing);
        TemplateSettings = new TemplateSettingValues(60);
    }

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public TemplateSettingValues TemplateSettings
    {
        get => (TemplateSettingValues)GetValue(TemplateSettingsProperty);
        set => SetValue(TemplateSettingsProperty, value);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _hasAppliedTemplate = true;
        UpdateState(IsActive);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var width = double.IsNaN(Width) == false ? Width : availableSize.Width;
        var height = double.IsNaN(Height) == false ? Height : availableSize.Height;

        TemplateSettings = new TemplateSettingValues(Math.Min(width, height));
        return base.MeasureOverride(availableSize);
    }

    private static void IsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        var pr = (ProgressRing)d;
        var isActive = (bool)args.NewValue;
        pr.UpdateState(isActive);
    }

    private void UpdateState(bool isActive)
    {
        if (_hasAppliedTemplate)
        {
            var state = isActive ? StateActive : StateInactive;
            VisualStateManager.GoToState(this, state, true);
        }
    }
}
