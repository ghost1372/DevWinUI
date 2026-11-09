using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Markup;
using System.Collections.Specialized;

namespace DevWinUI;

[ContentProperty(Name = nameof(Steps))]
[TemplatePart(Name = PartCard, Type = typeof(Border))]
[TemplatePart(Name = PartCounterText, Type = typeof(TextBlock))]
[TemplatePart(Name = PartProgressGrid, Type = typeof(Grid))]
[TemplatePart(Name = PartHost, Type = typeof(MorphHost))]
[TemplatePart(Name = PartBackButton, Type = typeof(Button))]
[TemplatePart(Name = PartNextButton, Type = typeof(Button))]
[TemplatePart(Name = PartContinueLabel, Type = typeof(TextBlock))]
[TemplatePart(Name = PartFinishLabel, Type = typeof(TextBlock))]
public sealed partial class MorphingWizard : Control
{
    private const string PartCard = "PART_Card";
    private const string PartCounterText = "PART_CounterText";
    private const string PartProgressGrid = "PART_ProgressGrid";
    private const string PartHost = "PART_Host";
    private const string PartBackButton = "PART_BackButton";
    private const string PartNextButton = "PART_NextButton";
    private const string PartContinueLabel = "PART_ContinueLabel";
    private const string PartFinishLabel = "PART_FinishLabel";

    private const string TrackStyleKey = "TrackStyle";
    private const string FillStyleKey = "FillStyle";
    private const string SegmentLabelStyleKey = "SegmentLabelStyle";
    private const string SegmentLabelActiveStyleKey = "SegmentLabelActiveStyle";

    private const float SlideDistance = 36f;
    private readonly ObservableCollection<MorphingWizardStep> _steps = new();
    private readonly List<Border> _tracks = new();
    private readonly List<Border> _fills = new();
    private readonly List<float> _fillValues = new();
    private readonly List<TextBlock> _labels = new();
    private readonly List<ContentPresenter> _leaving = new();

    private Border _card = null!;
    private TextBlock _counterText = null!;
    private Grid _progressGrid = null!;
    private MorphHost _host = null!;
    private Button _backButton = null!;
    private Button _nextButton = null!;
    private TextBlock _continueLabel = null!;
    private TextBlock _finishLabel = null!;

    private Style? _trackStyle;
    private Style? _fillStyle;
    private Style? _labelStyle;
    private Style? _activeLabelStyle;
    private CubicBezierEasingFunction? _ease;

    private ContentPresenter? _current;
    private int _index = -1;
    private bool _built;
    private bool _loaded;
    private bool _templated;

    public MorphingWizard()
    {
        DefaultStyleKey = typeof(MorphingWizard);
        _steps.CollectionChanged += OnStepsChanged;
        Loaded += OnLoaded;
    }

    public void GoTo(int index)
    {
        if (!_built || _steps.Count == 0)
        {
            return;
        }

        index = Math.Clamp(index, 0, _steps.Count - 1);
        if (index != _index)
        {
            ShowStep(index, index > _index ? 1 : -1, animate: true);
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_templated)
        {
            _backButton.Click -= OnBack;
            _nextButton.Click -= OnNext;
            FlushLeaving();
            if (_current is not null)
            {
                Detach(_current);
            }
        }

        _current = null;
        _index = -1;
        _built = false;
        _templated = false;

        var card = GetTemplateChild(PartCard) as Border;
        var counter = GetTemplateChild(PartCounterText) as TextBlock;
        var progress = GetTemplateChild(PartProgressGrid) as Grid;
        var host = GetTemplateChild(PartHost) as MorphHost;
        var back = GetTemplateChild(PartBackButton) as Button;
        var next = GetTemplateChild(PartNextButton) as Button;
        var continueLabel = GetTemplateChild(PartContinueLabel) as TextBlock;
        var finishLabel = GetTemplateChild(PartFinishLabel) as TextBlock;

        if (card is null || counter is null || progress is null || host is null ||
            back is null || next is null || continueLabel is null || finishLabel is null)
        {
            return;
        }

        _card = card;
        _counterText = counter;
        _progressGrid = progress;
        _host = host;
        _backButton = back;
        _nextButton = next;
        _continueLabel = continueLabel;
        _finishLabel = finishLabel;

        _trackStyle = FindStyle(card, TrackStyleKey);
        _fillStyle = FindStyle(card, FillStyleKey);
        _labelStyle = FindStyle(card, SegmentLabelStyleKey);
        _activeLabelStyle = FindStyle(card, SegmentLabelActiveStyleKey);

        _templated = true;

        _backButton.Click += OnBack;
        _nextButton.Click += OnNext;

        if (_loaded)
        {
            Setup();
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _loaded = true;
        if (_templated)
        {
            Setup();
        }
    }

    private void Setup()
    {
        if (_built)
        {
            return;
        }

        _built = true;
        RebuildProgress();
        ShowStep(DefaultIndex, 1, animate: false);
    }

    private void OnStepsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (!_built)
        {
            return;
        }

        FlushLeaving();
        if (_current is not null)
        {
            Detach(_current);
            _current = null;
            _host.Current = null;
        }

        int keep = Math.Clamp(_index, 0, Math.Max(0, _steps.Count - 1));
        RebuildProgress();
        _index = -1;
        ShowStep(keep, 1, animate: false);
    }

