using System.Diagnostics.CodeAnalysis;

namespace DevWinUI;
public partial class ColorPaletteItem : Control
{
    private ControlTemplate? ColorPaletteTabViewTemplate { get; set; }
    private ControlTemplate? ColorPaletteCircleTemplate { get; set; }
    private ControlTemplate? ColorPaletteRectangleTemplate { get; set; }


    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ControlTemplate))]
    public ColorPaletteItem()
    {
        DefaultStyleKey = typeof(ColorPaletteItem);

        if (Application.Current.Resources["ColorPaletteTabViewTemplate"] is ControlTemplate colorPaletteTabViewTemplate)
            this.ColorPaletteTabViewTemplate = colorPaletteTabViewTemplate;

        if (Application.Current.Resources["ColorPaletteCircleTemplate"] is ControlTemplate colorPaletteCircleTemplate)
            this.ColorPaletteCircleTemplate = colorPaletteCircleTemplate;

        if (Application.Current.Resources["ColorPaletteRectangleTemplate"] is ControlTemplate colorPaletteRectangleTemplate)
            this.ColorPaletteRectangleTemplate = colorPaletteRectangleTemplate;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateTemplate();
        UpdateColorName();
    }
    private void UpdateTemplate()
    {
        Template = ItemShape switch
        {
            ColorItemShape.Circle => ColorPaletteCircleTemplate,
            ColorItemShape.Rectangle => ColorPaletteRectangleTemplate,
            ColorItemShape.Tab => ColorPaletteTabViewTemplate,
            _ => Template
        };
    }
    private void UpdateToolTip()
    {
        if (ShowToolTip)
        {
            ToolTipService.SetToolTip(this, InternalColorName);
        }
        else
        {
            ToolTipService.SetToolTip(this, null);
        }
    }

    private void UpdateColorName()
    {
        if (ShowHexCode)
        {
            InternalColorName = Color.ToString();
        }
        else if (ShowColorName)
        {
            var namedColor = InternalColors?.FirstOrDefault(c => c.Color.Equals(Color));
            InternalColorName = namedColor?.ColorName ?? Color.ToString();
        }
        else
        {
            InternalColorName = string.Empty;
        }

        UpdateToolTip();
    }
}
