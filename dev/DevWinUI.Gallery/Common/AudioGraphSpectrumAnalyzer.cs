using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.Capture;
using Windows.Media.Render;

namespace DevWinUIGallery.Common;

public sealed partial class AudioGraphSpectrumAnalyzer : ISpectrumAnalyzer, IDisposable
{
    public event Action<float[]> SpectrumDataUpdated;

    private AudioGraph _graph;
    private AudioDeviceInputNode _input;
    private AudioFrameOutputNode _output;

    private const int FftLength = 2048;

    private readonly float[] _left = new float[FftLength];
    private readonly float[] _right = new float[FftLength];
    private readonly Complex[] _fftL = new Complex[FftLength];
    private readonly Complex[] _fftR = new Complex[FftLength];

    private float[] _window;

    private int _sampleRate;
    private bool _running;

    public int Gain { get; set; } = 1;

    private float[] _bufferA;
    private float[] _bufferB;

    private float[] _writeBuffer;
    private float[] _readBuffer;

    private readonly object _swapLock = new();

    /// <summary>
    /// Always safe snapshot for UI (Canvas Draw/Update polling).
    /// Never modified while being read.
    /// </summary>
    public float[] LatestSpectrum => _readBuffer;

    public void Start()
    {
        _ = StartInternalAsync();
    }

    public void Stop()
    {
        _running = false;
        _graph?.Stop();
    }

    public void Dispose()
    {
        Stop();
        _graph?.Dispose();
    }

    [GeneratedComInterface]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    public unsafe partial interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }

    private async Task StartInternalAsync()
    {
        if (_running)
            return;

        _window = new float[FftLength];

        for (int i = 0; i < FftLength; i++)
            _window[i] = (float)(0.54 - 0.46 * Math.Cos(2 * Math.PI * i / (FftLength - 1)));

        var settings = new AudioGraphSettings(AudioRenderCategory.Media)
        {
            QuantumSizeSelectionMode = QuantumSizeSelectionMode.ClosestToDesired,
            DesiredSamplesPerQuantum = 1024
        };

        var graphResult = await AudioGraph.CreateAsync(settings);
        if (graphResult.Status != AudioGraphCreationStatus.Success)
            return;

        _graph = graphResult.Graph;
        _sampleRate = (int)_graph.EncodingProperties.SampleRate;

        var inputResult = await _graph.CreateDeviceInputNodeAsync(
            MediaCategory.Other,
            _graph.EncodingProperties);

        if (inputResult.Status != AudioDeviceNodeCreationStatus.Success)
            return;

        _input = inputResult.DeviceInputNode;

        _output = _graph.CreateFrameOutputNode();
        _input.AddOutgoingConnection(_output);

        _graph.QuantumProcessed += OnQuantumProcessed;

        _running = true;
        _graph.Start();
    }

    private unsafe void OnQuantumProcessed(AudioGraph sender, object args)
    {
        if (!_running)
            return;

        using var frame = _output.GetFrame();
        using var buffer = frame.LockBuffer(AudioBufferAccessMode.Read);
        using var reference = buffer.CreateReference();

        ((IMemoryBufferByteAccess)reference).GetBuffer(out byte* data, out uint capacity);

        int floatCount = (int)(capacity / sizeof(float));
        if (floatCount < FftLength * 2)
            return;

        float* samples = (float*)data;

        for (int i = 0; i < FftLength; i++)
        {
            _left[i] = samples[i * 2];
            _right[i] = samples[i * 2 + 1];
        }

        for (int i = 0; i < FftLength; i++)
        {
            float w = (float)_window[i];
            _fftL[i] = new Complex(_left[i] * w, 0);
            _fftR[i] = new Complex(_right[i] * w, 0);
        }

        FFT(_fftL);
        FFT(_fftR);

        int bins = FftLength / 2;

        EnsureBuffers(bins);

        var write = _writeBuffer;

        for (int i = 0; i < bins; i++)
        {
            float freq = i * _sampleRate / (float)FftLength;
            float gain = Compensation(freq) * Gain;

            write[i] = (float)_fftL[i].Magnitude * gain;
            write[i + bins] = (float)_fftR[i].Magnitude * gain;
        }

        // swap buffers atomically
        lock (_swapLock)
        {
            (_readBuffer, _writeBuffer) = (_writeBuffer, _readBuffer);
        }

        // event AFTER swap (safe snapshot)
        SpectrumDataUpdated?.Invoke(_readBuffer);
    }

    private void EnsureBuffers(int size)
    {
        int len = size * 2;

        if (_bufferA == null || _bufferA.Length != len)
        {
            _bufferA = new float[len];
            _bufferB = new float[len];

            _writeBuffer = _bufferA;
            _readBuffer = _bufferB;
        }
    }

    private static void FFT(Complex[] buffer)
    {
        int n = buffer.Length;
        int bits = (int)Math.Log2(n);

        for (int j = 1, i = 0; j < n; j++)
        {
            int bit = n >> 1;
            for (; (i & bit) != 0; bit >>= 1)
                i &= ~bit;

            i |= bit;

            if (j < i)
                (buffer[j], buffer[i]) = (buffer[i], buffer[j]);
        }

        for (int len = 2; len <= n; len <<= 1)
        {
            double ang = -2 * Math.PI / len;
            Complex wlen = new(Math.Cos(ang), Math.Sin(ang));

            for (int i = 0; i < n; i += len)
            {
                Complex w = Complex.One;

                for (int j = 0; j < len / 2; j++)
                {
                    var u = buffer[i + j];
                    var v = buffer[i + j + len / 2] * w;

                    buffer[i + j] = u + v;
                    buffer[i + j + len / 2] = u - v;

                    w *= wlen;
                }
            }
        }
    }

    private static float Compensation(float freq)
    {
        float[] f = { 20, 50, 100, 200, 500, 1000, 2000, 4000, 8000, 16000, 20000 };
        float[] g = { 0.5f, 0.3f, 0.4f, 0.6f, 0.8f, 1.0f, 1.2f, 1.3f, 1.1f, 0.9f, 0.8f };

        if (freq <= f[0]) return g[0];
        if (freq >= f[^1]) return g[^1];

        int i = 0;
        while (freq > f[i + 1]) i++;

        float t = (freq - f[i]) / (f[i + 1] - f[i]);
        return g[i] + t * (g[i + 1] - g[i]);
    }
}
