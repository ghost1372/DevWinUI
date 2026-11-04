//https://github.com/Johnwikix/SpectrumVisualization

using Windows.Media.Control;

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

    private ISpectrumAnalyzer _analyzer;
    private SpectrumType spectrumMode = SpectrumType.Round;
    private SpectrumColorType colorType = SpectrumColorType.GradientLoop;

    private float[] _currentSpectrum;
    private float[] _smoothedSpectrum;
    private readonly Vector2[] _wavePoints;
    private readonly Vector2[] _quadPoints = new Vector2[4];
    private readonly int _barCount = 128;
    private bool _disposed = false;
    private int _mediaVersion = 0;
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
        _wavePoints = new Vector2[_barCount];
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (spectrumCanvasControl != null)
        {
            spectrumCanvasControl.Draw -= OnDraw;
            spectrumCanvasControl.Update -= OnUpdate;
            spectrumCanvasControl.CreateResources -= OnCreateResources;
        }

        spectrumCanvasControl = GetTemplateChild(PART_SpectrumCanvasControl) as CanvasAnimatedControl;
        if (spectrumCanvasControl == null) return;

        spectrumCanvasControl.Draw -= OnDraw;
        spectrumCanvasControl.Draw += OnDraw;
        spectrumCanvasControl.Update -= OnUpdate;
        spectrumCanvasControl.Update += OnUpdate;
        spectrumCanvasControl.CreateResources -= OnCreateResources;
        spectrumCanvasControl.CreateResources += OnCreateResources;

        InitializeText();
        if (_sessionManager == null)
        {
            _ = InitializeSMTCAsync();
        }
    }

    private void OnCreateResources(CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
    {
        _device = sender.Device;
        if (_currentSession != null)
        {
            _ = GetCurrentMediaInfo();
        }
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
            if (_disposed || _sessionManager == null) return;

            if (_currentSession != null)
            {
                _currentSession.MediaPropertiesChanged -= OnMediaPropertiesChanged;
            }
            var sessions = _sessionManager.GetSessions();
            _currentSession = sessions.Count > 0 ? sessions[0] : null;
            if (_currentSession != null)
            {
                _currentSession.MediaPropertiesChanged += OnMediaPropertiesChanged;
            }
            await GetCurrentMediaInfo();
        }
        catch (Exception)
        {
        }
    }

    private async void OnMediaPropertiesChanged(GlobalSystemMediaTransportControlsSession sender, MediaPropertiesChangedEventArgs args)
    {
        await GetCurrentMediaInfo();
    }

    public async Task GetCurrentMediaInfo()
    {
        var version = Interlocked.Increment(ref _mediaVersion);
        var session = _currentSession;
        if (session == null)
        {
            _title = null;
            _artist = null;
            SetAlbumArt(null);
            return;
        }
        try
        {
            var mediaProperties = await session.TryGetMediaPropertiesAsync();
            if (version != _mediaVersion) return;

            _mediaProperties = mediaProperties;
            _title = mediaProperties?.Title;
            _artist = mediaProperties?.Artist;

            var thumbnail = mediaProperties?.Thumbnail;
            var device = _device;
            if (thumbnail == null || device == null)
            {
                SetAlbumArt(null);
                return;
            }

            for (int attempt = 0; attempt < 2; attempt++)
            {
                try
                {
                    using var stream = await thumbnail.OpenReadAsync();
                    var bitmap = await CanvasBitmap.LoadAsync(device, stream);
                    if (version != _mediaVersion)
                    {
                        bitmap.Dispose();
                        return;
                    }
                    SetAlbumArt(bitmap);
                    return;
                }
                catch (Exception)
                {
                }
            }
        }
        catch (Exception)
        {
            if (version == _mediaVersion)
            {
                SetAlbumArt(null);
            }
        }
    }

    private void SetAlbumArt(CanvasBitmap bitmap)
    {
        if (_disposed)
        {
            bitmap?.Dispose();
            bitmap = null;
        }
        Interlocked.Exchange(ref _albumArtBitmap, bitmap)?.Dispose();
    }

    private void OnSpectrumDataUpdated(float[] spectrumData)
    {
        if (spectrumData == null || spectrumData.Length == 0) return;

        float scale = 250f * sensitivity;
        for (int i = 0; i < _barCount; i++)
        {
            int index = (int)((float)i / _barCount * spectrumData.Length);
            _currentSpectrum[i] = spectrumData[index] * scale;
        }
    }

    private void OnDraw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
    {
        try
        {
            var session = args.DrawingSession;
            var size = sender.Size;
            float currentSum = 0f;
            float smoothSum = 0f;
            for (int i = 0; i < _barCount; i++)
            {
                currentSum += _currentSpectrum[i];
                smoothSum += _smoothedSpectrum[i];
            }
            _currentAverage = currentSum / _barCount;
            _smoothAverage = smoothSum / _barCount;
            _centerX = (float)size.Width * 0.5f;
            _centerY = (float)size.Height * 0.5f;
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
        float halfHeight = (float)size.Height / 2;
        float maxHeight = (float)size.Height * 0.3f;
        float rectWidth = Math.Max(barWidth - 2, 0);

        for (int i = 0; i < _barCount; i++)
        {
            float x = i * barWidth;
            float height = Math.Max(Math.Min(_smoothedSpectrum[i], maxHeight), 0);
            float y = halfHeight - height;

            var color = GetSpectrumColor(height / maxHeight, i);

            var rectUp = new Windows.Foundation.Rect(
                x + 1, y,
                rectWidth, height);

            var rectDown = new Windows.Foundation.Rect(
                x + 1, halfHeight,
                rectWidth, height);

            session.FillRectangle(rectUp, color);
            session.FillRectangle(rectDown, color);

            if (height > 10)
            {
                var glowRectUp = new Windows.Foundation.Rect(
                    x, y - 5,
                    barWidth, height + 10);
                var glowRectDown = new Windows.Foundation.Rect(
                    x, halfHeight - 5,
                    barWidth, height + 10);

                var glowColor = Color.FromArgb((byte)(32 * spectrumOpacity), color.R, color.G, color.B);
                session.FillRectangle(glowRectUp, glowColor);
                session.FillRectangle(glowRectDown, glowColor);
            }
        }
    }

    private void FillRadialQuad(CanvasDrawingSession session, float innerRadius, float outerRadius, float cosStart, float sinStart, float cosEnd, float sinEnd, Color color)
    {
        _quadPoints[0] = new Vector2(_centerX + innerRadius * cosStart, _centerY + innerRadius * sinStart);
        _quadPoints[1] = new Vector2(_centerX + innerRadius * cosEnd, _centerY + innerRadius * sinEnd);
        _quadPoints[2] = new Vector2(_centerX + outerRadius * cosEnd, _centerY + outerRadius * sinEnd);
        _quadPoints[3] = new Vector2(_centerX + outerRadius * cosStart, _centerY + outerRadius * sinStart);

        using var geometry = CanvasGeometry.CreatePolygon(session, _quadPoints);
        session.FillGeometry(geometry, color);
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
            float cosStart = MathF.Cos(startAngle);
            float sinStart = MathF.Sin(startAngle);
            float cosEnd = MathF.Cos(endAngle);
            float sinEnd = MathF.Sin(endAngle);
            var color = GetSpectrumColor(height, i);

            FillRadialQuad(session, baseRadius, currentRadius, cosStart, sinStart, cosEnd, sinEnd, color);

            if (height > 0.05f)
            {
                var glowColor = Color.FromArgb((byte)(32 * spectrumOpacity), color.R, color.G, color.B);
                FillRadialQuad(session, currentRadius, currentRadius + 10, cosStart, sinStart, cosEnd, sinEnd, glowColor);
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
        var points = _wavePoints;
        float centerX = _centerX;
        float centerY = _centerY;

        float baseRadius = Math.Min(centerX, centerY) * 0.6f + _smoothAverage * 10;

        for (int i = 0; i < _barCount; i++)
        {
            float angle = (float)i / (_barCount - 1) * 2 * (float)Math.PI - _rotationOffset;
            float radius = baseRadius + _smoothedSpectrum[i] * 0.5f;

            float x = centerX + radius * MathF.Cos(angle);
            float y = centerY + radius * MathF.Sin(angle);
            points[i] = new Vector2(x, y);
        }

        var lineColor = Color.FromArgb(128, 0, 255, 200);
        for (int i = 0; i < points.Length - 1; i++)
        {
            session.DrawLine(points[i], points[i + 1], lineColor, 2f);
        }

        if (_barCount > 1)
        {
            session.DrawLine(points[points.Length - 1], points[0], lineColor, 2f);
        }
    }

    private void DrawAlbumArt(CanvasDrawingSession session, Windows.Foundation.Size size)
    {
        var albumArt = _albumArtBitmap;
        if (albumArt == null || !_showAlbumArt)
        {
            return;
        }
        var originalTransform = session.Transform;
        try
        {
            float baseRadius = Math.Min(_centerX, _centerY) * 0.5f;
            using var circleGeometry = CanvasGeometry.CreateCircle(session, _centerX, _centerY, baseRadius);

            var pixelSize = albumArt.SizeInPixels;
            float imageAspectRatio = (float)pixelSize.Width / pixelSize.Height;
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
            session.Transform = Matrix3x2.CreateRotation(-_rotationOffset, new Vector2(_centerX, _centerY)) * originalTransform;
            using (session.CreateLayer(1.0f, circleGeometry))
            {
                session.DrawImage(
                    albumArt,
                    new Rect(drawX, drawY, drawWidth, drawHeight),
                    new Rect(0, 0, pixelSize.Width, pixelSize.Height),
                    coverOpacity,
                    CanvasImageInterpolation.HighQualityCubic);
            }
        }
        catch (Exception)
        {
        }
        finally
        {
            session.Transform = originalTransform;
        }
    }

    private void DrawTitleAndArtist(CanvasDrawingSession session, Windows.Foundation.Size size)
    {
        if (string.IsNullOrEmpty(_title) && string.IsNullOrEmpty(_artist)) return;
        if (!_showTitle && !_showArtist) return;

        float maxTextWidth = Math.Min((float)size.Width, (float)size.Height) * 0.4f;
        float baseFontSize = 18f;
        float newFontSize = baseFontSize * (maxTextWidth / 200f);
        float centerX = _centerX - maxTextWidth / 2;
        float centerY = _centerY - newFontSize;
        _titleTextFormat.FontSize = newFontSize;
        _artistTextFormat.FontSize = newFontSize * 0.9f;

        using CanvasTextLayout titleLayout = new(session, _title ?? string.Empty, _titleTextFormat, maxTextWidth, 20);
        using CanvasTextLayout artistLayout = new(session, _artist ?? string.Empty, _artistTextFormat, maxTextWidth, 16);
        var fontColor = Color.FromArgb((byte)(255 * fontOpacity), 255, 255, 255);

        if (_showTitle)
        {
            session.DrawTextLayout(titleLayout, new Vector2(centerX, centerY), fontColor);
        }

        if (_showArtist)
        {
            session.DrawTextLayout(artistLayout, new Vector2(centerX, centerY + (float)titleLayout.LayoutBounds.Height), fontColor);
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
            _disposed = true;

            if (_analyzer != null)
            {
                _analyzer.SpectrumDataUpdated -= OnSpectrumDataUpdated;
                _analyzer.Dispose();
            }

            if (_sessionManager != null)
            {
                _sessionManager.SessionsChanged -= OnSessionsChanged;
            }

            if (_currentSession != null)
            {
                _currentSession.MediaPropertiesChanged -= OnMediaPropertiesChanged;
                _currentSession = null;
            }

            if (spectrumCanvasControl != null)
            {
                spectrumCanvasControl.Draw -= OnDraw;
                spectrumCanvasControl.Update -= OnUpdate;
                spectrumCanvasControl.CreateResources -= OnCreateResources;
                spectrumCanvasControl.RemoveFromVisualTree();
                spectrumCanvasControl = null;
            }

            SetAlbumArt(null);
        }
    }
}
