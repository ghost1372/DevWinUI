namespace DevWinUI;
public static partial class Extensions
{
    public static Color ChangeAlpha(this Color color, double alpha)
    {
        return ColorHelper.ChangeAlpha(color, alpha);
    }

    /// <summary>
    /// Finds the best contrasting color (black or white)
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color ContrastColorBlackWhite(this Color color)
    {
        return ColorHelper.ContrastColorBlackWhite(color);
    }

    /// <summary>
    /// Converts a hexadecimal color string into a SolidColorBrush object.
    /// </summary>
    /// <param name="hex">A string representing a color in hexadecimal format.</param>
    /// <returns>A SolidColorBrush that corresponds to the specified color.</returns>
    public static SolidColorBrush GetSolidColorBrush(this string hex)
    {
        return ColorHelper.GetSolidColorBrush(hex);
    }

    /// <summary>
    /// Formats a rich text block using a string that contains HTML formatting.
    /// </summary>
    /// <param name="textWithHTMLFormatting">The string containing HTML formatting to be applied to the rich text block.</param>
    /// <param name="richTextBlock">The rich text block that will be formatted with the provided HTML string.</param>
    public static void FormatRichTextBlock(this string textWithHTMLFormatting, RichTextBlock richTextBlock)
    {
        RichTextFormatterHelper.FormatRichTextBlock(textWithHTMLFormatting, richTextBlock);
    }

    /// <summary>
    /// Formats a text string with HTML for display in a TextBlock control.
    /// </summary>
    /// <param name="textWithHTMLFormatting">The string containing HTML formatting to be applied.</param>
    /// <param name="textBlock">The TextBlock control where the formatted text will be displayed.</param>
    public static void FormatTextBlock(this string textWithHTMLFormatting, TextBlock textBlock)
    {
        RichTextFormatterHelper.FormatTextBlock(textWithHTMLFormatting, textBlock);
    }
}
