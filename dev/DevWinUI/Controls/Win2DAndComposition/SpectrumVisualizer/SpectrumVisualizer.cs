//https://github.com/Johnwikix/SpectrumVisualization

using Windows.Media.Control;
using Windows.Storage.Streams;

namespace DevWinUI;

public partial class SpectrumVisualizer : Control
{
    private const string PART_SpectrumCanvasControl = "PART_SpectrumCanvasControl";
    private CanvasAnimatedControl spectrumCanvasControl;

    private GlobalSystemMediaTransportControlsSessionManager _sessionManager;
    private GlobalSystemMediaTransportControlsSession _currentSession;
    private GlobalSystemMediaTransportControlsSessionMediaProperties _mediaProperties;
    private CanvasDevice _device;
    private CanvasBitmap _albumArtBitmap;
    private CanvasTextFormat _titleTextFormat;
    private CanvasTextFormat _artistTextFormat;

    private IRandomAccessStreamWithContentType _thumbnail;
    private ISpectrumAnalyzer _analyzer;
    private SpectrumType spectrumMode = SpectrumType.Round;
    private SpectrumColorType colorType = SpectrumColorType.GradientLoop;

    private float[] _currentSpectrum;
    private float[] _smoothedSpectrum;
    private readonly int _barCount = 128;
    private bool _disposed = false;
    private float _rotationOffset = 0f;
    private int _middleNum = 0;
    private bool _isMiddleIncreasing = true;
    private bool _showTitle = true;
    private bool _showArtist = true;
    private bool _showAlbumArt = true;

    private string _title;
    private string _artist;
    private float _smoothAverage = 0f;
    private float _currentAverage = 0f;
    private float _centerX = 0f;
    private float _centerY = 0f;
    
    private float rotationSpeed = 10.0f;
    private float coverOpacity = 1.0f;
    private float spectrumOpacity = 1.0f;
    private float fontOpacity = 1.0f;
    private float smoothingFactor = 0.95f;
    private float sensitivity = 20.0f;
    public Func<float, int, Color> CustomColorProvider { get; set; }
    public SpectrumVisualizer()
    {
        DefaultStyleKey = typeof(SpectrumVisualizer);

        _currentSpectrum = new float[_barCount];
        _smoothedSpectrum = new float[_barCount];
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        spectrumCanvasControl = GetTemplateChild(PART_SpectrumCanvasControl) as CanvasAnimatedControl;
        spectrumCanvasControl.Draw -= OnDraw;
        spectrumCanvasControl.Draw += OnDraw;
        spectrumCanvasControl.Update -= OnUpdate;
        spectrumCanvasControl.Update += OnUpdate;

        InitializeText();
        _ = InitializeSMTCAsync();
    }

    private void InitializeText()
    {
        _titleTextFormat = new()
        {
            FontSize = 18f,
            FontWeight = TitleFontWeight,
            HorizontalAlignment = TitleHorizontalAlignment,
            WordWrapping = TitleWordWrapping,
            TrimmingSign = TitleTrimmingSign,
            TrimmingGranularity = TitleTrimmingGranularity,
        };
        _artistTextFormat = new()
        {
            FontSize = 16f,
            FontWeight = ArtistFontWeight,
            HorizontalAlignment = ArtistHorizontalAlignment,
            WordWrapping = ArtistWordWrapping,
            TrimmingSign = ArtistTrimmingSign,
            TrimmingGranularity = ArtistTrimmingGranularity,
        };
    }

    public async Task InitializeSMTCAsync()
    {
        try
        {
            _sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            if (_sessionManager != null)
            {
                _sessionManager.SessionsChanged -= OnSessionsChanged;
                _sessionManager.SessionsChanged += OnSessionsChanged;
                await UpdateCurrentSession();
            }
        }
        catch (Exception)
        {
        }
    }

    private async void OnSessionsChanged(GlobalSystemMediaTransportControlsSessionManager sender, SessionsChangedEventArgs args)
    {
        await UpdateCurrentSession();
    }

    private async Task UpdateCurrentSession()
    {
        try
        {
            if (_currentSession != null)
            {
                _currentSession.MediaPropertiesChanged -= OnMediaPropertiesChanged;
                _currentSession.PlaybackInfoChanged -= OnPlaybackInfoChanged;
            }
            var sessions = _sessionManager.GetSessions();
            _currentSession = sessions.Count > 0 ? sessions[0] : null;
            if (_currentSession != null)
            {
                _currentSession.MediaPropertiesChanged += OnMediaPropertiesChanged;
                _currentSession.PlaybackInfoChanged += OnPlaybackInfoChanged;
                await GetCurrentMediaInfo();
            }
        }
        catch (Exception)
        {
        }
    }

