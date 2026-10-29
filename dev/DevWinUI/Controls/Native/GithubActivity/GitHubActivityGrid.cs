using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Input;
using System.Collections.Specialized;
using System.Globalization;

namespace DevWinUI;

[TemplatePart(Name = CanvasPartName, Type = typeof(Canvas))]
[TemplatePart(Name = TotalPartName, Type = typeof(TextBlock))]
[TemplatePart(Name = LegendPartName, Type = typeof(StackPanel))]
[TemplatePart(Name = TipPartName, Type = typeof(Border))]
[TemplatePart(Name = TipCountPartName, Type = typeof(TextBlock))]
[TemplatePart(Name = TipDatePartName, Type = typeof(TextBlock))]
public sealed partial class GitHubActivityGrid : Control
{
    private const string CanvasPartName = "PART_Canvas";
    private const string TotalPartName = "PART_Total";
    private const string LegendPartName = "PART_Legend";
    private const string TipPartName = "PART_Tip";
    private const string TipCountPartName = "PART_TipCount";
    private const string TipDatePartName = "PART_TipDate";

    private const double LeftPad = 34;
    private const double TopPad = 20;

    private Canvas _canvas = new();
    private TextBlock _total = new();
    private StackPanel _legend = new();
    private Border _tip = new();
    private TextBlock _tipCount = new();
    private TextBlock _tipDate = new();

    private readonly List<GithubActivityGridCell> _cells = new();
    private readonly List<Border> _swatches = new();

    private double _last = double.NaN;
    private bool _loopOn;

    private bool _templated, _loaded, _inView, _hover;
    private bool _revealing, _revealed;
    private double _revealT, _stagger;

    private bool _tipShown;
    private double _tipOpacity;
    private double _tx, _ty, _vx, _vy, _tgx, _tgy;

    private double _canvasW;

    public GitHubActivityGrid()
    {
        DefaultStyleKey = typeof(GitHubActivityGrid);

        Loaded += (_, _) =>
        {
            _loaded = true;
            AttachDays();
            Rebuild();
        };
        Unloaded += (_, _) =>
        {
            _loaded = false;
            DetachDays();
            StopLoop();
        };
        EffectiveViewportChanged += OnEffectiveViewportChanged;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        StopLoop();

        _canvas.PointerEntered -= OnCanvasPointerEntered;
        _canvas.PointerExited -= OnCanvasPointerExited;

        _canvas = GetTemplateChild(CanvasPartName) as Canvas ?? new Canvas();
        _total = GetTemplateChild(TotalPartName) as TextBlock ?? new TextBlock();
        _legend = GetTemplateChild(LegendPartName) as StackPanel ?? new StackPanel();
        _tip = GetTemplateChild(TipPartName) as Border ?? new Border();
        _tipCount = GetTemplateChild(TipCountPartName) as TextBlock ?? new TextBlock();
        _tipDate = GetTemplateChild(TipDatePartName) as TextBlock ?? new TextBlock();

        _canvas.PointerEntered += OnCanvasPointerEntered;
        _canvas.PointerExited += OnCanvasPointerExited;

        _templated = true;
        Rebuild();
    }

    private void OnCanvasPointerEntered(object sender, PointerRoutedEventArgs e) => _hover = true;

    private void OnCanvasPointerExited(object sender, PointerRoutedEventArgs e)
    {
        _hover = false;
        HideTip();
    }

    private Brush? LevelBrush(int level) => level switch
    {
        0 => Level0Brush,
        1 => Level1Brush,
        2 => Level2Brush,
        3 => Level3Brush,
        _ => Level4Brush
    };

    private void ApplyBrushes()
    {
        foreach (var c in _cells)
        {
            c.Btn.Background = LevelBrush(c.Level);
        }

        for (int i = 0; i < _swatches.Count; i++)
        {
            _swatches[i].Background = LevelBrush(i);
        }
    }

    private static int LevelFor(int count, int max) =>
        count <= 0 ? 0 : Math.Clamp((int)Math.Ceiling(4.0 * count / max), 1, 4);

    private string Unit(int count) => count == 1 ? UnitSingular : UnitPlural;

