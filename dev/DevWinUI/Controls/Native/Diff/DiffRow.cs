using System.Windows.Input;
using Microsoft.UI.Xaml.Documents;

namespace DevWinUI;

public sealed partial class DiffRow
{
    private const int MaxDisplayLength = 4000;

    private readonly string _rawText;

    public DiffRow(DiffKind kind, string text, int oldNo, int newNo)
    {
        Kind = kind;
        _rawText = text;
        OldNo = oldNo;
        NewNo = newNo;
    }

    public DiffRow(int regionStart, int hiddenCount)
    {
        IsExpander = true;
        RegionStart = regionStart;
        HiddenCount = hiddenCount;
        _rawText = "";
    }

    public DiffKind Kind { get; }
    public bool IsExpander { get; }
    public int RegionStart { get; }
    public int HiddenCount { get; }
    public int OldNo { get; }
    public int NewNo { get; }

    public string ExpanderFormat { get; init; } = "{0}";
    public string ExpandAutomationFormat { get; init; } = "{0}";
    public string AddedFormat { get; init; } = "{0}";
    public string RemovedFormat { get; init; } = "{0}";

    public Brush? Background { get; set; }
    public Brush? AccentBrush { get; set; }
    public double OldGutterWidth { get; set; }
    public double NewGutterWidth { get; set; }
    public TextWrapping Wrapping { get; init; } = TextWrapping.NoWrap;
    public ICommand? ExpandCommand { get; init; }
    public IReadOnlyList<TextRange>? Highlights { get; init; }
    public Brush? HighlightBackground { get; init; }
    public Brush? HighlightForeground { get; init; }
    public int SelectionStart { get; init; }
    public int SelectionLength { get; init; }
    public Brush? SelectionBackground { get; init; }
    public Brush? SelectionForeground { get; init; }
    public bool UseNativeSelection { get; init; }

    public Visibility CodeVisibility => IsExpander ? Visibility.Collapsed : Visibility.Visible;
    public Visibility ExpanderVisibility => IsExpander ? Visibility.Visible : Visibility.Collapsed;
    public Visibility OldGutterVisibility => OldGutterWidth > 0 ? Visibility.Visible : Visibility.Collapsed;
    public Visibility NewGutterVisibility => NewGutterWidth > 0 ? Visibility.Visible : Visibility.Collapsed;

    public string OldNumber => OldNo > 0 ? OldNo.ToString() : "";
    public string NewNumber => NewNo > 0 ? NewNo.ToString() : "";

    public string Marker => Kind switch
    {
        DiffKind.Add => "+",
        DiffKind.Remove => "−",
        _ => " ",
    };

    public string Text => ToDisplay(_rawText);

    public static string ToDisplay(string raw)
    {
        string text = raw.Length > MaxDisplayLength
            ? string.Concat(raw.AsSpan(0, MaxDisplayLength), "…")
            : raw;

        return text.Contains('\t') ? text.Replace("\t", "    ") : text;
    }

    public string ExpanderText => string.Format(ExpanderFormat, HiddenCount);

    public string AutomationName => IsExpander
        ? string.Format(ExpandAutomationFormat, HiddenCount)
        : Kind switch
        {
            DiffKind.Add => string.Format(AddedFormat, _rawText),
            DiffKind.Remove => string.Format(RemovedFormat, _rawText),
            _ => _rawText,
        };
}
