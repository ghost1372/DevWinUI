using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DevWinUI;
/// <summary>
/// Helpers for working with strings and string representations.
/// </summary>
public static partial class StringExtensions
{
    /// <summary>
    /// Regular expression for matching a phone number.
    /// </summary>
    internal const string PhoneNumberRegex = @"^[+]?(\d{1,3})?[\s.-]?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$";

    /// <summary>
    /// Regular expression for matching a string that contains only letters.
    /// </summary>
    internal const string CharactersRegex = "^[A-Za-z]+$";

    /// <summary>
    /// Regular expression for matching an email address.
    /// </summary>
    /// <remarks>General Email Regex (RFC 5322 Official Standard) from https://emailregex.com.</remarks>
    internal const string EmailRegex = "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])";

    /// <summary>
    /// Regular expression of HTML tags to remove.
    /// </summary>
    private const string RemoveHtmlTagsRegex = @"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>";

    /// <summary>
    /// Regular expression for removing comments from HTML.
    /// </summary>
    private static readonly Regex RemoveHtmlCommentsRegex = new("<!--.*?-->", RegexOptions.Singleline);

    /// <summary>
    /// Regular expression for removing scripts from HTML.
    /// </summary>
    private static readonly Regex RemoveHtmlScriptsRegex = new(@"(?s)<script.*?(/>|</script>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

    /// <summary>
    /// Regular expression for removing styles from HTML.
    /// </summary>
    private static readonly Regex RemoveHtmlStylesRegex = new(@"(?s)<style.*?(/>|</style>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

    extension(string value)
    {
        /// <summary>
        /// Determines whether a string is a valid email address.
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns><c>true</c> for a valid email address; otherwise, <c>false</c>.</returns>
        public bool IsEmail() => Regex.IsMatch(value, EmailRegex);

        /// <summary>
        /// Determines whether a string is a valid decimal number.
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns><c>true</c> for a valid decimal number; otherwise, <c>false</c>.</returns>
        public bool IsDecimal()
        {
            return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out _);
        }

        /// <summary>
        /// Determines whether a string is a valid integer.
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns><c>true</c> for a valid integer; otherwise, <c>false</c>.</returns>
        public bool IsNumeric()
        {
            return int.TryParse(value, out _);
        }

        /// <summary>
        /// Determines whether a string is a valid phone number.
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns><c>true</c> for a valid phone number; otherwise, <c>false</c>.</returns>
        public bool IsPhoneNumber() => Regex.IsMatch(value, PhoneNumberRegex);

        /// <summary>
        /// Determines whether a string contains only letters.
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns><c>true</c> if the string contains only letters; otherwise, <c>false</c>.</returns>
        public bool IsCharacterString() => Regex.IsMatch(value, CharactersRegex);

        /// <summary>
        /// Returns a string with HTML comments, scripts, styles, and tags removed.
        /// </summary>
        /// <param name="value">HTML string.</param>
        /// <returns>Decoded HTML string.</returns>
        [return: NotNullIfNotNull(nameof(value))]
        public string? DecodeHtml()
        {
            if (value is null)
            {
                return null;
            }

            string? ret = value.FixHtml();

            // Remove html tags
            ret = new Regex(RemoveHtmlTagsRegex).Replace(ret, string.Empty);

            return WebUtility.HtmlDecode(ret);
        }

        /// <summary>
        /// Returns a string with HTML comments, scripts, and styles removed.
        /// </summary>
        /// <returns>Fixed HTML string.</returns>
        public string FixHtml()
        {
            // Remove comments
            string? withoutComments = RemoveHtmlCommentsRegex.Replace(value, string.Empty);

            // Remove scripts
            string? withoutScripts = RemoveHtmlScriptsRegex.Replace(withoutComments, string.Empty);

            // Remove styles
            string? withoutStyles = RemoveHtmlStylesRegex.Replace(withoutScripts, string.Empty);

            return withoutStyles;
        }

        /// <summary>
        /// Truncates a string to the specified length.
        /// </summary>
        /// <param name="value">The string to be truncated.</param>
        /// <param name="length">The maximum length.</param>
        /// <returns>Truncated string.</returns>
        public string Truncate(int length) => Truncate(value, length, false);

        /// <summary>
        /// Truncates a string to the specified length.
        /// </summary>
        /// <param name="value">The string to be truncated.</param>
        /// <param name="length">The maximum length.</param>
        /// <param name="ellipsis"><c>true</c> to add ellipsis to the truncated text; otherwise, <c>false</c>.</param>
        /// <returns>Truncated string.</returns>
        public string Truncate(int length, bool ellipsis)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value!.Trim();

                if (value.Length > length)
                {
                    if (ellipsis)
                    {
                        return value.Substring(0, length) + "...";
                    }

                    return value.Substring(0, length);
                }
            }

            return value ?? string.Empty;
        }

        public string GetHash(HashAlgorithm hashAlgorithm)
        {
            return SecurityHelper.GetHash(value, hashAlgorithm);
        }
    }
}
