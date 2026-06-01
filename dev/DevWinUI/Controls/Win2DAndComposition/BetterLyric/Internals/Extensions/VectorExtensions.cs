// https://github.com/jayfunc/BetterLyrics

namespace DevWinUI;

internal static partial class VectorExtensions
{
    extension(Vector2 vector2)
    {
        public Vector2 AddX(float x)
        {
            return new Vector2(vector2.X + x, vector2.Y);
        }
    }
}