    private void OnNext(object sender, RoutedEventArgs e)
    {
        if (_index < _steps.Count - 1)
        {
            ShowStep(_index + 1, 1, animate: true);
            return;
        }

        Completed?.Invoke(this, EventArgs.Empty);

        if (ResetOnCompleted && IsLoaded && _steps.Count > 0)
        {
            ShowStep(0, -1, animate: true);
        }
    }

    private void OnBack(object sender, RoutedEventArgs e)
    {
        if (_index > 0)
        {
            ShowStep(_index - 1, -1, animate: true);
        }
    }

    private void ShowStep(int index, int direction, bool animate)
    {
        if (_steps.Count == 0)
        {
            _counterText.Text = string.Empty;
            return;
        }

        index = Math.Clamp(index, 0, _steps.Count - 1);
        FlushLeaving();

        _host.AnimateHeight = animate;

        var content = _steps[index].Content;
        var outgoing = _current;

        if (outgoing is not null && ReferenceEquals(outgoing.Content, content))
        {
            _index = index;
            UpdateChrome();
            UpdateProgress(animate);
            return;
        }

        var incoming = new ContentPresenter
        {
            Content = content,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
        };
        ElementCompositionPreview.SetIsTranslationEnabled(incoming, true);
        var incomingVisual = ElementCompositionPreview.GetElementVisual(incoming);
        incomingVisual.Properties.InsertVector3("Translation", Vector3.Zero);

        _host.Children.Add(incoming);
        _host.Current = incoming;
        if (!animate || outgoing is null)
        {
            _host.SnapNext();
        }

        _current = incoming;
        _index = index;

        bool slide = animate && outgoing is not null;

        if (outgoing is not null)
        {
            Leave(outgoing, direction, slide);
        }

        if (slide)
        {
            Enter(incomingVisual, direction);
        }

        UpdateChrome();
        UpdateProgress(animate);
        StepChanged?.Invoke(this, index);
    }

    private void Enter(Visual visual, int direction)
    {
        var compositor = visual.Compositor;
        var ease = GetEase(compositor);

        var move = compositor.CreateVector3KeyFrameAnimation();
        move.InsertKeyFrame(0f, new Vector3(direction * SlideDistance, 0f, 0f));
        move.InsertKeyFrame(1f, Vector3.Zero, ease);
        move.Duration = TimeSpan.FromMilliseconds(420);

        var fade = compositor.CreateScalarKeyFrameAnimation();
        fade.InsertKeyFrame(0f, 0f);
        fade.InsertKeyFrame(1f, 1f, ease);
        fade.Duration = TimeSpan.FromMilliseconds(320);
        fade.DelayTime = TimeSpan.FromMilliseconds(60);
        fade.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;

        visual.StartAnimation("Translation", move);
        visual.StartAnimation("Opacity", fade);
    }

    private void Leave(ContentPresenter panel, int direction, bool slide)
    {
        panel.IsHitTestVisible = false;

        if (!slide)
        {
            Detach(panel);
            return;
        }

        _leaving.Add(panel);

        ElementCompositionPreview.SetIsTranslationEnabled(panel, true);
        var visual = ElementCompositionPreview.GetElementVisual(panel);
        var compositor = visual.Compositor;
        var ease = GetEase(compositor);
        var to = new Vector3(-direction * SlideDistance, 0f, 0f);

        var move = compositor.CreateVector3KeyFrameAnimation();
        move.InsertKeyFrame(0f, Vector3.Zero);
        move.InsertKeyFrame(1f, to, ease);
        move.Duration = TimeSpan.FromMilliseconds(260);

        var fade = compositor.CreateScalarKeyFrameAnimation();
        fade.InsertKeyFrame(0f, 1f);
        fade.InsertKeyFrame(1f, 0f, ease);
        fade.Duration = TimeSpan.FromMilliseconds(180);

        var batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
        visual.StartAnimation("Translation", move);
        visual.StartAnimation("Opacity", fade);
        batch.End();

        visual.Properties.InsertVector3("Translation", to);
        visual.Opacity = 0f;

        batch.Completed += (_, _) => DispatcherQueue.TryEnqueue(() =>
        {
            if (_leaving.Remove(panel))
            {
                Detach(panel);
            }
        });
    }

