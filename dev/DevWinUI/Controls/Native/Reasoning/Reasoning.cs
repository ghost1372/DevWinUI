using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[ContentProperty(Name = nameof(Body))]
public sealed partial class Reasoning : Control
{
    static readonly Func<double, string> DefaultFormatter = DefaultFormat;

    public event TypedEventHandler<Reasoning, bool>? OpenChanged;

    ReasoningTrigger _trigger = null!;
    FrameworkElement _chevronBox = null!;
    TextBlock _label = null!;
    FrameworkElement _previewHost = null!;
    TextBlock _previewText = null!;
    CollapsibleGrid _panel = null!;
    ReasoningSteps? _steps;
    string? _lastPreview;
    bool _templated, _loaded, _init, _touched, _isOpen;

    public Reasoning()
    {
        DefaultStyleKey = typeof(Reasoning);
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    protected override void OnApplyTemplate()
    {
        if (_templated) _trigger.Click -= OnTriggerClick;

        base.OnApplyTemplate();

        _trigger = (ReasoningTrigger)GetTemplateChild("PART_Trigger");
        _chevronBox = (FrameworkElement)GetTemplateChild("PART_ChevronBox");
        _label = (TextBlock)GetTemplateChild("PART_Label");
        _previewHost = (FrameworkElement)GetTemplateChild("PART_PreviewHost");
        _previewText = (TextBlock)GetTemplateChild("PART_PreviewText");
        _panel = (CollapsibleGrid)GetTemplateChild("PART_Panel");

        _trigger.Click += OnTriggerClick;
        _trigger.RequestOpen = open => OnUserToggle(open);
        _templated = true;

        ApplyVariant();
        UpdateLabel(false);
        ApplyOpen(false);
    }

    static string DefaultFormat(double seconds)
    {
        int total = (int)Math.Round(seconds);
        return total < 60 ? $"{total}s" : $"{total / 60}m {total % 60}s";
    }

    void OnTriggerClick(object sender, RoutedEventArgs e) => OnUserToggle(null);

    void OnLoaded(object sender, RoutedEventArgs e)
    {
        _loaded = true;

        if (!_init)
        {
            _init = true;
            if (Open is null && !_touched) _isOpen = Thinking || DefaultOpen;
        }

        if (!_templated) return;

        ApplyVariant();
        UpdateLabel(false);
        ApplyOpen(false);
    }

    void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _loaded = false;
        if (_templated) UpdateShimmer();
    }

    void OnThinkingChanged()
    {
        UpdateLabel(true);
        if (!_touched && Open is null) SetOpen(Thinking);
        UpdatePreview();
    }

    void OnUserToggle(bool? force)
    {
        bool next = force ?? !(Open ?? _isOpen);
        _touched = true;

        if (Open is null) SetOpen(next);
        else OpenChanged?.Invoke(this, next);
    }

    void SetOpen(bool value)
    {
        if (_isOpen == value) return;

        _isOpen = value;
        ApplyOpen(true);
        OpenChanged?.Invoke(this, value);
    }

    void ApplyOpen(bool animate)
    {
        if (!_templated) return;

        bool target = Open ?? _isOpen;
        animate &= _loaded && ReasoningFx.Motion;

        if (_trigger.IsExpanded != target)
        {
            bool old = _trigger.IsExpanded;
            _trigger.IsExpanded = target;
            _trigger.RaiseState(old, target);
        }

        ReasoningFx.RotateChevron(_chevronBox, target, animate);
        _panel.Set(target, animate);

        if (target && _loaded && Body is IReasoningReplay replay) replay.Replay();
        UpdatePreview();
    }

    void UpdateLabel(bool fade)
    {
        if (!_templated) return;

        string text = Thinking
            ? ThinkingLabel
            : double.IsNaN(Duration)
                ? CompletedLabel
                : string.Format(CompletedDurationLabel, (FormatDuration ?? DefaultFormatter)(Duration));

        bool changed = _label.Text != text;
        _label.Text = text;
        AutomationProperties.SetName(_trigger, text);

        UpdateShimmer();

        if (changed && fade && !Thinking && _loaded && ReasoningFx.Motion) ReasoningFx.FadeUp(_label, 0, 200, 4);
    }

    void UpdateShimmer() =>
        VisualStateManager.GoToState(this, Thinking && _loaded && ReasoningFx.Motion ? "Shimmer" : "Idle", false);

    void UpdatePreview()
    {
        if (!_templated) return;

        string? text = _steps?.ActiveLabel;
        bool show = !(Open ?? _isOpen) && Thinking && !string.IsNullOrEmpty(text);
        _previewHost.Visibility = show ? Visibility.Visible : Visibility.Collapsed;

        if (!show)
        {
            _lastPreview = null;
            return;
        }

        if (_lastPreview == text) return;

        _lastPreview = text;
        _previewText.Text = text!;
        ReasoningFx.FadeUp(_previewText, 0, 220, 8);
    }

    void OnStepsChanged() => UpdatePreview();

    void OnBodyChanged()
    {
        if (_steps is not null) _steps.Changed -= OnStepsChanged;

        _steps = Body as ReasoningSteps;
        if (_steps is not null) _steps.Changed += OnStepsChanged;

        UpdatePreview();
    }

    void ApplyVariant()
    {
        if (_templated) VisualStateManager.GoToState(this, Kind.ToString(), false);
    }
}