    private async void OnMediaPropertiesChanged(GlobalSystemMediaTransportControlsSession sender, MediaPropertiesChangedEventArgs args)
    {
        await GetCurrentMediaInfo();
    }

    private async void OnPlaybackInfoChanged(GlobalSystemMediaTransportControlsSession sender, PlaybackInfoChangedEventArgs args)
    {
        await GetCurrentMediaInfo();
    }

    public async Task GetCurrentMediaInfo()
    {
        if (_currentSession == null)
        {
            _albumArtBitmap = null;
            return;
        }
        try
        {
            _mediaProperties = await _currentSession.TryGetMediaPropertiesAsync();
            _title = _mediaProperties?.Title;
            _artist = _mediaProperties?.Artist;

            if (_mediaProperties != null && _mediaProperties.Thumbnail != null)
            {
                DispatcherQueue.TryEnqueue(async () => {
                    try
                    {
                        _thumbnail = await _mediaProperties.Thumbnail.OpenReadAsync();
                        _albumArtBitmap = await CanvasBitmap.LoadAsync(_device, _thumbnail);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            _thumbnail = await _mediaProperties.Thumbnail.OpenReadAsync();
                            _albumArtBitmap = await CanvasBitmap.LoadAsync(_device, _thumbnail);
                        }
                        catch (Exception)
                        {
                        }
                    }
                });
            }
            else
            {
                _albumArtBitmap = null;
            }
        }
        catch (Exception)
        {
            _albumArtBitmap = null;
        }
    }


    private void OnSpectrumDataUpdated(float[] spectrumData)
    {
        for (int i = 0; i < _barCount; i++)
        {
            int index = (int)((float)i / _barCount * spectrumData.Length);
            if (index < spectrumData.Length)
            {
                _currentSpectrum[i] = spectrumData[index] * 250f * sensitivity;
            }
        }
    }

    private void OnDraw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
    {
        try
        {
            _device = sender.Device;
            var session = args.DrawingSession;
            var size = sender.Size;
            _currentAverage = _currentSpectrum.Average();
            _smoothAverage = _smoothedSpectrum.Average();
            if (_currentAverage > 0)
            {
                _rotationOffset += 0.0001f * rotationSpeed;
                if (_rotationOffset >= 2 * (float)Math.PI)
                {
                    _rotationOffset = 0f;
                }
                if (_isMiddleIncreasing)
                {
                    if (_middleNum >= 255)
                    {
                        _isMiddleIncreasing = false;
                    }
                    else
                    {
                        _middleNum += 1;
                    }
                }
                else
                {
                    if (_middleNum <= 0)
                    {
                        _isMiddleIncreasing = true;
                    }
                    else
                    {
                        _middleNum -= 1;
                    }
                }
                _centerX = (float)size.Width * 0.5f;
                _centerY = (float)size.Height * 0.5f;
            }

            switch (spectrumMode)
            {
                case SpectrumType.Plain:
                    DrawPlainSpectrum(session, size);
                    break;
                case SpectrumType.Round:
                    DrawRoundSpectrum(session, size);
                    DrawAlbumArt(session, size);
                    DrawTitleAndArtist(session, size);
                    break;
                case SpectrumType.Wave:
                    DrawWaveform(session, size);
                    break;
            }

        }
        catch (Exception) { }
    }

    private void DrawPlainSpectrum(CanvasDrawingSession session, Windows.Foundation.Size size)
    {
        if (_smoothedSpectrum == null) return;

        float barWidth = (float)size.Width / _barCount;
        float maxHeight = (float)size.Height * 0.3f;

        for (int i = 0; i < _barCount; i++)
        {
            float x = i * barWidth;
            float height = Math.Max(Math.Min(_smoothedSpectrum[i], maxHeight), 0);
            float y = (float)size.Height / 2 - height;

            var color = GetSpectrumColor(height / maxHeight, i);

            var rectUp = new Windows.Foundation.Rect(
                x + 1, y,
                barWidth - 2, height);

            var rectDown = new Windows.Foundation.Rect(
                x + 1, (float)size.Height / 2,
                barWidth - 2, height);

            session.FillRectangle(rectUp, color);
            session.FillRectangle(rectDown, color);

            if (height > 10)
            {
                var glowRectUp = new Windows.Foundation.Rect(
                    x, y - 5,
                    barWidth, height + 10);
                var glowRectDown = new Windows.Foundation.Rect(
                    x, (float)size.Height / 2 - 5,
                    barWidth, height + 10);

                var glowColor = Color.FromArgb((byte)(32 * spectrumOpacity), color.R, color.G, color.B);
                session.FillRectangle(glowRectUp, glowColor);
                session.FillRectangle(glowRectDown, glowColor);
            }
        }
    }

    private void DrawRoundSpectrum(CanvasDrawingSession session, Windows.Foundation.Size size)
    {
        if (_smoothedSpectrum == null) return;
        float baseRadius = Math.Min(_centerX, _centerY) * 0.5f;
        float angleStep = 2 * (float)Math.PI / _barCount;
        float angleOffset = 0.01f;
        for (int i = 0; i < _barCount; i++)
        {
            float height = Math.Max(Math.Min(_smoothedSpectrum[i] * 0.02f, 0.8f), 0);
            float currentRadius = baseRadius + _smoothAverage + (height * baseRadius);

            float startAngle = i * angleStep + angleOffset - _rotationOffset;
            float endAngle = (i + 1) * angleStep - angleOffset - _rotationOffset;
            var color = GetSpectrumColor(height, i);

            var polygonPoints = new List<Vector2>();

            polygonPoints.Add(new Vector2(
                _centerX + baseRadius * (float)Math.Cos(startAngle),
                _centerY + baseRadius * (float)Math.Sin(startAngle)));

            polygonPoints.Add(new Vector2(
                _centerX + baseRadius * (float)Math.Cos(endAngle),
                _centerY + baseRadius * (float)Math.Sin(endAngle)));

            polygonPoints.Add(new Vector2(
                _centerX + currentRadius * (float)Math.Cos(endAngle),
                _centerY + currentRadius * (float)Math.Sin(endAngle)));

            polygonPoints.Add(new Vector2(
                _centerX + currentRadius * (float)Math.Cos(startAngle),
                _centerY + currentRadius * (float)Math.Sin(startAngle)));

            session.FillGeometry(CanvasGeometry.CreatePolygon(session, polygonPoints.ToArray()), color);

            if (height > 0.05f)
            {
                var glowColor = Color.FromArgb((byte)(32 * spectrumOpacity), color.R, color.G, color.B);
                float glowRadius = currentRadius + 10;
                var glowPoints = new List<Vector2>();

                glowPoints.Add(new Vector2(
                    _centerX + currentRadius * (float)Math.Cos(startAngle),
                    _centerY + currentRadius * (float)Math.Sin(startAngle)));

                glowPoints.Add(new Vector2(
                    _centerX + currentRadius * (float)Math.Cos(endAngle),
                    _centerY + currentRadius * (float)Math.Sin(endAngle)));

                glowPoints.Add(new Vector2(
                    _centerX + glowRadius * (float)Math.Cos(endAngle),
                    _centerY + glowRadius * (float)Math.Sin(endAngle)));

                glowPoints.Add(new Vector2(
                    _centerX + glowRadius * (float)Math.Cos(startAngle),
                    _centerY + glowRadius * (float)Math.Sin(startAngle)));

                session.FillGeometry(CanvasGeometry.CreatePolygon(session, glowPoints.ToArray()), glowColor);
            }
        }
    }

    private void OnUpdate(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedUpdateEventArgs args)
    {
        if (_currentSpectrum == null) return;

        for (int i = 0; i < _barCount; i++)
        {
            _smoothedSpectrum[i] = _smoothedSpectrum[i] * smoothingFactor +
                                 _currentSpectrum[i] * (1 - smoothingFactor);
        }
    }

    private void DrawWaveform(CanvasDrawingSession session, Windows.Foundation.Size size)
    {
        if (_smoothedSpectrum == null) return;
        var points = new Vector2[_barCount];
        float centerX = (float)size.Width * 0.5f;
        float centerY = (float)size.Height * 0.5f;
        Vector2 center = new Vector2(centerX, centerY);

        float baseRadius = Math.Min(centerX, centerY) * 0.6f;

        for (int i = 0; i < _barCount; i++)
        {
            float angle = (float)i / (_barCount - 1) * 2 * (float)Math.PI - _rotationOffset;
            float radius = baseRadius + _smoothedSpectrum.Average() * 10 + _smoothedSpectrum[i] * 0.5f;

            float x = centerX + radius * (float)Math.Cos(angle);
            float y = centerY + radius * (float)Math.Sin(angle);
            points[i] = new Vector2(x, y);
        }

        for (int i = 0; i < points.Length - 1; i++)
        {
            session.DrawLine(points[i], points[i + 1], Color.FromArgb(128, 0, 255, 200), 2f);
        }

        if (_barCount > 1)
        {
            session.DrawLine(points[points.Length - 1], points[0], Color.FromArgb(128, 0, 255, 200), 2f);
        }
    }

    private void DrawAlbumArt(CanvasDrawingSession session, Windows.Foundation.Size size)
    {
        try
        {
            if (_albumArtBitmap == null || !_showAlbumArt)
            {
                return;
            }
            float baseRadius = Math.Min(_centerX, _centerY) * 0.5f;
            var circleGeometry = CanvasGeometry.CreateCircle(session, _centerX, _centerY, baseRadius);

            float imageAspectRatio = (float)_albumArtBitmap.SizeInPixels.Width / _albumArtBitmap.SizeInPixels.Height;
            float targetWidth = baseRadius * 2;
            float targetHeight = baseRadius * 2;

            float drawX, drawY, drawWidth, drawHeight;

            if (imageAspectRatio > 1)
            {
                drawHeight = targetHeight;
                drawWidth = drawHeight * imageAspectRatio;
            }
            else
            {
                drawWidth = targetWidth;
                drawHeight = drawWidth / imageAspectRatio;
            }
            drawX = _centerX - drawWidth / 2;
            drawY = _centerY - drawHeight / 2;
            session.Transform = Matrix3x2.CreateRotation(-_rotationOffset, new Vector2(_centerX, _centerY)) * session.Transform;
            using (session.CreateLayer(1.0f, circleGeometry))
            {
                session.DrawImage(
                    _albumArtBitmap,
                    new Rect(drawX, drawY, drawWidth, drawHeight),
                    new Rect(0, 0, _albumArtBitmap.SizeInPixels.Width, _albumArtBitmap.SizeInPixels.Height),
                    coverOpacity,
                    CanvasImageInterpolation.HighQualityCubic);
            }
        }
        catch (Exception)
        {
        }
    }

    private void DrawTitleAndArtist(CanvasDrawingSession session, Windows.Foundation.Size size)
    {
        if (string.IsNullOrEmpty(_title) && string.IsNullOrEmpty(_artist)) return;

        float maxTextWidth = Math.Min((float)size.Width, (float)size.Height) * 0.4f;
        float baseFontSize = 18f;
        float newFontSize = baseFontSize * (maxTextWidth / 200f);
        float centerX = _centerX - maxTextWidth / 2;
        float centerY = _centerY - newFontSize;
        _titleTextFormat.FontSize = newFontSize;
        _artistTextFormat.FontSize = newFontSize * 0.9f;

        CanvasTextLayout titleLayout = new(_device, _title ?? string.Empty, _titleTextFormat, maxTextWidth, 20);
        CanvasTextLayout artistLayout = new(_device, _artist ?? string.Empty, _artistTextFormat, maxTextWidth, 16);

        if (_showTitle)
        {
            session.DrawTextLayout(titleLayout, new Vector2(centerX, centerY), Color.FromArgb((byte)(255 * fontOpacity), 255, 255, 255));
        }

        if (_showArtist)
        {
            session.DrawTextLayout(artistLayout, new Vector2(centerX, centerY + (float)titleLayout.LayoutBounds.Height), Color.FromArgb((byte)(255 * fontOpacity), 255, 255, 255));
        }
    }

    private Color GetSpectrumColorLoop(float intensity, int i = 0)
    {
        float coe = 2 * 256 / _barCount;
        return Color.FromArgb((byte)(255 * spectrumOpacity), (byte)Math.Abs(i * coe - 255), (byte)_middleNum, (byte)Math.Abs(255 - i * coe));
    }

    private Color GetSpectrumIntensityColor(float intensity)
    {
        if (intensity < 0.1f)
            return Color.FromArgb((byte)(255 * spectrumOpacity), 0, 100, 255);
        else if (intensity < 0.2f)
            return Color.FromArgb((byte)(255 * spectrumOpacity), 0, 255, 200);
        else if (intensity < 0.3f)
            return Color.FromArgb((byte)(255 * spectrumOpacity), 100, 255, 0);
        else if (intensity < 0.4f)
            return Color.FromArgb((byte)(255 * spectrumOpacity), 255, 200, 0);
        else
            return Color.FromArgb((byte)(255 * spectrumOpacity), 255, 100, 0);
    }
    private Color GetSpectrumColor(float intensity, int i = 0)
    {
        switch (colorType)
        {
            case SpectrumColorType.GradientLoop:
                return GetSpectrumColorLoop(intensity, i);

            case SpectrumColorType.Intensity:
                return GetSpectrumIntensityColor(intensity);

            case SpectrumColorType.Custom:
                return CustomColorProvider != null
                    ? CustomColorProvider(intensity, i)
                    : GetSpectrumColorLoop(intensity, i);
            default:
                return GetSpectrumColorLoop(intensity, i);
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _analyzer?.Dispose();
            _disposed = true;
        }
    }
}
