namespace DevWinUI;

public static partial class VectorExtensions
{
    extension(Vector3 vector)
    {
        internal Color ToColor()
        {
            vector *= 255;
            return Color.FromArgb(255, (byte)(vector.X), (byte)(vector.Y), (byte)(vector.Z));
        }
    }
}
