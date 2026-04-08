//https://github.com/cnbluefire/Squircle.Windows

namespace DevWinUI;

public static partial class SquircleCornerRadiusExtensions
{
    public static SquircleCornerRadius ToCornerRadius(this Microsoft.UI.Xaml.CornerRadius cornerRadius)
    {
        return new SquircleCornerRadius(cornerRadius.TopLeft, cornerRadius.TopRight, cornerRadius.BottomRight, cornerRadius.BottomLeft);
    }

    public static Microsoft.UI.Xaml.CornerRadius ToCornerRadius(this SquircleCornerRadius cornerRadius)
    {
        return new Microsoft.UI.Xaml.CornerRadius(cornerRadius.TopLeft, cornerRadius.TopRight, cornerRadius.BottomRight, cornerRadius.BottomLeft);
    }
}
