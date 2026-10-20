namespace DevWinUI;

public sealed partial class IssueLabel : Control
{
    public object? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(object), typeof(IssueLabel), new PropertyMetadata(null));

    public DataTemplate? TitleTemplate
    {
        get => (DataTemplate?)GetValue(TitleTemplateProperty);
        set => SetValue(TitleTemplateProperty, value);
    }
    public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register(
        nameof(TitleTemplate), typeof(DataTemplate), typeof(IssueLabel), new PropertyMetadata(null));

    public IssueLabelSeverity Severity
    {
        get => (IssueLabelSeverity)GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }
    public static readonly DependencyProperty SeverityProperty = DependencyProperty.Register(
        nameof(Severity), typeof(IssueLabelSeverity), typeof(IssueLabel), new PropertyMetadata(IssueLabelSeverity.Orange, OnSeverityChanged));

    public IssueLabel()
    {
        DefaultStyleKey = typeof(IssueLabel);
        IsTabStop = false;
        RegisterPropertyChangedCallback(ForegroundProperty, OnForegroundChanged);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateSeverityVisualState();
        UpdateForegroundVisual();
    }

    private static void OnForegroundChanged(DependencyObject sender, DependencyProperty property)
    {
        ((IssueLabel)sender).UpdateForegroundVisual();
    }

    private static void OnSeverityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ((IssueLabel)sender).UpdateSeverityVisualState();
    }

    private void UpdateSeverityVisualState()
    {
        VisualStateManager.GoToState(this, $"Severity{Severity}", false);
    }

    private void UpdateForegroundVisual()
    {
        var hasCustomForeground = Foreground is not null;

        if (GetTemplateChild("SeverityTitle") is FrameworkElement severityTitle)
        {
            severityTitle.Visibility = hasCustomForeground ? Visibility.Collapsed : Visibility.Visible;
        }

        if (GetTemplateChild("LabelTitle") is FrameworkElement customTitle)
        {
            customTitle.Visibility = hasCustomForeground ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
