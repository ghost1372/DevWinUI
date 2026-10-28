using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Shapes;
using System.Collections.Specialized;
using Color = Windows.UI.Color;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

[TemplatePart(Name = PartHost, Type = typeof(Grid))]
public sealed partial class BranchedMenu : Control
{
    private const string PartHost = "PART_Host";

    private const double IdleOpacity = 0.65;
    private const double MarkerSize = 8;
    private const double LineMix = 0.24;

    private static readonly FontFamily IconFont = new("Segoe Fluent Icons, Segoe MDL2 Assets");

    private sealed class Section
    {
        public int Index;
        public bool HasChildren;
        public bool IsOpen;
        public double ContentHeight;
        public Button Header = null!;
        public TextBlock Label = null!;
        public FontIcon? Chevron;
        public Grid? Wrapper;
        public RotateTransform? Rotate;
        public Storyboard? FoldStoryboard;
    }

    private sealed class Entry
    {
        public string Value = "";
        public object Item = null!;
        public Section Section = null!;
        public Button Button = null!;
        public TextBlock Label = null!;
        public FontIcon? Icon;
        public Path? Accent;
        public double Dash;
        public Storyboard? DrawStoryboard;
        public bool IsHeader;
    }

    private readonly List<Section> _sections = new();
    private readonly List<Entry> _entries = new();
    private readonly List<Path> _branches = new();
    private readonly Dictionary<string, Entry> _lookup = new();
    private readonly SolidColorBrush _lineFallback = new();
    private readonly SolidColorBrush _transparent = new(Colors.Transparent);
    private readonly CubicEase _ease = new() { EasingMode = EasingMode.EaseInOut };

    private Grid? _host;
    private Grid? _root;
    private Rectangle? _rail;
    private Ellipse? _marker;
    private Entry? _active;
    private Storyboard? _slideStoryboard;
    private INotifyCollectionChanged? _itemsSource;
    private bool _sliding;
    private bool _buildQueued;
    private bool _resetState = true;

    public BranchedMenu()
    {
        DefaultStyleKey = typeof(BranchedMenu);
        SetValue(ItemsProperty, new ObservableCollection<BranchedMenuItem>());

        RegisterPropertyChangedCallback(FontSizeProperty, (_, _) => Rebuild());
        RegisterPropertyChangedCallback(ForegroundProperty, (_, _) =>
        {
            UpdateLineFallback();
            ApplyBrushes();
        });

        ActualThemeChanged += (_, _) => UpdateLineFallback();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public void Select(string value, bool raiseEvent = false)
    {
        EnsureBuilt();
        if (_lookup.TryGetValue(value, out var entry)) SetActive(entry, animate: true, raise: raiseEvent);
    }

    public void SetOpen(int index, bool open)
    {
        EnsureBuilt();
        if (index < 0 || index >= _sections.Count) return;

        var section = _sections[index];
        if (!section.HasChildren || section.IsOpen == open) return;
        Toggle(section);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _host = GetTemplateChild(PartHost) as Grid;
        Rebuild();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        WatchItems();
        UpdateLineFallback();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_itemsSource is null) return;
        _itemsSource.CollectionChanged -= OnItemsCollectionChanged;
        _itemsSource = null;
    }

    private void WatchItems()
    {
        if (_itemsSource != null) _itemsSource.CollectionChanged -= OnItemsCollectionChanged;

        _itemsSource = IsLoaded ? Items as INotifyCollectionChanged : null;
        if (_itemsSource != null) _itemsSource.CollectionChanged += OnItemsCollectionChanged;
    }

    private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => Rebuild();

    private void Rebuild()
    {
        if (_buildQueued) return;
        _buildQueued = true;
        DispatcherQueue.TryEnqueue(FlushBuild);
    }

    private void FlushBuild()
    {
        if (_buildQueued) Build();
    }

    private void EnsureBuilt()
    {
        if (_buildQueued) Build();
    }

