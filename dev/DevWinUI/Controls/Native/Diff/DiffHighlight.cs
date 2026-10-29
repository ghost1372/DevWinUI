using Microsoft.UI.Xaml.Documents;

namespace DevWinUI;

public static partial class DiffHighlight
{
    public static DiffRow? GetRow(DependencyObject obj)
    {
        return (DiffRow?)obj.GetValue(RowProperty);
    }

    public static void SetRow(DependencyObject obj, DiffRow? value)
    {
        obj.SetValue(RowProperty, value);
    }

    public static readonly DependencyProperty RowProperty =
        DependencyProperty.RegisterAttached("Row", typeof(DiffRow), typeof(DiffHighlight), new PropertyMetadata(null, OnRowChanged));

    private static readonly DependencyProperty SelectionHighlighterProperty =
        DependencyProperty.RegisterAttached("SelectionHighlighter", typeof(TextHighlighter), typeof(DiffHighlight), new PropertyMetadata(null));

    public static void SetSelection(TextBlock textBlock, int start, int length, Brush? background, Brush? foreground)
    {
        if (textBlock.GetValue(SelectionHighlighterProperty) is TextHighlighter old)
        {
            textBlock.TextHighlighters.Remove(old);
            textBlock.SetValue(SelectionHighlighterProperty, null);
        }

        if (length <= 0 || background is null) return;

        var highlighter = new TextHighlighter { Background = background, Foreground = foreground };
        highlighter.Ranges.Add(new TextRange { StartIndex = start, Length = length });
        textBlock.TextHighlighters.Add(highlighter);
        textBlock.SetValue(SelectionHighlighterProperty, highlighter);
    }

    private static void OnRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBlock textBlock) return;

        var row = e.NewValue as DiffRow;
        textBlock.IsTextSelectionEnabled = row?.UseNativeSelection ?? false;
        if (row is { UseNativeSelection: true, SelectionBackground: SolidColorBrush nativeSelection })
        {
            textBlock.SelectionHighlightColor = nativeSelection;
        }

        textBlock.TextHighlighters.Clear();
        textBlock.SetValue(SelectionHighlighterProperty, null);
        textBlock.Inlines.Clear();
        textBlock.Inlines.Add(new Run { Text = row?.Text ?? string.Empty });

        if (row?.Highlights is { Count: > 0 } ranges)
        {
            var highlighter = new TextHighlighter
            {
                Background = row.HighlightBackground,
                Foreground = row.HighlightForeground,
            };

            foreach (TextRange range in ranges)
            {
                highlighter.Ranges.Add(range);
            }

            textBlock.TextHighlighters.Add(highlighter);
        }

        if (row is { SelectionLength: > 0 })
        {
            SetSelection(textBlock, row.SelectionStart, row.SelectionLength, row.SelectionBackground, row.SelectionForeground);
        }
    }
}