    private void OnDaysChanged(IList<ActivityDay>? oldValue, IList<ActivityDay>? newValue)
    {
        if (oldValue is INotifyCollectionChanged oldNotify)
        {
            oldNotify.CollectionChanged -= OnDaysCollectionChanged;
        }

        if (_loaded && newValue is INotifyCollectionChanged newNotify)
        {
            newNotify.CollectionChanged += OnDaysCollectionChanged;
        }

        _revealed = false;
        _revealing = false;
        Rebuild();
    }

    private void OnDaysCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => Rebuild();

    private void AttachDays()
    {
        if (Days is INotifyCollectionChanged notify)
        {
            notify.CollectionChanged -= OnDaysCollectionChanged;
            notify.CollectionChanged += OnDaysCollectionChanged;
        }
    }

    private void DetachDays()
    {
        if (Days is INotifyCollectionChanged notify)
        {
            notify.CollectionChanged -= OnDaysCollectionChanged;
        }
    }

    private void Rebuild()
    {
        if (!_loaded || !_templated)
        {
            return;
        }

        _canvas.Children.Clear();
        _cells.Clear();
        _legend.Children.Clear();
        _swatches.Clear();
        _tipShown = false;
        _tipOpacity = 0;
        _tip.Opacity = 0;
        _tip.Visibility = Visibility.Collapsed;

        var days = new List<ActivityDay>(Days ?? Array.Empty<ActivityDay>());
        days.Sort((a, b) => a.Date.CompareTo(b.Date));

        if (days.Count == 0)
        {
            _canvas.Width = _canvas.Height = 0;
            _total.Text = string.Empty;
            return;
        }

        double s = Math.Max(4, CellSize), g = Math.Max(0, CellGap), step = s + g;
        int n = days.Count;

        int max = MaxCount ?? days.Max(d => d.Count);
        if (max <= 0)
        {
            max = 1;
        }

        DateTime first = days[0].Date.Date;
        DateTime last = days[n - 1].Date.Date;
        DateTime weekStart = first.AddDays(-(int)first.DayOfWeek);

        var colFirst = new SortedDictionary<int, DateTime>();
        long total = 0;
        bool hideForReveal = !_revealed && !ReduceMotion;

        for (int i = 0; i < n; i++)
        {
            var day = days[i];
            DateTime d = day.Date.Date;
            int col = (int)(d - weekStart).TotalDays / 7;
            int row = (int)d.DayOfWeek;
            total += Math.Max(0, day.Count);
            if (!colFirst.ContainsKey(col) || d < colFirst[col])
            {
                colFirst[col] = d;
            }

            string label = string.Format(CultureInfo.CurrentCulture, "{0} {1} on {2:MMMM d, yyyy}", day.Count, Unit(day.Count), d);

            int level = LevelFor(day.Count, max);
            var t = new CompositeTransform();
            var btn = new Button
            {
                Width = s,
                Height = s,
                CornerRadius = new CornerRadius(Math.Max(2, s * 0.22)),
                Background = LevelBrush(level),
                RenderTransform = t,
                RenderTransformOrigin = new Point(0.5, 0.5),
                Opacity = hideForReveal ? 0 : 1
            };
            AutomationProperties.SetName(btn, label);

            var cell = new GithubActivityGridCell
            {
                Btn = btn,
                T = t,
                Day = day,
                Index = i,
                Row = row,
                Level = level,
                X = LeftPad + col * step,
                Y = TopPad + row * step
            };
            if (hideForReveal)
            {
                t.ScaleX = t.ScaleY = 0.5;
            }

            Canvas.SetLeft(btn, cell.X);
            Canvas.SetTop(btn, cell.Y);
            btn.PointerEntered += (_, _) => ShowTip(cell);
            btn.GotFocus += (_, _) =>
            {
                if (btn.FocusState == FocusState.Keyboard)
                {
                    ShowTip(cell);
                }
            };
            btn.LostFocus += (_, _) =>
            {
                if (!_hover)
                {
                    HideTip();
                }
            };

            _canvas.Children.Add(btn);
            _cells.Add(cell);
        }

        int cols = colFirst.Keys.Max() + 1;
        _canvasW = LeftPad + cols * step - g;
        _canvas.Width = _canvasW;
        _canvas.Height = TopPad + 7 * step - g;

        AddMonthLabels(colFirst, step);
        AddWeekdayLabels(step, s);

        _total.Text = string.Format(CultureInfo.CurrentCulture, "{0:N0} {1}, {2:MMM d, yyyy} – {3:MMM d, yyyy}", total, Unit((int)Math.Min(total, int.MaxValue)), first, last);
        BuildLegend(s);
        AutomationProperties.SetName(this, _total.Text);

        _stagger = 0.9 / Math.Max(1, n);
        if (_inView && !_revealed)
        {
            StartReveal();
        }
    }

