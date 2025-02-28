namespace DevWinUI;
public static partial class CompositionHelper
{
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
