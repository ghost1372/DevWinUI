using Microsoft.UI.Xaml.Media.Imaging;

namespace DevWinUI;

public static partial class StringExtensions
{
    extension(string value)
    {
        public string ConvertToPersianDigit()
        {
            return value.Replace("0", "۰").Replace("1", "۱").Replace("2", "۲").Replace("3", "۳").Replace("4", "۴")
                .Replace("5", "۵").Replace("6", "۶").Replace("7", "۷").Replace("8", "۸").Replace("9", "۹")
                .Replace(".", ".");
        }

        public string ConvertToPersianChar()
        {
            return value.Replace("q", "ض").Replace("w", "ص").Replace("e", "ث").Replace("r", "ق").Replace("t", "ف")
                .Replace("y", "غ").Replace("u", "ع").Replace("i", "ه").Replace("o", "خ").Replace("p", "ح")
                .Replace("[", "ج").Replace("]", "چ").Replace("a", "ش").Replace("s", "س").Replace("d", "ی")
                .Replace("f", "ب").Replace("g", "ل").Replace("h", "ا").Replace("j", "ت").Replace("k", "ن")
                .Replace("l", "م").Replace(";", "ک").Replace("\"", "گ").Replace("z", "ظ").Replace("x", "ط")
                .Replace("c", "ز").Replace("v", "ر").Replace("b", "ذ").Replace("n", "د").Replace("m", "پ")
                .Replace(")", "و").Replace("?", "؟");
        }

        public string ConvertToEnglishDigit()
        {
            return value.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4")
                .Replace("۵", "5").Replace("۶", "6").Replace("۷", "7").Replace("۸", "8").Replace("۹", "9")
                .Replace(".", ".");
        }

        /// <summary>
        /// Converts a hexadecimal color string into a SolidColorBrush object.
        /// </summary>
        /// <param name="hex">A string representing a color in hexadecimal format.</param>
        /// <returns>A SolidColorBrush that corresponds to the specified color.</returns>
        public SolidColorBrush GetSolidColorBrush()
        {
            return ColorHelper.GetSolidColorBrush(value);
        }

        /// <summary>
        /// Formats a rich text block using a string that contains HTML formatting.
        /// </summary>
        /// <param name="value">The string containing HTML formatting to be applied to the rich text block.</param>
        /// <param name="richTextBlock">The rich text block that will be formatted with the provided HTML string.</param>
        public void FormatRichTextBlock(RichTextBlock richTextBlock)
        {
            RichTextFormatterHelper.FormatRichTextBlock(value, richTextBlock);
        }

        /// <summary>
        /// Formats a text string with HTML for display in a TextBlock control.
        /// </summary>
        /// <param name="value">The string containing HTML formatting to be applied.</param>
        /// <param name="textBlock">The TextBlock control where the formatted text will be displayed.</param>
        public void FormatTextBlock(TextBlock textBlock)
        {
            RichTextFormatterHelper.FormatTextBlock(value, textBlock);
        }

        public BitmapImage ToBitmapImage() => new BitmapImage(new Uri(value));
    }
}
