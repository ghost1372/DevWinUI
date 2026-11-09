namespace DevWinUI;

public sealed partial class DiffView
{
    public string Filename
    {
        get { return (string)GetValue(FilenameProperty); }
        set { SetValue(FilenameProperty, value); }
    }

    public static readonly DependencyProperty FilenameProperty =
        DependencyProperty.Register(nameof(Filename), typeof(string), typeof(DiffView), new PropertyMetadata(string.Empty, OnChromeChanged));

    public string? OldText
    {
        get { return (string?)GetValue(OldTextProperty); }
        set { SetValue(OldTextProperty, value); }
    }

    public static readonly DependencyProperty OldTextProperty =
        DependencyProperty.Register(nameof(OldText), typeof(string), typeof(DiffView), new PropertyMetadata(null, OnTextsChanged));

    public string? NewText
    {
        get { return (string?)GetValue(NewTextProperty); }
        set { SetValue(NewTextProperty, value); }
    }

    public static readonly DependencyProperty NewTextProperty =
        DependencyProperty.Register(nameof(NewText), typeof(string), typeof(DiffView), new PropertyMetadata(null, OnTextsChanged));

    public int MaxEditDistance
    {
        get { return (int)GetValue(MaxEditDistanceProperty); }
        set { SetValue(MaxEditDistanceProperty, value); }
    }

    public static readonly DependencyProperty MaxEditDistanceProperty =
        DependencyProperty.Register(nameof(MaxEditDistance), typeof(int), typeof(DiffView), new PropertyMetadata(DiffEngine.DefaultMaxEditDistance, OnTextsChanged));

    public bool ShowHeader
    {
        get { return (bool)GetValue(ShowHeaderProperty); }
        set { SetValue(ShowHeaderProperty, value); }
    }

    public static readonly DependencyProperty ShowHeaderProperty =
        DependencyProperty.Register(nameof(ShowHeader), typeof(bool), typeof(DiffView), new PropertyMetadata(true, OnChromeChanged));

    public bool ShowLineNumbers
    {
        get { return (bool)GetValue(ShowLineNumbersProperty); }
        set { SetValue(ShowLineNumbersProperty, value); }
    }

    public static readonly DependencyProperty ShowLineNumbersProperty =
        DependencyProperty.Register(nameof(ShowLineNumbers), typeof(bool), typeof(DiffView), new PropertyMetadata(true, OnRowsChanged));

    public bool CollapseUnchanged
    {
        get { return (bool)GetValue(CollapseUnchangedProperty); }
        set { SetValue(CollapseUnchangedProperty, value); }
    }

    public static readonly DependencyProperty CollapseUnchangedProperty =
        DependencyProperty.Register(nameof(CollapseUnchanged), typeof(bool), typeof(DiffView), new PropertyMetadata(true, OnCollapseChanged));

    public int ContextLines
    {
        get { return (int)GetValue(ContextLinesProperty); }
        set { SetValue(ContextLinesProperty, value); }
    }

    public static readonly DependencyProperty ContextLinesProperty =
        DependencyProperty.Register(nameof(ContextLines), typeof(int), typeof(DiffView), new PropertyMetadata(3, OnRowsChanged));

    public bool WrapLines
    {
        get { return (bool)GetValue(WrapLinesProperty); }
        set { SetValue(WrapLinesProperty, value); }
    }

    public static readonly DependencyProperty WrapLinesProperty =
        DependencyProperty.Register(nameof(WrapLines), typeof(bool), typeof(DiffView), new PropertyMetadata(false, OnWrapChanged));

    public double MaxBodyHeight
    {
        get { return (double)GetValue(MaxBodyHeightProperty); }
        set { SetValue(MaxBodyHeightProperty, value); }
    }

    public static readonly DependencyProperty MaxBodyHeightProperty =
        DependencyProperty.Register(nameof(MaxBodyHeight), typeof(double), typeof(DiffView), new PropertyMetadata(480.0, OnChromeChanged));

    public DiffViewMode ViewMode
    {
        get { return (DiffViewMode)GetValue(ViewModeProperty); }
        set { SetValue(ViewModeProperty, value); }
    }

    public static readonly DependencyProperty ViewModeProperty =
        DependencyProperty.Register(nameof(ViewMode), typeof(DiffViewMode), typeof(DiffView), new PropertyMetadata(DiffViewMode.Unified, OnViewModeChanged));

