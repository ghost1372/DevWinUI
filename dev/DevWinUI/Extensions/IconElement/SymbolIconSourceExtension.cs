namespace DevWinUI;

[MarkupExtensionReturnType(ReturnType = typeof(FontIconSource))]
public partial class SymbolIconSourceExtension : TextIconExtension
{
    /// <summary>
    /// Gets or sets the <see cref="Symbol"/> value representing the icon to display.
    /// </summary>
    public Symbol Symbol { get; set; }

    /// <inheritdoc/>
    protected override object ProvideValue()
    {
        var fontIcon = new FontIconSource
        {
            Glyph = unchecked((char)Symbol).ToString(),
            FontFamily = SymbolThemeFontFamily,
            FontWeight = FontWeight,
            FontStyle = FontStyle,
            IsTextScaleFactorEnabled = IsTextScaleFactorEnabled,
            MirroredWhenRightToLeft = MirroredWhenRightToLeft
        };

        if (FontSize > 0)
        {
            fontIcon.FontSize = FontSize;
        }

        if (Foreground != null)
        {
            fontIcon.Foreground = Foreground;
        }

        return fontIcon;
    }
}