    private void AddMonthLabels(SortedDictionary<int, DateTime> colFirst, double step)
    {
        int prevMonth = -1, lastLabelCol = -100;
        foreach (var kv in colFirst)
        {
            int col = kv.Key;
            DateTime d = kv.Value;

            if (col == 0 && colFirst.TryGetValue(1, out var next) && next.Month != d.Month)
            {
                continue;
            }

            if (d.Month != prevMonth && col - lastLabelCol >= 3)
            {
                var tb = new TextBlock
                {
                    Text = d.ToString("MMM", CultureInfo.CurrentCulture),
                    FontSize = 11,
                    Opacity = 0.65,
                    IsHitTestVisible = false
                };
                Canvas.SetLeft(tb, LeftPad + col * step);
                Canvas.SetTop(tb, 0);
                _canvas.Children.Add(tb);
                lastLabelCol = col;
            }
            prevMonth = d.Month;
        }
    }

    private void AddWeekdayLabels(double step, double s)
    {
        foreach (int row in new[] { 1, 3, 5 })
        {
            var tb = new TextBlock
            {
                Text = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[row],
                FontSize = 10,
                Opacity = 0.6,
                IsHitTestVisible = false
            };
            Canvas.SetLeft(tb, 0);
            Canvas.SetTop(tb, TopPad + row * step + Math.Max(0, (s - 13) / 2));
            _canvas.Children.Add(tb);
        }
    }

    private void BuildLegend(double s)
    {
        _legend.Children.Add(new TextBlock { Text = LessText, FontSize = 11, Opacity = 0.65, VerticalAlignment = VerticalAlignment.Center });
        for (int i = 0; i < 5; i++)
        {
            var swatch = new Border
            {
                Width = s,
                Height = s,
                CornerRadius = new CornerRadius(Math.Max(2, s * 0.22)),
                Background = LevelBrush(i)
            };
            _swatches.Add(swatch);
            _legend.Children.Add(swatch);
        }
        _legend.Children.Add(new TextBlock { Text = MoreText, FontSize = 11, Opacity = 0.65, VerticalAlignment = VerticalAlignment.Center });
    }

    private void OnEffectiveViewportChanged(FrameworkElement sender, EffectiveViewportChangedEventArgs args)
    {
        if (ActualWidth <= 0 || ActualHeight <= 0)
        {
            return;
        }

        var bounds = new Rect(0, 0, ActualWidth, ActualHeight);
        bounds.Intersect(args.EffectiveViewport);
        _inView = !bounds.IsEmpty && bounds.Width > 0 && bounds.Height > 0;

        if (_inView && !_revealed && !_revealing && _cells.Count > 0)
        {
            StartReveal();
        }
    }

    private void StartReveal()
    {
        if (_cells.Count == 0)
        {
            return;
        }

        if (ReduceMotion)
        {
            ShowAllCells();
            _revealed = true;
            return;
        }

        _revealing = true;
        _revealT = 0;
        EnsureLoop();
    }

    private void ShowAllCells()
    {
        foreach (var c in _cells)
        {
            c.Btn.Opacity = 1;
            c.T.ScaleX = c.T.ScaleY = 1;
        }
    }

