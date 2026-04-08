//https://github.com/cnbluefire/Squircle.Windows

namespace DevWinUI;

internal partial class CompositionPathBuilder : SquirclePathBuilder
{
    public CompositionPath? CreateGeometry()
    {
        var win2dPathBuilder = new Win2DPathBuilder();
        this.CopyTo(win2dPathBuilder);

        using var geometry = win2dPathBuilder.CreateGeometry(null);
        if (geometry != null)
        {
            return new CompositionPath(geometry);
        }
        return null;
    }
}
