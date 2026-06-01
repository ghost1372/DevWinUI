namespace DevWinUI;

public partial class TextZoomEffect : ITextEffect
{
    public TimeSpan AnimationDuration { get; set; } = TimeSpan.FromMilliseconds(800);

    public TimeSpan DelayPerCluster { get; set; } = TimeSpan.FromMilliseconds(10);

    public void Update(string oldText,
        string newText,
        List<TextDiffResult> diffResults,
        CanvasTextLayout oldTextLayout,
        CanvasTextLayout newTextLayout,
        AnimatedTextBlockRedrawState state,
        ICanvasAnimatedControl canvas,
        CanvasAnimatedUpdateEventArgs args)
    {

    }

    public void DrawText(string oldText,
        string newText,
        List<TextDiffResult> diffResults,
        CanvasTextLayout oldTextLayout,
        CanvasTextLayout newTextLayout,
        CanvasTextFormat textFormat,
        Color textColor,
        CanvasLinearGradientBrush gradientBrush,
        AnimatedTextBlockRedrawState state,
        CanvasDrawingSession drawingSession,
        CanvasAnimatedDrawEventArgs args)
    {
        if (diffResults == null)
            return;

        var ds = args.DrawingSession;

        if (state == AnimatedTextBlockRedrawState.Idle)
        {
            DrawIdle(ds,
                oldTextLayout,
                newTextLayout,
                textFormat,
                textColor,
                gradientBrush);

            return;
        }

        for (int i = 0; i < diffResults.Count; i++)
        {
            var diffResult = diffResults[i];

            switch (diffResult.Type)
            {
                case AnimatedTextBlockDiffOperationType.Insert:
                    DrawInsert(ds,
                        diffResult.OldGlyphCluster,
                        diffResult.NewGlyphCluster,
                        oldTextLayout,
                        newTextLayout,
                        textFormat,
                        textColor,
                        gradientBrush);
                    break;
                case AnimatedTextBlockDiffOperationType.Remove:
                    DrawRemove(ds,
                        diffResult.OldGlyphCluster,
                        diffResult.NewGlyphCluster,
                        oldTextLayout,
                        newTextLayout,
                        textFormat,
                        textColor,
                        gradientBrush);
                    break;
                case AnimatedTextBlockDiffOperationType.Stay:
                case AnimatedTextBlockDiffOperationType.Move:
                    DrawMove(ds,
                        diffResult.OldGlyphCluster,
                        diffResult.NewGlyphCluster,
                        oldTextLayout,
                        newTextLayout,
                        textFormat,
                        textColor,
                        gradientBrush);
                    break;
                case AnimatedTextBlockDiffOperationType.Update:
                    DrawUpdate(ds,
                        diffResult.OldGlyphCluster,
                        diffResult.NewGlyphCluster,
                        oldTextLayout,
                        newTextLayout,
                        textFormat,
                        textColor,
                        gradientBrush);
                    break;
            }
        }
    }

    private void DrawIdle(CanvasDrawingSession ds,
        CanvasTextLayout oldTextLayout,
        CanvasTextLayout newTextLayout,
        CanvasTextFormat textFormat,
        Color textColor,
        CanvasLinearGradientBrush gradientBrush)
    {
        ds.Transform = Matrix3x2.Identity;
        ds.DrawTextLayout(newTextLayout, 0, 0, textColor);
    }

    private void DrawInsert(CanvasDrawingSession ds,
        GraphemeCluster oldCluster,
        GraphemeCluster newCluster,
        CanvasTextLayout oldTextLayout,
        CanvasTextLayout newTextLayout,
        CanvasTextFormat textFormat,
        Color textColor,
        CanvasLinearGradientBrush gradientBrush)
    {
        if (newCluster == null)
        {
            return;
        }

        float opacityProgress = (float)AnimationEasingHelper.Ease<float>(newCluster.Progress, AnimationEaseMode.Out, AnimationEasingHelper.EaseInElastic);
        float zoomProgress = (float)AnimationEasingHelper.Ease<float>(newCluster.Progress, AnimationEaseMode.Out, AnimationEasingHelper.EaseInElastic);
        using (ds.CreateLayer(opacityProgress))
        {
            ds.Transform = Matrix3x2.CreateScale(zoomProgress,
            new Vector2((float)(newCluster.LayoutBounds.X +
                                newCluster.LayoutBounds.Width * 0.5),
                                (float)(newCluster.LayoutBounds.Y +
                                newCluster.LayoutBounds.Height * 0.5)));

            ds.DrawText(
                newCluster.IsTrimmed
                    ? newTextLayout.GenerateTrimmingSign()
                    : newCluster.Characters,
                (float)newCluster.DrawBounds.X,
                (float)newCluster.DrawBounds.Y,
                textColor,
                textFormat);

            ds.Transform = Matrix3x2.Identity;
        }
    }

