namespace DevWinUIGallery.Models;

public partial class ThemedIconItem
{
    public object Key { get; set; }
    public Style Style { get; set; }

    public ThemedIconItem(object key, Style style)
    {
        this.Key = key;
        this.Style = style;
    }
}
