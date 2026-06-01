// https://github.com/jayfunc/BetterLyrics

namespace DevWinUI;

internal static partial class RectExtensions
{
    extension(Rect rect)
    {
        public Rect AddX(double x) => new(
            rect.X + x,
            rect.Y,
            rect.Width,
            rect.Height
        );

        public Rect AddY(double y) => new(
            rect.X,
            rect.Y + y,
            rect.Width,
            rect.Height
        );

        public Rect Extend(double left, double top, double right, double bottom) => new(
            rect.X - left,
            rect.Y - top,
            rect.Width + left + right,
            rect.Height + top + bottom
        );

        public Rect Extend(double padding) => Extend(rect, padding, padding, padding, padding);
        public Rect Extend(double horizontalPadding, double verticalPadding) => Extend(rect, horizontalPadding, verticalPadding, horizontalPadding, verticalPadding);

        public Rect Scale(double scale)
        {
            double originalWidth = rect.Width;
            double originalHeight = rect.Height;

            double scaledWidth = originalWidth * scale;
            double scaledHeight = originalHeight * scale;

            double scaleOffsetX = (scaledWidth - originalWidth) / 2;
            double scaleOffsetY = (scaledHeight - originalHeight) / 2;

            return new Rect(
                rect.X - scaleOffsetX,
                rect.Y - scaleOffsetY,
                scaledWidth,
                scaledHeight
            );
        }
    }
}