    private void DrawMove(CanvasDrawingSession ds,
        GraphemeCluster oldCluster,
        GraphemeCluster newCluster,
        CanvasTextLayout oldTextLayout,
        CanvasTextLayout newTextLayout,
        CanvasTextFormat textFormat,
        Color textColor,
        CanvasLinearGradientBrush gradientBrush)
    {
        if (oldCluster == null || newCluster == null)
        {
            return;
        }

        float oldProgress = (float)AnimationEasingHelper.Ease<float>(oldCluster.Progress, AnimationEaseMode.Out, AnimationEasingHelper.EaseInElastic);

        var oX = oldCluster.DrawBounds.X;
        var oY = oldCluster.DrawBounds.Y;
        var nX = newCluster.DrawBounds.X;
        var nY = newCluster.DrawBounds.Y;

        var dX = nX - oX;
        var dY = nY - oY;

        ds.DrawText(
            oldCluster.IsTrimmed
                ? oldTextLayout.GenerateTrimmingSign()
                : oldCluster.Characters,
            (float)(oX + dX * oldProgress),
            (float)(oY + dY * oldProgress),
            textColor,
            textFormat);
    }

    private void DrawUpdate(CanvasDrawingSession ds,
        GraphemeCluster oldCluster,
        GraphemeCluster newCluster,
        CanvasTextLayout oldTextLayout,
        CanvasTextLayout newTextLayout,
        CanvasTextFormat textFormat,
        Color textColor,
        CanvasLinearGradientBrush gradientBrush)
    {
        if (oldCluster == null || newCluster == null)
        {
            return;
        }

        float oldOpacityProgress = (float)AnimationEasingHelper.Ease<float>(1.0f - oldCluster.Progress, AnimationEaseMode.In, AnimationEasingHelper.EaseInElastic);
        float oldZoomProgress = (float)AnimationEasingHelper.Ease<float>(1.0f - oldCluster.Progress, AnimationEaseMode.In, AnimationEasingHelper.EaseInElastic);
        float newOpacityProgress = (float)AnimationEasingHelper.Ease<float>(newCluster.Progress, AnimationEaseMode.Out, AnimationEasingHelper.EaseInElastic);
        float newZoomProgress = (float)AnimationEasingHelper.Ease<float>(newCluster.Progress, AnimationEaseMode.Out, AnimationEasingHelper.EaseInElastic);

        using (ds.CreateLayer(oldOpacityProgress))
        {
            ds.Transform = Matrix3x2.CreateScale(oldZoomProgress,
                new Vector2((float)(oldCluster.LayoutBounds.X +
                                    oldCluster.LayoutBounds.Width * 0.5),
                    (float)(oldCluster.LayoutBounds.Y +
                            oldCluster.LayoutBounds.Height * 0.5)));

            ds.DrawText(
                oldCluster.IsTrimmed
                    ? oldTextLayout.GenerateTrimmingSign()
                    : oldCluster.Characters,
                (float)oldCluster.DrawBounds.X,
                (float)oldCluster.DrawBounds.Y,
                textColor,
                textFormat);

            ds.Transform = Matrix3x2.Identity;
        }

        using (ds.CreateLayer(newOpacityProgress))
        {
            ds.Transform = Matrix3x2.CreateScale(newZoomProgress,
                new Vector2((float)(newCluster.LayoutBounds.X +
                                    newCluster.LayoutBounds.Width * 0.5),
                    (float)(newCluster.LayoutBounds.Y +
                            newCluster.LayoutBounds.Height * 0.5)));

            ds.DrawText(
                newCluster.IsTrimmed
                    ? newTextLayout.GenerateTrimmingSign()
                    : newCluster.Characters,
                (float)newCluster.DrawBounds.X,
                (float)newCluster.DrawBounds.Y,
                textColor,
                textFormat);

            ds.Transform = Matrix3x2.Identity;
        }
    }

    private void DrawRemove(CanvasDrawingSession ds,
        GraphemeCluster oldCluster,
        GraphemeCluster newCluster,
        CanvasTextLayout oldTextLayout,
        CanvasTextLayout newTextLayout,
        CanvasTextFormat textFormat,
        Color textColor,
        CanvasLinearGradientBrush gradientBrush)
    {
        if (oldCluster == null)
        {
            return;
        }

        float opacityProgress = (float)AnimationEasingHelper.Ease<float>(1.0f - oldCluster.Progress, AnimationEaseMode.In, AnimationEasingHelper.EaseInElastic);
        float zoomProgress = (float)AnimationEasingHelper.Ease<float>(1.0f - oldCluster.Progress, AnimationEaseMode.In, AnimationEasingHelper.EaseInElastic);
        using (ds.CreateLayer(opacityProgress))
        {
            ds.Transform = Matrix3x2.CreateScale(zoomProgress,
                new Vector2((float)(oldCluster.LayoutBounds.X +
                                    oldCluster.LayoutBounds.Width * 0.5),
                    (float)(oldCluster.LayoutBounds.Y +
                            oldCluster.LayoutBounds.Height * 0.5)));
            ds.DrawText(
                oldCluster.IsTrimmed
                    ? oldTextLayout.GenerateTrimmingSign()
                    : oldCluster.Characters,
                (float)oldCluster.DrawBounds.X,
                (float)oldCluster.DrawBounds.Y,
                textColor,
                textFormat);

            ds.Transform = Matrix3x2.Identity;
        }
    }
}
