namespace DevWinUI;
public static partial class CompositionHelper
{
    /// <summary>
    /// Creates a long shadow effect for a text element by layering multiple shadow visuals with varying opacity and
    /// depth.
    /// </summary>
    /// <param name="depth">Specifies how many layers of shadow visuals to create for the text element.</param>
    /// <param name="opacity">Determines the transparency level of the shadow color applied to the text.</param>
    /// <param name="textElement">Represents the text element for which the shadow effect is being created.</param>
    /// <param name="shadowElement">Indicates the visual element that will display the generated shadow effect.</param>
    /// <param name="color">Defines the base color used to calculate the shadow colors.</param>
    public static void MakeLongShadow(int depth, float opacity, TextBlock textElement, FrameworkElement shadowElement, Color color)
    {
        if (textElement == null || shadowElement == null)
        {
            return;
        }

        var textVisual = ElementCompositionPreview.GetElementVisual(textElement);
        var compositor = textVisual.Compositor;
        var containerVisual = compositor.CreateContainerVisual();
        var mask = textElement.GetAlphaMask();
        Vector3 background = new Vector3(color.R, color.G, color.B);
        for (int i = 0; i < depth; i++)
        {
            var maskBrush = compositor.CreateMaskBrush();
            var shadowColor = background - ((background - new Vector3(0, 0, 0)) * opacity);
            shadowColor = Vector3.Max(Vector3.Zero, shadowColor);
            shadowColor += (background - shadowColor) * i / depth;
            maskBrush.Mask = mask;
            maskBrush.Source = compositor.CreateColorBrush(Color.FromArgb(255, (byte)shadowColor.X, (byte)shadowColor.Y, (byte)shadowColor.Z));
            var visual = compositor.CreateSpriteVisual();
            visual.Brush = maskBrush;
            visual.Offset = new Vector3(i + 1, i + 1, 0);
            var bindSizeAnimation = compositor.CreateExpressionAnimation("textVisual.Size");
            bindSizeAnimation.SetReferenceParameter("textVisual", textVisual);
            visual.StartAnimation("Size", bindSizeAnimation);

            containerVisual.Children.InsertAtBottom(visual);
        }

        ElementCompositionPreview.SetElementChildVisual(shadowElement, containerVisual);
    }
}
