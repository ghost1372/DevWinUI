namespace DevWinUI;

public sealed partial class WordWrapPanel : Panel
{
    public static readonly DependencyProperty WordProperty =
        DependencyProperty.RegisterAttached("Word", typeof(int), typeof(WordWrapPanel), new PropertyMetadata(0));

    public static readonly DependencyProperty KindProperty =
        DependencyProperty.RegisterAttached("Kind", typeof(ScrambledTextLetterKind), typeof(WordWrapPanel), new PropertyMetadata(ScrambledTextLetterKind.Normal));

    public static int GetWord(DependencyObject element)
    {
        return (int)element.GetValue(WordProperty);
    }

    public static void SetWord(DependencyObject element, int value)
    {
        element.SetValue(WordProperty, value);
    }

    public static ScrambledTextLetterKind GetKind(DependencyObject element)
    {
        return (ScrambledTextLetterKind)element.GetValue(KindProperty);
    }

    public static void SetKind(DependencyObject element, ScrambledTextLetterKind value)
    {
        element.SetValue(KindProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var probe = new Size(double.PositiveInfinity, double.PositiveInfinity);
        foreach (UIElement child in Children) child.Measure(probe);

        return Layout(availableSize.Width, arrange: false);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        Layout(finalSize.Width, arrange: true);
        return finalSize;
    }

    private Size Layout(double maxWidth, bool arrange)
    {
        var children = Children;
        double x = 0, y = 0, lineHeight = 0, lastHeight = 0, usedWidth = 0;
        int i = 0;

        while (i < children.Count)
        {
            int word = GetWord(children[i]);
            int end = i;
            double width = 0, trimmedWidth = 0, height = 0;
            bool lineBreak = false;

            while (end < children.Count && GetWord(children[end]) == word)
            {
                var child = children[end];
                var kind = GetKind(child);

                if (kind == ScrambledTextLetterKind.Break)
                {
                    lineBreak = true;
                }
                else
                {
                    width += child.DesiredSize.Width;
                    if (kind != ScrambledTextLetterKind.Space) trimmedWidth = width;
                    height = Math.Max(height, child.DesiredSize.Height);
                }

                end++;
            }

            if (lineBreak)
            {
                y += lineHeight > 0 ? lineHeight : lastHeight;
                x = 0;
                lineHeight = 0;
                i = end;
                continue;
            }

            if (x > 0 && x + trimmedWidth > maxWidth)
            {
                y += lineHeight;
                x = 0;
                lineHeight = 0;
            }

            if (arrange)
            {
                double cellX = x;
                for (int k = i; k < end; k++)
                {
                    var child = children[k];
                    child.Arrange(new Rect(cellX, y, child.DesiredSize.Width, child.DesiredSize.Height));
                    cellX += child.DesiredSize.Width;
                }
            }

            x += width;
            lineHeight = Math.Max(lineHeight, height);
            lastHeight = lineHeight;
            usedWidth = Math.Max(usedWidth, x);
            i = end;
        }

        return new Size(double.IsInfinity(maxWidth) ? usedWidth : Math.Min(usedWidth, maxWidth), y + lineHeight);
    }
}
