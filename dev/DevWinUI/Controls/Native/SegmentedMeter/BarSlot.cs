namespace DevWinUI;

internal sealed partial class BarSlot
{
    public readonly MeterSegment Segment;
    public readonly Border Element;
    public readonly ColumnDefinition? Column;
    public readonly RowDefinition? Row;
    public readonly double From;
    public readonly double To;
    public double Current;

    public BarSlot(MeterSegment segment, Border element, ColumnDefinition? column, RowDefinition? row, double from, double to)
    {
        Segment = segment;
        Element = element;
        Column = column;
        Row = row;
        From = from;
        To = to;
        Current = from;
    }
}
