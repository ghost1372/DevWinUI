namespace DevWinUI;

[MarkupExtensionReturnType(ReturnType = typeof(FontIcon))]
public partial class FontIconExtension : TextIconExtension
{
    /// <summary>
    /// Gets or sets the <see cref="string"/> value representing the icon to display.
    /// </summary>
    public string? Glyph { get; set; }

    public GlyphCode GlyphCode { get; set; } = GlyphCode.None;
    public GlyphName GlyphName { get; set; } = GlyphName.None;

    /// <summary>
    /// Gets or sets the font family to use to display the icon. If <see langword="null"/>, "Segoe MDL2 Assets" will be used.
    /// </summary>
    public FontFamily? FontFamily { get; set; }

    /// <inheritdoc/>
    protected override object ProvideValue()
    {
        if (string.IsNullOrEmpty(Glyph))
        {
            if (GlyphCode != GlyphCode.None)
            {
                Glyph = $"{GeneralHelper.GetGlyphCharacter(GlyphCode.ToString())}";
            }

            if (GlyphName != GlyphName.None)
            {
                var glyphNameIndex = (int)GlyphName;
                var glyphCode = (GlyphCode)glyphNameIndex;
                Glyph = $"{GeneralHelper.GetGlyphCharacter(glyphCode.ToString())}";
            }
        }

        default(ArgumentNullException).ThrowIfNull(Glyph);

        var fontIcon = new FontIcon
        {
            Glyph = Glyph,
            FontFamily = FontFamily ?? SymbolThemeFontFamily,
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
