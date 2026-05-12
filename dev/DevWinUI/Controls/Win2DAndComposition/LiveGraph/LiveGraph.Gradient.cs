namespace DevWinUI;

public partial class LiveGraph
{
    private Color[] purpleColors = new Color[]
    {
        Color.FromArgb(128, 216, 191, 216), // light purple
        Color.FromArgb(180, 160, 120, 200), // medium purple
        Color.FromArgb(220, 106, 13, 173) // dark purple
    };

    private Color[] greenColors = new Color[]
    {
        Color.FromArgb(128, 144, 238, 144), // light green
        Color.FromArgb(180, 60, 179, 113),  // medium green
        Color.FromArgb(220, 0, 128, 0)      // dark green
    };

    private Color[] blueColors = new Color[]
    {
        Color.FromArgb(128, 173, 216, 230), // light blue
        Color.FromArgb(180, 70, 130, 180),  // medium blue
        Color.FromArgb(220, 25, 25, 112)    // dark blue
    };

    private Color[] redColors = new Color[]
    {
        Color.FromArgb(128, 255, 182, 193), // light red/pink
        Color.FromArgb(180, 220, 20, 60),   // medium red
        Color.FromArgb(220, 139, 0, 0)      // dark red
    };

    private CanvasGradientStop[] CreateGradientStops(Color[] colors)
    {
        var stops = new CanvasGradientStop[colors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            stops[i] = new CanvasGradientStop
            {
                Color = colors[i],
                Position = i / (float)(colors.Length - 1)
            };
        }
        return stops;
    }

    public (CanvasLinearGradientBrush Brush, CanvasLinearGradientBrush OpacityBrush, CanvasSolidColorBrush BorderBrush) GetCustomBrush(ICanvasResourceCreator resourceCreator, Color[] colors)
    {
        var brush = new CanvasLinearGradientBrush(resourceCreator, CreateGradientStops(colors));

        var opacityBrush = new CanvasLinearGradientBrush(resourceCreator, new CanvasGradientStop[]
        {
            new CanvasGradientStop { Color = Color.FromArgb(0,0,0,0), Position = 0f },
            new CanvasGradientStop { Color = Color.FromArgb(255,255,255,255), Position = 1f }
        });

        var borderBrush = new CanvasSolidColorBrush(resourceCreator, colors[0]);

        brush.StartPoint = new Vector2(0, (float)canvas.ActualHeight);
        brush.EndPoint = new Vector2(0, 0);

        opacityBrush.StartPoint = new Vector2(0, (float)canvas.ActualHeight);
        opacityBrush.EndPoint = new Vector2(0, 0);

        return (brush, opacityBrush, borderBrush);
    }

    public (CanvasLinearGradientBrush Brush, CanvasLinearGradientBrush OpacityBrush, CanvasSolidColorBrush BorderBrush) GetGreenBrush(ICanvasResourceCreator resourceCreator)
    {
        return GetCustomBrush(resourceCreator, greenColors);
    }
    public (CanvasLinearGradientBrush Brush, CanvasLinearGradientBrush OpacityBrush, CanvasSolidColorBrush BorderBrush) GetRedBrush(ICanvasResourceCreator resourceCreator)
    {
        return GetCustomBrush(resourceCreator, redColors);
    }
    public (CanvasLinearGradientBrush Brush, CanvasLinearGradientBrush OpacityBrush, CanvasSolidColorBrush BorderBrush) GetBlueBrush(ICanvasResourceCreator resourceCreator)
    {
        return GetCustomBrush(resourceCreator, blueColors);
    }
    public (CanvasLinearGradientBrush Brush, CanvasLinearGradientBrush OpacityBrush, CanvasSolidColorBrush BorderBrush) GetPurpleBrush(ICanvasResourceCreator resourceCreator)
    {
        return GetCustomBrush(resourceCreator, purpleColors);
    }
}
