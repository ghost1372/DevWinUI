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

        // The Color changed callback doesn't run for the default value, so initialize the brush here.
        ColorBrush = new SolidColorBrush(Color);

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
        // The templates are looked up in Application resources and can be missing; never null out the Template.
        var template = ItemShape switch
        {
            ColorItemShape.Circle => ColorPaletteCircleTemplate,
            ColorItemShape.Rectangle => ColorPaletteRectangleTemplate,
            ColorItemShape.Tab => ColorPaletteTabViewTemplate,
            _ => Template
        };

        if (template != null && !ReferenceEquals(template, Template))
        {
            Template = template;
        }
    }
    private void UpdateToolTip()
    {
        if (ShowToolTip && !string.IsNullOrEmpty(InternalColorName))
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
