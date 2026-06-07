using System.Numerics;
using Windows.UI;

namespace DevWinUI;

internal static partial class ColorExtensions
{
    extension(Color color)
    {
        public Vector3 ToVector3RGB()
        {
            return new Vector3((float)color.R / 0xff, (float)color.G / 0xff, (float)color.B / 0xff);
        }
        public Vector4 ToVector4RGBA()
        {
            return new Vector4(color.R / 0xff, color.G / 0xff, color.B / 0xff, color.A / 0xff);
        }

    }
}
