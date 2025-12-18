using System.ComponentModel;
using Microsoft.UI.Dispatching;
using NAudio.Wave;

namespace DevWinUIGallery.Common;

public partial class NAudioEngine : INotifyPropertyChanged, ISpectrumPlayer, IWaveformPlayer, IDisposable
{
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private static NAudioEngine instance;
    private readonly DispatcherTimer positionTimer = new DispatcherTimer();
    private CancellationTokenSource waveformCts;
    private Task waveformTask;
    private readonly int fftDataSize = (int)FFTDataSize.FFT2048;
    private bool disposed;
    private bool canPlay;
    private bool canPause;
    private bool canStop;
    private bool isPlaying;
    private bool inChannelTimerUpdate;
    private double channelLength;
    private double channelPosition;
    private bool inChannelSet;
    private WaveOut waveOutDevice;
    private WaveStream activeStream;
    private WaveChannel32 inputStream;
    private SampleAggregator sampleAggregator;
    private SampleAggregator waveformAggregator;
    private string pendingWaveformPath;
    private float[] fullLevelData;
    private float[] waveformData;
    private TimeSpan repeatStart;
    private TimeSpan repeatStop;
    private bool inRepeatSet;

    private const int waveformCompressedPointCount = 2000;
    private const int repeatThreshold = 200;

    public static NAudioEngine Instance
    {
        get
        {
            if (instance == null)
                instance = new NAudioEngine();
            return instance;
        }
    }
    private NAudioEngine()
    {
        positionTimer.Interval = TimeSpan.FromMilliseconds(50);
        positionTimer.Tick += positionTimer_Tick;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                StopAndCloseStream();
            }

