using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using VirtualKey = global::Windows.System.VirtualKey;

namespace DevWinUI;

[TemplatePart(Name = "PART_HeaderBorder", Type = typeof(Border))]
[TemplatePart(Name = "PART_FilenameText", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_AddedText", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_RemovedText", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_ApproxText", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_UnifiedSwitcher", Type = typeof(SelectorBar))]
[TemplatePart(Name = "PART_InlineItem", Type = typeof(SelectorBarItem))]
[TemplatePart(Name = "PART_BeforeItem", Type = typeof(SelectorBarItem))]
[TemplatePart(Name = "PART_AfterItem", Type = typeof(SelectorBarItem))]
[TemplatePart(Name = "PART_BusyBar", Type = typeof(ProgressBar))]
[TemplatePart(Name = "PART_Scroller", Type = typeof(ScrollViewer))]
[TemplatePart(Name = "PART_Repeater", Type = typeof(ItemsRepeater))]
public sealed partial class DiffView : Control
{
    private const int EntranceWindowMs = 700;
    private const int FadeOutMs = 90;
    private const int FadeInMs = 180;

    private readonly HashSet<int> _expanded = new();
    private readonly ICommand _expandCommand;
    private List<DiffLine> _source = new();
    private DiffRowList? _rows;
    private List<SplitLine>? _pairs;
    private CancellationTokenSource? _cts;
    private bool _refreshQueued;
    private bool _refreshPending;
    private bool _resumeOnLoad;
    private bool _approximate;
    private bool _entrancePending;
    private bool _syncingMode;
    private long _entranceUntil;
    private int _entranceCounter;
    private int _switchVersion;
    private CompositionEasingFunction? _easing;

    private RowRef[] _rowRefs = [];
    private Func<int, int, string?>? _textAt;
    private bool _selActive;
    private bool _selecting;
    private int _selSide;
    private int _anchorRow;
    private int _anchorCol;
    private int _caretRow;
    private int _caretCol;
    private int _goalCol = -1;
    private TextBlock? _nativeBlock;
    private (int Start, int End) _nativeRange;
    private bool _nativeCaretAtStart;
    private int _clickCount;
    private long _lastPressTick;
    private Point _lastPressPoint;
    private Point _dragPoint;
    private bool _textCursorOn;
    private InputSystemCursor? _textCursor;
    private DispatcherQueueTimer? _scrollTimer;
    private MenuFlyout? _menu;

    private Border? _headerBorder;
    private TextBlock? _filenameText;
    private TextBlock? _addedText;
    private TextBlock? _removedText;
    private TextBlock? _approxText;
    private SelectorBar? _unifiedSwitcher;
    private SelectorBarItem? _inlineItem;
    private SelectorBarItem? _beforeItem;
    private SelectorBarItem? _afterItem;
    private ProgressBar? _busyBar;
    private ScrollViewer? _scroller;
    private ItemsRepeater? _repeater;

    public DiffView()
    {
        DefaultStyleKey = typeof(DiffView);
        _expandCommand = new DiffCommand(ExpandRegion);
        Loaded += FileDiff_Loaded;
        Unloaded += FileDiff_Unloaded;
        ActualThemeChanged += FileDiff_ActualThemeChanged;
        AddHandler(KeyDownEvent, new KeyEventHandler(FileDiff_NativeKeyDown), true);
    }

    public event EventHandler? DiffCompleted;

    public int AddedCount { get; private set; }

    public int RemovedCount { get; private set; }

    public bool IsApproximate => _approximate;

    private bool FitsWidth => WrapLines || ViewMode == DiffViewMode.Split;

    protected override void OnApplyTemplate()
    {
        if (_scroller is not null) _scroller.SizeChanged -= Scroller_SizeChanged;
        if (_repeater is not null)
        {
            _repeater.ElementPrepared -= Repeater_ElementPrepared;
            _repeater.PointerPressed -= Repeater_PointerPressed;
            _repeater.PointerMoved -= Repeater_PointerMoved;
            _repeater.PointerReleased -= Repeater_PointerReleased;
            _repeater.PointerCaptureLost -= Repeater_PointerCaptureLost;
            _repeater.PointerExited -= Repeater_PointerExited;
            _repeater.ContextRequested -= Repeater_ContextRequested;
        }
        if (_unifiedSwitcher is not null) _unifiedSwitcher.SelectionChanged -= UnifiedSwitcher_SelectionChanged;

        base.OnApplyTemplate();

        _headerBorder = GetTemplateChild("PART_HeaderBorder") as Border;
        _filenameText = GetTemplateChild("PART_FilenameText") as TextBlock;
        _addedText = GetTemplateChild("PART_AddedText") as TextBlock;
        _removedText = GetTemplateChild("PART_RemovedText") as TextBlock;
        _approxText = GetTemplateChild("PART_ApproxText") as TextBlock;
        _unifiedSwitcher = GetTemplateChild("PART_UnifiedSwitcher") as SelectorBar;
        _inlineItem = GetTemplateChild("PART_InlineItem") as SelectorBarItem;
        _beforeItem = GetTemplateChild("PART_BeforeItem") as SelectorBarItem;
        _afterItem = GetTemplateChild("PART_AfterItem") as SelectorBarItem;
        _busyBar = GetTemplateChild("PART_BusyBar") as ProgressBar;
        _scroller = GetTemplateChild("PART_Scroller") as ScrollViewer;
        _repeater = GetTemplateChild("PART_Repeater") as ItemsRepeater;

        if (_scroller is not null) _scroller.SizeChanged += Scroller_SizeChanged;
        if (_repeater is not null)
        {
            _repeater.ElementPrepared += Repeater_ElementPrepared;
            _repeater.PointerPressed += Repeater_PointerPressed;
            _repeater.PointerMoved += Repeater_PointerMoved;
            _repeater.PointerReleased += Repeater_PointerReleased;
            _repeater.PointerCaptureLost += Repeater_PointerCaptureLost;
            _repeater.PointerExited += Repeater_PointerExited;
            _repeater.ContextRequested += Repeater_ContextRequested;
        }

        SyncModeButtons();
        if (_unifiedSwitcher is not null) _unifiedSwitcher.SelectionChanged += UnifiedSwitcher_SelectionChanged;

        UpdateChrome();
        RebuildRows();
    }