    private void Build()
    {
        _buildQueued = false;
        if (_host is null) return;

        bool reset = _resetState;
        _resetState = false;

        HashSet<int> open;
        string? start;
        if (reset)
        {
            open = new HashSet<int>(DefaultOpen ?? Array.Empty<int>());
            start = string.IsNullOrEmpty(DefaultActive) ? null : DefaultActive;
        }
        else
        {
            open = new HashSet<int>();
            foreach (var previous in _sections)
            {
                if (previous.IsOpen) open.Add(previous.Index);
            }
            start = _active?.Value;
        }

        StopAll();
        _sections.Clear();
        _entries.Clear();
        _branches.Clear();
        _lookup.Clear();
        _active = null;

        var items = Items ?? Array.Empty<BranchedMenuItem>();
        double row = Math.Max(1, RowHeight);
        double lineWidth = Math.Max(0.1, LineWidth);
        double indent = Indent;
        double trunk = Trunk;
        double radius = Math.Max(0, Math.Min(Radius, row / 2));
        double endX = Math.Max(trunk + radius, indent - 6);
        string? fallback = null;

        var stack = new StackPanel { Orientation = Orientation.Vertical };

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var children = item.Children;
            var section = new Section
            {
                Index = i,
                HasChildren = children is { Count: > 0 },
            };
            section.IsOpen = section.HasChildren && open.Contains(i);
            _sections.Add(section);

            var header = CreateButton(row, new Thickness(indent, 0, 12, 0));
            section.Header = header;
            AutomationProperties.SetName(header, item.Label);

            var label = new TextBlock
            {
                Text = item.Label,
                FontSize = FontSize + 1,
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
            };
            section.Label = label;

            var headerGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = GridLength.Auto },
                },
            };
            headerGrid.Children.Add(label);

            if (section.HasChildren)
            {
                section.Rotate = new RotateTransform { Angle = section.IsOpen ? 0 : -90 };
                var chevron = new FontIcon
                {
                    Glyph = "\uE70D",
                    FontFamily = IconFont,
                    FontSize = 10,
                    Opacity = 0.6,
                    Margin = new Thickness(8, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    RenderTransform = section.Rotate,
                };
                section.Chevron = chevron;
                Grid.SetColumn(chevron, 1);
                headerGrid.Children.Add(chevron);
                AutomationProperties.SetItemStatus(header, section.IsOpen ? "Expanded" : "Collapsed");
            }

            header.Content = headerGrid;
            stack.Children.Add(header);

            if (!section.HasChildren)
            {
                var leaf = new Entry
                {
                    Value = item.Value ?? item.Label,
                    Item = item,
                    Section = section,
                    Button = header,
                    Label = label,
                    IsHeader = true,
                };
                Register(leaf);
                header.Click += (_, _) => SetActive(leaf, animate: true, raise: true);
                continue;
            }

            header.Click += (_, _) => Toggle(section);

            int count = children.Count;
            section.ContentHeight = count * row;

            var content = new Grid { Height = section.ContentHeight, VerticalAlignment = VerticalAlignment.Top };
            var canvas = new Canvas
            {
                Width = indent,
                Height = section.ContentHeight,
                HorizontalAlignment = HorizontalAlignment.Left,
                IsHitTestVisible = false,
            };
            var rows = new StackPanel { Margin = new Thickness(indent, 0, 0, 0) };
            content.Children.Add(canvas);
            content.Children.Add(rows);

            var accents = new List<Path>(count);

            for (int j = 0; j < count; j++)
            {
                var child = children[j];
                double cy = j * row + row / 2;

                var branch = new Path
                {
                    Data = BranchGeometry(trunk, cy, radius, endX),
                    StrokeThickness = lineWidth,
                    StrokeLineJoin = PenLineJoin.Round,
                    IsHitTestVisible = false,
                };
                _branches.Add(branch);
                canvas.Children.Add(branch);

                double length = (cy - radius) + Math.PI / 2 * radius + (endX - trunk - radius);
                double dash = length / lineWidth + 2;
                var accent = new Path
                {
                    Data = BranchGeometry(trunk, cy, radius, endX),
                    StrokeThickness = lineWidth,
                    StrokeLineJoin = PenLineJoin.Round,
                    StrokeDashArray = new DoubleCollection { dash, dash },
                    Visibility = Visibility.Collapsed,
                    IsHitTestVisible = false,
                };
                accents.Add(accent);

                var button = CreateButton(row, new Thickness(0, 0, 12, 0));
                AutomationProperties.SetName(button, child.Label);

                var rowGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    },
                };

                FontIcon? icon = null;
                if (!string.IsNullOrEmpty(child.Glyph))
                {
                    icon = new FontIcon
                    {
                        Glyph = child.Glyph,
                        FontFamily = IconFont,
                        FontSize = FontSize + 2,
                        Margin = new Thickness(0, 0, 8, 0),
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    rowGrid.Children.Add(icon);
                }

                var text = new TextBlock
                {
                    Text = child.Label,
                    FontSize = FontSize,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                };
                Grid.SetColumn(text, 1);
                rowGrid.Children.Add(text);
                button.Content = rowGrid;
                rows.Children.Add(button);

                var entry = new Entry
                {
                    Value = child.Value,
                    Item = child,
                    Section = section,
                    Button = button,
                    Label = text,
                    Icon = icon,
                    Accent = accent,
                    Dash = dash,
                };
                Register(entry);
                button.Click += (_, _) => SetActive(entry, animate: true, raise: true);

                if (fallback is null && section.IsOpen) fallback = child.Value;
            }

            foreach (var accent in accents) canvas.Children.Add(accent);

            var clip = new RectangleGeometry();
            var wrapper = new Grid
            {
                Height = section.IsOpen ? section.ContentHeight : 0,
                Visibility = section.IsOpen ? Visibility.Visible : Visibility.Collapsed,
                Clip = clip,
            };
            wrapper.Children.Add(content);
            wrapper.SizeChanged += (_, e) =>
            {
                clip.Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);
                if (_active is { } active && section.Index < active.Section.Index) SyncMarker();
            };
            section.Wrapper = wrapper;
            stack.Children.Add(wrapper);
        }

        _rail = new Rectangle
        {
            Width = lineWidth,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(trunk - lineWidth / 2, 0, 0, 0),
            IsHitTestVisible = false,
        };

        _marker = new Ellipse
        {
            Width = MarkerSize,
            Height = MarkerSize,
            Visibility = Visibility.Collapsed,
            IsHitTestVisible = false,
        };
        Canvas.SetLeft(_marker, trunk - MarkerSize / 2);

        var overlay = new Canvas { IsHitTestVisible = false };
        overlay.Children.Add(_marker);

        _root = new Grid { HorizontalAlignment = HorizontalAlignment.Left, MaxWidth = ResolveMaxWidth() };
        _root.Children.Add(_rail);
        _root.Children.Add(stack);
        _root.Children.Add(overlay);

        _host.Children.Clear();
        _host.Children.Add(_root);

        UpdateLineFallback();
        ApplyBrushes();

        if (reset) start ??= fallback;
        if (start != null && _lookup.TryGetValue(start, out var first))
            SetActive(first, animate: false, raise: false);
    }

    private void Register(Entry entry)
    {
        _entries.Add(entry);
        _lookup.TryAdd(entry.Value, entry);
    }

    private static PathGeometry BranchGeometry(double x, double cy, double radius, double endX)
    {
        var figure = new PathFigure { StartPoint = new Point(x, 0), IsClosed = false, IsFilled = false };
        if (radius > 0.01)
        {
            figure.Segments.Add(new LineSegment { Point = new Point(x, cy - radius) });
            figure.Segments.Add(new ArcSegment
            {
                Point = new Point(x + radius, cy),
                Size = new Size(radius, radius),
                SweepDirection = SweepDirection.Counterclockwise,
            });
        }
        else
        {
            figure.Segments.Add(new LineSegment { Point = new Point(x, cy) });
        }
        figure.Segments.Add(new LineSegment { Point = new Point(endX, cy) });

        var geometry = new PathGeometry();
        geometry.Figures.Add(figure);
        return geometry;
    }

    private Button CreateButton(double height, Thickness padding) => new()
    {
        Height = height,
        Padding = padding,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        HorizontalContentAlignment = HorizontalAlignment.Left,
        VerticalContentAlignment = VerticalAlignment.Center,
        Background = _transparent,
        BorderThickness = new Thickness(0),
        CornerRadius = new CornerRadius(6),
    };

    private double ResolveMaxWidth()
    {
        double width = MenuMaxWidth;
        return double.IsNaN(width) || width <= 0 ? double.PositiveInfinity : width;
    }

    private void SetActive(Entry next, bool animate, bool raise)
    {
        if (!ReferenceEquals(_active, next))
        {
            if (_active != null) Deactivate(_active);
            _active = next;
            ApplyStyle(next, active: true);
            DrawAccent(next, animate && next.Section.IsOpen);
            MoveMarker(animate);
        }

        if (raise) ItemSelected?.Invoke(this, new BranchedMenuSelectedEventArgs(next.Value, next.Item));
    }

    private void Deactivate(Entry entry)
    {
        ApplyStyle(entry, active: false);
        entry.DrawStoryboard?.Stop();
        entry.DrawStoryboard = null;
        if (entry.Accent != null) entry.Accent.Visibility = Visibility.Collapsed;
    }

    private void ApplyStyle(Entry entry, bool active)
    {
        var brush = active ? EffectiveAccent : EffectiveInk;

        entry.Label.Foreground = brush;
        entry.Label.Opacity = active || entry.IsHeader ? 1 : IdleOpacity;
        if (!entry.IsHeader) entry.Label.FontWeight = active ? FontWeights.Medium : FontWeights.Normal;

        if (entry.Icon != null)
        {
            entry.Icon.Foreground = brush;
            entry.Icon.Opacity = active ? 1 : IdleOpacity;
        }

        AutomationProperties.SetItemStatus(entry.Button, active ? "Selected" : "");
    }

    private void DrawAccent(Entry entry, bool animate)
    {
        var accent = entry.Accent;
        if (accent is null) return;

        entry.DrawStoryboard?.Stop();
        accent.Visibility = Visibility.Visible;

        if (!animate || DrawDuration <= 0)
        {
            accent.StrokeDashOffset = 0;
            return;
        }

        accent.StrokeDashOffset = entry.Dash;
        var animation = new DoubleAnimation
        {
            From = entry.Dash,
            To = 0,
            Duration = Ms(DrawDuration),
            EasingFunction = _ease,
            EnableDependentAnimation = true,
        };
        Storyboard.SetTarget(animation, accent);
        Storyboard.SetTargetProperty(animation, nameof(Shape.StrokeDashOffset));

        var storyboard = new Storyboard();
        storyboard.Children.Add(animation);
        storyboard.Completed += (_, _) =>
        {
            accent.StrokeDashOffset = 0;
            storyboard.Stop();
        };
        entry.DrawStoryboard = storyboard;
        storyboard.Begin();
    }

    private double MarkerY(Section section)
    {
        double y = 0;
        for (int i = 0; i < section.Index; i++)
        {
            y += RowHeight;
            if (_sections[i].Wrapper is { } wrapper) y += wrapper.Height;
        }
        return y + (RowHeight - MarkerSize) / 2;
    }

    private void MoveMarker(bool animate)
    {
        if (_marker is null || _active is null) return;

        double y = MarkerY(_active.Section);
        double from = Canvas.GetTop(_marker);
        bool wasVisible = _marker.Visibility == Visibility.Visible;
        _marker.Visibility = Visibility.Visible;

        _slideStoryboard?.Stop();
        _slideStoryboard = null;
        _sliding = false;

        if (!animate || !wasVisible || DrawDuration <= 0 || Math.Abs(from - y) < 0.5)
        {
            Canvas.SetTop(_marker, y);
            return;
        }

        var animation = new DoubleAnimation
        {
            From = from,
            To = y,
            Duration = Ms(DrawDuration),
            EasingFunction = _ease,
            EnableDependentAnimation = true,
        };
        Storyboard.SetTarget(animation, _marker);
        Storyboard.SetTargetProperty(animation, "(Canvas.Top)");

        var storyboard = new Storyboard();
        storyboard.Children.Add(animation);
        storyboard.Completed += (_, _) =>
        {
            if (_marker != null && _active != null) Canvas.SetTop(_marker, MarkerY(_active.Section));
            storyboard.Stop();
            _sliding = false;
        };
        _slideStoryboard = storyboard;
        _sliding = true;
        storyboard.Begin();
    }

    private void SyncMarker()
    {
        if (_sliding || _marker is null || _active is null) return;

        double y = MarkerY(_active.Section);
        if (Math.Abs(Canvas.GetTop(_marker) - y) > 0.25) Canvas.SetTop(_marker, y);
    }

    private void Toggle(Section section)
    {
        if (!section.HasChildren || section.Wrapper is null) return;

        var wrapper = section.Wrapper;
        bool open = !section.IsOpen;
        section.IsOpen = open;
        AutomationProperties.SetItemStatus(section.Header, open ? "Expanded" : "Collapsed");

        double target = open ? section.ContentHeight : 0;
        double angle = open ? 0 : -90;
        double from = wrapper.Height;

        section.FoldStoryboard?.Stop();
        if (open) wrapper.Visibility = Visibility.Visible;

        if (FoldDuration <= 0)
        {
            wrapper.Height = target;
            if (!open) wrapper.Visibility = Visibility.Collapsed;
            if (section.Rotate != null) section.Rotate.Angle = angle;
            SyncMarker();
        }
        else
        {
            wrapper.Height = from;

            var height = new DoubleAnimation
            {
                From = from,
                To = target,
                Duration = Ms(FoldDuration),
                EasingFunction = _ease,
                EnableDependentAnimation = true,
            };
            Storyboard.SetTarget(height, wrapper);
            Storyboard.SetTargetProperty(height, nameof(Height));

            var storyboard = new Storyboard();
            storyboard.Children.Add(height);

            if (section.Rotate != null)
            {
                var rotation = new DoubleAnimation { To = angle, Duration = Ms(FoldDuration), EasingFunction = _ease };
                Storyboard.SetTarget(rotation, section.Rotate);
                Storyboard.SetTargetProperty(rotation, nameof(RotateTransform.Angle));
                storyboard.Children.Add(rotation);
            }

            storyboard.Completed += (_, _) =>
            {
                wrapper.Height = target;
                if (section.Rotate != null) section.Rotate.Angle = angle;
                storyboard.Stop();
                if (!open) wrapper.Visibility = Visibility.Collapsed;
                SyncMarker();
            };
            section.FoldStoryboard = storyboard;
            storyboard.Begin();
        }

        if (open && _active is { IsHeader: false } active && ReferenceEquals(active.Section, section))
            DrawAccent(active, animate: true);

        SectionToggled?.Invoke(this, new BranchedMenuToggledEventArgs(section.Index, open));
    }

    private Brush EffectiveInk => InkBrush ?? Foreground;

    private Brush EffectiveAccent => AccentBrush ?? Foreground;

    private Brush EffectiveLine => LineBrush ?? _lineFallback;

    private void ApplyBrushes()
    {
        var ink = EffectiveInk;
        var accentBrush = EffectiveAccent;
        var line = EffectiveLine;

        foreach (var section in _sections)
        {
            if (section.Chevron is null) continue;
            section.Label.Foreground = ink;
            section.Chevron.Foreground = ink;
        }

        foreach (var entry in _entries)
        {
            ApplyStyle(entry, ReferenceEquals(entry, _active));
            if (entry.Accent != null) entry.Accent.Stroke = accentBrush;
        }

        foreach (var branch in _branches) branch.Stroke = line;

        if (_rail != null) _rail.Fill = line;
        if (_marker != null) _marker.Fill = accentBrush;
    }

    private void UpdateLineFallback()
    {
        bool dark = ActualTheme == ElementTheme.Dark;
        var foreground = (Foreground as SolidColorBrush)?.Color ?? (dark ? Colors.White : Colors.Black);
        var background = dark ? Color.FromArgb(255, 32, 32, 32) : Color.FromArgb(255, 243, 243, 243);
        _lineFallback.Color = Mix(foreground, background, LineMix);
    }

    private static Color Mix(Color a, Color b, double t) => Color.FromArgb(
        255,
        (byte)Math.Round(a.R * t + b.R * (1 - t)),
        (byte)Math.Round(a.G * t + b.G * (1 - t)),
        (byte)Math.Round(a.B * t + b.B * (1 - t)));

    private static Duration Ms(int milliseconds) => new(TimeSpan.FromMilliseconds(milliseconds));

    private void StopAll()
    {
        _slideStoryboard?.Stop();
        _slideStoryboard = null;
        _sliding = false;

        foreach (var section in _sections) section.FoldStoryboard?.Stop();
        foreach (var entry in _entries) entry.DrawStoryboard?.Stop();
    }
}