    public DiffUnifiedMode UnifiedMode
    {
        get { return (DiffUnifiedMode)GetValue(UnifiedModeProperty); }
        set { SetValue(UnifiedModeProperty, value); }
    }

    public static readonly DependencyProperty UnifiedModeProperty =
        DependencyProperty.Register(nameof(UnifiedMode), typeof(DiffUnifiedMode), typeof(DiffView), new PropertyMetadata(DiffUnifiedMode.Inline, OnUnifiedModeChanged));

    public DiffUnifiedModes AvailableUnifiedModes
    {
        get { return (DiffUnifiedModes)GetValue(AvailableUnifiedModesProperty); }
        set { SetValue(AvailableUnifiedModesProperty, value); }
    }

    public static readonly DependencyProperty AvailableUnifiedModesProperty =
        DependencyProperty.Register(nameof(AvailableUnifiedModes), typeof(DiffUnifiedModes), typeof(DiffView), new PropertyMetadata(DiffUnifiedModes.All, OnAvailableUnifiedModesChanged));

    public bool ShowModeSwitcher
    {
        get { return (bool)GetValue(ShowModeSwitcherProperty); }
        set { SetValue(ShowModeSwitcherProperty, value); }
    }

    public static readonly DependencyProperty ShowModeSwitcherProperty =
        DependencyProperty.Register(nameof(ShowModeSwitcher), typeof(bool), typeof(DiffView), new PropertyMetadata(true, OnModeSwitcherChanged));

    public bool IsAnimated
    {
        get { return (bool)GetValue(IsAnimatedProperty); }
        set { SetValue(IsAnimatedProperty, value); }
    }

    public static readonly DependencyProperty IsAnimatedProperty =
        DependencyProperty.Register(nameof(IsAnimated), typeof(bool), typeof(DiffView), new PropertyMetadata(true));

    public Brush? AddedBackground
    {
        get { return (Brush?)GetValue(AddedBackgroundProperty); }
        set { SetValue(AddedBackgroundProperty, value); }
    }

    public static readonly DependencyProperty AddedBackgroundProperty =
        DependencyProperty.Register(nameof(AddedBackground), typeof(Brush), typeof(DiffView), new PropertyMetadata(null, OnRowsChanged));

    public Brush? RemovedBackground
    {
        get { return (Brush?)GetValue(RemovedBackgroundProperty); }
        set { SetValue(RemovedBackgroundProperty, value); }
    }

    public static readonly DependencyProperty RemovedBackgroundProperty =
        DependencyProperty.Register(nameof(RemovedBackground), typeof(Brush), typeof(DiffView), new PropertyMetadata(null, OnRowsChanged));

    public Brush? AddedAccent
    {
        get { return (Brush?)GetValue(AddedAccentProperty); }
        set { SetValue(AddedAccentProperty, value); }
    }

    public static readonly DependencyProperty AddedAccentProperty =
        DependencyProperty.Register(nameof(AddedAccent), typeof(Brush), typeof(DiffView), new PropertyMetadata(null, OnRowsChanged));

    public Brush? RemovedAccent
    {
        get { return (Brush?)GetValue(RemovedAccentProperty); }
        set { SetValue(RemovedAccentProperty, value); }
    }

    public static readonly DependencyProperty RemovedAccentProperty =
        DependencyProperty.Register(nameof(RemovedAccent), typeof(Brush), typeof(DiffView), new PropertyMetadata(null, OnRowsChanged));

    public bool HighlightChanges
    {
        get { return (bool)GetValue(HighlightChangesProperty); }
        set { SetValue(HighlightChangesProperty, value); }
    }

    public static readonly DependencyProperty HighlightChangesProperty =
        DependencyProperty.Register(nameof(HighlightChanges), typeof(bool), typeof(DiffView), new PropertyMetadata(true, OnRowsChanged));

    public Brush? AddedHighlight
    {
        get { return (Brush?)GetValue(AddedHighlightProperty); }
        set { SetValue(AddedHighlightProperty, value); }
    }

    public static readonly DependencyProperty AddedHighlightProperty =
        DependencyProperty.Register(nameof(AddedHighlight), typeof(Brush), typeof(DiffView), new PropertyMetadata(null, OnRowsChanged));

    public Brush? RemovedHighlight
    {
        get { return (Brush?)GetValue(RemovedHighlightProperty); }
        set { SetValue(RemovedHighlightProperty, value); }
    }

    public static readonly DependencyProperty RemovedHighlightProperty =
        DependencyProperty.Register(nameof(RemovedHighlight), typeof(Brush), typeof(DiffView), new PropertyMetadata(null, OnRowsChanged));