    public void Replay()
    {
        BeginEntrance();
        RebuildRows();
    }

    private async Task RefreshAsync()
    {
        _cts?.Cancel();
        var cts = _cts = new CancellationTokenSource();
        var ct = cts.Token;

        string? oldText = OldText;
        string? newText = NewText;
        int maxD = MaxEditDistance;
        _refreshPending = true;

        if (oldText is null && newText is null)
        {
            _refreshPending = false;
            SetSource(new List<DiffLine>(), false);
            if (_busyBar is not null) _busyBar.Visibility = Visibility.Collapsed;
            UpdateChrome();
            RebuildRows();
            return;
        }

        if (_busyBar is not null) _busyBar.Visibility = Visibility.Visible;
        try
        {
            var result = await Task.Run(() => DiffEngine.Compute(
                DiffEngine.SplitLines(oldText ?? ""),
                DiffEngine.SplitLines(newText ?? ""),
                maxD,
                ct), ct);

            if (ct.IsCancellationRequested) return;

            _refreshPending = false;
            SetSource(result.Lines, result.Approximate);
            BeginEntrance();
            UpdateChrome();
            RebuildRows();
            DiffCompleted?.Invoke(this, EventArgs.Empty);
        }
        catch (OperationCanceledException) { }
        finally
        {
            if (ReferenceEquals(_cts, cts) && _busyBar is not null) _busyBar.Visibility = Visibility.Collapsed;
        }
    }

