namespace DevWinUI;

public readonly record struct DiffLine(DiffKind Kind, string Text, int? OldNumber = null, int? NewNumber = null, int Pair = -1);
