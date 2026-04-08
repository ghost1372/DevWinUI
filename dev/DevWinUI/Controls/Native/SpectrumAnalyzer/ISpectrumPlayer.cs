namespace DevWinUI;

public interface ISpectrumPlayer : ISoundPlayer
{
    /// <summary>
    /// Assigns current FFT data to a buffer.
    /// </summary>
    /// <remarks>
    /// The FFT data in the buffer should consist only of the real number intensity values. This means that if your FFT algorithm returns
    /// complex numbers (as many do), you'd run an algorithm similar to:
    /// for(int i = 0; i &lt; complexNumbers.Length / 2; i++)
    ///     fftResult[i] = Math.Sqrt(complexNumbers[i].Real * complexNumbers[i].Real + complexNumbers[i].Imaginary * complexNumbers[i].Imaginary);
    /// </remarks>
    /// <param name="fftDataBuffer">The buffer to copy FFT data. The buffer should consist of only non-imaginary numbers.</param>
    /// <returns>True if data was written to the buffer, otherwise false.</returns>
    bool GetFFTData(float[] fftDataBuffer);

    /// <summary>
    /// Gets the index in the FFT data buffer for a given frequency.
    /// </summary>
    /// <param name="frequency">The frequency for which to obtain a buffer index</param>
    /// <returns>An index in the FFT data buffer</returns>
    int GetFFTFrequencyIndex(int frequency);
}
