using System.Numerics;

namespace DevWinUIGallery.Common;

public static partial class FastFourierTransform
{
    /// <summary>
    /// In-place radix-2 FFT (forward).
    /// </summary>
    public static void FFT(Complex[] data, int exponent)
    {
        int n = 1 << exponent;

        // Bit-reversal
        int j = 0;
        for (int i = 0; i < n; i++)
        {
            if (i < j)
            {
                var temp = data[i];
                data[i] = data[j];
                data[j] = temp;
            }

            int m = n >> 1;
            while (j >= m && m >= 2)
            {
                j -= m;
                m >>= 1;
            }
            j += m;
        }

        // FFT stages
        for (int stage = 1; stage <= exponent; stage++)
        {
            int step = 1 << stage;
            int halfStep = step >> 1;

            double angleStep = -2.0 * Math.PI / step;
            Complex phaseStep = new Complex(
                Math.Cos(angleStep),
                Math.Sin(angleStep));

            for (int k = 0; k < n; k += step)
            {
                Complex phase = Complex.One;

                for (int i = 0; i < halfStep; i++)
                {
                    int evenIndex = k + i;
                    int oddIndex = evenIndex + halfStep;

                    Complex even = data[evenIndex];
                    Complex odd = phase * data[oddIndex];

                    data[evenIndex] = even + odd;
                    data[oddIndex] = even - odd;

                    phase *= phaseStep;
                }
            }
        }
    }
}
