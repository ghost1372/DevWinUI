namespace DevWinUI;

public static partial class DoubleExtensions
{
    extension(double value)
    {
        public float ToFloat() => (float)value;

        public float ToSingle()
        {
            // Double to float conversion can overflow.
            try
            {
                return Convert.ToSingle(value);
            }
            catch (OverflowException ex)
            {
                throw new ArgumentOutOfRangeException("Cannot convert the double value to float!", ex);
            }
        }
    }
}
