namespace DevWinUI;
public static partial class Extensions
{
    public static SolidColorBrush GetSolidColorBrush(this string hex)
    {
        return ColorHelper.GetSolidColorBrush(hex);
    }
    public static void FormatRichTextBlock(this string textWithHTMLFormatting, RichTextBlock richTextBlock)
    {
        RichTextFormatterHelper.FormatRichTextBlock(textWithHTMLFormatting, richTextBlock);
    }
    public static void FormatTextBlock(this string textWithHTMLFormatting, TextBlock textBlock)
    {
        RichTextFormatterHelper.FormatTextBlock(textWithHTMLFormatting, textBlock);
    }
}
