using System.Text.RegularExpressions;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Documents;
using Windows.UI.Text;

namespace DevWinUI;
public partial class RichTextFormatterHelper
{
    /// <summary>
    /// Formats a RichTextBlock using HTML-like formatting codes, supported tags:  br, b, u, i, font size='size', and pipe characters which are treated like br.  Also supports font color='hexcolor' and font bgcolor='hexcolor'.
    /// To combine both foreground and background colors, use font colors='hexforecolor,hexbackcolor' (colors separated by a comma).
    /// </summary>
    /// <param name="textWithHTMLFormatting"></param>
    /// <param name="richTextBlock"></param>
    public static void FormatRichTextBlock(string textWithHTMLFormatting, RichTextBlock richTextBlock)
    {
        // Clearing existing content
        richTextBlock.Blocks.Clear();

        if (textWithHTMLFormatting == null)
            return;

        // Replacing <br> with a unique placeholder that is unlikely to appear in the text
        string placeholder = "\uE000"; // Using a Private Use Area Unicode character as a placeholder
        string processedText = Regex.Replace(textWithHTMLFormatting, "<br>", placeholder, RegexOptions.IgnoreCase); // replace <br> with linebreak placeholder
        processedText = processedText.Replace("|", placeholder);  // replace | with linebreak placeholder

        // Pattern to match various tags including foreground and background colors
        string pattern = $@"<b>(.*?)</b>|<u>(.*?)</u>|<i>(.*?)</i>|<font size='(.*?)'>(.*?)</font>|<font color='(.*?)'>(.*?)</font>|<font bgcolor='(.*?)'>(.*?)</font>|<font colors='(.*?)'>(.*?)</font>|{Regex.Escape(placeholder)}";

        var paragraph = new Paragraph();
        ProcessFormattingText(processedText, pattern, paragraph.Inlines);
        richTextBlock.Blocks.Add(paragraph);
    }

    /// <summary>
    /// Formats a TextBlock using HTML-like formatting codes, supported tags:  br, b, u, i, font size='size', and pipe characters which are treated like br.  Also supports font color='hexcolor', but TextBlocks do not support the background colors in this fashion, so use the RichTextBlock overload instead if backcolors are needed.
    /// </summary>
    /// <param name="textWithHTMLFormatting"></param>
    /// <param name="textBlock"></param>
    public static void FormatTextBlock(string textWithHTMLFormatting, TextBlock textBlock)
    {
        // Clearing existing content
        textBlock.Inlines.Clear();

        if (textWithHTMLFormatting == null)
            return;

        // Replacing <br> with a unique placeholder that is unlikely to appear in the text
        string placeholder = "\uE000"; // Using a Private Use Area Unicode character as a placeholder
        string processedText = Regex.Replace(textWithHTMLFormatting, "<br>", placeholder, RegexOptions.IgnoreCase); // replace <br> with linebreak placeholder
        processedText = processedText.Replace("|", placeholder);  // replace | with linebreak placeholder

        // Pattern to match various tags including foreground colors
        string pattern = $@"<b>(.*?)</b>|<u>(.*?)</u>|<i>(.*?)</i>|<font size='(.*?)'>(.*?)</font>|<font color='(.*?)'>(.*?)</font>|{Regex.Escape(placeholder)}";

        ProcessFormattingText(processedText, pattern, textBlock.Inlines);
    }

    /// <summary>
    /// Used by FormatTextBlock() to process the text and apply formatting to apply to the RichTextBlock or TextBlock
    /// </summary>
    /// <param name="text"></param>
    /// <param name="pattern"></param>
    /// <param name="inlines"></param>
    private static void ProcessFormattingText(string text, string pattern, InlineCollection inlines)
    {
        int lastIndex = 0;

        foreach (Match match in Regex.Matches(text, pattern))
        {
            // Text before bold, underlined, italic, sized or line break
            if (match.Index > lastIndex)
            {
                var runBeforeTag = new Run
                {
                    Text = text.Substring(lastIndex, match.Index - lastIndex)
                };
                inlines.Add(runBeforeTag);
            }

            if (match.Value == "\uE000") // Handle line break
            {
                inlines.Add(new LineBreak());
            }
            else
            {
                if (match.Groups[1].Success) // Handle bold text
                {
                    var span = new Span
                    {
                        FontWeight = FontWeights.Bold
                    };
                    ProcessFormattingText(match.Groups[1].Value, pattern, span.Inlines);
                    inlines.Add(span);
                }
                else if (match.Groups[2].Success) // Handle underlined text
                {
                    var underline = new Underline();
                    ProcessFormattingText(match.Groups[2].Value, pattern, underline.Inlines);
                    inlines.Add(underline);
                }
                else if (match.Groups[3].Success) // Handle italic text
                {
                    var span = new Span
                    {
                        FontStyle = FontStyle.Italic
                    };
                    ProcessFormattingText(match.Groups[3].Value, pattern, span.Inlines);
                    inlines.Add(span);
                }
                else if (match.Groups[4].Success && match.Groups[5].Success) // Handle sized text
                {
                    var span = new Span
                    {
                        FontSize = double.Parse(match.Groups[4].Value)
                    };
                    ProcessFormattingText(match.Groups[5].Value, pattern, span.Inlines);
                    inlines.Add(span);
                }
                else if (match.Groups[6].Success && match.Groups[7].Success) // Handle foreground color text
                {
                    var span = new Span
                    {
                        Foreground = new SolidColorBrush(ColorHelper.GetColorFromHex(match.Groups[6].Value))
                    };
                    ProcessFormattingText(match.Groups[7].Value, pattern, span.Inlines);
                    inlines.Add(span);
                }
                else if (match.Groups[8].Success && match.Groups[9].Success) // Handle background color text
                {
                    var span = new Span();
                    var border = new Border
                    {
                        Background = new SolidColorBrush(ColorHelper.GetColorFromHex(match.Groups[8].Value)),
                        Child = new TextBlock
                        {
                            Text = match.Groups[9].Value,
                            TextWrapping = TextWrapping.Wrap
                        }
                    };

                    var inlineUIContainer = new InlineUIContainer
                    {
                        Child = border
                    };

                    inlines.Add(inlineUIContainer);
                }
                else if (match.Groups[10].Success && match.Groups[11].Success) // Handle combined foreground and background color text
                {
                    var colors = match.Groups[10].Value.Split(',');
                    var foregroundColor = ColorHelper.GetColorFromHex(colors[0]);
                    var backgroundColor = ColorHelper.GetColorFromHex(colors[1]);

                    var span = new Span
                    {
                        Foreground = new SolidColorBrush(foregroundColor)
                    };

                    var border = new Border
                    {
                        Background = new SolidColorBrush(backgroundColor),
                        Child = new TextBlock
                        {
                            Text = match.Groups[11].Value,
                            TextWrapping = TextWrapping.Wrap,
                            Foreground = new SolidColorBrush(foregroundColor)
                        }
                    };

                    var inlineUIContainer = new InlineUIContainer
                    {
                        Child = border
                    };

                    inlines.Add(inlineUIContainer);
                }
            }

            lastIndex = match.Index + match.Length;
        }

        // Text after the last tag, if any
        if (lastIndex < text.Length)
        {
            var runAfterLastTag = new Run
            {
                Text = text.Substring(lastIndex)
            };
            inlines.Add(runAfterLastTag);
        }
    }
}
