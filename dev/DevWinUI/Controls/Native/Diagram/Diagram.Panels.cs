using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

public sealed partial class Diagram
{
    private Border? _legend, _conflictPanel, _hint;
    private TextBlock? _hintText;
    private DispatcherTimer? _animTimer;
    private double _dashPhase;

    public ObservableCollection<DiagramConflict> Conflicts { get; } = new();

    private void BindPanels()
    {
        _legend = GetTemplateChild("PART_Legend") as Border;
        _conflictPanel = GetTemplateChild("PART_Conflicts") as Border;
        _hint = GetTemplateChild("PART_Hint") as Border;
        _hintText = GetTemplateChild("PART_HintText") as TextBlock;

        Swallow(_legend);
        Swallow(_conflictPanel);
    }

    private static void Swallow(UIElement? e)
    {
        if (e == null) return;

        e.PointerPressed += (_, a) => a.Handled = true;
        e.PointerWheelChanged += (_, a) => a.Handled = true;
    }

    private void BuildLegend()
    {
        if (_legend == null) return;

        _legend.Visibility = ShowLegend ? Visibility.Visible : Visibility.Collapsed;
        if (!ShowLegend) return;

        var stack = new StackPanel { Spacing = 4, Padding = new Thickness(12, 8, 12, 10) };
        stack.Children.Add(new TextBlock
        {
            Text = "CONNECTION TYPES",
            FontSize = 10,
            CharacterSpacing = 60,
            Foreground = NodeCaptionForeground,
        });

        foreach (var t in PinTypes.Where(t => !string.IsNullOrEmpty(t.Name)))
        {
            var brush = TypeBrush(t.Name!);
            var row = new Grid { ColumnSpacing = 8 };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var swatch = new Border
            {
                Width = 12,
                Height = 12,
                CornerRadius = new CornerRadius(3),
                Background = brush,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var name = new TextBlock
            {
                Text = char.ToUpperInvariant(t.Name![0]) + t.Name[1..],
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var line = new Rectangle
            {
                Width = 28,
                Height = 2,
                Fill = brush,
                VerticalAlignment = VerticalAlignment.Center,
            };

            Grid.SetColumn(name, 1);
            Grid.SetColumn(line, 2);
            row.Children.Add(swatch);
            row.Children.Add(name);
            row.Children.Add(line);
            stack.Children.Add(row);
        }

        _legend.Child = new ScrollViewer { MaxHeight = 340, Content = stack };
    }

    private void UpdateHint()
    {
        if (_hint == null) return;

        _hint.Visibility = ShowHint && HintHasSpace() ? Visibility.Visible : Visibility.Collapsed;
        if (_hintText != null) _hintText.Text = HintText ?? string.Empty;
    }

    private void UpdateConflicts()
    {
        var fresh = _edgeViews
            .Where(e => e.Conflict != null)
            .Select(e => new DiagramConflict(e.Edge, e.Conflict!,
                $"{e.From.Node.DisplayText}.{e.FromPin?.DisplayLabel} \u2192 {e.To.Node.DisplayText}.{e.ToPin?.DisplayLabel}"))
            .ToList();

        bool same = fresh.Count == Conflicts.Count &&
                    fresh.Zip(Conflicts, (a, b) => a.Message == b.Message && a.Detail == b.Detail).All(x => x);

        if (!same)
        {
            Conflicts.Clear();
            foreach (var c in fresh) Conflicts.Add(c);
            ConflictsChanged?.Invoke(this, EventArgs.Empty);
        }

        UpdateConflictPanel();
    }

    private void UpdateConflictPanel()
    {
        if (_conflictPanel == null) return;

        bool show = ShowConflicts && Conflicts.Count > 0;
        _conflictPanel.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        if (!show) return;

        var stack = new StackPanel { Spacing = 8, Padding = new Thickness(12, 8, 12, 10) };
        stack.Children.Add(new TextBlock
        {
            Text = $"Conflicts ({Conflicts.Count})",
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = ConflictFill,
        });

        foreach (var c in Conflicts)
        {
            var item = new StackPanel { Spacing = 2 };
            item.Children.Add(new TextBlock
            {
                Text = c.Message,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            });
            item.Children.Add(new TextBlock
            {
                Text = c.Detail,
                FontSize = 11,
                TextWrapping = TextWrapping.Wrap,
                Foreground = NodeCaptionForeground,
            });
            stack.Children.Add(item);
        }

        _conflictPanel.BorderBrush = ConflictFill;
        _conflictPanel.Child = new ScrollViewer { MaxHeight = 220, Content = stack };
    }

    private void UpdateAnimation()
    {
        bool any = IsLoaded && _edgeViews.Any(e => e.Animated || AnimateEdges);

        if (any && _animTimer == null)
        {
            _animTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(40) };
            _animTimer.Tick += (_, _) =>
            {
                _dashPhase -= 0.6;
                foreach (var ev in _edgeViews)
                {
                    if (ev.Path != null && ev.Conflict == null && (ev.Animated || AnimateEdges))
                        ev.Path.StrokeDashOffset = _dashPhase;
                }
            };
            _animTimer.Start();
        }
        else if (!any)
        {
            StopAnimation();
        }
    }

    private void StopAnimation()
    {
        _animTimer?.Stop();
        _animTimer = null;
    }

    public bool AllowConnect
    {
        get { return (bool)GetValue(AllowConnectProperty); }
        set { SetValue(AllowConnectProperty, value); }
    }

    public static readonly DependencyProperty AllowConnectProperty =
        DependencyProperty.Register(nameof(AllowConnect), typeof(bool), typeof(Diagram), new PropertyMetadata(true));

    public bool AllowConflictingConnections
    {
        get { return (bool)GetValue(AllowConflictingConnectionsProperty); }
        set { SetValue(AllowConflictingConnectionsProperty, value); }
    }

    public static readonly DependencyProperty AllowConflictingConnectionsProperty =
        DependencyProperty.Register(nameof(AllowConflictingConnections), typeof(bool), typeof(Diagram), new PropertyMetadata(false));

    public bool AllowDelete
    {
        get { return (bool)GetValue(AllowDeleteProperty); }
        set { SetValue(AllowDeleteProperty, value); }
    }

    public static readonly DependencyProperty AllowDeleteProperty =
        DependencyProperty.Register(nameof(AllowDelete), typeof(bool), typeof(Diagram), new PropertyMetadata(true));

    public bool ShowLegend
    {
        get { return (bool)GetValue(ShowLegendProperty); }
        set { SetValue(ShowLegendProperty, value); }
    }

    public static readonly DependencyProperty ShowLegendProperty =
        DependencyProperty.Register(nameof(ShowLegend), typeof(bool), typeof(Diagram),
            new PropertyMetadata(false, (d, e) => ((Diagram)d).BuildLegend()));

    public bool ShowConflicts
    {
        get { return (bool)GetValue(ShowConflictsProperty); }
        set { SetValue(ShowConflictsProperty, value); }
    }

    public static readonly DependencyProperty ShowConflictsProperty =
        DependencyProperty.Register(nameof(ShowConflicts), typeof(bool), typeof(Diagram),
            new PropertyMetadata(true, (d, e) => ((Diagram)d).UpdateConflictPanel()));

    public bool ShowHint
    {
        get { return (bool)GetValue(ShowHintProperty); }
        set { SetValue(ShowHintProperty, value); }
    }

    public static readonly DependencyProperty ShowHintProperty =
        DependencyProperty.Register(nameof(ShowHint), typeof(bool), typeof(Diagram),
            new PropertyMetadata(false, (d, e) => ((Diagram)d).UpdateHint()));

    public string? HintText
    {
        get { return (string?)GetValue(HintTextProperty); }
        set { SetValue(HintTextProperty, value); }
    }

    public static readonly DependencyProperty HintTextProperty =
        DependencyProperty.Register(nameof(HintText), typeof(string), typeof(Diagram),
            new PropertyMetadata("Drag a pin to connect  \u00B7  Click to select  \u00B7  Delete to remove", (d, e) => ((Diagram)d).UpdateHint()));

    public bool ShowNodeIds
    {
        get { return (bool)GetValue(ShowNodeIdsProperty); }
        set { SetValue(ShowNodeIdsProperty, value); }
    }

    public static readonly DependencyProperty ShowNodeIdsProperty =
        DependencyProperty.Register(nameof(ShowNodeIds), typeof(bool), typeof(Diagram),
            new PropertyMetadata(true, (d, e) => ((Diagram)d).OnStructureOptionChanged()));

    public bool ShowPinTypes
    {
        get { return (bool)GetValue(ShowPinTypesProperty); }
        set { SetValue(ShowPinTypesProperty, value); }
    }

    public static readonly DependencyProperty ShowPinTypesProperty =
        DependencyProperty.Register(nameof(ShowPinTypes), typeof(bool), typeof(Diagram),
            new PropertyMetadata(true, (d, e) => ((Diagram)d).OnStructureOptionChanged()));

    public DiagramPinShape PinShape
    {
        get { return (DiagramPinShape)GetValue(PinShapeProperty); }
        set { SetValue(PinShapeProperty, value); }
    }

    public static readonly DependencyProperty PinShapeProperty =
        DependencyProperty.Register(nameof(PinShape), typeof(DiagramPinShape), typeof(Diagram),
            new PropertyMetadata(DiagramPinShape.Square, (d, e) => ((Diagram)d).OnStructureOptionChanged()));

    public bool AnimateEdges
    {
        get { return (bool)GetValue(AnimateEdgesProperty); }
        set { SetValue(AnimateEdgesProperty, value); }
    }

    public static readonly DependencyProperty AnimateEdgesProperty =
        DependencyProperty.Register(nameof(AnimateEdges), typeof(bool), typeof(Diagram),
            new PropertyMetadata(false, (d, e) => { var g = (Diagram)d; g.ApplyEdgeBrushes(); g.UpdateAnimation(); }));

    public bool ColorEdgesByType
    {
        get { return (bool)GetValue(ColorEdgesByTypeProperty); }
        set { SetValue(ColorEdgesByTypeProperty, value); }
    }

    public static readonly DependencyProperty ColorEdgesByTypeProperty =
        DependencyProperty.Register(nameof(ColorEdgesByType), typeof(bool), typeof(Diagram),
            new PropertyMetadata(true, (d, e) => ((Diagram)d).ApplyEdgeBrushes()));

    public Brush? ConflictBrush
    {
        get { return (Brush?)GetValue(ConflictBrushProperty); }
        set { SetValue(ConflictBrushProperty, value); }
    }

    public static readonly DependencyProperty ConflictBrushProperty =
        DependencyProperty.Register(nameof(ConflictBrush), typeof(Brush), typeof(Diagram),
            new PropertyMetadata(null, (d, e) => { var g = (Diagram)d; g.ApplyEdgeBrushes(); g.UpdateConflictPanel(); }));
}