    private void ScheduleRefresh()
    {
        if (_refreshQueued) return;
        _refreshQueued = true;

        DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, async () =>
        {
            _refreshQueued = false;
            await RefreshAsync();
        });
    }

    private void SetSource(List<DiffLine> lines, bool approximate)
    {
        _source = lines;
        _pairs = null;
        _approximate = approximate;
        _expanded.Clear();

        int added = 0, removed = 0;
        foreach (var l in lines)
        {
            if (l.Kind == DiffKind.Add) added++;
            else if (l.Kind == DiffKind.Remove) removed++;
        }
        AddedCount = added;
        RemovedCount = removed;
    }

    private void HandleViewModeChanged()
    {
        SyncModeButtons();
        ApplyModeChange();
    }

    private void HandleUnifiedModeChanged()
    {
        if (!IsAvailable(UnifiedMode) && FirstAvailableUnified() is DiffUnifiedMode fallback)
        {
            UnifiedMode = fallback;
            return;
        }

        SyncModeButtons();
        if (ViewMode == DiffViewMode.Unified) ApplyModeChange();
    }

    private void HandleAvailableUnifiedModesChanged()
    {
        if (!IsAvailable(UnifiedMode) && FirstAvailableUnified() is DiffUnifiedMode fallback)
        {
            UnifiedMode = fallback;
            return;
        }
        SyncModeButtons();
    }

    private void ApplyModeChange()
    {
        _expanded.Clear();

        if (IsAnimated && IsLoaded && _scroller is not null) SwitchContentAnimated();
        else RebuildRows();
    }

    private bool IsAvailable(DiffUnifiedMode mode) => (AvailableUnifiedModes & ToFlag(mode)) != 0;

    private static DiffUnifiedModes ToFlag(DiffUnifiedMode mode) => mode switch
    {
        DiffUnifiedMode.Before => DiffUnifiedModes.Before,
        DiffUnifiedMode.After => DiffUnifiedModes.After,
        _ => DiffUnifiedModes.Inline,
    };

    private DiffUnifiedMode? FirstAvailableUnified()
    {
        if ((AvailableUnifiedModes & DiffUnifiedModes.Inline) != 0) return DiffUnifiedMode.Inline;
        if ((AvailableUnifiedModes & DiffUnifiedModes.Before) != 0) return DiffUnifiedMode.Before;
        if ((AvailableUnifiedModes & DiffUnifiedModes.After) != 0) return DiffUnifiedMode.After;
        return null;
    }

    private void SyncModeButtons()
    {
        _syncingMode = true;
        try
        {
            if (_unifiedSwitcher is not null)
            {
                if (_inlineItem is not null) _inlineItem.Visibility = Show((AvailableUnifiedModes & DiffUnifiedModes.Inline) != 0);
                if (_beforeItem is not null) _beforeItem.Visibility = Show((AvailableUnifiedModes & DiffUnifiedModes.Before) != 0);
                if (_afterItem is not null) _afterItem.Visibility = Show((AvailableUnifiedModes & DiffUnifiedModes.After) != 0);

                _unifiedSwitcher.SelectedItem = UnifiedMode switch
                {
                    DiffUnifiedMode.Before => _beforeItem,
                    DiffUnifiedMode.After => _afterItem,
                    _ => _inlineItem,
                };

                _unifiedSwitcher.Visibility = Show(
                    ShowModeSwitcher &&
                    ViewMode == DiffViewMode.Unified &&
                    BitOperations.PopCount((uint)AvailableUnifiedModes) > 1);
            }
        }
        finally
        {
            _syncingMode = false;
        }
    }

    private static Visibility Show(bool visible) => visible ? Visibility.Visible : Visibility.Collapsed;

    private void UnifiedSwitcher_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        if (_syncingMode) return;

        SelectorBarItem? selected = sender.SelectedItem;
        if (ReferenceEquals(selected, _inlineItem)) UnifiedMode = DiffUnifiedMode.Inline;
        else if (ReferenceEquals(selected, _beforeItem)) UnifiedMode = DiffUnifiedMode.Before;
        else if (ReferenceEquals(selected, _afterItem)) UnifiedMode = DiffUnifiedMode.After;
    }

    private void ApplyWrap()
    {
        if (_scroller is null) return;

        bool fit = FitsWidth;
        _scroller.HorizontalScrollBarVisibility = fit ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto;
        _scroller.HorizontalScrollMode = fit ? ScrollMode.Disabled : ScrollMode.Auto;

        if (_repeater is not null) _repeater.MinWidth = fit ? 0 : _scroller.ActualWidth;
    }

    private void UpdateChrome()
    {
        if (_headerBorder is not null)
            _headerBorder.Visibility = ShowHeader ? Visibility.Visible : Visibility.Collapsed;
        if (_filenameText is not null) _filenameText.Text = Filename;
        if (_scroller is not null) _scroller.MaxHeight = MaxBodyHeight;
        if (_addedText is not null) _addedText.Text = $"+{AddedCount:N0}";
        if (_removedText is not null) _removedText.Text = $"−{RemovedCount:N0}";
        if (_approxText is not null)
            _approxText.Visibility = _approximate ? Visibility.Visible : Visibility.Collapsed;

        AutomationProperties.SetName(this, FileAutomationFormat is { } nameFormat ? string.Format(nameFormat, Filename) : Filename);
        AutomationProperties.SetHelpText(this, SummaryAutomationFormat is { } helpFormat ? string.Format(helpFormat, AddedCount, RemovedCount) : string.Empty);
    }

    private List<DiffLine> SourceForMode()
    {
        if (ViewMode == DiffViewMode.Split) return _source;

        return UnifiedMode switch
        {
            DiffUnifiedMode.Before => _source.Where(l => l.Kind != DiffKind.Add).ToList(),
            DiffUnifiedMode.After => _source.Where(l => l.Kind != DiffKind.Remove).ToList(),
            _ => _source,
        };
    }

    private void RebuildRows()
    {
        if (_repeater is null) return;

        ApplyWrap();

        bool split = ViewMode == DiffViewMode.Split;
        List<DiffLine> src = SourceForMode();
        List<SplitLine>? pairs = split ? (_pairs ??= BuildPairs(src)) : null;
        int n = pairs?.Count ?? src.Count;

        bool IsChangeAt(int i) => pairs is not null ? pairs[i].IsChange : src[i].Kind != DiffKind.Context;

        int maxOld = 0, maxNew = 0;
        foreach (DiffLine l in src)
        {
            if (l.OldNumber is int o && o > maxOld) maxOld = o;
            if (l.NewNumber is int nn && nn > maxNew) maxNew = nn;
        }

        bool numbers = ShowLineNumbers;
        bool beforeOnly = !split && UnifiedMode == DiffUnifiedMode.Before;
        bool afterOnly = !split && UnifiedMode == DiffUnifiedMode.After;
        bool showOld = numbers && !afterOnly && maxOld > 0;
        bool showNew = numbers && !beforeOnly && maxNew > 0;
        double oldW = showOld ? Digits(maxOld) * 8 + 14 : 0;
        double newW = showNew ? Digits(maxNew) * 8 + 14 : 0;

        bool collapse = CollapseUnchanged && ContextLines >= 0;
        bool[]? keep = null;
        if (collapse)
        {
            keep = new bool[n];
            int c = ContextLines;
            for (int i = 0; i < n; i++)
            {
                if (!IsChangeAt(i)) continue;
                int lo = Math.Max(0, i - c), hi = Math.Min(n - 1, i + c);
                for (int j = lo; j <= hi; j++) keep[j] = true;
            }
        }

        var refs = new List<RowRef>(keep is null ? n : 64);
        int idx = 0;
        while (idx < n)
        {
            if (keep is null || keep[idx])
            {
                refs.Add(new RowRef(idx, 0));
                idx++;
                continue;
            }

            int end = idx;
            while (end < n && !keep[end]) end++;
            int hidden = end - idx;

            if (hidden < 2 || _expanded.Contains(idx))
            {
                for (int j = idx; j < end; j++) refs.Add(new RowRef(j, 0));
            }
            else
            {
                refs.Add(new RowRef(idx, hidden));
            }
            idx = end;
        }

        RowRef[] refArray = refs.ToArray();

        if (_entrancePending)
        {
            _entrancePending = false;
            _entranceCounter = 0;
            _entranceUntil = Environment.TickCount64 + EntranceWindowMs;
        }

        Brush? addedBackground = AddedBackground;
        Brush? removedBackground = RemovedBackground;
        Brush? addedAccent = AddedAccent;
        Brush? removedAccent = RemovedAccent;
        Brush? selectionBackground = SelectionBackground;
        Brush? selectionForeground = SelectionForeground;
        bool nativeSelection = UseSingleLineSelection;
        Brush? selectionOnAdded = Over(selectionBackground, addedBackground);
        Brush? selectionOnRemoved = Over(selectionBackground, removedBackground);
        Brush? selectionOnContext = Over(selectionBackground, Background is SolidColorBrush { Color.A: 255 } ? Background : SelectionBase);
        TextWrapping wrapping = WrapLines ? TextWrapping.Wrap : TextWrapping.NoWrap;
        ICommand expandCommand = _expandCommand;
        string expanderFormat = ExpanderFormat ?? "{0}";
        string expandAutomationFormat = ExpandAutomationFormat ?? "{0}";
        string addedLineFormat = AddedLineFormat ?? "{0}";
        string removedLineFormat = RemovedLineFormat ?? "{0}";

        Brush? placeholderBackground = PlaceholderBackground;
        Brush? addedHighlight = AddedHighlight;
        Brush? removedHighlight = RemovedHighlight;
        Brush? highlightForeground = HighlightForeground;
        bool highlight = HighlightChanges;
        List<DiffLine> full = _source;

        DiffRow MakeRow(DiffLine l, int rowIndex, int side)
        {
            bool paired = highlight && l.Kind != DiffKind.Context && l.Pair >= 0;
            (int selStart, int selLength) = SelectedRange(rowIndex, side);
            var row = new DiffRow(l.Kind, l.Text, l.OldNumber ?? 0, l.NewNumber ?? 0)
            {
                OldGutterWidth = oldW,
                NewGutterWidth = newW,
                Wrapping = wrapping,
                Highlights = paired ? IntraLineDiff.RangesFor(l, full[l.Pair]) : null,
                HighlightBackground = l.Kind == DiffKind.Add ? addedHighlight : removedHighlight,
                HighlightForeground = highlightForeground,
                SelectionStart = selStart,
                SelectionLength = selLength,
                SelectionBackground = l.Kind switch
                {
                    DiffKind.Add => selectionOnAdded,
                    DiffKind.Remove => selectionOnRemoved,
                    _ => selectionOnContext,
                },
                SelectionForeground = selectionForeground,
                UseNativeSelection = nativeSelection,
                AddedFormat = addedLineFormat,
                RemovedFormat = removedLineFormat,
            };

            switch (l.Kind)
            {
                case DiffKind.Add:
                    row.Background = addedBackground;
                    row.AccentBrush = addedAccent;
                    break;
                case DiffKind.Remove:
                    row.Background = removedBackground;
                    row.AccentBrush = removedAccent;
                    break;
            }
            return row;
        }

        DiffRow MakeBlank() => new(DiffKind.Context, "", 0, 0)
        {
            Background = placeholderBackground,
            OldGutterWidth = oldW,
            NewGutterWidth = newW,
            Wrapping = wrapping,
        };

        DiffRow MakeExpander(RowRef r) => new(r.Index, r.Hidden)
        {
            ExpandCommand = expandCommand,
            ExpanderFormat = expanderFormat,
            ExpandAutomationFormat = expandAutomationFormat,
        };

        object Create(int index, RowRef r)
        {
            if (pairs is null)
            {
                return r.Hidden > 0
                    ? MakeExpander(r)
                    : MakeRow(src[r.Index], index, 0);
            }

            if (r.Hidden > 0) return new SplitRow(MakeExpander(r));

            SplitLine p = pairs[r.Index];
            return new SplitRow(
                p.Left >= 0 ? MakeRow(src[p.Left], index, 0) : MakeBlank(),
                p.Right >= 0 ? MakeRow(src[p.Right], index, 1) : MakeBlank());
        }

        string? TextAt(int row, int side)
        {
            RowRef r = refArray[row];
            if (r.Hidden > 0) return null;

            if (pairs is null) return DiffRow.ToDisplay(src[r.Index].Text);

            SplitLine p = pairs[r.Index];
            int line = side == 0 ? p.Left : p.Right;
            return line >= 0 ? DiffRow.ToDisplay(src[line].Text) : null;
        }

        _rowRefs = refArray;
        _textAt = TextAt;
        _selActive = false;
        _selecting = false;
        _scrollTimer?.Stop();

        var rows = new DiffRowList(refArray, IsChangeAt, Create);
        _rows = rows;

        DataTemplate? template = split ? SplitRowTemplate : UnifiedRowTemplate;
        if (template is not null && !ReferenceEquals(_repeater.ItemTemplate, template))
        {
            _repeater.ItemsSource = null;
            _repeater.ItemTemplate = template;
        }

        _repeater.ItemsSource = rows;
    }

    private static List<SplitLine> BuildPairs(List<DiffLine> source)
    {
        var pairs = new List<SplitLine>(source.Count);
        int i = 0;
        while (i < source.Count)
        {
            if (source[i].Kind == DiffKind.Context)
            {
                pairs.Add(new SplitLine(i, i));
                i++;
                continue;
            }

            int removeStart = i;
            while (i < source.Count && source[i].Kind == DiffKind.Remove) i++;
            int addStart = i;
            while (i < source.Count && source[i].Kind == DiffKind.Add) i++;

            int removeCount = addStart - removeStart;
            int addCount = i - addStart;
            int rows = Math.Max(removeCount, addCount);
            for (int k = 0; k < rows; k++)
            {
                pairs.Add(new SplitLine(
                    k < removeCount ? removeStart + k : -1,
                    k < addCount ? addStart + k : -1));
            }
        }
        return pairs;
    }

    private static int Digits(int value) => value.ToString().Length;

    private void BeginEntrance()
    {
        _entrancePending = IsAnimated;
    }

    private void SwitchContentAnimated()
    {
        if (_scroller is null) return;

        int version = ++_switchVersion;
        var visual = ElementCompositionPreview.GetElementVisual(_scroller);
        var compositor = visual.Compositor;

        var fadeOut = compositor.CreateScalarKeyFrameAnimation();
        fadeOut.InsertKeyFrame(1f, 0f);
        fadeOut.Duration = TimeSpan.FromMilliseconds(FadeOutMs);

        var batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
        visual.StartAnimation("Opacity", fadeOut);
        batch.End();
        batch.Completed += (_, _) =>
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (version != _switchVersion || _scroller is null) return;

                BeginEntrance();
                RebuildRows();
                _scroller.ChangeView(null, 0, null, true);

                var fadeIn = compositor.CreateScalarKeyFrameAnimation();
                fadeIn.InsertKeyFrame(1f, 1f, GetEasing(compositor));
                fadeIn.Duration = TimeSpan.FromMilliseconds(FadeInMs);
                visual.StartAnimation("Opacity", fadeIn);
            });
        };
    }

    private CompositionEasingFunction GetEasing(Compositor compositor)
    {
        return _easing ??= compositor.CreateCubicBezierEasingFunction(new Vector2(0.2f, 0f), new Vector2(0f, 1f));
    }

    private void Repeater_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        var element = args.Element;
        var visual = ElementCompositionPreview.GetElementVisual(element);
        ElementCompositionPreview.SetIsTranslationEnabled(element, true);
        visual.StopAnimation("Opacity");
        visual.StopAnimation("Translation.X");
        visual.Opacity = 1f;
        visual.Properties.InsertVector3("Translation", Vector3.Zero);

        if (Environment.TickCount64 >= _entranceUntil) return;
        if (_rows is null || args.Index < 0 || args.Index >= _rows.Count || !_rows.IsChange(args.Index)) return;

        var compositor = visual.Compositor;
        var easing = GetEasing(compositor);
        var delay = TimeSpan.FromMilliseconds(Math.Min(_entranceCounter++ * 22, 420));
        var duration = TimeSpan.FromMilliseconds(320);

        var fade = compositor.CreateScalarKeyFrameAnimation();
        fade.InsertKeyFrame(0f, 0f);
        fade.InsertKeyFrame(1f, 1f, easing);
        fade.Duration = duration;
        fade.DelayTime = delay;
        fade.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;

        var slide = compositor.CreateScalarKeyFrameAnimation();
        slide.InsertKeyFrame(0f, -18f);
        slide.InsertKeyFrame(1f, 0f, easing);
        slide.Duration = duration;
        slide.DelayTime = delay;
        slide.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;

        visual.StartAnimation("Opacity", fade);
        visual.StartAnimation("Translation.X", slide);
    }

    private void ExpandRegion(object? parameter)
    {
        if (parameter is DiffRow row)
        {
            _expanded.Add(row.RegionStart);
            RebuildRows();
        }
    }

    private void FileDiff_Loaded(object sender, RoutedEventArgs e)
    {
        if (!_resumeOnLoad) return;

        _resumeOnLoad = false;
        ScheduleRefresh();
    }

    private void FileDiff_ActualThemeChanged(FrameworkElement sender, object args)
    {
        RebuildRows();
    }

    private void FileDiff_Unloaded(object sender, RoutedEventArgs e)
    {
        _cts?.Cancel();
        _resumeOnLoad = _refreshPending;
        _selecting = false;
        _scrollTimer?.Stop();
        if (_scroller is not null) ElementCompositionPreview.GetElementVisual(_scroller).Opacity = 1f;
    }

    private void Scroller_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_repeater is not null) _repeater.MinWidth = FitsWidth ? 0 : e.NewSize.Width;
    }

    public bool HasSelection => _selActive && (_anchorRow != _caretRow || _anchorCol != _caretCol);

    public string SelectedText
    {
        get
        {
            if (!HasSelection || _textAt is null) return string.Empty;

            OrderSelection(out int r1, out int c1, out int r2, out int c2);

            var sb = new StringBuilder();
            bool first = true;
            for (int row = r1; row <= r2; row++)
            {
                string? text = _textAt(row, _selSide);
                if (text is null) continue;

                int start = row == r1 ? Math.Min(c1, text.Length) : 0;
                int end = row == r2 ? Math.Min(c2, text.Length) : text.Length;

                if (!first) sb.Append(Environment.NewLine);
                first = false;
                if (end > start) sb.Append(text, start, end - start);
            }
            return sb.ToString();
        }
    }

    public void SelectAll()
    {
        if (UseSingleLineSelection || _rowRefs.Length == 0 || _textAt is null) return;

        int side = _selActive ? _selSide : 0;
        _selActive = true;
        _selSide = side;
        _anchorRow = 0;
        _anchorCol = 0;
        _caretRow = _rowRefs.Length - 1;
        _caretCol = _textAt(_caretRow, side)?.Length ?? 0;
        RefreshSelectionVisuals();
    }

    public void ClearSelection()
    {
        if (!_selActive) return;

        _selActive = false;
        _selecting = false;
        RefreshSelectionVisuals();
    }

    public void CopySelection()
    {
        string text = SelectedText;
        if (text.Length == 0) return;

        try
        {
            var package = new DataPackage();
            package.SetText(text);
            Clipboard.SetContent(package);
        }
        catch (COMException)
        {
        }
    }

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Handled || UseSingleLineSelection) return;

        bool ctrl = IsKeyDown(VirtualKey.Control);
        if (IsKeyDown(VirtualKey.Shift) && TryExtendSelection(e.Key, ctrl || IsKeyDown(VirtualKey.Menu)))
        {
            e.Handled = true;
            return;
        }

        if (!ctrl) return;

        if (e.Key == VirtualKey.C && HasSelection)
        {
            CopySelection();
            e.Handled = true;
        }
        else if (e.Key == VirtualKey.A)
        {
            SelectAll();
            e.Handled = true;
        }
    }

    private static bool IsKeyDown(VirtualKey key)
    {
        return InputKeyboardSource.GetKeyStateForCurrentThread(key).HasFlag(CoreVirtualKeyStates.Down);
    }

    private void FileDiff_NativeKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (!UseSingleLineSelection || !IsKeyDown(VirtualKey.Shift)) return;

        bool jump = IsKeyDown(VirtualKey.Control) || IsKeyDown(VirtualKey.Menu);
        if (TryExtendNativeSelection(e.Key, jump)) e.Handled = true;
    }

    private bool TryExtendNativeSelection(VirtualKey key, bool jump)
    {
        if (key is not (VirtualKey.Left or VirtualKey.Right or VirtualKey.Up or VirtualKey.Down or VirtualKey.Home or VirtualKey.End)) return false;
        if (XamlRoot is null || FocusManager.GetFocusedElement(XamlRoot) is not TextBlock { IsTextSelectionEnabled: true } block) return false;
        if (DiffHighlight.GetRow(block) is not { IsExpander: false }) return false;
        if (block.Inlines.Count == 0 || block.Inlines[0] is not Run run) return false;
        if (block.SelectionStart is not TextPointer selStart || block.SelectionEnd is not TextPointer selEnd) return false;

        string text = run.Text;
        int origin = run.ContentStart.Offset;
        int start = Math.Clamp(selStart.Offset - origin, 0, text.Length);
        int end = Math.Clamp(selEnd.Offset - origin, 0, text.Length);
        if (start > end) (start, end) = (end, start);

        if (!ReferenceEquals(block, _nativeBlock) || _nativeRange != (start, end)) _nativeCaretAtStart = false;

        int anchor = _nativeCaretAtStart ? end : start;
        int caret = _nativeCaretAtStart ? start : end;

        caret = key switch
        {
            VirtualKey.Left => jump ? WordStart(text, caret) : Math.Max(0, caret - 1),
            VirtualKey.Right => jump ? WordEnd(text, caret) : Math.Min(text.Length, caret + 1),
            VirtualKey.Up or VirtualKey.Home => 0,
            _ => text.Length,
        };

        if (caret != anchor) _nativeCaretAtStart = caret < anchor;

        int low = Math.Min(anchor, caret), high = Math.Max(anchor, caret);
        TextPointer? from = run.ContentStart.GetPositionAtOffset(low, LogicalDirection.Forward);
        TextPointer? to = run.ContentStart.GetPositionAtOffset(high, LogicalDirection.Forward);
        if (from is null || to is null) return false;

        block.Select(from, to);
        _nativeBlock = block;
        _nativeRange = (low, high);
        return true;
    }

    private bool TryExtendSelection(VirtualKey key, bool jump)
    {
        if (!_selActive || _textAt is null || FocusState == FocusState.Unfocused) return false;
        if (key is not (VirtualKey.Left or VirtualKey.Right or VirtualKey.Up or VirtualKey.Down or VirtualKey.Home or VirtualKey.End)) return false;

        int row = _caretRow, col = _caretCol;
        string text = _textAt(row, _selSide) ?? string.Empty;
        bool vertical = key is VirtualKey.Up or VirtualKey.Down;

        switch (key)
        {
            case VirtualKey.Left:
                if (col == 0)
                {
                    int previous = NextTextRow(row, -1);
                    if (previous >= 0) (row, col) = (previous, TextLength(previous));
                }
                else
                {
                    col = jump ? WordStart(text, col) : col - 1;
                }
                break;

            case VirtualKey.Right:
                if (col >= text.Length)
                {
                    int next = NextTextRow(row, 1);
                    if (next >= 0) (row, col) = (next, 0);
                }
                else
                {
                    col = jump ? WordEnd(text, col) : col + 1;
                }
                break;

            case VirtualKey.Up:
            case VirtualKey.Down:
                int target = NextTextRow(row, key == VirtualKey.Up ? -1 : 1);
                if (_goalCol < 0) _goalCol = col;
                if (target < 0)
                {
                    col = key == VirtualKey.Up ? 0 : text.Length;
                }
                else
                {
                    row = target;
                    col = Math.Min(_goalCol, TextLength(target));
                }
                break;

            case VirtualKey.Home:
                if (jump && NextTextRow(-1, 1) is int first and >= 0) row = first;
                col = 0;
                break;

            case VirtualKey.End:
                if (jump && NextTextRow(_rowRefs.Length, -1) is int last and >= 0) row = last;
                col = TextLength(row);
                break;
        }

        if (!vertical) _goalCol = -1;
        if (row == _caretRow && col == _caretCol) return true;

        _caretRow = row;
        _caretCol = col;
        RefreshSelectionVisuals();
        EnsureCaretVisible();
        return true;
    }

    private int NextTextRow(int from, int step)
    {
        for (int r = from + step; r >= 0 && r < _rowRefs.Length; r += step)
        {
            if (_textAt!(r, _selSide) is not null) return r;
        }
        return -1;
    }

    private int TextLength(int row)
    {
        return _textAt?.Invoke(row, _selSide)?.Length ?? 0;
    }

    private static int WordStart(string text, int col)
    {
        int i = Math.Min(col, text.Length);
        while (i > 0 && CharKind(text[i - 1]) == 1) i--;
        if (i > 0)
        {
            int kind = CharKind(text[i - 1]);
            while (i > 0 && CharKind(text[i - 1]) == kind) i--;
        }
        return i;
    }

    private static int WordEnd(string text, int col)
    {
        int i = col;
        if (i < text.Length)
        {
            int kind = CharKind(text[i]);
            while (i < text.Length && CharKind(text[i]) == kind) i++;
        }
        while (i < text.Length && CharKind(text[i]) == 1) i++;
        return i;
    }

    private void EnsureCaretVisible()
    {
        if (_repeater is null || _scroller is null) return;
        if (_repeater.GetOrCreateElement(_caretRow) is not FrameworkElement element) return;

        element.UpdateLayout();

        double x = _scroller.HorizontalOffset;
        string text = _textAt?.Invoke(_caretRow, _selSide) ?? string.Empty;
        List<TextBlock> blocks = CodeBlocks(element);

        if (!FitsWidth && text.Length > 0 && _selSide < blocks.Count &&
            blocks[_selSide].Inlines.Count > 0 && blocks[_selSide].Inlines[0] is Run run)
        {
            int index = Math.Min(_caretCol, text.Length - 1);
            TextPointer? pointer = run.ContentStart.GetPositionAtOffset(index, LogicalDirection.Forward);
            if (pointer is not null)
            {
                Rect rect = pointer.GetCharacterRect(LogicalDirection.Forward);
                double left = rect.Left + (_caretCol > index ? rect.Width : 0);
                x = Math.Max(0, blocks[_selSide].TransformToVisual(element).TransformPoint(new Point(left, 0)).X - 24);
            }
        }

        element.StartBringIntoView(new BringIntoViewOptions
        {
            AnimationDesired = false,
            TargetRect = new Rect(x, 0, 48, element.ActualHeight),
        });
    }

    private void OrderSelection(out int r1, out int c1, out int r2, out int c2)
    {
        bool forward = _anchorRow < _caretRow || (_anchorRow == _caretRow && _anchorCol <= _caretCol);
        r1 = forward ? _anchorRow : _caretRow;
        c1 = forward ? _anchorCol : _caretCol;
        r2 = forward ? _caretRow : _anchorRow;
        c2 = forward ? _caretCol : _anchorCol;
    }

    private (int Start, int Length) SelectedRange(int row, int side)
    {
        if (!HasSelection || side != _selSide || _textAt is null) return default;

        OrderSelection(out int r1, out int c1, out int r2, out int c2);
        if (row < r1 || row > r2) return default;

        string? text = _textAt(row, side);
        if (text is null) return default;

        int start = row == r1 ? Math.Min(c1, text.Length) : 0;
        int end = row == r2 ? Math.Min(c2, text.Length) : text.Length;
        return end > start ? (start, end - start) : default;
    }

    private void RefreshSelectionVisuals()
    {
        if (_repeater is null) return;

        int count = VisualTreeHelper.GetChildrenCount(_repeater);
        for (int i = 0; i < count; i++)
        {
            if (VisualTreeHelper.GetChild(_repeater, i) is not FrameworkElement element) continue;

            int row = _repeater.GetElementIndex(element);
            if (row < 0 || row >= _rowRefs.Length) continue;

            List<TextBlock> blocks = CodeBlocks(element);
            for (int side = 0; side < blocks.Count; side++)
            {
                (int start, int length) = SelectedRange(row, side);
                DiffRow? diffRow = DiffHighlight.GetRow(blocks[side]);
                DiffHighlight.SetSelection(blocks[side], start, length, diffRow?.SelectionBackground, diffRow?.SelectionForeground);
            }
        }
    }

    private static Brush? Over(Brush? top, Brush? bottom)
    {
        if (top is not SolidColorBrush t || bottom is not SolidColorBrush b) return top;

        double a = t.Color.A / 255.0 * t.Opacity;
        byte Mix(byte x, byte y) => (byte)Math.Round(x * a + y * (1 - a));

        return new SolidColorBrush(global::Windows.UI.Color.FromArgb(0xFF, Mix(t.Color.R, b.Color.R), Mix(t.Color.G, b.Color.G), Mix(t.Color.B, b.Color.B)));
    }

    private static List<TextBlock> CodeBlocks(DependencyObject root)
    {
        var blocks = new List<TextBlock>(2);
        CollectCodeBlocks(root, blocks);
        return blocks;
    }

    private static void CollectCodeBlocks(DependencyObject parent, List<TextBlock> blocks)
    {
        int count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, i);
            if (child is TextBlock textBlock)
            {
                if (DiffHighlight.GetRow(textBlock) is { IsExpander: false }) blocks.Add(textBlock);
                continue;
            }
            CollectCodeBlocks(child, blocks);
        }
    }

    private bool TryFindRow(double y, out int row, [NotNullWhen(true)] out FrameworkElement? element)
    {
        row = -1;
        element = null;
        if (_repeater is null) return false;

        double best = double.MaxValue;
        int count = VisualTreeHelper.GetChildrenCount(_repeater);
        for (int i = 0; i < count; i++)
        {
            if (VisualTreeHelper.GetChild(_repeater, i) is not FrameworkElement child) continue;

            int index = _repeater.GetElementIndex(child);
            if (index < 0 || index >= _rowRefs.Length) continue;

            double top = child.ActualOffset.Y;
            double bottom = top + child.ActualHeight;
            double distance = y < top ? top - y : y > bottom ? y - bottom : 0;
            if (distance >= best) continue;

            best = distance;
            row = index;
            element = child;
            if (distance == 0) break;
        }
        return element is not null;
    }

    private bool TryHit(Point point, int fixedSide, out int row, out int side, out int col)
    {
        row = 0;
        side = 0;
        col = 0;
        if (_scroller is null || _repeater is null || _textAt is null) return false;

        var clamped = new Point(point.X, Math.Clamp(point.Y, 0, Math.Max(0, _scroller.ViewportHeight - 1)));
        double repeaterY = _scroller.TransformToVisual(_repeater).TransformPoint(clamped).Y;
        if (!TryFindRow(repeaterY, out row, out FrameworkElement? element)) return false;

        List<TextBlock> blocks = CodeBlocks(element);
        side = fixedSide;
        if (side < 0)
        {
            side = 0;
            if (blocks.Count > 1)
            {
                double edge = blocks[0].TransformToVisual(element).TransformPoint(new Point(blocks[0].ActualWidth, 0)).X;
                double x = _scroller.TransformToVisual(element).TransformPoint(clamped).X;
                if (x > edge) side = 1;
            }
        }

        if (side >= blocks.Count) return true;

        TextBlock block = blocks[side];
        string text = _textAt(row, side) ?? string.Empty;
        col = HitChar(block, _scroller.TransformToVisual(block).TransformPoint(clamped), text.Length);
        return true;
    }

    private static int HitChar(TextBlock block, Point p, int length)
    {
        if (length == 0 || block.Inlines.Count == 0 || block.Inlines[0] is not Run run) return 0;

        TextPointer start = run.ContentStart;

        Rect RectAt(int index)
        {
            TextPointer? pointer = start.GetPositionAtOffset(index, LogicalDirection.Forward);
            return pointer is null
                ? new Rect(double.PositiveInfinity, double.PositiveInfinity, 0, 0)
                : pointer.GetCharacterRect(LogicalDirection.Forward);
        }

        bool IsBefore(int index)
        {
            Rect r = RectAt(index);
            return r.Bottom <= p.Y || (r.Top <= p.Y && r.Left <= p.X);
        }

        if (!IsBefore(0)) return 0;

        int lo = 0, hi = length - 1;
        while (lo < hi)
        {
            int mid = (lo + hi + 1) / 2;
            if (IsBefore(mid)) lo = mid;
            else hi = mid - 1;
        }

        Rect rect = RectAt(lo);
        bool after = rect.Bottom <= p.Y || p.X > rect.Left + rect.Width / 2;
        return after ? lo + 1 : lo;
    }

    private static (int Start, int End) WordAt(string text, int col)
    {
        if (text.Length == 0) return (0, 0);

        int i = Math.Clamp(col, 0, text.Length - 1);
        int kind = CharKind(text[i]);
        int start = i, end = i + 1;
        while (start > 0 && CharKind(text[start - 1]) == kind) start--;
        while (end < text.Length && CharKind(text[end]) == kind) end++;
        return (start, end);
    }

    private static int CharKind(char c)
    {
        if (char.IsLetterOrDigit(c) || c == '_') return 0;
        return char.IsWhiteSpace(c) ? 1 : 2;
    }

    private void Repeater_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (UseSingleLineSelection || _scroller is null || _repeater is null) return;
        if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch) return;

        PointerPoint point = e.GetCurrentPoint(_scroller);
        if (!point.Properties.IsLeftButtonPressed) return;
        if (!TryHit(point.Position, -1, out int row, out int side, out int col)) return;
        if (_rowRefs[row].Hidden > 0) return;

        long now = Environment.TickCount64;
        bool repeated = now - _lastPressTick <= 500 &&
                        Math.Abs(point.Position.X - _lastPressPoint.X) < 4 &&
                        Math.Abs(point.Position.Y - _lastPressPoint.Y) < 4;
        _clickCount = repeated ? _clickCount % 3 + 1 : 1;
        _lastPressTick = now;
        _lastPressPoint = point.Position;

        Focus(FocusState.Pointer);

        string text = _textAt?.Invoke(row, side) ?? string.Empty;
        _selActive = true;
        _selecting = true;
        _selSide = side;
        _anchorRow = _caretRow = row;
        _goalCol = -1;

        if (_clickCount == 2 && text.Length > 0)
        {
            (_anchorCol, _caretCol) = WordAt(text, col);
        }
        else if (_clickCount == 3)
        {
            _anchorCol = 0;
            _caretCol = text.Length;
        }
        else
        {
            _anchorCol = _caretCol = col;
        }

        _dragPoint = point.Position;
        _repeater.CapturePointer(e.Pointer);
        e.Handled = true;
        RefreshSelectionVisuals();
    }

    private void Repeater_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!_selecting)
        {
            SetTextCursor(!UseSingleLineSelection && e.OriginalSource is TextBlock block && DiffHighlight.GetRow(block) is { IsExpander: false });
            return;
        }

        if (_scroller is null) return;

        PointerPoint point = e.GetCurrentPoint(_scroller);
        if (!point.Properties.IsLeftButtonPressed)
        {
            EndSelecting(e.Pointer);
            return;
        }

        _dragPoint = point.Position;
        UpdateCaret();
        UpdateAutoScroll();
    }

    private void Repeater_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (!_selecting) return;

        EndSelecting(e.Pointer);
        e.Handled = true;
    }

    private void Repeater_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
    {
        _selecting = false;
        _scrollTimer?.Stop();
    }

    private void Repeater_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (!_selecting) SetTextCursor(false);
    }

    private void EndSelecting(Pointer pointer)
    {
        _selecting = false;
        _scrollTimer?.Stop();
        _repeater?.ReleasePointerCapture(pointer);
    }

    private void UpdateCaret()
    {
        if (!TryHit(_dragPoint, _selSide, out int row, out _, out int col)) return;
        if (row == _caretRow && col == _caretCol) return;

        _caretRow = row;
        _caretCol = col;
        _goalCol = -1;
        RefreshSelectionVisuals();
    }

    private void UpdateAutoScroll()
    {
        if (_scroller is null) return;

        if (AutoScrollDelta(_dragPoint.X, _scroller.ViewportWidth) == 0 && AutoScrollDelta(_dragPoint.Y, _scroller.ViewportHeight) == 0)
        {
            _scrollTimer?.Stop();
            return;
        }

        if (_scrollTimer is null)
        {
            _scrollTimer = DispatcherQueue.CreateTimer();
            _scrollTimer.Interval = TimeSpan.FromMilliseconds(30);
            _scrollTimer.Tick += (_, _) => AutoScrollStep();
        }

        if (!_scrollTimer.IsRunning) _scrollTimer.Start();
    }

    private void AutoScrollStep()
    {
        if (!_selecting || _scroller is null)
        {
            _scrollTimer?.Stop();
            return;
        }

        double dx = FitsWidth ? 0 : AutoScrollDelta(_dragPoint.X, _scroller.ViewportWidth);
        double dy = AutoScrollDelta(_dragPoint.Y, _scroller.ViewportHeight);
        if (dx == 0 && dy == 0)
        {
            _scrollTimer?.Stop();
            return;
        }

        _scroller.ChangeView(_scroller.HorizontalOffset + dx, _scroller.VerticalOffset + dy, null, true);
        UpdateCaret();
    }

    private static double AutoScrollDelta(double position, double size)
    {
        double over = position < 0 ? position : position > size ? position - size : 0;
        if (over == 0) return 0;

        return Math.Sign(over) * Math.Clamp(Math.Abs(over) + 6, 6, 80);
    }

    private void SetTextCursor(bool on)
    {
        if (_textCursorOn == on) return;

        _textCursorOn = on;
        ProtectedCursor = on ? (_textCursor ??= InputSystemCursor.Create(InputSystemCursorShape.IBeam)) : null;
    }

    private void Repeater_ContextRequested(UIElement sender, ContextRequestedEventArgs args)
    {
        if (UseSingleLineSelection) return;

        _menu ??= CreateMenu();

        if (args.TryGetPosition(sender, out Point position)) _menu.ShowAt(sender, position);
        else _menu.ShowAt((FrameworkElement)sender);

        args.Handled = true;
    }

    private MenuFlyout CreateMenu()
    {
        var copy = new MenuFlyoutItem { Icon = new SymbolIcon(Symbol.Copy) };
        copy.KeyboardAccelerators.Add(new KeyboardAccelerator { Key = VirtualKey.C, Modifiers = global::Windows.System.VirtualKeyModifiers.Control });
        copy.Click += (_, _) => CopySelection();

        var selectAll = new MenuFlyoutItem { Icon = new SymbolIcon(Symbol.SelectAll) };
        selectAll.KeyboardAccelerators.Add(new KeyboardAccelerator { Key = VirtualKey.A, Modifiers = global::Windows.System.VirtualKeyModifiers.Control });
        selectAll.Click += (_, _) => SelectAll();

        var menu = new MenuFlyout();
        menu.Items.Add(copy);
        menu.Items.Add(selectAll);
        menu.Opening += (_, _) =>
        {
            copy.Text = CopyText ?? string.Empty;
            selectAll.Text = SelectAllText ?? string.Empty;
            copy.IsEnabled = HasSelection;
        };
        return menu;
    }

    private sealed class DiffCommand(Action<object?> execute) : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter) => parameter is DiffRow { IsExpander: true };

        public void Execute(object? parameter) => execute(parameter);
    }
}
