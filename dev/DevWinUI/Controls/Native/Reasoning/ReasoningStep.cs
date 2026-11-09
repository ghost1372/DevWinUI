using Microsoft.UI.Xaml.Automation;

namespace DevWinUI;

public sealed partial class ReasoningStep : ItemsControl
{
    public string Label
    {
        get { return (string)GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(ReasoningStep), new PropertyMetadata("", (d, _) => ((ReasoningStep)d).OnLabelChanged()));

    public ReasoningStepStatus Status
    {
        get { return (ReasoningStepStatus)GetValue(StatusProperty); }
        set { SetValue(StatusProperty, value); }
    }

    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(nameof(Status), typeof(ReasoningStepStatus), typeof(ReasoningStep), new PropertyMetadata(ReasoningStepStatus.Pending, (d, e) => ((ReasoningStep)d).OnStatusChanged((ReasoningStepStatus)e.OldValue, (ReasoningStepStatus)e.NewValue)));

    CollapsibleGrid _reveal = null!;
    CollapsibleGrid _detailsPanel = null!;
    ReasoningTrigger _header = null!;
    FrameworkElement _glyph = null!;
    FrameworkElement _chevronBox = null!;
    bool _templated;

    public ReasoningStep()
    {
        DefaultStyleKey = typeof(ReasoningStep);
        Items.VectorChanged += (_, _) => RefreshInteractive();
        Loaded += (_, _) => RefreshInteractive();
    }

    internal event Action? Changed;

    public ItemCollection Details => Items;

    protected override void OnApplyTemplate()
    {
        if (_templated) _header.Click -= OnHeaderClick;

        base.OnApplyTemplate();

        _reveal = (CollapsibleGrid)GetTemplateChild("PART_Reveal");
        _detailsPanel = (CollapsibleGrid)GetTemplateChild("PART_DetailsPanel");
        _header = (ReasoningTrigger)GetTemplateChild("PART_Header");
        _glyph = (FrameworkElement)GetTemplateChild("PART_Glyph");
        _chevronBox = (FrameworkElement)GetTemplateChild("PART_ChevronBox");

        _header.Click += OnHeaderClick;
        _header.RequestOpen = open => ToggleDetails(open);
        _templated = true;

        AutomationProperties.SetName(_header, Label);
        VisualStateManager.GoToState(this, Status.ToString(), false);
        _reveal.Set(Status != ReasoningStepStatus.Pending, false);
        RefreshInteractive();
    }

    void OnHeaderClick(object sender, RoutedEventArgs e) => ToggleDetails(null);

    void OnLabelChanged()
    {
        if (_templated) AutomationProperties.SetName(_header, Label);
        Changed?.Invoke();
    }

    void OnStatusChanged(ReasoningStepStatus old, ReasoningStepStatus now)
    {
        if (_templated)
        {
            VisualStateManager.GoToState(this, now.ToString(), false);
            RefreshInteractive();

            bool visible = now != ReasoningStepStatus.Pending;
            if (visible != _reveal.IsOpen)
            {
                _reveal.Set(visible, true);
                if (visible && IsLoaded) ReasoningFx.FadeUp(_header, 0, 220, 0);
            }

            if (now == ReasoningStepStatus.Done && old != ReasoningStepStatus.Done && IsLoaded && ReasoningFx.Motion)
                ReasoningFx.Pop(_glyph);
        }

        Changed?.Invoke();
    }

    void RefreshInteractive()
    {
        if (!_templated) return;

        bool hasDetails = Items.Count > 0;
        _header.IsLeaf = !hasDetails;
        _header.IsTabStop = hasDetails;
        _header.IsHitTestVisible = hasDetails;
        _chevronBox.Visibility = hasDetails ? Visibility.Visible : Visibility.Collapsed;
    }

    void ToggleDetails(bool? force)
    {
        if (Items.Count == 0) return;

        bool next = force ?? !_detailsPanel.IsOpen;
        if (next == _detailsPanel.IsOpen) return;

        bool old = _header.IsExpanded;
        _header.IsExpanded = next;
        _header.RaiseState(old, next);
        _detailsPanel.Set(next, true);
        ReasoningFx.RotateChevron(_chevronBox, next, IsLoaded && ReasoningFx.Motion);
    }
}