    private void FlushLeaving()
    {
        for (int i = _leaving.Count - 1; i >= 0; i--)
        {
            var panel = _leaving[i];
            _leaving.RemoveAt(i);
            Detach(panel);
        }
    }

    private void Detach(ContentPresenter panel)
    {
        var visual = ElementCompositionPreview.GetElementVisual(panel);
        visual.StopAnimation("Translation");
        visual.StopAnimation("Opacity");

        panel.Content = null;
        _host.Children.Remove(panel);
    }

    private void RebuildProgress()
    {
        _progressGrid.Children.Clear();
        _progressGrid.ColumnDefinitions.Clear();
        _tracks.Clear();
        _fills.Clear();
        _fillValues.Clear();
        _labels.Clear();

        for (int i = 0; i < _steps.Count; i++)
        {
            _progressGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var track = new Border { Style = _trackStyle };
            var fill = new Border { Style = _fillStyle };
            var label = new TextBlock { Text = _steps[i].Label, Style = _labelStyle };

            SetOrClear(track, Border.BackgroundProperty, TrackBrush);
            SetOrClear(fill, Border.BackgroundProperty, ProgressBrush);
            SetOrClear(label, TextBlock.ForegroundProperty, SegmentForeground);

            var segment = new Grid { RowSpacing = 8 };
            segment.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            segment.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            segment.Children.Add(track);
            segment.Children.Add(fill);
            segment.Children.Add(label);
            Grid.SetRow(label, 1);
            Grid.SetColumn(segment, i);
            _progressGrid.Children.Add(segment);

            ElementCompositionPreview.GetElementVisual(fill).Scale = new Vector3(0f, 1f, 1f);

            _tracks.Add(track);
            _fills.Add(fill);
            _fillValues.Add(0f);
            _labels.Add(label);
        }
    }

    private void ApplyBrushes()
    {
        for (int i = 0; i < _fills.Count; i++)
        {
            SetOrClear(_tracks[i], Border.BackgroundProperty, TrackBrush);
            SetOrClear(_fills[i], Border.BackgroundProperty, ProgressBrush);
            ApplyLabel(i, i <= _index);
        }
    }

    private void ApplyLabel(int index, bool reached)
    {
        var label = _labels[index];
        label.Style = reached ? _activeLabelStyle : _labelStyle;
        SetOrClear(label, TextBlock.ForegroundProperty, reached ? ActiveSegmentForeground : SegmentForeground);
    }

    private static void SetOrClear(DependencyObject target, DependencyProperty property, Brush? brush)
    {
        if (brush is null)
        {
            target.ClearValue(property);
        }
        else
        {
            target.SetValue(property, brush);
        }
    }

    private void UpdateProgress(bool animate)
    {
        bool motion = animate;

        for (int i = 0; i < _fills.Count; i++)
        {
            bool reached = i <= _index;
            float target = reached ? 1f : 0f;
            float previous = _fillValues[i];

            if (Math.Abs(previous - target) <= 0.001f)
            {
                continue;
            }

            ApplyLabel(i, reached);

            var visual = ElementCompositionPreview.GetElementVisual(_fills[i]);
            if (motion)
            {
                var compositor = visual.Compositor;
                var grow = compositor.CreateScalarKeyFrameAnimation();
                grow.InsertKeyFrame(0f, previous);
                grow.InsertKeyFrame(1f, target, GetEase(compositor));
                grow.Duration = TimeSpan.FromMilliseconds(480);
                visual.StartAnimation("Scale.X", grow);
            }

            visual.Scale = new Vector3(target, 1f, 1f);
            _fillValues[i] = target;
        }
    }

    private void UpdateChrome()
    {
        if (!_templated || _index < 0 || _steps.Count == 0)
        {
            return;
        }

        bool first = _index == 0;
        bool last = _index == _steps.Count - 1;

        _counterText.Text = FormatCounter();

        _backButton.IsEnabled = !first;
        _backButton.Opacity = first ? 0 : 1;

        _continueLabel.Opacity = last ? 0 : 1;
        _finishLabel.Opacity = last ? 1 : 0;
        AutomationProperties.SetName(_nextButton, last ? FinishText : ContinueText);
    }

    private string FormatCounter()
    {
        try
        {
            return string.Format(CounterFormat, _index + 1, _steps.Count);
        }
        catch (Exception ex) when (ex is FormatException or ArgumentNullException)
        {
            return $"{_index + 1}/{_steps.Count}";
        }
    }

    private static Style? FindStyle(Border card, string key)
    {
        try
        {
            return card.Resources[key] as Style;
        }
        catch (Exception ex) when (ex is KeyNotFoundException or ArgumentException)
        {
            return null;
        }
    }

    private CubicBezierEasingFunction GetEase(Compositor compositor) =>
        _ease ??= compositor.CreateCubicBezierEasingFunction(new Vector2(0.22f, 1f), new Vector2(0.36f, 1f));
}
