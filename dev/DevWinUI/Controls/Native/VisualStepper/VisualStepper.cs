using System.ComponentModel;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Windows.System;

namespace DevWinUI;

[TemplatePart(Name = RootPartName, Type = typeof(Grid))]
[TemplatePart(Name = StepsPanelPartName, Type = typeof(StackPanel))]
[TemplatePart(Name = CardHostPartName, Type = typeof(Grid))]
[TemplatePart(Name = CardAPartName, Type = typeof(ContentPresenter))]
[TemplatePart(Name = CardBPartName, Type = typeof(ContentPresenter))]
public sealed partial class VisualStepper : Control
{
    private const string RootPartName = "PART_Root";
    private const string StepsPanelPartName = "PART_StepsPanel";
    private const string CardHostPartName = "PART_CardHost";
    private const string CardAPartName = "PART_CardA";
    private const string CardBPartName = "PART_CardB";

    public int SelectedIndex
    {
        get { return (int)GetValue(SelectedIndexProperty); }
        set { SetValue(SelectedIndexProperty, value); }
    }

    public static readonly DependencyProperty SelectedIndexProperty =
        DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(VisualStepper), new PropertyMetadata(0, SelectedIndexChanged));

    public bool Autoplay
    {
        get { return (bool)GetValue(AutoplayProperty); }
        set { SetValue(AutoplayProperty, value); }
    }

    public static readonly DependencyProperty AutoplayProperty =
        DependencyProperty.Register(nameof(Autoplay), typeof(bool), typeof(VisualStepper), new PropertyMetadata(true, RefreshChanged));

    public double Interval
    {
        get { return (double)GetValue(IntervalProperty); }
        set { SetValue(IntervalProperty, value); }
    }

    public static readonly DependencyProperty IntervalProperty =
        DependencyProperty.Register(nameof(Interval), typeof(double), typeof(VisualStepper), new PropertyMetadata(2500.0, RefreshChanged));

    public bool PauseOnHover
    {
        get { return (bool)GetValue(PauseOnHoverProperty); }
        set { SetValue(PauseOnHoverProperty, value); }
    }

    public static readonly DependencyProperty PauseOnHoverProperty =
        DependencyProperty.Register(nameof(PauseOnHover), typeof(bool), typeof(VisualStepper), new PropertyMetadata(true, PauseOnHoverChanged));

    public bool ShowCard
    {
        get { return (bool)GetValue(ShowCardProperty); }
        set { SetValue(ShowCardProperty, value); }
    }

    public static readonly DependencyProperty ShowCardProperty =
        DependencyProperty.Register(nameof(ShowCard), typeof(bool), typeof(VisualStepper), new PropertyMetadata(true, ShowCardChanged));

    private static readonly VisualStepperTemplate DefaultCardTemplateResources = new();

    private static readonly DataTemplate DefaultCardTemplate =
        (DataTemplate)DefaultCardTemplateResources["DefaultVisualStepperCardTemplate"];

    public DataTemplate? CardTemplate
    {
        get { return (DataTemplate?)GetValue(CardTemplateProperty); }
        set { SetValue(CardTemplateProperty, value); }
    }

    public static readonly DependencyProperty CardTemplateProperty =
        DependencyProperty.Register(nameof(CardTemplate), typeof(DataTemplate), typeof(VisualStepper), new PropertyMetadata(DefaultCardTemplate, CardTemplateChanged));

    public double InactiveStepOpacity
    {
        get { return (double)GetValue(InactiveStepOpacityProperty); }
        set { SetValue(InactiveStepOpacityProperty, value); }
    }

    public static readonly DependencyProperty InactiveStepOpacityProperty =
        DependencyProperty.Register(nameof(InactiveStepOpacity), typeof(double), typeof(VisualStepper), new PropertyMetadata(0.55, RefreshChanged));

    public Brush? TrackBrush
    {
        get { return (Brush?)GetValue(TrackBrushProperty); }
        set { SetValue(TrackBrushProperty, value); }
    }

    public static readonly DependencyProperty TrackBrushProperty =
        DependencyProperty.Register(nameof(TrackBrush), typeof(Brush), typeof(VisualStepper), new PropertyMetadata(null, BrushChanged));

    public Brush? ProgressBrush
    {
        get { return (Brush?)GetValue(ProgressBrushProperty); }
        set { SetValue(ProgressBrushProperty, value); }
    }

    public static readonly DependencyProperty ProgressBrushProperty =
        DependencyProperty.Register(nameof(ProgressBrush), typeof(Brush), typeof(VisualStepper), new PropertyMetadata(null, BrushChanged));

    public Brush? DescriptionForeground
    {
        get { return (Brush?)GetValue(DescriptionForegroundProperty); }
        set { SetValue(DescriptionForegroundProperty, value); }
    }

    public static readonly DependencyProperty DescriptionForegroundProperty =
        DependencyProperty.Register(nameof(DescriptionForeground), typeof(Brush), typeof(VisualStepper), new PropertyMetadata(null, BrushChanged));

    public ObservableCollection<VisualStepperStep> Steps { get; } = new();

    public event TypedEventHandler<VisualStepper, int>? StepChanged;

    private static void SelectedIndexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is VisualStepper control)
        {
            control.OnSelectedIndexChanged((int)args.NewValue);
        }
    }

    private static void RefreshChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is VisualStepper control)
        {
            control.Apply();
        }
    }

    private static void BrushChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is VisualStepper control)
        {
            control.ApplyBrushes();
        }
    }

    private static void PauseOnHoverChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is VisualStepper control)
        {
            control.UpdatePause();
        }
    }

    private static void ShowCardChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is VisualStepper control)
        {
            control.ApplyCardVisibility();
        }
    }

    private static void CardTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is VisualStepper control)
        {
            control.SwapCard(control.SelectedIndex, animate: false);
        }
    }

    private readonly List<VisualStepperRow> _rows = new();
    private readonly List<VisualStepperStep> _tracked = new();
    private Storyboard? _sb;
    private bool _hover, _focused, _loaded;

    private Grid? _root;
    private StackPanel? _stepsPanel;
    private Grid? _cardHost;
    private ContentPresenter? _cardA, _cardB;
    private ContentPresenter? _front, _back;

    public VisualStepper()
    {
        DefaultStyleKey = typeof(VisualStepper);

        Steps.CollectionChanged += (_, _) => Rebuild();

        Loaded += (_, _) =>
        {
            _loaded = true;
            Track();
            Apply();
        };
        Unloaded += (_, _) =>
        {
            _loaded = false;
            _hover = false;
            _focused = false;
            StopAnimation();
            Untrack();
        };
        GotFocus += (_, _) =>
        {
            _focused = FocusState == FocusState.Keyboard;
            UpdatePause();
        };
        LostFocus += (_, _) =>
        {
            _focused = false;
            UpdatePause();
        };
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_root is not null)
        {
            _root.PointerEntered -= Root_PointerEntered;
            _root.PointerExited -= Root_PointerExited;
        }

        StopAnimation();

        _root = GetTemplateChild(RootPartName) as Grid;
        _stepsPanel = GetTemplateChild(StepsPanelPartName) as StackPanel;
        _cardHost = GetTemplateChild(CardHostPartName) as Grid;
        _cardA = GetTemplateChild(CardAPartName) as ContentPresenter;
        _cardB = GetTemplateChild(CardBPartName) as ContentPresenter;
        _front = _cardA;
        _back = _cardB;

        if (_root is not null)
        {
            _root.PointerEntered += Root_PointerEntered;
            _root.PointerExited += Root_PointerExited;
        }

        Rebuild();
    }

    private void Track()
    {
        Untrack();
        if (!_loaded) return;

        foreach (var step in Steps)
        {
            step.PropertyChanged += OnStepPropertyChanged;
            _tracked.Add(step);
        }
    }

    private void Untrack()
    {
        foreach (var step in _tracked) step.PropertyChanged -= OnStepPropertyChanged;
        _tracked.Clear();
    }

    private void OnStepPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not VisualStepperStep step) return;

        var row = _rows.Find(r => ReferenceEquals(r.Step, step));
        if (row != null) ConfigureRow(row);
    }

    private VisualStepperRow CreateRow(VisualStepperStep step, int index)
    {
        var root = new Grid
        {
            Background = new SolidColorBrush(Colors.Transparent),
            ColumnSpacing = 16,
            Padding = new Thickness(0, 10, 0, 10),
            OpacityTransition = new ScalarTransition { Duration = TimeSpan.FromMilliseconds(250) }
        };
        root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3) });
        root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var track = new Rectangle { RadiusX = 1.5, RadiusY = 1.5 };
        var fill = new ScaleTransform { ScaleY = 0 };
        var progress = new Rectangle
        {
            RadiusX = 1.5,
            RadiusY = 1.5,
            RenderTransformOrigin = new Point(0, 0),
            RenderTransform = fill
        };

        var title = new TextBlock
        {
            FontSize = 14,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            TextWrapping = TextWrapping.Wrap
        };
        var description = new TextBlock
        {
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap
        };
        var text = new StackPanel { Spacing = 2 };
        text.Children.Add(title);
        text.Children.Add(description);
        Grid.SetColumn(text, 1);

        root.Children.Add(track);
        root.Children.Add(progress);
        root.Children.Add(text);

        root.Tapped += (_, _) =>
        {
            Focus(FocusState.Pointer);
            SelectedIndex = index;
        };

        AutomationProperties.SetPositionInSet(root, index + 1);
        AutomationProperties.SetSizeOfSet(root, Steps.Count);

        var row = new VisualStepperRow
        {
            Step = step,
            Root = root,
            Track = track,
            Progress = progress,
            Fill = fill,
            Title = title,
            Description = description
        };
        ConfigureRow(row);
        return row;
    }

    private void ConfigureRow(VisualStepperRow row)
    {
        var step = row.Step;
        row.Title.Text = step.Title;
        row.Description.Text = step.Description ?? "";
        row.Description.Visibility = string.IsNullOrEmpty(step.Description) ? Visibility.Collapsed : Visibility.Visible;

        AutomationProperties.SetName(row.Root, step.Title);
        AutomationProperties.SetHelpText(row.Root, step.Description ?? "");

        ApplyBrushes(row);
    }

    private void ApplyBrushes()
    {
        foreach (var row in _rows) ApplyBrushes(row);
    }

    private void ApplyBrushes(VisualStepperRow row)
    {
        row.Track.Fill = TrackBrush;
        row.Progress.Fill = ProgressBrush;

        if (DescriptionForeground is { } brush)
        {
            row.Description.Foreground = brush;
        }
        else
        {
            row.Description.ClearValue(TextBlock.ForegroundProperty);
        }
    }

    private void Rebuild()
    {
        if (_stepsPanel is null) return;

        StopAnimation();
        _stepsPanel.Children.Clear();
        _rows.Clear();

        for (int i = 0; i < Steps.Count; i++)
        {
            var row = CreateRow(Steps[i], i);
            _rows.Add(row);
            _stepsPanel.Children.Add(row.Root);
        }

        Track();
        ApplyCardVisibility();

        if (Steps.Count > 0 && (SelectedIndex < 0 || SelectedIndex >= Steps.Count))
        {
            SelectedIndex = Math.Clamp(SelectedIndex, 0, Steps.Count - 1);
            return;
        }

        SwapCard(SelectedIndex, animate: false);
        Apply();
    }

    private void ApplyCardVisibility()
    {
        if (_root is null || _cardHost is null || _root.ColumnDefinitions.Count < 2) return;

        _root.ColumnDefinitions[1].Width = ShowCard ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
        _cardHost.Visibility = ShowCard ? Visibility.Visible : Visibility.Collapsed;
        _root.ColumnSpacing = ShowCard ? 40 : 0;
    }

    private void OnSelectedIndexChanged(int index)
    {
        if (Steps.Count == 0) return;

        int clamped = Math.Clamp(index, 0, Steps.Count - 1);
        if (clamped != index)
        {
            SelectedIndex = clamped;
            return;
        }

        SwapCard(clamped, animate: true);
        Apply();
        StepChanged?.Invoke(this, clamped);
    }

    private void SwapCard(int index, bool animate)
    {
        if (_cardA is null || _cardB is null || !ShowCard || index < 0 || index >= Steps.Count) return;

        var item = new VisualStepperItem { Index = index, Step = Steps[index] };

        if (!animate)
        {
            _front = _cardA;
            _back = _cardB;
            _cardA.ContentTemplate = CardTemplate;
            _cardA.Content = item;
            _cardA.IsHitTestVisible = true;
            _cardA.Opacity = 1;
            _cardB.Content = null;
            _cardB.IsHitTestVisible = false;
            _cardB.Opacity = 0;
            return;
        }

        if (_front is null || _back is null) return;

        var incoming = _back;
        var outgoing = _front;

        incoming.ContentTemplate = CardTemplate;
        incoming.Content = item;
        incoming.IsHitTestVisible = true;
        outgoing.IsHitTestVisible = false;

        incoming.Opacity = 1;
        outgoing.Opacity = 0;

        _front = incoming;
        _back = outgoing;
    }

    private void StopAnimation()
    {
        _sb?.Stop();
        _sb = null;
    }

    private void Apply()
    {
        StopAnimation();

        int active = SelectedIndex;
        for (int i = 0; i < _rows.Count; i++)
        {
            _rows[i].Root.Opacity = i == active ? 1.0 : InactiveStepOpacity;
            _rows[i].Fill.ScaleY = i < active ? 1 : 0;
            AutomationProperties.SetItemStatus(_rows[i].Root, i == active ? "current" : string.Empty);
        }

        if (!_loaded || active < 0 || active >= _rows.Count) return;

        var fill = _rows[active].Fill;

        if (!Autoplay || _rows.Count < 2)
        {
            fill.ScaleY = 1;
            return;
        }

        var anim = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = new Duration(TimeSpan.FromMilliseconds(Math.Max(100, Interval))),
        };
        Storyboard.SetTarget(anim, fill);
        Storyboard.SetTargetProperty(anim, "ScaleY");

        var sb = new Storyboard();
        sb.Children.Add(anim);
        sb.Completed += (_, _) =>
        {
            if (ReferenceEquals(_sb, sb)) Advance();
        };

        _sb = sb;
        sb.Begin();
        if (IsPaused) sb.Pause();
    }

    private bool IsPaused => PauseOnHover && (_hover || _focused);

    private void Advance()
    {
        if (Steps.Count < 2) return;
        SelectedIndex = (SelectedIndex + 1) % Steps.Count;
    }

    private void UpdatePause()
    {
        if (_sb is null) return;
        if (IsPaused) _sb.Pause(); else _sb.Resume();
    }

    private void Root_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        _hover = true;
        UpdatePause();
    }

    private void Root_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        _hover = false;
        UpdatePause();
    }

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        base.OnKeyDown(e);

        if (Steps.Count == 0) return;

        switch (e.Key)
        {
            case VirtualKey.Down:
            case VirtualKey.Right:
                SelectedIndex = Math.Min(SelectedIndex + 1, Steps.Count - 1);
                e.Handled = true;
                break;
            case VirtualKey.Up:
            case VirtualKey.Left:
                SelectedIndex = Math.Max(SelectedIndex - 1, 0);
                e.Handled = true;
                break;
            case VirtualKey.Home:
                SelectedIndex = 0;
                e.Handled = true;
                break;
            case VirtualKey.End:
                SelectedIndex = Steps.Count - 1;
                e.Handled = true;
                break;
        }
    }
}