    public Brush? HighlightForeground
    {
        get { return (Brush?)GetValue(HighlightForegroundProperty); }
        set { SetValue(HighlightForegroundProperty, value); }
    }

    public static readonly DependencyProperty HighlightForegroundProperty =
        DependencyProperty.Register(nameof(HighlightForeground), typeof(Brush), typeof(DiffView), new PropertyMetadata(null, OnRowsChanged));

    public Brush? SelectionBackground
    {
        get { return (Brush?)GetValue(SelectionBackgroundProperty); }
        set { SetValue(SelectionBackgroundProperty, value); }
    }

    public static readonly DependencyProperty SelectionBackgroundProperty =
        DependencyProperty.Register(nameof(SelectionBackground), typeof(Brush), typeof(DiffView), new PropertyMetadata(null, OnRowsChanged));

    public Brush? SelectionForeground
    {
        get { return (Brush?)GetValue(SelectionForegroundProperty); }
        set { SetValue(SelectionForegroundProperty, value); }
    }

    public static readonly DependencyProperty SelectionForegroundProperty =
        DependencyProperty.Register(nameof(SelectionForeground), typeof(Brush), typeof(DiffView), new PropertyMetadata(null, OnRowsChanged));

    public Brush? PlaceholderBackground
    {
        get { return (Brush?)GetValue(PlaceholderBackgroundProperty); }
        set { SetValue(PlaceholderBackgroundProperty, value); }
    }

    public static readonly DependencyProperty PlaceholderBackgroundProperty =
        DependencyProperty.Register(nameof(PlaceholderBackground), typeof(Brush), typeof(DiffView), new PropertyMetadata(null, OnRowsChanged));

    public bool UseSingleLineSelection
    {
        get { return (bool)GetValue(UseSingleLineSelectionProperty); }
        set { SetValue(UseSingleLineSelectionProperty, value); }
    }

    public static readonly DependencyProperty UseSingleLineSelectionProperty =
        DependencyProperty.Register(nameof(UseSingleLineSelection), typeof(bool), typeof(DiffView), new PropertyMetadata(false, OnRowsChanged));

    public DataTemplate? UnifiedRowTemplate
    {
        get { return (DataTemplate?)GetValue(UnifiedRowTemplateProperty); }
        set { SetValue(UnifiedRowTemplateProperty, value); }
    }

    public static readonly DependencyProperty UnifiedRowTemplateProperty =
        DependencyProperty.Register(nameof(UnifiedRowTemplate), typeof(DataTemplate), typeof(DiffView), new PropertyMetadata(DefaultUnifiedRowItemTemplate, OnRowsChanged));

    public DataTemplate? SplitRowTemplate
    {
        get { return (DataTemplate?)GetValue(SplitRowTemplateProperty); }
        set { SetValue(SplitRowTemplateProperty, value); }
    }

    public static readonly DependencyProperty SplitRowTemplateProperty =
        DependencyProperty.Register(nameof(SplitRowTemplate), typeof(DataTemplate), typeof(DiffView), new PropertyMetadata(DefaultSplitRowItemTemplate, OnRowsChanged));

    public Brush? SelectionBase
    {
        get { return (Brush?)GetValue(SelectionBaseProperty); }
        set { SetValue(SelectionBaseProperty, value); }
    }

    public static readonly DependencyProperty SelectionBaseProperty =
        DependencyProperty.Register(nameof(SelectionBase), typeof(Brush), typeof(DiffView), new PropertyMetadata(null, OnRowsChanged));

    public string? ApproximateText
    {
        get { return (string?)GetValue(ApproximateTextProperty); }
        set { SetValue(ApproximateTextProperty, value); }
    }

    public static readonly DependencyProperty ApproximateTextProperty =
        DependencyProperty.Register(nameof(ApproximateText), typeof(string), typeof(DiffView), new PropertyMetadata(null, OnChromeChanged));

    public string? InlineText
    {
        get { return (string?)GetValue(InlineTextProperty); }
        set { SetValue(InlineTextProperty, value); }
    }

    public static readonly DependencyProperty InlineTextProperty =
        DependencyProperty.Register(nameof(InlineText), typeof(string), typeof(DiffView), new PropertyMetadata(null));

    public string? BeforeText
    {
        get { return (string?)GetValue(BeforeTextProperty); }
        set { SetValue(BeforeTextProperty, value); }
    }

