namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Border), Type = typeof(Border))]
public partial class ShortcutPreview : BaseShortcut
{
    private const string PART_Border = "PART_Border";
    private Border border;
    internal double originalMinWidth = 498;
    public ShortcutPreview()
    {
        DefaultStyleKey = typeof(ShortcutPreview);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        border = GetTemplateChild(PART_Border) as Border;
        border.MinWidth = originalMinWidth;
    }

    public void SetMinWidth(double width)
    {
        border?.MinWidth = Math.Min(originalMinWidth, width);
    }
}
