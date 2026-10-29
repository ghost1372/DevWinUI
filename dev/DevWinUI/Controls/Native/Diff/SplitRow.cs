using System.Windows.Input;

namespace DevWinUI;

internal readonly record struct SplitLine(int Left, int Right)
{
    public bool IsChange => Left != Right;
}

public sealed partial class SplitRow
{
    public SplitRow(DiffRow left, DiffRow right)
    {
        Left = left;
        Right = right;
    }

    public SplitRow(DiffRow expander)
    {
        Expander = expander;
    }

    public DiffRow? Left { get; }
    public DiffRow? Right { get; }
    public DiffRow? Expander { get; }

    public bool IsExpander => Expander is not null;
    public ICommand? ExpandCommand => Expander?.ExpandCommand;
    public string ExpanderText => Expander?.ExpanderText ?? "";

    public Visibility CodeVisibility => IsExpander ? Visibility.Collapsed : Visibility.Visible;
    public Visibility ExpanderVisibility => IsExpander ? Visibility.Visible : Visibility.Collapsed;

    public string AutomationName
    {
        get
        {
            if (Expander is not null) return Expander.AutomationName;
            if (Left is null || Right is null) return "";

            if (Left.Kind == DiffKind.Remove && Right.Kind == DiffKind.Add)
                return $"{Left.AutomationName}. {Right.AutomationName}";
            if (Left.Kind == DiffKind.Remove) return Left.AutomationName;
            if (Right.Kind == DiffKind.Add) return Right.AutomationName;
            return Left.AutomationName;
        }
    }
}
