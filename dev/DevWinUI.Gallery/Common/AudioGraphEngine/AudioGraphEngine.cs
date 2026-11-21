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

    private AudioGraph? graph;
    private AudioFileInputNode? fileNode;
    private AudioDeviceOutputNode? deviceNode;
    private AudioFrameOutputNode? frameNode;

    private readonly int fftDataSize = 2048;
    private readonly SampleAggregator sampleAggregator;

    private double channelPosition;

    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly AudioSampleRingBuffer sampleBuffer;
    private readonly CancellationTokenSource processingCts;

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
            while (!processingCts.IsCancellationRequested)
            {
                if (sampleBuffer.TryRead(out var left) &&
                    sampleBuffer.TryRead(out var right))
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
        var maxFrequency = graph != null
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
        get;
        private set
        {
            field = value;
            NotifyPropertyChanged(nameof(ChannelLength));
        }
    }

    public float[] WaveformData
    {
        get;
        private set
        {
            field = value;
            NotifyPropertyChanged(nameof(WaveformData));
        }
    } = [];
    private async Task GenerateWaveformAsync(string wavPath)
    {
        await Task.Run(() =>
        {
            var data = File.ReadAllBytes(wavPath);

            if (data.Length < 44)
                throw new InvalidOperationException("Invalid WAV file");

            int channels = BitConverter.ToInt16(data, 22);
            int bitsPerSample = BitConverter.ToInt16(data, 34);

            var dataChunkOffset = -1;
            var dataChunkSize = 0;

            for (var i = 12; i < data.Length - 8;)
            {
                var chunkId = Encoding.ASCII.GetString(data, i, 4);
                var chunkSize = BitConverter.ToInt32(data, i + 4);

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

            List<float> waveform = [];

            float leftSumSq = 0, rightSumSq = 0;
            float leftPeak = 0, rightPeak = 0;
            var sampleCounter = 0;

            var bytesPerSample = bitsPerSample / 8;
            var frameSize = bytesPerSample * channels;

            for (var i = dataChunkOffset; i + frameSize <= dataChunkOffset + dataChunkSize; i += frameSize)
            {
                float l, r;

                if (bitsPerSample == 16)
                {
                    l = BitConverter.ToInt16(data, i) / 32768f;
                    r = channels == 2 ? BitConverter.ToInt16(data, i + 2) / 32768f : l;
                }
                else if (bitsPerSample == 24)
                {
                    var left = (data[i + 2] << 24) | (data[i + 1] << 16) | (data[i] << 8);
                    left >>= 8;
                    l = left / 8388608f;

                    if (channels == 2)
                    {
                        var right = (data[i + 5] << 24) | (data[i + 4] << 16) | (data[i + 3] << 8);
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
                    var leftRms = MathF.Sqrt(leftSumSq / sampleCounter);
                    var rightRms = MathF.Sqrt(rightSumSq / sampleCounter);

                    waveform.Add((leftRms + leftPeak) * 0.5f);
                    waveform.Add((rightRms + rightPeak) * 0.5f);

                    leftSumSq = rightSumSq = 0;
                    leftPeak = rightPeak = 0;
                    sampleCounter = 0;
                }
            }

            var max = 0f;
            for (var i = 0; i < waveform.Count; i++)
            {
                var v = Math.Abs(waveform[i]);
                if (v > max)
                    max = v;
            }

            if (max > 0)
            {
                var gain = 1f / max;
                for (var i = 0; i < waveform.Count; i++)
                    waveform[i] *= gain;
            }

            dispatcherQueue.TryEnqueue(() =>
            {
                WaveformData = [.. waveform];
            });
        });
    }

    public bool IsPlaying
    {
        get;
        private set
        {
            field = value;
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

        var newGraph = graphResult.Graph;
        graph = newGraph;

        var deviceResult = await newGraph.CreateDeviceOutputNodeAsync();
        var newDeviceNode = deviceResult.DeviceOutputNode;
        deviceNode = newDeviceNode;

        var fileResult = await newGraph.CreateFileInputNodeAsync(storageFile);
        var newFileNode = fileResult.FileInputNode;
        fileNode = newFileNode;

        var newFrameNode = newGraph.CreateFrameOutputNode();
        frameNode = newFrameNode;

        newFileNode.AddOutgoingConnection(newDeviceNode);
        newFileNode.AddOutgoingConnection(newFrameNode);

        ChannelLength = newFileNode.Duration.TotalSeconds;

        newGraph.QuantumStarted += OnQuantumStarted;
    }

    public void Play()
    {
        if (graph == null || fileNode == null)
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
        if (graph == null || fileNode == null)
            return;

        graph.Stop();
        fileNode.Seek(TimeSpan.Zero);
        ChannelPosition = 0;
        IsPlaying = false;
    }

    private unsafe void OnQuantumStarted(AudioGraph sender, object args)
    {
        var output = frameNode;
        if (output == null)
            return;

        var frame = output.GetFrame();
        using var buffer = frame.LockBuffer(AudioBufferAccessMode.Read);
        using var reference = buffer.CreateReference();

        ((IMemoryBufferByteAccess)reference)
            .GetBuffer(out var data, out var capacity);

        var samples = (float*)data;
        var count = (int)(capacity / sizeof(float));

        for (var i = 0; i < count; i++)
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