    private void ShowTip(GithubActivityGridCell cell)
    {
        if (_revealing)
        {
            return;
        }

        int c = cell.Day.Count;
        _tipCount.Text = string.Format(CultureInfo.CurrentCulture, "{0} {1}", c, Unit(c));
        _tipDate.Text = cell.Day.Date.ToString("dddd, MMM d, yyyy", CultureInfo.CurrentCulture);

        _tip.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        Size size = _tip.DesiredSize;
        double s = Math.Max(4, CellSize);

        double x = cell.X + s / 2 - size.Width / 2;
        x = Math.Clamp(x, 0, Math.Max(0, _canvasW - size.Width));
        double y = cell.Row < 2 ? cell.Y + s + 6 : cell.Y - size.Height - 6;

        bool fresh = !_tipShown && _tipOpacity <= 0.01;
        _tipShown = true;
        _tgx = x;
        _tgy = y;
        if (fresh)
        {
            _tx = x;
            _ty = y;
            _vx = _vy = 0;
            Canvas.SetLeft(_tip, _tx);
            Canvas.SetTop(_tip, _ty);
        }
        EnsureLoop();
    }

    private void HideTip()
    {
        if (!_tipShown)
        {
            return;
        }

        _tipShown = false;
        EnsureLoop();
    }

    private bool StepTip(double dt, bool motion)
    {
        bool moving = false;

        double to = _tipShown ? 1 : 0;
        if (!motion)
        {
            _tipOpacity = to;
        }
        else
        {
            _tipOpacity += (to - _tipOpacity) * (1 - Math.Exp(-dt * 18));
        }

        if (Math.Abs(to - _tipOpacity) < 0.01)
        {
            _tipOpacity = to;
        }
        else
        {
            moving = true;
        }

        _tip.Opacity = _tipOpacity;
        _tip.Visibility = (_tipShown || _tipOpacity > 0) ? Visibility.Visible : Visibility.Collapsed;

        if (!motion)
        {
            _tx = _tgx;
            _ty = _tgy;
            _vx = _vy = 0;
        }
        else
        {
            const double k = 420, damp = 26;
            int steps = Math.Max(1, (int)Math.Ceiling(dt * 120));
            double h = dt / steps;
            for (int i = 0; i < steps; i++)
            {
                _vx += (-k * (_tx - _tgx) - damp * _vx) * h;
                _vy += (-k * (_ty - _tgy) - damp * _vy) * h;
                _tx += _vx * h;
                _ty += _vy * h;
            }

            bool settled = Math.Abs(_tx - _tgx) < 0.1 && Math.Abs(_ty - _tgy) < 0.1
                           && Math.Abs(_vx) < 0.5 && Math.Abs(_vy) < 0.5;
            if (settled)
            {
                _tx = _tgx;
                _ty = _tgy;
                _vx = _vy = 0;
            }
            else if (_tipShown)
            {
                moving = true;
            }
        }

        Canvas.SetLeft(_tip, _tx);
        Canvas.SetTop(_tip, _ty);
        return moving;
    }

    private void EnsureLoop()
    {
        if (_loopOn || !_loaded)
        {
            return;
        }

        _loopOn = true;
        _last = double.NaN;
        CompositionTarget.Rendering += OnRendering;
    }

    private void StopLoop()
    {
        if (!_loopOn)
        {
            return;
        }

        _loopOn = false;
        CompositionTarget.Rendering -= OnRendering;
    }

    private void OnRendering(object? sender, object e)
    {
        double now = e is RenderingEventArgs args
            ? args.RenderingTime.TotalSeconds
            : (double.IsNaN(_last) ? 0 : _last) + 1.0 / 60;
        double dt = double.IsNaN(_last) ? 1.0 / 60 : Math.Clamp(now - _last, 0.001, 0.05);
        _last = now;

        bool anim = false;

        if (_revealing)
        {
            _revealT += dt;
            const double dur = 0.35;
            int n = _cells.Count;
            bool done = true;

            for (int i = 0; i < n; i++)
            {
                double k = n - 1 - _cells[i].Index;
                double p = Math.Clamp((_revealT - k * _stagger) / dur, 0, 1);
                if (p < 1)
                {
                    done = false;
                }
                double ease = 1 - Math.Pow(1 - p, 3);
                var c = _cells[i];
                c.Btn.Opacity = ease;
                c.T.ScaleX = c.T.ScaleY = 0.5 + 0.5 * ease;
            }

            if (done)
            {
                _revealing = false;
                _revealed = true;
            }
            else
            {
                anim = true;
            }
        }

        anim |= StepTip(dt, !ReduceMotion);
        if (!anim)
        {
            StopLoop();
        }
    }
}
