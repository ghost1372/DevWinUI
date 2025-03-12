namespace DevWinUI;
public static partial class Extensions
{
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
