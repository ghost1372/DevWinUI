namespace DevWinUI;

internal partial class DigitalSegmentHelper
{
    public const string COLON_TOP_KEY = "ColonTop";
    public const string COLON_BOTTOM_KEY = "ColonBottom";
    public static byte[]? GetCharPattern(string pattern, int columns, int rows)
    {
        if (pattern.Length != rows * columns)
            return null;

        byte[] patternByte = new byte[rows];
        for (int row = 0; row < rows; row++)
        {
            string rowStr = pattern.Substring(row * columns, columns);
            byte rowByte = 0;
            for (int col = 0; col < columns; col++)
            {
                if (rowStr[col] == '1')
                    rowByte |= (byte)(1 << (columns - 1 - col));
            }
            patternByte[row] = rowByte;
        }
        return patternByte;
    }
}
