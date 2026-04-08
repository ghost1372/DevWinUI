namespace DevWinUI;

public static partial class ColorExtensions
{
    extension(Color color)
    {
        public Color ChangeAlpha(double alpha)
        {
            return ColorHelper.ChangeAlpha(color, alpha);
        }

        /// <summary>
        /// Finds the best contrasting color (black or white)
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public Color ContrastColorBlackWhite()
        {
            return ColorHelper.ContrastColorBlackWhite(color);
        }

        public Vector3 ToVector3()
        {
            var vector = new Vector3(color.R, color.G, color.B);
            return vector / 255;
        }

        internal double ContrastRatio(Color color2)
        {
            double luminance1 = color.RelativeLuminance();
            double luminance2 = color2.RelativeLuminance();

            double lighter = Math.Max(luminance1, luminance2);
            double darker = Math.Min(luminance1, luminance2);

            return (lighter + 0.05) / (darker + 0.05);
        }

        internal double RelativeLuminance()
        {
            float sRGBtoRGB(float s)
            {
                if (s <= 0.03928f)
                    return s / 12.92f;

                return MathF.Pow(((s + 0.055f) / 1.055f), 2.4f);
            }

            var vec = color.ToVector3();
            var r = sRGBtoRGB(vec.X);
            var g = sRGBtoRGB(vec.Y);
            var b = sRGBtoRGB(vec.Z);

            return (0.2126f * r + 0.7152f * g + 0.0722 * b);
        }

        internal float FindColorfulness()
        {
            var vectorColor = color.ToVector3();
            var rg = vectorColor.X - vectorColor.Y;
            var yb = ((vectorColor.X + vectorColor.Y) / 2) - vectorColor.Z;
            return 0.3f * new Vector2(rg, yb).Length();
        }
    }

    extension(Color[] colors)
    {
        internal float FindColorfulness()
        {
            var vectorColors = colors.Select(ToVector3);

            var rg = vectorColors.Select(x => Math.Abs(x.X - x.Y));
            var yb = vectorColors.Select(x => Math.Abs(0.5f * (x.X + x.Y) - x.Z));

            var rg_std = FindStandardDeviation(rg, out var rg_mean);
            var yb_std = FindStandardDeviation(yb, out var yb_mean);

            var std = new Vector2(rg_mean, yb_mean).Length();
            var mean = new Vector2(rg_std, yb_std).Length();

            return std + (0.3f * mean);
        }
    }

    private static float FindStandardDeviation(IEnumerable<float> data, out float avg)
    {
        var average = data.Average();
        avg = average;
        var sumOfSquares = data.Select(x => (x - average) * (x - average)).Sum();
        return (float)Math.Sqrt(sumOfSquares / data.Count());
    }
}
