using System.Numerics;

namespace DevWinUIGallery.Common;

public sealed partial class SampleAggregator
{
    private readonly int bufferSize;
    private readonly int fftExponent;

    private readonly Complex[] channelData;
    private int channelDataPosition;

    private float volumeLeftMaxValue;
    private float volumeLeftMinValue;
    private float volumeRightMaxValue;
    private float volumeRightMinValue;

    public SampleAggregator(int bufferSize)
    {
        if ((bufferSize & (bufferSize - 1)) != 0)
            throw new ArgumentException("FFT buffer size must be a power of two");

        this.bufferSize = bufferSize;
        fftExponent = (int)Math.Log(bufferSize, 2);
        channelData = new Complex[bufferSize];

        Clear();
    }

    public void Clear()
    {
        volumeLeftMaxValue = float.MinValue;
        volumeRightMaxValue = float.MinValue;
        volumeLeftMinValue = float.MaxValue;
        volumeRightMinValue = float.MaxValue;
        channelDataPosition = 0;
    }

    /// <summary>
    /// Adds a stereo sample.
    /// </summary>
    public void Add(float leftValue, float rightValue)
    {
        if (channelDataPosition == 0)
        {
            volumeLeftMaxValue = float.MinValue;
            volumeRightMaxValue = float.MinValue;
            volumeLeftMinValue = float.MaxValue;
            volumeRightMinValue = float.MaxValue;
        }

        // Convert stereo → mono (average)
        float mono = (leftValue + rightValue) * 0.5f;

        channelData[channelDataPosition] = new Complex(mono, 0);
        channelDataPosition++;

        volumeLeftMaxValue = Math.Max(volumeLeftMaxValue, leftValue);
        volumeLeftMinValue = Math.Min(volumeLeftMinValue, leftValue);
        volumeRightMaxValue = Math.Max(volumeRightMaxValue, rightValue);
        volumeRightMinValue = Math.Min(volumeRightMinValue, rightValue);

        if (channelDataPosition >= bufferSize)
            channelDataPosition = 0;
    }

    /// <summary>
    /// Copies FFT magnitude data into the provided buffer.
    /// Buffer length must be >= bufferSize / 2.
    /// </summary>
    public void GetFFTResults(float[] fftBuffer)
    {
        if (fftBuffer == null)
            throw new ArgumentNullException(nameof(fftBuffer));

        if (fftBuffer.Length < bufferSize / 2)
            throw new ArgumentException("FFT buffer is too small");

        // Clone so capture thread is not disturbed
        var fftData = new Complex[bufferSize];
        Array.Copy(channelData, fftData, bufferSize);

        FastFourierTransform.FFT(fftData, fftExponent);

        for (int i = 0; i < bufferSize / 2; i++)
        {
            double real = fftData[i].Real;
            double imag = fftData[i].Imaginary;

            fftBuffer[i] = (float)Math.Sqrt(real * real + imag * imag);
        }
    }

    public float LeftMaxVolume => volumeLeftMaxValue;
    public float LeftMinVolume => volumeLeftMinValue;
    public float RightMaxVolume => volumeRightMaxValue;
    public float RightMinVolume => volumeRightMinValue;
}