    public static readonly DependencyProperty BeforeTextProperty =
        DependencyProperty.Register(nameof(BeforeText), typeof(string), typeof(DiffView), new PropertyMetadata(null));

    public string? AfterText
    {
        get { return (string?)GetValue(AfterTextProperty); }
        set { SetValue(AfterTextProperty, value); }
    }

    public static readonly DependencyProperty AfterTextProperty =
        DependencyProperty.Register(nameof(AfterText), typeof(string), typeof(DiffView), new PropertyMetadata(null));

    public string? CopyText
    {
        get { return (string?)GetValue(CopyTextProperty); }
        set { SetValue(CopyTextProperty, value); }
    }

    public static readonly DependencyProperty CopyTextProperty =
        DependencyProperty.Register(nameof(CopyText), typeof(string), typeof(DiffView), new PropertyMetadata(null));

    public string? SelectAllText
    {
        get { return (string?)GetValue(SelectAllTextProperty); }
        set { SetValue(SelectAllTextProperty, value); }
    }

    public static readonly DependencyProperty SelectAllTextProperty =
        DependencyProperty.Register(nameof(SelectAllText), typeof(string), typeof(DiffView), new PropertyMetadata(null));

    public string? ExpanderFormat
    {
        get { return (string?)GetValue(ExpanderFormatProperty); }
        set { SetValue(ExpanderFormatProperty, value); }
    }

    public static readonly DependencyProperty ExpanderFormatProperty =
        DependencyProperty.Register(nameof(ExpanderFormat), typeof(string), typeof(DiffView), new PropertyMetadata(null, OnRowsChanged));

    public string? ExpandAutomationFormat
    {
        get { return (string?)GetValue(ExpandAutomationFormatProperty); }
        set { SetValue(ExpandAutomationFormatProperty, value); }
    }

    public static readonly DependencyProperty ExpandAutomationFormatProperty =
        DependencyProperty.Register(nameof(ExpandAutomationFormat), typeof(string), typeof(DiffView), new PropertyMetadata(null, OnRowsChanged));

    public string? AddedLineFormat
    {
        get { return (string?)GetValue(AddedLineFormatProperty); }
        set { SetValue(AddedLineFormatProperty, value); }
    }

    public static readonly DependencyProperty AddedLineFormatProperty =
        DependencyProperty.Register(nameof(AddedLineFormat), typeof(string), typeof(DiffView), new PropertyMetadata(null, OnRowsChanged));

    public string? RemovedLineFormat
    {
        get { return (string?)GetValue(RemovedLineFormatProperty); }
        set { SetValue(RemovedLineFormatProperty, value); }
    }

    public static readonly DependencyProperty RemovedLineFormatProperty =
        DependencyProperty.Register(nameof(RemovedLineFormat), typeof(string), typeof(DiffView), new PropertyMetadata(null, OnRowsChanged));

    public string? FileAutomationFormat
    {
        get { return (string?)GetValue(FileAutomationFormatProperty); }
        set { SetValue(FileAutomationFormatProperty, value); }
    }

    public static readonly DependencyProperty FileAutomationFormatProperty =
        DependencyProperty.Register(nameof(FileAutomationFormat), typeof(string), typeof(DiffView), new PropertyMetadata(null, OnChromeChanged));

    public string? SummaryAutomationFormat
    {
        get { return (string?)GetValue(SummaryAutomationFormatProperty); }
        set { SetValue(SummaryAutomationFormatProperty, value); }
    }

    public static readonly DependencyProperty SummaryAutomationFormatProperty =
        DependencyProperty.Register(nameof(SummaryAutomationFormat), typeof(string), typeof(DiffView), new PropertyMetadata(null, OnChromeChanged));

    private static void OnChromeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((DiffView)d).UpdateChrome();
    }

    private static void OnRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((DiffView)d).RebuildRows();
    }

    private static void OnTextsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((DiffView)d).ScheduleRefresh();
    }

    private static void OnViewModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((DiffView)d).HandleViewModeChanged();
    }

    private static void OnUnifiedModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((DiffView)d).HandleUnifiedModeChanged();
    }

    private static void OnAvailableUnifiedModesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((DiffView)d).HandleAvailableUnifiedModesChanged();
    }

    private static void OnModeSwitcherChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((DiffView)d).SyncModeButtons();
    }

    private static void OnCollapseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (DiffView)d;
        control._expanded.Clear();
        control.RebuildRows();
    }

    private static void OnWrapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((DiffView)d).RebuildRows();
    }
}
