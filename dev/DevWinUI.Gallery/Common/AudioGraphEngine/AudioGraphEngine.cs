using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using Microsoft.UI.Dispatching;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Storage;

namespace DevWinUIGallery.Common;

[GeneratedComInterface]
[Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
public unsafe partial interface IMemoryBufferByteAccess
{
    void GetBuffer(out byte* buffer, out uint capacity);
}

public sealed partial class AudioGraphEngine : ISpectrumPlayer, IWaveformPlayer, INotifyPropertyChanged, IDisposable
{
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    private AudioGraph graph;
    private AudioFileInputNode fileNode;
    private AudioDeviceOutputNode deviceNode;
    private AudioFrameOutputNode frameNode;

    private readonly int fftDataSize = 2048;
    private readonly SampleAggregator sampleAggregator;

    private bool isPlaying;
    private double channelLength;
    private double channelPosition;

    public event PropertyChangedEventHandler PropertyChanged;
    private AudioSampleRingBuffer sampleBuffer;
    private CancellationTokenSource processingCts;

    private TimeSpan playbackStartTime;
    public TimeSpan SelectionBegin { get; set; }
    public TimeSpan SelectionEnd { get; set; }
    public AudioGraphEngine()
    {
        sampleAggregator = new SampleAggregator(fftDataSize);

        sampleBuffer = new AudioSampleRingBuffer(fftDataSize * 8);
        processingCts = new CancellationTokenSource();
        StartProcessingLoop();
    }
    private void StartProcessingLoop()
    {
        Task.Run(async () =>
        {
            float left, right;

            while (!processingCts.IsCancellationRequested)
            {
                if (sampleBuffer.TryRead(out left) &&
                    sampleBuffer.TryRead(out right))
                {
                    sampleAggregator.Add(left, right);
                }
                else
                {
                    await Task.Delay(1);
                }
            }
        }, processingCts.Token);
    }

    public bool GetFFTData(float[] fftDataBuffer)
    {
        sampleAggregator.GetFFTResults(fftDataBuffer);
        return IsPlaying;
    }

    public int GetFFTFrequencyIndex(int frequency)
    {
        double maxFrequency = graph != null
            ? graph.EncodingProperties.SampleRate / 2.0
            : 22050;

        return (int)((frequency / maxFrequency) * (fftDataSize / 2));
    }

    public double ChannelPosition
    {
        get => channelPosition;
        set
        {
            if (fileNode == null)
                return;

            value = Math.Max(0, Math.Min(value, ChannelLength));
            fileNode.Seek(TimeSpan.FromSeconds(value));
            channelPosition = value;
            NotifyPropertyChanged(nameof(ChannelPosition));
        }
    }

    public double ChannelLength
    {
        get => channelLength;
        private set
        {
            channelLength = value;
            NotifyPropertyChanged(nameof(ChannelLength));
        }
    }

    private float[] waveformData;

    public float[] WaveformData
    {
        get => waveformData;
        private set
        {
            waveformData = value;
            NotifyPropertyChanged(nameof(WaveformData));
        }
    }
    private async Task GenerateWaveformAsync(string wavPath)
    {
        await Task.Run(() =>
        {
            byte[] data = File.ReadAllBytes(wavPath);

            if (data.Length < 44)
                throw new InvalidOperationException("Invalid WAV file");

            int channels = BitConverter.ToInt16(data, 22);
            int bitsPerSample = BitConverter.ToInt16(data, 34);

            int dataChunkOffset = -1;
            int dataChunkSize = 0;

            for (int i = 12; i < data.Length - 8;)
            {
                string chunkId = Encoding.ASCII.GetString(data, i, 4);
                int chunkSize = BitConverter.ToInt32(data, i + 4);

                if (chunkId == "data")
                {
                    dataChunkOffset = i + 8;
                    dataChunkSize = chunkSize;
                    break;
                }

                i += 8 + chunkSize;
            }

            if (dataChunkOffset < 0)
                throw new InvalidOperationException("WAV data chunk not found");

            const int samplesPerBucket = 1024;

            List<float> waveform = new();

            float leftSumSq = 0, rightSumSq = 0;
            float leftPeak = 0, rightPeak = 0;
            int sampleCounter = 0;

            int bytesPerSample = bitsPerSample / 8;
            int frameSize = bytesPerSample * channels;

            for (int i = dataChunkOffset; i + frameSize <= dataChunkOffset + dataChunkSize; i += frameSize)
            {
                float l, r;

                if (bitsPerSample == 16)
                {
                    l = BitConverter.ToInt16(data, i) / 32768f;
                    r = channels == 2 ? BitConverter.ToInt16(data, i + 2) / 32768f : l;
                }
                else if (bitsPerSample == 24)
                {
                    int left = (data[i + 2] << 24) | (data[i + 1] << 16) | (data[i] << 8);
                    left >>= 8;
                    l = left / 8388608f;

                    if (channels == 2)
                    {
                        int right = (data[i + 5] << 24) | (data[i + 4] << 16) | (data[i + 3] << 8);
                        right >>= 8;
                        r = right / 8388608f;
                    }
                    else
                        r = l;
                }
                else if (bitsPerSample == 32)
                {
                    l = BitConverter.ToSingle(data, i);
                    r = channels == 2 ? BitConverter.ToSingle(data, i + 4) : l;
                }
                else
                {
                    throw new NotSupportedException($"Unsupported WAV bit depth: {bitsPerSample}");
                }

                leftPeak = Math.Max(leftPeak, Math.Abs(l));
                rightPeak = Math.Max(rightPeak, Math.Abs(r));

                leftSumSq += l * l;
                rightSumSq += r * r;
                sampleCounter++;

                if (sampleCounter >= samplesPerBucket)
                {
                    float leftRms = MathF.Sqrt(leftSumSq / sampleCounter);
                    float rightRms = MathF.Sqrt(rightSumSq / sampleCounter);

                    waveform.Add((leftRms + leftPeak) * 0.5f);
                    waveform.Add((rightRms + rightPeak) * 0.5f);

                    leftSumSq = rightSumSq = 0;
                    leftPeak = rightPeak = 0;
                    sampleCounter = 0;
                }
            }

            float max = 0f;
            for (int i = 0; i < waveform.Count; i++)
            {
                float v = Math.Abs(waveform[i]);
                if (v > max)
                    max = v;
            }

            if (max > 0)
            {
                float gain = 1f / max;
                for (int i = 0; i < waveform.Count; i++)
                    waveform[i] *= gain;
            }

            dispatcherQueue.TryEnqueue(() =>
            {
                WaveformData = waveform.ToArray();
            });
        });
    }

    public bool IsPlaying
    {
        get => isPlaying;
        private set
        {
            isPlaying = value;
            NotifyPropertyChanged(nameof(IsPlaying));
        }
    }

    public async Task OpenFileAsync(StorageFile storageFile, bool generateWaveform)
    {
        DisposeGraph();

        if (generateWaveform)
        {
            await GenerateWaveformAsync(storageFile.Path);
        }

        var settings = new AudioGraphSettings(AudioRenderCategory.Media)
        {
            QuantumSizeSelectionMode = QuantumSizeSelectionMode.ClosestToDesired,
            DesiredSamplesPerQuantum = fftDataSize
        };

        var graphResult = await AudioGraph.CreateAsync(settings);
        if (graphResult.Status != AudioGraphCreationStatus.Success)
            throw new InvalidOperationException("AudioGraph creation failed");

        graph = graphResult.Graph;

        var deviceResult = await graph.CreateDeviceOutputNodeAsync();
        deviceNode = deviceResult.DeviceOutputNode;

        var fileResult = await graph.CreateFileInputNodeAsync(storageFile);
        fileNode = fileResult.FileInputNode;

        frameNode = graph.CreateFrameOutputNode();

        fileNode.AddOutgoingConnection(deviceNode);
        fileNode.AddOutgoingConnection(frameNode);

        ChannelLength = fileNode.Duration.TotalSeconds;

        graph.QuantumStarted += OnQuantumStarted;
    }

    public void Play()
    {
        if (graph == null)
            return;

        playbackStartTime = fileNode.Position;
        graph.Start();
        IsPlaying = true;
    }


    public void Pause()
    {
        if (graph == null)
            return;

        graph.Stop();
        IsPlaying = false;
    }

    public void Stop()
    {
        if (graph == null)
            return;

        graph.Stop();
        fileNode.Seek(TimeSpan.Zero);
        ChannelPosition = 0;
        IsPlaying = false;
    }

    private unsafe void OnQuantumStarted(AudioGraph sender, object args)
    {
        var frame = frameNode.GetFrame();
        using var buffer = frame.LockBuffer(AudioBufferAccessMode.Read);
        using var reference = buffer.CreateReference();

        ((IMemoryBufferByteAccess)reference)
            .GetBuffer(out byte* data, out uint capacity);

        float* samples = (float*)data;
        int count = (int)(capacity / sizeof(float));

        for (int i = 0; i < count; i++)
            sampleBuffer.Write(samples[i]);

        double positionSeconds = 0;
        if (fileNode != null && IsPlaying)
            positionSeconds = fileNode.Position.TotalSeconds;

        dispatcherQueue.TryEnqueue(() =>
        {
            channelPosition = positionSeconds;
            NotifyPropertyChanged(nameof(ChannelPosition));
        });
    }

    public void Dispose()
    {
        DisposeGraph();
        GC.SuppressFinalize(this);
    }

    private void DisposeGraph()
    {
        if (graph != null)
        {
            graph.QuantumStarted -= OnQuantumStarted;
            graph.Stop();
            graph.Dispose();
            graph = null;
        }

        fileNode = null;
        deviceNode = null;
        frameNode = null;
    }
    private void NotifyPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
