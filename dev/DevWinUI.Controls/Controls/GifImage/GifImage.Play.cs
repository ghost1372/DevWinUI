using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace DevWinUI;
public partial class GifImage
{
    private DispatcherTimer _timer = null;

    private int _index = 0;
    List<BitmapFrame> _frames = null;

    private int _width;
    private int _height;
    private byte[] _pixels = null;

    private void InitializeTimer()
    {
        _timer = new DispatcherTimer();
        _timer.Tick += OnTimerTick;
    }

    private async void OnTimerTick(object sender, object e)
    {
        if (_frames != null && _frames.Count > 0)
        {
            await PlayFrameAsync();

            _index = (_index + 1) % _frames.Count;

            if (_index == 0 && !this.IsLooping)
            {
                this.Stop();
            }
        }
        else
        {
            this.Stop();
        }
    }

    private async Task PlayFrameAsync()
    {
        if (_frames != null && _frames.Count > 0)
        {
            var frame = _frames[_index];
            var props = await GetFramePropertiesAsync(frame);
            _timer.Interval = TimeSpan.FromMilliseconds(props.DelayMilliseconds);

            if (_index == 0 || props.ShouldDispose)
            {
                _width = (int)props.Rect.Width;
                _height = (int)props.Rect.Height;
                _pixels = await GetPixelsAsync(frame);
            }
            else
            {
                var pixels = await GetPixelsAsync(frame);
                MergePixels(_pixels, _width, pixels, props.Rect);
            }

            _image.Source = LoadImage(_pixels, _width, _height);
        }
    }

    private async Task ProcessSourceAsync(Uri uri)
    {
        try
        {
            if (uri.IsAbsoluteUri && (uri.Scheme == "http" || uri.Scheme == "https"))
            {
                using (var httpClient = new HttpClient())
                {
                    using (var httpMessage = await httpClient.GetAsync(uri))
                    {
                        using var stream = await httpMessage.Content.ReadAsStreamAsync();
                        _frames = await LoadFramesAsync(stream.AsRandomAccessStream());
                    }
                }
            }
            else
            {
                var file = await FileHelper.GetStorageFile(uri);
                using (var stream = await file.OpenReadAsync())
                {
                    _frames = await LoadFramesAsync(stream);
                }
            }

            this.ImageOpened?.Invoke(this, new RoutedEventArgs());
        }
        catch (Exception ex)
        {
            this.ImageFailed?.Invoke(this, ex);
            System.Diagnostics.Debug.WriteLine("ProcessSourceAsync. {0}", ex.Message);
        }
    }

    private async Task<List<BitmapFrame>> LoadFramesAsync(IRandomAccessStream stream)
    {
        var frames = new List<BitmapFrame>();
        var decoder = await BitmapDecoder.CreateAsync(stream);

        uint count = decoder.FrameCount;
        for (uint n = 0; n < count; n++)
        {
            var frame = await decoder.GetFrameAsync(n);
            frames.Add(frame);
        }

        return frames;
    }
}