            disposed = true;
        }
    }
    public bool GetFFTData(float[] fftDataBuffer)
    {
        sampleAggregator.GetFFTResults(fftDataBuffer);
        return isPlaying;
    }

    public int GetFFTFrequencyIndex(int frequency)
    {
        double maxFrequency;
        if (ActiveStream != null)
            maxFrequency = ActiveStream.WaveFormat.SampleRate / 2.0d;
        else
            maxFrequency = 22050; // Assume a default 44.1 kHz sample rate.
        return (int)((frequency / maxFrequency) * (fftDataSize / 2));
    }
    public TimeSpan SelectionBegin
    {
        get { return repeatStart; }
        set
        {
            if (!inRepeatSet)
            {
                inRepeatSet = true;
                TimeSpan oldValue = repeatStart;
                repeatStart = value;
                if (oldValue != repeatStart)
                    NotifyPropertyChanged("SelectionBegin");
                inRepeatSet = false;
            }
        }
    }

    public TimeSpan SelectionEnd
    {
        get { return repeatStop; }
        set
        {
            if (!inChannelSet)
            {
                inRepeatSet = true;
                TimeSpan oldValue = repeatStop;
                repeatStop = value;
                if (oldValue != repeatStop)
                    NotifyPropertyChanged("SelectionEnd");
                inRepeatSet = false;
            }
        }
    }

    public float[] WaveformData
    {
        get { return waveformData; }
        protected set
        {
            float[] oldValue = waveformData;
            waveformData = value;
            if (oldValue != waveformData)
                NotifyPropertyChanged("WaveformData");
        }
    }

    public double ChannelLength
    {
        get { return channelLength; }
        protected set
        {
            double oldValue = channelLength;
            channelLength = value;
            if (oldValue != channelLength)
                NotifyPropertyChanged("ChannelLength");
        }
    }

    public double ChannelPosition
    {
        get { return channelPosition; }
        set
        {
            if (!inChannelSet)
            {
                inChannelSet = true; // Avoid recursion
                double oldValue = channelPosition;
                double position = Math.Max(0, Math.Min(value, ChannelLength));
                if (!inChannelTimerUpdate && ActiveStream != null)
                    ActiveStream.Position = (long)((position / ActiveStream.TotalTime.TotalSeconds) * ActiveStream.Length);
                channelPosition = position;
                if (oldValue != channelPosition)
                    NotifyPropertyChanged("ChannelPosition");
                inChannelSet = false;
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(String info)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
    private partial class WaveformGenerationParams
    {
        public WaveformGenerationParams(int points, string path)
        {
            Points = points;
            Path = path;
        }

        public int Points { get; protected set; }
        public string Path { get; protected set; }
    }

    private void GenerateWaveformData(string path)
    {
        // If a task is running, cancel it and queue the new request
        if (waveformTask != null && !waveformTask.IsCompleted)
        {
            pendingWaveformPath = path;
            waveformCts.Cancel();
            return;
        }

        if (waveformCompressedPointCount != 0)
        {
            waveformCts = new CancellationTokenSource();
            waveformTask = GenerateWaveformDataAsync(
                new WaveformGenerationParams(waveformCompressedPointCount, path),
                waveformCts.Token);
        }
    }

    private async Task GenerateWaveformDataAsync(
        WaveformGenerationParams waveformParams,
        CancellationToken token)
    {
        try
        {
            await Task.Run(() =>
            {
                using var waveformMp3Stream = new Mp3FileReader(waveformParams.Path);
                using var waveformInputStream = new WaveChannel32(waveformMp3Stream);

                waveformInputStream.Sample += waveStream_Sample;

                int frameLength = fftDataSize;
                int frameCount = (int)((double)waveformInputStream.Length / frameLength);
                int waveformLength = frameCount * 2;

                byte[] readBuffer = new byte[frameLength];
                waveformAggregator = new SampleAggregator(frameLength);

                float maxLeftPointLevel = float.MinValue;
                float maxRightPointLevel = float.MinValue;

                int currentPointIndex = 0;
                int readCount = 0;

                float[] waveformCompressedPoints = new float[waveformParams.Points];
                List<float> waveformData = new();
                List<int> waveMaxPointIndexes = new();

                for (int i = 1; i <= waveformParams.Points; i++)
                {
                    waveMaxPointIndexes.Add(
                        (int)Math.Round(
                            waveformLength * ((double)i / waveformParams.Points), 0));
                }

                while (currentPointIndex * 2 < waveformParams.Points)
                {
                    token.ThrowIfCancellationRequested();

                    waveformInputStream.ReadExactly(readBuffer);

                    waveformData.Add(waveformAggregator.LeftMaxVolume);
                    waveformData.Add(waveformAggregator.RightMaxVolume);

                    maxLeftPointLevel = Math.Max(maxLeftPointLevel, waveformAggregator.LeftMaxVolume);
                    maxRightPointLevel = Math.Max(maxRightPointLevel, waveformAggregator.RightMaxVolume);

                    if (readCount > waveMaxPointIndexes[currentPointIndex])
                    {
                        waveformCompressedPoints[currentPointIndex * 2] = maxLeftPointLevel;
                        waveformCompressedPoints[currentPointIndex * 2 + 1] = maxRightPointLevel;

                        maxLeftPointLevel = float.MinValue;
                        maxRightPointLevel = float.MinValue;
                        currentPointIndex++;
                    }

                    if (readCount % 3000 == 0)
                    {
                        var cloned = (float[])waveformCompressedPoints.Clone();
                        dispatcherQueue.TryEnqueue(() =>
                        {
                            WaveformData = cloned;
                        });
                    }

                    readCount++;
                }

                var finalClone = (float[])waveformCompressedPoints.Clone();
                dispatcherQueue.TryEnqueue(() =>
                {
                    fullLevelData = waveformData.ToArray();
                    WaveformData = finalClone;
                });
            }, token);
        }
        catch (OperationCanceledException)
        {
            // Expected on cancellation
        }
        finally
        {
            // If cancellation occurred and a new path is pending, restart
            if (!string.IsNullOrEmpty(pendingWaveformPath) &&
                waveformCompressedPointCount != 0)
            {
                string nextPath = pendingWaveformPath;
                pendingWaveformPath = null;

                waveformCts = new CancellationTokenSource();
                waveformTask = GenerateWaveformDataAsync(
                    new WaveformGenerationParams(waveformCompressedPointCount, nextPath),
                    waveformCts.Token);
            }
        }
    }
    private void StopAndCloseStream()
    {
        if (waveOutDevice != null)
        {
            waveOutDevice.Stop();
        }
        if (activeStream != null)
        {
            inputStream.Close();
            inputStream = null;
            ActiveStream.Close();
            ActiveStream = null;
        }
        if (waveOutDevice != null)
        {
            waveOutDevice.Dispose();
            waveOutDevice = null;
        }
    }
    public void Stop()
    {
        if (waveOutDevice != null)
        {
            waveOutDevice.Stop();
        }
        IsPlaying = false;
        CanStop = false;
        CanPlay = true;
        CanPause = false;
    }

    public void Pause()
    {
        if (IsPlaying && CanPause)
        {
            waveOutDevice.Pause();
            IsPlaying = false;
            CanPlay = true;
            CanPause = false;
        }
    }

    public void Play()
    {
        if (CanPlay)
        {
            waveOutDevice.Play();
            IsPlaying = true;
            CanPause = true;
            CanPlay = false;
            CanStop = true;
        }
    }

    public void OpenFile(string path)
    {
        Stop();

        if (ActiveStream != null)
        {
            SelectionBegin = TimeSpan.Zero;
            SelectionEnd = TimeSpan.Zero;
            ChannelPosition = 0;
        }

        StopAndCloseStream();

        if (System.IO.File.Exists(path))
        {
            try
            {
                waveOutDevice = new WaveOut()
                {
                    DesiredLatency = 100
                };
                ActiveStream = new Mp3FileReader(path);
                inputStream = new WaveChannel32(ActiveStream);
                sampleAggregator = new SampleAggregator(fftDataSize);
                inputStream.Sample += inputStream_Sample;
                waveOutDevice.Init(inputStream);
                ChannelLength = inputStream.TotalTime.TotalSeconds;
                GenerateWaveformData(path);
                CanPlay = true;
            }
            catch
            {
                ActiveStream = null;
                CanPlay = false;
            }
        }
    }
    public WaveStream ActiveStream
    {
        get { return activeStream; }
        protected set
        {
            WaveStream oldValue = activeStream;
            activeStream = value;
            if (oldValue != activeStream)
                NotifyPropertyChanged("ActiveStream");
        }
    }

    public bool CanPlay
    {
        get { return canPlay; }
        protected set
        {
            bool oldValue = canPlay;
            canPlay = value;
            if (oldValue != canPlay)
                NotifyPropertyChanged("CanPlay");
        }
    }

    public bool CanPause
    {
        get { return canPause; }
        protected set
        {
            bool oldValue = canPause;
            canPause = value;
            if (oldValue != canPause)
                NotifyPropertyChanged("CanPause");
        }
    }

    public bool CanStop
    {
        get { return canStop; }
        protected set
        {
            bool oldValue = canStop;
            canStop = value;
            if (oldValue != canStop)
                NotifyPropertyChanged("CanStop");
        }
    }


    public bool IsPlaying
    {
        get { return isPlaying; }
        protected set
        {
            bool oldValue = isPlaying;
            isPlaying = value;
            if (oldValue != isPlaying)
                NotifyPropertyChanged("IsPlaying");

            if (isPlaying)
                positionTimer.Start();
            else
                positionTimer.Stop();
        }
    }
    private void inputStream_Sample(object sender, SampleEventArgs e)
    {
        sampleAggregator.Add(e.Left, e.Right);
        long repeatStartPosition = (long)((SelectionBegin.TotalSeconds / ActiveStream.TotalTime.TotalSeconds) * ActiveStream.Length);
        long repeatStopPosition = (long)((SelectionEnd.TotalSeconds / ActiveStream.TotalTime.TotalSeconds) * ActiveStream.Length);
        if (((SelectionEnd - SelectionBegin) >= TimeSpan.FromMilliseconds(repeatThreshold)) && ActiveStream.Position >= repeatStopPosition)
        {
            sampleAggregator.Clear();
            ActiveStream.Position = repeatStartPosition;
        }
    }

    void waveStream_Sample(object sender, SampleEventArgs e)
    {
        waveformAggregator.Add(e.Left, e.Right);
    }

    void positionTimer_Tick(object sender, object e)
    {
        inChannelTimerUpdate = true;
        ChannelPosition = ((double)ActiveStream.Position / (double)ActiveStream.Length) * ActiveStream.TotalTime.TotalSeconds;
        inChannelTimerUpdate = false;
    }
}
